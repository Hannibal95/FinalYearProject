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

        public string Validation()
        {
            return "SUCCESS";
        }

        public void ResetData()
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
