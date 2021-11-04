using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelStepStatistics
{
    public class ExportStatisticDataJson
    {
        public string UserName { get; set; }
        public double AverageCountOfSteps { get; set; }
        public int MinCountOfSteps { get; set; }
        public int MaxCountOfSteps { get; set; }

        public Dictionary<string,Data> StatisticDataForPeriod { get; set; }
    }

    public class Data
    {
        public int Rank { get; set; }
        public int Steps { get; set; }
        public string Status { get; set; }
    }
}
