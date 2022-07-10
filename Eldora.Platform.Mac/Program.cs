#region

using System;
using Eto;
using Eto.Forms;
using Eto.Mac.Forms;

#endregion

namespace Eldora.Mac;

class Program
{

	[STAThread]
	public static void Main(string[] args)
	{
		Style.Add<ApplicationHandler>(null, handler => handler.AllowClosingMainForm = true);

		new Application(Platforms.Mac64).Run(new Eldora(new MacPlatformHandler()).MainForm);
	}
}