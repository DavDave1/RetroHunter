
using System;
using System.Xml.Serialization;

namespace RaSetMaker.Models
{
    public class ModelBase
    {
        public Guid Id { get; set; }

        [XmlIgnore]
        public ModelBase? Parent { get; set; }

        public ModelBase()
        {
            Id = Guid.NewGuid();
        }

        public ModelBase(ModelBase parent)
        {
            Parent = parent;
        }
    }
}
