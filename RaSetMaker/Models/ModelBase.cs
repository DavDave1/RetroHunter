
using System.Text.Json.Serialization;

namespace RaSetMaker.Models
{
    public class ModelBase
    {
        [JsonIgnore]
        public ModelBase? Parent { get; set; }

        public ModelBase()
        {
        }

        public ModelBase(ModelBase parent)
        {
            Parent = parent;
        }
    }
}
