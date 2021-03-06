﻿using KantorLr1.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using KantorLr1.Infrastructure.Commands;
using System.Windows.Input;
using CompMathLibrary;
using System.IO;

namespace KantorLr1.ViewModels
{
	public class AutoGenerateDataViewModel : BaseViewModel
	{
		private string title = "Hello";
		public string Title { get => title; set => Set(ref title, value); }

		private const string FILE_TO_SAVE_DATA = "input2.dat";
		private const string APPROXIMATION_FILE = "approx.dat";

		private double[][] matrix;
		private double[] vector;
		private double[] approximation;

		public AutoGenerateDataViewModel()
		{
			matrix = null;
			vector = null;
			approximation = null;
			Title = "Hello";
			GenerateMatrixCommand = new LambdaCommand(OnGenerateMatrixCommandExecuted, CanGenerateMatrixCommandExecute);
			GenerateVectorCommand = new LambdaCommand(OnGenerateVectorCommandExecuted, CanGenerateVectorCommandExecute);
			GenerateApproximationCommand = new LambdaCommand(OnGenerateApproximationCommandExecuted, CanGenerateApproximationCommandExecute);
			AcceptCommand = new LambdaCommand(OnAcceptCommandExecuted, CanAcceptCommandExecute);
		}

		#region Properties
		private string countOfRows;
		public string CountOfRows { get => countOfRows; set => Set(ref countOfRows, value); }

		private string matrixMin;
		public string MatrixMin { get => matrixMin; set => Set(ref matrixMin, value); }

		private string matrixMax;
		public string MatrixMax { get => matrixMax; set => Set(ref matrixMax, value); }

		private string vectorMin;
		public string VectorMin { get => vectorMin; set => Set(ref vectorMin, value); }

		private string vectorMax;
		public string VectorMax { get => vectorMax; set => Set(ref vectorMax, value); }

		private string approximationMin;
		public string ApproximationMin { get => approximationMin; set => Set(ref approximationMin, value); }

		private string approximationMax;
		public string ApproximationMax { get => approximationMax; set => Set(ref approximationMax, value); }

		private string matrixResult;
		public string MatrixResult { get => matrixResult; set => Set(ref matrixResult, value); }

		private string vectorResult;
		public string VectorResult { get => vectorResult; set => Set(ref vectorResult, value); }

		private string approximationResult;
		public string ApproximationResult { get => approximationResult; set => Set(ref approximationResult, value); }
		#endregion

		#region Commands
		public ICommand GenerateMatrixCommand { get; }
		private void OnGenerateMatrixCommandExecuted(object param)
		{
			int min = int.Parse(MatrixMin);
			int max = int.Parse(MatrixMax);
			int count = int.Parse(CountOfRows);
			matrix = new double[count][];
			Random random = new Random();
			for (int i = 0; i < count; i++)
			{
				matrix[i] = new double[count];
				for (int j = 0; j < count; j++)
				{
					matrix[i][j] = random.Next(min, max);
				}
			}			
		}
		private bool CanGenerateMatrixCommandExecute(object param)
		{
			return int.TryParse(CountOfRows, out int a) && CheckFields(MatrixMin, MatrixMax);
		}

		public ICommand GenerateVectorCommand { get; }
		private void OnGenerateVectorCommandExecuted(object param)
		{
			int min = int.Parse(MatrixMin);
			int max = int.Parse(MatrixMax);
			int count = int.Parse(CountOfRows);
			vector = new double[count];
			Random random = new Random();
			for (int i = 0; i < count; i++)
			{
				vector[i] = random.Next(min, max);
			}			
		}
		private bool CanGenerateVectorCommandExecute(object param) =>
			int.TryParse(CountOfRows, out int a) && CheckFields(VectorMin, VectorMax);

		public ICommand GenerateApproximationCommand { get; }
		private void OnGenerateApproximationCommandExecuted(object param)
		{
			int min = int.Parse(MatrixMin);
			int max = int.Parse(MatrixMax);
			int count = int.Parse(CountOfRows);
			approximation = new double[count];
			Random random = new Random();
			for (int i = 0; i < count; i++)
			{
				approximation[i] = random.Next(min, max);
			}
			
		}
		private bool CanGenerateApproximationCommandExecute(object param) =>
			int.TryParse(CountOfRows, out int a) && CheckFields(approximationMin, approximationMin);

		public ICommand AcceptCommand { get; }
		private void OnAcceptCommandExecuted(object param)
		{
			CloseWindowDialogCommand closeWindowDialog = new CloseWindowDialogCommand();
			if (matrix != null)
				InputOutput.SaveMatrixToFileCorrectly(FILE_TO_SAVE_DATA, matrix);
			if (vector != null)
				InputOutput.SaveVectorToFileCorrectly(FILE_TO_SAVE_DATA, vector);
			if (approximation != null)
			{
				StreamWriter writer = new StreamWriter(APPROXIMATION_FILE);
				for (int i = 0; i < approximation.Length; i++)
				{
					writer.Write(approximation[i] + " ");
				}
				writer.Close();
			}
			closeWindowDialog.DialogResult = true;
			closeWindowDialog.Execute(param);
		}
		private bool CanAcceptCommandExecute(object param) => !string.IsNullOrWhiteSpace(VectorResult) ||
			!string.IsNullOrWhiteSpace(MatrixResult) || !string.IsNullOrWhiteSpace(ApproximationResult);
		#endregion


		private bool CheckFields(string field1, string field2) =>
			int.TryParse(field1, out int a) && int.TryParse(field2, out int b);
	}
}
