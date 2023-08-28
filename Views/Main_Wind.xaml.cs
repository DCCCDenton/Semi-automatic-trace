using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Collections;
using QuickGraph;
using Semi_automatic_trace.ViewModels;

namespace Semi_automatic_trace
{
    /// <summary>
    /// Interaction logic for Main_Wind.xaml
    /// </summary>
    public partial class Main_Wind : Window
    {

        public Main_Wind(SATViewModel satViewModel)
        {
            InitializeComponent();          
            DataContext = satViewModel;
        }

    }
}
