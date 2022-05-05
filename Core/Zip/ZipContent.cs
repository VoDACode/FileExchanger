using System;
using System.Collections.Generic;

namespace Core.Zip
{
    public class ZipContent
    {
        public DateTime CreateDate { get; }
        public List<ZipItem> Content { get; } = new List<ZipItem>();
        public string Name { get; set; }
        public bool Done { get; set; } = false;
        public ZipContent()
        {
            CreateDate = DateTime.Now;
        }
    }
}
