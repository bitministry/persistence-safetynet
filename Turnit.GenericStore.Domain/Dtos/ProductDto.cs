using System;

namespace Turnit.GenericStore.Domain.Dtos
{

    public class ProductDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public AvailabilityDto[] Availability { get; set; }

        public class AvailabilityDto
        {
            public Guid StoreId { get; set; }

            public int Availability { get; set; }
        }
    }


}