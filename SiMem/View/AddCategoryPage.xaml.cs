using Autofac;
using SiMem.Common;
using SiMem.Data;
using SiMem.DataModel;
using SiMem.Logic;
using System;
using System.Diagnostics;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Standardseite" ist unter "http://go.microsoft.com/fwlink/?LinkID=390556" dokumentiert.

namespace SiMem.View
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Frames navigiert werden kann.
    /// </summary>
    public sealed partial class AddCategoryPage : Page
    {
        public const int EDIT_MODE = 0;
        public const int ADD_MODE = 1;

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private IDataSource<MemoryType> memoryTypeDataSource;
        private ISiMemTileFactory siMemTileFactory;
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");
        /// <summary>
        /// Modues der Page
        /// </summary>
        private int mode;
        /// <summary>
        /// Zu bearbeitende Memory
        /// </summary>
        private MemoryType memoryType;
        /// <summary>
        /// Speichert den alten Typ der Memory
        /// </summary>
        private int oldType;


        public AddCategoryPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            memoryTypeDataSource = DI.Container.Resolve<IDataSource<MemoryType>>();
            siMemTileFactory = DI.Container.Resolve<ISiMemTileFactory>();
            instantiateMemoryTypeComboBox();
        }

        /// <summary>
        /// Ruft den <see cref="NavigationHelper"/> ab, der mit dieser <see cref="Page"/> verknüpft ist.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get
            {
                return this.navigationHelper;
            }
        }

        /// <summary>
        /// Ruft das Anzeigemodell für diese <see cref="Page"/> ab.
        /// Dies kann in ein stark typisiertes Anzeigemodell geändert werden.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get
            {
                return this.defaultViewModel;
            }
        }
      
        /// <summary>
        /// Füllt die Seite mit Inhalt auf, der bei der Navigation übergeben wird.  Gespeicherte Zustände werden ebenfalls
        /// bereitgestellt, wenn eine Seite aus einer vorherigen Sitzung neu erstellt wird.
        /// </summary>
        /// <param name="sender">
        /// Die Quelle des Ereignisses, normalerweise <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Ereignisdaten, die die Navigationsparameter bereitstellen, die an
        /// <see cref="Frame.Navigate(Type, Object)"/> als diese Seite ursprünglich angefordert wurde und
        /// ein Wörterbuch des Zustands, der von dieser Seite während einer früheren
        /// beibehalten wurde.  Der Zustand ist beim ersten Aufrufen einer Seite NULL.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {    
         
            if (e.NavigationParameter != null)
            {
                //Übergabe der Id der Memory
                int memoryTypeId = (int)e.NavigationParameter;
                //Holen der Memory
                memoryType = memoryTypeDataSource.GetById(memoryTypeId);
                //Setzen des aktuellen Modus der Page
                mode = EDIT_MODE;
            }
            else
            {
                memoryType = new MemoryType();
                //Setzen des aktuellen Modus der Page
                mode = ADD_MODE;
            }
            //Bestücken der Input-Felder
            this.DefaultViewModel["Category"] = memoryType;
        }
      
        /// <summary>
        /// Behält den dieser Seite zugeordneten Zustand bei, wenn die Anwendung angehalten oder
        /// die Seite im Navigationscache verworfen wird. Die Werte müssen den Serialisierungsanforderungen
        /// von <see cref="SuspensionManager.SessionState"/> entsprechen.
        /// </summary>
        /// <param name="sender">Die Quelle des Ereignisses, normalerweise <see cref="NavigationHelper"/></param>
        /// <param name="e">Ereignisdaten, die ein leeres Wörterbuch zum Auffüllen bereitstellen
        /// serialisierbarer Zustand.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            //TODO: speichern
        }

        private void SaveButton_Clicked(object sender, RoutedEventArgs e)
        {
    
            //Validiert den Input
            if (validateInput())
            {
                //Speichern der aktuellen Daten
                memoryType.Name = titleText.Text;
                switch (mode)
                {
                    case ADD_MODE:
                        memoryType.Id = memoryTypeDataSource.GetMax() + 1;
                        //Einfügen in die Datenbank
                        memoryTypeDataSource.Insert(memoryType);
                        break;
                    case EDIT_MODE:
                        //Update der Datenbank
                        memoryTypeDataSource.Update(memoryType);
                        //Update der secondary tiles, sofern vorhanden
                        break;
                    default:
                        break;
                }
                //Starten der Seite MainSeite mit Array [Type, OldType]
                if (!Frame.Navigate(typeof(PivotPage)))
                {
                    throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
                }
            }            
        }

        /// <summary>
        /// Validates the Input
        /// </summary>
        /// <returns>Input is correct or not</returns>
        private bool validateInput()
        {
            bool result = true;
            //Validierung der Felder
            if (string.IsNullOrEmpty(titleText.Text))
            {
                result = false;
            }
            return result;
        }

        #region NavigationHelper-Registrierung

        /// <summary>
        /// Die in diesem Abschnitt bereitgestellten Methoden werden einfach verwendet,
        /// damit NavigationHelper auf die Navigationsmethoden der Seite reagieren kann.
        /// <para>
        /// Platzieren Sie seitenspezifische Logik in Ereignishandlern für  
        /// <see cref="NavigationHelper.LoadState"/>
        /// und <see cref="NavigationHelper.SaveState"/>.
        /// Der Navigationsparameter ist in der LoadState-Methode zusätzlich 
        /// zum Seitenzustand verfügbar, der während einer früheren Sitzung gesichert wurde.
        /// </para>
        /// </summary>
        /// <param name="e">Stellt Daten für Navigationsmethoden und -ereignisse bereit.
        /// Handler, bei denen die Navigationsanforderung nicht abgebrochen werden kann.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            //Starten der Seite MainSeite
            if (!Frame.Navigate(typeof(PivotPage)))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }
    }
}
