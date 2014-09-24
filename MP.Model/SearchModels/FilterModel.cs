using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP.Model.SearchModels
{
    public class FilterModel
    {
        public string field { get; set; }
        public string oparator { get; set; }
        public string value { get; set; }
    }
}
