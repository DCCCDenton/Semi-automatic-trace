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

        public List<ElectricalElement> TraceRoom(Room room, ElectricalSystemElements electricalSystemElements)
        {
            List<ElectricalElement> listOfTracingElements = new();
            List<Element> elementInRoom = electricalSystemElements.FeedElectricalElements.Where(e => (e as FamilyInstance).Room.Id == room.Id).ToList();
            Element firstElement = GetClosestElement(elementInRoom, electricalSystemElements.Panel);
            ElectricalElement firestElectricalElement = new() { MainElement = firstElement };
            listOfTracingElements.Add(firestElectricalElement);
            int i = 1;
            while (i < elementInRoom.Count)
            {
                Element closesElement = GetClosestElement(elementInRoom, elementInRoom[i]);
                elementInRoom.Remove(closesElement);
                List<Element> untraceElement = elementInRoom;
                for (int j = 0; j < 3; j++)
                {

                }
            }
            return listOfTracingElements;
        }

        private List<Element> Find4Neibor(Room room, List<Element> elementInRoom, Element sourceElement)
        {
            List<Element> listOfNeiborElements = new();
            List<Element> sourceList = listOfNeiborElements;
            sourceList.Remove(sourceElement);
            int neiborsCount = 4;
            if (elementInRoom.Count < 4)
            {
                neiborsCount = elementInRoom.Count;
            }
            int i = 0;
            while (i < neiborsCount)
            {
                Element neibor = GetClosestElement(sourceList, sourceElement);
                listOfNeiborElements.Add(neibor);
                sourceList.Remove(neibor);
            }
            return listOfNeiborElements;
        }

        private List<Element> GetClearNeiborList(List<Element> listOfNeiborElements, Element sourceElement)
        {
            List<Element> removeList = new();
            for (int i = 0; i < listOfNeiborElements.Count-1; i++)
            {
                for (int j = i + 1; j < listOfNeiborElements.Count; j++)
                {
                    if (DistanceBetweenElements(listOfNeiborElements[i], listOfNeiborElements[j]) < DistanceBetweenElements(sourceElement, listOfNeiborElements[j]))
                    {
                        removeList.Add(listOfNeiborElements[j]);
                    }
                }
            }
            List<Element> clearNeiborList = RemoveElement(listOfNeiborElements, removeList)
            return clearNeiborList;
        }

        private List<Element> RemoveElement(List<Element> sourceList, List<Element> removeList)
        {
            foreach (Element element in removeList)
            {
                sourceList.Remove(element);
            }
            return sourceList;
        }
        private Element GetClosestElement(List<Element> elementInRoom, Element sourceElement)
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
