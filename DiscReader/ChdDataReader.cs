

using DiskReader.Chd;
using System;
using System.Runtime.Versioning;

namespace DiskReader
{


    public partial class ChdDataReader : IDiskDatakReader, IDisposable
    {
        public ChdDataReader(string filePath)
        {
            _chdFileName = filePath;

            var result = LibChdAccess.Open(filePath, ChdConstants.CHD_OPEN_READ, UIntPtr.Zero, ref _chd);

            if (result != 0)
            {
                throw new FileLoadException($"Chd reader failed to open file with error {result}", filePath);
            }

            _hunkSize = LibChdAccess.GetHunkSize(_chd);
            _sectorsPerHunk = _hunkSize / ChdConstants.CHD_CD_SECTOR_DATA_SIZE;
            _hunkBuffer = new byte[_hunkSize];

            ParseTrackList();
            OpenFirstTrack();
        }

        public bool Seek(uint lba)
        {
            uint disc_frame = _trackFileOffset + lba;
            uint hunk_index = disc_frame / _sectorsPerHunk;

            _currentHunkOffset = disc_frame % _sectorsPerHunk * ChdConstants.CHD_CD_SECTOR_DATA_SIZE;
            _currentLba = lba;

            if (_currentHunk != hunk_index)
            {
                var result = LibChdAccess.ReadHunk(_chd, hunk_index, _hunkBuffer);
                if (result != 0)
                {
                    return false;
                }

                _currentHunk = (int)hunk_index;
            }

            return true;
        }

        public bool SeekRelative(uint lba)
        {
            return Seek(lba);
        }

        public bool Read(byte[] buffer)
        {
            int readLen = 0;
            uint nextLba = _currentLba;
            while (readLen < buffer.Length)
            {
                var sizeToRead = Math.Min(2048, buffer.Length - readLen);

                var hunkStart = (int)_currentHunkOffset + _tracks[_trackIndex].SectorHeaderSize;
                var hunkEnd = (int)(hunkStart + sizeToRead);

                _hunkBuffer[hunkStart..hunkEnd].CopyTo(buffer, readLen);

                readLen += sizeToRead;
                nextLba++;
                if (!Seek(nextLba))
                {
                    return false;
                }
            }

            return true;
        }
        public List<string> GetAllTrackFiles() => [_chdFileName];

        public void Dispose()
        {
            if (_chd != UIntPtr.Zero)
            {
                int result = LibChdAccess.Close(_chd);
                _chd = UIntPtr.Zero;

                if (result != 0)
                {
                    throw new DiskReaderException($"Failed to close CHD file with error {result}");
                }
            }
        }

        private void ParseTrackList()
        {
            uint trackIndex = 0;
            while (true)
            {
                var track = LibChdAccess.GetMetadata(_chd, trackIndex);
                if (track == null)
                {
                    break;
                }

                _tracks.Add(track);

                trackIndex++;
            }
        }

        public bool OpenFirstTrack()
        {
            _trackIndex = -1;
            _trackFileOffset = 0;

            foreach (var track in _tracks)
            {
                _trackIndex++;

                if (track.TrackType != Track.ETrackType.Audio)
                {
                    break;
                }

                _trackFileOffset += (uint)track.TrackSize;
            }

            return _trackIndex >= 0 && _trackIndex < _tracks.Count;
        }

        public bool OpenNextTrack()
        {
            _trackIndex++;

            for (; _trackIndex < _tracks.Count; _trackIndex++)
            {
                _trackFileOffset += (uint)_tracks[_trackIndex].TrackSize;

                if (_tracks[_trackIndex].TrackType != Track.ETrackType.Audio)
                {
                    return true;
                }
            }

            return false;
        }

        private readonly string _chdFileName;
        private UIntPtr _chd = UIntPtr.Zero;
        private readonly List<Track> _tracks = [];

        private uint _hunkSize;
        private uint _sectorsPerHunk;
        private int _currentHunk = -1;
        private uint _currentLba;
        private uint _currentHunkOffset;
        private byte[] _hunkBuffer = [];

        private int _trackIndex;
        private uint _trackFileOffset;
    }
}
