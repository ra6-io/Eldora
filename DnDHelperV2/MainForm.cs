using System;
using System.ComponentModel;
using Eto.Forms;
using Serilog;
using Size = Eto.Drawing.Size;

// TODO: API Support https://www.dnd5eapi.co/docs/#overview--getting-started
// TODO: API Support https://www.dnddeutsch.de/api/
// TODO: https://docs.github.com/en/rest/repos/repos
// TODO: Add Plugin Repo https://github.com/ra6-io/DnDHelperV2.Plugins

namespace DnDHelperV2
{
    public sealed class MainForm : Form
    {
	    /// <summary>
	    /// Destroys the window when exited.
	    ///
	    /// Fixes the closing on mac.
	    /// </summary>
	    /// <param name="sender"></param>
	    /// <param name="e"></param>
        private static void KillProgramOnMainWindowExit(object sender, EventArgs e)
        {
            Application.Instance.Quit();
        }

        public MainForm()
        {
            Closed += KillProgramOnMainWindowExit;
            Application.Instance.Terminating += (s, e) => Closed -= KillProgramOnMainWindowExit;

            Title = "D&D Helper V2";
            MinimumSize = new Size(1280, 720);
            ClientSize = MinimumSize;

            var tabControl = new TabControl
            {
                TabPosition = DockPosition.Left
            };

            Content = tabControl;
        }
        /*
            Content = new StackLayout
            {
                    Padding = 10,
                    Items =
                    {
                            "Hello World!",
                            // add more controls here
                    }
            };

            // create a few commands that can be used for the menu and toolbar
            var clickMe = new Command { MenuText = "Click Me!", ToolBarText = "Click Me!" };
            clickMe.Executed += (sender, e) => MessageBox.Show(this, "I was clicked!");

            var quitCommand = new Command
                    { MenuText = "Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q };
            quitCommand.Executed += (sender, e) => Application.Instance.Quit();

            var aboutCommand = new Command { MenuText = "About..." };
            aboutCommand.Executed += (sender, e) => new AboutDialog().ShowDialog(this);

            // create menu
            Menu = new MenuBar
            {
                    Items =
                    {
                            // File submenu
                            new SubMenuItem { Text = "&File", Items = { clickMe } },
                            new SubMenuItem { Text = "&Edit", Items = {  commands/items } },
new SubMenuItem { Text = "&View", Items = {  commands/items } },
},
ApplicationItems =
{
// application (OS X) or file menu (others)
new ButtonMenuItem { Text = "&Preferences..." },
},
QuitItem = quitCommand,
AboutItem = aboutCommand
};

// create toolbar			
ToolBar = new ToolBar { Items = { clickMe } };
         */
    }
}