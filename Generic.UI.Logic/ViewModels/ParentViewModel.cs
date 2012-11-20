using System;
using System.Collections.ObjectModel;
using Generic.UI.Logic.Common;
using Generic.UI.Logic.Enumerations;
using Generic.UI.Logic.Models;
using Windows.UI.Xaml;
using System.Linq;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Storage.FileProperties;

namespace Generic.UI.Logic.ViewModels
{
    public class ParentViewModel : ItemViewModel
    {
        IModel model;

        public ParentViewModel()
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