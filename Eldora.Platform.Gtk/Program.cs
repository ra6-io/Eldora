#region

using System;
using Eto;
using Eto.Forms;

#endregion

namespace Eldora.Gtk;

class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		new Application(Platforms.Gtk).Run(new Eldora(new LinuxPlatformHandler()).MainForm);
	}
}