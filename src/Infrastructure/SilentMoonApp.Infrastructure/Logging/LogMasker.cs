using System.Reflection;
using System.Collections;
using SilentMoonApp.Application.Abstractions.Logging;


namespace SilentMoonApp.Infrastructure.Logging;

public class LogMasker : ILogMasker
{

	private static readonly HashSet<string> SensitiveKeywords = new(StringComparer.OrdinalIgnoreCase)
	{
		"Password",
		"ConfirmPassword",
		"Otp",
		"OtpCode",
		"RawCode",
		"AccessToken",
		"RefreshToken",
		"IdToken",
		"SecretKey",
		"ClientSecret",
		"Authorization",
		"Cookie"
	};


	private static readonly HashSet<string> PartialKeywords = new(StringComparer.OrdinalIgnoreCase)
	{
		"Email",
		"RecipientEmail",
		"RequestEmail"
	};


	public object? Mask(object? value)
	{
		if (value is null)
			return null;

		if (value is Stream stream)
			return "***[Stream]***";

		Type valueType = value.GetType();


		if (IsNotSensitive(valueType))
			return value;


		if (value is IEnumerable enumerable && value is not string)
			return enumerable.Cast<object?>()
							 .Select(Mask)
							 .ToArray();


		Dictionary<string, object?> maskedProperties = new();


		foreach (PropertyInfo property in valueType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
		{
			if (!property.CanRead)
				continue;


			object? propertyValue;

			try
			{
				propertyValue = property.GetValue(value);
			}

			catch
			{
				maskedProperties[property.Name] = "***[UNREADABLE]***";
				continue;
			}


			if (SensitiveKeywords.Contains(property.Name))
			{
				maskedProperties[property.Name] = "***[REDACTED]***";
				continue;
			}

			if (PartialKeywords.Contains(property.Name) && propertyValue is string strValue)
			{
				maskedProperties[property.Name] = MaskPartial(strValue);
				continue;
			}


			maskedProperties[property.Name] = IsNotSensitive(property.PropertyType)
				? propertyValue
				: Mask(propertyValue);
		}


		return maskedProperties;
	}



	// Helpers

	private static bool IsNotSensitive(Type valueType)

		=> valueType.IsPrimitive ||
		   valueType.IsEnum ||
		   valueType == typeof(Guid) ||
		   valueType == typeof(string) ||
		   valueType == typeof(decimal) ||
		   valueType == typeof(TimeSpan) ||
		   valueType == typeof(DateTime) ||
		   valueType == typeof(DateTimeOffset);


	private static string MaskPartial(string value)
	{

		if (value.Contains('@'))
		{
			var atIndex = value.IndexOf('@');


			if (atIndex <= 0)
				return "***[REDACTED]***";


			string local = value[..atIndex];
			string domain = value[atIndex..];


			if (local.Length <= 1)




				return $"******{domain}";

			else
				return $"{local[0]}*****{domain}";
		}

		///

		else
			return "";
	}

}
