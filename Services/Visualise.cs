using Autodesk.Revit.DB;
using Semi_automatic_trace.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Semi_automatic_trace.Services
{
    public class Visualise
    {
        public void VisualTraces(Document doc, List<ElectricalElement> elementInRoom)
        {
            
           
            foreach (ElectricalElement electricalElement in elementInRoom)
            {
                foreach (ElectricalElement electricalElement1 in electricalElement.OutElements)
                {
                    Transaction t = new Transaction(doc);
                    t.Start("Vis");
                    XYZ startPoint = (electricalElement.MainElement.Location as LocationPoint).Point;
                    XYZ endPoint = (electricalElement1.MainElement.Location as LocationPoint).Point;
                    Plane geomPlane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, new XYZ(0, 0, startPoint.Z));
                    SketchPlane sketch = SketchPlane.Create(doc, geomPlane);
                    Line line = Line.CreateBound(startPoint, endPoint);
                    DetailCurve detailCurve = doc.Create.NewDetailCurve(doc.ActiveView, line);
                    ModelLine modelLine = doc.Create.NewModelCurve(line, sketch) as ModelLine;
                    t.Commit();
                }
                foreach (ElectricalElement electricalElement2 in electricalElement.IncomingElement)
                {
                    Transaction t = new Transaction(doc);
                    t.Start("Vis");
                    XYZ startPoint = (electricalElement.MainElement.Location as LocationPoint).Point;
                    XYZ endPoint = (electricalElement2.MainElement.Location as LocationPoint).Point;
                    Plane geomPlane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, new XYZ(0, 0, startPoint.Z));
                    SketchPlane sketch = SketchPlane.Create(doc, geomPlane);
                    Line line = Line.CreateBound(startPoint, endPoint);
                    DetailCurve detailCurve = doc.Create.NewDetailCurve(doc.ActiveView, line);
                    ModelLine modelLine = doc.Create.NewModelCurve(line, sketch) as ModelLine;
                    t.Commit();
                }
            }
            
        }
    }
}
