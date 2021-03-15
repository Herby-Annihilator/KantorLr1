using System;
using System.Collections.Generic;
using System.Text;
using CompMathLibrary;
using CompMathLibrary.Creators.MethodCreators.Base;
using CompMathLibrary.Creators.MethodCreators;


namespace KantorLr1.Model.IterativeSearching
{
	public struct IterativeMethodSearch
	{
		public int NumberOfIterations { get; set; }
		public IterativeMethodsCreator IterativeMethodCreator { get; set; }
		public IterativeMethodSearch(int numberOfIterations, IterativeMethodsCreator creator)
		{
			NumberOfIterations = numberOfIterations;
			IterativeMethodCreator = creator;
		}
	}
}
