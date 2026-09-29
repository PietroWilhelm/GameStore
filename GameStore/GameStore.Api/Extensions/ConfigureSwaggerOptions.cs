using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GameStore.API.Extensions;

/// <summary>
/// Cria um <c>SwaggerDoc</c> para cada versão de API descoberta pelo
/// <see cref="IApiVersionDescriptionProvider"/> (CP5: v1.0 obsoleta e v2.0 atual do GameController;
/// os demais recursos são [ApiVersionNeutral] e aparecem em todos os grupos automaticamente).
/// Registrado como <see cref="IConfigureOptions{SwaggerGenOptions}"/> para que o provider,
/// que depende do pipeline de versionamento, só seja resolvido depois que todos os serviços
/// tiverem sido registrados.
/// </summary>
public sealed class ConfigureSwaggerOptions(
    IApiVersionDescriptionProvider provider,
    IConfiguration configuration) : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }
    }

    private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var info = new OpenApiInfo
        {
            Title = configuration.GetSection("Swagger:Title").Value ?? "GameStore API",
            Version = description.ApiVersion.ToString(),
            Description = configuration.GetSection("Swagger:Description").Value
                           ?? "API para gerenciamento de catálogo de jogos."
        };

        if (description.IsDeprecated)
        {
            info.Description += " ⚠️ Esta versão da API está obsoleta (deprecated) — utilize a versão mais recente.";
        }

        return info;
    }
}
