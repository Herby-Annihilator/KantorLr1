using System;
using System.Collections.Generic;
using System.Text;

namespace KantorLr1
{
	public static class Program
	{
		[STAThread]
		public static void Main()
		{
			var app = new App();
			app.InitializeComponent();
			app.Run();
		}
	}
}
