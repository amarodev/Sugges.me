
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Generic.UI.Logic.Common;
using Generic.UI.Logic.Enumerations;
using Generic.UI.Logic.ViewModels;
using System.Collections.Generic;
using System.IO;
using Generic.UI.Logic.DataModel;
using Windows.ApplicationModel.Resources;


namespace Generic.UI.Logic.Models
{
    internal class DatabaseModel : IModel
    {
        private string connectionString = string.Empty;
        private static ResourceLoader loader = new ResourceLoader();

        public Guid SignUpAsync(UserViewModel user)
        {
            return new Guid(loader.GetString("GuidNull"));
        }

        public async Task InitializeAsync()
        {
            var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            connectionString = loader.GetString("ActualDataBasePath");

            if (!await DoesFileExistAsync(loader.GetString("ActualDataBasePath")))
            {
                SQLite.SQLiteAsyncConnection context = new SQLite.SQLiteAsyncConnection(connectionString);
                await context.CreateTableAsync<Generic.UI.Logic.DataModel.Item>();
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

        public async Task<List<ParentViewModel>> GetParentsAsync(Guid user)
        {
            SQLite.SQLiteAsyncConnection context = new SQLite.SQLiteAsyncConnection(connectionString);

            List<ParentViewModel> parents = new List<ParentViewModel>();
            List<Item> result = await context.Table<Item>().Where(p => p.Parent == 0).ToListAsync();

            foreach (Item item in result)
            {
                parents.Add(new ParentViewModel()
                { 
                    Title = item.Title,
                    Description = item.Description,
                    StartDate = Convert.ToDateTime(item.StartDate),
                    Category = (Int16)Generic.UI.Logic.Enumerations.Category.Trips,
                    Cost = item.Cost,
                    Identifier = item.Identifier,
                    Latitude = item.Latitude,
                    Longitude = item.Longitude,
                    LocalPathImage = item.Image == null ? loader.GetString("DefaultParentImage") : item.Image,
                    User = new Guid(item.User)
                });
            }

            return parents;
        }

        public async Task<int> SaveParentAsync(ParentViewModel trip)
        {
            SQLite.SQLiteAsyncConnection context = new SQLite.SQLiteAsyncConnection(connectionString);

            Item newItem = new Item()
            {
                Identifier = trip.Identifier,
                Title = trip.Title,
                Description = trip.Description,
                Cost = trip.Cost,
                StartDate = trip.StartDate,
                Latitude = trip.Latitude,
                Longitude = trip.Longitude,
                User = trip.User.ToString(),
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

        public async Task<List<ItemViewModel>> GetItemsByParentAsync(int trip)
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
                    User = new Guid(item.User),
                    Category = (byte)item.Category,
                    Identifier = item.Identifier,
                    LocalPathImage = item.Image,
                });
            }

            return result;
        }

        Task<Guid> IModel.SignUpAsync(UserViewModel user)
        {
            throw new NotImplementedException();
        }

        public Task SignInAsync(UserViewModel user)
        {
            throw new NotImplementedException();
        }
        
        public Task<UserViewModel> GetUserInformationAsync(UserViewModel user)
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
                User = item.User.ToString(),
                Category = item.Category
            };

            await context.InsertAsync(newItem);
            return newItem.Identifier;
        }


        async public Task UpdateParent(ParentViewModel trip)
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
            await context.CreateTableAsync<Generic.UI.Logic.DataModel.TrashImage>();
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


        async public Task<ParentViewModel> GetParentAsync(int identifier)
        {
            SQLite.SQLiteAsyncConnection context = new SQLite.SQLiteAsyncConnection(connectionString);

            ParentViewModel trip = new ParentViewModel();
            //trips have the parent null, items have a trip as parent
            Item item = await context.Table<Item>().Where(p => p.Identifier == identifier && p.Parent == 0).FirstOrDefaultAsync();

            trip = new ParentViewModel()
            {
                Title = item.Title,
                Description = item.Description,
                StartDate = Convert.ToDateTime(item.StartDate),
                Category = (Int16)Generic.UI.Logic.Enumerations.Category.Trips,
                Cost = item.Cost,
                Identifier = item.Identifier,
                Latitude = item.Latitude,
                Longitude = item.Longitude,
                LocalPathImage = item.Image == null ? loader.GetString("DefaultParentImage") : item.Image,
                User = new Guid(item.User)
            };

            return trip;
        }


        async public Task<List<ParentViewModel>> GetGroupsByCriteria(string queryText)
        {
            SQLite.SQLiteAsyncConnection context = new SQLite.SQLiteAsyncConnection(connectionString);

            List<ParentViewModel> parents = new List<ParentViewModel>();
            List<Item> result = await context.Table<Item>().Where(p => p.Parent == 0
                && (p.Title.Contains(queryText) || p.Description.Contains(queryText))).ToListAsync();

            foreach (Item item in result)
            {
                parents.Add(new ParentViewModel()
                {
                    Title = item.Title,
                    Description = item.Description,
                    StartDate = Convert.ToDateTime(item.StartDate),
                    Category = (Int16)Generic.UI.Logic.Enumerations.Category.Trips,
                    Cost = item.Cost,
                    Identifier = item.Identifier,
                    Latitude = item.Latitude,
                    Longitude = item.Longitude,
                    LocalPathImage = item.Image == null ? loader.GetString("DefaultParentImage") : item.Image,
                    User = new Guid(item.User)
                });
            }

            return parents;
        }
    }
}
