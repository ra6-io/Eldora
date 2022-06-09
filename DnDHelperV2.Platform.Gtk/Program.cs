using System;
using Eto.Forms;

namespace DnDHelperV2.Gtk
{
    class Program
    {
	    [STAThread]
	    public static void Main(string[] args)
	    {
		    new Application(Eto.Platforms.Gtk).Run(new DnDHelper().MainForm);
	    }
    }
}