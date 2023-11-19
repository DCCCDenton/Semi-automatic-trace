
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using QuickGraph;
using Semi_automatic_trace.Data;

namespace Semi_automatic_trace
{
    class MEPElements
    {
        private Element SelectionFirstElement(Document doc, UIDocument uidoc)
        {
            Selection selection = uidoc.Selection;
            ICollection<ElementId> allTray = uidoc.Selection.GetElementIds();
            Element tray = null;
            foreach (ElementId IDtray in allTray)
            {
                tray = doc.GetElement(IDtray);
            }
        
            return tray;
        }

        private ConnectorSet FindElementConnector(Element elm)
        {
            ConnectorSet connectors = new ConnectorSet();
            try
            {
                MEPCurve inst = elm as MEPCurve;
                connectors = inst.ConnectorManager.Connectors;
            }
            catch
            {
                FamilyInstance ins = elm as FamilyInstance;
                MEPModel i = ins.MEPModel;
                if (i != null && i.ConnectorManager != null)
                {
                    connectors = i.ConnectorManager.Connectors;
                }
            }

           
            return connectors;
        }

        private List<Element> FindConnectorOwner(ConnectorSet connectors)
        {
            List<Element> allOwner = new List<Element>();
            foreach (Connector con in connectors)
            {
                ConnectorSet allref = con.AllRefs;
                foreach (Connector reff in allref)
                {
                    Element e = reff.Owner;
                    allOwner.Add(e);
                }
            }
            return allOwner;
        }


        public BidirectionalGraph<object, IEdge<object>> BuildGraph(Document doc, Element element)
        {
            BidirectionalGraph<object, IEdge<object>> graph = new ();
            List<ElementId> closeElement = new ();
            ConnectorSet ConSet = new ();
            List<Element> allElementOfSystem = new ();
            Queue<Element> graphQueue = new ();
            ElementId elementId = null;
            Element baseElement = null;

            graphQueue.Enqueue(element);
            while (graphQueue.Count != 0)
            {
                Element elm = graphQueue.Dequeue();
                baseElement = elm;
                graph.AddVertex(baseElement);
                ConSet = null;
                allElementOfSystem = null;
                ConSet = FindElementConnector(elm);
                allElementOfSystem = FindConnectorOwner(ConSet);
                if (allElementOfSystem != null)
                {
                    foreach (Element e in allElementOfSystem)
                    {
                        if (!closeElement.Contains(e.Id))
                        {
                            graph.AddVertex(e);
                            Edge<object> edge = new (baseElement, e);
                            graph.AddEdge(edge);
                            graphQueue.Enqueue(e);
                            closeElement.Add(e.Id);
                        }
                    }
                }
            }
            return graph;
        }

        public List<object> graph_vertex(BidirectionalGraph<object, IEdge<object>> g)
        {
            List<object> vertex_list = new List<object>();
            vertex_list = g.Vertices.ToList();
            return vertex_list;
        }

        public List<ElectricalSystemElements> GetAllElementOfElectricalSystem(Document doc, Element element)
        {
            List<ElectricalSystemElements> listOfElementOfElectricalSystem = new();
            if (element.Category.BuiltInCategory == BuiltInCategory.OST_ElectricalEquipment)
            {
                FamilyInstance panelInstance = element as FamilyInstance;
                ISet<ElectricalSystem> electricalSystems = panelInstance.MEPModel.GetElectricalSystems();
                foreach (ElectricalSystem electricalSystem in electricalSystems)
                {
                    ElectricalSystemElements electricalSystemElements = new() { Panel = new ElectricalElement() { MainElement = element } };
                    foreach (Element elm in electricalSystem.Elements)
                    {
                        electricalSystemElements.FeedElectricalElements.Add(new ElectricalElement() { MainElement = elm });   
                    }
                    listOfElementOfElectricalSystem.Add(electricalSystemElements);  
                }
            }
            return listOfElementOfElectricalSystem;
        }
    }
}


 