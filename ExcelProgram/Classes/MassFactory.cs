using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;

namespace ExcelProgram
{
    public class MassFactory
    {
        private Document _ProjectDoc;
        private string _Name;
        private XYZ[] _Vertices = new XYZ[4];
        private const double _Height = 10.0;
        private const string _TemplateFileName = @"L:\4_Revit\Imperial Templates\Mass.rft";

        public MassFactory(Document projectDoc, Shape shape)
        {
            _ProjectDoc = projectDoc;
            _Name = shape.Name;

            for (int i = 0; i < 4; ++i)
            {
                _Vertices[i] = new XYZ(shape.Points[i].X, shape.Points[i].Y, 0.0);
            }
        }

        public void Make()
        {
            try
            {
                Document familyDoc = _ProjectDoc.Application.NewFamilyDocument(_TemplateFileName);
                if (null == familyDoc)
                {
                    throw new Exception("Failed to open the family document");
                }
                CreateFamily(familyDoc);
                FamilySymbolSetIterator symbolItor = null;
                Transaction transLoad = new Transaction(familyDoc, "Load Family");
                if (transLoad.Start() == TransactionStatus.Started)
                {
                    Family family = familyDoc.LoadFamily(_ProjectDoc);
                    symbolItor = family.Symbols.ForwardIterator();
                }
                transLoad.Commit();
                familyDoc.Close(false);
            }

            catch (Exception ex)
            {
                TaskDialog.Show("Load Error", ex.Message);
            }
        }

        private void CreateFamily(Document familyDoc)
        {
            try
            {
                Transaction trans = new Transaction(familyDoc, "Create Family");

                if (trans.Start() == TransactionStatus.Started)
                {
                    Form form = CreateExtrusionForm(familyDoc);
                    trans.Commit();
                }
            }

            catch (Exception ex)
            {
                TaskDialog.Show("Create Error", ex.Message);
            }
        }

        private Form CreateExtrusionForm(Document familyDoc)
        {
            ReferenceArray ra = new ReferenceArray();
            ModelCurve mc;
            int i = 0;
            while (i < 3)
            {
                mc = MakeLine(familyDoc, _Vertices[i], _Vertices[i + 1]);
                ra.Append(mc.GeometryCurve.Reference);
                ++i;
            }
            mc = MakeLine(familyDoc, _Vertices[i], _Vertices[i - i]);
            ra.Append(mc.GeometryCurve.Reference);
            XYZ dir = new XYZ(0, 0, _Height);
            Form extrusion = familyDoc.FamilyCreate.NewExtrusionForm(true, ra, dir);

            return extrusion;
        }

        private ModelCurve MakeLine(Document familyDoc, XYZ ptA, XYZ ptB)
        {
            Application app = familyDoc.Application;

            Line line = app.Create.NewLine(ptA, ptB, true);
            XYZ norm = ptA.CrossProduct(ptB);
            if (norm.IsZeroLength()) norm = XYZ.BasisZ;
            Plane plane = app.Create.NewPlane(norm, ptB);
            SketchPlane skplane = familyDoc.FamilyCreate.NewSketchPlane(plane);

            return familyDoc.FamilyCreate.NewModelCurve(line, skplane);
        }

    }
}
