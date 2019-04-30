using FinalProjectApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinalProjectApp.Pages
{
    /// <summary>
    /// Interaction logic for EditOption.xaml
    /// </summary>
    public partial class EditOption : Page
    {
        FinalProjectDbEntities context = new FinalProjectDbEntities();
        private BlackScholes blackSchole = new BlackScholes();
        public EditOption()
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
                    var query = (from x in context.Options
                                 where x.Id == SysVars.CurrentOptionId
                                 select x);
                    foreach (Option option in query)
                    {
                        double callVal = blackSchole.CalculateOption(option.ValueToSystem, option.CostToBuild, 
                            (Convert.ToInt32(txtDurationInDays.Text)/364.0), 1, option.Volatility);

                        bool check = false;
                        if (cbComplete.IsChecked == true) { check = false; }

                        option.Name = txtName.Text;
                        option.Description = txtDescription.Text;
                        option.DurationDays = Convert.ToInt32(txtDurationInDays.Text);
                        option.Complete = check;
                        option.CallValue = callVal;
                    }

                    context.SaveChanges();
                    MessageBox.Show("Successfully modified option " + txtName.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Option Modification Failed: " + ex.ToString());
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
            return "SUCCESS";
        }

        public void ResetData()
        {
            var query = (from x in context.Options
                         where x.Id == SysVars.CurrentOptionId
                         select x);
            foreach (Option option in query)
            {
                txtName.Text = option.Name;
                txtDescription.Text = option.Description;
                txtDurationInDays.Text = option.DurationDays.ToString();
                cbComplete.IsChecked = option.Complete;
            }

            if (cbComplete.IsChecked == true) { cbComplete.Content = "Set To: Completed";  }
            else { cbComplete.Content = "Set To: Not Completed"; }
        }

        private void HandleCheck(object sender, RoutedEventArgs e)
        {
            cbComplete.Content = "Set To: Completed";
        }

        private void HandleUnchecked(object sender, RoutedEventArgs e)
        {
            cbComplete.Content = "Set To: Not Completed";
        }
    }
}
