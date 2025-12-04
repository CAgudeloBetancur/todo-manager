using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ToDoManager.Api.OpenApi;

public class ConfigureSwaggerGenOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
	private readonly IApiVersionDescriptionProvider _apiVersionDescriptionProvider;

	public ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider apiVersionDescriptionProvider)
	{
		_apiVersionDescriptionProvider = apiVersionDescriptionProvider;
	}

	public void Configure(SwaggerGenOptions options)
	{
		foreach (var description in _apiVersionDescriptionProvider.ApiVersionDescriptions)
		{
			var openApiInfo = new OpenApiInfo
			{
				Title = $"ToDoManager API v{description.ApiVersion}",
				Description = description.ApiVersion.ToString(),
			};
			
			options.SwaggerDoc(description.GroupName, openApiInfo);
		}
	}

	public void Configure(string? name, SwaggerGenOptions options)
	{
		Configure(options);
	}
}