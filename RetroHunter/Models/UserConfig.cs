
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
        public string InputRomsDirectory { get; set; } = string.Empty;
        public string OutputRomsDirectory { get; set; } = string.Empty;

        public DirStructureStyle DirStructureStyle { get; set; } = DirStructureStyle.Retroachievements;

        public List<GameType> GameTypesFilter { get; set; } = [];

        public UserConfig() : base()
        {

        }
    }
}
