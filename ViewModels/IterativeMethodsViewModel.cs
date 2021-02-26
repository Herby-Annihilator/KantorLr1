using System;
using System.Collections.Generic;
using System.Text;
using CompMathLibrary;

namespace KantorLr1.ViewModels
{
	public class IterativeMethodsViewModel : ContentControlViewModel
	{
		private double[] approximation;
		public IterativeMethodsViewModel()
		{
			reshala = new CMReshala();
			methodType = MethodType.Gauss;
			matrix = null;
			vector = null;
		}

		#region Properties

		#endregion

		#region Commands

		#endregion
	}
}
