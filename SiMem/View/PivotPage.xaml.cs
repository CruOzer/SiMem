using Autofac;
using SiMem.Common;
using SiMem.Data;
using SiMem.DataModel;
using SiMem.Logic;
using SiMem.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// Die Vorlage "Pivotanwendung" ist unter http://go.microsoft.com/fwlink/?LinkID=391641 dokumentiert.

namespace SiMem
{
    public sealed partial class PivotPage : Page
    {
        //Constanten müssen mit der UID, dem Namen und dem Binding der Pivots übereinstimmen
        private const string RECENTGROUPNAME = "RecentPivot";
        private const string SETTINGSGROUPNAME = "SettingsPivot";
        private const string CATEGORYGROUPNAME = "CategoryPivot";
        private const int RECENTMEMORYCOUNT = 5;
        /// <summary>
        /// Datenbankschnittstelle Memory
        /// </summary>
        private IDataSource<Memory> memoryDataSource;
        private IDataSource<MemoryType> memoryTypeDataSource;
        private ISiMemTileFactory siMemTileFactory;

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
            memoryDataSource =  DI.Container.Resolve<IDataSource<Memory>>();
            memoryTypeDataSource = DI.Container.Resolve<IDataSource<MemoryType>>();
            siMemTileFactory = DI.Container.Resolve<ISiMemTileFactory>();
            
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
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            loadRecentMemories();
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
            //TODO
        }

        /// <summary>
        /// Lädt ein Dialog und fügt bei Erfolg der Liste ein Element hinzu, wenn auf die App-Leisten-Schaltfläche geklickt wird.
        /// </summary>
        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //Starten der Seite zum neu Einfügen einer Memory
            if (!Frame.Navigate(typeof(AddItemPage)))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        /// <summary>
        /// Wird aufgerufen, wenn auf ein Element innerhalb eines Abschnitts geklickt wird. Lädt die entsprechende Seite
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
        /// Lädt ein Dialog und editiert bei Erfolg das Listenelement.
        /// </summary>
        private void ItemView_Edit(object sender, RoutedEventArgs e)
        {
            // Zur entsprechenden Zielseite navigieren und die neue Seite konfigurieren,
            // indem die erforderlichen Informationen als Navigationsparameter übergeben werden
            var memory = (Memory)(e.OriginalSource as FrameworkElement).DataContext;
            if (!Frame.Navigate(typeof(AddItemPage), memory.Id))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
            loadCategories();
        }
        /// <summary>
        /// Löscht das ausgewählte Item aus der Datenbank und aus der Liste
        /// </summary>
        private async void ItemView_Delete(object sender, RoutedEventArgs e)
        {
            // Löscht das momentan ausgewählt Element, sowohl aus der Datenbank, als auch aus der Liste
            var memory = (Memory)(e.OriginalSource as FrameworkElement).DataContext;
            //Delete Memory
            memoryDataSource.Delete(memory);

            await siMemTileFactory.DeleteTile(memory);
            //Reload the Memories within the type
            //loadMemories(memory.MemoryType);
            //Reload the RecentMemories
            loadRecentMemories();
        }
        /// <summary>
        /// Wird aufgerufen, wenn auf ein Element innerhalb eines Abschnitts geklickt wird. Lädt die entsprechende Seite
        /// </summary>
        private void CategoryView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Zur entsprechenden Zielseite navigieren und die neue Seite konfigurieren,
            // indem die erforderlichen Informationen als Navigationsparameter übergeben werden
            var itemId = ((MemoryType)e.ClickedItem).Id;
            if (!Frame.Navigate(typeof(CategoryPage), itemId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }
        /// <summary>
        /// Lädt ein Dialog und editiert bei Erfolg das Listenelement.
        /// </summary>
        private void CategoryView_Edit(object sender, RoutedEventArgs e)
        {
            // Zur entsprechenden Zielseite navigieren und die neue Seite konfigurieren,
            // indem die erforderlichen Informationen als Navigationsparameter übergeben werden
            var memoryType = (MemoryType)(e.OriginalSource as FrameworkElement).DataContext;
            if (!Frame.Navigate(typeof(AddCategoryPage), memoryType.Id))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
            loadCategories();
        }
        /// <summary>
        /// Löscht die auswählte Kategorie und alle Items aus der Datenbank und aus der Liste
        /// </summary>
        private async void CategoryView_Delete(object sender, RoutedEventArgs e)
        {
            // Löscht das momentan ausgewählt Element, sowohl aus der Datenbank, als auch aus der Liste
            var memoryType = (MemoryType)(e.OriginalSource as FrameworkElement).DataContext;
            //Delete Memories within the category
            List<Memory> lMemory = memoryDataSource.GetByType(memoryType.Id);
            foreach (var item in lMemory)
            {
                memoryDataSource.Delete(item);
                await siMemTileFactory.DeleteTile(item);
            }
            //Delete Category
            memoryTypeDataSource.Delete(memoryType);
            //Reload the Memories within the type
            loadCategories();
        }

        /// <summary>
        /// Lädt die letzten Memories
        /// </summary>
        private void loadRecentMemories()
        {
            var recentMemories = memoryDataSource.GetRecent(RECENTMEMORYCOUNT);
            this.DefaultViewModel[RECENTGROUPNAME] = recentMemories;
        }
        /// <summary>
        /// Lädt alle Kategorien
        /// </summary>
        private void loadCategories()
        {
            this.DefaultViewModel[CATEGORYGROUPNAME] = memoryTypeDataSource.GetAll();
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
            // If there is a parameter then continue the navigation to the appropriate page.
            this.navigationHelper.OnNavigatedTo(e);

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
        /// <summary>
        /// Lädt den Inhalt für das Pivotelement, wenn es per Bildlauf in die Anzeige verschoben wird.
        /// </summary>
        private void Pivot_Loaded(object sender, RoutedEventArgs e)
        {
            PivotItem pivotItem = (PivotItem)sender;
            switch (pivotItem.Name)
            {
                case CATEGORYGROUPNAME:
                    loadCategories();
                    break;
                case SETTINGSGROUPNAME:
                    break;
                case RECENTGROUPNAME:
                    loadRecentMemories();
                    break;
            }
        }
    }
}