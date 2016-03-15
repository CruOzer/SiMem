using SQLite;
using System;
using System.Collections.Generic;

namespace SiMem.Data
{
    /// <summary>
    /// Klasse für ein Memory-Objekt
    /// </summary>
    public class Memory
    {
        public Memory():this("Leerer Titel")
        {

        }
        public Memory(string _title)
        {
            Datum = DateTime.Now;
            Title = _title;
        }
        public Memory(int _id, int _groupId)
        {
            Datum = DateTime.Now;
            Id = _id;
            GroupId = _groupId;
        }
        public Memory(int _id, int _groupId, string _title)
        {
            Datum = DateTime.Now;
            Id = _id;
            GroupId = _groupId;
            Title = _title;
        }

        public Memory(int _id, int _groupId, string _title, string _text)
        {
            Datum = DateTime.Now;
            Id = _id;
            GroupId = _groupId;
            Title = _title;
            Text = _text;
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

        private int groupId;
        /// <summary>
        /// Foreign Key für eine MemoryGroup
        /// </summary>
        [NotNull]
        public int GroupId
        {
            get
            {
                return groupId;
            }

            set
            {
                if (value < 0)
                {
                    throw new FormatException("Die Gruppen-Id darf nicht leer sein");
                }
                else
                {
                    this.groupId = value;
                }
            }
        }
    }
}
