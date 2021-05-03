using KantorLr1.ViewModels.Base;
using KantorLr1.Infrastructure.Commands;
using System.Windows.Input;
using System.Windows.Markup;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using CompMathLibrary;

namespace KantorLr1.ViewModels
{
	[MarkupExtensionReturnType(typeof(PartialEigenvalueProblemViewModel))]
	public class PartialEigenvalueProblemViewModel : BaseViewModel
	{
		private double[][] _matrix;
		private double[] _vector;
		private CMReshala _reshala;
		public PartialEigenvalueProblemViewModel()
		{
			_reshala = new CMReshala();
			SaveVectorCommand = new LambdaCommand(OnSaveVectorCommandExecuted, CanSaveVectorCommandExecute);
			SaveMatrixCommand = new LambdaCommand(OnSaveMatrixCommandExecuted, CanSaveMatrixCommandExecute);
			RestoreDataFromFileCommand = new LambdaCommand(OnRestoreDataFromFileCommandExecuted,
				CanRestoreDataFromFileCommandExecute);
			FindLargestEigenvalueABSAndEigenvectorCommand = new LambdaCommand(
				OnFindLargestEigenvalueABSAndEigenvectorCommandExecuted,
				CanFindLargestEigenvalueABSAndEigenvectorCommandExecute);
			FindEigenvalueClosestToAGivenOneCommand = new LambdaCommand(
				OnFindEigenvalueClosestToAGivenOneCommandExecuted,
				CanFindEigenvalueClosestToAGivenOneCommandExecute);
			FindLargetsEigenvalueCommand = new LambdaCommand(OnFindLargetsEigenvalueCommandExecuted,
				CanFindLargetsEigenvalueCommandExecute);
			FindSmallestEigenvalueAbsComand = new LambdaCommand(
				OnFindSmallestEigenvalueAbsComandExecuted,
				CanFindSmallestEigenvalueAbsComandExecute);
			FindSmallestEigenvalueCommand = new LambdaCommand(OnFindSmallestEigenvalueCommandExecuted,
				CanFindSmallestEigenvalueCommandExecute);
			FindLargestAndSmallestEigenvaluesCommand = new LambdaCommand(
				OnFindLargestAndSmallestEigenvaluesCommandExecuted,
				CanFindLargestAndSmallestEigenvaluesCommandExecute);
		}


		#region Properties
		private string _textMatrix;
		public string TextMatrix { get => _textMatrix; set => Set(ref _textMatrix, value); }

		private string _textVector;
		public string TextVector { get => _textVector; set => Set(ref _textVector, value); }

		private string _textPrecision;
		public string TextPrecision { get => _textPrecision; set => Set(ref _textPrecision, value);  }

		private string _textStartLambda;
		public string TextStartLambda { get => _textStartLambda; set => Set(ref _textStartLambda, value); }

		private string _status = "Default";
		public string Status { get => _status; set => Set(ref _status, value); }

		private string _largestEigenvalueABS;
		public string LargestEigenvalueABS { get => _largestEigenvalueABS; set => Set(ref _largestEigenvalueABS, value); }

		private string _largestEigenvalue;
		public string LargestEigenvalue { get => _largestEigenvalue; set => Set(ref _largestEigenvalue, value); }

		private string _smallestEigenvalue;
		public string SmallestEigenvalue { get => _smallestEigenvalue; set => Set(ref _smallestEigenvalue, value); }

		private string _smallestEigenvalueABS;
		public string SmallestEigenvalueABS { get => _smallestEigenvalueABS; set => Set(ref _smallestEigenvalueABS, value); }

		private string _countOfIterations;
		public string CountOfIterations { get => _countOfIterations; set => Set(ref _countOfIterations, value); }
		#endregion



		#region Commands
		public ICommand SaveMatrixCommand { get; }
		private void OnSaveMatrixCommandExecuted(object param)
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
				InputOutput.SaveMatrixToFileCorrectly("input.dat", matrix);
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
		private bool CanSaveMatrixCommandExecute(object param)
		{
			return !string.IsNullOrWhiteSpace(TextMatrix);
		}

		public ICommand SaveVectorCommand { get; }
		private void OnSaveVectorCommandExecuted(object param)
		{
			try
			{
				string[] numbers = TextVector.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				double[] vector = new double[numbers.Length];
				for (int i = 0; i < numbers.Length; i++)
				{
					vector[i] = Convert.ToDouble(numbers[i]);
				}
				InputOutput.SaveVectorToFileCorrectly("input.dat", vector);
				DestroyVector();
				CacheTheVector(vector);
				Status = "Vector was saved";
			}
			catch (Exception e)
			{
				Status = "Operation failed. Reason: " + e.Message;
				DestroyVector();
			}
		}
		private bool CanSaveVectorCommandExecute(object param)
		{
			return !string.IsNullOrWhiteSpace(TextVector);
		}

		public ICommand RestoreDataFromFileCommand { get; }
		private void OnRestoreDataFromFileCommandExecuted(object param)
		{
			try
			{
				double[][] matrix;
				double[] vector;
				InputOutput.ReadVectorBAndMatrixAFromFile("input.dat", out matrix, out vector);
				TextMatrix = "";
				TextVector = "";
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
				if (TextVector != null)
				{
					for (int i = 0; i < vector.Length; i++)
					{
						TextVector += vector[i] + " ";
					}
					DestroyVector();
					CacheTheVector(vector);
				}
				Status = "Data restored";
			}
			catch (Exception e)
			{
				Status = "Operation failed. Reason: " + e.Message;
				DestroyMatrix();
				DestroyVector();
			}
		}
		private bool CanRestoreDataFromFileCommandExecute(object param)
		{
			return File.Exists("input.dat");
		}

		public ICommand FindLargestEigenvalueABSAndEigenvectorCommand { get; }
		private void OnFindLargestEigenvalueABSAndEigenvectorCommandExecuted(object p)
		{

		}
		private bool CanFindLargestEigenvalueABSAndEigenvectorCommandExecute(object p)
		{
			return true;
		}

		public ICommand FindEigenvalueClosestToAGivenOneCommand { get; }
		private void OnFindEigenvalueClosestToAGivenOneCommandExecuted(object p)
		{

		}
		private bool CanFindEigenvalueClosestToAGivenOneCommandExecute(object p)
		{
			return true;
		}

		public ICommand FindSmallestEigenvalueAbsComand { get; }
		private void OnFindSmallestEigenvalueAbsComandExecuted(object p)
		{

		}
		private bool CanFindSmallestEigenvalueAbsComandExecute(object p)
		{
			return true;
		}

		public ICommand FindSmallestEigenvalueCommand { get; }
		private void OnFindSmallestEigenvalueCommandExecuted(object p)
		{

		}
		private bool CanFindSmallestEigenvalueCommandExecute(object p)
		{
			return true;
		}

		public ICommand FindLargetsEigenvalueCommand { get; }
		private void OnFindLargetsEigenvalueCommandExecuted(object p)
		{

		}
		private bool CanFindLargetsEigenvalueCommandExecute(object p)
		{
			return true;
		}

		public ICommand FindLargestAndSmallestEigenvaluesCommand { get; }
		private void OnFindLargestAndSmallestEigenvaluesCommandExecuted(object p)
		{

		}
		private bool CanFindLargestAndSmallestEigenvaluesCommandExecute(object p)
		{
			return true;
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
		private void CacheTheVector(double[] vect)
		{
			_vector = vect;
		}
		private void DestroyVector()
		{
			_vector = null;
		}
	}
}
