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
        /// <summary>
        /// Hilfsobjekt, um Daten in die Datenbank zu schreiben
        /// </summary>
        IDataSource<Memory> memoryDataSource;
        /// <summary>
        /// Standardkonstruktor instanziert nötige Objekte;
        /// </summary>
        public AddItemPage()
        {
            this.InitializeComponent();
            //Holt sich alle die Schnittstelle als Dependency Inject
            memoryDataSource = App.Container.Resolve<IDataSource<Memory>>();
        }
        /// <summary>
        /// Ergebnis des Dialogs
        /// </summary>
        public AddItemResult Result
        {
            get; private set;
        }

        public Memory Memory;
   
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
            Memory = new Memory();
            Memory.Id = memoryDataSource.GetMax() + 1;
            Memory.Text = textText.Text;
            Memory.Datum = DateTime.Now;
            Memory.Title = titleText.Text;
            //TODO löschen, ändern?
            Memory.GroupId = 1;
            //Speichern in die Datenbank
            memoryDataSource.Insert(Memory);
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
