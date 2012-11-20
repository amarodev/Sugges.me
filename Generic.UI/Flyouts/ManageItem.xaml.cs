using Generic.UI.Logic.Enumerations;
using Generic.UI.Logic.ViewModels;
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

namespace Generic.UI.Flyouts
{
    public sealed partial class ManageItem : UserControl
    {
        private ItemViewModel itemViewModel;

        public ManageItem()
        {
            this.InitializeComponent();
            this.tbTitle.Text = "New Item";

            InitializeFlyout();
        }

        public ManageItem(ItemViewModel itemViewModel)
        {
            this.InitializeComponent();
            this.tbTitle.Text = "Update Item";

            InitializeFlyout();

            this.itemViewModel = itemViewModel;
            this.txtTitle.Text = itemViewModel.Title;
            this.txtDescription.Text = itemViewModel.Description;
            this.txtCost.Text = itemViewModel.Cost.ToString();
            this.cboCategories.SelectedItem = cboCategories.Items[((int)itemViewModel.Category) - 2];
        }

        private void InitializeFlyout()
        {
            LoadCategories();
        }

        async private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.txtStatusMessage.Visibility = Windows.UI.Xaml.Visibility.Visible;
                this.txtStatusMessage.Text = "Please wait...";

                if (AreFieldsValid())
                {
                    if (this.itemViewModel == null)
                    {
                        ItemViewModel item = new ItemViewModel()
                        {
                            Identifier = -1,
                            Title = this.txtTitle.Text,
                            Description = this.txtDescription.Text,
                            Cost = Convert.ToInt64(this.txtCost.Text),
                            Trip = MainViewModel.GetSelectedTrip(),
                            User = MainViewModel.GetTraveler(),
                            Category = (short)((PairViewModel)cboCategories.SelectedItem).Identifier
                        };

                        item.Identifier = await MainViewModel.SaveItem(item);

                        this.txtStatusMessage.Text = "Trip has been created";
                    }
                    else
                    {
                        itemViewModel.Title = this.txtTitle.Text;
                        itemViewModel.Description = this.txtDescription.Text;
                        itemViewModel.Trip = MainViewModel.GetSelectedTrip();

                        itemViewModel.Trip.ItemsCost -= itemViewModel.Cost;
                        itemViewModel.Cost = Convert.ToInt64(this.txtCost.Text);
                        
                        itemViewModel.User = MainViewModel.GetTraveler();
                        itemViewModel.Category = (short)((PairViewModel)cboCategories.SelectedItem).Identifier;                        

                        await MainViewModel.SaveItem(itemViewModel);

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
                txtStatusMessage.Text = "Please give a valid name for the item (Máx 50)";
                return false;
            }
            if (String.IsNullOrEmpty(this.txtDescription.Text) || this.txtDescription.Text.Length > 100)
            {
                txtStatusMessage.Text = "Please give a valid description for the item (Máx 100)";
                return false;
            }
            long cost = 0;
            if (String.IsNullOrEmpty(this.txtCost.Text))
            {
                txtStatusMessage.Text = "Please give a valid cost for the item, use only numbers.";
                return false;
            }

            if (!long.TryParse(this.txtCost.Text, out cost))
            {
                txtStatusMessage.Text = "Please give a valid cost for the trip, use only numbers)";
                return false;
            }

            return true;
        }

        private void LoadCategories()
        {
            foreach (int identifier in Enum.GetValues(typeof(Category)))
            {
                if (identifier != 1)
                {
                    cboCategories.Items.Add(new PairViewModel
                    {
                        Identifier = identifier,
                        Description = Enum.GetName(typeof(Category), identifier)
                    });
                }

                cboCategories.DisplayMemberPath = "Description";
            }

            cboCategories.SelectedIndex = 0;
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Parent.GetType() == typeof(Popup))
            {
                ((Popup)this.Parent).IsOpen = false;
            }
        }
    }
}
