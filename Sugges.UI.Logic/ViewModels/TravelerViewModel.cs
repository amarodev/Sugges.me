using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sugges.UI.Logic.Models;
using Sugges.UI.Logic.ViewModels;
using Windows.UI.Xaml;

namespace Sugges.UI.Logic.ViewModels
{
    public class TravelerViewModel: Common.BindableBase
    {
        public ObservableCollection<TripViewModel> Trips { get; private set; }
        IModel model;

        public TravelerViewModel()
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            model = new DatabaseModel();

            Trips = new ObservableCollection<TripViewModel>();
            this.Identifier = new Guid(loader.GetString("GuidNull"));
            this.StatusMessage = "See more options on settings";
            this.Status = Enumerations.TravelerStatus.Stopped;
        }

        async public Task SignIn()
        {
            try
            {
                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

                this.StatusMessage = string.Empty;
                this.Status = Enumerations.TravelerStatus.Working;

                if (this.CheckCredentials())
                    await model.SignInAsync(this);

                this.Status = !this.Identifier.Equals(new Guid(loader.GetString("GuidNull"))) ? Enumerations.TravelerStatus.LoggedIn : Enumerations.TravelerStatus.Stopped;
                
                if (this.StatusMessage.Equals(string.Empty))
                    this.StatusMessage = !this.Identifier.Equals(new Guid(loader.GetString("GuidNull"))) ? "Welcome" : "User or password incorrect";
            }
            catch (Exception)
            {
                this.StatusMessage = "Sorry, something is wrong, please try later";
                this.Status = Enumerations.TravelerStatus.Stopped;
            }
        }

        async public Task SignUp()
        {
            try
            {
                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

                this.StatusMessage = string.Empty;
                this.Status = Enumerations.TravelerStatus.Working;

                if (this.CheckData())
                {
                    this.Identifier = await model.SignUpAsync(this);
                    
                    if (this.Identifier == new Guid(loader.GetString("GuidNull")))
                    {
                        this.StatusMessage = "The username already exists";
                        this.Status = Enumerations.TravelerStatus.Stopped;
                    }
                    else
                    {
                        this.StatusMessage = "Your account has been created";
                        this.Status = Enumerations.TravelerStatus.AccountCreated;
                    }
                }
            }
            catch (Exception)
            {
                this.StatusMessage = "Sorry, something is wrong, please try later";
                this.Status = Enumerations.TravelerStatus.Stopped;
            }
        }

        #region ComplementaryMethods
        private bool CheckCredentials()
        {
            if (this.UserName == null ||this.UserName.Trim() == string.Empty)
            {
                this.StatusMessage = "Please, review your username";
                return false;
            }

            if (this.Password == null || (this.Password.Trim() == string.Empty))
            {
                this.StatusMessage = "Please, review your password";
                return false;
            }

            return true;
        }
        private bool CheckData()
        {
            if (this.UserName.Trim() == string.Empty)
            {
                this.StatusMessage = "Please, review your username";
                return false;
            }

            if (this.Password.Trim() == string.Empty)
            {
                this.StatusMessage = "Please, review your password";
                return false;
            }

            if (this.Email.Trim() == string.Empty)
            {
                this.StatusMessage = "Please, review your email";
                return false;
            }

            if (this.FirstName.Trim() == string.Empty)
            {
                this.StatusMessage = "Please, review your first name";
                return false;
            }

            if (this.LastName.Trim() == string.Empty)
            {
                this.StatusMessage = "Please, review your last name";
                return false;
            }

            return true;
        }
        #endregion

        #region Properties

        private string _statusMessage;

        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                _statusMessage = value;
                OnPropertyChanged("StatusMessage");
            }
        }

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

        private Sugges.UI.Logic.Enumerations.TravelerStatus _status;

        public Sugges.UI.Logic.Enumerations.TravelerStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged("State");
                OnPropertyChanged("WorkingVisibility");
            }
        }

        public Visibility WorkingVisibility
        {
            get { return _status == Enumerations.TravelerStatus.Working ? Visibility.Visible : Visibility.Collapsed; }
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

        async public Task GetInfo()
        {
            try
            {
                this.StatusMessage = string.Empty;
                this.Status = Enumerations.TravelerStatus.Working;

                TravelerViewModel traveler = await model.GetTravelerInformationAsync(this);

                this.Status = traveler == null ? Enumerations.TravelerStatus.Stopped : Enumerations.TravelerStatus.LoggedIn;
                this.StatusMessage = traveler == null ? "Account doesn't exists" : "Welcome";
            }
            catch (Exception)
            {
                this.StatusMessage = "Sorry, something is wrong, please try later";
                this.Status = Enumerations.TravelerStatus.Stopped;
            }
        }
    }
}
