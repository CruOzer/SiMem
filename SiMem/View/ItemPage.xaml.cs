﻿using SiMem.Common;
using SiMem.Data;
using SiMem.DataModel;
using Windows.UI.Xaml.Controls;
using Autofac;
using Windows.UI.Xaml.Navigation;
using System;
using Windows.ApplicationModel.Resources;
using SiMem.View;
using SiMem.Logic;

// Die Vorlage "Pivotanwendung" ist unter http://go.microsoft.com/fwlink/?LinkID=391641 dokumentiert.

namespace SiMem
{
    /// <summary>
    /// Eine Seite, auf der Details für ein einzelnes Element innerhalb einer Gruppe angezeigt werden.
    /// </summary>
    public sealed partial class ItemPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");
        private IDataSource<Memory> memoryDataSource;
        private ISiMemTileFactory siMemTileFactory;
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
            memoryDataSource = DI.Container.Resolve<IDataSource<Memory>>();
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
            TogglePinAppBarButton(!siMemTileFactory.TileExists(memory));
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
        /// <summary>
        /// Pins or unpins a secondary tile from the start screenu
        /// </summary>
        private async void PinAppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Memory memory = memoryDataSource.GetById(memoryId);
            //If the createTile was not successul
            if (!await siMemTileFactory.CreateTile(memory))
            {
                //unload the tile
                await siMemTileFactory.DeleteTile(memory);
                //and update the layout
                TogglePinAppBarButton(true);
            }
            else
            {
                //Update the layout
                TogglePinAppBarButton(false);
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
                this.PinAppBarButton.Label = resourceLoader.GetString("PinAppBarButtonPin");
                this.PinAppBarButton.Icon = new SymbolIcon(Symbol.Pin);
            }
            else
            {
                this.PinAppBarButton.Label = resourceLoader.GetString("PinAppBarButtonUnpin");
                this.PinAppBarButton.Icon = new SymbolIcon(Symbol.UnPin);
            }

            this.PinAppBarButton.UpdateLayout();
        }

        /// <summary>
        /// Löscht die Memory und lädt die PivotPage
        /// </summary>
        private async void DeleteAppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Memory memory = memoryDataSource.GetById(memoryId);
            //Delete Memory
            memoryDataSource.Delete(memory);
            await siMemTileFactory.DeleteTile(memory);
            if (!Frame.Navigate(typeof(PivotPage)))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }
    }
}