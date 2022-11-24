using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSA_Lab5.Models
{
    public class PeriodSeasonsRowModel
    {
        public static int id {get;set;}

        public int Id { get; set; }
        public double AverageValue { get; set; }
        public double Sum { get; set; }
        public double CorrectAverageValue { get; set; }

        public PeriodSeasonsRowModel(double averageValue , double sum)
        {
            Id = id;
            AverageValue = averageValue;
            Sum = sum;
            id++;
        }
    }
}
