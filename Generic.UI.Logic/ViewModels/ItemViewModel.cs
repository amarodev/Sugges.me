using System;
using Generic.UI.Logic.Common;
using Generic.UI.Logic.Enumerations;
using Generic.UI.Logic.Models;
using Windows.UI.Xaml;

namespace Generic.UI.Logic.ViewModels
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

        private Guid? _user;

        public Guid? User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged("User");
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

        public CategoryViewModel Group { get; set; }

        public ParentViewModel Trip { get; set; }
    }
}
