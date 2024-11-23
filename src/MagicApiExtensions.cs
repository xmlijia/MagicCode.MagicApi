using MagicCode.MagicApi.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MagicCode.MagicApi.Providers;
using MagicCode.MagicApi.Conventions;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
namespace MagicCode.MagicApi
{
    public static class MagicApiExtensions
    {
        public static IMvcBuilder AddMagicApi(this IMvcBuilder builder)
        {
            builder.AddMagicApiParts().AddNewtonsoftJson();
            builder.Services.AddMagicApi();
            return builder;
        }

        public static IMvcBuilder AddMagicfulResult(this IMvcBuilder mvc)
        {
            MagicApi.MagicfulResult = true;
            return mvc;
        }
        public static IMvcCoreBuilder AddMagicfulResult(this IMvcCoreBuilder mvc)
        {
            MagicApi.MagicfulResult = true;
            return mvc;
        }

        public static IMvcCoreBuilder AddMagicApi(this IMvcCoreBuilder builder)
        {
            builder.AddMagicApiParts().AddNewtonsoftJson();
            builder.Services.AddMagicApi();
            return builder;
        }
        public static IApplicationBuilder UseMagicApi(this IApplicationBuilder builder)
        {
            return builder.InitMagicApi();
        }
    }

    public static class MagicApi
    {
        internal static IEnumerable<Assembly> GetAssemblies()
        {
            var ass = AppDomain.CurrentDomain.GetAssemblies();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.ExportedTypes.Any(et => typeof(IMagicApi).IsAssignableFrom(et)));
            var _ = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.ExportedTypes.Any(et => et.GetCustomAttributes<MagicApiAttribute>().Any()));
            if (_.Any()) { assemblies = assemblies.Concat(_); }
            return assemblies.Where(a => a.ExportedTypes.Any(et => et.IsInterface == false)).Distinct();
        }

        internal static IMvcBuilder AddMagicApiParts(this IMvcBuilder mvc)
        {
            ConfigureApplicationPartManager(mvc.PartManager);


            return mvc;
        }

        internal static IMvcCoreBuilder AddMagicApiParts(this IMvcCoreBuilder mvc)
        {
            ConfigureApplicationPartManager(mvc.PartManager);


            return mvc;
        }
        internal static void ConfigureApplicationPartManager(ApplicationPartManager m)
        {
            var assemblies = GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (m.ApplicationParts.Any(ap => ((AssemblyPart)ap).Assembly == assembly) == false)
                {
                    m.ApplicationParts.Add(new AssemblyPart(assembly));
                }
            }
            var defaultFeatureProvider = m.FeatureProviders.FirstOrDefault(fp => fp is ControllerFeatureProvider);
            if (defaultFeatureProvider != null)
            {
                m.FeatureProviders.Remove(defaultFeatureProvider);
            }
            m.FeatureProviders.Add(new MagicApiControllerFeatureProvider());

        }

        internal static IServiceCollection AddMagicApi(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(o =>
            {
                o.Conventions.Add(new MagicApiApplicationModelConvention(services));
            });
            services.AddSwaggerGen();
            return services;
        }


        public static IApplicationBuilder InitMagicApi(this IApplicationBuilder builder)
        {
            ServiceProvider = builder.ApplicationServices;
            return builder;
        }

        internal static IServiceProvider ServiceProvider { get; set; }
        internal static HttpContext HttpContext { get { return ServiceProvider.GetRequiredService<IHttpContextAccessor>()?.HttpContext ?? default; } }
        internal static bool MagicfulResult { get; set; }
    }

}
