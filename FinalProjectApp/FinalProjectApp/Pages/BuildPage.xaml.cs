using FinalProjectApp.ProjectPages;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Data;
using System.Linq;
using System.Windows.Media;
using FinalProjectApp.Classes;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

namespace FinalProjectApp.Pages
{
    public partial class BuildPage : Page
    {
        FinalProjectDbEntities context = new FinalProjectDbEntities();

        private List<ChecklistItems> FilterListObjects;
        private BlackScholes blackScholes = new BlackScholes();
        private FilterList filterList = new FilterList();
        private GenerateGraph graphGen = new GenerateGraph();

        private Component component;
        private Image draggedImage;
        private Point mousePosition;

        public BuildPage()
        {
            InitializeComponent();
            CreateFilterList();
            Refresh();
        }

        public void Refresh()
        {
            lblTitle.Content = SysVars.CurrentBuildName;
            lblGraph.Content = SysVars.CurrentGraphType.ToString();
            SysVars.CurrentComponentId = 0;
            SysVars.CurrentOptionId = 0;
            PopulateComponentsByProjectId();
            PopulateOptionsByBuildId();
            SetBudgetandCost();
            txtMaxRisk.Text = SysVars.MaxRisk.ToString();
            txtMinRisk.Text = SysVars.MinRisk.ToString();
            PopulateFilterList();
            canGraph.Children.Clear();

            try
            {
                graphGen.GetGraph(canGraph, SysVars.MinRisk, SysVars.MaxRisk, FilterListObjects);
            }
            catch { }
        }

        private void SetBudgetandCost()
        {
            double totalCost = 0.00;
            double budget = 0.00;
            var buildQuery = (from x in context.Builds
                         where x.Id == SysVars.CurrentBuildId
                         select x);
            foreach (var build in buildQuery)
            {
                budget += build.Budget;
            }
            lblBudget.Content = "Build Budget: " + budget;

            var optionQuery = (from x in context.Options
                               where x.BuildId == SysVars.CurrentBuildId
                               select x);
            foreach (var option in optionQuery)
            {
                totalCost += option.CostToBuild;
            }
            lblCost.Content = "Total Cost of Options: " + totalCost;

            if (totalCost > budget) { lblCost.Foreground = Brushes.Red; }
            else if (totalCost == budget) { lblCost.Foreground = Brushes.Yellow; }
            else { lblCost.Foreground = Brushes.Green; }
        }

        private void CreateFilterList()
        {
            FilterListObjects = filterList.CreateList(SysVars.CurrentGraphType);
        }

        private void PopulateFilterList()
        {
            clFilterList.ItemsSource = FilterListObjects;
        }

        private void AddComp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Tuple<long, string, string> currentCompSelected = (Tuple<long, string, string>)ComponentsList.SelectedItem;

                var query = (from x in context.Components
                             where x.Id == currentCompSelected.Item1
                             select x);
                foreach (var comp in query)
                {
                    component = comp;
                }

                double callOption = blackScholes.CalculateOption(component.ValueToSystem, component.CostToBuild, (component.DurationDays/364.0), 
                    component.RFInterestRate, component.Volatility);

                try
                {
                    Option option = new Option
                    {
                        BuildId = SysVars.CurrentBuildId,
                        Name = component.Name,
                        Description = component.Description,
                        CallValue = callOption,
                        ValueToSystem = component.ValueToSystem,
                        CostToBuild = component.CostToBuild,
                        ValueToCostRatio = component.ValueToSystem / component.CostToBuild,
                        Volatility = component.Volatility,
                        DurationDays= component.DurationDays,
                        Complete = false
                    };
                    context.Options.Add(option);
                    context.SaveChanges();
                    CompOption compOption = new CompOption
                    {
                        OptionId = Convert.ToInt32(option.Id),
                        ComponentId = Convert.ToInt32(component.Id)
                    };
                    context.CompOptions.Add(compOption);
                    context.SaveChanges();
                    var compQuery = (from x in context.Components
                                 where x.Id == component.Id
                                 select x);
                    foreach (Component component in compQuery)
                    {
                        component.Selected = true;
                    }
                    context.SaveChanges();
                    Refresh();
                    System.Windows.MessageBox.Show("Successfully added Option " + option.Name);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Option Creation Failed: " + ex.ToString());
                }
            }
            catch
            {
                System.Windows.MessageBox.Show("No Component Selected");
            }
        }

        private void RemoveOption_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Tuple<long, string, string> currentOptionSelected = (Tuple<long, string, string>)OptionsList.SelectedItem;
                RemoveOption(currentOptionSelected);
                System.Windows.MessageBox.Show("Option Deletion Successful");
                Refresh();
            }
            catch
            {
                System.Windows.MessageBox.Show("No Option Seleted");
            }
        }

        private void RemoveOption(Tuple<long, string, string> currentOptionSelected)
        {
            var optionQuery = (from x in context.Options
                               where x.Id == currentOptionSelected.Item1
                               select x);
            foreach (var option in optionQuery)
            {
                var compOptionQry = (from x in context.CompOptions
                                     where x.OptionId == option.Id
                                     select x);
                foreach (var compOption in compOptionQry)
                {
                    var compQuery = (from x in context.Components
                                     where x.Id == compOption.ComponentId
                                     select x);
                    foreach (var comp in compQuery)
                    {
                        comp.Selected = false;
                    }
                    context.CompOptions.Remove(compOption);
                }
                context.Options.Remove(option);
            }
            context.SaveChanges();
        }

        public void PopulateComponentsByProjectId()
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
                        new Tuple<long, string, string>(comp.Id, comp.Name, "Cost: "+comp.CostToBuild.ToString());
                    ComponentsList.Items.Add(compListTuple);
                }
            }
            catch { }
        }

        public void PopulateOptionsByBuildId()
        {
            OptionsList.Items.Clear();
            try
            {
                var query = (from x in context.Options
                             where x.BuildId == SysVars.CurrentBuildId
                             select x);
                foreach (var option in query)
                {
                    Tuple<long, string, string> optionListTuple =
                        new Tuple<long, string, string>(option.Id, option.Name, "CallOption: " + option.CallValue.ToString());
                    OptionsList.Items.Add(optionListTuple);
                }
            }
            catch { }
        }

        private void NewComponent_Click(object sender, RoutedEventArgs e)
        {
            CreateComponent createComponent = new CreateComponent();
            NavigationService.GetNavigationService(this).Navigate(createComponent);
        }

        private void EditComponent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Tuple<long, string, string> currentCompSelected = (Tuple<long, string, string>)ComponentsList.SelectedItem;
                SetCurrentComponent(currentCompSelected.Item1);
                EditComponent editComponent = new EditComponent();
                NavigationService.GetNavigationService(this).Navigate(editComponent);
            }
            catch
            {
                System.Windows.MessageBox.Show("No Component Selected.");
            }
        }

        public void SetCurrentComponent(long id)
        {
            try { SysVars.CurrentComponentId = id; }
            catch (Exception e) { System.Windows.MessageBox.Show(e.ToString()); }
        }

        private void DeleteComponent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Tuple<long, string, string> currentCompSelected = (Tuple<long, string, string>)ComponentsList.SelectedItem;
                MessageBoxResult dialogResult = System.Windows.MessageBox.Show("You are about to delete component " + currentCompSelected.Item2
                    + ". Are you sure you want to delete this component?", "Component Deletion", MessageBoxButton.OKCancel);
                if (dialogResult == MessageBoxResult.OK)
                {
                    try
                    {
                        var compQuery = (from x in context.Components
                                          where x.Id == currentCompSelected.Item1
                                          select x);
                        foreach (var comp in compQuery)
                        {
                            context.Components.Remove(comp);
                        }

                        context.SaveChanges();
                        Refresh();
                        System.Windows.MessageBox.Show("Component deleted successfully.");
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show("Component deletion failed: " + ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Component deletion failed: " + ex.ToString());
            }
        }

        private void CombineOptions_Click(object sender, RoutedEventArgs e)
        {
            CreateComboOption createComboOption = new CreateComboOption();
            NavigationService.GetNavigationService(this).Navigate(createComboOption);
        }

        private void EditOption_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Tuple<long, string, string> currentOptionSelected = (Tuple<long, string, string>)OptionsList.SelectedItem;
                SetCurrentOption(currentOptionSelected.Item1);
                EditOption editOption = new EditOption();
                NavigationService.GetNavigationService(this).Navigate(editOption);
            }
            catch
            {
                System.Windows.MessageBox.Show("No Option Selected.");
            }
        }

        public void SetCurrentOption(long id)
        {
            try { SysVars.CurrentOptionId = id; }
            catch (Exception e) { System.Windows.MessageBox.Show(e.ToString()); }
        }

        private void ClearOptions_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dialogResult = System.Windows.MessageBox.Show("You are about to delete all the options in this list." +
                    " Are you sure you want to clear the options list?", "Clear Options List", MessageBoxButton.OKCancel);
            if (dialogResult == MessageBoxResult.OK)
            {
                try
                {
                    foreach (var option in OptionsList.Items)
                    {
                        RemoveOption((Tuple<long, string, string>)option);
                    }
                    System.Windows.MessageBox.Show("Options Cleared Successfully");
                    Refresh();
                }
                catch { }
            }
        }

        private void SaveGraph_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                canGraph.UpdateLayout();

                SaveFileDialog s = new SaveFileDialog();
                s.FileName = "OptionsGraph";
                s.DefaultExt = ".png";
                s.Filter = "PNG files (.png)|*.png";

                var result = s.ShowDialog();

                if (result == DialogResult.OK)
                {
                    RenderTargetBitmap rtb = new RenderTargetBitmap((int)canGraph.RenderSize.Width,
                    (int)canGraph.RenderSize.Height, 96, 96, PixelFormats.Pbgra32);

                    Size visualSize = new Size(canGraph.ActualWidth, canGraph.ActualHeight);
                    canGraph.Measure(visualSize);
                    canGraph.Arrange(new Rect(visualSize));

                    rtb.Render(canGraph);

                    BitmapFrame frame = BitmapFrame.Create(rtb);

                    PngBitmapEncoder pngImage = new PngBitmapEncoder();
                    pngImage.Frames.Add(frame);

                    using (Stream fileStream = File.Create(s.FileName))
                    {
                        pngImage.Save(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
            BuildPage buildPage = new BuildPage();
            NavigationService.GetNavigationService(this).Navigate(buildPage);
        }

        private void GraphFilter_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
            if (SysVars.CurrentGraphType == GraphTypes.OptionsPlot && FilterListObjects.ElementAt(6).IsChecked)
            {
                BitmapImage img = new BitmapImage(new Uri("pack://application:,,,/smallOptionsLegend.png", UriKind.Absolute));
                try
                {
                    Image legend = new Image();
                    legend.Source = img;
                    legend.Name = "optionsLegend";
                    legend.MouseLeftButtonDown += CanvasMouseLeftButtonDown;
                    legend.MouseLeftButtonUp += CanvasMouseLeftButtonUp;
                    legend.MouseMove += CanvasMouseMove;
                    canGraph.Children.Add(legend);
                    Canvas.SetTop(legend, 230);
                    Canvas.SetLeft(legend, 450);
                }
                catch (Exception ex) { System.Windows.MessageBox.Show(ex.ToString()); }
            }
            else if (SysVars.CurrentGraphType == GraphTypes.CallValuesChart && FilterListObjects.ElementAt(2).IsChecked)
            {
                //Change to CallValuesChart Legend
                BitmapImage img = new BitmapImage(new Uri("pack://application:,,,/smallCallValuesChartLegend.png", UriKind.Absolute));
                try
                {
                    Image legend = new Image();
                    legend.Source = img;
                    legend.Name = "callValuesLegend"; //Change to CallValuesChart Legend
                    legend.MouseLeftButtonDown += CanvasMouseLeftButtonDown;
                    legend.MouseLeftButtonUp += CanvasMouseLeftButtonUp;
                    legend.MouseMove += CanvasMouseMove;
                    canGraph.Children.Add(legend);
                    Canvas.SetTop(legend, 230);
                    Canvas.SetLeft(legend, 450);
                }
                catch (Exception ex) { System.Windows.MessageBox.Show(ex.ToString()); }
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            StartPage project = new StartPage();
            NavigationService.GetNavigationService(this).Navigate(project);
        }

        private void EditBuild_Click(object sender, RoutedEventArgs e)
        {
            EditBuild editBuild = new EditBuild();
            NavigationService.GetNavigationService(this).Navigate(editBuild);
        }

        private void ProduceReport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ChangeGraph_Click(object sender, RoutedEventArgs e)
        {
            if (SysVars.CurrentGraphType == GraphTypes.OptionsPlot)
            {
                SysVars.CurrentGraphType = GraphTypes.CallValuesChart;
            }
            else if (SysVars.CurrentGraphType == GraphTypes.CallValuesChart)
            {
                SysVars.CurrentGraphType = GraphTypes.OptionsPlot;
            }
            BuildPage buildPage = new BuildPage();
            NavigationService.GetNavigationService(this).Navigate(buildPage);
        }

        private void SetParams_Click(object sender, RoutedEventArgs e)
        {
            //PUT VALIDATION IN HERE
            SysVars.MinRisk = Convert.ToDouble(txtMinRisk.Text);
            SysVars.MaxRisk = Convert.ToDouble(txtMaxRisk.Text);
            Refresh();
        }

        //Left click image event
        private void CanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var image = e.Source as Image;

            if (image != null && canGraph.CaptureMouse())
            {
                mousePosition = e.GetPosition(canGraph);
                draggedImage = image;
            }
        }

        //Release left clicked image event
        private void CanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (draggedImage != null)
            {
                canGraph.ReleaseMouseCapture();
                draggedImage = null;
                canGraph.UpdateLayout();
            }
        }

        //Dragging left clicked image event
        private void CanvasMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (draggedImage != null)
            {
                var position = e.GetPosition(canGraph);
                var offset = position - mousePosition;
                mousePosition = position;
                Canvas.SetLeft(draggedImage, Canvas.GetLeft(draggedImage) + offset.X);
                Canvas.SetTop(draggedImage, Canvas.GetTop(draggedImage) + offset.Y);
            }
        }
    }
}
