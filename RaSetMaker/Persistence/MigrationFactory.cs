using RaSetMaker.Models;
using RaSetMaker.Persistence.Migrations;
using System;
using System.Collections.Generic;

namespace RaSetMaker.Persistence
{
    public static class MigrationFactory
    {

        static MigrationFactory()
        {
            _migrations[typeof(Migration_FixSnesExtensions)] = new Migration_FixSnesExtensions(); 
            _migrations[typeof(Migration_DeduplicateGames)] = new Migration_DeduplicateGames();
            _migrations[typeof(Migration_DetectSubset)] = new Migration_DetectSubset();

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
