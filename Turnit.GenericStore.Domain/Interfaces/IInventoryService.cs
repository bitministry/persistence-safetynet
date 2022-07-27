using System;
using System.Threading.Tasks;
using Turnit.GenericStore.Domain.Entities;

namespace Turnit.GenericStore.Domain.Interfaces
{

    public enum UpdateAvailabilityAction { Add, Remove };

    public interface IInventoryService
    {
        Store[] GetStores();

        Task<int>UpdateAvailabilityAsync(Guid productId, Guid storeId, int quantity, UpdateAvailabilityAction action );
    }
}