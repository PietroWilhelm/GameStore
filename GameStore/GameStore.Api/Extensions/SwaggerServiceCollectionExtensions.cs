using System.Reflection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GameStore.API.Extensions;

public static class SwaggerServiceCollectionExtensions
{
    public static IServiceCollection AddGameStoreSwagger(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Um SwaggerDoc por versão de API é registrado por ConfigureSwaggerOptions, que depende
        // de IApiVersionDescriptionProvider (adicionado por AddApiVersioning().AddApiExplorer()).
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        services.AddSwaggerGen(options =>
        {
            var order = new List<string>
            {
                "Game",
                "Studio",
                "Genre",
                "Customer",
                "Order"
            };

            options.TagActionsBy(api =>
            {
                var tag = api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] ?? "Other";
                return [tag];
            });

            options.OrderActionsBy(api =>
            {
                var controller = api.ActionDescriptor.RouteValues["controller"] ?? "";
                var index = order.IndexOf(controller);
                return index == -1
                    ? $"{order.Count}_{controller}"
                    : $"{index:D2}_{controller}";
            });

            var xml = Path.Combine(
                AppContext.BaseDirectory,
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"
            );

            if (File.Exists(xml))
                options.IncludeXmlComments(xml, includeControllerXmlComments: true);
        });

        return services;
    }
}
