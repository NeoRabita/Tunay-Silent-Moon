using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SilentMoonApp.WebAPI.Filters;

public class AcceptLanguageHeaderFilter : IOperationFilter
{
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		operation.Parameters ??= [];

		operation.Parameters.Add(new OpenApiParameter
		{
			Name = "Accept-Language",
			In = ParameterLocation.Header,
			Required = false,
			Schema = new OpenApiSchema
			{
				Type = "string",
				Default = new OpenApiString("az"),
				Enum =
				[
					new OpenApiString("az"),
					new OpenApiString("en"),
					new OpenApiString("ru")
				]
			},
			Description = "Supported Languages: az, en, ru"
		});
	}
}
