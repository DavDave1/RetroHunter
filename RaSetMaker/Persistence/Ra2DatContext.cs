using RaSetMaker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RaSetMaker.Persistence
{
    public class Ra2DatContext
    {
        public UserConfig UserConfig
        {
            get => _model.UserConfig;
            set => _model.UserConfig = value;
        }

        public string FilePath { get; set; } = string.Empty;

        public Ra2DatContext()
        {
            _model.InitGameSystems();
        }

        public IEnumerable<GameSystem> GetSystems() => _model.Systems;

        public IEnumerable<GameSystem> GetCheckedSystems() => _model.Systems.Where(s => s.IsChecked);

        public void LoadModel(string xmlModelFilepath)
        {
            FilePath = xmlModelFilepath;
            var serializer = new XmlSerializer(typeof(Ra2DatModel));

            var finfo = new FileInfo(xmlModelFilepath);
            if (finfo.Exists)
            {
                TextReader reader = new StreamReader(xmlModelFilepath);
                var ra2DatModel = serializer.Deserialize(reader) as Ra2DatModel;
                reader.Dispose();

                if (ra2DatModel != null)
                {
                    _model = ra2DatModel;
                }

                bool modelChanged = MigrationFactory.ExecuteAll(_model);
                if (modelChanged)
                {
                    SaveChanges();
                }
            }
        }

        public void SaveChanges()
        {
            if (FilePath == string.Empty)
            {
                throw new InvalidOperationException("Model file path not set");
            }

            var serializer = new XmlSerializer(typeof(Ra2DatModel));
            var writer = new StreamWriter(FilePath);
            serializer.Serialize(writer, _model);           
        }

        public async Task SaveChangesAsync()
        {
            await Task.Run(SaveChanges);
        }

        public void SyncSystems(IEnumerable<GameSystem> raSystems)
        {
            foreach (var system in GetSystems())
            {
                var raSystem = raSystems.FirstOrDefault(raSys => raSys.Name == system.Name);
                if (raSystem != null)
                {
                    system.RaId = raSystem.RaId;
                    system.IconUrl = raSystem.IconUrl;
                }
            }
        }

        public void SyncGames(IEnumerable<Game> raGames, GameSystem system)
        {
            var newGames = raGames.Where(rag => system.Games.All(g => g.Id != rag.Id));
            system.Games.AddRange(newGames);
            system.LastUpdate = DateTime.UtcNow;
        }

        public bool ValidateRoms()
        {
            bool dataChanged = false;

            var roms = GetSystems()
                .SelectMany(s => s.Games)
                .SelectMany(g => g.Roms)
                .Where(r => r.IsValid)
                .Select(r => (r, new FileInfo(r.FilePath)));

            var outDir = new DirectoryInfo(UserConfig.OutputRomsDirectory);

            foreach (var (rom, fileInfo) in roms)
            {
                var expectedPath = $"{outDir.FullName}{fileInfo.Directory?.Name??""}\\{fileInfo.Name}";

                if (new FileInfo(expectedPath).Exists == false)
                {
                    rom.FilePath = string.Empty;
                    dataChanged = true;
                }
            }

            return dataChanged;
        }

        private Ra2DatModel _model = new();
    }
}
