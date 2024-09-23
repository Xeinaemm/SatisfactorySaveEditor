using SatisfactorySaveEditor.Model;
using SatisfactorySaveParser;
using System.Windows;

namespace SatisfactorySaveEditor.Cheats;

public class CrateSummonCheat : ICheat
{
    public string Name => "Teleport crates to host player";

    public bool Apply(SaveObjectModel rootItem, SatisfactorySave saveGame)
    {
        var cratesList = rootItem.FindChild("BP_Crate.BP_Crate_C", false);
        if (cratesList == null)
        {
            MessageBox.Show("This save does not appear to have any crates.\nCrates appear on death and when your inventory is too full to hold items during deconstruction.", "Cannot find any Crates", MessageBoxButton.OK, MessageBoxImage.Information);
            return false;
        }

        var hostPlayerModel = rootItem.FindChild("Char_Player.Char_Player_C", false);
        if (hostPlayerModel == null || hostPlayerModel.Items.Count < 1)
        {
            MessageBox.Show("This save does not contain a host player or it is corrupt.\nTry loading and re-saving the save from within the game.", "Cannot find host player", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
        var playerEntityModel = (SaveEntityModel)hostPlayerModel.Items[0];

        var crateSpacingOffset = 80;
        var stackHeight = 5;

        var playerX = playerEntityModel.Position.X + (1.5f * crateSpacingOffset);
        var playerY = playerEntityModel.Position.Y;
        var playerZ = playerEntityModel.Position.Z - crateSpacingOffset;

        var counter = 0;
        
        foreach (var thisCrate in cratesList.DescendantSelfViewModel)
        {
            //MessageBox.Show($"Modifying crate {counter}: {thisCrate.ToString()}");
            ((SaveEntityModel)thisCrate).Position.X = playerX + (counter / stackHeight * crateSpacingOffset);//((counter % stackHeight == 0) ? crateHeightSpacingOffset * counter / stackHeight : 0);
            ((SaveEntityModel)thisCrate).Position.Y = playerY;
            ((SaveEntityModel)thisCrate).Position.Z = playerZ + (crateSpacingOffset * (counter % stackHeight));
            counter++;
        }
        MessageBox.Show($"Successfully moved {counter} crates to the host player. They are stacked in piles of 5 slightly east of you.");
        return true;
    }
}
