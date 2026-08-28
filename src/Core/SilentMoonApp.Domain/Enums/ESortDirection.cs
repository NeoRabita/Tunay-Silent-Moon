using System.Text.Json.Serialization;

namespace SilentMoonApp.Domain.Enums;


[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ESortDirection
{
	Ascending = 1,
	Descending = 2
}
