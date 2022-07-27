using Microsoft.AspNetCore.Mvc;
using Turnit.GenericStore.Api.Features;
using Turnit.GenericStore.Domain.Dtos;
using Turnit.GenericStore.Domain.Interfaces;

namespace Turnit.GenericStore.WebApi.CatalogHost.Controllers
{

    [Route("categories")]
    public class CategoriesController : ApiControllerBase
    {
        ICatalogService _productService;

        public CategoriesController(ICatalogService productService)
        {
            _productService = productService;
        }

        [HttpGet, Route("")]
        public ActionResult<CategoryDto[]> AllCategories()
        {
            var result =  _productService.AllCategories();

            return Ok(result);
        }
    }
}