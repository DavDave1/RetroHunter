
using System.Collections.Generic;

namespace RetroHunter.Models
{
    public enum DirStructureStyle
    {
        Retroachievements,
        EmuDeck,
    }

    public class UserConfig : ModelBase
    {

        #region  RA Config
        public string Name { get; set; } = string.Empty;

        public string RaApiKey { get; set; } = string.Empty;
        #endregion

        #region Generator config
        public string InputRomsDirectory { get; set; } = string.Empty;
        public string OutputRomsDirectory { get; set; } = string.Empty;

        public DirStructureStyle DirStructureStyle { get; set; } = DirStructureStyle.Retroachievements;

        public List<GameType> GameTypesFilter { get; set; } = [];

        public string ChdmanExePath { get; set; } = string.Empty;
        #endregion

        public UserConfig() : base()
        {

        }
    }
}
