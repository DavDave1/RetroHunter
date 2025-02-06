
using RaSetMaker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using static RaSetMaker.Models.GameSystemData;

namespace RaSetMaker.Models
{
    public class GameSystem : ModelBase
    {
        public GameSystemType GameSystemType { get; set; }

        [XmlIgnore]
        public int RaId => (int)GameSystemType;

        [XmlIgnore]
        public string Name => GameSystemType.Name();

        public string IconUrl { get; set; } = string.Empty;

        public bool IsChecked { get; set; } = true;

        public DateTime LastUpdate { get; set; } = DateTime.UnixEpoch;

        [XmlIgnore]
        public GameSystemCompany Company => GameSystemType.Company();

        [XmlIgnore]
        public RomMatcherType MatcherType => GameSystemType.Matcher();

        [XmlIgnore]
        public List<string> SupportedExtensions => GameSystemType.Extensions();


        public List<Game> Games
        {
            get => _games;
            set
            {
                _games = value;
                foreach (var game in _games)
                {
                    game.Parent = this;
                }
            }
        }

        public GameSystem() : base()
        {
        }

        public GameSystem(GameSystemType type)
        {
            GameSystemType = type;
        }

        public IEnumerable<Game> GetGamesMatchingFilter(List<GameType> allowedTypes)
        {
            return Games.Where(g => g.GameTypes.Count == 0 || g.GameTypes.Intersect(allowedTypes).Any());
        }

        public string GetDirName(DirStructureStyle style) => GameSystemType.FolderName(style);

        private List<Game> _games = [];
    }
}
