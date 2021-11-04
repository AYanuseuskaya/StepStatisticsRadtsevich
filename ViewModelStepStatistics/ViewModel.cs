using ModelStepStatistics;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;

namespace ViewModelStepStatistics
{
    public class ViewModel
    {
        IModel Model { get; set; }

        private ObservableCollection<StatisticData> _statisticDataForPeriod;
        public ObservableCollection<StatisticData> StatisticDataForPeriod
        {
            get { return _statisticDataForPeriod; }
            set
            {
                _statisticDataForPeriod = value;
            }
        }
        private StatisticData _selectedItem;
        public StatisticData SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
            }
        }
        public PlotModel PlotModel { get; private set; }
        public ICommand DataGridSelectionChanged
        {
            get { return new DelegateCommand<object>(AddDataPoint, FuncToEvaluate); }
        }
        public ICommand ExportToJson
        {
            get { return new DelegateCommand<object>(ExportJson, FuncToEvaluate); }
        }
        public ICommand ExportToXML
        {
            get { return new DelegateCommand<object>(ExportXML, FuncToEvaluate); }
        }
        public ViewModel(IModel model)
        {
            this.Model = model;
            CulculatingStatistics();
            this.PlotModel = new PlotModel { Title = "Шаги за период" };
        }
        private void AddDataPoint(object obj)
        {
            if (_selectedItem != null && _selectedItem.UserName != null)
            {
                AddDataPointInLine(_selectedItem.UserName);
            }
        }
        private void ExportJson(object obj)
        {
            if (_selectedItem != null)
            {
                string fileDirectory = ShowFolderBrowserDialog();
                bool resultExport = Model.ExportStatisticsDataToJson(_selectedItem.UserName, _selectedItem.AverageCountOfSteps, _selectedItem.MinCountOfSteps, _selectedItem.MaxCountOfSteps, fileDirectory);
                if (resultExport)
                {
                    MessageBox.Show("Данные сохранены");
                }
                else
                {
                    MessageBox.Show("Ошибка при сохранении");
                }
            }
        }
        private void ExportXML(object obj)
        {
            if (_selectedItem != null)
            {
                string fileDirectory = ShowFolderBrowserDialog();
                bool resultExport = Model.ExportStatisticsDataToXML(_selectedItem.UserName, _selectedItem.AverageCountOfSteps, _selectedItem.MinCountOfSteps, _selectedItem.MaxCountOfSteps, fileDirectory);
                if (resultExport)
                {
                    MessageBox.Show("Данные сохранены");
                }
                else
                {
                    MessageBox.Show("Ошибка при сохранении");
                }
            }
        }
        private string ShowFolderBrowserDialog()
        {
            string folderPath = null;
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            DialogResult result = folderBrowser.ShowDialog();
            if (!string.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
            {
                folderPath = folderBrowser.SelectedPath;
            }
            return folderPath;
        }
        private bool FuncToEvaluate(object obj)
        {
            return true;
        }
        private void AddDataPointInLine(string userName)
        {
            var stepsByDayLine = new OxyPlot.Series.LineSeries()
            {
                Color = OxyPlot.OxyColors.Red,
                StrokeThickness = 5,
            };
            if (Model.StatisticDataForPeriodJson != null && Model.StatisticDataForPeriodJson.Count > 0)
            {
                foreach (StatisticsDataForPeriod statisticsDataForPeriod in Model.StatisticDataForPeriodJson)
                {
                    StatisticDataJson statisticDataJson = statisticsDataForPeriod.StatisticDatas.Where(q => q.User == userName).FirstOrDefault();
                    if (statisticDataJson != null && statisticsDataForPeriod.Day != null)
                    {
                        stepsByDayLine.Points.Add(new OxyPlot.DataPoint(Convert.ToInt32(statisticsDataForPeriod.Day), statisticDataJson.Steps));
                    }
                }
                this.PlotModel.Series.Clear();
                this.PlotModel.Series.Add(stepsByDayLine);
                this.PlotModel.Axes.Clear();
                this.PlotModel.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Title = "Шаги"
                });
                this.PlotModel.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    Title = "Дни",
                    Minimum = 1
                });
                this.PlotModel.InvalidatePlot(true);
            }
        }
        private async void CulculatingStatistics()
        {
            string fileDirectory = ConfigurationManager.AppSettings["dataUri"];
            _statisticDataForPeriod = new ObservableCollection<StatisticData>();
            var results = (await Model.GetStatisticsDataJson(fileDirectory)).SelectMany(e => e.StatisticDatas).GroupBy(d => d.User);
            if (results != null)
            {
                foreach (IGrouping<string, StatisticDataJson> groupByUser in results)
                {
                    StatisticData statisticData = new StatisticData();
                    statisticData.UserName = groupByUser.Key;
                    statisticData.AverageCountOfSteps = Math.Round(groupByUser.Average(q => q.Steps), 0);
                    statisticData.MinCountOfSteps = groupByUser.Min(q => q.Steps);
                    statisticData.MaxCountOfSteps = groupByUser.Max(q => q.Steps);
                    double condition = Math.Round(statisticData.AverageCountOfSteps * 0.2, 0);
                    if (Math.Round(statisticData.AverageCountOfSteps - statisticData.MinCountOfSteps, 0) > condition || Math.Round(statisticData.MaxCountOfSteps - statisticData.AverageCountOfSteps, 0) > condition)
                    {
                        statisticData.IsActive = true;
                    }
                    else
                    {
                        statisticData.IsActive = false;
                    }
                    _statisticDataForPeriod.Add(statisticData);
                }
            }
        }
    }
}
