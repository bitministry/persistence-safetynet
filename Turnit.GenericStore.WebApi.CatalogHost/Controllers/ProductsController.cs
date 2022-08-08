// <copyright file="ProductsController.cs" company="BitMinistry">
// Copyright (c) 2022 All Rights Reserved
// Licensed under the Apache License, Version 2.0
// </copyright>
// <author>Andrew Rebane</author>
// <date>2022-7-26</date>


using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Turnit.GenericStore.Api.Features;
using Turnit.GenericStore.Domain.Dtos;
using Turnit.GenericStore.Domain.Interfaces;
using Turnit.GenericStore.Dtos;
using Turnit.GenericStore.WebApi.Common;

namespace Turnit.GenericStore.WebApi.CatalogHost.Controllers
{

    [Route("products")]
    public class ProductsController : ApiControllerBase
    {
        ICatalogService _productService;
        UnprocessedRequestsQueue _safetyNet = new UnprocessedRequestsQueue( nameof(ProductsController));

        public ProductsController(ICatalogService productService)
        {
            _productService = productService;
        }

        [HttpGet, Route("by-category/{categoryId:guid}")]
        public ActionResult<ProductDto[]> ProductsByCategory(Guid categoryId)
        {
            var result =  _productService.ProductsByCategory(categoryId);

            return Ok(result);
        }

        [HttpGet, Route("")]
        public ActionResult<ProductCategoryDto[]> AllProducts()
        {
            var result =  _productService.AllProducts();

            return Ok(result);
        }


        public record AddProductCategoryDto([Required] Guid productId, [Required] Guid categoryId);
        [HttpPut, Route("{productId:guid}/category/{categoryId:guid}")]
        public async Task<IActionResult> AddProductCategory([FromRoute] AddProductCategoryDto model)
        {
            return await _safetyNet.ExecuteRequest(model, async x =>
            {
                try
                {
                    await _productService.ModifyProductCategoryAsync(
                                    productId: x.productId,
                                    categoryId: x.categoryId,
                                    action: ProductCategoryAction.Add );

                    return Ok();
                }
                catch (Exception ex)
                {
                    await _safetyNet.Enqueue(model, ex);
                    return Problem("Unable to persist to database, request queued");
                }
            });
        }


        public record RemoveProductCategoryDto([Required] Guid productId, [Required] Guid categoryId);
        [HttpDelete, Route("{productId:guid}/category/{categoryId:guid}")]
        public async Task<IActionResult> RemoveProductCategory([FromRoute] RemoveProductCategoryDto model)
        {
            return await _safetyNet.ExecuteRequest(model, async x =>
            {
                try
                {
                    await _productService.ModifyProductCategoryAsync(
                                    productId: x.productId,
                                    categoryId: x.categoryId,
                                    action: ProductCategoryAction.Remove);

                    return Ok();
                }
                catch (NotSupportedException ex)
                {
                    return NotFound();
                }
                catch (Exception ex)
                {
                    await _safetyNet.Enqueue(model, ex);
                    return Problem("Unable to persist to database, request queued");
                }
            });                

            
        }


    }



}