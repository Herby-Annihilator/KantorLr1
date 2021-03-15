using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.IO;
using KantorLr1.Infrastructure.Commands;
using CompMathLibrary;
using CompMathLibrary.Methods;
using CompMathLibrary.Creators.MethodCreators.Base;
using CompMathLibrary.Creators.MethodCreators;
using KantorLr1.Model;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace KantorLr1.ViewModels
{
	[MarkupExtensionReturnType(typeof(MainWindowViewModel))]
	public class MainWindowViewModel : ContentControlViewModel
	{
		private DirectMethodsCreator creator;
		public MainWindowViewModel()
		{
			creator = new GaussMethodCreator();
			matrix = null;
			vector = null;
			reshala = new CMReshala();
			SaveVectorCommand = new LambdaCommand(OnSaveVectorCommandExecuted, CanSaveVectorCommandExecute);
			SaveMatrixCommand = new LambdaCommand(OnSaveMatrixCommandExecuted, CanSaveMatrixCommandExecute);
			RestoreDataFromFileCommand = new LambdaCommand(OnRestoreDataFromFileCommandExecuted, 
				CanRestoreDataFromFileCommandExecute);
			GetSolutionCommand = new LambdaCommand(OnGetSolutionCommandExecuted, CanGetSolutionCommandExecute);
			RadioButtonCommand = new LambdaCommand(OnRadioButtonCommandExecuted, CanRadioButtonCommandExecute);
			CompareCommand = new LambdaCommand(OnCompareCommandExecuted, CanCompareCommandExecute);
			SearchCommand = new LambdaCommand(OnSearchCommandExecited, CanSearchCommandExecute);
		}
		
		#region Properties

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

		private string gaussSolution;
		public string GaussSolution { get => gaussSolution; set => Set(ref gaussSolution, value); }

		private string squareRootSolution;
		public string SquareRootSolution { get => squareRootSolution; set => Set(ref squareRootSolution, value); }

		private string differences;
		public string Differences { get => differences; set => Set(ref differences, value); }
		
		private string numberN;
		public string NumberN { get => numberN; set => Set(ref numberN, value); }

		public ObservableCollection<Dependence> Dependences { get; set; } = new ObservableCollection<Dependence>();
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
					creator);
				SolutionStatus = answer.AnswerStatus.ToString();
				if (answer.AnswerStatus == AnswerStatus.OneSolution)
				{
					if (answer.Solution != null)
					{
						for (int i = 0; i < answer.Solution[0].Length; i++)
						{
							Solution += Math.Round(answer.Solution[0][i], 5) + "\r\n";
						}
						double[][] reversedMatrix = reshala.GetReversedMatrix(matrix, creator);
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
						Status = "Successful!";
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

		public ICommand RadioButtonCommand { get; }
		private void OnRadioButtonCommandExecuted(object param)
		{
			if ((string)param == "Gauss")
			{
				creator = new GaussMethodCreator();
			}
			else if ((string)param == "SquareRoot")
			{
				creator = new SquareRootMethodCreator();
			}
		}
		private bool CanRadioButtonCommandExecute(object param)
		{
			return true;
		}

		public ICommand CompareCommand { get; }
		private void OnCompareCommandExecuted(object param)
		{
			try
			{
				Differences = "";
				GaussSolution = "";
				SquareRootSolution = "";
				Answer gauss = reshala.SolveSystemOfLinearAlgebraicEquations(matrix, vector, creator);
				Answer square = reshala.SolveSystemOfLinearAlgebraicEquations(matrix, vector, creator);
				for (int i = 0; i < gauss.Solution[0].Length; i++)
				{
					GaussSolution += "x" + (i + 1) + " =  " + gauss.Solution[0][i] + "\r\n";
					SquareRootSolution += "x" + (i + 1) + " =  " + square.Solution[0][i] + "\r\n";
					Differences += "Gauss(" + (i + 1) + ") - SquareRoot(" + (i + 1) + ") = " +
						(Math.Abs(gauss.Solution[0][i]) - Math.Abs(square.Solution[0][i])) + "\r\n";
				}
			}
			catch(Exception e)
			{
				Status = "Operation failed. Reason: " + e.Message;
			}
		}
		private bool CanCompareCommandExecute(object param)
		{
			return CanGetSolutionCommandExecute(param);
		}

		public ICommand SearchCommand { get; }
		private void OnSearchCommandExecited(object param)
		{
			try
			{
				int.TryParse(NumberN, out int number);
				Dependences.Clear();
				double conditionNumber;
				double[][] gilbertMatrix;
				for (int k = 2; k <= number; k++)
				{
					gilbertMatrix = reshala.CreateGilbertMatrix(k);
					conditionNumber = reshala.GetMatrixMNorm(gilbertMatrix);
					gilbertMatrix = reshala.GetReversedMatrix(gilbertMatrix, creator);
					conditionNumber *= reshala.GetMatrixMNorm(gilbertMatrix);
					Dependences.Add(new Dependence(k, conditionNumber));
				}
			}
			catch(Exception e)
			{
				Status = "Operation failed. Reason: " + e.Message;
			}			
		}
		
		private bool CanSearchCommandExecute(object param)
		{
			if (int.TryParse(NumberN, out int number))
			{
				if (number > 1 && number < 8)
				{
					return true;
				}
			}
			return false;
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
