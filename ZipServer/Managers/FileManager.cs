using Core.Context;
using System;

namespace ZipServer.Managers
{
    public class FileManager : FileManagetContext
    {
        private static FileManager instance;
        public static FileManager Instance => instance ?? (instance = new FileManager());
        public override string[] CreateDirAfretStart => new string[]
        {
            TempPath
        };
        public string TempPath => $"{BaseDirectory}tmp";
    }
}
