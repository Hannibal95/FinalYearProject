using FinalProjectApp.ProjectPages;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace FinalProjectApp.Pages
{
    public partial class EditProject : Page
    {
        FinalProjectDbEntities context = new FinalProjectDbEntities();
        public EditProject()
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
                    var query = (from x in context.Projects
                                 where x.Id == SysVars.CurrentProjectId
                                 select x);
                    foreach (Project proj in query)
                    {
                        proj.Name = txtName.Text;
                        proj.Description = txtDescription.Text;
                        SysVars.CurrentProjectName = txtName.Text;
                        SysVars.CurrentProjectDescription = txtDescription.Text;
                    }
                    context.SaveChanges();
                    MessageBox.Show("Successfully modified project " + txtName.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Project Modification Failed: " + ex.ToString());
                    ResetData();
                }
            }
            else { MessageBox.Show(val); }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ResetData();
            StartPage startPage = new StartPage();
            NavigationService.GetNavigationService(this).Navigate(startPage);
        }

        public string Validation()
        {
            if (txtDescription.Text.ToCharArray().Length > 50)
            {
                return "Description length must be 50 characters or less.";
            }
            if (txtName.Text.ToCharArray().Length > 30)
            {
                return "Name length must be 30 characters or less.";
            }
            if (txtName.Text.ToCharArray().Length < 1)
            {
                return "Name field cannot be left blank.";
            }
            return "SUCCESS";
        }

        public void ResetData()
        {
            var query = (from x in context.Projects
                         where x.Id == SysVars.CurrentProjectId
                         select x);
            foreach (Project proj in query)
            {
                txtName.Text = proj.Name;
                txtDescription.Text = proj.Description;
            }
        }
    }
}
