using Core.Zip;
using System.Collections.Generic;

namespace ZipServer
{
    public sealed class Storage
    {
        private static Storage instance;
        public static Storage Instance => instance ?? (instance = new Storage());
        
        private Storage()
        {

        }

        public Dictionary<string, ZipContent> ZipStorage { get; } = new Dictionary<string, ZipContent>(10);
    }
}
