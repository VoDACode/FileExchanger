using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Core.Context
{
    public abstract class FileManagetContext
    {
        public string BaseDirectory => AppContext.BaseDirectory;
        public virtual string[] CreateDirAfretStart { get; } = Array.Empty<string>();
        public FileManagetContext()
        {
            void createDir(string path)
            {
                if (Directory.Exists(path))
                    return;
                var pathParts = path.Split('\\').ToList();
                pathParts.RemoveAt(pathParts.Count - 1);
                var rootPath = Path.Combine(pathParts.ToArray());
                if (!Directory.Exists(rootPath))
                    createDir(rootPath);
                Directory.CreateDirectory(path);
            }
            foreach(var dir in CreateDirAfretStart)
                createDir(dir);
        }
    }
}
