using System;


namespace Turnit.GenericStore.Domain.Entities
{

    public class Category
    {
        public virtual Guid Id { get; set; }

        public virtual string Name { get; set; }
    }


}