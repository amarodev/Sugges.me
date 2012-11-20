using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generic.UI.Logic.Enumerations;
using Generic.UI.Logic.Models;
using Windows.ApplicationModel.Resources;

namespace Generic.UI.Logic.ViewModels
{
    public class GroupViewModel: ItemViewModel
    {
        private IModel model;
        private static ResourceLoader loader = new ResourceLoader();

        public GroupViewModel()
        {
            model = new DatabaseModel();
        }

        private ObservableCollection<ParentViewModel> _items = new ObservableCollection<ParentViewModel>();
        public ObservableCollection<ParentViewModel> Items
        {
            set { _items = value; }
            get { return this._items; }
        }

        public void CreateFake()
        {
            ParentViewModel fake;

            ParentViewModel fakeTrip = this.Items.FirstOrDefault(p => p.Identifier == -1);
            if (fakeTrip != null)
                this.Items.Remove(fakeTrip);

            fake = new ParentViewModel()
            {
                Identifier = -1,
                Title = "New parent?",
                Description = "Click here to add a parent",
                LocalPathImage = loader.GetString("NewParentImage")
            };

            fake.ItemGroups.Clear();
            fake.ItemGroups = new ObservableCollection<CategoryViewModel>();
            this.Items.Add(fake);
        }
    }
}
