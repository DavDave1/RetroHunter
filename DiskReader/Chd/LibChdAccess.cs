
using DiskReader.BinCue;
using System.Runtime.InteropServices;
using System.Text;
using static DiskReader.BinCue.Track;

namespace DiskReader.Chd
{

    internal static partial class LibChdAccess
    {
        [LibraryImport("chdr", EntryPoint = "chd_open", StringMarshalling = StringMarshalling.Utf8)]
        internal static partial int Open(string filename, int mode, UIntPtr parent, ref UIntPtr chd_file);

        [LibraryImport("chdr", EntryPoint = "chd_close")]
        internal static partial void Close(UIntPtr chd_file);

        [LibraryImport("chdr", EntryPoint = "chd_read")]
        internal static partial int ReadHunk(UIntPtr chd_file, uint hunknum, byte[] buffer);

        internal static uint GetHunkSize(UIntPtr chd_file)
        {
            ChdHeader header = GetHeader(chd_file);
            return header.hunkbytes;
        }

        internal static bool IsDvd(UIntPtr chd_file)
        {
            byte[] metadataStr = new byte[256];
            UInt32 resultLen = 0;

            return GetMetadataInternal(chd_file, ChdConstants.DVD_METADATA_TAG, 0, metadataStr, (uint)metadataStr.Length, ref resultLen, UIntPtr.Zero, UIntPtr.Zero) == 0;
        }

        internal static Track? GetMetadata(UIntPtr chd_file, uint tack_index)
        {
            byte[] metadataStr = new byte[256];
            UInt32 resultLen = 0;

            int result = GetMetadataInternal(chd_file, ChdConstants.CDROM_TRACK_METADATA2_TAG, tack_index, metadataStr, (uint)metadataStr.Length, ref resultLen, UIntPtr.Zero, UIntPtr.Zero);

            // Failed, try with V1 metadata
            if (result != 0)
            {
                result = GetMetadataInternal(chd_file, ChdConstants.CDROM_TRACK_METADATA_TAG, tack_index, metadataStr, (uint)metadataStr.Length, ref resultLen, UIntPtr.Zero, UIntPtr.Zero);
            }

            // Failed, is this a GDROM
            if (result != 0)
            {
                result = GetMetadataInternal(chd_file, ChdConstants.GDROM_TRACK_METADATA_TAG, tack_index, metadataStr, (uint)metadataStr.Length, ref resultLen, UIntPtr.Zero, UIntPtr.Zero);
            }

            // Failed, is this a DVD
            if (result != 0)
            {
                result = GetMetadataInternal(chd_file, ChdConstants.DVD_METADATA_TAG, tack_index, metadataStr, (uint)metadataStr.Length, ref resultLen, UIntPtr.Zero, UIntPtr.Zero);
                if (result == 0)
                {
                    ChdHeader header = GetHeader(chd_file);
                    return new()
                    {
                        TrackNr = 0,
                        TrackType = ETrackType.Mode1,
                        TrackSize = (int)(header.hunkbytes * header.totalhunks),
                        SectorSize = (int)header.hunkbytes,
                        SessionNr = 1,
                    };
                }
            }

            if (result != 0)
            {
                return null;
            }

            var metadata = Encoding.UTF8.GetString(metadataStr[..(int)resultLen]).Split(' ').ToList();

            return new()
            {
                TrackNr = int.Parse(metadata[0].Replace("TRACK:", "")),
                TrackType = TypeStringToTrackMode(metadata[1].Replace("TYPE:", "")),
                TrackSize = int.Parse(metadata[3].Replace("FRAMES:", "")),
                SectorSize = ChdConstants.CHD_CD_SECTOR_DATA_SIZE,
                SessionNr = 1,
            };
        }


        private static Track.ETrackType TypeStringToTrackMode(string type)
        {
            return type switch
            {
                "MODE1_RAW" => Track.ETrackType.Mode1Raw,
                "MODE1" => Track.ETrackType.Mode1,
                "MODE2" => Track.ETrackType.Mode2,
                "MODE2_RAW" => Track.ETrackType.Mode2Raw,
                "MODE2_FORM1" => Track.ETrackType.Mode2Form1,
                "MODE2_FORM2" => Track.ETrackType.Mode2Form2,
                "MODE2_FORM_MIX" => Track.ETrackType.Mode2FormMix,
                "AUDIO" => Track.ETrackType.Audio,
                _ => throw new Exception($"Unknown track type {type}"),
            };
        }

        [LibraryImport("chdr", EntryPoint = "chd_get_metadata")]
        private static partial int GetMetadataInternal(UIntPtr chd_file, UInt32 searchtag, UInt32 searchindex, byte[] output, UInt32 outputLen, ref UInt32 resultLen, UIntPtr resultTag, UIntPtr resultflags);

        [DllImport("chdr", EntryPoint = "chd_get_header")]
        private static extern ref ChdHeader GetHeader(UIntPtr chd_file);

    }
}
