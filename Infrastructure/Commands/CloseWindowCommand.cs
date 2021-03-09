using KantorLr1.Infrastructure.Commands.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace KantorLr1.Infrastructure.Commands
{
	public class CloseWindowCommand : Command
	{
		public override bool CanExecute(object parameter)
		{
			return parameter is Window;
		}

		public override void Execute(object parameter)
		{
			if (!CanExecute(parameter)) return;
			Window window = (Window)parameter;
			window.Close();
		}
	}
}
