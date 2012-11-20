using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Generic.UI.Logic.Models;
using Generic.UI.Logic.ViewModels;
using Windows.UI.Xaml;

namespace Generic.UI.Logic.ViewModels
{
    public class UserViewModel: Common.BindableBase
    {
        IModel model;

        public UserViewModel()
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            model = new DatabaseModel();

            this.Identifier = new Guid(loader.GetString("GuidNull"));
        }

        #region Properties

        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set 
            {
                _userName = value;
                OnPropertyChanged("UserName");
            }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set 
            { 
                _password = value;
                OnPropertyChanged("Password");
            }
        }

        private string _firstName;

        public string FirstName
        {
            get { return _firstName; }
            set 
            { 
                _firstName = value;
                OnPropertyChanged("Name");
            }
        }

        private string _lastName;

        public string LastName
        {
            get { return _lastName; }
            set 
            { 
                _lastName = value;
                OnPropertyChanged("LastName");
            }
        }

        private string _email;

        public string Email
        {
            get { return _email; }
            set 
            { 
                _email = value;
                OnPropertyChanged("Email");
            }
        }

        private Guid? _identifier;

        public Guid? Identifier
        {
            get { return _identifier; }
            set
            {
                _identifier = value;
                OnPropertyChanged("Identifier");
            }
        }

        #endregion
    }
}
