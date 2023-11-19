﻿using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semi_automatic_trace.Data
{
    internal class ElectricalElement
    {
        public Element MainElement { get; set; }
        public List<ElectricalElement> IncomingElement { get; set; }
        public List<ElectricalElement> OutElements { get; set; }  = new List<ElectricalElement>();
    }
}
