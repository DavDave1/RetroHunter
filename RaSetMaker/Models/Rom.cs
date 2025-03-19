
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace RaSetMaker.Models
{
    public class Rom : ModelBase
    {
        public string Hash { get; set; } = string.Empty;

        public List<string> FilePaths { get; set; } = [];

        public string PatchUrl { get; set; } = string.Empty;

        public string RaName { get; set; } = string.Empty;

        [XmlIgnore]
        public Game? Game => Parent != null ? (Game)Parent : null;

        public long GetSize(string basePath) => FilePaths.Sum(path => new FileInfo(Path.Combine(basePath, path)).Length);

        public bool Exists(string basePath) => FilePaths.Count > 0 && FilePaths.All(relPath => Path.Exists(Path.Combine(basePath, relPath)));

        public Rom() : base()
        {
        }

        public Rom(Game parent) : base(parent)
        {
        }
    }
}
