using SilentMoonApp.Domain.Enums;
using System.Text.Json.Serialization;

namespace SilentMoonApp.WebAPI.Contracts.Health.GetHealth;

public sealed class GetHealthResponse
{
	public string Status { get; set; } = default!;
	public DateTimeOffset TimeStamp { get; set; }
	public string Version { get; set; } = default!;

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public EDatabaseStatus DatabaseStatus { get; set; }
}
