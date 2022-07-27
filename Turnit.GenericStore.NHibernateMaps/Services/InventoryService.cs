using NHibernate;
using NHibernate.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Turnit.GenericStore.Domain.Entities;
using Turnit.GenericStore.Domain.Interfaces;
using Turnit.GenericStore.NHibernateMaps;

namespace Turnit.GenericStore.NHiber.Services
{
    public class InventoryService : IInventoryService
    {

        ISession _session;

        public InventoryService(ISession session)
        {
            _session = session;
        }

        public Store[] GetStores()
        {
            var result = _session.Query<Store>().ToArray();

            return result;
        }

        public async Task<int> UpdateAvailabilityAsync(Guid productId, Guid storeId, int quantity, UpdateAvailabilityAction action )
        {
            if (action == UpdateAvailabilityAction.Remove)
                quantity = quantity * -1;

            using (var outOfRequestSess = UnitOfWork.SessionFactory.OpenSession())
            {
                var availability = await _session.Query<ProductAvailability>()
                    .FirstOrDefaultAsync(x => x.Product.Id == productId && x.Store.Id == storeId);

                if (action == UpdateAvailabilityAction.Remove)
                {
                    if (availability == null)
                        throw new NotSupportedException("not available");
                    if (availability.Availability + quantity < 0)
                        throw new NotSupportedException($"only { availability.Availability } available");
                }
                else
                {
                    availability = availability ?? new ProductAvailability()
                    {
                        Product = new Product() { Id = productId },
                        Store = new Store() { Id = storeId },
                        Availability = 0
                    };
                }
                availability.Availability += quantity;

                await _session.SaveOrUpdateAsync(availability);

                await _session.FlushAsync();

                return availability.Availability;

            }

    

        }
    }

}
