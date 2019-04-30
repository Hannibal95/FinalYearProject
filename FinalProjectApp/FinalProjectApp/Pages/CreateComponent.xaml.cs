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

        private string Validation()
        {
            if (txtName.Text.ToCharArray().Length > 30)
            {
                return "Name length must be 30 characters or less.";
            }
            if (txtName.Text.ToCharArray().Length < 1)
            {
                return "Name field cannot be left blank.";
            }
            if (txtDescription.Text.ToCharArray().Length > 100)
            {
                return "Description length must be 100 characters or less.";
            }
            if (Double.TryParse(txtValueToSystem.Text, out Double n) == false)
            {
                return "Estimated value field is not numeric.";
            }
            if (Double.TryParse(txtValueToSystem.Text, out Double x) == true && Convert.ToDouble(txtValueToSystem.Text) < 0)
            {
                return "Estimated value cannot be a minus value.";
            }
            if (txtValueToSystem.Text.ToCharArray().Length < 1)
            {
                return "Estimated value field cannot be left blank.";
            }
            if (Double.TryParse(txtCostToBuild.Text, out Double y) == false)
            {
                return "Estimated cost field is not numeric.";
            }
            if (Double.TryParse(txtCostToBuild.Text, out Double z) == true && Convert.ToDouble(txtCostToBuild.Text) < 0)
            {
                return "Estimated cost cannot be a minus value.";
            }
            if (txtCostToBuild.Text.ToCharArray().Length < 1)
            {
                return "Estimated cost field cannot be left blank.";
            }
            if (int.TryParse(txtDurationInDays.Text, out int a) == false)
            {
                return "Duration field is not a valid integer.";
            }
            if (int.TryParse(txtDurationInDays.Text, out int b) == true && Convert.ToInt32(txtDurationInDays.Text) < 0)
            {
                return "Duration cannot be a minus value";
            }
            if (txtDurationInDays.Text.ToCharArray().Length < 1)
            {
                return "Duration field cannot be left blank.";
            }
            if (Double.TryParse(txtVolatility.Text, out Double c) == false)
            {
                return "Volatility field is not numeric.";
            }
            if (Double.TryParse(txtVolatility.Text, out Double d) == true && Convert.ToDouble(txtCostToBuild.Text) < 0)
            {
                return "Volatility cannot be a minus value.";
            }
            if (txtVolatility.Text.ToCharArray().Length < 1)
            {
                return "Volatility field cannot be left blank.";
            }
            return "SUCCESS";
        }

        private void ClearData()
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
