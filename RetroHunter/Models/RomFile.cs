using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using System;
using System.IO;
using System.IO.Hashing;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RetroHunter.Models
{
    public class RomFile : ModelBase
    {
        public string FilePath { get; set; } = string.Empty;

        public uint Crc32 { get; set; }

        public void SetParent(Rom rom)
        {
            Parent = rom;
        }

        public string AbsolutePath() => Path.Combine(GetBasePath(), FilePath);
        public bool Exists() => Path.Exists(AbsolutePath());

        public long GetSize() => new FileInfo(AbsolutePath()).Length;

        public bool IsCompressed() => Path.GetExtension(FilePath) == ".zip";

        public RomFileStream GetRomFileStream() => new(this);

        public void Compress()
        {
            var uncompressedRomPath = AbsolutePath();
            var compressedRomPath = Path.ChangeExtension(uncompressedRomPath, ".zip");

            if (Path.Exists(compressedRomPath))
            {
                File.Delete(compressedRomPath);
            }

            {
                using var archive = ZipArchive.Create();
                archive.AddEntry(Path.GetFileName(uncompressedRomPath), uncompressedRomPath);
                archive.SaveTo(compressedRomPath, CompressionType.Deflate);
            }

            FilePath = Path.GetFileName(compressedRomPath);

            File.Delete(uncompressedRomPath);

        }

        private string GetBasePath()
        {
            var userConfig = Rom?.Game?.Root?.UserConfig;
            var gameSystem = Rom?.Game?.GameSystem;
            if (userConfig == null || gameSystem == null)
            {
                return "";
            }

            return Path.Combine(userConfig.OutputRomsDirectory, gameSystem.GetDirName(userConfig.DirStructureStyle));
        }

        [JsonIgnore]
        private Rom? Rom => Parent as Rom;
    }

    public class RomFileStream(RomFile rf) : IDisposable
    {

        public enum OpenMode
        {
            ExtractToTempFile,
            StreamCompressed,
        }

        public Stream GetStream() => _fileStream!;

        public void Open(OpenMode openMode)
        {
            if (_fileStream != null)
            {
                return;
            }

            if (!_romFile.IsCompressed())
            {
                _fileStream = File.OpenRead(_romFile.AbsolutePath());
            }
            else
            {
                _archiveStream?.Dispose();
                _archiveStream = File.OpenRead(_romFile.AbsolutePath());
                _archiveReader?.Dispose();
                _archiveReader = ZipArchive.Open(_archiveStream);

                var entry = _archiveReader.Entries.First();
                _crc32 = entry.Crc;

                _isTempFile = openMode == OpenMode.ExtractToTempFile;
                if (_isTempFile)
                {
                    _tempFolderPath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(_romFile.FilePath));
                    Directory.CreateDirectory(_tempFolderPath);

                    entry.WriteToDirectory(_tempFolderPath, new ExtractionOptions

                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    }
                    );

                    _fileStream = File.OpenRead(new DirectoryInfo(_tempFolderPath).GetFiles().First().FullName);
                }
                else
                {
                    _fileStream = entry.OpenEntryStream();
                }

            }
        }

        public void Close()
        {
            if (_isTempFile)
            {
                Directory.Delete(_tempFolderPath, true );
            }

            _fileStream?.Dispose();
            _fileStream = null;

            _archiveReader?.Dispose();
            _archiveReader = null;

            _archiveStream?.Dispose();
            _archiveStream = null;

        }

        public async Task<uint> GetCrc32()
        {
            Open(OpenMode.StreamCompressed);

            // If crc is not computed, compute it now.
            // NOTE: for compressed files, CRC is always computed
            if (_crc32 == -1)
            {
                var buffer = new byte[_fileStream!.Length];
                await _fileStream!.ReadExactlyAsync(buffer);
                _crc32 = Crc32.HashToUInt32(buffer);
            }

            Close();
            return (uint) _crc32;
        }

        private RomFile _romFile = rf;

        private Stream? _fileStream;
        private Stream? _archiveStream;
        private ZipArchive? _archiveReader;

        private bool _isTempFile;
        private string _tempFolderPath = "";
        private long _crc32 = -1;

        public void Dispose()
        {
            Close();
        }
    }
}
