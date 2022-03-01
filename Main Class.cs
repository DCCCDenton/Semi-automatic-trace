using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System.Reflection;

namespace Semi_automatic_trace
{
    public class CsAddPnBtn : IExternalApplication
    {
        // Both OnStartup and OnShutdown must be implemented as public method
        public Result OnStartup(UIControlledApplication application)
        {
            // Add a new ribbon panel
            RibbonPanel ribbonPanel = application.CreateRibbonPanel("Semi_automatic_trace");

            // Create a push button to trigger a command add it to the ribbon panel.
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;
            PushButtonData buttonData = new PushButtonData("cmdSemi Automatic Trace", "Automatic Trace", thisAssemblyPath, "Semi_automatic_trace.Semi__automatic_trace");

            PushButton pushButton = ribbonPanel.AddItem(buttonData) as PushButton;

            // Optionally, other properties may be assigned to the button
            // a) tool-tip
            pushButton.ToolTip = "Semi_automatic_trace";


            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            // nothing to clean up in this simple case
            return Result.Succeeded;
        }
    }

    [TransactionAttribute(TransactionMode.Manual)]  //активация транзакции
    [RegenerationAttribute(RegenerationOption.Manual)]// активация регенрации 
    public class Semi__automatic_trace : IExternalCommand
    {
        public Result Execute(ExternalCommandData externalCommand, ref string message, ElementSet elements)
        {
            Document doc = externalCommand.Application.ActiveUIDocument.Document;
            UIDocument uidoc = externalCommand.Application.ActiveUIDocument;
            Main_Wind userWind = new Main_Wind(doc, uidoc);
            userWind.ShowDialog();
            return Result.Succeeded;
        }
    }
}

