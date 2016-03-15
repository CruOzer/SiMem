using System;
using System.IO;
using Windows.Storage;

namespace SiMem.Database
{
    public static class DBStatics
    {
        public static String DB_NAME = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "mems.db"));//DataBase Name
        public static int DB_VERSION = 1;

    }
}
