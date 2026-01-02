using RetroHunter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RetroHunter.Persistence
{
    public class DbContext
    {
        public UserConfig UserConfig
        {
            get => _model.UserConfig;
            set => _model.UserConfig = value;
        }

        public string FilePath { get; set; } = string.Empty;

        public DbContext()
        {
            _model.InitGameSystems();
            _model.InitParents();
        }

        public IEnumerable<GameSystem> GetSystems() => _model.Systems;

        public IEnumerable<GameSystem> GetCheckedSystems() => _model.Systems.Where(s => s.IsChecked);

        public Dictionary<string, Rom> GetRomsDictionary()  => _model.Systems.SelectMany(s => s.Games).SelectMany(g => g.Roms).ToDictionary(r => r.Hash, r => r);

        public async Task LoadModelAsync(string modelFilePath, bool newProject)
        {
            FilePath = modelFilePath;

            if (Path.Exists(modelFilePath) && !newProject)
            {
                {

                    await using FileStream stream = File.OpenRead(modelFilePath);
                    var model = await JsonSerializer.DeserializeAsync<Ra2DatModel>(stream);
                    if (model != null)
                    {
                        _model = model;
                    }
                }

                _model.InitParents();

                bool modelChanged = MigrationFactory.ExecuteAll(_model);
                if (modelChanged)
                {
                    await SaveChangesAsync();
                }
            }
        }

        public async Task SaveChangesAsync()
        {
            if (FilePath == string.Empty)
            {
                throw new InvalidOperationException("Model file path not set");
            }

            await using FileStream fileStream = File.Create(FilePath);
            
            await JsonSerializer.SerializeAsync(fileStream, _model, _options);
        }

        public void SyncSystems(IEnumerable<GameSystem> raSystems)
        {
            foreach (var system in GetSystems())
            {
                var raSystem = raSystems.FirstOrDefault(raSys => raSys.Name == system.Name);
                if (raSystem != null)
                {
                    system.IconUrl = raSystem.IconUrl;
                }
            }
        }

        public void SyncGames(IEnumerable<Game> raGames, GameSystem system)
        {
            var newGames = raGames.Where(rag => system.Games.All(g => g.RaId != rag.RaId));
            system.Games.AddRange(newGames);
            system.LastUpdate = DateTime.UtcNow;
        }

        public bool ValidateRoms()
        {
            bool dataChanged = false;

            var roms = GetSystems()
                .SelectMany(s => s.Games)
                .SelectMany(g => g.Roms)
                .Where(r => r.RomFiles.Count > 0);

            foreach (var rom in roms)
            {
                if (!rom.Exists())
                {
                    rom.RomFiles.Clear();
                    dataChanged = true;
                }
            }

            return dataChanged;
        }

        private Ra2DatModel _model = new();
        JsonSerializerOptions _options = new()
        {
            WriteIndented = true
        };
    }
}
