namespace SilentMoonApp.Application.DTOs.Common;

public class PaginationQueryRequest
{
	public int PageNumber { get; set; } = 1;
	public int PageSize { get; set; } = 20;
}
