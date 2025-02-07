
using System.IO;
using System.Xml.Serialization;

namespace RaSetMaker.Models
{
    public class Rom : ModelBase
    {
        public string Hash { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public string PatchUrl { get; set; } = string.Empty;

        [XmlIgnore]
        public Game? Game => Parent != null ? (Game)Parent : null;

        [XmlIgnore]
        public bool IsValid => FilePath != string.Empty;

        public Rom() : base()
        {
        }

        public Rom(Game parent) : base(parent)
        {
        }
    }
}
