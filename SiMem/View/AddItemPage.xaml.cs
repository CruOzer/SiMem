using SiMem.Data;
using SiMem.DataModel;
using Windows.UI.Xaml.Controls;
using Autofac;
using System;
// Die Elementvorlage "Inhaltsdialog" ist unter "http://go.microsoft.com/fwlink/?LinkID=390556" dokumentiert.

namespace SiMem.View
{
    /// <summary>
    /// Ergebniswerte des Dialogs
    /// </summary>
    public enum AddItemResult
    {
        AddItemOK,
        AddItemCancel
    }
    /// <summary>
    /// Dialog zum erstellen von neuen Memories
    /// </summary>
    public sealed partial class AddItemPage : ContentDialog
    {
        public static int EDIT_MODE = 0;
        public static int ADD_MODE = 1;
        /// <summary>
        /// Hilfsobjekt, um Daten in die Datenbank zu schreiben
        /// </summary>
        IDataSource<Memory> memoryDataSource;
        private int mode;
        /// <summary>
        /// Ergebnis des Dialogs
        /// </summary>
        public AddItemResult Result
        {
            get; private set;
        }

        private Memory memory;
        /// <summary>
        /// Standardkonstruktor mit dem Modus ADD
        /// </summary>
        public AddItemPage() : this(ADD_MODE, new Memory())
        {
        }
        /// <summary>
        /// Konstuktor mit explizitem Modus und der übergabe eines Memories (Standardmäßiger Modus ist Add)
        /// </summary>
        /// <param name="mode">Dialog Modus</param>
        /// <param name="memory">Memory Object was geladen wird</param>
        public AddItemPage(int mode, Memory memory)
        {
            this.mode = mode;
            this.memory = memory;
            this.InitializeComponent();
            //Holt sich alle die Schnittstelle als Dependency Inject
            memoryDataSource = App.Container.Resolve<IDataSource<Memory>>();
            //Sofern der Editmodus aktiviert ist, wird das Objekt schon geladen
            if (this.mode == EDIT_MODE)
            {
                initializeTextBlocks();
            }            
        }

        /// <summary>
        /// Laden der Memory in die Textfelder
        /// </summary>
        private void initializeTextBlocks()
        {
            titleText.Text = memory.Title;
            textText.Text = memory.Text;
        }

      
   
        /// <summary>
        /// Der PrimaryButton steht für Speichern. Sind die Felder validiert wird die Memory gespeichert und der Dialog schließt sich
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //Validierung der Felder
            if (string.IsNullOrEmpty(textText.Text))
            {
                return;
            }
            if (string.IsNullOrEmpty(titleText.Text))
            {
                return;
            }
            //Speichern der aktuellen Daten
            memory.Id = memoryDataSource.GetMax() + 1;
            memory.Text = textText.Text;
            memory.Datum = DateTime.Now;
            memory.Title = titleText.Text;
            //TODO löschen, ändern?
            memory.GroupId = 1;
            //Speichern in die Datenbank
            memoryDataSource.Insert(memory);
            //Setzen des Ergbenisses
            this.Result = AddItemResult.AddItemOK;
            //Schließen des Dialogs
            this.Hide();
        }
        /// <summary>
        /// Der SecondaryButton steht für Schließen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //Setzen des Ergbenisses
            this.Result = AddItemResult.AddItemCancel;
            //Schließen des Dialogs
            this.Hide();
        }
    }
}
