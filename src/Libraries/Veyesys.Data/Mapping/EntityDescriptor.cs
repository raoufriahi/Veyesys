using System.Collections.Generic;

namespace Veyesys.Data.Mapping
{
    public class EntityDescriptor
    {
        public EntityDescriptor()
        {
            Fields = new List<EntityFieldDescriptor>();
        }

        public string EntityName { get; set; }
        public ICollection<EntityFieldDescriptor> Fields { get; set; }
    }
}