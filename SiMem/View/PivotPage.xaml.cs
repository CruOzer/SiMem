using Autofac;
using SiMem.Common;
using SiMem.Data;
using SiMem.DataModel;
using SiMem.View;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// Die Vorlage "Pivotanwendung" ist unter http://go.microsoft.com/fwlink/?LinkID=391641 dokumentiert.

namespace SiMem
{
    public sealed partial class PivotPage : Page
    {
        private const string FirstGroupName = "FirstGroup";
        private const string SecondGroupName = "SecondGroup";

        /// <summary>
        /// Datenbankschnittstelle Memory
        /// </summary>
        private IDataSource<Memory> memoryDataSource;
        /// <summary>
        /// Datenbankschnittstelle MemoryGroup
        /// </summary>
        private IDataSource<MemoryGroup> memoryGroupDataSource;

        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        public PivotPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            //Laden der Dependency Injects der Datenbankschnittstellen
            memoryGroupDataSource = App.Container.Resolve<IDataSource<MemoryGroup>>();
            memoryDataSource =  App.Container.Resolve<IDataSource<Memory>>();
        }
 
        /// <summary>
        /// Ruft den <see cref="NavigationHelper"/> ab, der mit dieser <see cref="Page"/> verknüpft ist.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Ruft das Anzeigemodell für diese <see cref="Page"/> ab.
        /// Dies kann in ein stark typisiertes Anzeigemodell geändert werden.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Füllt die Seite mit Inhalt auf, der bei der Navigation übergeben wird. Gespeicherte Zustände werden ebenfalls
        /// bereitgestellt, wenn eine Seite aus einer vorherigen Sitzung neu erstellt wird.
        /// </summary>
        /// <param name="sender">
        /// Die Quelle des Ereignisses, normalerweise <see cref="NavigationHelper"/>.
        /// </param>
        /// <param name="e">Ereignisdaten, die die Navigationsparameter bereitstellen, die an
        /// <see cref="Frame.Navigate(Type, Object)"/> als diese Seite ursprünglich angefordert wurde und
        /// ein Wörterbuch des Zustands, der von dieser Seite während einer früheren
        /// beibehalten wurde. Der Zustand ist beim ersten Aufrufen einer Seite NULL.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            loadMemories();
        }
       
        /// <summary>
        /// Behält den dieser Seite zugeordneten Zustand bei, wenn die Anwendung angehalten oder
        /// die Seite im Navigationscache verworfen wird. Die Werte müssen den Serialisierungsanforderungen
        /// von <see cref="SuspensionManager.SessionState"/> entsprechen.
        /// </summary>
        /// <param name="sender">Die Quelle des Ereignisses, normalerweise <see cref="NavigationHelper"/>.</param>
        /// <param name="e">Ereignisdaten, die ein leeres Wörterbuch zum Auffüllen bereitstellen
        /// serialisierbarer Zustand.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: den eindeutigen Zustand der Seite hier speichern.
        }

        /// <summary>
        /// Fügt der Liste ein Element hinzu, wenn auf die App-Leisten-Schaltfläche geklickt wird.
        /// </summary>
        private async void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //Starten des Dialogs zum neu Einfügen einer Memory
            var dialog = new AddItemPage();
            var result = await dialog.ShowAsync();
            //Wenn ein neues Element eingefügt wurde, werden die Memories neu geladen 
            if (dialog.Result == AddItemResult.AddItemOK)
            {
                loadMemories();
                // Verschieben Sie das neue Element per Bildlauf in die Anzeige.
                var container = this.pivot.ContainerFromIndex(this.pivot.SelectedIndex) as ContentControl;
                var listView = container.ContentTemplateRoot as ListView;
                //Einfügen des neuen Memory Objekts
                listView.ScrollIntoView(dialog.Memory, ScrollIntoViewAlignment.Leading);
            }
        }

        /// <summary>
        /// Wird aufgerufen, wenn auf ein Element innerhalb eines Abschnitts geklickt wird.
        /// </summary>
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Zur entsprechenden Zielseite navigieren und die neue Seite konfigurieren,
            // indem die erforderlichen Informationen als Navigationsparameter übergeben werden
            var itemId = ((Memory)e.ClickedItem).Id;
            if (!Frame.Navigate(typeof(ItemPage), itemId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        /// <summary>
        /// Lädt den Inhalt für das zweite Pivotelement, wenn es per Bildlauf in die Anzeige verschoben wird.
        /// </summary>
        private async void SecondPivot_Loaded(object sender, RoutedEventArgs e)
        {
            return;
        }

        /// <summary>
        /// Laden alle Memories
        /// </summary>
        private void loadMemories()
        {
            var memories = memoryDataSource.GetAll(1);
            this.DefaultViewModel[FirstGroupName] = memories;
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
    }
}
