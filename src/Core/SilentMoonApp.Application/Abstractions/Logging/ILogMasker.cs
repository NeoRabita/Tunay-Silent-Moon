namespace SilentMoonApp.Application.Abstractions.Logging;

public interface ILogMasker
{
	object? Mask(object? value);
}
