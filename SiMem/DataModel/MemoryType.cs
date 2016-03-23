using System;
using Windows.ApplicationModel.Resources;

namespace SiMem.DataModel
{
    /// <summary>
    /// Contains the different types of memories
    /// </summary>
    public class MemoryType
    {
        public const int MemoryTypeLength = 3;
        /// <summary>
        /// Default type
        /// </summary>
        public static int STANDARD = 1;
        /// <summary>
        /// Important memories
        /// </summary>
        public static int IMPORTANT = 2;
        /// <summary>
        /// Shopping related memories
        /// </summary>
        public static int SHOPPING = 3;

        private static readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        private static string[] memoryTypeName = new string[]{
        String.Empty,
        resourceLoader.GetString("MemoryTypeStandard"),
        resourceLoader.GetString("MemoryTypeImportant"),
        resourceLoader.GetString("MemoryTypeShopping")
    };

       /// <summary>
       /// Get the String of the MemoryType
       /// </summary>
       /// <param name="memoryType">MemoryType</param>
       /// <returns>The name of the type </returns>
        public static string getMemoryTypeName(int memoryType)
        {
            return memoryTypeName[memoryType];
        }
    }
}
