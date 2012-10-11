using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sugges.UI.Logic.DataModel
{
    class Item
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Identifier { get; set; }
        [SQLite.MaxLength(100)]
        public string Title { get; set; }
        [SQLite.MaxLength(500)]
        public string Description { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? Cost { get; set; }
        public long Parent { get; set; }
        public short Category { get; set; }
        [SQLite.MaxLength(500)]
        public string Image { get; set; }
        public string Traveler { get; set; }
        public bool IsSuggestion { get; set; }
    }
}
