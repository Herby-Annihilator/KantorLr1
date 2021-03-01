using System;
using System.Collections.Generic;
using System.Text;
using CompMathLibrary;


namespace KantorLr1.Model.IterativeSearching
{
	public struct IterativeMethodSearch
	{
		public int NumberOfIterations { get; set; }
		public IterativeMethodType IterativeMethodType { get; set; }
		public IterativeMethodSearch(int numberOfIterations, IterativeMethodType type)
		{
			NumberOfIterations = numberOfIterations;
			IterativeMethodType = type;
		}
	}
}
