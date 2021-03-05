using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace KantorLr1.ViewModels
{
	internal class ViewModelLocator
	{
		public MainWindowViewModel MainWindowViewModel = 
			App.Host.Services.GetRequiredService<MainWindowViewModel>();
	}
}
