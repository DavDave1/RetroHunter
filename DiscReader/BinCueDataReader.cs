namespace DiskReader
{
    internal class BinCueDataReader : IDiskDatakReader
    {
        public bool Load(string FilePath)
        {
            _cueFile = new FileInfo(FilePath);
            using var cueStream = _cueFile.OpenRead();

            if (cueStream == null)
            {
                return false;
            }

            bool cueOK = _cue.Parse(cueStream);

            if (!cueOK)
            {
                return false;
            }

            _track = GetBin(_cue.Tracks[_trackIndex].FilePath);
            _trackSize = (int)GetBinLength(_cue.Tracks[_trackIndex].FilePath);

            if (_track == null)
            {
                return false;
            }

            _sectorSize = _cue.Tracks[_trackIndex].SectorSize;
            _sectorHeaderSize = _cue.Tracks[_trackIndex].SectorHeaderSize;

            Seek(0);

            return true;
        }

        public List<string> GetAllTrackFiles()
        {
            return [_cueFile?.FullName, .. _cue.Tracks.Select(t => GetBinAbsolutePath(t.FilePath))];
        }

        public bool Seek(uint lba)
        {
            if (_track == null)
            {
                return false;
            }

            // TODO: seek to next track. Is this even necessary for the purpose of 
            // computing MD5 hash? For PS1 games, it looks only first track contains data.
            if (_currentPosition >= _trackSize)
            {
                return false;
            }

            var position = (lba * _sectorSize) + _sectorHeaderSize;

            if (_currentPosition == position)
            {
                return true;
            }

            _currentLba = lba;
            _currentPosition = position;
            _positionInSector = 0;
            _track.Seek(_currentPosition, SeekOrigin.Begin);

            return true;
        }

        public bool Read(byte[] buffer)
        {
            if (_track == null)
            {
                return false;
            }

            int readLen = 0;
            var nextLba = _currentLba;
            while (readLen < buffer.Length)
            {
                var sizeToRead = (int)Math.Min(2048 - _positionInSector, buffer.Length - readLen);

                _track.ReadExactly(buffer, readLen, sizeToRead);
                readLen += sizeToRead;

                _positionInSector += (uint)sizeToRead;

                if (_positionInSector >= 2048)
                {
                    ++nextLba;
                    _positionInSector = 0;
                }

                if (!Seek(nextLba))
                {
                    return false;
                }
            }

            return true;
        }

        private FileStream? GetBin(string fileName)
        {
            if (_cueFile == null)
            {
                return null;
            }

            return new FileInfo(GetBinAbsolutePath(fileName)).OpenRead();
        }

        private long GetBinLength(string fileName) => new FileInfo(GetBinAbsolutePath(fileName)).Length;

        private string GetBinAbsolutePath(string fileName) => $"{_cueFile?.Directory?.FullName}/{fileName}";

        private Stream? _track;
        private int _trackIndex;
        private int _trackSize;
        private int _sectorSize;
        private int _sectorHeaderSize;

        private FileInfo? _cueFile;
        private readonly Cue _cue = new();

        private long _currentPosition;
        private uint _currentLba;

        private uint _positionInSector;
    }
}
