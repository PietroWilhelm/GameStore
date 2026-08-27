using System.Reflection;
using Microsoft.OpenApi;

namespace GameStore.API.Extensions;

public static class SwaggerServiceCollectionExtensions
{
    public static IServiceCollection AddGameStoreSwagger(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = configuration.GetSection("Swagger:Title").Value ?? "GameStore API",
                Version = configuration.GetSection("Swagger:Version").Value ?? "v1",
                Description = configuration.GetSection("Swagger:Description").Value
                              ?? "API para gerenciamento de catálogo de jogos."
            });

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
