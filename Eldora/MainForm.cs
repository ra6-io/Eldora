#region

using System;
using Eto.Drawing;
using Eto.Forms;

#endregion

// TODO: API Support https://www.dnd5eapi.co/docs/#overview--getting-started
// TODO: API Support https://www.dnddeutsch.de/api/
// TODO: GITHUB API: https://docs.github.com/en/rest/repos/repos

namespace Eldora;

public sealed class MainForm : Form
{
	private const string IconResourcePath = "Eldora.Resources.icon.ico";

	/// <summary>
	///     Destroys the window when exited.
	///     Fixes the closing on mac.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private static void KillProgramOnMainWindowExit(object sender, EventArgs e)
	{
		Application.Instance.Quit();
	}

	public MainForm()
	{
		// Fixes termination on mac
		Closed += KillProgramOnMainWindowExit;
		Application.Instance.Terminating += (s, e) => Closed -= KillProgramOnMainWindowExit;

		Title = "Eldora";
		MinimumSize = new Size(1280, 720);
		ClientSize = MinimumSize;

		Icon = Icon.FromResource(IconResourcePath, typeof(MainForm));

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