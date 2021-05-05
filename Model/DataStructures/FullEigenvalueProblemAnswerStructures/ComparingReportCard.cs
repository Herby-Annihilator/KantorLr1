using System;
using System.Collections.Generic;
using System.Text;

namespace KantorLr1.Model.DataStructures.FullEigenvalueProblemAnswerStructures
{
	public class ComparingReportCard
	{
		public double RatioOfMaxEigenvalueAbsAndMinEigenvalueAbs { get; set; }
		public double ConditionNumber { get; set; }
		public int Size { get; set; }
		public ComparingReportCard(int size, double ratio, double conditionNumber)
		{
			Size = size;
			ConditionNumber = conditionNumber;
			RatioOfMaxEigenvalueAbsAndMinEigenvalueAbs = ratio;
		}
	}
}
