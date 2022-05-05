using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Core.Zip
{
    public abstract class ZipContext
    {
        protected string _key;
        public string Key => _key;
        public abstract Task<ZipContext> Create();
        public abstract Task<ZipContext> AddRage(List<ZipItem> items);
        public abstract Task<ZipContext> Add(ZipItem item);
        public abstract Task<ZipContext> Pack();
        public abstract Stream GetStream();
    }
}
