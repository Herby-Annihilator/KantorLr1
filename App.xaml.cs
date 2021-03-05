using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace KantorLr1
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private static IHost host;
		public static IHost Host => host ??= Program.CreateHostBuilder(Environment.GetCommandLineArgs())
			.Build();
		protected override async void OnStartup(StartupEventArgs e)
		{
			IHost host1 = Host;
			base.OnStartup(e);
			await host1.StartAsync().ConfigureAwait(false);
		}
		protected override async void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
			IHost host1 = Host;
			await host1.StopAsync().ConfigureAwait(false);
			host1.Dispose();
			host = null;
		}
		internal static void ConfigureServices(HostBuilderContext host, IServiceCollection services)
		{
			
		}
	}
}
