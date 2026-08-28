using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Abstractions.Storage;
using SilentMoonApp.Application.DTOs.Storage;
using SilentMoonApp.Domain.Entities;
using System.Data;

namespace SilentMoonApp.Application.Features.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler : IQueryHandler<GetCategoriesQuery, IReadOnlyList<GetCategoriesResult>>
{
	private readonly IStorageService _storageService;
	private readonly IUnitOfWork _unitOfWork;

	public GetCategoriesQueryHandler(IUnitOfWork unitOfWork,
									 IStorageService storageService)
	{
		_unitOfWork = unitOfWork;
		_storageService = storageService;
	}


	public async Task<Result<IReadOnlyList<GetCategoriesResult>>> Handle(GetCategoriesQuery query, CancellationToken ct = default)
	{
		string? typeSlug = string.IsNullOrWhiteSpace(query.Type)
						 ? null
						 : query.Type.Trim().ToLowerInvariant();


		IReadOnlyList<Category> categories = await _unitOfWork.Repository<ICategoryReadRepository>()
															  .GetAllCategoriesWithTypeAsync(typeSlug: typeSlug,
																							 tracking: false,
																							 cancellationToken: ct);

		var iconUrlTasks = categories.Select(async category =>
		{
			if (category.IconFile is null || category.IconFile.IsDeleted)
				return new
				{
					Category = category,
					IconUrl = string.Empty
				};

			Result<string> iconUrlResult = await _storageService.GetFileUrlAsync(
				fileReference: new StorageFileReference(StorageProvider: category.IconFile.StorageProvider,
														ContainerName: category.IconFile.ContainerName,
														StoredFileName: category.IconFile.StoredFileName),
				cancellationToken: ct);

			return new
			{
				Category = category,
				IconUrl = iconUrlResult.IsSuccess
						? iconUrlResult.Value
						: string.Empty
			};
		});


		Dictionary<Guid, string> categoryIconUrls = (await Task.WhenAll(iconUrlTasks))
															   .ToDictionary(item => item.Category.Id, x => x.IconUrl);

		IReadOnlyList<GetCategoriesResult> result = categories.Select(category => new GetCategoriesResult
		(
			Id: category.Id,
			Title: category.Title,
			Slug: category.Slug,
			Type: category.CategoryType.Slug,
			IconUrl: categoryIconUrls.GetValueOrDefault(category.Id, string.Empty))
		).ToList();


		return Result<IReadOnlyList<GetCategoriesResult>>.Success(result);
	}
}
