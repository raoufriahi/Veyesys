﻿/*
 * Copyright (c) 2022-2023 Veyesys
 *
 * The computer program contained herein contains proprietary
 * information which is the property of Veyesys.
 * The program may be used and/or copied only with the written
 * permission of Veyesys or in accordance with the
 * terms and conditions stipulated in the agreement/contract under
 * which the programs have been supplied.
 */

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Veyesys.Core.Infrastructure;
using Veyesys.Web.Framework.Infrastructure.Extensions;

namespace Veyesys.Web.Framework.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring WebMarkupMin services on application startup
    /// </summary>
    public class VeWebMarkupMinStartup : IVeStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //add WebMarkupMin services to the services container
            services.AddNopWebMarkupMin();
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            //use WebMarkupMin
            application.UseVeWebMarkupMin();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 300; //Ensure that "UseNopWebMarkupMin" method is invoked before "UseRouting". Otherwise, HTML minification won't work
    }
}