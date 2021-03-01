using System;
using System.Collections.Generic;
using System.Text;

namespace KantorLr1.Model.IterativeSearching
{
	public struct PrecisionSearch
	{
		public int NumberOfIterations { get; set; }
		public double Precision { get; set; }
		public PrecisionSearch(int numberOfIterations, double precision)
		{
			NumberOfIterations = numberOfIterations;
			Precision = precision;
		}
	}
}
