
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Sugges.UI.Logic.Common;
using Sugges.UI.Logic.Enumerations;
/*using Suggestions = Sugges.UI.Logic.SuggestionsService;
using Travelers = Sugges.UI.Logic.TravelersService;
using Trips = Sugges.UI.Logic.TripsService;
using Sugges.UI.Logic.TripsService;*/
using Sugges.UI.Logic.ViewModels;
using System.Collections.Generic;
using System.IO;
using Sugges.UI.Logic.DataModel;


namespace Sugges.UI.Logic.Models
{
    internal class DatabaseModel : IModel
    {
        private string connectionString = string.Empty;

        public Guid SignUpAsync(TravelerViewModel traveler)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            return new Guid(loader.GetString("GuidNull"));
        }

        public async Task InitializeAsync()
        {
            var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            connectionString = loader.GetString("DataBasePath");

            if (!await DoesFileExistAsync(loader.GetString("DataBasePath")))
            {
                SQLite.SQLiteAsyncConnection context = new SQLite.SQLiteAsyncConnection(connectionString);
                await context.CreateTableAsync<Sugges.UI.Logic.DataModel.Item>();
            }
        }
        
        private async Task<bool> DoesFileExistAsync(string fileName) 
        {
            try
            {
                var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                await localFolder.GetFileAsync(fileName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<TripViewModel>> GetTripsAsync(Guid traveler)
        {
            SQLite.SQLiteAsyncConnection context = new SQLite.SQLiteAsyncConnection(connectionString);

            List<TripViewModel> trips = new List<TripViewModel>();
            //trips have the parent null, items have a trip as parent
            List<Item> result = await context.Table<Item>().Where(p => p.IsSuggestion == false && p.Parent == 0).ToListAsync();

            foreach (Item item in result)
            {
                trips.Add(new TripViewModel()
                { 
                    Title = item.Title,
                    Description = item.Description,
                    EndDate = Convert.ToDateTime(item.EndDate),
                    StartDate = Convert.ToDateTime(item.StartDate),
                    Category = (Int16)Sugges.UI.Logic.Enumerations.Category.Trips,
                    Cost = item.Cost,
                    Identifier = item.Identifier,
                    Latitude = item.Latitude,
                    Longitude = item.Longitude,
                    RemotePathImage = item.Image == null ? "/Assets/Trip.png" : item.Image,
                    LocalPathImage = item.Image == null ? "/Assets/Trip.png" : item.Image,
                    Traveler = new Guid(item.Traveler)
                });
            }

            return trips;
        }

        public async Task<List<TripViewModel>> GetSuggestionsAsync()
        {
            SQLite.SQLiteAsyncConnection context = new SQLite.SQLiteAsyncConnection(connectionString);

            List<TripViewModel> suggestions = new List<TripViewModel>();
            List<Item> result = await context.Table<Item>().Where(p => p.IsSuggestion == true).ToListAsync();

            foreach (Item item in result)
            {
                suggestions.Add(new TripViewModel()
                {
                    Title = item.Title,
                    Description = item.Description,
                    EndDate = Convert.ToDateTime(item.EndDate),
                    StartDate = Convert.ToDateTime(item.StartDate),
                    Category = (Int16)Sugges.UI.Logic.Enumerations.Category.Trips,
                    Cost = item.Cost,
                    Identifier = item.Identifier,
                    Latitude = item.Latitude,
                    Longitude = item.Longitude,
                    RemotePathImage = item.Image == null ? "/Assets/Suggestion.png" : item.Image,
                    LocalPathImage = item.Image == null ? "/Assets/Suggestion.png" : item.Image,
                    Traveler = new Guid(item.Traveler),
                    IsSuggestion = true
                });
            }

            return suggestions;
        }

        public async Task<int> SaveTripAsync(TripViewModel trip)
        {
            SQLite.SQLiteAsyncConnection context = new SQLite.SQLiteAsyncConnection(connectionString);

            Item newItem = new Item()
            {
                Identifier = trip.Identifier,
                Title = trip.Title,
                Description = trip.Description,
                Cost = trip.Cost,
                EndDate = trip.EndDate,
                StartDate = trip.StartDate,
                Latitude = trip.Latitude,
                Longitude = trip.Longitude,
                Traveler = trip.Traveler.ToString(),
                Image = trip.LocalPathImage
            };

            await context.InsertAsync(newItem);
            return newItem.Identifier;
        }

        public async Task DeleteItemAsync(int identifier)
        {
            SQLite.SQLiteAsyncConnection context = new SQLite.SQLiteAsyncConnection(connectionString);

            List<Item> childs = await context.Table<Item>().Where(p => p.Parent == identifier).ToListAsync();

            foreach(Item child in childs)
            {
                await context.DeleteAsync(child);
            }

            Item item = await context.Table<Item>().Where(p => p.Identifier == identifier).FirstOrDefaultAsync();

            if (item != null)
                await context.DeleteAsync(item);
        }

        public async Task<List<ItemViewModel>> GetItemsByTripAsync(int trip)
        {
            SQLite.SQLiteAsyncConnection context = new SQLite.SQLiteAsyncConnection(connectionString);

            List<ItemViewModel> result = new List<ItemViewModel>();
            List<Item> items = await context.Table<Item>().Where(p => p.Parent == trip).ToListAsync();

            foreach (Item item in items)
            { 
                result.Add(new ItemViewModel()
                {
                    Title = item.Title,
                    Description = item.Description,
                    Cost = item.Cost,
                    Latitude = item.Latitude,
                    Longitude = item.Longitude,
                    Traveler = new Guid(item.Traveler),
                    Category = (byte)item.Category,
                    Identifier = item.Identifier,
                    LocalPathImage = item.Image,
                });
            }

            return result;
        }

        Task<Guid> IModel.SignUpAsync(TravelerViewModel traveler)
        {
            throw new NotImplementedException();
        }

        public Task SignInAsync(TravelerViewModel traveler)
        {
            throw new NotImplementedException();
        }
        
        public Task<TravelerViewModel> GetTravelerInformationAsync(TravelerViewModel traveler)
        {
            throw new NotImplementedException();
        }

        async public Task<int> SaveItemAsync(ItemViewModel item)
        {
            SQLite.SQLiteAsyncConnection context = new SQLite.SQLiteAsyncConnection(connectionString);

            Item newItem = new Item()
            {
                Identifier = item.Identifier,
                Title = item.Title,
                Description = item.Description,
                Cost = item.Cost,
                Latitude = item.Latitude,
                Longitude = item.Longitude,
                Parent = item.Trip.Identifier,
                Traveler = item.Traveler.ToString(),
                Category = item.Category
            };

            await context.InsertAsync(newItem);
            return newItem.Identifier;
        }


        async public Task UpdateTrip(TripViewModel trip)
        {
            SQLite.SQLiteAsyncConnection context = new SQLite.SQLiteAsyncConnection(connectionString);

            List<ItemViewModel> result = new List<ItemViewModel>();
            Item item = await context.Table<Item>().Where(p => p.Identifier == trip.Identifier).FirstOrDefaultAsync();

            item.Image = trip.LocalPathImage;
            item.Title = trip.Title;
            item.Description = trip.Description;
            item.Cost = trip.Cost;
            item.StartDate = trip.StartDate;

            if (item != null)
            {
                await context.UpdateAsync(item);
            }
        }

        async public Task UpdateItem(ItemViewModel itemViewModel)
        {
            SQLite.SQLiteAsyncConnection context = new SQLite.SQLiteAsyncConnection(connectionString);

            List<ItemViewModel> result = new List<ItemViewModel>();
            Item item = await context.Table<Item>().Where(p => p.Identifier == itemViewModel.Identifier).FirstOrDefaultAsync();

            item.Title = itemViewModel.Title;
            item.Description = itemViewModel.Description;
            item.Cost = itemViewModel.Cost;
            item.Category = itemViewModel.Category;

            if (item != null)
            {
                await context.UpdateAsync(item);
            }
        }

        async public Task RegisterTrashImage(string imageName)
        {
            SQLite.SQLiteAsyncConnection context = new SQLite.SQLiteAsyncConnection(connectionString);
            TrashImage image = await context.Table<TrashImage>().Where(p => p.Name == imageName).FirstOrDefaultAsync();

            if (image == null)
            {
                TrashImage newItem = new TrashImage()
                {
                    Name = imageName,
                };

                await context.InsertAsync(newItem);
            }
        }


        async public Task<List<TrashImage>> GetTrashImagesAsync()
        {
            SQLite.SQLiteAsyncConnection context = new SQLite.SQLiteAsyncConnection(connectionString);
            await context.CreateTableAsync<Sugges.UI.Logic.DataModel.TrashImage>();
            List<TrashImage> result = await context.Table<TrashImage>().ToListAsync();
            return result;
        }

        async public Task DeleteTrashImageAsync(string imageName)
        {
            SQLite.SQLiteAsyncConnection context = new SQLite.SQLiteAsyncConnection(connectionString);
            TrashImage image = await context.Table<TrashImage>().Where(p => p.Name == imageName).FirstOrDefaultAsync();

            if (image != null)
                await context.DeleteAsync(image);
        }


        async public Task<TripViewModel> GetTripAsync(int identifier)
        {
            SQLite.SQLiteAsyncConnection context = new SQLite.SQLiteAsyncConnection(connectionString);

            TripViewModel trip = new TripViewModel();
            //trips have the parent null, items have a trip as parent
            Item item = await context.Table<Item>().Where(p => p.IsSuggestion == false && p.Identifier == identifier && p.Parent == 0).FirstOrDefaultAsync();

            trip = new TripViewModel()
            {
                Title = item.Title,
                Description = item.Description,
                EndDate = Convert.ToDateTime(item.EndDate),
                StartDate = Convert.ToDateTime(item.StartDate),
                Category = (Int16)Sugges.UI.Logic.Enumerations.Category.Trips,
                Cost = item.Cost,
                Identifier = item.Identifier,
                Latitude = item.Latitude,
                Longitude = item.Longitude,
                RemotePathImage = item.Image == null ? "/Assets/Trip.png" : item.Image,
                LocalPathImage = item.Image == null ? "/Assets/Trip.png" : item.Image,
                Traveler = new Guid(item.Traveler)
            };

            return trip;
        }


        async public Task<List<TripViewModel>> GetGroupsByCriteria(string queryText)
        {
            SQLite.SQLiteAsyncConnection context = new SQLite.SQLiteAsyncConnection(connectionString);

            List<TripViewModel> trips = new List<TripViewModel>();
            //trips have the parent null, items have a trip as parent
            List<Item> result = await context.Table<Item>().Where(p => p.IsSuggestion == false && p.Parent == 0
                && (p.Title.Contains(queryText) || p.Description.Contains(queryText))).ToListAsync();

            foreach (Item item in result)
            {
                trips.Add(new TripViewModel()
                {
                    Title = item.Title,
                    Description = item.Description,
                    EndDate = Convert.ToDateTime(item.EndDate),
                    StartDate = Convert.ToDateTime(item.StartDate),
                    Category = (Int16)Sugges.UI.Logic.Enumerations.Category.Trips,
                    Cost = item.Cost,
                    Identifier = item.Identifier,
                    Latitude = item.Latitude,
                    Longitude = item.Longitude,
                    RemotePathImage = item.Image == null ? "/Assets/Trip.png" : item.Image,
                    LocalPathImage = item.Image == null ? "/Assets/Trip.png" : item.Image,
                    Traveler = new Guid(item.Traveler)
                });
            }

            return trips;
        }
    }
}
