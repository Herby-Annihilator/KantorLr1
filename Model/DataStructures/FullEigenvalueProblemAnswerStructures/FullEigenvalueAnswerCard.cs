using System;
using System.Collections.Generic;
using System.Text;
using CompMathLibrary.Extensions;
using KantorLr1.Model.Extensions;

namespace KantorLr1.Model.DataStructures.FullEigenvalueProblemAnswerStructures
{
	public class FullEigenvalueAnswerCard
	{
		public double Eigenvalue { get; set; }
		public string Eigenvector { get; set; }
		public string Residuals { get; set; }

		public FullEigenvalueAnswerCard(double eigenvalue, double[] eigenvector, double[] residuals)
		{
			Eigenvalue = eigenvalue;
			Eigenvector = eigenvector.GetEquivalentString();
			Residuals = residuals.GetEquivalentString();
		}
	}
}
