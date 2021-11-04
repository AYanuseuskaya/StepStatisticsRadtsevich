using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ModelStepStatistics
{
    public class ModelWPF : IModel
    {
        public ObservableCollection<StatisticsDataForPeriod> StatisticDataForPeriodJson { get; set; }
        public ModelWPF()
        {
            StatisticDataForPeriodJson = new ObservableCollection<StatisticsDataForPeriod>();
        }

        public async Task<ObservableCollection<StatisticsDataForPeriod>> GetStatisticsDataJson(string fileDirectory)
        {
            string[] fileEntries = null;
            try
            {
                fileEntries = Directory.GetFiles(fileDirectory);
            }
            catch
            {

            }
            if (fileEntries != null)
            {
                foreach (string fileName in fileEntries)
                {
                   await DeserializeObject(fileName);
                   StatisticDataForPeriodJson = new ObservableCollection<StatisticsDataForPeriod>(StatisticDataForPeriodJson.OrderBy(q => Convert.ToInt32(q.Day)));
                }
            }
            return StatisticDataForPeriodJson;
        }
        public bool ExportStatisticsDataToJson(string userName, double averageCountOfSteps, int minCountOfSteps, int maxCountOfSteps, string fileDirectory)
        {
            ExportStatisticDataJson exportStatisticDataJson = new ExportStatisticDataJson();
            exportStatisticDataJson.UserName = userName;
            exportStatisticDataJson.AverageCountOfSteps = averageCountOfSteps;
            exportStatisticDataJson.MinCountOfSteps = minCountOfSteps;
            exportStatisticDataJson.MaxCountOfSteps = maxCountOfSteps;
            exportStatisticDataJson.StatisticDataForPeriod = new Dictionary<string, Data>();
            if (StatisticDataForPeriodJson != null && StatisticDataForPeriodJson.Count > 0)
            {
                foreach (StatisticsDataForPeriod statisticsDataForPeriod in StatisticDataForPeriodJson)
                {
                    StatisticDataJson statisticDataJson = statisticsDataForPeriod.StatisticDatas.Where(q => q.User == userName).FirstOrDefault();
                    if (statisticDataJson != null)
                    {
                        Data statisticDataForDay = new Data();
                        statisticDataForDay.Rank = statisticDataJson.Rank;
                        statisticDataForDay.Steps = statisticDataJson.Steps;
                        statisticDataForDay.Status = statisticDataJson.Status;
                        exportStatisticDataJson.StatisticDataForPeriod.Add("day" + statisticsDataForPeriod.Day, statisticDataForDay);
                    }
                }
            }
            string filePath = fileDirectory + "\\" + userName + ".json";
            bool result = SerializeObject(filePath, exportStatisticDataJson);
            return result;
        }

        public bool ExportStatisticsDataToXML(string userName, double averageCountOfSteps, int minCountOfSteps, int maxCountOfSteps, string fileDirectory)
        {
            XDocument xdoc = new XDocument();
            XElement user = new XElement("user");
            XAttribute name = new XAttribute("name", userName);
            user.Add(name);
            XElement averageCount = new XElement("averageCountOfSteps", averageCountOfSteps);
            user.Add(averageCount);
            XElement minCount = new XElement("minCountOfSteps", minCountOfSteps);
            user.Add(minCount);
            XElement maxCounts = new XElement("maxCountOfSteps", maxCountOfSteps);
            user.Add(maxCounts);
            XElement statisticDataForPeriod = new XElement("statisticDataForPeriod");
            user.Add(statisticDataForPeriod);
            if (StatisticDataForPeriodJson != null && StatisticDataForPeriodJson.Count > 0)
            {
                foreach (StatisticsDataForPeriod statisticsDataForPeriod in StatisticDataForPeriodJson)
                {
                    StatisticDataJson statisticDataJson = statisticsDataForPeriod.StatisticDatas.Where(q => q.User == userName).FirstOrDefault();
                    if (statisticDataJson != null)
                    {
                        XElement day = new XElement("day", "day" + statisticsDataForPeriod.Day);
                        XElement rank = new XElement("rank", statisticDataJson.Rank);
                        day.Add(rank);
                        XElement steps = new XElement("steps", statisticDataJson.Steps);
                        day.Add(steps);
                        XElement status = new XElement("status", statisticDataJson.Status);
                        day.Add(status);
                        statisticDataForPeriod.Add(day);
                    }
                }
            }
            xdoc.Add(user);
            string filePath = fileDirectory + "\\" + userName + ".xml";
            bool result = false;
            try
            {
                xdoc.Save(filePath);
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }


        private bool SerializeObject (string filePath, ExportStatisticDataJson exportStatisticData)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
                };
                string jsonString = JsonSerializer.Serialize(exportStatisticData, options);
                File.WriteAllText(filePath, jsonString);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private async Task DeserializeObject(string filePath)
        {
            try
            {
                using FileStream openStream = File.OpenRead(filePath);
                ObservableCollection<StatisticDataJson> statisticData = await JsonSerializer.DeserializeAsync<ObservableCollection<StatisticDataJson>>(openStream);
                if (statisticData != null)
                {
                    StatisticsDataForPeriod statisticsDataForPeriod = new StatisticsDataForPeriod();
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    char[] charRemove = { 'd', 'a', 'y' };
                    string day = fileName.TrimStart(charRemove);
                    statisticsDataForPeriod.Day = day;
                    statisticsDataForPeriod.StatisticDatas = statisticData;
                    StatisticDataForPeriodJson.Add(statisticsDataForPeriod);
                }
            }
            catch
            {

            }
        }
    }
}
