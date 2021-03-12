using KantorLr1.Infrastructure.Converters.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace KantorLr1.Infrastructure.Converters
{
	public class DiagonalDominanceConditionConverter : Converter
	{
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is bool?)) return null;
			if ((bool?)value == true)
				return "Выполнено";
			else if ((bool?)value == false)
				return "Не выполнено";
			return "";
		}
	}
}
