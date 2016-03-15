using System;
using SQLite;
namespace SiMem.DataModel
{
    public class MemoryGroup
    {
        public MemoryGroup():this("Leerer Titel")
        {

        }
        public MemoryGroup(string _title)
        {
            Title = _title;
        }
        public MemoryGroup(int _id, string _title)
        {
            Id = _id;
            Title = _title;
        }

        private int id;
        [PrimaryKey,NotNull]
        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                if (value < 0)
                {
                    throw new FormatException("Die Gruppen-Id darf nicht leer sein");
                }
                else
                {
                    this.id = value;
                }
                
            }
        }
        private string title;
        [NotNull]
        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                if (value == null || value == "")
                {
                    throw new FormatException("Der Gruppentitel darf nicht leer sein");
                }
                else
                {
                    this.title = value;
                }
            }
        }
    }
}
