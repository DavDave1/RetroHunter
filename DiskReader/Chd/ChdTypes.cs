
namespace DiskReader.Chd
{
    internal static class ChdConstants
    {
        public const int CHD_MD5_BYTES = 16;

        public const int CHD_OPEN_READ = 1;
        public const int CHD_OPEN_READ_WRITE = 2;


        public static uint CDROM_TRACK_METADATA_TAG = MakeTag('C', 'H', 'T', 'R');
        public static uint CDROM_TRACK_METADATA2_TAG = MakeTag('C', 'H', 'T', '2');

        public const int CHD_CD_SECTOR_DATA_SIZE = 2352 + 96;

        private static uint MakeTag(char c1, char c2, char c3, char c4)
        {
            return (uint)((c1 << 24) | (c2 << 16) | (c3 << 8) | c4);
        }

    }
    internal unsafe struct ChdHeader
    {
        UInt32 length;                        /* length of header data */
        UInt32 version;                   /* drive format version */
        UInt32 flags;                     /* flags field */

        fixed UInt32 compression[4];                /* compression type */
        public UInt32 hunkbytes;                 /* number of bytes per hunk */
        UInt32 totalhunks;                    /* total # of hunks represented */
        UInt64 logicalbytes;              /* logical size of the data */
        UInt64 metaoffset;                    /* offset in file of first metadata */
        UInt64 mapoffset;                 /* TOOD V5 */
        fixed byte md5[ChdConstants.CHD_MD5_BYTES];         /* overall MD5 checksum */
        fixed byte parentmd5[ChdConstants.CHD_MD5_BYTES];   /* overall MD5 checksum of parent */
        fixed byte sha1[ChdConstants.CHD_MD5_BYTES];       /* overall SHA1 checksum */
        fixed byte rawsha1[ChdConstants.CHD_MD5_BYTES];    /* SHA1 checksum of raw data */
        fixed byte parentsha1[ChdConstants.CHD_MD5_BYTES]; /* overall SHA1 checksum of parent */
        UInt32 unitbytes;                 /* TODO V5 */
        UInt64 unitcount;                 /* TODO V5 */
        UInt32 hunkcount;                  /* TODO V5 */

        /* map information */
        UInt32 mapentrybytes;              /* length of each entry in a map (V5) */
        UIntPtr rawmap;                   /* raw map data */

        UInt32 obsolete_cylinders;            /* obsolete field -- do not use! */
        UInt32 obsolete_sectors;          /* obsolete field -- do not use! */
        UInt32 obsolete_heads;                /* obsolete field -- do not use! */
        UInt32 obsolete_hunksize;         /* obsolete field -- do not use! */
    };
}
