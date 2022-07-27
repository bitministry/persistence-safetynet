using System;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Turnit.GenericStore.Domain.Entities;
using Turnit.GenericStore.NHibernateMaps;

using System.Linq;

using Turnit.GenericStore.Dtos;
using System.Threading.Tasks;
using NHibernate;
using Turnit.GenericStore.NHiber.Services;
using Turnit.GenericStore.Domain.Interfaces;
using Turnit.GenericStore.Domain.Dtos;

namespace ConsoleApp1
{
    class Program
    {
        ISession _session;

        static void Main(string[] args) => new Program().Run();
        void Run( )
        {


            UnitOfWork.ConnectionString = "Server=localhost;Port=5632;Database=turnit_store;User ID=postgres;Password=postgres;";

            UnitOfWork.ConnectionString = "server=database-1.cpqot4dgfm9m.us-east-2.rds.amazonaws.com;Port=5432;Database=turnit_store;User Id=postgres;Password=Turnit123;";

            _session = UnitOfWork.CreateSessionFactory( null ).OpenSession();

            var ps = new CatalogService( _session );

            ps.ModifyProductCategoryAsync(
                productId: new Guid("6f326510-e65c-11ec-a1b4-24ee9a88c06f"),
                categoryId: new Guid("4f10a98b-e65b-11ec-a1ad-24ee9a88c06f"),
                action: ProductCategoryAction.Remove).Wait();

            _ = Foo();

            Console.ReadLine();
            return;

            var guids = _session.Query<ProductCategory>()
                    .Where(x => x.Category.Id == new Guid("4f0f4a0e-e65b-11ec-a1ab-24ee9a88c06f"))
                    .Select(x => x.Product.Id)
                    .ToList();


            var qqq = _session.Query<ProductAvailability>()
                .Where(x => guids.Contains(x.Product.Id))
                .GroupBy(x => x.Product)
                .Select(x => new ProductDto()
                {
                    Id = x.Key.Id,
                    Name = x.Key.Name,
                    Availability = x.Select(x => new ProductDto.AvailabilityDto()
                    {
                        Availability = x.Availability,
                        StoreId = x.Store.Id
                    }).ToArray()
                }).ToListAsync();




            // allprods.Join( allprodscats, )


        }

        async Task Foo() {


            var cat = await _session.Query<ProductCategory>().FirstOrDefaultAsync(x => x.Product.Id == new Guid("6f326510-e65c-11ec-a1b4-24ee9a88c06f"));


            return;


            var allProdsuctCategories = await _session.Query<ProductCategory>().ToListAsync();
            var allAvailability = await _session.Query<ProductAvailability>().ToListAsync();

            var productsGuidsWithCats = allProdsuctCategories.Select(x => x.Product.Id).Distinct().ToList();

            var productsWoCats = await _session.Query<Product>()
                .Where(x => ! productsGuidsWithCats.Contains( x.Id)).ToListAsync();



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





        }
    }
}
