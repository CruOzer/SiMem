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
        public const int STANDARD = 0;
        /// <summary>
        /// Important memories
        /// </summary>
        public const int IMPORTANT = 1;
        /// <summary>
        /// Shopping related memories
        /// </summary>
        public const int OTHER = 2;

        private static readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        private static string[] memoryTypeName = new string[]{
        resourceLoader.GetString("MemoryTypeStandard"),
        resourceLoader.GetString("MemoryTypeImportant"),
        resourceLoader.GetString("MemoryTypeOther")
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
