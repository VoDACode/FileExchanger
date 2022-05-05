using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Zip
{
    public static class ZipCommands
    {
        public const string Create = "CREATE_ZIP";
        public const string StartAddItem = "S_ADD_ITEM";
        public const string EndAddItem = "E_ADD_ITEM";
        public const string Pack = "PACK";
        public const string Bye = "BYE";
        public const string SetName = "SET_NAME";
    }
}
