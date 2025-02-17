namespace DiskReader
{
    internal class BinCueDataReader : IDiskDatakReader
    {
        public BinCueDataReader(string filePath)
        {

            _cue = new(filePath);

            if (!OpenFirstTrack())
            {
                throw new UnsupportedFormatException($"{filePath} is not a valid cue file");
            }
        }
        public List<string> GetAllTrackFiles() => _cue.GetAllFiles();

        public bool Seek(uint lba)
        {
            if (_track == null)
            {
                return false;
            }


            var lbaInTrack = lba - _cue.CurrentTrack().TrackSectorBegin;

            var position = (lbaInTrack * _cue.CurrentTrack().SectorSize) + _cue.CurrentTrack().SectorHeaderSize;

            if (_currentPosition == position)
            {
                return true;
            }

            if (position >= _cue.CurrentTrack().TrackSize)
            {
                return OpenNextTrack() && Seek(lba);
            }

            _currentLba = lba;
            _currentPosition = position;
            _positionInSector = 0;
            _track.Seek(_currentPosition, SeekOrigin.Begin);

            return true;
        }

        public bool SeekRelative(uint lba)
        {
            return Seek(lba + (uint)_cue.CurrentTrack().TrackSectorBegin);
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


        public bool OpenFirstTrack()
        {
            return _cue.SelectFirstTrack() && OpenTrack();
        }

        public bool OpenNextTrack()
        {
            return _cue.SelectNextTrack() && OpenTrack();
        }

        private bool OpenTrack()
        {
            _track = _cue.OpenTrack();
            return SeekRelative(0);
        }

        private Stream? _track;
        private readonly Cue _cue;

        private long _currentPosition;
        private uint _currentLba;
        private uint _positionInSector;
    }
}
