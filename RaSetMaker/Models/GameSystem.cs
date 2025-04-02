
using RaSetMaker.Utils.Matchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using static RaSetMaker.Models.GameSystemData;

namespace RaSetMaker.Models
{
    public class GameSystem : ModelBase
    {
        public GameSystemType GameSystemType { get; set; }

        [JsonIgnore]
        public int RaId => (int)GameSystemType;

        [JsonIgnore]
        public string Name => GameSystemType.Name();

        public string IconUrl { get; set; } = string.Empty;

        public bool IsChecked { get; set; } = true;

        public DateTime LastUpdate { get; set; } = DateTime.UnixEpoch;

        [JsonIgnore]
        public GameSystemCompany Company => GameSystemType.Company();

        [JsonIgnore]
        public List<string> SupportedExtensions => GameSystemType.Extensions();

        public List<Game> Games { get; set; } = [];

        [JsonIgnore]
        public Ra2DatModel? Root => Parent as Ra2DatModel;

        public GameSystem() : base()
        {
        }

        public GameSystem(GameSystemType type)
        {
            GameSystemType = type;
        }

        public IEnumerable<Game> GetGamesMatchingFilter()
        {
            return Games.Where(g => g.GameTypes.Count == 0 || g.GameTypes.Intersect(Root!.UserConfig.GameTypesFilter).Any());
        }

        public string GetDirName(DirStructureStyle style) => GameSystemType.FolderName(style);

        public MatcherBase CreateMatcher()
        {
            return (MatcherBase)Activator.CreateInstance(GameSystemType.Matcher(), this)!;
        }

        public void InitParents(Ra2DatModel root)
        {
            Parent = root;
            Games.ForEach(g => g.InitParents(root, this));
        }
    }
}
