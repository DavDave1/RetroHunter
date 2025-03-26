using System;
using System.Collections.Generic;
using static RaSetMaker.Models.GameSystemData;

namespace RaSetMaker.Models
{
    public class Ra2DatModel : ModelBase
    {
        public UserConfig UserConfig { get; set; } = new();

        public List<GameSystem> Systems { get; set; } = [];

        public Ra2DatModel() : base()
        {
        }

        public void InitGameSystems()
        {
            foreach (var type in Enum.GetValues<GameSystemType>())
            {
                Systems.Add(new(type));
            }
        }

        public void InitParents()
        {
            Systems.ForEach(s => s.InitParents(this));
        }
    }
}
