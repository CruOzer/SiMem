using System;
using System.IO;
using Windows.Storage;

namespace SiMem.Database
{
    public static class DBStatics
    {
        /// <summary>
        /// DataBase Name
        /// </summary>
        public static String DB_NAME = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "simem" + DB_VERSION + ".db"));
        /// <summary>
        /// DatenbankVersion
        /// </summary>
        public static int DB_VERSION = 1;
    }
}
