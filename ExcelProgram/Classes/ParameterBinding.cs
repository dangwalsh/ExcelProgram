using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;


namespace ExcelProgram
{
    class ParameterBinding
    {
        const string _sharedFilePath = @"L:\4_Revit\SharedPARAM.txt";
        bool _doesExist = false;
        string _groupName = "";
        string _paramName = "";
        DefinitionFile _sharedFile;
        FamilyManager _manager = null;
        Document _doc;
        Application _app;

        public ParameterBinding(Document doc, string groupName, string paramName)
        {
            _doc = doc;
            _app = doc.Application;
            _manager = doc.FamilyManager;
            _groupName = groupName;
            _paramName = paramName;

            if (LoadSharedParameterFromFile(out _doesExist) && _doesExist)
            {
                bool result = AddSharedParameter();
            }
        }
        /*
        private DefinitionFile SetAndOpenExternalSharedParamFile(Application app, string sharedParamFile)
        {
            // set the path of shared parameter file to current Revit
            app.SharedParametersFilename = sharedParamFile;
            // open the file
            return app.OpenSharedParameterFile();
        }
        
        public bool SetNewParameterToTypeGenMod()
        {
            // Create a new group in the shared parameters file
            DefinitionGroups groups = this._defFile.Groups;
            DefinitionGroup group = groups.get_Item("Area");
            if (null == group) return false;
            // get the definitions in that group
            Definitions definitions = group.Definitions;
            // get the definition named Program Area
            ExternalDefinition extDef = definitions.get_Item("Program Area") as ExternalDefinition;
            if (null == extDef) return false;
            
            // Create a category set and insert category of wall to it
            CategorySet categories = this._uiapp.Application.Create.NewCategorySet();
            // Use BuiltInCategory to get category of wall
            Category category = this._uiapp.ActiveUIDocument.Document.Settings.Categories.get_Item(BuiltInCategory.OST_GenericModel);

            categories.Insert(category);
            //Create an object of TypeBinding according to the Categories
            InstanceBinding instanceBinding = this._uiapp.Application.Create.NewInstanceBinding(categories);
            // Get the BingdingMap of current document.
            BindingMap bindingMap = this._uiapp.ActiveUIDocument.Document.ParameterBindings;
            // Bind the definitions to the document
            bool typeBindOK = bindingMap.Insert(extDef, instanceBinding, BuiltInParameterGroup.PG_AREA);

            return typeBindOK;
        }
        */
        private bool LoadSharedParameterFromFile(out bool exist)
        {
            exist = true;

            // verify that the file exists before continuing
            if (!File.Exists(_sharedFilePath))
            {
                exist = false;
                return true;
            }

            // if the file exists make it the current shared parameter file
            _app.SharedParametersFilename = _sharedFilePath;

            try
            {
                // access the shared parameter file
                _sharedFile = _app.OpenSharedParameterFile();
            }
            catch (System.Exception e)
            {
                TaskDialog.Show("Error", e.Message);
                return false;
            }

            return true;
        }

        private bool AddSharedParameter()
        {
            // check to make sure that the file still exists and is valid
            if (File.Exists(_sharedFilePath) && null == _sharedFile)
            {
                TaskDialog.Show("Error", "SharedPARAM.txt has an invalid format.");
                return false;
            }

            // get the Areas group from shared param file
            DefinitionGroup group = _sharedFile.Groups.get_Item(_groupName);
            if (null == group) return false;

            // get the definition named Program Area
            ExternalDefinition def = group.Definitions.get_Item(_paramName) as ExternalDefinition;
            if (null == def) return false;

            // check whether the parameter already exists in the document
            FamilyParameter param = _manager.get_Parameter(def.Name);
            if (null != param) return false;   
                
            try
            {
                // add that parameter to the current family document
                _manager.AddParameter(def, def.ParameterGroup, true);
            }
            catch (System.Exception e)
            {
                //MessageManager.MessageBuff.AppendLine(e.Message);
                return false;
            }

            return true;
        }
    }
}
