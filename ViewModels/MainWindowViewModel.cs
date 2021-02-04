using System;
using System.Collections.Generic;
using System.Text;
using KantorLr1.ViewModels.Base;
using CompMathLibrary;
using CompMathLibrary.Methods;

namespace KantorLr1.ViewModels
{
	public class MainWindowViewModel : BaseViewModel
	{
		public MainWindowViewModel()
		{
			CMReshala reshala = new CMReshala();
		}
		private string status = "Hello";
		public string Status { get => status; set => Set(ref status, value); }

		private string title = "GaussMethod";
		public string Title { get => title; set => Set(ref title, value); }

	}
}
