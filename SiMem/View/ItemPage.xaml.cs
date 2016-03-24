using SiMem.Common;
using SiMem.Data;
using SiMem.DataModel;
using Windows.UI.Xaml.Controls;
using Autofac;
using Windows.UI.Xaml.Navigation;
using System;
using Windows.ApplicationModel.Resources;
using SiMem.View;
using Windows.UI.StartScreen;
using NotificationsExtensions.BadgeContent;
using NotificationsExtensions.TileContent;
using Windows.UI.Notifications;

// Die Vorlage "Pivotanwendung" ist unter http://go.microsoft.com/fwlink/?LinkID=391641 dokumentiert.

namespace SiMem
{
    /// <summary>
    /// Eine Seite, auf der Details für ein einzelnes Element innerhalb einer Gruppe angezeigt werden.
    /// </summary>
    public sealed partial class ItemPage : Page
    {

        public static string SECONDARY_TILE = "SecondaryTile";
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");
        private IDataSource<Memory> memoryDataSource;
        /// <summary>
        /// Aktuell angezeigte MemoryId
        /// </summary>
        private int memoryId;

        public ItemPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            memoryDataSource = App.Container.Resolve<IDataSource<Memory>>();
              
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
        /// beibehalten wurde.  Der Zustand ist beim ersten Aufrufen einer Seite NULL.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            //Übergabe der Id der Memory
            memoryId = (int)e.NavigationParameter;
            //Holen der Memory
            Memory memory = memoryDataSource.GetById(memoryId);
            //Visualisieren der Memory
            this.DefaultViewModel["Item"] = memory;

            //Instanziiere den PinAppBarButton
            TogglePinAppBarButton(!SecondaryTile.Exists(memoryId.ToString()));
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
        /// <summary>
        /// Lädt die Seite zum Editieren der Memory
        /// </summary>
        private void EditAppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Zur entsprechenden Zielseite navigieren und die neue Seite konfigurieren,
            // indem die erforderlichen Informationen als Navigationsparameter übergeben werden

            if (!Frame.Navigate(typeof(AddItemPage), memoryId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        private async void PinAppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

            if (SecondaryTile.Exists(memoryId.ToString()))
            {
                // Unpin
                SecondaryTile secondaryTile = new SecondaryTile(memoryId.ToString());
                bool isUnpinned = await secondaryTile.RequestDeleteAsync();

                TogglePinAppBarButton(isUnpinned);
            }
            else
            {
                Memory memory = memoryDataSource.GetById(memoryId);
                SecondaryTile secondaryTile = new SecondaryTile(memoryId.ToString(), memory.Title, SECONDARY_TILE + memoryId.ToString(), new Uri("ms-appx:///Assets/Logo.scale-240.png"), TileSize.Square150x150);
                // Whether or not the app name should be displayed on the tile can be controlled for each tile size.  The default is false.
                secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;

                // Adding the wide tile logo.
                secondaryTile.VisualElements.Wide310x150Logo = new Uri("ms-appx:///Assets/WideLogo.scale-240.png");
                // Specify a foreground text value.
                // The tile background color is inherited from the parent unless a separate value is specified.
                secondaryTile.VisualElements.ForegroundText = ForegroundText.Dark;
                // Since pinning a secondary tile on Windows Phone will exit the app and take you to the start screen, any code after 
                // RequestCreateForSelectionAsync or RequestCreateAsync is not guaranteed to run.  For an example of how to use the OnSuspending event to do
                // work after RequestCreateForSelectionAsync or RequestCreateAsync returns, see Scenario9_PinTileAndUpdateOnSuspend in the SecondaryTiles.WindowsPhone project.
                bool isPinned =await secondaryTile.RequestCreateAsync();
                TogglePinAppBarButton(!isPinned);

                // Note: This sample contains an additional reference, NotificationsExtensions, which you can use in your apps
                ITileWide310x150PeekImage01 tileContent = TileContentFactory.CreateTileWide310x150PeekImage01();
                tileContent.TextHeading.Text = memory.Title;
                tileContent.TextBodyWrap.Text = memory.Text;
                tileContent.Image.Src= "ms-appx:///Assets/WideLogo.scale-240.png";

                ITileSquare150x150PeekImageAndText02 squareContent = TileContentFactory.CreateTileSquare150x150PeekImageAndText02();
                squareContent.TextBodyWrap.Text = memory.Text;
                squareContent.TextHeading.Text = memory.Title;
                squareContent.Image.Src = "ms-appx:///Assets/Logo.scale-240.png";
                tileContent.Square150x150Content = squareContent;

                // Send the notification to the secondary tile by creating a secondary tile updater
                TileUpdateManager.CreateTileUpdaterForSecondaryTile(memoryId.ToString()).Update(tileContent.CreateNotification());
            }
        }

        /// <summary>
        /// Updates the layout of the PinAppBarButton to unpin or pin
        /// </summary>
        /// <param name="showPinButton">Unpin: false, Pin: true</param>
        private void TogglePinAppBarButton(bool showPinButton)
        {
            if (showPinButton)
            {
                this.PinAppBarButton.Label = resourceLoader.GetString("PinAppBarButton.Pin");
                this.PinAppBarButton.Icon = new SymbolIcon(Symbol.Pin);
            }
            else
            {
                this.PinAppBarButton.Label = resourceLoader.GetString("PinAppBarButton.Unpin");
                this.PinAppBarButton.Icon = new SymbolIcon(Symbol.UnPin);
            }

            this.PinAppBarButton.UpdateLayout();
        }

        /// <summary>
        /// Löscht die Memory und lädt die PivotPage
        /// </summary>
        private void DeleteAppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //Delete Memory
            memoryDataSource.Delete(memoryDataSource.GetById(memoryId));
            if (!Frame.Navigate(typeof(PivotPage)))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }
    }
}