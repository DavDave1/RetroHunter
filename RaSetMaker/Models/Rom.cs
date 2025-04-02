
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace RaSetMaker.Models
{
    public class Rom : ModelBase
    {
        public string Hash { get; set; } = string.Empty;

        public List<RomFile> RomFiles { get; set; } = [];

        public string PatchUrl { get; set; } = string.Empty;

        public string RaName { get; set; } = string.Empty;

        [JsonIgnore]
        public Game? Game => Parent != null ? (Game)Parent : null;

        public long GetSize()
            => Game == null ? 0 : RomFiles.Sum(rf => rf.GetSize());

        public bool Exists()
            => Game != null && RomFiles.Count > 0 && RomFiles.All(rf => rf.Exists());

        public Rom() : base()
        {
        }

        public RomFile AddRomFile(string filePath)
        {
            var romFile = new RomFile { FilePath = filePath };
            romFile.SetParent(this);
            RomFiles.Add(romFile);

            return romFile;
        }

        public void InitParents(ModelBase parent)
        {
            Parent = parent;
            RomFiles.ForEach(rf => rf.SetParent(this));
        }
    }
}
