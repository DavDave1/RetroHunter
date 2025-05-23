﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RetroHunter.Utils
{
    public static class DirectoryInfoExtensions
    {
        public static bool IsEmptyRecursive(this DirectoryInfo dirInfo)
        {
            var files = dirInfo.GetFiles();
            var dirs = dirInfo.GetDirectories();
            return files.Length == 0 && (dirs.Length == 0 || dirs.All(d => d.IsEmptyRecursive()));
        }

        public static List<string> GetAllFilesRecursive(this DirectoryInfo dirInfo)
        {
            return dirInfo
                .GetFiles()
                .Select(f => f.FullName)
                .Concat(
                    dirInfo
                    .GetDirectories()
                    .SelectMany(d => d.GetAllFilesRecursive()))
                .ToList();
        }

        public static void CleanEmptySubdirs(this DirectoryInfo dirInfo)
        {
            foreach (var inSubdir in dirInfo.GetDirectories())
            {
                if (inSubdir.IsEmptyRecursive())
                {
                    inSubdir.Delete(true);
                }
            }
        }
    }
}
