using System.Collections.Generic;

namespace Veyesys.Data.Mapping
{
    public class VeEntityDescriptor
    {
        public VeEntityDescriptor()
        {
            Fields = new List<VeEntityFieldDescriptor>();
        }

        public string EntityName { get; set; }
        public ICollection<VeEntityFieldDescriptor> Fields { get; set; }
    }
}