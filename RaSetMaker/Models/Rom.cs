
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace RaSetMaker.Models
{
    public class Rom : ModelBase
    {
        public string Hash { get; set; } = string.Empty;

        public List<RomFile> RomFiles { get; set; } = [];

        public string PatchUrl { get; set; } = string.Empty;

        public string RaName { get; set; } = string.Empty;

        [XmlIgnore]
        public Game? Game => Parent != null ? (Game)Parent : null;

        public long GetSize()
            => Game == null ? 0 : RomFiles.Sum(rf => rf.GetSize(Game.Root!.UserConfig.OutputRomsDirectory));

        public bool Exists()
            => Game != null && RomFiles.Count > 0 && RomFiles.All(rf => rf.Exists(Game.Root!.UserConfig.OutputRomsDirectory));

        public Rom() : base()
        {
        }

        public void InitParents(ModelBase parent)
        {
            Parent = parent;
        }
    }

    public class RomFile
    {
        public string FilePath { get; set; } = string.Empty;

        public uint Crc32 { get; set; }

        public bool Exists(string basePath) => Path.Exists(Path.Combine(basePath, FilePath));

        public long GetSize(string basePath) => new FileInfo(Path.Combine(basePath, FilePath)).Length;

    }
}
