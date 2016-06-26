using SQLite;
using System;

namespace SiMem.Data
{
    /// <summary>
    /// Klasse für ein Memory-Objekt
    /// </summary>
    public class Memory : IComparable
    {
        /// <summary>
        /// Constructor Instanziiert den Standard Typ mit leerem String  mit Datum = aktuelle Zeit
        /// </summary>
        public Memory()
        {
            this.Id = 0;
            this.MemoryType = 1;
            this.Title = "";
            this.Text = "";            
            this.Datum = DateTime.Now;
        }

        /// <summary>
        /// Kopierkonstruktor
        /// </summary>
        /// <param name="memory">Zu kopierende Objekt</param>
        public Memory(Memory memory)
        {
            this.Id = memory.Id;
            this.MemoryType = memory.MemoryType;
            this.Datum = memory.Datum;
            this.Text = memory.Text;
            this.Title= memory.Title;
        }

        private int id;
        /// <summary>
        /// Id und PrimaryKey der Memory
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
                this.id = value;
            }
        }
        private DateTime datum;
        /// <summary>
        /// Zeitpunkt der Memory (Defaultmäßig das Erstellungsdatum)
        /// </summary>
        [NotNull]
        public DateTime Datum
        {
            get
            {
                return datum;
            }

            set
            {
                this.datum = value;
            }
        }
        private string title;
        /// <summary>
        /// Titel der Memory
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
                this.title = value;
            }
        }

        private string text;
        /// <summary>
        /// Inhalt der Memory
        /// </summary>
        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                this.text = value;
            }
        }

        private int memoryType;
        /// <summary>
        /// MemoryType der Memory (Default ist STANDARD)
        /// </summary>
        [NotNull]
        public int MemoryType
        {
            get
            {
                return memoryType;
            }

            set
            {
                if (value <= 0 )
                {
                    throw new FormatException("Der MemoryType ist falsch");
                }
                else
                {
                    this.memoryType = value;
                }
            }
        }
        /// <summary>
        /// Vergleicht die Ids zwischen den beiden Memories Kleiner 0: This Instanz ist früher; Gleich 0: This Instanz ist gleich; Größer 0: This Istance ist später, das Objekt ist null oder kein Memory
        /// </summary>
        /// <param name="obj">Zu vergleichendeMemory übergeben</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if(obj == null)
                    return 1;

            Memory objAsMemory = obj as Memory;
            if (objAsMemory == null)
                return 1;

            else
                return this.Id.CompareTo(objAsMemory.Id);
        }
    }
}