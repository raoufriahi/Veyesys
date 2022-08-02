﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using FluentMigrator.Builders.Alter.Table;
using FluentMigrator.Builders.Create;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Infrastructure.Extensions;
using FluentMigrator.Model;
using LinqToDB.Mapping;
using Veyesys.Core;
using Veyesys.Core.Infrastructure;
using Veyesys.Data.Mapping;
using Veyesys.Data.Mapping.Builders;

namespace Veyesys.Data.Extensions
{
    /// <summary>
    /// FluentMigrator extensions
    /// </summary>
    public static class FluentMigratorExtensions
    {
        #region  Utils

        private static Dictionary<Type, Action<ICreateTableColumnAsTypeSyntax>> TypeMapping { get; } = new Dictionary<Type, Action<ICreateTableColumnAsTypeSyntax>>
        {
            [typeof(int)] = c => c.AsInt32(),
            [typeof(long)] = c => c.AsInt64(),
            [typeof(string)] = c => c.AsString(int.MaxValue).Nullable(),
            [typeof(bool)] = c => c.AsBoolean(),
            [typeof(decimal)] = c => c.AsDecimal(18, 4),
            [typeof(DateTime)] = c => c.AsDateTime2(),
            [typeof(byte[])] = c => c.AsBinary(int.MaxValue),
            [typeof(Guid)] = c => c.AsGuid()
        };

        private static void DefineByOwnType(string columnName, Type propType, CreateTableExpressionBuilder create, bool canBeNullable = false)
        {
            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentException("The column name cannot be empty");

            if (propType == typeof(string) || propType.FindInterfaces((t, o) => t.FullName?.Equals(o.ToString(), StringComparison.InvariantCultureIgnoreCase) ?? false, "System.Collections.IEnumerable").Length > 0)
                canBeNullable = true;

            var column = create.WithColumn(columnName);

            TypeMapping[propType](column);

            if (canBeNullable)
                create.Nullable();
        }

        #endregion

        /// <summary>
        /// Specifies a foreign key
        /// </summary>
        /// <param name="column">The foreign key column</param>
        /// <param name="primaryTableName">The primary table name</param>
        /// <param name="primaryColumnName">The primary tables column name</param>
        /// <param name="onDelete">Behavior for DELETEs</param>
        /// <typeparam name="TPrimary"></typeparam>
        /// <returns>Set column options or create a new column or set a foreign key cascade rule</returns>
        public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax ForeignKey<TPrimary>(this ICreateTableColumnOptionOrWithColumnSyntax column, string primaryTableName = null, string primaryColumnName = null, Rule onDelete = Rule.Cascade) where TPrimary : BaseEntity
        {
            if (string.IsNullOrEmpty(primaryTableName))
                primaryTableName = NameCompatibilityManager.GetTableName(typeof(TPrimary));

            if (string.IsNullOrEmpty(primaryColumnName))
                primaryColumnName = nameof(BaseEntity.Id);

            return column.Indexed().ForeignKey(primaryTableName, primaryColumnName).OnDelete(onDelete);
        }

        /// <summary>
        /// Specifies a foreign key
        /// </summary>
        /// <param name="column">The foreign key column</param>
        /// <param name="primaryTableName">The primary table name</param>
        /// <param name="primaryColumnName">The primary tables column name</param>
        /// <param name="onDelete">Behavior for DELETEs</param>
        /// <typeparam name="TPrimary"></typeparam>
        /// <returns>Alter/add a column with an optional foreign key</returns>
        public static IAlterTableColumnOptionOrAddColumnOrAlterColumnOrForeignKeyCascadeSyntax ForeignKey<TPrimary>(this IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax column, string primaryTableName = null, string primaryColumnName = null, Rule onDelete = Rule.Cascade) where TPrimary : BaseEntity
        {
            if (string.IsNullOrEmpty(primaryTableName))
                primaryTableName = NameCompatibilityManager.GetTableName(typeof(TPrimary));

            if (string.IsNullOrEmpty(primaryColumnName))
                primaryColumnName = nameof(BaseEntity.Id);

            return column.Indexed().ForeignKey(primaryTableName, primaryColumnName).OnDelete(onDelete);
        }

        /// <summary>
        /// Retrieves expressions into ICreateExpressionRoot
        /// </summary>
        /// <param name="expressionRoot">The root expression for a CREATE operation</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public static void TableFor<TEntity>(this ICreateExpressionRoot expressionRoot) where TEntity : BaseEntity
        {
            var type = typeof(TEntity);
            var builder = expressionRoot.Table(NameCompatibilityManager.GetTableName(type)) as CreateTableExpressionBuilder;
            builder.RetrieveTableExpressions(type);
        }

        /// <summary>
        /// Retrieves expressions for building an entity table
        /// </summary>
        /// <param name="builder">An expression builder for a FluentMigrator.Expressions.CreateTableExpression</param>
        /// <param name="type">Type of entity</param>
        public static void RetrieveTableExpressions(this CreateTableExpressionBuilder builder, Type type)
        {
            var typeFinder = Singleton<ITypeFinder>.Instance
                .FindClassesOfType(typeof(IEntityBuilder))
                .FirstOrDefault(t => t.BaseType?.GetGenericArguments().Contains(type) ?? false);

            if (typeFinder != null)
                (EngineContext.Current.ResolveUnregistered(typeFinder) as IEntityBuilder)?.MapEntity(builder);

            var expression = builder.Expression;
            if (!expression.Columns.Any(c => c.IsPrimaryKey))
            {
                var pk = new ColumnDefinition
                {
                    Name = nameof(BaseEntity.Id),
                    Type = DbType.Int32,
                    IsIdentity = true,
                    TableName = NameCompatibilityManager.GetTableName(type),
                    ModificationType = ColumnModificationType.Create,
                    IsPrimaryKey = true
                };
                expression.Columns.Insert(0, pk);
                builder.CurrentColumn = pk;
            }

            var propertiesToAutoMap = type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty)
                .Where(pi => pi.DeclaringType != typeof(BaseEntity) &&
                !pi.HasAttribute<NotMappedAttribute>() && !pi.HasAttribute<NotColumnAttribute>() &&
                !expression.Columns.Any(x => x.Name.Equals(NameCompatibilityManager.GetColumnName(type, pi.Name), StringComparison.OrdinalIgnoreCase)) &&
                TypeMapping.ContainsKey(GetTypeToMap(pi.PropertyType).propType));

            foreach (var prop in propertiesToAutoMap)
            {
                var columnName = NameCompatibilityManager.GetColumnName(type, prop.Name);
                var (propType, canBeNullable) = GetTypeToMap(prop.PropertyType);
                DefineByOwnType(columnName, propType, builder, canBeNullable);
            }
        }

        public static (Type propType, bool canBeNullable) GetTypeToMap(this Type type)
        {
            if (Nullable.GetUnderlyingType(type) is Type uType)
                return (uType, true);

            return (type, false);
        }
    }
}