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

            foreach (var track in _cue.Tracks)
            {
                track.TrackSize = (int)GetBinLength(track.FilePath);
            }

            var largestTrack = _cue.Tracks.Where(t => t.TrackType != Track.ETrackType.Audio).OrderBy(t => t.TrackSize).Last();

            var largestTrackIndex = _cue.Tracks.IndexOf(largestTrack);

            return OpenTrack(0);
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


            var lbaInTrack = lba - _cue.Tracks[_trackIndex].TrackSectorBegin;

            var position = (lbaInTrack * _sectorSize) + _sectorHeaderSize;

            if (_currentPosition == position)
            {
                return true;
            }

            if (position >= _trackSize)
            {
                OpenTrack(_trackIndex + 1);
                return Seek(lba);
            }

            _currentLba = lba;
            _currentPosition = position;
            _positionInSector = 0;
            _track.Seek(_currentPosition, SeekOrigin.Begin);

            return true;
        }

        public bool SeekRelative(uint lba)
        {
            return Seek(lba + (uint)_cue.Tracks[_trackIndex].TrackSectorBegin);
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

        public void OpenFirstTrack()
        {
            var firstTrack = _cue.Tracks.FindIndex(t => t.TrackType != Track.ETrackType.Audio);

            OpenTrack(firstTrack);
        }

        public bool OpenNextTrack()
        {
            int nextTrack = _trackIndex + 1;

            while (nextTrack < _cue.Tracks.Count && _cue.Tracks[nextTrack].TrackType == Track.ETrackType.Audio)
            {
                nextTrack++;
            }

            return OpenTrack(nextTrack);
        }

        private bool OpenTrack(int trackIndex)
        {
            if (trackIndex >= _cue.Tracks.Count)
            {
                return false;
            }

            _trackIndex = trackIndex;
            _track = GetBin(_cue.Tracks[_trackIndex].FilePath);
            _trackSize = (int)GetBinLength(_cue.Tracks[_trackIndex].FilePath);
            _sectorSize = _cue.Tracks[_trackIndex].SectorSize;
            _sectorHeaderSize = _cue.Tracks[_trackIndex].SectorHeaderSize;

            return SeekRelative(0);
        }

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
