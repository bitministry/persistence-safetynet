// <copyright file="StoreController.cs" company="BitMinistry">
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
using Turnit.GenericStore.WebApi.Common;

namespace Turnit.GenericStore.WebApi.CatalogHost.Controllers
{

    [Route("stores")]
    public class StoreController : ApiControllerBase
    {
        IInventoryService _inventoryService;
        UnprocessedRequestsQueue _safetyNet = new UnprocessedRequestsQueue(nameof(StoreController));

        public StoreController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }


        [HttpGet, Route("")]
        public ActionResult<CategoryDto[]> AllStores()
        {
            var result = _inventoryService.GetStores();

            return Ok(result);
        }



        public record InventoryDto([Required] Guid storeId, [Required] Guid productId, [Range(1,10000)] int quantity);

        [HttpPost, Route("book")]
        public async Task<ActionResult<InventoryDto>> Book(InventoryDto model)
        {

            return await _safetyNet.ExecuteRequest(model, async x =>
            {
                try
                {
                    var nuQuantity = await _inventoryService.UpdateAvailabilityAsync(
                                    productId: x.productId,
                                    storeId: x.storeId,
                                    quantity: x.quantity, 
                                    action: UpdateAvailabilityAction.Remove);

                    return Ok(new InventoryDto( storeId: model.storeId, productId: model.productId, quantity: nuQuantity ));
                }
                catch (NotSupportedException ex)
                {
                    return NotFound( ex.Message );
                }
                catch (Exception ex)
                {
                    await _safetyNet.Enqueue(model, ex);
                    return Problem("Unable to persist to database, request queued");
                }
            });

        }

        [HttpPost, Route("restock")]
        public async Task<IActionResult> Restock(InventoryDto model)
        {

            return await _safetyNet.ExecuteRequest(model, async x =>
            {
                try
                {
                    var nuQuantity = await _inventoryService.UpdateAvailabilityAsync(
                                    productId: x.productId,
                                    storeId: x.storeId,
                                    quantity: x.quantity,
                                    action: UpdateAvailabilityAction.Add );

                    return Ok(new InventoryDto(storeId: model.storeId, productId: model.productId, quantity: nuQuantity));
                }
                catch (Exception ex)
                {
                    _safetyNet.Enqueue(model, ex);
                    return Problem("Unable to persist to database, request queued");
                }
            });

        }



    }



}