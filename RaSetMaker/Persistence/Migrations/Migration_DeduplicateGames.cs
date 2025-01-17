using RaSetMaker.Models;
using System.Collections.Generic;
using System.Linq;

namespace RaSetMaker.Persistence.Migrations
{
    public class Migration_DeduplicateGames : IMigration
    {
        public bool Execute(Ra2DatModel model)
        {
            bool modelChanged = false;
            foreach (var system in model.Systems)
            {
                var originalCount = system.Games.Count;

                system.Games = system.Games.DistinctBy(g => g.RaId).ToList();

                if (system.Games.Count != originalCount)
                {
                    modelChanged = true;
                }
            }

            return modelChanged;
        }
    }
}
