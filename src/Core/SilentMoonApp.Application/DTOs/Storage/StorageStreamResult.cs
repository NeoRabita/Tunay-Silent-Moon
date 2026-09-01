namespace SilentMoonApp.Application.DTOs.Storage;

public class StorageStreamResult : IDisposable
{
	public IDisposable? _lease { get; }
	
	public StorageStreamResult(Stream stream,
							 string contentType,
							 long? contentLength,
							 string? contentRange,
							 bool acceptRanges,
							 int statusCode,
							 IDisposable? lease = null)
	{
		Stream = stream;
		ContentType = contentType;
		ContentLength = contentLength;
		ContentRange = contentRange;
		AcceptRanges = acceptRanges;
		StatusCode = statusCode;
		_lease = lease;
	}

	public Stream Stream { get; }
	public string ContentType { get; }
	public long? ContentLength { get; }
	public string? ContentRange { get; }
	public bool AcceptRanges { get; }
	public int StatusCode { get; }
	

	public void Dispose()
	{
		Stream.Dispose();
		_lease?.Dispose();
	}
}
