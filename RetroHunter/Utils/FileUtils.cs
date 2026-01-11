
using SharpCompress.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace RetroHunter.Utils;

public static class FileUtils
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

    public static void MoveToTrash(string fileName)
    {
        if (!File.Exists(fileName) && !Directory.Exists(fileName))
        {
            throw new Exception($"File not found at {fileName}");
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(
                fileName,
                Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("osascript", $"-e 'tell application \"Finder\" to delete POSIX file \"{fileName}\"'");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("gio", $"trash \"{fileName}\"");
        }
    }
}
