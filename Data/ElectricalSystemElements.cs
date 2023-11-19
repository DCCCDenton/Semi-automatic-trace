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
        public List<ElectricalElement> FeedElectricalElements { get; set; } = new();
        public List<ElectricalElement> PowerSupplyElectricalElements { get; set; } = new();    

        public ElectricalElement Panel { get; set; }
    }
}
