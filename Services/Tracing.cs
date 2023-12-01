using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Semi_automatic_trace.Data;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Semi_automatic_trace.Services
{
    public class Tracing
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
            Element panelElement = (elementsInRoom.First().MainElement as FamilyInstance).MEPModel.GetElectricalSystems().First().BaseEquipment;
            ElectricalElement panel = new() { MainElement = panelElement };
            ElectricalElement firstElement = GetClosestElement(elementsInRoom, panel);

            //firstElement.IncomingElement.Add(allElectricalElements.First().Panel);

            GetNeibors(firstElement, elementsInRoom);
            bool completeRoom = false;
            foreach (ElectricalElement electricalElement in elementsInRoom)
            {
                if (electricalElement.IncomingElement.Count > 0 && electricalElement.OutElements.Count == 0)
                {
                    GetNeibors(electricalElement, elementsInRoom);
                }
            }
            int i = elementsInRoom.Where(x => x.IncomingElement.Count == 0).Count();
            DeleteDublicates(elementsInRoom);
            if (i > 0)
            {
                foreach (ElectricalElement electricalElement in elementsInRoom)
                {
                    if (electricalElement.IncomingElement.Count == 0)
                    {
                        ElectricalElement electricalElement1 = GetClosestElement(elementsInRoom, electricalElement);
                        electricalElement1.OutElements.Add(electricalElement);
                        electricalElement.IncomingElement.Add(electricalElement1);  
                    }
                }
            }
            var tmp = elementsInRoom;
            return elementsInRoom;
        }

        private void DeleteDublicates(List<ElectricalElement> elementsInRoom)
        {
            foreach (ElectricalElement electricalElement in elementsInRoom)
            {
                List<ElectricalElement> badInputElements = new();
                DeleteIncomngOutcomingDublicates(electricalElement);
                if (electricalElement.IncomingElement.Count > 1)
                {
                    ElectricalElement closesElement = GetClosestElement(electricalElement.IncomingElement, electricalElement);
                    badInputElements = electricalElement.IncomingElement.Where(x => x.MainElement.Id.IntegerValue != closesElement.MainElement.Id.IntegerValue).ToList();
                    electricalElement.IncomingElement.Clear();
                    electricalElement.IncomingElement.Add(closesElement);   
                }
                List<ElectricalElement> removeList = new();
                foreach (ElectricalElement badIncoming in badInputElements)
                {
                    badIncoming.OutElements.Remove(electricalElement);
                }
            }
        }

        private void DeleteIncomngOutcomingDublicates(ElectricalElement electricalElement)
        {
            foreach (ElectricalElement outElement in electricalElement.OutElements)
            {
                List<ElectricalElement> dublicateList = electricalElement.IncomingElement.Where(x => x.MainElement.Id.IntegerValue == outElement.MainElement.Id.IntegerValue).ToList();
                foreach (ElectricalElement dublicateElement in dublicateList)
                {
                    electricalElement.IncomingElement.Remove(dublicateElement); 
                }
            }
        }

        private void ClearTrace(List<ElectricalElement> elementsInRoom)
        {
            
            foreach (ElectricalElement electricalElement in elementsInRoom)
            {
                          
            }
        }

        private void GetNeibors(ElectricalElement electricalElement, List<ElectricalElement> elementsInRoom)
        {
            List<ElectricalElement> tmpNeibor = new();
            int i = 0; 
            while (i < 3)
            {
                ElectricalElement neiborElement = GetClosestElement(elementsInRoom, tmpNeibor, electricalElement);
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
                        if (GetClearNeibor(tmpNeibor, neiborElement, electricalElement))
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

        private bool GetClearNeibor(List<ElectricalElement> listOfNeiborElements, ElectricalElement neiborElement, ElectricalElement sourceElement)
        {
            bool clearNeibor = true;
            for (int i = 0; i < listOfNeiborElements.Count; i++)
            {
                double dist1 = DistanceBetweenElements(listOfNeiborElements[i], neiborElement);
                double dist2 = DistanceBetweenElements(sourceElement, listOfNeiborElements[i]);
                if (dist1 < dist2 || neiborElement.IncomingElement.Count > 0)
                {
                    clearNeibor = false;
                    break;
                }
            }
            return clearNeibor;
        }

        //private ElectricalElement GetClosestElement(List<ElectricalElement> listOfTracingElements, ElectricalElement sourceElement)
        //{
        //    ElectricalElement closestElement = null;            
        //    double minDistance = double.MaxValue;
        //    foreach (ElectricalElement element in listOfTracingElements)
        //    {
        //        if (sourceElement.MainElement.Id.IntegerValue != element.MainElement.Id.IntegerValue)
        //        {
        //            double distance = DistanceBetweenElements(sourceElement, element);
        //            if (distance < minDistance && element.IncomingElement.Count == 0 && element.OutElements.Count == 0)
        //            {
        //                minDistance = distance;
        //                closestElement = element;
        //            }
        //        }               
        //    }
        //    return closestElement;
        //}

        private ElectricalElement GetClosestElement(List<ElectricalElement> listOfTracingElements, List<ElectricalElement> tmpNeibor, ElectricalElement sourceElement)
        {
            ElectricalElement closestElement = null;
            double minDistance = double.MaxValue;
            foreach (ElectricalElement element in listOfTracingElements)
            {
                int contains = tmpNeibor.Where(x => x.MainElement.Id.IntegerValue == element.MainElement.Id.IntegerValue).Count();
                if (sourceElement.MainElement.Id.IntegerValue != element.MainElement.Id.IntegerValue && contains == 0)
                {
                    double distance = DistanceBetweenElements(sourceElement, element);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestElement = element;
                    }
                }
            }
            return closestElement;
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
                    if (distance < minDistance)
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
