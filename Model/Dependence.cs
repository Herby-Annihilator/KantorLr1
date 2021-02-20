using System;
using System.Collections.Generic;
using System.Text;

namespace KantorLr1.Model
{
	public struct Dependence
	{
		public int NumberN { get; private set; }
		public double ConditionNumber { get; private set; }
		public Dependence(int numberN, double conditionNumber)
		{
			NumberN = numberN;
			ConditionNumber = conditionNumber;
		}
	}
}
