using System;

namespace OSA_Lab5.Models
{
    public class AnalyticAligmentRowModel
    {
        public double T { get; set; }
        public double Y { get; set; }
        public double PowT { get; private set; }
        public double PowY { get; private set; }
        public double TY { get; private set; }

        public AnalyticAligmentRowModel(double t, double y)
        {
            this.T = t;
            this.Y = y;
            PowT = Math.Pow(t,2);
            PowY = Math.Pow(y, 2);
            TY = t * y;
        }

    }
}
