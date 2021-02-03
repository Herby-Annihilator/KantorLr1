using System;
using System.Collections.Generic;
using System.Text;
using KantorLr1.ViewModels.Base;

namespace KantorLr1.ViewModels
{
	public class MainWindowViewModel : BaseViewModel
	{
		private string status = "Hello";
		public string Status { get => status; set => Set(ref status, value); }

		private string title = "GaussMethod";
		public string Title { get => title; set => Set(ref title, value); }
	}
}
