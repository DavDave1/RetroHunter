using System.Text;

namespace DiskReader.IsoFS
{
    public static class Constants
    {
        public const int DESCRIPTORS_START_ADDR = 0x10;
        public const int PRIMARY_VOLUME_ID = 0x01;
        public static readonly byte[] STANDARD_ID = [0x43, 0x44, 0x30, 0x30, 0x31]; // CD001 

        public static int Int32LsbMsbToInt(byte[] buffer)
        {
            return BitConverter.IsLittleEndian ?
                BitConverter.ToInt32(buffer[0..4], 0) :
                BitConverter.ToInt32(buffer[4..8], 0);
        }
        public static int Int16LsbMsbToInt(byte[] buffer)
        {
            return BitConverter.IsLittleEndian ?
                BitConverter.ToInt16(buffer[0..2], 0) :
                BitConverter.ToInt16(buffer[2..4], 0);
        }
    }
    public class VolumeDescriptor
    {
        public byte Type;
        public string Identifier = "";
        public byte Version;
    }

    public class PrimaryVolume : VolumeDescriptor
    {
        public string SystemIdentifier = "";
        public string VolumeIdentifier = "";
        public int VolumeSpaceSize;
        public int VolumeSetSize;
        public int VolumeSequenceNumber;
        public int LogicalBlockSize;
        public int PathTableSize;
        public int PathTableLocation;
        public int OptPathTableLocation;
        public int PathTableBELbaLocation;
        public int OptPathTableBELbaLocation;
        public DirectoryRecord RootDirectory;

        public PrimaryVolume(byte[] buffer)
        {
            Type = buffer[0];
            Identifier = Encoding.UTF8.GetString(buffer[1..6]);
            Version = buffer[6];
            SystemIdentifier = Encoding.UTF8.GetString(buffer[8..40]).TrimEnd();
            VolumeIdentifier = Encoding.UTF8.GetString(buffer[40..72]).TrimEnd();
            VolumeSpaceSize = Constants.Int32LsbMsbToInt(buffer[80..88]);
            VolumeSetSize = Constants.Int16LsbMsbToInt(buffer[120..124]);
            VolumeSequenceNumber = Constants.Int16LsbMsbToInt(buffer[124..128]);
            LogicalBlockSize = Constants.Int16LsbMsbToInt(buffer[128..132]);
            PathTableSize = Constants.Int32LsbMsbToInt(buffer[132..140]);
            PathTableLocation = BitConverter.IsLittleEndian ?
                BitConverter.ToInt32(buffer[140..144], 0) :
                BitConverter.ToInt32(buffer[144..148], 0);
            OptPathTableLocation = BitConverter.IsLittleEndian ?
                BitConverter.ToInt32(buffer[148..152], 0) :
                BitConverter.ToInt32(buffer[152..156], 0);
            RootDirectory = new(buffer[156..190]);
        }


    }

    public class DirectoryRecord
    {
        public byte RecordLength;
        public byte ExtendedAttributeRecordLength;
        public uint Location;
        public int DataLength;
        public byte InterleavedFileUnitSize;
        public byte InterleaveGapSize;
        public int VolumeSequenceNumber;
        public byte FileNameLength;
        public string FileName;

        public DirectoryRecord(byte[] buffer)
        {
            RecordLength = buffer[0];
            ExtendedAttributeRecordLength = buffer[1];
            Location = (uint)Constants.Int32LsbMsbToInt(buffer[2..10]);
            DataLength = Constants.Int32LsbMsbToInt(buffer[10..18]);
            // TODO: parse date and time
            InterleavedFileUnitSize = buffer[26];
            InterleaveGapSize = buffer[27];
            VolumeSequenceNumber = Constants.Int16LsbMsbToInt(buffer[28..32]);
            FileNameLength = buffer[32];

            int fileNameBufferEnd = 33 + FileNameLength;
            FileName = Encoding.UTF8.GetString(buffer[33..fileNameBufferEnd]).Replace(";1", "");
        }
    }
}
