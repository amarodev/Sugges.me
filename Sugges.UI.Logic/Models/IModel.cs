
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Sugges.UI.Logic.Common;
using Sugges.UI.Logic.Enumerations;
using Sugges.UI.Logic.ViewModels;
using System.Collections.Generic;

namespace Sugges.UI.Logic.Models
{
    interface IModel 
    {
        Task InitializeAsync();
        Task<Guid> SignUpAsync(TravelerViewModel traveler);
        Task SignInAsync(TravelerViewModel traveler);
        Task<List<TripViewModel>> GetTripsAsync(Guid traveler);
        Task<List<TripViewModel>> GetSuggestionsAsync();
        Task<TravelerViewModel> GetTravelerInformationAsync(TravelerViewModel traveler);
        Task<int> SaveTripAsync(TripViewModel trip);
        Task DeleteItemAsync(int identifier);
        Task<List<ItemViewModel>> GetItemsByTripAsync(int trip);
        Task<int> SaveItemAsync(ItemViewModel item);
        Task UpdateTrip(TripViewModel tripViewModel);
        Task UpdateItem(ItemViewModel item);
        Task RegisterTrashImage(string name);
        Task<List<DataModel.TrashImage>> GetTrashImagesAsync();
        Task DeleteTrashImageAsync(string p);
        Task<TripViewModel> GetTripAsync(int identifier);
        Task<List<TripViewModel>> GetGroupsByCriteria(string queryText);
    }
}
