using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Windows;
using Semi_automatic_trace.ViewModels;

namespace Semi_automatic_trace
{
    //public class CsAddPnBtn : IExternalApplication
    //{
    //    // Both OnStartup and OnShutdown must be implemented as public method
    //    public Result OnStartup(UIControlledApplication application)
    //    {
    //        // Add a new ribbon panel
    //        RibbonPanel ribbonPanel = application.CreateRibbonPanel("Semi_automatic_trace");

    //        // Create a push button to trigger a command add it to the ribbon panel.
    //        string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;
    //        PushButtonData buttonData = new PushButtonData("cmdSemi Automatic Trace", "Automatic Trace", thisAssemblyPath, "Semi_automatic_trace.Semi__automatic_trace");

    //        PushButton pushButton = ribbonPanel.AddItem(buttonData) as PushButton;

    //        // Optionally, other properties may be assigned to the button
    //        // a) tool-tip
    //        pushButton.ToolTip = "Semi_automatic_trace";


    //        return Result.Succeeded;
    //    }

    //    public Result OnShutdown(UIControlledApplication application)
    //    {
    //        return Result.Succeeded;
    //    }
    //}

    [TransactionAttribute(TransactionMode.Manual)]  //активация транзакции
    [RegenerationAttribute(RegenerationOption.Manual)]// активация регенрации 
    public class Semi__automatic_trace : IExternalCommand
    {
        Window userWind;

        public Result Execute(ExternalCommandData externalCommand, ref string message, ElementSet elements)
        {
            try
            {
                UIDocument uidoc = externalCommand.Application.ActiveUIDocument;
                Document doc = externalCommand.Application.ActiveUIDocument.Document;

                Selection selectionService = uidoc.Selection;

                SATViewModel satViewModel = new (doc, uidoc, selectionService);
                userWind = new Main_Wind(satViewModel);

                if (userWind.ShowDialog() != true)
                    return Result.Cancelled;

                return Result.Succeeded;
            }
            catch (System.Exception ex)
            {
 
                return Result.Failed;
            }
        }
    }
}

