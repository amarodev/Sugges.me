using Sugges.UI.Common;
using Sugges.UI.Flyouts;
using Sugges.UI.Logic.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Group Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234229

namespace Sugges.UI
{
    /// <summary>
    /// A page that displays an overview of a single group, including a preview of the items
    /// within the group.
    /// </summary>
    public sealed partial class ItemDetailPage : Sugges.UI.Common.LayoutAwarePage
    {
        public ItemDetailPage()
        {
            this.InitializeComponent();

            DataTransferManager.GetForCurrentView().DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);

            this.btnDelete.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        void DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            TripViewModel trip = MainViewModel.GetSelectedTrip();

            DataPackage requestData = e.Request.Data;
            e.Request.Data.Properties.Title = trip.Title;
            e.Request.Data.Properties.Description = trip.Description;

            // It's recommended to use both SetBitmap and SetStorageItems for sharing a single image
            // since the target app may only support one or the other.

            Uri imageUri = ((Windows.UI.Xaml.Media.Imaging.BitmapImage)(imgTripPhoto.Source)).UriSource;

            //List<IStorageItem> imageItems = new List<IStorageItem>();
            //imageItems.Add(await StorageFile.GetFileFromApplicationUriAsync(imageUri));
            //requestData.SetStorageItems(imageItems);

            RandomAccessStreamReference imageStreamRef = RandomAccessStreamReference.CreateFromUri(imageUri);
            requestData.Properties.Thumbnail = imageStreamRef;
            requestData.SetBitmap(imageStreamRef);

        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        async protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // Allow saved page state to override the initial item to display
            if (pageState != null && pageState.ContainsKey("SelectedItem"))
            {
                navigationParameter = pageState["SelectedItem"];
            }

            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            TripViewModel item = await MainViewModel.GetFullTrip((Int32)navigationParameter);
            RemoveFakeTrip(item);

            this.DefaultViewModel["ItemGroups"] = item.ItemGroups;
            this.DefaultViewModel["Trips"] = item.Group.Items;
                        
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            DataTransferManager.GetForCurrentView().DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            /*var selectedTrip = (TripViewModel)this.flipViewGrid.SelectedItem;
            pageState["SelectedItem"] = selectedTrip.Identifier;*/
        }

        private void btnAdd_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.bottomAppBar.IsOpen = false;
            var trips = new Flyout();
            trips.ShowFlyout(new ManageItem());
        }

        async private void btnDelete_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Create the message dialog and set its content and title
            var messageDialog = new MessageDialog("The seleted items will be deleting permanently. Are you sure to do this action?", "Deleting items");

            // Add commands and set their callbacks
            messageDialog.Commands.Add(new UICommand("Yes", (command) =>
            {
                MainViewModel.DeleteSelectedItemsAsync();
            }));

            messageDialog.Commands.Add(new UICommand("No, please cancel", (command) =>
            {
                //OK!
            }));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 1;

            // Show the message dialog
            await messageDialog.ShowAsync();
        }

        private static void RemoveFakeTrip(TripViewModel item)
        {
            TripViewModel fake = item.Group.Items[item.Group.Items.Count - 1];

            if (fake.Identifier == -1)
                item.Group.Items.Remove(fake);
        }

        async private void btnSelectPhoto_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            StorageFile selectedFile = await openPicker.PickSingleFileAsync();

            if (selectedFile != null)
            {

                var localFolder = ApplicationData.Current.LocalFolder;
                StorageFolder imagesFolder = await localFolder.CreateFolderAsync("images", CreationCollisionOption.OpenIfExists);

                TripViewModel trip = MainViewModel.GetSelectedTrip();
                var oldImage = trip.LocalPathImage;
                var desiredName = string.Format("{0}.jpg", trip.Identifier.ToString());

                if (!oldImage.Equals("/Assets/Trip.png"))
                {
                    try
                    {
                        StorageFile oldFile = await imagesFolder.GetFileAsync(desiredName);
                        await oldFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                        oldFile = null;
                    }
                    catch (Exception)
                    {
                        ///TODO: GridView please free the imageeeeeeeee! :'(
                        MainViewModel.RegisterTrashImage(desiredName);
                    }
                }
                
                //StorageFile newFile = await imagesFolder.CreateFileAsync(desiredName);
                StorageFile newFile = await imagesFolder.CreateFileAsync(desiredName, CreationCollisionOption.GenerateUniqueName);
                await selectedFile.CopyAndReplaceAsync(newFile);

                //MainViewModel.UpdatePhotoToSelectedTrip(string.Format("ms-appdata:///local/images/{0}", desiredName));
                MainViewModel.UpdatePhotoToSelectedTrip(string.Format("ms-appdata:///local/images/{0}", newFile.Name));
            }
        }

        private void btnEdit_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
        	this.bottomAppBar.IsOpen = false;
            var trips = new Flyout();
            trips.ShowFlyout(new ManageTrip(MainViewModel.GetSelectedTrip()));
        }

        private void itemGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (((ItemViewModel)e.ClickedItem).Identifier != -1)
            {
                this.bottomAppBar.IsOpen = false;
                var trips = new Flyout();
                trips.ShowFlyout(new ManageItem((ItemViewModel)e.ClickedItem));
            }
        }

        private void itemListView_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            foreach (object item in e.AddedItems)
            {
                if (((ItemViewModel)item).Identifier == -1)
                {
                    this.itemListView.SelectedItems.Remove(item);
                    e.AddedItems.Remove(item);
                }
            }

            foreach (object item in e.AddedItems)
            {
                itemGridView.SelectedItems.Add(item);
                ((ItemViewModel)item).IsSelected = true;
            }

            foreach (object item in e.RemovedItems)
            {
                itemGridView.SelectedItems.Remove(item);
                ((ItemViewModel)item).IsSelected = false;
            }

            if (this.itemListView.SelectedItems.Count > 0)
                btnDelete.Visibility = Windows.UI.Xaml.Visibility.Visible;
            else
                btnDelete.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void itemGridView_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            foreach (object item in e.AddedItems)
            {
                if (((ItemViewModel)item).Identifier == -1)
                {
                    this.itemGridView.SelectedItems.Remove(item);
                    e.AddedItems.Remove(item);
                }
            }

            foreach (object item in e.AddedItems)
            {
                itemListView.SelectedItems.Add(item);
                ((ItemViewModel)item).IsSelected = true;
            }

            foreach (object item in e.RemovedItems)
            {
                itemListView.SelectedItems.Remove(item);
                ((ItemViewModel)item).IsSelected = false;
            }

            if (this.itemGridView.SelectedItems.Count > 0)
                btnDelete.Visibility = Windows.UI.Xaml.Visibility.Visible;
            else
                btnDelete.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void itemListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (((ItemViewModel)e.ClickedItem).Identifier != -1)
            {
                this.bottomAppBar.IsOpen = false;
                var trips = new Flyout();
                trips.ShowFlyout(new ManageItem((ItemViewModel)e.ClickedItem));
            }
        }

    }
}
