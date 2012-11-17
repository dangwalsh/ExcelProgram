using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ExcelProgram
{
    public class ProjSet
    {
        private UIApplication _UIApp;
        private UIDocument _UIDoc;
        private Document _Doc;

        public ProjSet(ExternalCommandData c)
        {
            _UIApp = c.Application;
            _UIDoc = _UIApp.ActiveUIDocument;
            _Doc = _UIDoc.Document;
        }

        public Document Doc
        {
            get { return _Doc; }
        }

        public UIApplication UIApp
        {
            get { return _UIApp; }
        }
    }
}
