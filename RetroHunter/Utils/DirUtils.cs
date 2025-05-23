
using System.IO;

namespace RetroHunter.Utils;

public static class DirUtils
{
    public static string UniquePath(string path)
    {
        int counter = 0;
        while (Path.Exists(path))
        {
            path = $"{path} ({counter++})";
        }

        return path;
    }
}
