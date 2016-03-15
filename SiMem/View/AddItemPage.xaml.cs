using SiMem.Data;
using SiMem.DataModel;
using Windows.UI.Xaml.Controls;
using Autofac;
using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using System.Diagnostics;
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
            // Subscribe keyboardTimer Tick event
            keyboardTimer.Tick += keyboardTimer_Tick;

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
            memory.Text = textText.Text;
            memory.Title = titleText.Text;
            if (mode == ADD_MODE)
            {
                memory.Id = memoryDataSource.GetMax() + 1;
                memory.Datum = DateTime.Now;
                //TODO löschen, ändern?
                memory.GroupId = 1;
                //Einfügen in die Datenbank
                memoryDataSource.Insert(memory);
            }
            else
            {
                //Update der Datenbank
                memoryDataSource.Update(memory);
            }
            
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
        /*---------------------------------------------------------------- TASTATUR STUFF ----------------------------------------------------------------
        */
        // Handle InputPane manually so the UI doesn't scroll when the keyboard appears
        InputPane inputPane = InputPane.GetForCurrentView();
        // DispatcherTimer to ChangeView() of ScrollViewer
        DispatcherTimer keyboardTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };

        private void textText_GotFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Subscribe InputPane events to handle UI scrolling
            inputPane.Showing += this.InputPaneShowing;
            inputPane.Hiding += this.InputPaneHiding;
        }
        private void InputPaneShowing(InputPane sender, InputPaneVisibilityEventArgs e)
        {
            // Set EnsuredFocusedElementInView to true so the UI doesn't scroll
            e.EnsuredFocusedElementInView = true;

            // Set new margins to LayoutRoot (to compensate keyboard)
            textText.MaxHeight = 80;
            Debug.WriteLine(textText.MaxHeight);

            // Unsubscribe InputPane Showing event
            inputPane.Showing -= this.InputPaneShowing;
        }

        private void InputPaneHiding(InputPane sender, InputPaneVisibilityEventArgs e)
        {
            // Set EnsuredFocusedElementInView to false so the UI scrolls
            e.EnsuredFocusedElementInView = false;

            // Reset LayoutRoot margins
            textText.MaxHeight = 200;

            // Unsubscribe InputPane Hiding event to handle UI scrolling
            inputPane.Hiding -= this.InputPaneHiding;
        }
        private void keyboardTimer_Tick(object sender, object e)
        {
            // Stop timer so it doesn't repeat
            keyboardTimer.Stop();

            // Invoke ChangeView() on NoteContentScrollViewer, and use GetRectFromCharacterIndex to scroll to caret position
            if (textText.Text != "")
                // textScrollView.ChangeView(0, textText.GetRectFromCharacterIndex(textText.SelectionStart - 1, true).Y, null);
                textScrollView.ChangeView(0, textScrollView.ExtentHeight, null);
        }
    }
}
