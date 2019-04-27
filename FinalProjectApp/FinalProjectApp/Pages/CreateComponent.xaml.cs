using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace FinalProjectApp.Pages
{
    public partial class CreateComponent : Page
    {
        FinalProjectDbEntities context = new FinalProjectDbEntities();

        public CreateComponent()
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
                    Component component = new Component()
                    {
                        ProjectId = SysVars.CurrentProjectId,
                        Name = txtName.Text,
                        Description = txtDescription.Text,
                        ValueToSystem = Convert.ToDouble(txtValueToSystem.Text),
                        CostToBuild = Convert.ToDouble(txtCostToBuild.Text),
                        DurationDays = Convert.ToInt32(txtDurationInDays.Text),
                        Volatility = Convert.ToDouble(txtVolatility.Text),
                        RFInterestRate = 1,
                        Selected = false
                    };
                    context.Components.Add(component);
                    context.SaveChanges();
                    MessageBox.Show("Successfully created component " + component.Name);
                    ClearData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Component Creation Failed: " + ex.ToString());
                }
            }
            else { MessageBox.Show(val); }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ClearData();
            BuildPage buildPage = new BuildPage();
            NavigationService.GetNavigationService(this).Navigate(buildPage);
        }

        public string Validation()
        {
            return "SUCCESS";
        }

        public void ClearData()
        {
            txtName.Text = "";
            txtDescription.Text = "";
            txtValueToSystem.Text = "";
            txtCostToBuild.Text = "";
            txtDurationInDays.Text = "";
            txtVolatility.Text = "";
        }
    }
}
