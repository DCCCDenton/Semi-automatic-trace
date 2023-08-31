using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using QuickGraph;
using Semi_automatic_trace.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Semi_automatic_trace.ViewModels
{
    public class SATViewModel : INotifyPropertyChanged
    {          
        public Document _doc { get; set; } 
        public UIDocument _uidoc { get; set; }

        public Selection _selectionService { get; set; }

        public Element Reference { get; set; }

        public List<object> Vertex { get; set; } 

        public BidirectionalGraph<object, IEdge<object>> Graph { get; set; }

        private RelayCommand selectTrace; 
        public RelayCommand SelectTrace
        {
            get
            {
                return selectTrace ??
                    (selectTrace = new RelayCommand(obj =>
                    {                       
                        if (!(obj is Window window))
                            return;

                        window.Hide();
                        try
                        {
                            Reference = _doc.GetElement(_selectionService.PickObject(ObjectType.Element).ElementId);
                        }
                        catch (OperationCanceledException) { }
                        window.ShowDialog();

                    }));

            }
        }

        private RelayCommand getElectricalSystem;
        public RelayCommand GetElectricalSystem
        {
            get
            {
                return getElectricalSystem ??
                    (getElectricalSystem = new RelayCommand(obj =>
                    {
                        MEPElements mepElements = new();
                        var tmp = mepElements.GetAllElementOfElectricalSystem(_doc, Reference);
                    }));
            }
        }

        private RelayCommand bGraph; 
        public RelayCommand BGraph
        {
            get
            {
                return bGraph ??
                    (bGraph = new RelayCommand(obj =>
                    {
                        MEPElements mepElements = new();
                        BidirectionalGraph<object, IEdge<object>> Graph = mepElements.BuildGraph(_doc, Reference);
                        Vertex = mepElements.graph_vertex(Graph);
                    }));
            }
        }

        public SATViewModel(Document doc, UIDocument uidoc, Selection selectionService)
        {
            _doc = doc;
            _uidoc = uidoc;
            _selectionService = selectionService;
            Vertex = new();
            BidirectionalGraph<object, IEdge<object>> Graph = new();
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
