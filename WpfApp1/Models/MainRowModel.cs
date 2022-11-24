using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OSA_Lab5.Models
{
    public class MainRowModel : ViewModelBase
    {
        public static int count = 1;

        int id;
        public int Id
        {
            get => id;
            private set
            {
                this.id = value;
                OnPropertyChanged("Id");
            }
        }

        double x;
        public double X
        {
            get => x;
            set
            {
                this.x = value;
                OnPropertyChanged("X");
            }
        }

        double value;
        public double Value
        {
            get => value;
            set
            {
                this.value = value;
            }
        }
        double movingAverage;
        public double MovingAverage
        {
            get => movingAverage;
            set
            {
                this.movingAverage = value;
                OnPropertyChanged("MovingAverage");
            }
        }
        double centredMovingAverage;
        public double CentredMovingAverage
        {
            get => centredMovingAverage;
            set
            {
                this.centredMovingAverage = value;
                OnPropertyChanged("CentredMovingAverage");
            }
        }

        double estimationSeasonalComponent;
        public double EstimationSeasonalComponent
        {
            get => estimationSeasonalComponent;
            set
            {
                this.estimationSeasonalComponent = value;
                OnPropertyChanged("EstimationSeasonalComponent");
            }
        }

        double _YminusS;
        public double YminusS
        {
            get => _YminusS;
            set
            {
                this._YminusS = value;
                OnPropertyChanged("YminusS");
            }
        }

        double s;
        public double S
        {
            get => s;
            set
            {
                this.s = value;
                OnPropertyChanged("S");
            }
        }

        double t;
        public double T
        {
            get => t;
            set
            {
                this.t = value;
                OnPropertyChanged("T");
            }
        }
        double tplusS;
        public double TplusS
        {
            get => tplusS;
            set
            {
                this.tplusS = value;
                OnPropertyChanged("TplusS");
            }
        }

        double e;
        public double E
        {
            get => e;
            set
            {
                this.e = value;
                OnPropertyChanged("E");
            }
        }
        double epow;
        public double EPow
        {
            get => epow;
            set
            {
                this.epow = value;
                OnPropertyChanged("EPow");
            }
        }


        public MainRowModel(double X , double Value)
        {
            Id = count;
            this.X = X;
            this.Value = Value;
            count++;
        }

        ~MainRowModel()
        {
            count--;
        }

        public MainRowModel() : this(count,0)
        {
        }
    }
}
