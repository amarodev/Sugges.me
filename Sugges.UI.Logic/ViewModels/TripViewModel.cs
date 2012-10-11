using System;
using System.Collections.ObjectModel;
using Sugges.UI.Logic.Common;
using Sugges.UI.Logic.Enumerations;
using Sugges.UI.Logic.Models;
using Windows.UI.Xaml;
using System.Linq;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Storage.FileProperties;

namespace Sugges.UI.Logic.ViewModels
{
    public class TripViewModel : ItemViewModel
    {
        IModel model;

        public TripViewModel()
        {
            model = new DatabaseModel();
            ItemGroups = new ObservableCollection<CategoryViewModel>();
        }

        private GroupViewModel _group;

        public GroupViewModel Group
        {
            get { return _group; }
            set { _group = value; }
        }



        private ObservableCollection<ItemViewModel> _selectedItems;

        public ObservableCollection<ItemViewModel> SelectedItems
        {
            get { return _selectedItems; }
            set
            {
                _selectedItems = value;
            }
        }

        private ObservableCollection<CategoryViewModel> _itemGroups;

        public ObservableCollection<CategoryViewModel> ItemGroups
        {
            get { return _itemGroups; }
            set
            {
                _itemGroups = value;
            }
        }

        private long? _itemsCost;

        public long? ItemsCost
        {
            get { return _itemsCost; } 
            set 
            {
                _itemsCost = value;
                OnPropertyChanged("ItemsCost");
            }
        }

        private bool _isSuggestion;

        public bool IsSuggestion
        {
            get { return _isSuggestion; }
            set
            {
                _isSuggestion = value;
                OnPropertyChanged("IsSuggestion");
            }
        }

        private DateTime _startDate; //Mandatory for Trips

        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                OnPropertyChanged("StartDate");
            }
        }

        private DateTime? _endDate; //Not used yet for Trips

        public DateTime? EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                OnPropertyChanged("EndDate");
            }
        }

        public CategoryViewModel CreateCategory(Category category)
        {
            CategoryViewModel categorySelected = ItemGroups.FirstOrDefault(p => p.Identifier == (Int32)category);

            if (categorySelected == null)
            {
                categorySelected = new CategoryViewModel()
                {
                    Identifier = (Int32)category,
                    Title = Enum.GetName(typeof(Category), (Int32)category),
                    Trip = this,
                };

                ItemGroups.Add(categorySelected);
            }

            return categorySelected;
        }

        internal void DeleteFakeItem()
        {
            ItemViewModel matches = this.ItemGroups.SelectMany(group => group.Items).Where((item) => item.Identifier == -1).FirstOrDefault();
            if (matches != null)
            {
                CategoryViewModel category = matches.Group;
                category.Items.Remove(matches);

                if (category.Items.Count == 0)
                    this.ItemGroups.Remove(category);
            }
        }
 
    }    
}