
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Generic.UI.Logic.Common;
using Generic.UI.Logic.Enumerations;
using Generic.UI.Logic.ViewModels;
using System.Collections.Generic;

namespace Generic.UI.Logic.Models
{
    interface IModel 
    {
        Task InitializeAsync();
        Task<Guid> SignUpAsync(UserViewModel user);
        Task SignInAsync(UserViewModel user);
        Task<List<ParentViewModel>> GetParentsAsync(Guid user);
        Task<UserViewModel> GetUserInformationAsync(UserViewModel user);
        Task<int> SaveParentAsync(ParentViewModel trip);
        Task DeleteItemAsync(int identifier);
        Task<List<ItemViewModel>> GetItemsByParentAsync(int trip);
        Task<int> SaveItemAsync(ItemViewModel item);
        Task UpdateParent(ParentViewModel parentViewModel);
        Task UpdateItem(ItemViewModel item);
        Task RegisterTrashImage(string name);
        Task<List<DataModel.TrashImage>> GetTrashImagesAsync();
        Task DeleteTrashImageAsync(string imageName);
        Task<ParentViewModel> GetParentAsync(int identifier);
        Task<List<ParentViewModel>> GetGroupsByCriteria(string queryText);
    }
}
