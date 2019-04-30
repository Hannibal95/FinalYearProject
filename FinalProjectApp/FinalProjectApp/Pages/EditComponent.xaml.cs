using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace FinalProjectApp.Pages
{
    public partial class EditComponent : Page
    {
        FinalProjectDbEntities context = new FinalProjectDbEntities();

        public EditComponent()
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
                    var query = (from x in context.Components
                                 where x.Id == SysVars.CurrentComponentId
                                 select x);
                    foreach (Component comp in query)
                    {
                        comp.Name = txtName.Text;
                        comp.Description = txtDescription.Text;
                        comp.ValueToSystem = Convert.ToDouble(txtValueToSystem.Text);
                        comp.CostToBuild = Convert.ToDouble(txtCostToBuild.Text);
                        comp.DurationDays = Convert.ToInt32(txtDurationInDays.Text);
                        comp.Volatility = Convert.ToDouble(txtVolatility.Text);
                        comp.RFInterestRate = 1;
                    }
                    context.SaveChanges();
                    MessageBox.Show("Successfully modified component " + txtName.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Component Modification Failed: " + ex.ToString());
                    ResetData();
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

        private void ResetData()
        {
            var query = (from x in context.Components
                         where x.Id == SysVars.CurrentComponentId
                         select x);
            foreach (Component comp in query)
            {
                txtName.Text = comp.Name;
                txtDescription.Text = comp.Description;
                txtValueToSystem.Text = comp.ValueToSystem.ToString();
                txtCostToBuild.Text = comp.CostToBuild.ToString();
                txtDurationInDays.Text = comp.DurationDays.ToString();
                txtVolatility.Text = comp.Volatility.ToString();
            }
        }
    }
}
