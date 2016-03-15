using System;
using SQLite;
using System.Collections.Generic;

namespace SiMem.Data
{
    /// <summary>
    /// Klasse für ein MemoryGroup-Objekt
    /// </summary>
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
        /// <summary>
        /// Id und PrimaryKey der MemoryGroup
        /// </summary>
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
        /// <summary>
        /// Titel der MemoryGroup
        /// </summary>
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
