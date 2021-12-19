using System;
using System.Text;
using System.Text.RegularExpressions;

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

        public static double SizeParser(string size)
        {
            Regex gb = new Regex(@"\dGb");
            Regex mb = new Regex(@"\dMb");
            Regex kb = new Regex(@"\dKb");
            double result = 0;
            StringBuilder strNum = new StringBuilder(size.Length - 2);
            for(int i = 0; i < strNum.Capacity; i++)
                strNum.Append(size[i]);
            result = double.Parse(strNum.ToString());
            if(gb.IsMatch(size))
                return result * Math.Pow(1024, 3);
            if (mb.IsMatch(size))
                return result * Math.Pow(1024, 2);
            if (kb.IsMatch(size))
                return result * 1024;
            throw new Exception("Incorrect config parameter 'MaxSaveSize'");
        }
    }
}
