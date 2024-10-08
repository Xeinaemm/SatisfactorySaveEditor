﻿using SatisfactorySaveEditor.Model;
using SatisfactorySaveEditor.View;
using SatisfactorySaveEditor.ViewModel;
using SatisfactorySaveParser;
using System.Windows;

namespace SatisfactorySaveEditor.Cheats;

class SpawnDoggoCheat(DeleteEnemiesCheat deleter) : ICheat
{
    public string Name => "Spawn doggos...";

    private DeleteEnemiesCheat deleteEnemiesCheat = deleter; //uses the add doggo code from delete enemies to avoid duplicating code

    public bool Apply(SaveObjectModel rootItem, SatisfactorySave saveGame)
    {
        var doggocount = 1;

        var dialog = new StringPromptWindow
        {
            Owner = Application.Current.MainWindow
        };
        var cvm = (StringPromptViewModel)dialog.DataContext;
        cvm.WindowTitle = "Enter doggo count";
        cvm.PromptMessage = "Count (integer):";
        cvm.ValueChosen = "1";
        cvm.OldValueMessage = "";
        dialog.ShowDialog();

        try
        {
            doggocount = int.Parse(cvm.ValueChosen);
            //MessageBox.Show("" + doggocount);

            if (doggocount > 0)
            {
                int counter;
                var pastSuccess = true; //don't keep running the loop if one run fails
                for (counter = 0; counter < doggocount && pastSuccess; counter++)
                {
                    pastSuccess = deleteEnemiesCheat.AddDoggo(rootItem, saveGame);
                }

                if (pastSuccess)
                {
                    MessageBox.Show("Spawned " + counter + " doggo(s) at the host player.");
                    return true;
                }
                else
                {
                    //failed to spawn some doggos for some reason
                    return false;
                }
            }
            else
            {
                MessageBox.Show("You can't spawn " + doggocount + " doggos.");
                return false;
            }
        }
        catch (Exception)
        {
            if (!(cvm.ValueChosen == "cancel"))
            {
                MessageBox.Show("Could not parse: " + cvm.ValueChosen);
            }
            return false;
        }
    }
    
}
