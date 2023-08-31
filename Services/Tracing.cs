using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Semi_automatic_trace.Data;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semi_automatic_trace.Services
{
    internal class Tracing
    {
        public Element GetClosestElementInRoom(Room room, ElectricalSystemElements electricalSystemElements)
        {
            Element closestElement = null;
            List<Element> elementInRoom = electricalSystemElements.FeedElectricalElements.Where(e => (e as FamilyInstance).Room.Id == room.Id).ToList();
            foreach (Element element in elementInRoom)
            {
                element.
            }
            return closestElement;
        }

        private double DistanceBetweenElements (Element element1, Element element2)
        {
            double distance = 0;
            XYZ locationElement1 = ((element1.Location) as LocationPoint).Point;
            XYZ locationElement2 = ((element2.Location) as LocationPoint).Point;
            distance locationElement1.DistanceTo(locationElement2);
            return distance;
        }
    }
}
