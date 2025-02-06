
using DiscUtils.Streams;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;

namespace DiskReader.Chd
{

    internal static partial class LibChdAccess
    {

#if Linux
        private const string LibChdName = "libchdr.so.0.2";
#elif Windows
        private const string LibChdName = "chdr.dll";
#endif


        [LibraryImport(LibChdName, EntryPoint = "chd_open", StringMarshalling = StringMarshalling.Utf8)]
        internal static partial int Open(string filename, int mode, UIntPtr parent, ref UIntPtr chd_file);

        [LibraryImport(LibChdName, EntryPoint = "chd_close")]
        internal static partial int Close(UIntPtr chd_file);

        [LibraryImport(LibChdName, EntryPoint = "chd_read")]
        internal static partial int ReadHunk(UIntPtr chd_file, uint hunknum, byte[] buffer);

        internal static uint GetHunkSize(UIntPtr chd_file)
        {
            ChdHeader header = GetHeader(chd_file);
            return header.hunkbytes;
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

        [LibraryImport(LibChdName, EntryPoint = "chd_get_metadata")]
        private static partial int GetMetadataInternal(UIntPtr chd_file, UInt32 searchtag, UInt32 searchindex, byte[] output, UInt32 outputLen, ref UInt32 resultLen, UIntPtr resultTag, UIntPtr resultflags);

        [DllImport(LibChdName, EntryPoint = "chd_get_header")]
        private static extern ref ChdHeader GetHeader(UIntPtr chd_file);

    }
}
