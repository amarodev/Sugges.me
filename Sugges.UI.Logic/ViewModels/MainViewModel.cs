using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Sugges.UI.Logic.Common;
using Sugges.UI.Logic.Enumerations;
using Sugges.UI.Logic.Models;
using Windows.Foundation;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using System.Collections.Generic;
using System.Linq;
using Sugges.UI.Logic.DataModel;
using Windows.Storage;


namespace Sugges.UI.Logic.ViewModels
{
    public class MainViewModel : BindableBase
	{

        private static IModel model;
        private static MainViewModel _mainViewModel = new MainViewModel();

        public MainViewModel()
        {
            model = new DatabaseModel();
            CreateDesignData();
            
        }

        public MainViewModel(TravelerViewModel traveler)
        {
            model = new DatabaseModel();

            AllGroups = new ObservableCollection<GroupViewModel>();
            _traveler = traveler;

            this.AllGroups.Add(new GroupViewModel()
            {
                Identifier = 0,
                Title = "Your trips",
                Description = "Your planned trips",
                LocalPathImage = "/Assets/Trip.png",
            });

            this.AllGroups.Add(new GroupViewModel()
            {
                Identifier = 1,
                Title = "Suggestions",
                Description = "Suggestions from our platform",
                LocalPathImage = "/Assets/Suggestion.png",
                AreSuggestions = true
            });
        }

        #region Properties

        private TripViewModel _selectedTrip;

        public TripViewModel SelectedTrip
        {
            get { return this._selectedTrip; }
            set { this.SetProperty(ref this._selectedTrip, value); }
        }

        private string _statusMessage;

        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                _statusMessage = value;
                OnPropertyChanged("StatusMessage");
            }
        }

        private Status _status;

        public Status Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        private void CreateDesignData()
        {
            this.AllGroups.Add(new GroupViewModel()
            {
                Title = "d:Your trips",
                Description = "Your planned trips",
                LocalPathImage = "/Assets/Trip.png",
            });

            this.AllGroups.Add(new GroupViewModel()
            {
                Title = "d:Suggestions",
                Description = "Suggestion from our platform",
                LocalPathImage = "/Assets/Suggestion.png",
            });
            
            //Your Trips
            if (this.AllGroups[0].Items.Count < 4)
            {
                for (int i = this.AllGroups[0].Items.Count; i < 4; i++)
                {
                    this.AllGroups[0].Items.Add(
                    new TripViewModel()
                    {
                        Title = "d:Probando nombre muy muy muy muy pero muy largo",
                        Description = "Probando nombre muy muy muy muy pero muy largo y este tambien muy largo demasiado largo",
                        LocalPathImage = "/Assets/Trip.png",
                        StartDate = new DateTime(2012,05,05),
                        Cost = 100,
                        ItemGroups = new ObservableCollection<CategoryViewModel>() 
                        {
                            new CategoryViewModel()
                            {
                                Title = "d:Cosas",
                                Description = "Probando nombre muy muy muy muy pero muy largo y este tambien muy largo demasiado largo",
                                LocalPathImage = "/Assets/Trip.png",
                                Items = new ObservableCollection<ItemViewModel>()
                                {
                                    new ItemViewModel()
                                    {
                                        Title = "d:Bufanda",
                                        Description = "Probando nombre muy muy muy muy pero muy largo y este tambien muy largo demasiado largo",
                                        LocalPathImage = "/Assets/Trip.png",
                                        Cost = 24,
                                        Trip = new TripViewModel()
                                        {
                                            Title = "Argentina",
                                            Description = "Testing",
                                            Cost = 100,
                                            ItemsCost = 150,
                                            LocalPathImage = "/Assets/Trip.png",
                                            StartDate = new DateTime(2012,9,14),
                                        }
                                    },
                                    new ItemViewModel()
                                    {
                                        Title = "d:Guantes",
                                        LocalPathImage = "/Assets/Trip.png",
                                        Cost = 100,
                                        Trip = new TripViewModel()
                                        {
                                            Title = "Argentina",
                                            Description = "Testing",
                                            Cost = 100,
                                            ItemsCost = 150,
                                            LocalPathImage = "/Assets/Trip.png",
                                            StartDate = new DateTime(2012,9,14),
                                        }
                                    },
                                    new ItemViewModel()
                                    {
                                        Title = "d:Plata",
                                        Description = "Probando nombre muy muy muy muy pero muy largo y este tambien muy largo demasiado largo",
                                        LocalPathImage = "/Assets/Trip.png",
                                        Trip = new TripViewModel()
                                        {
                                            Title = "Argentina",
                                            Description = "Testing",
                                            Cost = 100,
                                            ItemsCost = 150,
                                            LocalPathImage = "/Assets/Trip.png",
                                            StartDate = new DateTime(2012,9,14),
                                        }
                                    }
                                }
                            },
                            new CategoryViewModel()
                            {
                                Title = "d:Amigos",
                                Description = "Probando nombre muy muy muy muy pero muy largo y este tambien muy largo demasiado largo",
                                LocalPathImage = "/Assets/Trip.png",
                                Items = new ObservableCollection<ItemViewModel>()
                                {
                                    new ItemViewModel()
                                    {
                                        Title = "d:Cristian",
                                        Description = "Probando nombre muy muy muy muy pero muy largo y este tambien muy largo demasiado largo",
                                        LocalPathImage = "/Assets/Trip.png",
                                    }
                                }
                            },
                            new CategoryViewModel()
                            {
                                Title = "d:Amigos",
                                Description = "Probando nombre muy muy muy muy pero muy largo y este tambien muy largo demasiado largo",
                                LocalPathImage = "/Assets/Trip.png",
                                Items = new ObservableCollection<ItemViewModel>()
                                {
                                    new ItemViewModel()
                                    {
                                        Title = "d:Cristian",
                                        Description = "Probando nombre muy muy muy muy pero muy largo y este tambien muy largo demasiado largo",
                                        LocalPathImage = "/Assets/Trip.png",
                                    }
                                }
                            }
                        },
                    });
                }
            }

            //Suggestions
            if (this.AllGroups[1].Items.Count < 4)
            {
                for (int i = this.AllGroups[1].Items.Count; i < 4; i++)
                {
                    this.AllGroups[1].Items.Add(
                    new TripViewModel()
                    {
                        Title = "Colombia",
                        Description = "El unico riesgo es que quieras quedarte",
                        LocalPathImage = "/Assets/Suggestion.png",
                        Cost = 100
                    });
                }
            }
        }

        private TravelerViewModel _traveler;

        public TravelerViewModel Traveler
        {
            get { return _traveler; }
            set
            {
                _traveler = value;
                OnPropertyChanged("Traveler");
            }
        }

        private ObservableCollection<GroupViewModel> _allGroups = new ObservableCollection<GroupViewModel>();
        public ObservableCollection<GroupViewModel> AllGroups
        {
            get { return this._allGroups; }
            set { _allGroups = value; }
        }

        public GroupViewModel SelectedGroup { get; set; }

        public bool IsRegisteredForShare { get; set; }

        #endregion

        async public static Task LoadSuggestionsAsync()
        {
            try
            {
                _mainViewModel.StatusMessage = "Please wait...";
                _mainViewModel.Status = Enumerations.Status.Working;

                List<TripViewModel> suggestions = await model.GetSuggestionsAsync();

                foreach (TripViewModel suggestion in suggestions)
                {
                    suggestion.Group = _mainViewModel.AllGroups[1];  //Very important!
                    suggestion.IsSuggestion = true;
                    _mainViewModel.AllGroups[1].Items.Add(suggestion);
                }

                CompleteFakeSuggestions(_mainViewModel.AllGroups[1]);

                _mainViewModel.Status = Enumerations.Status.Stopped;
                _mainViewModel.StatusMessage = string.Empty;
            }
            catch (Exception)
            {
                _mainViewModel.Status = Enumerations.Status.Stopped;
                _mainViewModel.StatusMessage = "An error has ocurred, please check your internet connection";
            }
        }

        async public static Task LoadTripsAsync()
        {
            try
            {
                _mainViewModel.StatusMessage = "Please wait...";
                _mainViewModel.Status = Enumerations.Status.Working;

                List<TripViewModel> trips = await model.GetTripsAsync((Guid)_mainViewModel.Traveler.Identifier);
                
                foreach (TripViewModel trip in trips)
                {
                    trip.Group = _mainViewModel.AllGroups[0]; //Very important!
                    _mainViewModel.AllGroups[0].Items.Add(trip);
                }

                _mainViewModel.AllGroups[0].CreateFake(FakeType.Trip);

                _mainViewModel.Status = Enumerations.Status.Stopped;
                _mainViewModel.StatusMessage = string.Empty;
            }
            catch (Exception)
            {
                _mainViewModel.Status = Enumerations.Status.Stopped;
                _mainViewModel.StatusMessage = "An error has ocurred, please check your internet connection";
            }
        }

        public static void CompleteFakeSuggestions(GroupViewModel group)
        {
            if (group.Items.Count < 4)
            {
                for (int i = group.Items.Count; i < 4; i++)
                {
                   group.CreateFake(FakeType.Suggestion);
                }
            }
        }

        #region Static Default Methods

        public static TripViewModel GetItem(object identifier)
        {
            return new TripViewModel()
            {
                Group = new GroupViewModel() 
                { 
                    Items = new ObservableCollection<TripViewModel>()
                }
            };
        }

        public static IEnumerable<GroupViewModel> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");
            //This constructor load data from Models. Void constructor load design data
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            //TODO: This version does not have travaler associated

            _mainViewModel = new MainViewModel(new TravelerViewModel() { Identifier = new Guid(loader.GetString("GuidNull")) });
            
            return _mainViewModel.AllGroups;
        }

        public static GroupViewModel GetGroup(Int32 uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _mainViewModel.AllGroups.Where((group) => group.Identifier.Equals(uniqueId));
            _mainViewModel.AllGroups[0].CreateFake(FakeType.Trip);
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        async public static Task<TripViewModel> GetTrip(Int32 identifier)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _mainViewModel.AllGroups.SelectMany(group => group.Items).Where((item) => item.Identifier.Equals(identifier));
            if (matches.Count() == 1) return matches.First();
            return new TripViewModel();
        }

        #endregion

        async public static Task InitializeAsync()
        {
            _mainViewModel.AllGroups[0].Items.Clear();
            _mainViewModel.AllGroups[1].Items.Clear();
            await model.InitializeAsync();

            await DeleteTrashImages();
        }

        async private static Task DeleteTrashImages()
        {
            List<TrashImage> images = await model.GetTrashImagesAsync();

            foreach (TrashImage image in images)
            {
                try
                {
                    var localFolder = ApplicationData.Current.LocalFolder;
                    StorageFolder imagesFolder = await localFolder.CreateFolderAsync("images", CreationCollisionOption.OpenIfExists);

                    StorageFile oldFile = await imagesFolder.GetFileAsync(image.Name);
                    await oldFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    oldFile = null;

                    await model.DeleteTrashImageAsync(image.Name);
                }
                catch (Exception)
                {
                    //Nothing to do
                }
            }
        }

        async public static Task<int> SaveTripAsync(TripViewModel trip)
        {
            int tripIdentifier = -1;

            if (trip.Identifier == -1)
            {
                tripIdentifier = await model.SaveTripAsync(trip);

                if (tripIdentifier != -1)
                {
                    _mainViewModel.AllGroups[0].Items.Add(trip);
                    _mainViewModel.AllGroups[0].CreateFake(FakeType.Trip);
                }
            }
            else
            {
                await model.UpdateTrip(trip);
                tripIdentifier = trip.Identifier;
            }

            return tripIdentifier;
        }

        public static Guid GetTraveler()
        {
            //TODO: Only to local version
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            return new Guid(loader.GetString("GuidNull"));
        }

        async public static void DeleteSelectedTripsAsync()
        {
            // Simple linear search is acceptable for small data sets
            var matches = _mainViewModel.AllGroups.SelectMany(group => group.Items).Where((item) => item.IsSelected == true).ToList();

            var identifiers = from m in matches
                                    select m.Identifier;

            foreach (int identifier in identifiers)
            {
                await model.DeleteItemAsync(identifier);
                TripViewModel trip = await GetTrip(identifier);
                _mainViewModel.AllGroups[0].Items.Remove(trip);
            }
        }

        async public static Task<TripViewModel> GetFullTrip(int identifier)
        {
            TripViewModel trip = await GetTrip(identifier);
            trip.ItemsCost = 0;
            List<ItemViewModel> items = await model.GetItemsByTripAsync(identifier);

            if (items.Count == 0)
            {
                CreateFakeItem(trip);
            }
            else
            {
                foreach (ItemViewModel item in items)
                {
                    CategoryViewModel category = trip.CreateCategory((Category)item.Category);

                    item.Group = category;
                    item.Trip = trip;
                    category.Items.Add(item);
                    item.Trip.ItemsCost += Convert.ToInt64(item.Cost);
                }
            }

            return trip;
        }

        private static void CreateFakeItem(TripViewModel trip)
        {
            CategoryViewModel category = trip.CreateCategory(Category.Things);
            if (category.Items.Count == 0)
            {
                ItemViewModel item = new ItemViewModel();
                item.Identifier = -1;
                item.Title = "Note";
                item.Description = "Add items from the botton toolbar";
                item.Group = category;
                item.Trip = trip;
                category.Items.Add(item);
            }
        }

        public static TripViewModel GetSelectedTrip()
        {
            return _mainViewModel.SelectedTrip;
        }

        public static void SetSelectedTrip(TripViewModel trip)
        {
            _mainViewModel.SelectedTrip = trip;
            trip.SelectedItems = new ObservableCollection<ItemViewModel>();
        }

        async public static Task<int> SaveItem(ItemViewModel item)
        {
            if (item.Identifier == -1)
            {
                item.Identifier = await model.SaveItemAsync(item);
                item.Trip.DeleteFakeItem();
            }
            else
            {
                await model.UpdateItem(item);

                item.Group.Items.Remove(item);

                if (item.Group.Items.Count == 0)
                {
                    item.Trip.ItemGroups.Remove(item.Group);
                }

            }


            item.Trip.ItemsCost += Convert.ToInt64(item.Cost);

            CategoryViewModel category = item.Trip.CreateCategory((Category)item.Category);
            item.Group = category;
            category.Items.Add(item);

            return item.Identifier;        
        }

        public static void UpdatePhotoToSelectedTrip(string localPathImage)
        {
            if (_mainViewModel.SelectedTrip != null)
            {
                _mainViewModel.SelectedTrip.LocalPathImage = localPathImage;
                model.UpdateTrip(_mainViewModel.SelectedTrip);
            }
        }

        async public static void DeleteSelectedItemsAsync()
        {
            // Simple linear search is acceptable for small data sets
            var matches = _mainViewModel.SelectedTrip.ItemGroups.SelectMany(group => group.Items).Where((item) => item.IsSelected == true).ToList();

            foreach (ItemViewModel item in matches)
            {
                item.Trip.ItemsCost -= item.Cost;

                await model.DeleteItemAsync(item.Identifier);
                item.Group.Items.Remove(item);

                if (item.Group.Items.Count == 0)
                {
                    item.Trip.ItemGroups.Remove(item.Group);
                }

                if (item.Trip.ItemGroups.Count == 0)
                {
                    CreateFakeItem(item.Trip);
                }
            }
        }

        public static void RegisterTrashImage(string name)
        {
            model.RegisterTrashImage(name);
        }
    }
}