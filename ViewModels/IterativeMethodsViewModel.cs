﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using CompMathLibrary;
using CompMathLibrary.Methods;
using KantorLr1.Infrastructure.Commands;
using KantorLr1.Model.IterativeSearching;
using System.IO;
using System.Windows.Markup;

namespace KantorLr1.ViewModels
{
	[MarkupExtensionReturnType(typeof(IterativeMethodsViewModel))]
	public class IterativeMethodsViewModel : ContentControlViewModel
	{
		private double[] approximation;
		private const string FILE_TO_SAVE_DATA = "input2.dat";
		private const string APPROXIMATION_FILE = "approx.dat";
		private IterativeMethodType methodType;
		private double precision;

		public IterativeMethodsViewModel()
		{
			reshala = new CMReshala();
			methodType = IterativeMethodType.Jacobi;
			matrix = null;
			vector = null;
			approximation = null;
			PrecisionSearches = new ObservableCollection<PrecisionSearch>();
			IterativeMethodSearches = new ObservableCollection<IterativeMethodSearch>();
			ApproximationSearches = new ObservableCollection<ApproximationSearch>();

			SaveMatrixCommand = new LambdaCommand(OnSaveMatrixCommandExecuted, CanSaveMatrixCommandExecute);
			SaveVectorCommand = new LambdaCommand(OnSaveVectorCommandExecuted, CanSaveVectorCommandExecute);
			SaveApproximationCommand = new LambdaCommand(OnSaveApproximationCommandExecuted, CanSaveApproximationCommandExecute);
			RestoreDataFromFileCommand = new LambdaCommand(OnRestoreDataFromFileCommandExecuted, CanRestoreDataFromFileCommandExecute);
			GetSolutionCommand = new LambdaCommand(OnGetSolutionCommandExecuted, CanGetSolutionCommandExecute);
			RadioButtonCommand = new LambdaCommand(OnRadioButtonCommandExecuted, CanRadioButtonCommandExecute);

		}

		#region Properties

		#region SearchPrecision
		
		private string startPrecision;
		public string StartPrecison { get => startPrecision; set => Set(ref startPrecision, value); }

		private string endPrecision;
		public string EndPrecision { get => endPrecision; set => Set(ref endPrecision, value); }

		private string precisionStep;
		public string PrecisionStep { get => precisionStep; set => Set(ref precisionStep, value); }

		public ObservableCollection<PrecisionSearch> PrecisionSearches { get; set; }
		#endregion

		#region SearchIterativeMethods
		public ObservableCollection<IterativeMethodSearch> IterativeMethodSearches { get; set; }
		#endregion

		#region SearchApproximation
		
		private string approximations;
		public string Approximations { get => approximations; set => Set(ref approximations, value); }

		public ObservableCollection<ApproximationSearch> ApproximationSearches { get; set; }
		#endregion

		#region SolveInputData

		private string matrixA;
		public string MatrixA { get => matrixA; set => Set(ref matrixA, value); }

		private string vectorB;
		public string VectorB { get => vectorB; set => Set(ref vectorB, value); }

		private string desiredPrecision;
		public string DesiredPrecision { get => desiredPrecision; set => Set(ref desiredPrecision, value); }

		private string startApproximation;
		public string StartApproximation { get => startApproximation; set => Set(ref startApproximation, value); }

		#endregion

		#region SolveOutputData
		
		private string solutionStatus;
		public string SolutionStatus { get => solutionStatus; set => Set(ref solutionStatus, value); }

		private string solutionAccuracy;
		public string SolutionAccuracy { get => solutionAccuracy; set => Set(ref solutionAccuracy, value); }

		private string solution;
		public string Solution { get => solution; set => Set(ref solution, value); }

		private string reversedMatrix;
		public string ReversedMatrix { get => reversedMatrix; set => Set(ref reversedMatrix, value); }

		private string residuals;
		public string Residuals { get => residuals; set => Set(ref residuals, value); }

		private string matrixANorm;
		public string MatrixANorm { get => matrixANorm; set => Set(ref matrixANorm, value); }

		private string productOfMatrixNorms;
		public string ProductOfMatrixNorms { get => productOfMatrixNorms; set => Set(ref productOfMatrixNorms, value); }

		private string numberOfIterations;
		public string NumberOfIterations { get => numberOfIterations; set => Set(ref numberOfIterations, value); }

		private string diagonalDominanceCondition;
		public string DiagonalDominanceCondition { get => diagonalDominanceCondition; set => Set(ref diagonalDominanceCondition, value); }


		#endregion

		#endregion

		#region Commands
		#region SolveCommands
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
				InputOutput.SaveMatrixToFileCorrectly(FILE_TO_SAVE_DATA, matrix);
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
				InputOutput.SaveVectorToFileCorrectly(FILE_TO_SAVE_DATA, vector);
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
			return !string.IsNullOrWhiteSpace(VectorB);
		}

		public ICommand SaveApproximationCommand { get; set; }
		private void OnSaveApproximationCommandExecuted(object param)
		{
			try
			{
				string[] numbers = StartApproximation.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				double[] vector = new double[numbers.Length];
				for (int i = 0; i < numbers.Length; i++)
				{
					vector[i] = Convert.ToDouble(numbers[i]);
				}
				StreamWriter writer = new StreamWriter(APPROXIMATION_FILE);
				for (int i = 0; i < vector.Length; i++)
				{
					writer.Write(vector[i] + " ");
				}
				writer.Close();
				DestroyApproximation();
				CacheApproximation(vector);
				Status = "Approximation was saved";
			}
			catch(Exception e)
			{
				Status = "Operation failed. Reason: " + e.Message;
				DestroyApproximation();
			}
		}
		private bool CanSaveApproximationCommandExecute(object param)
		{
			return !string.IsNullOrWhiteSpace(StartApproximation);
		}

		public ICommand RestoreDataFromFileCommand { get; }
		private void OnRestoreDataFromFileCommandExecuted(object param)
		{
			try
			{
				double[][] matrix;
				double[] vector;
				double[] app;
				InputOutput.ReadVectorBAndMatrixAFromFile(FILE_TO_SAVE_DATA, out matrix, out vector);
				MatrixA = "";
				VectorB = "";
				StartApproximation = "";
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
				if (StartApproximation != null)
				{
					StreamReader reader = new StreamReader(APPROXIMATION_FILE);
					StartApproximation = reader.ReadToEnd();
					reader.Close();
					string[] numbers = StartApproximation.Split(" ", StringSplitOptions.RemoveEmptyEntries);
					app = new double[numbers.Length];
					for (int i = 0; i < numbers.Length; i++)
					{
						app[i] = Convert.ToDouble(numbers[i]);
					}
					DestroyApproximation();
					CacheApproximation(app);
				}
				Status = "Data restored";
			}
			catch (Exception e)
			{
				Status = "Operation failed. Reason: " + e.Message;
				DestroyMatrix();
				DestroyVector();
				DestroyApproximation();
			}
		}
		private bool CanRestoreDataFromFileCommandExecute(object param)
		{
			return true;
		}

		public ICommand GetSolutionCommand { get; }
		private void OnGetSolutionCommandExecuted(object param)
		{
			try
			{
				ClearResultFields();
				precision = double.Parse(DesiredPrecision);
				IterativeAnswer answer = reshala.SolveSystemOfLinearAlgebraicEquationsIteratively(
					matrix, vector, approximation, precision, methodType);
				SolutionStatus = answer.AnswerStatus.ToString();
				SetAccuracy(answer.Solution[0]);
				for (int i = 0; i < answer.Solution[0].Length; i++)
				{
					Solution += Math.Round(answer.Solution[0][i], 5) + "\r\n";
				}
				double[] resids = GetResiduals(answer);
				for (int i = 0; i < resids.Length; i++)
				{
					Residuals += "String " + i + " has residual: " + Math.Round(resids[i], 5) + "\r\n";
				}
				double[][] reversedMatrix = reshala.GetReversedMatrix(matrix);
				for (int i = 0; i < reversedMatrix.GetLength(0); i++)
				{
					for (int j = 0; j < reversedMatrix[i].Length; j++)
					{
						ReversedMatrix += reversedMatrix[i][i] + " ";
					}
					ReversedMatrix += "\r\n";
				}
				NumberOfIterations = answer.NumberOfIterations.ToString();
				//
				// Установить условие диагонального преобладания
				//
				double firstNorm = reshala.GetMatrixMNorm(matrix);
				double secondNorm = reshala.GetMatrixMNorm(reversedMatrix);
				ProductOfMatrixNorms = Math.Round((firstNorm * secondNorm), 5).ToString();
				Status = "Successful!";
			}
			catch(Exception e)
			{
				Status = "Operation failed. Reason: " + e.Message;
			}
		}
		private bool CanGetSolutionCommandExecute(object param) =>
			!(string.IsNullOrWhiteSpace(MatrixA) || 
			string.IsNullOrWhiteSpace(VectorB) ||
			string.IsNullOrWhiteSpace(StartApproximation) ||
			!double.TryParse(DesiredPrecision, out double a));

		public ICommand RadioButtonCommand { get; }
		private void OnRadioButtonCommandExecuted(object param)
		{
			if ((string)param == "Jacobi")
			{
				methodType = IterativeMethodType.Jacobi;
			}
			if ((string)param == "Seidel")
			{
				methodType = IterativeMethodType.Seidel;
			}
		}
		private bool CanRadioButtonCommandExecute(object param) => true;
		#endregion
		#endregion

		private void ClearResultFields()
		{
			Solution = "";
			Residuals = "";
			ReversedMatrix = "";
			MatrixANorm = "";
			ProductOfMatrixNorms = "";
			NumberOfIterations = "";
			DiagonalDominanceCondition = "";
		}

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
		private void CacheApproximation(double[] app)
		{
			approximation = app;
		}
		private void DestroyApproximation()
		{
			approximation = null;
		}
	}
}
