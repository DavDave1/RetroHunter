using RaSetMaker.Models;
using System.Linq;

namespace RaSetMaker.Persistence.Migrations
{
    public class Migration_FixSnesExtensions : IMigration
    {
        public bool Execute(Ra2DatModel model)
        {
            var snes = model.Systems.First(s => s.Name == "SNES/Super Famicom");

            if (snes.SupportedExtensions.Count == 13)
            {
                snes.SupportedExtensions = [".sfc", ".smc", ".st", ".swc"];
                return true;
            }

            return false;
        }
    }
}
