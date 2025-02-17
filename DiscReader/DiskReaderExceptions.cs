
namespace DiskReader;

public class DiskReaderException(string message) : Exception(message) { }

public class UnsupportedFormatException(string message) : DiskReaderException(message)
{
}
