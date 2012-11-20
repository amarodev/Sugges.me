using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.UI.Logic.ViewModels
{
    public class CategoryViewModel : ItemViewModel
    {
        private ObservableCollection<ItemViewModel> _items = new ObservableCollection<ItemViewModel>();
        public ObservableCollection<ItemViewModel> Items
        {
            set { _items = value; }
            get { return this._items; }
        }
    }
}
