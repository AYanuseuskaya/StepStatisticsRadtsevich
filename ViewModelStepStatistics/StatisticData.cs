using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModelStepStatistics
{
    public class StatisticData
    {
        public string UserName { get; set; }
        public double AverageCountOfSteps { get; set; }
        public int MinCountOfSteps { get; set; }
        public int MaxCountOfSteps { get; set; }
        public bool IsActive { get; set; }
    }
}
