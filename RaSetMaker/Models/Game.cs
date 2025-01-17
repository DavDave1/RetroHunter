using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace RaSetMaker.Models
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

        [XmlIgnore]
        public GameSystem? GameSystem => Parent != null ? (GameSystem)Parent : null;

        [XmlIgnore]
        public bool HasValidRom => Roms.Any(r => r.IsValid);

        public List<Rom> Roms { 
            get => _roms;
            set
            {
                _roms = value;
                foreach (var rom in _roms)
                {
                    rom.Parent = this;
                }
            }
        }

        public Game() : base()
        {
        }

        public Game(GameSystem parent) : base(parent)
        {
        }

        private List<Rom> _roms = []; 
    }
}
