﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace KantorLr1.ViewModels
{
	public class ViewModelLocator
	{
		public MainWindowViewModel MainWindowViewModel { get; } =
			App.Host.Services.GetRequiredService<MainWindowViewModel>();
		public IterativeMethodsViewModel IterativeMethodsViewModel =>
			App.Host.Services.GetRequiredService<IterativeMethodsViewModel>();
		public AutoGenerateDataViewModel AutoGenerateDataViewModel =>
			App.Host.Services.GetRequiredService<AutoGenerateDataViewModel>();
	}
}
