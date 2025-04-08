using RetroHunter.Models;
using RetroHunter.Persistence.Migrations;
using System;
using System.Collections.Generic;

namespace RetroHunter.Persistence
{
    public static class MigrationFactory
    {

        static MigrationFactory()
        {
        }


        public static bool ExecuteAll(Ra2DatModel model)
        {
            bool modelChanged = false;
            foreach (var migration in _migrations.Values)
            {
                modelChanged |= migration.Execute(model);
            }

            return modelChanged;
        }

        private static Dictionary<Type, IMigration> _migrations = [];
    }
}
