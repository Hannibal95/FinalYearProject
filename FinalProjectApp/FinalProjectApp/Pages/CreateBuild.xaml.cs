using FinalProjectApp.ProjectPages;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace FinalProjectApp.Pages
{
    public partial class CreateBuild : Page
    {
        FinalProjectDbEntities context = new FinalProjectDbEntities();

        public CreateBuild()
        {
            InitializeComponent();
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            string val = Validation();
            if (val == "SUCCESS")
            {
                try
                {
                    var date = Convert.ToDateTime(txtReleaseDate.Text);
                    try
                    {
                        Build build = new Build()
                        {
                            ProjectId = SysVars.CurrentProjectId,
                            Version = txtVersion.Text,
                            Budget = Convert.ToDouble(txtBudget.Text),
                            ReleaseDate = date,
                            MaxVolatility = 0,
                            Completed = false
                        };
                        context.Builds.Add(build);
                        context.SaveChanges();
                        MessageBox.Show("Successfully created build " + build.Version);
                        ClearData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Build Creation Failed: " + ex.ToString());
                    }
                }
                catch
                {
                    try
                    {
                        Build build = new Build()
                        {
                            ProjectId = SysVars.CurrentProjectId,
                            Version = txtVersion.Text,
                            Budget = Convert.ToDouble(txtBudget.Text),
                            ReleaseDate = null,
                            MaxVolatility = 0,
                            Completed = false
                        };
                        context.Builds.Add(build);
                        context.SaveChanges();
                        MessageBox.Show("Successfully created build " + build.Version);
                        ClearData();
                    }
                    catch (Exception ex1)
                    {
                        MessageBox.Show("Build Creation Failed: " + ex1.ToString());
                    }
                }
            }
            else { MessageBox.Show(val); }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ClearData();
            StartPage startPage = new StartPage();
            NavigationService.GetNavigationService(this).Navigate(startPage);
        }

        public string Validation()
        {
            return "SUCCESS";
        }

        public void ClearData()
        {
            txtVersion.Text = "";
            txtBudget.Text = "";
            txtReleaseDate.Text = "";
        }
    }
}
