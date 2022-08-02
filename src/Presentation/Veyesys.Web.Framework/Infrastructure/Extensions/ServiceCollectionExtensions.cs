using System;
using System.Linq;
using System.Net;
using Azure.Identity;
using Azure.Storage.Blobs;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Veyesys.Core;
using Veyesys.Core.Configuration;
using Veyesys.Core.Http;
using Veyesys.Core.Domain.Common;
using Veyesys.Core.Infrastructure;
using Veyesys.Core.Security;
using Veyesys.Data;
//using Veyesys.Services.Authentication;
/*using Veyesys.Services.Authentication.External;
using Veyesys.Services.Common;
using Veyesys.Services.Security;
using Veyesys.Web.Framework.Configuration;
using Veyesys.Web.Framework.Mvc.ModelBinding;
using Veyesys.Web.Framework.Mvc.ModelBinding.Binders;
using Veyesys.Web.Framework.Mvc.Routing;
using Veyesys.Web.Framework.Security.Captcha;
using Veyesys.Web.Framework.Themes;
using Veyesys.Web.Framework.Validators;*/
using StackExchange.Profiling.Storage;
using WebMarkupMin.AspNetCore6;
using WebMarkupMin.NUglify;

namespace Veyesys.Web.Framework.Infrastructure.Extensions
{
    /// <summary>
    /// Represents extensions of IServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add services to the application and configure service provider
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="builder">A builder for web applications and services</param>
        public static void ConfigureApplicationServices(this IServiceCollection services,
            WebApplicationBuilder builder)
        {
            //let the operating system decide what TLS protocol version to use
            //see https://docs.microsoft.com/dotnet/framework/network-programming/tls
            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;

            //create default file provider
            CommonHelper.DefaultFileProvider = new FileProvider(builder.Environment);

            //add accessor to HttpContext
            services.AddHttpContextAccessor();

            //initialize plugins
            var mvcCoreBuilder = services.AddMvcCore();
            var pluginConfig = new PluginConfig();
            builder.Configuration.GetSection(nameof(PluginConfig)).Bind(pluginConfig, options => options.BindNonPublicProperties = true);
            mvcCoreBuilder.PartManager.InitializePlugins(pluginConfig);

            //register type finder
            var typeFinder = new WebAppTypeFinder();
            Singleton<ITypeFinder>.Instance = typeFinder;
            services.AddSingleton<ITypeFinder>(typeFinder);

            //add configuration parameters
            var configurations = typeFinder
                .FindClassesOfType<IConfig>()
                .Select(configType => (IConfig)Activator.CreateInstance(configType))
                .ToList();
            foreach (var config in configurations)
            {
                builder.Configuration.GetSection(config.Name).Bind(config, options => options.BindNonPublicProperties = true);
            }
            var appSettings = AppSettingsHelper.SaveAppSettings(configurations, CommonHelper.DefaultFileProvider, false);
            services.AddSingleton(appSettings);

            //create engine and configure service provider
            var engine = EngineContext.Create();

            engine.ConfigureServices(services, builder.Configuration);
        }

        /// <summary>
        /// Register HttpContextAccessor
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddHttpContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        /// <summary>
        /// Adds services required for anti-forgery support
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddAntiForgery(this IServiceCollection services)
        {
            //override cookie name
            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.AntiforgeryCookie}";
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });
           
        }

        /// <summary>
        /// Adds services required for application session state
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddHttpSession(this IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.Cookie.Name = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.SessionCookie}";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });
          
        }

        /// <summary>
        /// Adds services required for themes support
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddThemes(this IServiceCollection services)
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //themes support
            services.Configure<RazorViewEngineOptions>(options =>
            {
               //raouf options.ViewLocationExpanders.Add(new ThemeableViewLocationExpander());
            });
           
        }

        /// <summary>
        /// Adds services required for distributed cache
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddDistributedCache(this IServiceCollection services)
        {
            var appSettings = Singleton<AppSettings>.Instance;
            var distributedCacheConfig = appSettings.Get<DistributedCacheConfig>();

            if (!distributedCacheConfig.Enabled)
                return;

            switch (distributedCacheConfig.DistributedCacheType)
            {
                case DistributedCacheType.Memory:
                    services.AddDistributedMemoryCache();
                    break;

                case DistributedCacheType.SqlServer:
                    services.AddDistributedSqlServerCache(options =>
                    {
                        options.ConnectionString = distributedCacheConfig.ConnectionString;
                        options.SchemaName = distributedCacheConfig.SchemaName;
                        options.TableName = distributedCacheConfig.TableName;
                    });
                    break;

                case DistributedCacheType.Redis:
                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = distributedCacheConfig.ConnectionString;
                    });
                    break;
            }
        }

        /// <summary>
        /// Adds data protection services
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddNopDataProtection(this IServiceCollection services)
        {
            
            var appSettings = Singleton<AppSettings>.Instance;
            if (appSettings.Get<AzureBlobConfig>().Enabled && appSettings.Get<AzureBlobConfig>().StoreDataProtectionKeys)
            {
                var blobServiceClient = new BlobServiceClient(appSettings.Get<AzureBlobConfig>().ConnectionString);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(appSettings.Get<AzureBlobConfig>().DataProtectionKeysContainerName);
                var blobClient = blobContainerClient.GetBlobClient(DataProtectionDefaults.AzureDataProtectionKeyFile);

                var dataProtectionBuilder = services.AddDataProtection().PersistKeysToAzureBlobStorage(blobClient);

                if (!appSettings.Get<AzureBlobConfig>().DataProtectionKeysEncryptWithVault)
                    return;

                var keyIdentifier = appSettings.Get<AzureBlobConfig>().DataProtectionKeysVaultId;
                var credentialOptions = new DefaultAzureCredentialOptions();
                var tokenCredential = new DefaultAzureCredential(credentialOptions);

                dataProtectionBuilder.ProtectKeysWithAzureKeyVault(new Uri(keyIdentifier), tokenCredential);
            }
            else
            {
                var dataProtectionKeysPath = CommonHelper.DefaultFileProvider.MapPath(DataProtectionDefaults.DataProtectionKeysPath);
                var dataProtectionKeysFolder = new System.IO.DirectoryInfo(dataProtectionKeysPath);

                //configure the data protection system to persist keys to the specified directory
                services.AddDataProtection().PersistKeysToFileSystem(dataProtectionKeysFolder);
            }
           
        }

        /// <summary>
        /// Adds authentication service
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddNopAuthentication(this IServiceCollection services)
        {
            ////set default authentication schemes
            // var authenticationBuilder = services.AddAuthentication(options =>
            //{
            //    options.DefaultChallengeScheme = NopAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultScheme = NopAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignInScheme = NopAuthenticationDefaults.ExternalAuthenticationScheme;
            //});

            ////add main cookie authentication
            //authenticationBuilder.AddCookie(NopAuthenticationDefaults.AuthenticationScheme, options =>
            //{
            //    options.Cookie.Name = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.AuthenticationCookie}";
            //    options.Cookie.HttpOnly = true;
            //    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            //    options.LoginPath = NopAuthenticationDefaults.LoginPath;
            //    options.AccessDeniedPath = NopAuthenticationDefaults.AccessDeniedPath;
            //});

            ////add external authentication
            //authenticationBuilder.AddCookie(NopAuthenticationDefaults.ExternalAuthenticationScheme, options =>
            //{
            //    options.Cookie.Name = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.ExternalAuthenticationCookie}";
            //    options.Cookie.HttpOnly = true;
            //    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            //    options.LoginPath = NopAuthenticationDefaults.LoginPath;
            //    options.AccessDeniedPath = NopAuthenticationDefaults.AccessDeniedPath;
            //});

            ////register and configure external authentication plugins now
            //var typeFinder = Singleton<ITypeFinder>.Instance;
            //var externalAuthConfigurations = typeFinder.FindClassesOfType<IExternalAuthenticationRegistrar>();
            //var externalAuthInstances = externalAuthConfigurations
            //    .Select(x => (IExternalAuthenticationRegistrar)Activator.CreateInstance(x));

            //foreach (var instance in externalAuthInstances)
            //    instance.Configure(authenticationBuilder);
            
        }

        /// <summary>
        /// Add and configure MVC for the application
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <returns>A builder for configuring MVC services</returns>
        public static IMvcBuilder AddNopMvc(this IServiceCollection services)
        {
           
            return null;
        }

        /// <summary>
        /// Register custom RedirectResultExecutor
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddNopRedirectResultExecutor(this IServiceCollection services)
        {
        }

        /// <summary>
        /// Add and configure MiniProfiler service
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddNopMiniProfiler(this IServiceCollection services)
        {
           
        }

        /// <summary>
        /// Add and configure WebMarkupMin service
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddNopWebMarkupMin(this IServiceCollection services)
        {

        }

        /// <summary>
        /// Adds WebOptimizer to the specified <see cref="IServiceCollection"/> and enables CSS and JavaScript minification.
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddNopWebOptimizer(this IServiceCollection services)
        {
           
        }

        /// <summary>
        /// Add and configure default HTTP clients
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddNopHttpClients(this IServiceCollection services)
        {

        }
    }
}