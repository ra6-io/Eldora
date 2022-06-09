using System;
using Eto.Forms;

namespace DnDHelperV2.Wpf
{
	class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			new Application(Eto.Platforms.Wpf).Run(new DnDHelper().MainForm);
		}
	}
}