using System;
using System.Collections.Generic;
using System.Text;
using CompMathLibrary;
using KantorLr1.ViewModels.Base;

namespace KantorLr1.ViewModels
{
	public class ContentControlViewModel : BaseViewModel
	{
		protected string status = "Default";
		public string Status { get => status; set => Set(ref status, value); }
		protected CMReshala reshala;
		protected double[][] matrix;
		protected double[] vector;
	}
}
