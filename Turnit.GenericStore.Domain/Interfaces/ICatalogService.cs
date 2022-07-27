using System;
using System.Threading.Tasks;
using Turnit.GenericStore.Domain.Dtos;
using Turnit.GenericStore.Dtos;

namespace Turnit.GenericStore.Domain.Interfaces
{

    public enum ProductCategoryAction { Add, Remove };

    public interface ICatalogService
    {

        CategoryDto[] AllCategories();
        ProductCategoryDto[] AllProducts();
        Task ModifyProductCategoryAsync(Guid productId, Guid categoryId, ProductCategoryAction action);
        ProductCategoryDto ProductsByCategory(Guid categoryId);
    }
}