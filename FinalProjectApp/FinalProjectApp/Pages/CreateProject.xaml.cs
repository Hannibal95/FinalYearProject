using FinalProjectApp.ProjectPages;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace FinalProjectApp.Pages
{
    public partial class CreateProject : Page
    {
        FinalProjectDbEntities context = new FinalProjectDbEntities();
        public CreateProject()
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
                    Project project = new Project()
                    {
                        Name = txtName.Text,
                        Description = txtDescription.Text
                    };
                    context.Projects.Add(project);
                    context.SaveChanges();
                    MessageBox.Show("Successfully created project " + project.Name);
                    ClearData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Project Creation Failed: " + ex.ToString());
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

        private string Validation()
        {
            if(txtDescription.Text.ToCharArray().Length > 100)
            {
                return "Description length must be 100 characters or less.";
            }
            if(txtName.Text.ToCharArray().Length > 30)
            {
                return "Name length must be 30 characters or less.";
            }
            if(txtName.Text.ToCharArray().Length < 1)
            {
                return "Name field cannot be left blank.";
            }
            return "SUCCESS";
        }

        private void ClearData()
        {
            txtName.Text = "";
            txtDescription.Text = "";
        }
    }
}
