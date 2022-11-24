using LiveCharts;
using LiveCharts.Defaults;
using OSA_Lab5.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace OSA_Lab5
{
    public class MainViewModel : ViewModelBase
    {
        public ChartValues<ObservablePoint> MainLineSeriesValues { get; set; }

        public ChartValues<ObservablePoint> AutoCorellationCoefficientsSeries { get; set; }

        public ObservableCollection<MainRowModel> Rows { get; set; }

        public ObservableCollection<PeriodSeasonsRowModel> PeriodSeasonsRows { get; set; }

        public ObservableCollection<AnalyticAligmentRowModel> AnalyticAligmentsRows { get; set; }

        enum Mode
        {
            Additive,
            Multiplicate
        }
        private Mode CurrentMode;



        #region Propertys

        int period;
        public int Period
        {
            get => period;
            set
            {
                period = value;
                OnPropertyChanged("Period");
            }
        }
        double maxAutoCorellationCoefficients;
        public double MaxAutoCorellationCoefficient
        {
            get => maxAutoCorellationCoefficients;
            set
            {
                maxAutoCorellationCoefficients = value;
                OnPropertyChanged("MaxAutoCorellationCoefficient");
            }
        }
        double correctCoefficient;
        public double CorrectCoefficient
        {
            get => correctCoefficient;
            set
            {
                correctCoefficient = value;
                OnPropertyChanged("CorrectCoefficient");
            }
        }
        double rPow;
        public double RPow
        {
            get => rPow;
            set
            {
                rPow = value;
                OnPropertyChanged("RPow");
            }
        }
        string stringQuality;
        public string StringQuality
        {
            get => stringQuality;
            set
            {
                stringQuality = value;
                OnPropertyChanged("StringQuality");
            }
        }
        double percentQuality;
        public double PercentQuality
        {
            get => percentQuality;
            set
            {
                percentQuality = value;
                OnPropertyChanged("PercentQuality");
            }
        }
        double b1Coefficient;
        public double B1Coefficient
        {
            get => b1Coefficient;
            set
            {
                b1Coefficient = value;
                OnPropertyChanged("B1Coefficient");
            }
        }
        double b0Coefficient;
        public double B0Coefficient
        {
            get => b0Coefficient;
            set
            {
                b0Coefficient = value;
                OnPropertyChanged("B0Coefficient");
            }
        }
        int predictT;
        public int PredictT
        {
            get => predictT;
            set
            {
                predictT = value;
                OnPropertyChanged("PredictT");
            }
        }
        double predictValue;
        public double PredictValue
        {
            get => predictValue;
            set
            {
                predictValue = value;
                OnPropertyChanged("PredictValue");
            }
        }
        double averageValue;
        public double AverageValue
        {
            get => averageValue;
            set
            {
                averageValue = value;
                OnPropertyChanged("AverageValue");
            }
        }
        double sumValue;
        public double SumValue
        {
            get => sumValue;
            set
            {
                sumValue = value;
                OnPropertyChanged("SumValue");
            }
        }
        #endregion
        

        #region Commands
        private ICommand calculateAdditiveCommand;
        public ICommand CalculateAdditiveCommand
        {
            get
            {
                if (calculateAdditiveCommand == null)
                {
                    calculateAdditiveCommand = new RelayCommand(
                        param => this.CalculateAdditive()
                    );
                }
                return calculateAdditiveCommand;
            }
        }


        private ICommand сalculateMultiplicateCommand;
        public ICommand CalculateMultiplicateCommand
        {
            get
            {
                if (сalculateMultiplicateCommand == null)
                {
                    сalculateMultiplicateCommand = new RelayCommand(
                        param => this.CalculateMultiplicate()
                    );
                }
                return сalculateMultiplicateCommand;
            }
        }


        private ICommand predictCommand;
        public ICommand PredictCommand
        {
            get
            {
                if (predictCommand == null)
                {
                    predictCommand = new RelayCommand(
                        param => this.Predict()
                    );
                }
                return predictCommand;
            }
        }
        #endregion

        public void CalculateAdditive()
        {
            CurrentMode = Mode.Additive;
            Start();
        }
        public void CalculateMultiplicate()
        {
            CurrentMode = Mode.Multiplicate;
            Start();
        }

        public void Start()
        {
            ClearAll();
            AverageValue = Rows.Select(x => x.Value).Average();
            SumValue = Rows.Select(x => x.Value).Sum();
            DrawMainChart();
            CalculateAutocorrelationCoefficient();
            CalculateMovingAverage();
            CalculateCentredMovingAverage();
            CalculateEstimationSeasonalComponent();
            CalculatePeriodSeasonsValues();
            CalculateCorrectCoefficient();

            switch (CurrentMode)
            {
                case Mode.Additive:
                    CalculateYminusS();
                    CalculateAnalyticalAligmentOfSeriesDynamicsInAStraightLine();
                    CalculateTplusS();
                    break;
                case Mode.Multiplicate:
                    CalculateYSplitS();
                    CalculateAnalyticalAligmentOfSeriesDynamicsInAStraightLine();
                    CalculateTMulS();
                    break;
                default:
                    break;
            }
            CalculateE();
            CalculateQuality();

        }

        public void ClearAll()
        {
            MainLineSeriesValues.Clear();
            AutoCorellationCoefficientsSeries.Clear();
            AnalyticAligmentsRows.Clear();
            PeriodSeasonsRows.Clear();
            AverageValue = 0;
            SumValue = 0;
            Period = 0;
            PeriodSeasonsRowModel.id = 1;
            foreach (var item in Rows)
            {
                item.MovingAverage = 0;
                item.CentredMovingAverage = 0;
                item.EstimationSeasonalComponent = 0;
                item.S = 0;
                item.EPow = 0;
                item.T = 0;
            }
        }


        public void DrawMainChart()
        {
            foreach (var item in Rows)
            {
                MainLineSeriesValues.Add(new ObservablePoint(item.X,item.Value));
            }            
        }

        #region Calculating
        /// <summary>
        /// Вычисление коэффициента автокорреляции
        /// </summary>
        public void CalculateAutocorrelationCoefficient()
        {
            for (int startindex = 1; startindex <= Rows.Count/4+1; startindex++)
            {
                double Numerator = 0;
                double LeftDenominator = 0;
                double RightDenominator = 0;

                double PrevioslyAverage = Rows.Select(x => x.Value).Take(Rows.Count - startindex).Average();
                for (int i = startindex; i < Rows.Count; i++)
                {
                    Numerator += (Rows[i].Value - AverageValue) * (Rows[i - startindex].Value - PrevioslyAverage);
                    LeftDenominator += Math.Pow(Rows[i].Value - AverageValue, 2);
                    RightDenominator += Math.Pow(Rows[i - startindex].Value - PrevioslyAverage, 2);
                }

                double result = Math.Abs(Numerator / Math.Sqrt(LeftDenominator * RightDenominator));
                AutoCorellationCoefficientsSeries.Add(new ObservablePoint(startindex, result));
            }
            MaxAutoCorellationCoefficient = AutoCorellationCoefficientsSeries.Max(y => y.Y);
            Period = (int)AutoCorellationCoefficientsSeries.First(x => x.Y == AutoCorellationCoefficientsSeries.Max(y => y.Y)).X;
        }

        /// <summary>
        /// Вычисление скользящего среднего
        /// </summary>
        public void CalculateMovingAverage()
        {
            int CountValuesInPeriod = Rows.Count / Period;

            int iterator = 0;

            for (int i = (int)Math.Ceiling((double)CountValuesInPeriod / 2); i <= Rows.Count - CountValuesInPeriod / 2; i++)
            {
                Rows[i - 1].MovingAverage = Rows.Select(x => x.Value).Skip(iterator).Take(CountValuesInPeriod).Average();
                iterator++;
            }
        }

        /// <summary>
        /// Вычисление центрированного скользяжего среднего
        /// </summary>
        public void CalculateCentredMovingAverage()
        {
            int CountValuesInPeriod = Rows.Count / Period;
            int iterator = 0;

            for (int i = (int)Math.Ceiling((double)CountValuesInPeriod / 2) + 1; i <= Rows.Count - CountValuesInPeriod / 2; i++)
            {
                Rows[i - 1].CentredMovingAverage = Rows.SkipWhile(x => x.MovingAverage == 0).Skip(iterator).Take(2).Select(x => x.MovingAverage).Average();
                iterator++;
            }
        }

        /// <summary>
        /// Вычисление сезонной компоненты
        /// </summary>
        public void CalculateEstimationSeasonalComponent()
        {
            switch (CurrentMode)
            {
                case Mode.Additive:
                    foreach (var item in Rows.Where(x => x.CentredMovingAverage != 0))
                    {
                        item.EstimationSeasonalComponent = item.Value - item.CentredMovingAverage;
                    }
                    break;
                case Mode.Multiplicate:
                    foreach (var item in Rows.Where(x => x.CentredMovingAverage != 0))
                    {
                        item.EstimationSeasonalComponent = item.Value / item.CentredMovingAverage;
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Вычисление оценки сезонной компоненты
        /// </summary>
        public void CalculatePeriodSeasonsValues()
        {
            int CountValuesInPeriod = Rows.Count / Period;
            for (int i = 0; i < CountValuesInPeriod; i++)
            {
                var temp = Rows.Where(x => (x.Id-1) % CountValuesInPeriod == i).Select(x=>x.EstimationSeasonalComponent);
                if (temp.Where(x => x != 0).Count() == 0)
                {
                    PeriodSeasonsRows.Add(new PeriodSeasonsRowModel(temp.Average(), temp.Sum()));
                }
                else
                {
                    PeriodSeasonsRows.Add(new PeriodSeasonsRowModel(temp.Where(x => x != 0).Average(), temp.Sum()));
                }
            }
        }

        /// <summary>
        /// Вычисление корректирующего коэффициента
        /// </summary>
        public void CalculateCorrectCoefficient()
        {
            int CountValuesInPeriod = Rows.Count / Period;
            var sum = PeriodSeasonsRows.Select(x => x.AverageValue).Sum();

            switch (CurrentMode)
            {
                case Mode.Additive:
                    CorrectCoefficient = sum / CountValuesInPeriod;
                    foreach (var item in PeriodSeasonsRows)
                    {
                        item.CorrectAverageValue = item.AverageValue - CorrectCoefficient;
                    }
                    break;
                case Mode.Multiplicate:
                    CorrectCoefficient = CountValuesInPeriod / sum;
                    foreach (var item in PeriodSeasonsRows)
                    {
                        item.CorrectAverageValue = item.AverageValue * CorrectCoefficient;
                    }
                    break;
                default:
                    break;
            }

            for (int i = 0; i < CountValuesInPeriod; i++)
            {
                for (int j = 0; j < Period; j++)
                {
                    Rows[j * CountValuesInPeriod + i].S = PeriodSeasonsRows[i].CorrectAverageValue;
                }
            }
        }

        /// <summary>
        /// Y - S
        /// </summary>
        public void CalculateYminusS()
        {
            foreach (var item in Rows)
            {
                item.YminusS = item.Value - item.S;
            }
        }

        /// <summary>
        /// Y/S
        /// </summary>
        public void CalculateYSplitS()
        {
            foreach (var item in Rows)
            {
                item.YminusS = item.Value / item.S;
            }
        }

        /// <summary>
        /// Вычисление аналитического выравнивания ряда по линейному тренду
        /// </summary>
        public void CalculateAnalyticalAligmentOfSeriesDynamicsInAStraightLine()
        {
            foreach (var item in Rows)
            {
                AnalyticAligmentsRows.Add(new AnalyticAligmentRowModel(item.X, item.YminusS));
            }
            B1Coefficient = (AnalyticAligmentsRows.Select(x => x.TY).Average() - AnalyticAligmentsRows.Select(x=>x.T).Average() * AnalyticAligmentsRows.Select(x=>x.Y).Average()) / (AnalyticAligmentsRows.Select(x => x.PowT).Average() - Math.Pow(AnalyticAligmentsRows.Select(x=>x.T).Average(),2));
            B0Coefficient = AnalyticAligmentsRows.Select(x => x.Y).Average() - (B1Coefficient * AnalyticAligmentsRows.Select(x => x.T).Average());
            foreach (var item in Rows)
            {
                item.T = B0Coefficient + B1Coefficient * item.X;
            }
        }

        /// <summary>
        /// T + S
        /// </summary>
        public void CalculateTplusS()
        {
            foreach (var item in Rows)
            {
                item.TplusS = item.T + item.S;
            }
        }

        /// <summary>
        /// T * S
        /// </summary>
        public void CalculateTMulS()
        {
            foreach (var item in Rows)
            {
                item.TplusS = item.T * item.S;
            }
        }

        /// <summary>
        /// Вычисление абсолютных ошибок
        /// </summary>
        public void CalculateE()
        {
            switch (CurrentMode)
            {
                case Mode.Additive:
                    foreach (var item in Rows)
                    {
                        item.E = item.Value - (item.T + item.S);
                        item.EPow = Math.Pow(item.E, 2);
                    }
                    break;
                case Mode.Multiplicate:
                    foreach (var item in Rows)
                    {
                        item.E = item.Value / (item.T * item.S);
                        item.EPow = Math.Abs(Math.Pow(item.Value - (item.T * item.S), 2));
                    }
                    break;
                default:
                    break;
            }
        }     

        /// <summary>
        /// Оценка качества построенной модели
        /// </summary>
        public void CalculateQuality()
        {
            switch (CurrentMode)
            {
                case Mode.Additive:
                    RPow = 1 - (Rows.Select(x => x.EPow).Sum() / (Rows.Select(x => Math.Pow(x.Value - Rows.Select(y => y.Value).Average(), 2)).Sum()));
                    break;
                case Mode.Multiplicate:
                    RPow = 1 - (Rows.Select(x => x.EPow).Average() / (Rows.Select(x => Math.Pow(x.Value - Rows.Select(y => y.Value).Average(), 2)).Sum()));
                    break;
                default:
                    break;
            }
            if (RPow == 1) StringQuality = "Отличное";
            else if (RPow < 1.2 && RPow > 0.8) StringQuality = "Нормальное";
            else StringQuality = "Плохое";
            PercentQuality = Math.Round(RPow * 100, 2);
        }

        #endregion

        /// <summary>
        /// Прогнозирование новых данных
        /// </summary>
        public void Predict()
        {
            if (B0Coefficient == 0 || B1Coefficient == 0 || Period==0)
            {
                MessageBox.Show("Сначала необходимо сделать рассчет");
                return;
            }
            int CountValuesInPeriod = Rows.Count / Period;

            switch (CurrentMode)
            {
                case Mode.Additive:
                    PredictValue = B0Coefficient + B1Coefficient * predictT + Rows.First(x => x.Id % CountValuesInPeriod == predictT % CountValuesInPeriod).S;
                    break;
                case Mode.Multiplicate:
                    PredictValue = (B0Coefficient + B1Coefficient * predictT) * Rows.First(x => x.Id % CountValuesInPeriod == predictT % CountValuesInPeriod).S;
                    break;
                default:
                    break;
            }
        } 
        public MainViewModel()
        {
            Rows = new ObservableCollection<MainRowModel>();
            AutoCorellationCoefficientsSeries = new ChartValues<ObservablePoint>();
            MainLineSeriesValues = new ChartValues<ObservablePoint>();
            AnalyticAligmentsRows = new ObservableCollection<AnalyticAligmentRowModel>();
            PeriodSeasonsRows = new ObservableCollection<PeriodSeasonsRowModel>();

            Rows.Add(new MainRowModel(1, 5.6));
            Rows.Add(new MainRowModel(2, 4.7));
            Rows.Add(new MainRowModel(3, 5.2));
            Rows.Add(new MainRowModel(4, 9.1));

            Rows.Add(new MainRowModel(5, 7.0));
            Rows.Add(new MainRowModel(6, 5.1));
            Rows.Add(new MainRowModel(7, 6.0));
            Rows.Add(new MainRowModel(8, 10.2));

            Rows.Add(new MainRowModel(9, 8.2));
            Rows.Add(new MainRowModel(10, 5.6));
            Rows.Add(new MainRowModel(11, 6.4));
            Rows.Add(new MainRowModel(12, 10.8));

            Rows.Add(new MainRowModel(13, 9.1));
            Rows.Add(new MainRowModel(14, 6.7));
            Rows.Add(new MainRowModel(15, 7.5));
            Rows.Add(new MainRowModel(16, 11.3)); 

        }
    }
}
