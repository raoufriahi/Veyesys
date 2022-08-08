/*
 * Copyright (c) 2022-2023 Veyesys
 *
 * The computer program contained herein contains proprietary
 * information which is the property of Veyesys.
 * The program may be used and/or copied only with the written
 * permission of Veyesys or in accordance with the
 * terms and conditions stipulated in the agreement/contract under
 * which the programs have been supplied.
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Veyesys.Web.Framework.Mvc.ModelBinding.Binders
{
    /// <summary>
    /// Represents a model binder provider for binding numeric types
    /// </summary>
    public class InvariantNumberModelBinderProvider : IModelBinderProvider
    {
        #region Fields

        private static readonly HashSet<Type> _integerTypes = new()
        {
            typeof(int), typeof(long), typeof(short), typeof(sbyte),
            typeof(byte), typeof(ulong), typeof(ushort), typeof(uint), typeof(BigInteger)
        };

        private static readonly HashSet<Type> _floatingPointTypes = new()
        {
            typeof(double), typeof(decimal), typeof(float)
        };

        #endregion

        /// <summary>
        /// Creates a model binder
        /// </summary>
        /// <param name="context">Context object</param>
        /// <returns>Instance of model binder for floating-point types</returns>
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            var modelType = context.Metadata.UnderlyingOrModelType;

            if (modelType is null)
                return null;

            if (_floatingPointTypes.Contains(modelType))
                return new InvariantNumberModelBinder(NumberStyles.Float, new FloatingPointTypeModelBinderProvider().GetBinder(context));

            if (_integerTypes.Contains(modelType))
                return new InvariantNumberModelBinder(NumberStyles.Integer, new SimpleTypeModelBinderProvider().GetBinder(context));

            return null;
        }
    }
}