using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.IO;
using KantorLr1.ViewModels.Base;
using KantorLr1.Infrastructure.Commands;
using CompMathLibrary;
using CompMathLibrary.Methods;

namespace KantorLr1.ViewModels
{
	public class MainWindowViewModel : BaseViewModel
	{
		private CMReshala reshala;
		double[][] matrix;
		double[] vector;
		public MainWindowViewModel()
		{
			matrix = null;
			vector = null;
			reshala = new CMReshala();
			SaveVectorCommand = new LambdaCommand(OnSaveVectorCommandExecuted, CanSaveVectorCommandExecute);
			SaveMatrixCommand = new LambdaCommand(OnSaveMatrixCommandExecuted, CanSaveMatrixCommandExecute);
			RestoreDataFromFileCommand = new LambdaCommand(OnRestoreDataFromFileCommandExecuted, 
				CanRestoreDataFromFileCommandExecute);
			GetSolutionCommand = new LambdaCommand(OnGetSolutionCommandExecuted, CanGetSolutionCommandExecute);
		}
		
		#region Properties
		private string status = "Hello";
		public string Status { get => status; set => Set(ref status, value); }

		private string title = "GaussMethod";
		public string Title { get => title; set => Set(ref title, value); }

		private string matrixA;
		public string MatrixA { get => matrixA; set => Set(ref matrixA, value); }

		private string vectorB;
		public string VectorB { get => vectorB; set => Set(ref vectorB, value); }

		private string determinantValue;
		public string DeterminantValue { get => determinantValue; set => Set(ref determinantValue, value); }

		private string productOfMatrixNorms;
		public string ProductOfMatrixNorms { get => productOfMatrixNorms; set => Set(ref productOfMatrixNorms, value); }

		private string inverseMatrix;
		public string InverseMatrix { get => inverseMatrix; set => Set(ref inverseMatrix, value); }

		private string residuals;
		public string Residuals { get => residuals; set => Set(ref residuals, value); }

		private string solutionStatus;
		public string SolutionStatus { get => solutionStatus; set => Set(ref solutionStatus, value); }

		private string solutionAccuracy;
		public string SolutionAccuracy { get => solutionAccuracy; set => Set(ref solutionAccuracy, value); }

		private string solution;
		public string Solution { get => solution; set => Set(ref solution, value); }
		#endregion

		private void ClearResultsFields()
		{
			DeterminantValue = "";
			ProductOfMatrixNorms = "";
			InverseMatrix = "";
			Residuals = "";
			SolutionStatus = "";
			SolutionAccuracy = "";
			Solution = "";
		}

		#region Commands
		public ICommand SaveMatrixCommand { get; }
		private void OnSaveMatrixCommandExecuted(object param)
		{
			try
			{
				string[] rows = MatrixA.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
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
			catch(Exception e)
			{
				Status = "Operation failed. Reason: " + e.Message;
				DestroyMatrix();
			}
		}
		private bool CanSaveMatrixCommandExecute(object param)
		{
			return !string.IsNullOrWhiteSpace(MatrixA);
		}

		public ICommand SaveVectorCommand { get; }
		private void OnSaveVectorCommandExecuted(object param)
		{
			try
			{
				string[] numbers = VectorB.Split(' ', StringSplitOptions.RemoveEmptyEntries);
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
			catch(Exception e)
			{
				Status = "Operation failed. Reason: " + e.Message;
				DestroyVector();
			}			
		}
		private bool CanSaveVectorCommandExecute(object param)
		{
			return !string.IsNullOrWhiteSpace(VectorB);
		}

		public ICommand RestoreDataFromFileCommand { get; }
		private void OnRestoreDataFromFileCommandExecuted(object param)
		{
			try
			{
				double[][] matrix;
				double[] vector;
				InputOutput.ReadVectorBAndMatrixAFromFile("input.dat", out matrix, out vector);
				MatrixA = "";
				VectorB = "";
				if (matrix != null)
				{
					for (int i = 0; i < matrix.GetLength(0); i++)
					{
						for (int j = 0; j < matrix[i].Length; j++)
						{
							MatrixA += matrix[i][j] + " ";
						}
						matrixA += "\r\n";
					}
					DestroyMatrix();
					CacheTheMatrix(matrix);
				}
				if (VectorB != null)
				{
					for (int i = 0; i < vector.Length; i++)
					{
						VectorB += vector[i] + " ";
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

		public ICommand GetSolutionCommand { get; }
		private void OnGetSolutionCommandExecuted(object param)
		{
			try
			{
				ClearResultsFields();
				Answer answer = reshala.SolveSystemOfLinearAlgebraicEquations(matrix, vector,
					MethodType.Gauss);
				SolutionStatus = answer.AnswerStatus.ToString();
				if (answer.AnswerStatus == AnswerStatus.OneSolution)
				{
					if (answer.Solution != null)
					{
						for (int i = 0; i < answer.Solution[0].Length; i++)
						{
							Solution += Math.Round(answer.Solution[0][i], 5) + "\r\n";
						}
						double[][] reversedMatrix = GetReversedMatrix();
						for (int i = 0; i < reversedMatrix.GetLength(0); i++)
						{
							for (int j = 0; j < reversedMatrix[i].GetLength(0); j++)
							{
								InverseMatrix += Math.Round(reversedMatrix[i][j], 5) + " ";
							}
							InverseMatrix += "\r\n";
						}
						double[] resids = GetResiduals(answer);
						for (int i = 0; i < resids.Length; i++)
						{
							Residuals += "String " + i + " has residual: " + Math.Round(resids[i], 5) + "\r\n";
						}
						SetAccuracy(resids);
						DeterminantValue += Math.Round(answer.Determinant, 5);
						double firstNorm = reshala.GetMatrixMNorm(matrix);
						double secondNorm = reshala.GetMatrixMNorm(reversedMatrix);
						ProductOfMatrixNorms = Math.Round((firstNorm * secondNorm), 5).ToString();
					}
				}
			}
			catch(Exception e)
			{
				Status = "Operation failed. Reason: " + e.Message;
			}
			
		}
		private bool CanGetSolutionCommandExecute(object param)
		{
			return !(matrix == null || vector == null);
		}
		#endregion

		private void SetAccuracy(double[] answer)
		{
			int index = FindMaxAbsInArray(answer);
			if (Math.Abs(answer[index]) > 0 && Math.Abs(answer[index]) < 3)
			{
				SolutionAccuracy = "Good";
			}
			else if (Math.Abs(answer[index]) >= 3 && Math.Abs(answer[index]) < 7)
			{
				SolutionAccuracy = "Not good";
			}
			else
			{
				SolutionAccuracy = "Terrible";
			}
		}
		private int FindMaxAbsInArray(double[] arr)
		{
			int index = 0;
			for (int i = 0; i < arr.Length; i++)
			{
				if (Math.Abs(arr[i]) > Math.Abs(arr[index]))
				{
					index = i;
				}
			}
			return index;
		}

		private void CacheTheMatrix(double[][] matr)
		{
			matrix = matr;
		}
		private void DestroyMatrix()
		{
			if (matrix != null)
			{
				for (int i = 0; i < matrix.GetLength(0); i++)
				{
					matrix[i] = null;
				}
			}
			matrix = null;
		}
		private void CacheTheVector(double[] vect)
		{
			vector = vect;
		}
		private void DestroyVector()
		{
			vector = null;
		}

		private double[][] GetReversedMatrix()
		{
			double[][] reversedMatrix = new double[matrix.GetLength(0)][];
			for (int i = 0; i < reversedMatrix.Length; i++)
			{
				reversedMatrix[i] = new double[matrix[i].Length];
			}
			int colsCount = reversedMatrix[0].Length;
			double[] tmpVector = new double[vector.Length];
			List<double[]> currentSolution;
			int nextIndex = 0;
			for (int i = 0; i < colsCount; i++)
			{
				tmpVector[nextIndex] = 1;
				currentSolution = reshala.SolveSystemOfLinearAlgebraicEquations(matrix, tmpVector,
					MethodType.Gauss).Solution;
				for (int j = 0; j < reversedMatrix.GetLength(0); j++)
				{
					reversedMatrix[j][i] = currentSolution[0][j];
				}
				tmpVector[nextIndex] = 0;
				nextIndex++;
			}
			return reversedMatrix;
		}

		private double[] GetResiduals(Answer answer)
		{
			if (answer.Solution != null)
			{
				double[] resid = new double[vector.Length];
				double sum;
				for (int i = 0; i < resid.Length; i++)
				{
					sum = 0;
					for (int j = 0; j < matrix[i].Length; j++)
					{
						sum += matrix[i][j] * answer.Solution[0][j];
					}
					resid[i] = vector[i] - sum;
				}
				return resid;
			}
			return null;
		}
	}
}
