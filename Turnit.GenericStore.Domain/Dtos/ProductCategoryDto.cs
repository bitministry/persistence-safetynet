using System;
using Turnit.GenericStore.Domain.Dtos;

namespace Turnit.GenericStore.Dtos
{
    public class ProductCategoryDto
    {
        public Guid? CategoryId { get; set; }

        public ProductDto[] Products { get; set; }
    }
}
