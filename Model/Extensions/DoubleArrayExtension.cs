using System;
using System.Collections.Generic;
using System.Text;

namespace KantorLr1.Model.Extensions
{
	public static class DoubleArrayExtension
	{
		public static string GetEquivalentString(this double[] arr)
		{
			string toReturn = "";
			for (int i = 0; i < arr.Length; i++)
			{
				toReturn += arr[i] + " ";
			}
			return toReturn;
		}
	}
}
