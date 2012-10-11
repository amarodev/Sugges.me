using System;
using Sugges.UI.Logic.Common;
using Sugges.UI.Logic.Enumerations;
using Sugges.UI.Logic.Models;
using Windows.UI.Xaml;

namespace Sugges.UI.Logic.ViewModels
{
    public class ItemViewModel : BindableBase
    {
        IModel model;

        public ItemViewModel()
        {
            model = new DatabaseModel();
        }

        private int _identifier;

        public int Identifier
        {
            get { return _identifier; }
            set
            {
                _identifier = value;
                OnPropertyChanged("Identifier");
            }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        private string _description;

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        private string _latitude;

        public string Latitude
        {
            get { return _latitude; }
            set
            {
                _latitude = value;
                OnPropertyChanged("Latitude");
            }
        }

        private string _longitude;

        public string Longitude
        {
            get { return _longitude; }
            set
            {
                _longitude = value;
                OnPropertyChanged("Longitude");
            }
        }

        private long? _cost; //Mandatory always

        public long? Cost
        {
            get { return _cost; }
            set
            {
                _cost = value;
                OnPropertyChanged("Cost");
            }
        }

        /*private long _parent;

        public long Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                OnPropertyChanged("Parent");
            }
        }*/

        private Int16 _category;

        public Int16 Category
        {
            get { return _category; }
            set
            {
                _category = value;
                OnPropertyChanged("Category");
            }
        }

        private Guid? _traveler;

        public Guid? Traveler
        {
            get { return _traveler; }
            set
            {
                _traveler = value;
                OnPropertyChanged("Category");
            }
        }

        private string _localPathImage;

        public string LocalPathImage
        {
            get { return _localPathImage; }
            set
            {
                _localPathImage = value;
                OnPropertyChanged("LocalPathImage");
            }
        }

        private string _localRemoteImage;

        public string RemotePathImage
        {
            get { return _localRemoteImage; }
            set
            {
                _localRemoteImage = value;
                OnPropertyChanged("LocalRemoteImage");
            }
        }

        public CategoryViewModel Group { get; set; }

        public TripViewModel Trip { get; set; }
    }
}
