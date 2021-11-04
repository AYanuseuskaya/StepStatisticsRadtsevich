using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelStepStatistics
{
    public class StatisticsDataForPeriod
    {
        public string Day { get; set; }
        public ObservableCollection<StatisticDataJson> StatisticDatas { get; set; }
    }
}
