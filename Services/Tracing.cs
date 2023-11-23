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

        public List<ElectricalElement> TraceRoom(Document doc, Room room)
        {
            MEPElements mepElements = new();
            List<ElectricalElement>  listOfTracingElements = new();
            List<ElectricalSystemElements> allElectricalElements = mepElements.GetAllElementOfElectricalSystem(doc, room);
            ElectricalSystemElements electricalSystemElements = new();
            List<ElectricalElement> elementsInRoom = new();
            foreach (ElectricalSystemElements systemElement in allElectricalElements)
            {
                elementsInRoom = systemElement.FeedElectricalElements.Where(e => (e.MainElement as FamilyInstance).Room.Id == room.Id).ToList();
                if (elementsInRoom.Count > 0)
                {
                    break;
                }
            }
            ElectricalElement firstElement = GetClosestElement(elementsInRoom, allElectricalElements.First().Panel);
            firstElement.IncomingElement.Add(allElectricalElements.First().Panel);

            GetNeibors(firstElement, elementsInRoom);
            foreach (ElectricalElement electricalElement in elementsInRoom)
            {
                if (electricalElement.IncomingElement.Count > 0 && electricalElement.OutElements.Count == 0)
                {
                    GetNeibors(electricalElement, elementsInRoom);
                }
                
            }          
            return elementsInRoom;
        }

        private void GetNeibors(ElectricalElement electricalElement, List<ElectricalElement> elementsInRoom)
        {
            List<ElectricalElement> tmpNeibor = new();
            int i = 0; 
            while (i < 3)
            {
                ElectricalElement neiborElement = GetClosestElement(elementsInRoom, electricalElement);
                if (neiborElement != null)
                {
                    if (tmpNeibor.Count == 0)
                    {
                        electricalElement.OutElements.Add(neiborElement);
                        neiborElement.IncomingElement.Add(electricalElement);
                        tmpNeibor.Add(neiborElement);
                    }
                    else
                    {
                        if (GetClearNeibor(tmpNeibor, neiborElement))
                        {
                            electricalElement.OutElements.Add(neiborElement);
                            neiborElement.IncomingElement.Add(electricalElement);
                            tmpNeibor.Add(neiborElement);
                        }
                    }                   
                }
                i++;
            }
        }



        //private List<ElectricalElement> Find4Neibor(Room room, List<ElectricalElement> elementInRoom, ElectricalElement sourceElement)
        //{
        //    List<ElectricalElement> listOfNeiborElements = new();
        //    List<ElectricalElement> sourceList = listOfNeiborElements;
        //    sourceList.Remove(sourceElement);
        //    int neiborsCount = 4;
        //    if (elementInRoom.Count < 4)
        //    {
        //        neiborsCount = elementInRoom.Count;
        //    }
        //    int i = 0;
        //    while (i < neiborsCount)
        //    {
        //        ElectricalElement neibor = GetClosestElement(sourceList, sourceElement);
        //        listOfNeiborElements.Add(neibor);
        //        sourceList.Remove(neibor);
        //    }
        //    return listOfNeiborElements;
        //}

        private bool GetClearNeibor(List<ElectricalElement> listOfNeiborElements, ElectricalElement sourceElement)
        {
            bool clearNeibor = true;
            for (int i = 0; i < listOfNeiborElements.Count - 1; i++)
            {
                if (listOfNeiborElements.Count < 2)
                {
                    if (listOfNeiborElements[i].IncomingElement.Count > 0)
                    {
                        clearNeibor = false;
                        break;
                    }
                }
                for (int j = i + 1; j < listOfNeiborElements.Count; j++)
                {
                    double dist1 = DistanceBetweenElements(listOfNeiborElements[i], listOfNeiborElements[j]);
                    double dist2 = DistanceBetweenElements(sourceElement, listOfNeiborElements[i]);

                    if (dist1 < dist2 || listOfNeiborElements[i].IncomingElement.Count > 0)
                    {
                        clearNeibor = false;
                        break;
                    }
                }
            }
            return clearNeibor;
        }

        private ElectricalElement GetClosestElement(List<ElectricalElement> listOfTracingElements, ElectricalElement sourceElement)
        {
            ElectricalElement closestElement = null;            
            double minDistance = double.MaxValue;
            foreach (ElectricalElement element in listOfTracingElements)
            {
                if (sourceElement.MainElement.Id.IntegerValue != element.MainElement.Id.IntegerValue)
                {
                    double distance = DistanceBetweenElements(sourceElement, element);
                    if (distance < minDistance && element.IncomingElement.Count == 0 && element.OutElements.Count == 0)
                    {
                        minDistance = distance;
                        closestElement = element;
                    }
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
