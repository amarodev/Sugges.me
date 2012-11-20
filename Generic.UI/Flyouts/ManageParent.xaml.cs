using Generic.UI.Logic.Enumerations;
using Generic.UI.Logic.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Generic.UI.Flyouts
{
    public sealed partial class ManageTrip : UserControl
    {

        private static ResourceLoader loader = new ResourceLoader();
        private ParentViewModel parentViewModel;

        public ManageTrip()
        {
            this.InitializeComponent();
            this.tbTitle.Text = "New Trip";

            InitializeFlyout();        
        }

        public ManageTrip(ParentViewModel parentViewModel)
        {
            this.InitializeComponent();
            this.tbTitle.Text = "Update Trip";

            InitializeFlyout();
            this.parentViewModel = parentViewModel;

            this.txtTitle.Text = parentViewModel.Title;
            this.txtDescription.Text = parentViewModel.Description;
            this.txtCost.Text = parentViewModel.Cost.ToString();
            cboYears.SelectedValue = parentViewModel.StartDate.Year;

            cboMonths.SelectedValue = this.cboMonths.Items[parentViewModel.StartDate.Month - 1];
            cboDays.SelectedValue = parentViewModel.StartDate.Day;
        }

        private void InitializeFlyout()
        {
            LoadDateItems();
            cboYears.SelectedIndex = 0;
            cboMonths.SelectedIndex = DateTime.Now.Month - 1;
        }

        private void LoadDateItems()
        {
            foreach (int identifier in Enum.GetValues(typeof(Month)))
            {
                this.cboMonths.Items.Add(new PairViewModel
                {
                    Identifier = identifier,
                    Description = Enum.GetName(typeof(Month), identifier)
                });
            }

            cboMonths.DisplayMemberPath = "Description";

            for (int i = DateTime.Now.Year; i <= DateTime.Now.Year + 50; i++)
            {
                cboYears.Items.Add(i);
            }
        }

        async private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.txtStatusMessage.Visibility = Windows.UI.Xaml.Visibility.Visible;
                this.txtStatusMessage.Text = "Please wait...";

                if (AreFieldsValid())
                {
                    if (parentViewModel == null)
                    {
                        ParentViewModel newTrip = new ParentViewModel()
                        {
                            Identifier = -1,
                            Title = this.txtTitle.Text,
                            LocalPathImage = loader.GetString("DefaultParentImage"),
                            Group = MainViewModel.GetGroup(0),
                            Description = this.txtDescription.Text,
                            Cost = Convert.ToInt64(this.txtCost.Text),
                            User = MainViewModel.GetTraveler(),
                            StartDate = new DateTime(
                                Convert.ToInt32(cboYears.SelectedItem),
                                Convert.ToInt32(((PairViewModel)cboMonths.SelectedItem).Identifier + 1),
                                Convert.ToInt32(cboDays.SelectedItem))
                        };

                        newTrip.Identifier = await MainViewModel.SaveTripAsync(newTrip);

                        this.txtStatusMessage.Text = "Trip has been created";
                    }
                    else
                    {
                            parentViewModel.Title = this.txtTitle.Text;
                            parentViewModel.Description = this.txtDescription.Text;
                            parentViewModel.Cost = Convert.ToInt64(this.txtCost.Text);
                            parentViewModel.StartDate = new DateTime(
                                Convert.ToInt32(cboYears.SelectedItem),
                                Convert.ToInt32(((PairViewModel)cboMonths.SelectedItem).Identifier + 1),
                                Convert.ToInt32(cboDays.SelectedItem));

                            await MainViewModel.SaveTripAsync(parentViewModel);
                            this.txtStatusMessage.Text = "Trip has been updated";
                    }
                    
                    this.backButton_Click(sender, e);
                }
            }
            catch (Exception)
            {
                this.txtStatusMessage.Text = "Something is wrong, please review the fields and try again";
            }
        }

        private bool AreFieldsValid()
        {
            if (String.IsNullOrEmpty(this.txtTitle.Text) || this.txtTitle.Text.Length > 50)
            {
                txtStatusMessage.Text = "Please give a valid name for the trip (Máx 50)";
                return false;
            }
            if (String.IsNullOrEmpty(this.txtDescription.Text) || this.txtDescription.Text.Length > 100)
            {
                txtStatusMessage.Text = "Please give a valid description for the trip (Máx 100)";
                return false;
            }
            long cost = 0;
            if (String.IsNullOrEmpty(this.txtCost.Text))
            {
                txtStatusMessage.Text = "Please give a valid cost for the trip.)";
                return false;
            }

            if (!long.TryParse(this.txtCost.Text, out cost))
            {
                txtStatusMessage.Text = "Please give a valid cost for the trip, use only numbers.";
                return false;
            }

            return true;
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Parent.GetType() == typeof(Popup))
            {
                ((Popup)this.Parent).IsOpen = false;
            }
        }

        private void cboMonths_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshDays();
        }

        private void cboYears_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshDays();
        }

        private void RefreshDays()
        {
            if (cboMonths.SelectedValue != null && cboYears.SelectedValue != null)
            {
                int maxDays = System.DateTime.DaysInMonth(Convert.ToInt32(cboYears.SelectedValue), Convert.ToInt32(((PairViewModel)cboMonths.SelectedValue).Identifier + 1));
                int selectedDay = 0;

                if (cboDays.SelectedValue == null)
                    selectedDay = DateTime.Now.Day;
                else
                    selectedDay = Convert.ToInt32(cboDays.SelectedValue) > maxDays ? maxDays : Convert.ToInt32(cboDays.SelectedValue);

                this.cboDays.Items.Clear();
                for (int i = 1; i <= maxDays; i++)
                    cboDays.Items.Add(i);

                this.cboDays.SelectedValue = selectedDay;
            }
        }
    }
}
