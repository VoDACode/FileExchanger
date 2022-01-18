using System;
using System.Text;
using System.Text.Json;
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
                result = $"{kb}Kb";
            else
                result = $"{b}b";
            return result.Replace(",", ".");
        }

        public static double SizeParser(string size)
        {
            var tmp = size.Split(' ');
            double result = 0;
            try
            {
                result = tmp[0].ToDouble();
                if (tmp[1] == "Gb")
                    return result * Math.Pow(1024, 3);
                if (tmp[1] == "Mb")
                    return result * Math.Pow(1024, 2);
                if (tmp[1] == "Kb")
                    return result * 1024;
                throw new Exception();
            }
            catch(Exception ex)
            {
                throw new ArgumentException($"Incorrect config parameter MaxSaveSize: '{tmp[0]}' (len: {tmp[0].Length}; {result})\n" +
                    $"Input: '{size}'\n" +
                    $"{JsonSerializer.Serialize(tmp)}\n------------------\n" +
                    $"Message: '{ex}'");
            }
        }

        public static double ToDouble(this string str)
        {
            if(str == null)
                throw new ArgumentNullException(nameof(str));
            double result = 0;
            string[] vs = str.Split(new char[]{ ',', '.' });
            for (int i = 0, k = vs[0].Length - 1; i < vs[0].Length; i++, k--)
                result += (int)(vs[0][i] - 48)*Math.Pow(10, k);
            for (int i = 0; i < vs[1].Length; i++)
                result += (int)(vs[1][i] - 48)/Math.Pow(10, i+1);
            return result;
        }
    }
}
