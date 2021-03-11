using System;
using System.Collections.Generic;
using System.Text;

namespace KantorLr1.Model.IterativeSearching
{
	public struct ApproximationSearch
	{
		public int NumberOfIterations { get; set; }
		public string Approximation { get; set; }
		public ApproximationSearch(int numberOfIterations, string approximation)
		{
			NumberOfIterations = numberOfIterations;
			Approximation = approximation;
		}
	}
}
