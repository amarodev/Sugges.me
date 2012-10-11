using Sugges.UI.Logic.Enumerations;
using Sugges.UI.Logic.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace Sugges.UI.Flyouts
{
    public sealed partial class ManageTrip : UserControl
    {
        private TripViewModel tripViewModel;

        public ManageTrip()
        {
            this.InitializeComponent();
            this.tbTitle.Text = "New Trip";

            InitializeFlyout();        
        }

        public ManageTrip(TripViewModel tripViewModel)
        {
            this.InitializeComponent();
            this.tbTitle.Text = "Update Trip";

            InitializeFlyout();
            this.tripViewModel = tripViewModel;

            this.txtTitle.Text = tripViewModel.Title;
            this.txtDescription.Text = tripViewModel.Description;
            this.txtCost.Text = tripViewModel.Cost.ToString();
            cboYears.SelectedValue = tripViewModel.StartDate.Year;

            cboMonths.SelectedValue = this.cboMonths.Items[tripViewModel.StartDate.Month - 1];
            cboDays.SelectedValue = tripViewModel.StartDate.Day;
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
                    if (tripViewModel == null)
                    {
                        TripViewModel newTrip = new TripViewModel()
                        {
                            Identifier = -1,
                            Title = this.txtTitle.Text,
                            LocalPathImage = "/Assets/Trip.png",
                            Group = MainViewModel.GetGroup(0),
                            Description = this.txtDescription.Text,
                            Cost = Convert.ToInt64(this.txtCost.Text),
                            Traveler = MainViewModel.GetTraveler(),
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
                            tripViewModel.Title = this.txtTitle.Text;
                            tripViewModel.Description = this.txtDescription.Text;
                            tripViewModel.Cost = Convert.ToInt64(this.txtCost.Text);
                            tripViewModel.StartDate = new DateTime(
                                Convert.ToInt32(cboYears.SelectedItem),
                                Convert.ToInt32(((PairViewModel)cboMonths.SelectedItem).Identifier + 1),
                                Convert.ToInt32(cboDays.SelectedItem));

                            await MainViewModel.SaveTripAsync(tripViewModel);
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
