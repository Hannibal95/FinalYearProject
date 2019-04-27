using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace FinalProjectApp.Classes
{
    public class GenerateGraph
    {
        FinalProjectDbEntities context = new FinalProjectDbEntities();
        CreateOptionsPlot createOptionsPlot = new CreateOptionsPlot();
        CreateCallValuesChart createCallValuesChart = new CreateCallValuesChart();

        private Canvas canGraph;
        private double minAcceptableRisk, maxAcceptableRisk;
        private List<ChecklistItems> checklist;
        private double maxValue, minValue, maxRisk, minRisk;
        private double yUpperLimit, xUpperLimit, yLowerLimit, xLowerLimit;
        private bool labelSwitch = false;

        public Canvas GetGraph(Canvas canvas, double minVal, double maxRisk, List<ChecklistItems> checklistItems)
        {
            canGraph = canvas;
            minAcceptableRisk = minVal;
            maxAcceptableRisk = maxRisk;
            checklist = checklistItems;

            if (SysVars.CurrentGraphType == GraphTypes.OptionsPlot) { OptionsPlot(); }
            if (SysVars.CurrentGraphType == GraphTypes.CallValuesChart) { CallValuesChart(); }

            return canGraph;
        }

        private Canvas CallValuesChart()
        {
            List<Option> optionList = GetOptions();

            foreach (var item in checklist)
            {
                if (item.Id == 1 && item.IsChecked)
                {
                    yUpperLimit = optionList.Max(s => s.CallValue);
                    yLowerLimit = optionList.Min(s => s.CallValue - ((optionList.Min(t => t.CallValue) / 100) * 5));
                }
                else if (item.Id == 1 && item.IsChecked == false)
                {
                    yUpperLimit = optionList.Max(s => s.CallValue) + ((optionList.Max(t => t.CallValue) / 100) * 5);
                    yLowerLimit = 0;
                }

                if (item.Id == 2 && item.IsChecked) { labelSwitch = true; }
                else if (item.Id == 2 && item.IsChecked == false) { labelSwitch = false; }
            }

            canGraph = createCallValuesChart.CreateGraph(canGraph, optionList, yUpperLimit, yLowerLimit, labelSwitch);

            return canGraph;
        }

        private Canvas OptionsPlot()
        {
            List<Option> optionList = GetOptions();
            
            foreach (var item in checklist)
            {
                if (item.Id == 1 && item.IsChecked)
                {
                    maxValue = optionList.Max(s => s.ValueToCostRatio);
                    minValue = optionList.Min(s => s.ValueToCostRatio);
                    maxRisk = optionList.Max(s => s.Volatility);
                    minRisk = optionList.Min(s => s.Volatility);

                    yUpperLimit = maxRisk + ((maxRisk / 100) * 10);
                    yLowerLimit = 0;
                    xUpperLimit = maxValue + ((maxValue / 100) * 10);
                    xLowerLimit = minValue - ((maxValue / 100) * 10);
                }
                else if (item.Id == 1 && item.IsChecked == false)
                {
                    maxValue = optionList.Max(s => s.ValueToCostRatio);
                    yUpperLimit = 100;
                    yLowerLimit = 0;
                    xUpperLimit = maxValue;
                    xLowerLimit = 0;
                }

                if (item.Id == 2 && item.IsChecked)
                {
                    for (int i = 0; i < optionList.Count(); i++)
                    {
                        if (optionList[i].Volatility < minAcceptableRisk && optionList[i].ValueToCostRatio < 1)
                        {
                            optionList.Remove(optionList[i]);
                            i--;
                        }
                        else if (optionList[i].Volatility > maxAcceptableRisk)
                        {
                            optionList.Remove(optionList[i]);
                            i--;
                        }
                    }
                }

                if (item.Id == 3 && item.IsChecked)
                {
                    for (int i = 0; i < optionList.Count(); i++)
                    {
                        if (optionList[i].Volatility > minAcceptableRisk && optionList[i].ValueToCostRatio < 1 && optionList[i].Volatility < maxAcceptableRisk)
                        {
                            optionList.Remove(optionList[i]);
                            i--;
                        }
                    }
                }

                if (item.Id == 4 && item.IsChecked)
                {
                    for (int i = 0; i < optionList.Count(); i++)
                    {
                        if (optionList[i].Volatility > minAcceptableRisk && optionList[i].ValueToCostRatio >= 1 && optionList[i].Volatility < maxAcceptableRisk)
                        {
                            optionList.Remove(optionList[i]);
                            i--;
                        }
                    }
                }

                if (item.Id == 5 && item.IsChecked)
                {
                    for (int i = 0; i < optionList.Count(); i++)
                    {
                        if (optionList[i].Volatility < minAcceptableRisk && optionList[i].ValueToCostRatio >= 1)
                        {
                            optionList.Remove(optionList[i]);
                            i--;
                        }
                    }
                }

                if (item.Id == 6 && item.IsChecked) { labelSwitch = true; }
                else if (item.Id == 6 && item.IsChecked == false) { labelSwitch = false; }
            }
            
            canGraph = createOptionsPlot.CreateGraph(canGraph, optionList, 
                xUpperLimit, xLowerLimit ,yUpperLimit, yLowerLimit, minAcceptableRisk, maxAcceptableRisk,
                labelSwitch);

            return canGraph;
        }

        private List<Option> GetOptions()
        {
            List<Option> optionList = new List<Option>();
            var optionQuery = (from x in context.Options
                               where x.BuildId == SysVars.CurrentBuildId
                               select x);
            foreach (var option in optionQuery)
            {
                optionList.Add(option);
            }

            return optionList;
        }
    }
}
