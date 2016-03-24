using NotificationsExtensions.TileContent;
using SiMem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace SiMem.Logic
{
    public interface ISiMemTileFactory
    {
        void UpdateTile(Memory memory);
        Task<bool> CreateTile(Memory memory);
        Task<bool> DeleteTile(Memory memory);
    }
    /// <summary>
    /// 
    /// </summary>
    public class SiMemTileFactory : ISiMemTileFactory
    {
        public static string SECONDARY_TILE = "SecondaryTile";

        /// <summary>
        /// Sends (Adds/Updates) a tile notification to the secondary tile, if it exists
        /// </summary>
        /// <param name="memory">Memory</param>
        public void UpdateTile(Memory memory)
        {
            //If a SecondaryTile for the memory exists
            if (SecondaryTile.Exists(memory.Id.ToString()))
            {
                //Load the wide screen and sets it up
                ITileWide310x150PeekImage01 tileContent = TileContentFactory.CreateTileWide310x150PeekImage01();
                tileContent.TextHeading.Text = memory.Title;
                tileContent.TextBodyWrap.Text = memory.Text;
                tileContent.Image.Src = "ms-appx:///Assets/WideLogo.scale-240.png";
                //Load a standard screen and sets it up
                ITileSquare150x150PeekImageAndText02 squareContent = TileContentFactory.CreateTileSquare150x150PeekImageAndText02();
                squareContent.TextBodyWrap.Text = memory.Text;
                squareContent.TextHeading.Text = memory.Title;
                squareContent.Image.Src = "ms-appx:///Assets/Logo.scale-240.png";
                //Append standard screen to wide screen
                tileContent.Square150x150Content = squareContent;
                // Send the notification to the secondary tile by creating a secondary tile updater
                TileUpdateManager.CreateTileUpdaterForSecondaryTile(memory.Id.ToString()).Update(tileContent.CreateNotification());
            }
        }
        /// <summary>
        /// Creates a tile to the start screen
        /// </summary>
        /// <param name="memory">Identifies the tile</param>
        /// <returns>True for success</returns>
        public async Task<bool> CreateTile(Memory memory)
        {
            bool isPinned = false;
            if (!SecondaryTile.Exists(memory.Id.ToString()))
            {
                SecondaryTile secondaryTile = new SecondaryTile(memory.Id.ToString(), memory.Title, SECONDARY_TILE + memory.Id.ToString(), new Uri("ms-appx:///Assets/Logo.scale-240.png"), TileSize.Square150x150);
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
                isPinned = await secondaryTile.RequestCreateAsync();
                
                //Loads the better interface for the tile
                if (isPinned)
                    UpdateTile(memory);
                
            }
            return await Task.FromResult(isPinned);
        }
        /// <summary>
        /// Deletes a tile from the start screen
        /// </summary>
        /// <param name="memory">Identifies the tile</param>
        /// <returns>True for success</returns>
        public async Task<bool> DeleteTile(Memory memory)
        {
            bool isUnpinned = false;
            if (SecondaryTile.Exists(memory.Id.ToString()))
            {
                // Unpin
                SecondaryTile secondaryTile = new SecondaryTile(memory.Id.ToString());
                isUnpinned = await secondaryTile.RequestDeleteAsync();
            }
            return await Task.FromResult(isUnpinned);
        }
    }
}
