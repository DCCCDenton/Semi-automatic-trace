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

        public List<ElectricalElement> TraceRoom(Room room)
        {
            List<ElectricalElement> listOfTracingElements = new();
            ElectricalSystemElements electricalSystemElements = new();
            List<ElectricalElement> elementsInRoom = electricalSystemElements.FeedElectricalElements.Where(e => (e.MainElement as FamilyInstance).Room.Id == room.Id).ToList();
            ElectricalElement firstElement = GetClosestElement(elementsInRoom, electricalSystemElements.Panel);
            elementsInRoom.Remove(firstElement);            
            listOfTracingElements.Add(firstElement);
            int i = 1;
            while (i < elementsInRoom.Count)
            {
                int j = 0;
                List<ElectricalElement> localTracingElements = new();
                while (j < 3)
                {
                    ElectricalElement closesElement = GetClosestElement(elementsInRoom, elementsInRoom[i]);
                    localTracingElements.Add(closesElement);
                    j++;
                }
                List<ElectricalElement> clearNeibor = GetClearNeiborList(localTracingElements, firstElement);

                elementsInRoom.Except(clearNeibor);
                firstElement = elementsInRoom.First();
                i++;               
            }
            return listOfTracingElements;
        }

        private List<ElectricalElement> Find4Neibor(Room room, List<ElectricalElement> elementInRoom, ElectricalElement sourceElement)
        {
            List<ElectricalElement> listOfNeiborElements = new();
            List<ElectricalElement> sourceList = listOfNeiborElements;
            sourceList.Remove(sourceElement);
            int neiborsCount = 4;
            if (elementInRoom.Count < 4)
            {
                neiborsCount = elementInRoom.Count;
            }
            int i = 0;
            while (i < neiborsCount)
            {
                ElectricalElement neibor = GetClosestElement(sourceList, sourceElement);
                listOfNeiborElements.Add(neibor);
                sourceList.Remove(neibor);
            }
            return listOfNeiborElements;
        }

        private List<ElectricalElement> GetClearNeiborList(List<ElectricalElement> listOfNeiborElements, ElectricalElement sourceElement)
        {
            List<ElectricalElement> removeList = new();

            for (int i = 0; i < listOfNeiborElements.Count-1; i++)
            {
                for (int j = i + 1; j < listOfNeiborElements.Count; j++)
                {
                    if (DistanceBetweenElements(listOfNeiborElements[i], listOfNeiborElements[j]) < DistanceBetweenElements(sourceElement, listOfNeiborElements[i]))
                    {
                        if (DistanceBetweenElements(sourceElement, listOfNeiborElements[i]) > DistanceBetweenElements(sourceElement, listOfNeiborElements[j]))
                        {
                            removeList.Add(listOfNeiborElements[i]);
                        }
                        else
                        {
                            removeList.Add(listOfNeiborElements[j]);
                        }                        
                    }
                }
            }
            List<ElectricalElement> clearNeiborList = listOfNeiborElements.Except(removeList).ToList();
            return clearNeiborList;
        }

        private ElectricalElement GetClosestElement(List<ElectricalElement> listOfTracingElements, ElectricalElement sourceElement)
        {
            ElectricalElement closestElement = null;            
            double minDistance = double.MaxValue;
            foreach (ElectricalElement element in listOfTracingElements)
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

        private double DistanceBetweenElements (ElectricalElement element1, ElectricalElement element2)
        {
            double distance = 0;
            XYZ locationElement1 = ((element1.MainElement.Location) as LocationPoint).Point;
            XYZ locationElement2 = ((element2.MainElement.Location) as LocationPoint).Point;
            distance = locationElement1.DistanceTo(locationElement2);
            return distance;
        }
    }
}
