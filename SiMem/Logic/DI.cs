using Autofac;
using SiMem.Data;
using SiMem.Database;
using SiMem.DataModel;

namespace SiMem.Logic
{
    class DI
    {

        public static IContainer Container;
        private static bool initialized = false;
        /// <summary>
        /// Instanziert den Dependency Inject Container und beginnt die Datenbankconnection
        /// </summary>
        public static void initializeContainer()
        {
            if (!initialized)
            {
                ContainerBuilder builder = new ContainerBuilder();
                builder.RegisterType<DBConnection>().As<IDBConnection>();
                builder.RegisterType<MemoryDataSource>().As<IDataSource<Memory>>();
                builder.RegisterType<SiMemTileFactory>().As<ISiMemTileFactory>();
                Container = builder.Build();
                IDBConnection dbConn = Container.Resolve<IDBConnection>();
                dbConn.onAppStart();
                initialized = true;
            }
        }
    }
}
