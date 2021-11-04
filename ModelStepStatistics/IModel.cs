using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelStepStatistics
{
    public interface IModel
    {
        ObservableCollection<StatisticsDataForPeriod> StatisticDataForPeriodJson { get; set; }
        Task<ObservableCollection<StatisticsDataForPeriod>> GetStatisticsDataJson(string fileDirectory);
        bool ExportStatisticsDataToJson(string userName, double averageCountOfSteps, int minCountOfSteps, int maxCountOfSteps, string fileDirectory);
        bool ExportStatisticsDataToXML(string userName, double averageCountOfSteps, int minCountOfSteps, int maxCountOfSteps, string fileDirectory);

    }
}
