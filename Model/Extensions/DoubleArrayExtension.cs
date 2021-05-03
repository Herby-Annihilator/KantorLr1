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
		public static T[] Map<T>(this T[] arr, Func<T, T> func)
		{
			T[] toReturn = new T[arr.Length];
			for (int i = 0; i < arr.Length; i++)
			{
				toReturn[i] = func(arr[i]);
			}
			return toReturn;
		}
	}
}
