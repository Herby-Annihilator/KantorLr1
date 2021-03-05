using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

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

		public static IHostBuilder CreateHostBuilder(string[] args)
		{
			IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args);
			hostBuilder.UseContentRoot(App.CurrentDirectory);
			hostBuilder.ConfigureAppConfiguration((host, config) =>
			{
				config.SetBasePath(App.CurrentDirectory);
				config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
			});
			hostBuilder.ConfigureServices(App.ConfigureServices);
			return hostBuilder;
		}
	}
}
