#region

using System;
using Eto;
using Eto.Forms;

#endregion

namespace DnDHelperV2.Wpf
{
	class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			new Application(Platforms.Wpf).Run(new DnDHelper(new WindowsPlatformHandler()).MainForm);
		}
	}
}