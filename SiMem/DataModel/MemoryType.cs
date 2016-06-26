using Windows.ApplicationModel.Resources;

namespace SiMem.Data
{

    /// <summary>
    /// Contains the different types of memories
    /// </summary>
    public class MemoryType
    {
        public MemoryType(MemoryType memType)
        {
            this.Id = memType.Id;
            this.Name = memType.Name;
        }
        public MemoryType() : this(0, "Category"){}

        public MemoryType(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        private int id;
        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                if (value >= 0)
                {
                    this.id = value;
                }
                else
                {
                    throw new System.FormatException("Id must be over 0");
                }
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                if (value != "")
                {
                    this.name = value;
                }
                else
                {
                    this.name = "Category";
                }
            }
        }

        private string name;




        //-----------------ALT----------------
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
