
using System;
using System.IO;
using System.Runtime.InteropServices;

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

    public static string FindTool(string toolName)
    {
        string? pathEnv = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrEmpty(pathEnv))
        {
            return string.Empty;
        }

        string[] paths = pathEnv.Split(Path.PathSeparator);

        string executableName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? toolName + ".exe" : toolName;

        foreach (string path in paths)
        {
            string fullPath = Path.Combine(path, executableName);

            if (File.Exists(fullPath))
            {
               return fullPath;
            }
        }

        return string.Empty;
    }
}
