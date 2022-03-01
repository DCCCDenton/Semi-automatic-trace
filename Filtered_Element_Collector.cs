using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semi_automatic_trace
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    class Filtered_Element_Collector
    {     
        public ICollection<Element> GetElement(Document doc)
		{
			FilteredElementCollector Filtered_elm_elm = new FilteredElementCollector(doc);
			ICollection<Element> allElement = Filtered_elm_elm.OfCategory(BuiltInCategory.OST_CableTray).WhereElementIsNotElementType().ToElements();
			return allElement;
		}
    }
}
