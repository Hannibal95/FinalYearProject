﻿using FinalProjectApp.Classes;
using FinalProjectApp.Pages;
using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace FinalProjectApp.ProjectPages
{
    public partial class StartPage : Page
    {
        FinalProjectDbEntities context = new FinalProjectDbEntities(); 

        public StartPage()
        {
            InitializeComponent();
            Refresh();
        }

        //Refreshes the page
        public void Refresh()
        {
            SetVisible();
            ClearData();
            PopulateFields();
        }

        //Clears the GUI component data
        public void ClearData()
        {
            projectListItems.Items.Clear();
            buildListItems.Items.Clear();
            txtDescription.Text = "";
            txtStats.Text = "";
            lblProjectName.Content = "";
        }

        //Resets system variables when currently selected project is deleted.
        public void Reset()
        {
            SysVars.CurrentProjectId = 0;
            SysVars.CurrentProjectName = "";
            SysVars.CurrentProjectDescription = "";
            SysVars.CurrentBuildId = 0;
            SysVars.CurrentBuildName = "";
            ClearData();
            Refresh();
        }

        //Set Project Screen GUI Component Visibility
        public void SetVisible()
        {
            if(SysVars.CurrentProjectId == 0)
            {
                lblProjectName.Visibility = Visibility.Hidden;
                lblBuilds.Visibility = Visibility.Hidden;
                lblDescription.Visibility = Visibility.Hidden;
                lblStats.Visibility = Visibility.Hidden;
                txtDescription.Visibility = Visibility.Hidden;
                txtStats.Visibility = Visibility.Hidden;
                buildListItems.Visibility = Visibility.Hidden;
                EditProject.Visibility = Visibility.Hidden;
                DeleteProject.Visibility = Visibility.Hidden;
                OpenBuild.Visibility = Visibility.Hidden;
                NewBuild.Visibility = Visibility.Hidden;
                DeleteBuild.Visibility = Visibility.Hidden;
            }
            else if(SysVars.CurrentProjectId > 0)
            {
                lblProjectName.Visibility = Visibility.Visible;
                lblBuilds.Visibility = Visibility.Visible;
                lblDescription.Visibility = Visibility.Visible;
                lblStats.Visibility = Visibility.Visible;
                txtDescription.Visibility = Visibility.Visible;
                txtStats.Visibility = Visibility.Visible;
                buildListItems.Visibility = Visibility.Visible;
                NewProject.Visibility = Visibility.Visible;
                EditProject.Visibility = Visibility.Visible;
                DeleteProject.Visibility = Visibility.Visible;
                OpenBuild.Visibility = Visibility.Visible;
                NewBuild.Visibility = Visibility.Visible;
                DeleteBuild.Visibility = Visibility.Visible;
            }
        }

        //Populates the GUI component data
        public void PopulateFields()
        {
            lblProjectName.Content = SysVars.CurrentProjectName;
            txtDescription.Text = SysVars.CurrentProjectDescription;
            txtStats.Text = "N/A";
            PopulateProjects();
            if (SysVars.CurrentProjectId > 0)
            {
                PopulateBuildsByProjectId(SysVars.CurrentProjectId);
            }
        }

        //Populates the projects list
        public void PopulateProjects()
        {
            foreach (var proj in context.Projects.ToList())
            {
                projectListItems.Items.Add(proj.Name);
            }
        }

        //When a selected project is opened, this sets the appropriate system variables
        public void SetCurrentProjectByName(string name)
        {
            try
            {
                var query = (from x in context.Projects
                             where x.Name == name
                             select x).Distinct();

                foreach (var project in query)
                {
                    SysVars.CurrentProjectId = project.Id;
                    SysVars.CurrentProjectName = project.Name;
                    SysVars.CurrentProjectDescription = project.Description;
                }
            }
            catch(Exception e) { MessageBox.Show(e.ToString()); }
        }

        //Populates the builds list with all builds connected to the selected project by id.
        public void PopulateBuildsByProjectId(int id)
        {
            buildListItems.Items.Clear();
            try
            {
                var query = (from x in context.Builds
                             where x.ProjectId == id
                             select x);

                foreach (var build in query)
                {
                    Tuple<int, string> listTuple = new Tuple<int, string>(build.Id, build.Version);
                    buildListItems.Items.Add(listTuple);
                }
            }
            catch{ }
        }

        /* Passes currently selected string in project list to the SetCurrentProjectByName method
         * and refreshes the page. */
        private void OpenProject_Click(object sender, RoutedEventArgs e)
        {
            try { 
                string currentProjectSelected = projectListItems.SelectedItem.ToString();
                SetCurrentProjectByName(currentProjectSelected);
                Refresh();
            }
            catch
            {
                MessageBox.Show("No Project Selected.");
            }
        }

        //Sets current page to the CreateProject page
        private void NewProject_Click(object sender, RoutedEventArgs e)
        {
            CreateProject createProject = new CreateProject();
            NavigationService.GetNavigationService(this).Navigate(createProject);
        }
        
        //Sets current page to the EditProject page
        private void EditProject_Click(object sender, RoutedEventArgs e)
        {
            EditProject editProject = new EditProject();
            NavigationService.GetNavigationService(this).Navigate(editProject);
        }

        //Deletes the selected project and its associated assets
        private void DeleteProject_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dialogResult = MessageBox.Show("You are about to delete project "+SysVars.CurrentProjectName
                + ". This will also delete all attached assets. Are you sure you want to delete this project?", "Project Deletion", MessageBoxButton.OKCancel);
            if (dialogResult == MessageBoxResult.OK)
            {
                try
                {
                    //Remove all projects by Id
                    var projQuery = (from x in context.Projects
                                 where x.Id == SysVars.CurrentProjectId
                                 select x);
                    foreach (var project in projQuery)
                    {
                        context.Projects.Remove(project);
                    }

                    //Using Project Id, identify build Ids connected to project and remove all their options
                    var buildOptQuery = (from x in context.Builds
                                          where x.ProjectId == SysVars.CurrentProjectId
                                          select x.Id);
                    foreach (var bId in buildOptQuery)
                    {
                        var optionQuery = (from x in context.Options
                                         where x.BuildId == bId
                                         select x);
                        foreach (var option in optionQuery)
                        {
                            var compOptionQry = (from x in context.CompOptions
                                                 where x.OptionId == option.Id
                                                 select x);
                            foreach (var compOption in compOptionQry)
                            {
                                context.CompOptions.Remove(compOption);
                            }
                            context.Options.Remove(option);
                        }
                    }
                    
                    //Using Project Id, remove all builds connected to the project
                    var buildQuery = (from x in context.Builds
                                     where x.ProjectId == SysVars.CurrentProjectId
                                     select x);
                    foreach (var build in buildQuery)
                    {
                        context.Builds.Remove(build);
                    }

                    //Using Project Id, remove all components connected to the project
                    var compQuery = (from x in context.Components
                                     where x.ProjectId == SysVars.CurrentProjectId
                                     select x);
                    foreach (var comp in compQuery)
                    {
                        context.Components.Remove(comp);
                    }

                    context.SaveChanges();
                    Reset();
                    MessageBox.Show("Project deleted successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Project deletion failed: " + ex.ToString());
                }
            }
        }

        /* Passes currently selected string in build list to the SetCurrentBuildByName method
         * and navigates to the BuildPage. */
        private void OpenBuild_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Tuple<int,string> currentBuildSelected = (Tuple<int,string>)buildListItems.SelectedItem;
                SetCurrentBuild(currentBuildSelected.Item1, currentBuildSelected.Item2);
                BuildPage BuildPage = new BuildPage();
                NavigationService.GetNavigationService(this).Navigate(BuildPage);
            }
            catch
            {
                MessageBox.Show("No Build Selected.");
            }
        }

        //When a selected build is opened, this sets the appropriate system variables
        public void SetCurrentBuild(int id, string name)
        {
            try
            {
                SysVars.CurrentBuildId = id;
                SysVars.CurrentBuildName = name;
                SysVars.CurrentGraphType = GraphTypes.OptionsPlot;
            }
            catch (Exception e) { MessageBox.Show(e.ToString()); }
        }

        /* Deletes the selected build and its associated assets bar the components which 
         * can still be used in other builds for the project*/
        private void DeleteBuild_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Tuple<int, string> currentBuildSelected = (Tuple<int, string>)buildListItems.SelectedItem;
                MessageBoxResult dialogResult = MessageBox.Show("You are about to delete build version " + currentBuildSelected.Item2
                    + ". This will also delete all attached assets. Are you sure you want to delete this build?", "Build Deletion", MessageBoxButton.OKCancel);
                if (dialogResult == MessageBoxResult.OK)
                {
                    try
                    {
                        //Remove all builds by Id
                        var buildQuery = (from x in context.Builds
                                          where x.Id == currentBuildSelected.Item1
                                          select x);
                        foreach (var build in buildQuery)
                        {
                            context.Builds.Remove(build);
                        }

                       /* This query selects the relevant options by BuildId, 
                        * queries the CompOption table and selects the records by OptionId,
                        * queries the Component table and selects the records by their ComponentId.
                        * Each component for each CompOption is set to deselected, each compOption for 
                        * each Option is deleted, and each Option is deleted connected to the BuildId. */
                        var optionQuery = (from x in context.Options
                                           where x.BuildId == currentBuildSelected.Item1
                                           select x);
                        foreach (var option in optionQuery)
                        {
                            var compOptQuery = (from x in context.CompOptions
                                                where x.OptionId == option.Id
                                                select x);
                            foreach (var compOpt in compOptQuery)
                            {
                                var compQuery = (from x in context.Components
                                                 where x.Id == compOpt.ComponentId
                                                 select x);
                                foreach (var comp in compQuery)
                                {
                                    comp.Selected = false;
                                }
                                context.CompOptions.Remove(compOpt);
                            }
                            context.Options.Remove(option);
                        }
                        context.SaveChanges();
                        Refresh();
                        MessageBox.Show("Build deleted successfully.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Build deletion failed: " + ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Build deletion failed: " + ex.ToString());
            }
        }

        //Sets current page to the CreateBuild page
        private void NewBuild_Click(object sender, RoutedEventArgs e)
        {
            CreateBuild createBuild = new CreateBuild();
            NavigationService.GetNavigationService(this).Navigate(createBuild);
        }
    }

}
