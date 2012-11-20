using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.UI.Logic.DataModel
{
    class TrashImage
    {
        [SQLite.PrimaryKey]
        public string Name { get; set; }
    }
}
