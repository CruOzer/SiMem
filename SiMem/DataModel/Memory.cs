using SQLite;
using System;


namespace SiMem.DataModel
{
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
