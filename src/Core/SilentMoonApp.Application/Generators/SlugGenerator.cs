using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SilentMoonApp.Application.Generators;

public static class SlugGenerator
{
	public static string GenerateSlug(string title)
	{
		if (string.IsNullOrWhiteSpace(title))
			return string.Empty;

		string normalized = title.Trim().ToLowerInvariant();

		normalized = normalized
			.Replace("ə", "e")
			.Replace("ö", "o")
			.Replace("ü", "u")
			.Replace("ı", "i")
			.Replace("ğ", "g")
			.Replace("ş", "s")
			.Replace("ç", "c");

		string withoutDiacritics = RemoveDiacritics(normalized);

		string slug = Regex.Replace(withoutDiacritics, @"[^a-z0-9]+", "-");
		slug = Regex.Replace(slug, @"-+", "-");

		return slug.Trim('-');
	}


	private static string RemoveDiacritics(string value)
	{
		string normalized = value.Normalize(NormalizationForm.FormD);

		StringBuilder builder = new();

		foreach (char character in normalized)
		{
			UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(character);

			if (category != UnicodeCategory.NonSpacingMark)
				builder.Append(character);
		}

		return builder.ToString().Normalize(NormalizationForm.FormC);
	}

}
