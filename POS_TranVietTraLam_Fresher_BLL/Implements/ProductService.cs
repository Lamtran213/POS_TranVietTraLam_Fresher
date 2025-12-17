using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using POS_TranVietTraLam_Fresher_BLL.Defines;
using POS_TranVietTraLam_Fresher_BLL.DTO.PaymentDTO;
using POS_TranVietTraLam_Fresher_BLL.DTO.ProductDTO;
using POS_TranVietTraLam_Fresher_BLL.Pagination;
using POS_TranVietTraLam_Fresher_DAL.Defines;
using POS_TranVietTraLam_Fresher_Entities.Entity;

namespace POS_TranVietTraLam_Fresher_BLL.Implements
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductService> _logger;
        private readonly IStorageService _storageService;
        public ProductService(IUnitOfWork unitOfWork, ILogger<ProductService> logger, IStorageService storageService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _storageService = storageService;
        }

        public async Task<AddProductResponse> AddProductAsync(AddProductRequest addProductRequest)
        {
            string imageUrl = null;

            if (addProductRequest.ImageUrl != null && addProductRequest.ImageUrl.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(addProductRequest.ImageUrl.FileName)}";

                await using var stream = addProductRequest.ImageUrl.OpenReadStream();
                imageUrl = await _storageService.UploadFileAsync(fileName, stream);
            }

            var product = new Product
            {
                ProductName = addProductRequest.ProductName,
                Description = addProductRequest.Description,
                UnitPrice = addProductRequest.UnitPrice,
                UnitsInStock = addProductRequest.UnitsInStock,
                CategoryId = addProductRequest.CategoryId,
                ImageUrl = imageUrl, 
                Discount = addProductRequest.Discount,
                IsActive = addProductRequest.IsActive
            };

            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.Save();

            return new AddProductResponse
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                UnitPrice = product.UnitPrice,
                UnitsInStock = product.UnitsInStock,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl,
                Discount = product.Discount,
                IsActive = product.IsActive
            };
        }

        public async Task<PagedResult<GetProductResponse>> GetAllProductsAsync(
            int pageIndex,
            int pageSize)
        {
            var query = _unitOfWork.ProductRepository
                .Query()
                .Where(p => p.IsActive);

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.ProductId)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new GetProductResponse
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    UnitPrice = p.UnitPrice,
                    ImageUrl = p.ImageUrl,
                    CategoryId = p.CategoryId,
                    Description = p.Description
                })
                .ToListAsync();

            return new PagedResult<GetProductResponse>
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }
    }
}
