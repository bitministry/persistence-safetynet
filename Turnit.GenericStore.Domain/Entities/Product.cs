using System;


namespace Turnit.GenericStore.Domain.Entities
{
    public class Product
    {
        public virtual Guid Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }
    }


}