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
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using MagicCode.MagicApi.MagicfulResult;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using MagicCode.MagicApi.Options;
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
            mvc.Services.AddMagicfulResult();
            return mvc;
        }
        public static IMvcCoreBuilder AddMagicfulResult(this IMvcCoreBuilder mvc)
        {
            mvc.Services.AddMagicfulResult();
            return mvc;
        }

        public static IMvcCoreBuilder AddMagicApi(this IMvcCoreBuilder builder)
        {
            builder.AddMagicApiParts().AddNewtonsoftJson();
            builder.Services.AddMagicApi();
            return builder;
        }
        public static IApplicationBuilder UseMagicApi(this IApplicationBuilder builder,Action<MagicApiOptions>? configureOptions=null)
        {
            if (configureOptions == null)
            {
                configureOptions = o =>
                {
                    o.RouteParserProvider = new DefaultMagicApiRouteParserProvider();
                };
            }

            return builder.InitMagicApi(configureOptions);
        }
        public static IServiceCollection AddMagicfulResult(this IServiceCollection services)
        {

            MagicApi.MagicfulResult = true;
            services.AddMvcFilter<MagicfulResultFilter>();
            services.AddMvcFilter<MagicfulExceptionFilter>();
            MagicApi.Envrionment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("ASPNET_ENVIRONMENT") ?? "";
            return services;
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
            services.AddHttpContextAccessor();
            services.Configure<MvcOptions>(o =>
            {
                o.Conventions.Add(new MagicApiApplicationModelConvention(services));

            });
            services.AddOptions<MagicApiOptions>();
            services.AddSwaggerGen();
            return services;
        }

        internal static IServiceCollection AddMvcFilter(this IServiceCollection services, Type filter)
        {
            services.Configure<MvcOptions>(o =>
            {
                o.Filters.Add(filter);
            });
            return services;
        }
        internal static IServiceCollection AddMvcFilter<T>(this IServiceCollection services) where T : IFilterMetadata
        {
            services.Configure<MvcOptions>(o =>
            {
                if (o.Filters.Any(f => f is T) == false)
                {
                    o.Filters.Add<T>();

                }
            });
            return services;
        }


        internal static IApplicationBuilder InitMagicApi(this IApplicationBuilder builder,Action<MagicApiOptions>? action=default)
        {
            ServiceProvider = builder.ApplicationServices; 
            Configuration = ServiceProvider.GetService<IConfiguration>(); 
            Options = Configuration?.GetSection("MagicCode:MagicApi").Get<MagicApiOptions>() ?? new MagicApiOptions() ;
            HttpContextAccessor = ServiceProvider.GetRequiredService<IHttpContextAccessor>() ?? default;
            action?.Invoke(Options);
            return builder;
        }

        public static IServiceProvider ServiceProvider { get; internal set; }
        public static IConfiguration Configuration { get; internal set; }
        internal static IHttpContextAccessor HttpContextAccessor { get; set; }
        public static HttpContext HttpContext { get { return HttpContextAccessor.HttpContext; } }
        internal static bool MagicfulResult { get; set; }
        public static string Envrionment { get; internal set; }
        public static MagicApiOptions Options { get; internal set; }

    }

}
