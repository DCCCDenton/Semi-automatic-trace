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

        public List<Element> TraceRoom(Room room, ElectricalSystemElements electricalSystemElements)
        {
            List<Element> listOfTracingElements = new();
            List<Element> elementInRoom = electricalSystemElements.FeedElectricalElements.Where(e => (e as FamilyInstance).Room.Id == room.Id).ToList();
            Element firstElement = GetClosestElements(room, elementInRoom, electricalSystemElements.Panel);
            listOfTracingElements.Add(firstElement);
            List<Element> removeList = new();
            removeList.Add(firstElement);   
            foreach (Element element in elementInRoom)
            {
                List<Element> newList = RemoveElement(elementInRoom, removeList);
                foreach (Element elm in newList)
                {
                    Element closestElement = GetClosestElements(room, elementInRoom, firstElement);
                    firstElement = closestElement;
                    removeList.Add(firstElement); 
                }               
            }
            return listOfTracingElements;
        }

        private List<Element> RemoveElement(List<Element> sourceList, List<Element> removeList)
        {
            foreach (Element element in removeList)
            {
                sourceList.Remove(element);
            }
            return sourceList;
        }
        private Element GetClosestElements(Room room, List<Element> elementInRoom, Element sourceElement)
        {
            Element closestElement = null;            
            double minDistance = double.MaxValue;
            foreach (Element element in elementInRoom)
            {
                double distance = DistanceBetweenElements(sourceElement, element);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestElement = element;
                }
            }
            return closestElement;
        }

        private double DistanceBetweenElements (Element element1, Element element2)
        {
            double distance = 0;
            XYZ locationElement1 = ((element1.Location) as LocationPoint).Point;
            XYZ locationElement2 = ((element2.Location) as LocationPoint).Point;
            distance = locationElement1.DistanceTo(locationElement2);
            return distance;
        }
    }
}
