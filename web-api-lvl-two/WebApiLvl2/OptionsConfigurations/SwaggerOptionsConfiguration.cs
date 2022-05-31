using Microsoft.Extensions.Options;

namespace WebApiLvl2.OptionsConfigurations;

public class SwaggerOptionsConfiguration : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _versionDescriptionProvider;


    public SwaggerOptionsConfiguration(IApiVersionDescriptionProvider versionDescriptionProvider) =>
        _versionDescriptionProvider = versionDescriptionProvider;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _versionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateVersionInfo(description));
        }
    }

    public void Configure(string name, SwaggerGenOptions options)
    {
        Configure(options);
    }

    private OpenApiInfo CreateVersionInfo(ApiVersionDescription description)
    {
        var versionInfo = new OpenApiInfo
        {
            Title = "WebApi lvl 2",
            Version = description.ApiVersion.ToString(),
        };

        if (description.IsDeprecated)
        {
            versionInfo.Description += " This version is deprecated.";
        }

        return versionInfo;
    }
}