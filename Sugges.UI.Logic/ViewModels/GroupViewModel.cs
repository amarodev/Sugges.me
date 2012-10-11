using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sugges.UI.Logic.Enumerations;
using Sugges.UI.Logic.Models;

namespace Sugges.UI.Logic.ViewModels
{
    public class GroupViewModel: ItemViewModel
    {
        IModel model;

        public GroupViewModel()
        {
            model = new DatabaseModel();
        }

        private ObservableCollection<TripViewModel> _items = new ObservableCollection<TripViewModel>();
        public ObservableCollection<TripViewModel> Items
        {
            set { _items = value; }
            get { return this._items; }
        }

        private bool _areSuggestions;
        public bool AreSuggestions
        {
            set { _areSuggestions = value; }
            get { return this._areSuggestions; }
        }

        public void CreateFake(FakeType fakeType)
        {
            TripViewModel fake;

            if (fakeType == FakeType.Trip)
            {
                TripViewModel fakeTrip = this.Items.FirstOrDefault(p => p.Identifier == -1);
                if (fakeTrip != null)
                    this.Items.Remove(fakeTrip);

                fake = new TripViewModel()
                {
                    Identifier = -1,
                    Title = "New Trip?",
                    Description = "Click here to add a trip",
                    LocalPathImage = "Assets/NewTrip.png",
                };
            }
            else
            {
                fake = new TripViewModel()
                {
                    Identifier = -1,
                    Title = "Comming Soon",
                    Description = "We are working",
                    LocalPathImage = "Assets/Suggestion.png",
                    IsSuggestion = true,
                };
            }

            fake.ItemGroups.Clear();
            fake.ItemGroups = new ObservableCollection<CategoryViewModel>();
            this.Items.Add(fake);
        }
    }
}
