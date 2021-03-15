using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using CompMathLibrary;
using CompMathLibrary.Methods;
using CompMathLibrary.Creators.MethodCreators.Base;
using CompMathLibrary.Creators.MethodCreators;
using KantorLr1.Infrastructure.Commands;
using KantorLr1.Model.IterativeSearching;
using System.IO;
using System.Windows.Markup;
using KantorLr1.Model.Extensions;
using System.Threading.Tasks;
using System.Timers;

namespace KantorLr1.ViewModels
{
	[MarkupExtensionReturnType(typeof(IterativeMethodsViewModel))]
	public class IterativeMethodsViewModel : ContentControlViewModel
	{
		private double[] approximation;
		private const string FILE_TO_SAVE_DATA = "input2.dat";
		private const string APPROXIMATION_FILE = "approx.dat";
		private IterativeMethodsCreator creator;
		private double precision;
		private bool isExamplesSearching = false;
		private int searchTimeInMinutes = 10;

		public IterativeMethodsViewModel()
		{
			reshala = new CMReshala();
			creator = new JacobiMethodCreator();
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
			PrecisionSearchCommand = new LambdaCommand(OnPrecisionSearchCommandExecuted, CanPrecisionSearchCommandExecute);
			ClearApproximationTableCommand = new LambdaCommand(OnClearApproximationTableExecuted, CanClearApproximationTableCommandExecute);
			ClearMethodTableCommand = new LambdaCommand(OnClearMethodTableCommandExecuted, CanClearMethodTableCommandExecute);
			ClearPrecisionTableCommand = new LambdaCommand(OnClearPrecisionTableCommandExecuted, CanClearPrecisionTableCommandExecute);
			ApproximationSearchCommand = new LambdaCommand(OnApproximationSearchCommandExecuted, CanApproximationSearchCommandExecute);
			IterativeMethodSearchCommand = new LambdaCommand(OnIterativeMethodSearchCommandExecuted, CanIterativeMethodSearchCommandExecute);
			GetExamplesCommand = new LambdaCommand(OnGetExamplesCommandExecuted, CanGetExamplesCommandExecute);

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
		public string DesiredPrecision
		{
			get => desiredPrecision;
			set
			{
				Set(ref desiredPrecision, value);
				double.TryParse(desiredPrecision, out precision);
			}
		}

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

		private bool? diagonalDominanceCondition;
		public bool? DiagonalDominanceCondition { get => diagonalDominanceCondition; set => Set(ref diagonalDominanceCondition, value); }


		#endregion

		#region SearchExamples
		private string searchStatus;
		public string SearchStatus { get => searchStatus; set => Set(ref searchStatus, value); }
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
		private bool CanRestoreDataFromFileCommandExecute(object param) =>
			File.Exists(FILE_TO_SAVE_DATA) && File.Exists(APPROXIMATION_FILE);


		public ICommand GetSolutionCommand { get; }
		private void OnGetSolutionCommandExecuted(object param)
		{
			try
			{
				ClearResultFields();
				precision = double.Parse(DesiredPrecision);
				DiagonalDominanceCondition = reshala.IsTheConditionOfDiagonalDominanceSatisfied(matrix);
				IterativeAnswer answer = reshala.SolveSystemOfLinearAlgebraicEquationsIteratively(
					matrix, vector, approximation, precision, creator);
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
				double[][] reversedMatrix = reshala.GetReversedMatrix(matrix, new GaussMethodCreator());
				for (int i = 0; i < reversedMatrix.GetLength(0); i++)
				{
					for (int j = 0; j < reversedMatrix[i].Length; j++)
					{
						ReversedMatrix += Math.Round(reversedMatrix[i][j], 5) + " ";
					}
					ReversedMatrix += "\r\n";
				}
				NumberOfIterations = answer.NumberOfIterations.ToString();
				double firstNorm = reshala.GetMatrixMNorm(matrix);
				double secondNorm = reshala.GetMatrixMNorm(reversedMatrix);
				MatrixANorm = firstNorm.ToString();
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
			string.IsNullOrWhiteSpace(StartApproximation)) &&
			double.TryParse(DesiredPrecision, out double a);

		public ICommand RadioButtonCommand { get; }
		private void OnRadioButtonCommandExecuted(object param)
		{
			if ((string)param == "Jacobi")
			{
				creator = new JacobiMethodCreator();
			}
			if ((string)param == "Seidel")
			{
				creator = new SeidelMethodCreator();
			}
		}
		private bool CanRadioButtonCommandExecute(object param) => true;
		#endregion

		#region SearchCommands
		public ICommand PrecisionSearchCommand { get; }
		private void OnPrecisionSearchCommandExecuted(object param)
		{
			try
			{
				double start = double.Parse(StartPrecison);
				double end = double.Parse(EndPrecision);
				double step = double.Parse(PrecisionStep);
				double stepSum = Math.Abs(step);
				double startPrecision;
				double endPrecision;
				IterativeAnswer answer;
				PrecisionSearches.Clear();
				if (end > start)
				{
					startPrecision = start;
					endPrecision = end;
				}
				else
				{
					startPrecision = end;
					endPrecision = start;
				}
				while ((endPrecision - startPrecision) > stepSum)
				{
					answer = reshala.SolveSystemOfLinearAlgebraicEquationsIteratively(matrix, vector,
						approximation, endPrecision - stepSum, creator);
					PrecisionSearches.Add(new PrecisionSearch(answer.NumberOfIterations, endPrecision - stepSum));
					stepSum += Math.Abs(step);
				}
			}
			catch(Exception e)
			{
				Status = e.Message;
			}
		}
		private bool CanPrecisionSearchCommandExecute(object param) =>
			CheckFieldsForPrecisionSearch() && 
			!(string.IsNullOrWhiteSpace(MatrixA) ||
			string.IsNullOrWhiteSpace(VectorB) ||
			string.IsNullOrWhiteSpace(StartApproximation));
		
		public ICommand ApproximationSearchCommand { get; }
		private void OnApproximationSearchCommandExecuted(object param)
		{
			try
			{
				string[] strs = Approximations.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
				string[] numbers;
				double[][] approximations = new double[strs.Length][];
				ApproximationSearches.Clear();
				for (int i = 0; i < strs.Length; i++)
				{
					numbers = strs[i].Split(" ", StringSplitOptions.RemoveEmptyEntries);
					approximations[i] = new double[numbers.Length];
					for (int j = 0; j < numbers.Length; j++)
					{
						approximations[i][j] = Convert.ToDouble(numbers[j]);
					}
				}
				IterativeAnswer answer;
				for (int i = 0; i < strs.Length; i++)
				{
					answer = reshala.SolveSystemOfLinearAlgebraicEquationsIteratively(matrix, vector,
						approximations[i], precision, creator);
					ApproximationSearches.Add(new ApproximationSearch(answer.NumberOfIterations, approximations[i].GetEquivalentString()));
				}
			}
			catch(Exception e)
			{
				Status = e.Message;
			}
		}
		private bool CanApproximationSearchCommandExecute(object param) => !string.IsNullOrWhiteSpace(Approximations);

		public ICommand IterativeMethodSearchCommand { get; }
		private void OnIterativeMethodSearchCommandExecuted(object param)
		{
			IterativeAnswer answer;
			IterativeMethodSearches.Clear();
			try
			{
				answer = reshala.SolveSystemOfLinearAlgebraicEquationsIteratively(matrix, vector,
				approximation, precision, new JacobiMethodCreator());
				IterativeMethodSearches.Add(new IterativeMethodSearch(answer.NumberOfIterations, new JacobiMethodCreator()));
			}
			catch(Exception e)
			{
				Status = "Метод Якоби не справился. Причина " + e.Message;
			}
			try
			{
				answer = reshala.SolveSystemOfLinearAlgebraicEquationsIteratively(matrix, vector,
				approximation, precision, new SeidelMethodCreator());
				IterativeMethodSearches.Add(new IterativeMethodSearch(answer.NumberOfIterations,
					new SeidelMethodCreator()));
			}
			catch(Exception e)
			{
				Status = "Метод Зейделя не справился. Причина " + e.Message;
			}
		}
		private bool CanIterativeMethodSearchCommandExecute(object param) => CanGetSolutionCommandExecute(param);
		public ICommand ClearPrecisionTableCommand { get; }
		private void OnClearPrecisionTableCommandExecuted(object param)
		{
			PrecisionSearches.Clear();
		}
		private bool CanClearPrecisionTableCommandExecute(object param) => true;

		public ICommand ClearApproximationTableCommand { get; }
		private void OnClearApproximationTableExecuted(object param)
		{
			ApproximationSearches.Clear();
		}
		private bool CanClearApproximationTableCommandExecute(object param) => true;

		public ICommand ClearMethodTableCommand { get; }
		private void OnClearMethodTableCommandExecuted(object param)
		{
			IterativeMethodSearches.Clear();
		}
		private bool CanClearMethodTableCommandExecute(object param) => true;

		public ICommand GetExamplesCommand { get; }
		private void OnGetExamplesCommandExecuted(object param)
		{
			searchTimeInMinutes = 10;
			BeginToFindExamples();
		}
		private bool CanGetExamplesCommandExecute(object param) => !isExamplesSearching;
		#endregion
		#endregion

		private bool CheckFieldsForPrecisionSearch()
		{
			if (!double.TryParse(StartPrecison, out double start))
				return false;
			if (!double.TryParse(EndPrecision, out double end))
				return false;
			if (!double.TryParse(PrecisionStep, out double step))
				return false;
			if (end < double.Epsilon || start < double.Epsilon)
				return false;
			return true;
		}

		private void ClearResultFields()
		{
			Solution = "";
			Residuals = "";
			ReversedMatrix = "";
			MatrixANorm = "";
			ProductOfMatrixNorms = "";
			NumberOfIterations = "";
			DiagonalDominanceCondition = null;
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

		private async void BeginToFindExamples()
		{
			Timer timer = new Timer
			{
				Interval = 60000  // 1 min
			};
			timer.Elapsed += Timer_Elapsed;
			timer.Start();
			isExamplesSearching = true;
			SearchStatus = "Выполняю, времени осталось: " + searchTimeInMinutes + " мин";
			await Task.Run(TryToFindExamples);
			isExamplesSearching = false;
			SearchStatus = "Поиск завершен. Ищете файл Examples.txt";
			timer.Stop();
		}

		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			searchTimeInMinutes--;
			SearchStatus = "Выполняю, времени осталось: " + searchTimeInMinutes + " мин";
		}

		private void TryToFindExamples()
		{
			DateTime time = DateTime.Now;
			int minutesToWork = 10;
			int resultCount = 0;
			int necessaryResultCount = 5;
			CMReshala localReshala = new CMReshala();
			int localMatrixSize;
			double[][] workingMatrix;
			double[] localVector;
			double[] localApproximation;
			double localPrecision;
			Random localRandom = new Random();
			IterativeAnswer answer;
			string fileForExamples = "Examples.txt";
			while(DateTime.Now.Subtract(time).TotalMinutes < minutesToWork && resultCount < necessaryResultCount)
			{
				localMatrixSize = localRandom.Next(5, 16);
				workingMatrix = localReshala.CreateMatrixWithoutDiagonalDominance(localMatrixSize, -50, 50);
				localVector = localReshala.CreateRandomVector(localMatrixSize, -50, 50);
				localApproximation = localReshala.CreateRandomVector(localMatrixSize, 0, 10);
				localPrecision = localRandom.NextDouble();
				try
				{
					answer = localReshala.SolveSystemOfLinearAlgebraicEquationsIteratively(workingMatrix, localVector,
					localApproximation, localPrecision, creator);
					WriteExampleToFile(fileForExamples, workingMatrix, localVector, localApproximation,
						localPrecision, localReshala.CalculateDiagonalDominance(workingMatrix), answer.Solution[0]);
					resultCount++;
				}
				catch (Exception)
				{

				}				
			}
		}

		private void WriteExampleToFile(string fileName, double[][] matr, double[] vect, double[] app,
			double prec, double dominantValue, double[] solution)
		{
			StreamWriter writer = new StreamWriter(fileName, true);
			writer.WriteLine("Пример" + "\r\n");
			writer.WriteLine("Матрица" + new string('\t', matr.GetLength(0)) + "Вектор" + "\t" + "Приближение" + "\r\n");
			for (int i = 0; i < matr.GetLength(0); i++)
			{

				writer.Write(matr[i].GetEquivalentString() + new string('\t', matr.GetLength(0)) + vect[i] +
					"\t" + app[i]);
				writer.WriteLine();
			}
			writer.WriteLine("\r\n" + "Точность " + prec + "\r\nЗначение диагонального преобладания " + dominantValue + "\r\n");
			writer.WriteLine("Решение:");
			for (int i = 0; i < solution.Length; i++)
			{
				writer.Write(solution[i] + " ");
			}
			writer.WriteLine();
			writer.WriteLine("\r\n\r\n");
			writer.Close();
		}
	}
}
