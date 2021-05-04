using KantorLr1.ViewModels.Base;
using KantorLr1.Infrastructure.Commands;
using System.Windows.Input;
using System.Windows.Markup;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using CompMathLibrary;
using KantorLr1.Model.Extensions;
using CompMathLibrary.Creators.MethodCreators;

namespace KantorLr1.ViewModels
{
	[MarkupExtensionReturnType(typeof(FullEigenvalueProblemViewModel))]
	public class FullEigenvalueProblemViewModel : BaseViewModel
	{
		private CMReshala _reshala;
		private double[][] _matrix;

		public FullEigenvalueProblemViewModel()
		{
			SaveMatrixCommand = new LambdaCommand(OnSaveMatrixCommandExecuted, CanSaveMatrixCommandExecute);
			RestoreDataCommand = new LambdaCommand(OnRestoreDataCommandExecuted, CanRestoreDataCommandExecute);
			GenerateGilbertMatrixCommand = new LambdaCommand(OnGenerateGilbertMatrixCommandExecuted,
				CanGenerateGilbertMatrixCommandExecute);
			GenerateReportCommand = new LambdaCommand(OnGenerateReportCommandExecuted,
				CanGenerateReportCommandExecute);
			CalculateEigenvaluesAndEigenvectorsCommand = new LambdaCommand(
				OnCalculateEigenvaluesAndEigenvectorsCommandExecuted,
				CanCalculateEigenvaluesAndEigenvectorsCommandExecute);
		}

		#region Properties
		private string _status = "Полная проблема собственных чисел";
		public string Status { get => _status; set => Set(ref _status, value); }

		private string _textMatrix;
		public string TextMatrix { get => _textMatrix; set => Set(ref _textMatrix, value); }

		private string _textPrecision;
		public string TextPrecision { get => _textPrecision; set => Set(ref _textPrecision, value); }

		private string _gilbertMatrixSize;
		public string GilbertMatrixSize { get => _gilbertMatrixSize; set => Set(ref _gilbertMatrixSize, value); }
		#endregion

		#region Commands
		public ICommand SaveMatrixCommand { get; }
		private void OnSaveMatrixCommandExecuted(object p)
		{

		}
		private bool CanSaveMatrixCommandExecute(object p)
		{
			return true;
		}

		public ICommand RestoreDataCommand { get; }
		private void OnRestoreDataCommandExecuted(object p)
		{

		}
		private bool CanRestoreDataCommandExecute(object p)
		{
			return true;
		}

		public ICommand GenerateGilbertMatrixCommand { get; }
		private void OnGenerateGilbertMatrixCommandExecuted(object p)
		{

		}
		private bool CanGenerateGilbertMatrixCommandExecute(object p)
		{
			return true;
		}

		public ICommand GenerateReportCommand { get; }
		private void OnGenerateReportCommandExecuted(object p)
		{

		}
		private bool CanGenerateReportCommandExecute(object p)
		{
			return true;
		}

		public ICommand CalculateEigenvaluesAndEigenvectorsCommand { get; }
		private void OnCalculateEigenvaluesAndEigenvectorsCommandExecuted(object p)
		{

		}
		private bool CanCalculateEigenvaluesAndEigenvectorsCommandExecute(object p)
		{
			return true;
		}
		#endregion
	}
}
