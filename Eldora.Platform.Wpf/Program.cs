#region

using System;
using Eto;
using Eto.Forms;

#endregion

namespace Eldora.Wpf;

class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		new Application(Platforms.Wpf).Run(new Eldora(new WindowsPlatformHandler()).MainForm);
	}
}