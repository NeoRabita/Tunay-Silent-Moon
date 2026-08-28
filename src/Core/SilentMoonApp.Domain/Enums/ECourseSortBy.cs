using System.Text.Json.Serialization;

namespace SilentMoonApp.Domain.Enums;


[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ECourseSortBy
{
	CreatedAt = 1,
	Title = 2,
	Popularity = 3
}
