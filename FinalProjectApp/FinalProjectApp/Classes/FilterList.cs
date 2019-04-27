using System.Collections.Generic;

namespace FinalProjectApp.Classes
{
    public class FilterList
    {
        private List<ChecklistItems> FilterObjects;
        public List<ChecklistItems> CreateList(GraphTypes types)
        {
            if (types == GraphTypes.OptionsPlot)
            {
                OptionsPlotFilter();
            }
            else if (types == GraphTypes.CallValuesChart)
            {
                CallValuesChartFilter();
            }
            return FilterObjects;
        }

        private void OptionsPlotFilter()
        {
            FilterObjects = new List<ChecklistItems>();
            FilterObjects.Add(new ChecklistItems
            {
                Id = 1,
                Name = "Graph Axis Scaling",
                IsChecked = false
            });
            FilterObjects.Add(new ChecklistItems
            {
                Id = 2,
                Name = "Remove [Never] Options",
                IsChecked = false
            });
            FilterObjects.Add(new ChecklistItems
            {
                Id = 3,
                Name = "Remove [ProbNever/MaybeLater]",
                IsChecked = false
            });
            FilterObjects.Add(new ChecklistItems
            {
                Id = 4,
                Name = "Remove [ProbLater/MaybeNow]",
                IsChecked = false
            });
            FilterObjects.Add(new ChecklistItems
            {
                Id = 5,
                Name = "Remove [Now] Options",
                IsChecked = false
            });
            FilterObjects.Add(new ChecklistItems
            {
                Id = 6,
                Name = "Add Options Labels",
                IsChecked = false
            });
            FilterObjects.Add(new ChecklistItems
            {
                Id = 7,
                Name = "Add Draggable Legend",
                IsChecked = false
            });
        }
        private void CallValuesChartFilter()
        {
            FilterObjects = new List<ChecklistItems>();
            FilterObjects.Add(new ChecklistItems
            {
                Id = 1,
                Name = "Graph Axis Scaling",
                IsChecked = false
            });
            FilterObjects.Add(new ChecklistItems
            {
                Id = 2,
                Name = "Add Options Labels",
                IsChecked = false
            });
            FilterObjects.Add(new ChecklistItems
            {
                Id = 3,
                Name = "Add Draggable Legend",
                IsChecked = false
            });
        }
    }
}
