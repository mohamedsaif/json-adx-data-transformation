using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTransformer.Models
{
    public class TransformedRow
    {
        public string From { get; set; }
        public string To { get; set; }
        
        //Place
        public string PlaceId { get; set; }
        public string PlaceName { get; set; }
        public string PlaceType { get; set; }

        //Kpi
        public string KpiParentId { get; set; }
        public string KpiParentName { get; set; }
        public string KpiParentType { get; set; }
        public string KpiId { get; set; }
        public string KpiName { get; set; }
        public string KpiType { get; set; }
        public string KpiUnit { get; set; }
        public string KpiValue { get; set; }
        public string? KpiDataType { get; internal set; }
    }
}
