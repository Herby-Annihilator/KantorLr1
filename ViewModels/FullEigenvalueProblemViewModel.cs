using KantorLr1.ViewModels.Base;
using KantorLr1.Infrastructure.Commands;
using System.Windows.Input;
using System.Windows.Markup;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using CompMathLibrary;
using CompMathLibrary.Extensions;
using KantorLr1.Model.Extensions;
using KantorLr1.Model.DataStructures.FullEigenvalueProblemAnswerStructures;
using System.Collections.ObjectModel;
using CompMathLibrary.Creators.MethodCreators;
using CompMathLibrary.EigenvalueProblems.Answers;

namespace KantorLr1.ViewModels
{
	[MarkupExtensionReturnType(typeof(FullEigenvalueProblemViewModel))]
	public class FullEigenvalueProblemViewModel : BaseViewModel
	{
		private CMReshala _reshala;
		private double[][] _matrix;

		public FullEigenvalueProblemViewModel()
		{
			_reshala = new CMReshala();
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
		public ObservableCollection<ComparingReportCard> ComparingReportCards { get; set; } = new ObservableCollection<ComparingReportCard>();
		public ObservableCollection<FullEigenvalueAnswerCard> FullEigenvalueAnswerCards { get; set; } = new ObservableCollection<FullEigenvalueAnswerCard>();

		private string _status = "Полная проблема собственных чисел";
		public string Status { get => _status; set => Set(ref _status, value); }

		private string _textMatrix;
		public string TextMatrix { get => _textMatrix; set => Set(ref _textMatrix, value); }

		private string _textPrecision;
		public string TextPrecision { get => _textPrecision; set => Set(ref _textPrecision, value); }

		private string _gilbertMatrixSize;
		public string GilbertMatrixSize { get => _gilbertMatrixSize; set => Set(ref _gilbertMatrixSize, value); }

		private string _textGilbertMatrix;
		public string TextGilbertMatrix { get => _textGilbertMatrix; set => Set(ref _textGilbertMatrix, value); }
		#endregion

		#region Commands
		public ICommand SaveMatrixCommand { get; }
		private void OnSaveMatrixCommandExecuted(object p)
		{
			try
			{
				string[] rows = TextMatrix.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
				double[][] matrix = new double[rows.Length][];
				for (int i = 0; i < rows.Length; i++)
				{
					string[] elems = rows[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
					matrix[i] = new double[elems.Length];
					for (int j = 0; j < elems.Length; j++)
					{
						matrix[i][j] = Convert.ToDouble(elems[j]);
					}
				}
				InputOutput.SaveMatrixToFile("input.dat", matrix);
				DestroyMatrix();
				CacheTheMatrix(matrix);
				Status = "Matrix was saved";
			}
			catch (Exception e)
			{
				Status = "Operation failed. Reason: " + e.Message;
				DestroyMatrix();
			}
		}
		private bool CanSaveMatrixCommandExecute(object p)
		{
			return !string.IsNullOrWhiteSpace(TextMatrix);
		}

		public ICommand RestoreDataCommand { get; }
		private void OnRestoreDataCommandExecuted(object p)
		{
			try
			{
				double[][] matrix;
				double[] vector;
				InputOutput.ReadVectorBAndMatrixAFromFile("input.dat", out matrix, out vector);
				TextMatrix = "";
				if (matrix != null)
				{
					for (int i = 0; i < matrix.GetLength(0); i++)
					{
						for (int j = 0; j < matrix[i].Length; j++)
						{
							TextMatrix += matrix[i][j] + " ";
						}
						TextMatrix += "\r\n";
					}
					DestroyMatrix();
					CacheTheMatrix(matrix);
				}
				Status = "Data restored";
			}
			catch (Exception e)
			{
				Status = "Operation failed. Reason: " + e.Message;
				DestroyMatrix();
			}
		}
		private bool CanRestoreDataCommandExecute(object p)
		{
			return File.Exists("input.dat");
		}

		public ICommand GenerateGilbertMatrixCommand { get; }
		private void OnGenerateGilbertMatrixCommandExecuted(object p)
		{
			try
			{
				TextGilbertMatrix = "";
				int size = Convert.ToInt32(GilbertMatrixSize);
				TextGilbertMatrix = _reshala.CreateGilbertMatrix(size).GetEqualString();
				Status = "Generated";
			}
			catch(Exception e)
			{
				Status = "Operation failed. Reason: " + e.Message;
			}
		}
		private bool CanGenerateGilbertMatrixCommandExecute(object p)
		{
			return !string.IsNullOrWhiteSpace(GilbertMatrixSize);
		}

		public ICommand GenerateReportCommand { get; }
		private void OnGenerateReportCommandExecuted(object p)
		{
			try
			{
				double precision = Convert.ToDouble(TextPrecision);
				ComparingReportCards.Clear();
				double conditionNumber;
				double[][] gilbertMatrix;
				for (int k = 2; k <= 6; k += 2)
				{
					gilbertMatrix = _reshala.CreateGilbertMatrix(k);
					conditionNumber = _reshala.GetMatrixMNorm(gilbertMatrix);
					gilbertMatrix = _reshala.GetReversedMatrix(gilbertMatrix, new GaussMethodCreator());
					conditionNumber *= _reshala.GetMatrixMNorm(gilbertMatrix);
					RoundMatrix(gilbertMatrix, 5);
					ComparingReportCards.Add(new ComparingReportCard(k, FindRatio(_reshala.SolveFullEigenValueProblem(gilbertMatrix, precision)), conditionNumber));
				}
				Status = "Report generated";
			}
			catch (Exception e)
			{
				Status = "Operation failed. Reason: " + e.Message;
			}
		}
		private bool CanGenerateReportCommandExecute(object p)
		{
			return !string.IsNullOrWhiteSpace(TextPrecision);
		}

		public ICommand CalculateEigenvaluesAndEigenvectorsCommand { get; }
		private void OnCalculateEigenvaluesAndEigenvectorsCommandExecuted(object p)
		{
			try
			{
				double precision = Convert.ToDouble(TextPrecision);
				FullEigenvalueAnswerCards.Clear();
				var answer = _reshala.SolveFullEigenValueProblem(_matrix, precision);
				for (int i = 0; i < answer.Eigenvalues.Length; i++)
				{
					FullEigenvalueAnswerCards.Add(new FullEigenvalueAnswerCard(answer.Eigenvalues[i], answer.Eigenvectors[i], answer.Residuals[i]));
				}
				Status = $"Successful";
			}
			catch(Exception e)
			{
				Status = $"Operation failed. Reason: {e.Message}";
			}
		}
		private bool CanCalculateEigenvaluesAndEigenvectorsCommandExecute(object p)
		{
			return _matrix != null && !string.IsNullOrWhiteSpace(TextPrecision);
		}
		#endregion


		private void CacheTheMatrix(double[][] matr)
		{
			_matrix = matr;
		}
		private void DestroyMatrix()
		{
			if (_matrix != null)
			{
				for (int i = 0; i < _matrix.GetLength(0); i++)
				{
					_matrix[i] = null;
				}
			}
			_matrix = null;
		}

		private double FindRatio(MethodOfRotationsAnswer answer)
		{
			double max = Math.Abs(answer.Eigenvalues[0]);
			double currentMax;
			double min = Math.Abs(answer.Eigenvalues[0]);
			double currentMin;
			for (int i = 0; i < answer.Eigenvalues.Length; i++)
			{
				currentMax = Math.Abs(answer.Eigenvalues[i]);
				currentMin = Math.Abs(answer.Eigenvalues[i]);
				if (currentMax > max)
				{
					max = currentMax;
				}
				if (currentMin < min)
				{
					min = currentMin;
				}
			}
			return max / min;
		}

		private void RoundMatrix(double[][] matrix, int numbersAfterPoint)
		{
			for (int i = 0; i < matrix.GetLength(0); i++)
			{
				for (int j = 0; j < matrix[i].Length; j++)
				{
					matrix[i][j] = Math.Round(matrix[i][j], numbersAfterPoint);
				}
			}
		}
	}
}
