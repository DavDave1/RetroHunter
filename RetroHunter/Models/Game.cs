using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace RetroHunter.Models
{
    public enum GameType
    {
        Demo,
        Unlicensed,
        Hack,
        Homebrew,
        Prototype,
        TestKit,
        Subset
    }

    public class Game : ModelBase
    {
        public string Name { get; set; } = string.Empty;

        public string IconUrl { get; set; } = string.Empty;

        public int RaId { get; set; }

        public List<GameType> GameTypes { get; set; } = [];

        public DateTime LastUpdated { get; set; } = DateTime.UnixEpoch;

        public List<Rom> Roms { get; set; } = [];

        [JsonIgnore]
        public GameSystem? GameSystem => Parent != null ? (GameSystem)Parent : null;

        [JsonIgnore]
        public Ra2DatModel? Root { get; private set; }

        public bool HasValidRom() => Roms.Any(r => r.Exists());


        public Game() : base()
        {
        }

        public void InitParents(Ra2DatModel root, ModelBase parent)
        {
            Parent = parent;
            Root = root;
            Roms.ForEach(r => r.InitParents(this));
        }
    }
}
