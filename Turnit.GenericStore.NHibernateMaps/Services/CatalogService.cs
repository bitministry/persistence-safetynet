using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Turnit.GenericStore.Domain.Dtos;
using Turnit.GenericStore.Domain.Entities;
using Turnit.GenericStore.Domain.Interfaces;
using Turnit.GenericStore.Dtos;
using Turnit.GenericStore.NHibernateMaps;

namespace Turnit.GenericStore.NHiber.Services
{
    public class CatalogService : ICatalogService
    {
        ISession _session;

        public CatalogService(ISession sess)
        {
            this._session = sess;
        }

        public CategoryDto[] AllCategories()
        {
            var categories = _session.Query<Category>().ToList();

            var result = categories
                .Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .OrderBy(x => x.Name)
                .ToArray();

            return result;
        }


        public ProductCategoryDto ProductsByCategory(Guid categoryId)
        {
            var guids = _session.Query<ProductCategory>()
                .Where(x => x.Category.Id == categoryId )
                .Select(x => x.Product.Id)
                .ToList();

            var availability = _session.Query<ProductAvailability>()
                .Where(x => guids.Contains(x.Product.Id))
                .ToList();

            var productsByCategories = availability.GroupBy(x => x.Product)
                .Select(x => new ProductDto()
                {
                    Id = x.Key.Id,
                    Name = x.Key.Name,
                    Availability = x.Select(x => new ProductDto.AvailabilityDto()
                    {
                        Availability = x.Availability,
                        StoreId = x.Store.Id
                    }).ToArray()
                }).ToList();

            return new ProductCategoryDto()
            {
                CategoryId = categoryId,
                Products = productsByCategories.ToArray()
            };
        }

        public ProductCategoryDto[] AllProducts()
        {

            var allProdsuctCategories = _session.Query<ProductCategory>().ToList();
            var allAvailability = _session.Query<ProductAvailability>().ToList();

            var productsGuidsWithCats = allProdsuctCategories.Select(x => x.Product.Id).Distinct().ToList();

            var productsWoCats = _session.Query<Product>()
                .Where(x => !productsGuidsWithCats.Contains(x.Id)).ToList();


            var allAvailabilityByProductGuid = allAvailability
                .GroupBy(x => x.Product.Id)
                .ToDictionary(x => x.Key, x => x);


            ProductDto createProductModel(Guid id, string name, IEnumerable<ProductAvailability> availability) =>
                new ProductDto()
                {
                    Id = id,
                    Name = name,
                    Availability = availability?.Select(z => new ProductDto.AvailabilityDto()
                    {
                        Availability = z.Availability,
                        StoreId = z.Store.Id
                    }).ToArray()
                };

            var productCatModels = allProdsuctCategories
                .GroupBy(productCategory => productCategory.Category)
                .Select(groupingByCategory => new ProductCategoryDto()
                {
                    CategoryId = groupingByCategory.Key.Id,
                    Products = groupingByCategory.Select(productCategory => createProductModel(
                        productCategory.Product.Id,
                        productCategory.Product.Name,
                        allAvailabilityByProductGuid[productCategory.Product.Id])
                    ).ToArray()
                }).ToList();


            productCatModels.Add(new ProductCategoryDto()
            {
                CategoryId = Guid.Empty,
                Products = productsWoCats.Select(product => createProductModel(
                   product.Id,
                   product.Name,
                   allAvailabilityByProductGuid[product.Id])).ToArray()
            });

            return productCatModels.ToArray();
        }

        public async Task ModifyProductCategoryAsync(Guid productId, Guid categoryId, ProductCategoryAction action)
        {
            using (var outOfRequestSess = UnitOfWork.SessionFactory.OpenSession())
            {

                var cat = await outOfRequestSess.Query<ProductCategory>()
                    .FirstOrDefaultAsync(x =>
                        x.Product.Id == productId && x.Category.Id == categoryId
                    );

                if (action == ProductCategoryAction.Add && cat == null)
                    await outOfRequestSess.SaveAsync(new ProductCategory()
                    {
                        Product = new Product() { Id = productId },
                        Category = new Category() { Id = categoryId }
                    });

                if (action == ProductCategoryAction.Remove)
                    if (cat == null)
                        throw new NotSupportedException();
                    else
                        await outOfRequestSess.DeleteAsync(cat);

                await outOfRequestSess.FlushAsync();

            }

                

        }

    }

}
