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

            // Desde o CP5, api.GroupName deixou de ser null: o Asp.Versioning.Mvc.ApiExplorer
            // preenche esse campo com o nome do grupo de versão ("v1"/"v2") em toda
            // ApiDescription, para separar os SwaggerDocs (ver DocInclusionPredicate abaixo).
            // Se ele fosse usado aqui como tag, todas as actions de um mesmo doc cairiam juntas
            // sob uma única seção "V1"/"V2" em vez de uma seção por recurso — por isso a tag
            // usa só o nome do controller, igual ao comportamento anterior ao CP5.
            options.TagActionsBy(api =>
            {
                var tag = api.ActionDescriptor.RouteValues["controller"] ?? "Other";
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

            // GameController expõe cada action em duas rotas (com e sem segmento de versão na
            // URL) para que api-version por query string/header funcione em "api/game" e o
            // segmento opcional funcione em "api/v{version}/game". Sem este filtro, o Swagger
            // listaria cada operação duas vezes (ex.: GET /api/v1/game e GET /api/game lado a
            // lado). Aqui só a rota sem segmento é documentada — a rota com segmento continua
            // funcionando normalmente, só não aparece duplicada na documentação.
            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                var isUrlSegmentVersionedRoute = apiDesc.RelativePath is not null &&
                    System.Text.RegularExpressions.Regex.IsMatch(
                        apiDesc.RelativePath, @"^api/v\d", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                if (isUrlSegmentVersionedRoute)
                    return false;

                return apiDesc.GroupName is null || apiDesc.GroupName == docName;
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
