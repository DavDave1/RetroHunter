using RaSetMaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaSetMaker.Persistence.Migrations
{
    public class Migration_DetectSubset : IMigration
    {
        public bool Execute(Ra2DatModel model)
        {
            bool modelChanged = false;
            foreach(var game in model.Systems.SelectMany(s => s.Games))
            {
                // detect subset
                if (game.GameTypes.Count == 0 && game.Name.Contains("[Subset"))
                {
                    game.GameTypes.Add(GameType.Subset);
                    modelChanged = true;
                }
            }

            return modelChanged;
        }
    }
}
