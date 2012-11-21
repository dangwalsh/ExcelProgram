using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ExcelProgram
{
    /*
     * modify these methods to handle overwriting families
     * that are already defined in the target project document\
     * */
    public class FamilyLoadOpt:IFamilyLoadOptions
    {
        public bool OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
        {
            overwriteParameterValues = true;        // do you want to replace the param values too?
            return true;
        }

        public bool OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
        {
            source = FamilySource.Family;           // could implement a test to decided whether to use the source or target family
            overwriteParameterValues = true;        // do you want to replace the param values too?
            return true;
        }
    }
}
