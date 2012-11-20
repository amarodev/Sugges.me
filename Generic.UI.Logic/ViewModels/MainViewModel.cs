using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Generic.UI.Logic.Common;
using Generic.UI.Logic.Enumerations;
using Generic.UI.Logic.Models;
using Windows.Foundation;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using System.Collections.Generic;
using System.Linq;
using Generic.UI.Logic.DataModel;
using Windows.Storage;
using Windows.ApplicationModel.Resources;


namespace Generic.UI.Logic.ViewModels
{
    public class MainViewModel : BindableBase
	{
        private static ResourceLoader loader = new ResourceLoader();

        private static IModel _model;
        private static MainViewModel _mainViewModel = new MainViewModel();

        /// <summary>
        /// This constructor is used in design time
        /// </summary>
        public MainViewModel()
        {
            CreateDesignData();
        }

        /// <summary>
        /// This constructor should be used in execution time
        /// </summary>
        /// <param name="user"></param>
        public MainViewModel(UserViewModel user)
        {
            _user = user;
            _model = new DatabaseModel();
            AllGroups = new ObservableCollection<GroupViewModel>();

            CreateDefaultGroups();
        }

        private void CreateDefaultGroups()
        {
            this.AllGroups.Add(new GroupViewModel()
            {
                Identifier = 0,
                Title = "Your trips",
                Description = "Your planned trips"
            });
        }

        #region Properties

        private UserViewModel _user;

        public UserViewModel User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged("User");
            }
        }

        private ParentViewModel _selectedTrip;

        public ParentViewModel SelectedTrip
        {
            get { return this._selectedTrip; }
            set { this.SetProperty(ref this._selectedTrip, value); }
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

        async public static Task LoadTripsAsync()
        {
            List<ParentViewModel> trips = await _model.GetParentsAsync((Guid)_mainViewModel.User.Identifier);
                
            foreach (ParentViewModel trip in trips)
            {
                trip.Group = _mainViewModel.AllGroups[0]; //Very important!
                _mainViewModel.AllGroups[0].Items.Add(trip);
            }

            _mainViewModel.AllGroups[0].CreateFake();
        }

        #region Static Default Methods

        public static ParentViewModel GetItem(object identifier)
        {
            return new ParentViewModel()
            {
                Group = new GroupViewModel() 
                { 
                    Items = new ObservableCollection<ParentViewModel>()
                }
            };
        }

        public static IEnumerable<GroupViewModel> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");
            //TODO: This version does not have user associated
            _mainViewModel = new MainViewModel(new UserViewModel() { Identifier = new Guid(loader.GetString("GuidNull")) });
            
            return _mainViewModel.AllGroups;
        }

        public static GroupViewModel GetGroup(Int32 uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _mainViewModel.AllGroups.Where((group) => group.Identifier.Equals(uniqueId));
            _mainViewModel.AllGroups[0].CreateFake();
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        async public static Task<ParentViewModel> GetTrip(Int32 identifier)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _mainViewModel.AllGroups.SelectMany(group => group.Items).Where((item) => item.Identifier.Equals(identifier));
            if (matches.Count() == 1) return matches.First();
            return new ParentViewModel();
        }

        #endregion

        async public static Task InitializeAsync()
        {
            _mainViewModel.AllGroups[0].Items.Clear();
            await _model.InitializeAsync();

            await DeleteTrashImages();
        }

        async private static Task DeleteTrashImages()
        {
            List<TrashImage> images = await _model.GetTrashImagesAsync();

            foreach (TrashImage image in images)
            {
                try
                {
                    var localFolder = ApplicationData.Current.LocalFolder;
                    StorageFolder imagesFolder = await localFolder.CreateFolderAsync("images", CreationCollisionOption.OpenIfExists);

                    StorageFile oldFile = await imagesFolder.GetFileAsync(image.Name);
                    await oldFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    oldFile = null;

                    await _model.DeleteTrashImageAsync(image.Name);
                }
                catch (Exception)
                {
                    //Nothing to do
                }
            }
        }

        async public static Task<int> SaveTripAsync(ParentViewModel trip)
        {
            int tripIdentifier = -1;

            if (trip.Identifier == -1)
            {
                tripIdentifier = await _model.SaveParentAsync(trip);

                if (tripIdentifier != -1)
                {
                    _mainViewModel.AllGroups[0].Items.Add(trip);
                    _mainViewModel.AllGroups[0].CreateFake();
                }
            }
            else
            {
                await _model.UpdateParent(trip);
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
                await _model.DeleteItemAsync(identifier);
                ParentViewModel trip = await GetTrip(identifier);
                _mainViewModel.AllGroups[0].Items.Remove(trip);
            }
        }

        async public static Task<ParentViewModel> GetFullTrip(int identifier)
        {
            ParentViewModel trip = await GetTrip(identifier);
            trip.ItemsCost = 0;
            List<ItemViewModel> items = await _model.GetItemsByParentAsync(identifier);

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

        private static void CreateFakeItem(ParentViewModel trip)
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

        public static ParentViewModel GetSelectedTrip()
        {
            return _mainViewModel.SelectedTrip;
        }

        public static void SetSelectedTrip(ParentViewModel trip)
        {
            _mainViewModel.SelectedTrip = trip;
            trip.SelectedItems = new ObservableCollection<ItemViewModel>();
        }

        async public static Task<int> SaveItem(ItemViewModel item)
        {
            if (item.Identifier == -1)
            {
                item.Identifier = await _model.SaveItemAsync(item);
                item.Trip.DeleteFakeItem();
            }
            else
            {
                await _model.UpdateItem(item);

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
                _model.UpdateParent(_mainViewModel.SelectedTrip);
            }
        }

        async public static void DeleteSelectedItemsAsync()
        {
            // Simple linear search is acceptable for small data sets
            var matches = _mainViewModel.SelectedTrip.ItemGroups.SelectMany(group => group.Items).Where((item) => item.IsSelected == true).ToList();

            foreach (ItemViewModel item in matches)
            {
                item.Trip.ItemsCost -= item.Cost;

                await _model.DeleteItemAsync(item.Identifier);
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
            _model.RegisterTrashImage(name);
        }

        async public static Task<ObservableCollection<GroupViewModel>> GetGroupsByCriteria(string queryText)
        {
            ObservableCollection<GroupViewModel> query = new ObservableCollection<GroupViewModel>();
            List<ParentViewModel> trips = await _model.GetGroupsByCriteria(queryText);

            query.Add(new GroupViewModel()
            {
                Identifier = 0,
                Title = "Your trips",
                Description = "Result of a search",
                LocalPathImage = loader.GetString("DefaultParentImage"),
            });

            foreach (ParentViewModel trip in trips)
            {
                query[0].Items.Add(trip);
            }

            return query;
        }

        #region DesignData
        private void CreateDesignData()
        {
            this.AllGroups.Add(new GroupViewModel()
            {
                Title = "d:Your trips",
                Description = "Your planned trips",
                LocalPathImage = loader.GetString("DefaultParentImage"),
            });

            this.AllGroups.Add(new GroupViewModel()
            {
                Title = "d:Suggestions",
                Description = "Suggestion from our platform",
                LocalPathImage = "/Assets/Suggestion.png",
            });

            if (this.AllGroups[0].Items.Count < 4)
            {
                for (int i = this.AllGroups[0].Items.Count; i < 4; i++)
                {
                    this.AllGroups[0].Items.Add(
                    new ParentViewModel()
                    {
                        Title = "d:Probando nombre muy muy muy muy pero muy largo",
                        Description = "Probando nombre muy muy muy muy pero muy largo y este tambien muy largo demasiado largo",
                        LocalPathImage = loader.GetString("DefaultParentImage"),
                        StartDate = new DateTime(2012, 05, 05),
                        Cost = 100,
                        ItemGroups = new ObservableCollection<CategoryViewModel>() 
                        {
                            new CategoryViewModel()
                            {
                                Title = "d:Cosas",
                                Description = "Probando nombre muy muy muy muy pero muy largo y este tambien muy largo demasiado largo",
                                LocalPathImage = loader.GetString("DefaultParentImage"),
                                Items = new ObservableCollection<ItemViewModel>()
                                {
                                    new ItemViewModel()
                                    {
                                        Title = "d:Bufanda",
                                        Description = "Probando nombre muy muy muy muy pero muy largo y este tambien muy largo demasiado largo",
                                        LocalPathImage = loader.GetString("DefaultParentImage"),
                                        Cost = 24,
                                        Trip = new ParentViewModel()
                                        {
                                            Title = "Argentina",
                                            Description = "Testing",
                                            Cost = 100,
                                            ItemsCost = 150,
                                            LocalPathImage = loader.GetString("DefaultParentImage"),
                                            StartDate = new DateTime(2012,9,14),
                                        }
                                    },
                                    new ItemViewModel()
                                    {
                                        Title = "d:Guantes",
                                        LocalPathImage = loader.GetString("DefaultParentImage"),
                                        Cost = 100,
                                        Trip = new ParentViewModel()
                                        {
                                            Title = "Argentina",
                                            Description = "Testing",
                                            Cost = 100,
                                            ItemsCost = 150,
                                            LocalPathImage = loader.GetString("DefaultParentImage"),
                                            StartDate = new DateTime(2012,9,14),
                                        }
                                    },
                                    new ItemViewModel()
                                    {
                                        Title = "d:Plata",
                                        Description = "Probando nombre muy muy muy muy pero muy largo y este tambien muy largo demasiado largo",
                                        LocalPathImage = loader.GetString("DefaultParentImage"),
                                        Trip = new ParentViewModel()
                                        {
                                            Title = "Argentina",
                                            Description = "Testing",
                                            Cost = 100,
                                            ItemsCost = 150,
                                            LocalPathImage = loader.GetString("DefaultParentImage"),
                                            StartDate = new DateTime(2012,9,14),
                                        }
                                    }
                                }
                            },
                            new CategoryViewModel()
                            {
                                Title = "d:Amigos",
                                Description = "Probando nombre muy muy muy muy pero muy largo y este tambien muy largo demasiado largo",
                                LocalPathImage = loader.GetString("DefaultParentImage"),
                                Items = new ObservableCollection<ItemViewModel>()
                                {
                                    new ItemViewModel()
                                    {
                                        Title = "d:Cristian",
                                        Description = "Probando nombre muy muy muy muy pero muy largo y este tambien muy largo demasiado largo",
                                        LocalPathImage = loader.GetString("DefaultParentImage"),
                                    }
                                }
                            },
                            new CategoryViewModel()
                            {
                                Title = "d:Amigos",
                                Description = "Probando nombre muy muy muy muy pero muy largo y este tambien muy largo demasiado largo",
                                LocalPathImage = loader.GetString("DefaultParentImage"),
                                Items = new ObservableCollection<ItemViewModel>()
                                {
                                    new ItemViewModel()
                                    {
                                        Title = "d:Cristian",
                                        Description = "Probando nombre muy muy muy muy pero muy largo y este tambien muy largo demasiado largo",
                                        LocalPathImage = loader.GetString("DefaultParentImage"),
                                    }
                                }
                            }
                        },
                    });
                }
            }
        }
        #endregion
    }
}