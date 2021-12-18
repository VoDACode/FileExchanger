using System;

namespace FileExchanger.Helpers
{
    public static class SizeHelper
    {
        public static string ByteSizeToString(this double size)
        {
            double b = size;
            double kb = b / 1024;
            b %= 1024;
            double mb = kb / 1024;
            kb %= 1024;
            double gb = mb / 1024;
            mb %= 1024;
            string result = "";
            gb = Math.Round(gb, 2);
            mb = Math.Round(mb, 2);
            kb = Math.Round(kb, 2);
            b = Math.Round(b, 2);
            if (gb > 0)
                result = $"{gb}Gb";
            else if (mb > 0)
                result = $"{mb}Mb";
            else if (kb > 0)
                result = $"{kb}Mb";
            else
                result = $"{b}b";
            return result.Replace(",", ".");
        }
    }
}
