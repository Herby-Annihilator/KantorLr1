using System;
using System.Collections.Generic;
using System.Text;

namespace KantorLr1.Model.Extensions
{
	public static class MatrixExtensions
	{
		public static string GetEqualString<T>(this T[][] matrix)
		{
			string result = "";
			for (int i = 0; i < matrix.GetLength(0); i++)
			{
				for (int j = 0; j < matrix[i].Length; j++)
				{
					result += matrix[i][j] + " ";
				}
				result += "\r\n";
			}
			return result;
		}
	}
}
