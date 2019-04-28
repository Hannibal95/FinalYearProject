using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace FinalProjectApp.Pages
{
    public partial class EditBuild : Page
    {
        FinalProjectDbEntities context = new FinalProjectDbEntities();
        public EditBuild()
        {
            InitializeComponent();
            ResetData();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            string val = Validation();
            if (val == "SUCCESS")
            {
                try
                {
                    var date = Convert.ToDateTime(txtReleaseDate.Text);
                    try
                    {
                        var query = (from x in context.Builds
                                     where x.Id == SysVars.CurrentBuildId
                                     select x);
                        foreach (Build build in query)
                        {
                            build.Version = txtVersion.Text;
                            build.Budget = Convert.ToDouble(txtBudget.Text);
                            build.ReleaseDate = date;
                        }
                        context.SaveChanges();
                        SysVars.CurrentBuildName = txtVersion.Text;
                        SysVars.CurrentBuildDeadline = date;
                        MessageBox.Show("Successfully modified project " + txtVersion.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Project Modification Failed: " + ex.ToString());
                        ResetData();
                    }
                }
                catch
                {
                    try
                    {
                        var query = (from x in context.Builds
                                     where x.Id == SysVars.CurrentBuildId
                                     select x);
                        foreach (Build build in query)
                        {
                            build.Version = txtVersion.Text;
                            build.Budget = Convert.ToDouble(txtBudget.Text);
                            build.ReleaseDate = null;
                            build.MaxVolatility = 0;
                        }
                        context.SaveChanges();
                        MessageBox.Show("Successfully modified project " + txtVersion.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Project Modification Failed: " + ex.ToString());
                        ResetData();
                    }
                }
            }
            else { MessageBox.Show(val); }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ResetData();
            BuildPage buildPage = new BuildPage();
            NavigationService.GetNavigationService(this).Navigate(buildPage);
        }

        public void ResetData()
        {
            var query = (from x in context.Builds
                         where x.Id == SysVars.CurrentBuildId
                         select x);
            foreach (Build build in query)
            {
                txtVersion.Text = build.Version.ToString();
                txtBudget.Text = build.Budget.ToString();
                txtReleaseDate.Text = ((DateTime)build.ReleaseDate).ToShortDateString();
            }
        }

        public string Validation()
        {
            return "SUCCESS";
        }
    }
}
