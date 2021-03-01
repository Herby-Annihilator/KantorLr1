using System;
using System.Collections.Generic;
using System.Text;

namespace KantorLr1.Model.IterativeSearching
{
	public struct ApproximationSearch
	{
		public int NumberOfIterations { get; set; }
		public double[] Approximation { get; set; }
		public ApproximationSearch(int numberOfIterations, double[] approximation)
		{
			NumberOfIterations = numberOfIterations;
			Approximation = approximation;
		}
	}
}
