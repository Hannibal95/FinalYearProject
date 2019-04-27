using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using FinalProjectApp.Classes;

namespace FinalProjectApp.Pages
{
    public partial class CreateComboOption : Page
    {
        public List<ChecklistItems> AvailablePresentationObjects = new List<ChecklistItems>();
        FinalProjectDbEntities context = new FinalProjectDbEntities();
        BlackScholes blackScholes = new BlackScholes();

        public CreateComboOption()
        {
            InitializeComponent();
            PopulateList();
        }

        public void PopulateList()
        {
            ComponentsList.Items.Clear();
            try
            {
                var query = (from x in context.Components
                             where x.ProjectId == SysVars.CurrentProjectId && x.Selected == false
                             select x);
                foreach (var comp in query)
                {
                    Tuple<long, string, string> compListTuple =
                        new Tuple<long, string, string>(comp.Id, comp.Name, "Cost: " + comp.CostToBuild.ToString());

                    AvailablePresentationObjects.Add(new ChecklistItems
                    {
                        Id = comp.Id,
                        Name = compListTuple.ToString(),
                        IsChecked = false
                    });
                }
                ComponentsList.ItemsSource = AvailablePresentationObjects;
            }
            catch { }
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            List<long> checkedIds = new List<long>();
            double totalValue = 0;
            double totalCostVariance = 0;
            double totalCost = 0;
            int totalDays = 0;
            double compoundVolatility = 0;
            string comboName = "";
            string comboDesc = "";

            foreach (var item in AvailablePresentationObjects)
            {
                if (item.IsChecked == true)
                {
                    checkedIds.Add(item.Id);
                }
            }
            if (checkedIds.Count() > 0)
            {
                foreach (var id in checkedIds)
                {
                    var query = (from x in context.Components
                                 where x.Id == id
                                 select x);
                    foreach (var comp in query)
                    {
                        totalValue += comp.ValueToSystem;
                        totalCostVariance += (comp.CostToBuild / 100) * comp.Volatility;
                        totalCost += comp.CostToBuild;
                        totalDays += comp.DurationDays;
                        comboName += "[" + comp.Id.ToString() + "]";
                        comboDesc += "[" + comp.Name + "]";

                    }
                }
                compoundVolatility = (totalCostVariance / totalCost) * 100;

                double callOption = blackScholes.CalculateOption(totalValue, totalCost, totalDays, 1, compoundVolatility);

                Option option = new Option
                {
                    BuildId = SysVars.CurrentBuildId,
                    Name = comboName,
                    Description = "Combined Option of: " + comboDesc,
                    CallValue = callOption,
                    CostToBuild = totalCost,
                    ValueToSystem = totalValue,
                    ValueToCostRatio = totalValue / totalCost,
                    Volatility = compoundVolatility,
                    DurationDays = totalDays,
                    Complete = false
                };
                context.Options.Add(option);
                context.SaveChanges();
                foreach (var id in checkedIds)
                {
                    CompOption compOption = new CompOption
                    {
                        OptionId = option.Id,
                        ComponentId = id
                    };
                    context.CompOptions.Add(compOption);
                    context.SaveChanges();
                    var query = (from x in context.Components
                                 where x.Id == id
                                 select x);
                    foreach (Component comp in query)
                    {
                        comp.Selected = true;
                    }
                    context.SaveChanges();
                }
                MessageBox.Show("Combined Option Creation Successful");
                BuildPage buildPage = new BuildPage();
                NavigationService.GetNavigationService(this).Navigate(buildPage);
            }
            else
            {
                MessageBox.Show("No Components have been selected.");
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            BuildPage buildPage = new BuildPage();
            NavigationService.GetNavigationService(this).Navigate(buildPage);
        }
    }
}
