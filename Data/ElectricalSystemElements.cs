using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semi_automatic_trace.Data
{
    internal class ElectricalSystemElements
    {
        public List<Element> FeedElectricalElements { get; set; } = new();
        public List<Element> PowerSupplyElectricalElements { get; set; } = new();    

        public Element Panel { get; set; }
    }
}
