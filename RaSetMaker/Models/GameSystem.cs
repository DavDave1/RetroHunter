
using RaSetMaker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace RaSetMaker.Models
{
    public class GameSystem : ModelBase
    {
        public int RaId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string IconUrl { get; set; } = string.Empty;

        public bool IsChecked { get; set; } = true;

        public DateTime LastUpdate { get; set; } = DateTime.UnixEpoch;

        public GameSystemCompany Company { get; set; } = GameSystemCompany.Other;

        public RomMatcherType MatcherType { get; set; } = RomMatcherType.Null;

        public List<string> SupportedExtensions { get; set; } = [];

        [XmlIgnore]
        public Dictionary<DirStructureStyle, string> DirNames { get; set; } = [];

        public List<Game> Games { 
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

        public GameSystem(GameSystemCompany company)
        {
            Company = company;
        }

        public GameSystem(
            GameSystemCompany company, 
            string name, 
            RomMatcherType matcherType, 
            Dictionary<DirStructureStyle, string> dirNames,
            List<string> supportedExtensions) : base()
        {
            Company = company;
            Name = name;
            DirNames = dirNames;
            MatcherType = matcherType;
            SupportedExtensions = supportedExtensions;
        }

        public IEnumerable<Game> GetGamesMatchingFilter(List<GameType> allowedTypes)
        {
            return Games.Where(g => g.GameTypes.Count == 0 || g.GameTypes.Intersect(allowedTypes).Any());
        }

        #region serialization proxy
        [XmlElement("DirNames")]
        public SerializableDictionary<DirStructureStyle, string> DirNamesSerializeProxy
        {
            get
            {
                return new SerializableDictionary<DirStructureStyle, string>(DirNames);
            }
            set
            {
                DirNames = value.ToDictionary();
            }
        }
        #endregion

        private List<Game> _games = [];
    }
}
