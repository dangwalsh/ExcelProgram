using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.Architecture;

namespace ExcelProgram
{
    public class MassFactory
    {
        private Document _ProjectDoc;
        private string _Name;
        private int _Count;
        private double _Side;
        private static double _X = 0.0;
        private double _Y;
        private XYZ[] _Vertices = new XYZ[4];
        private const double _Height = 10.0;
        private const string _TemplateFileName = @"L:\4_Revit\Imperial Templates\Mass.rft";
        private const string _TempLocation = @"C:\masstmp";
        private const int _Padding = 4;
        private bool _IsDefaultArea;
        private bool _IsDefaultCount;

        public MassFactory(Document projectDoc, Shape shape)
        {
            _ProjectDoc = projectDoc;
            _Name = shape.Name;
            _Count = shape.Count;
            _Side = shape.Side;
            _Y = _Side + FormFile.yPad;
            _IsDefaultArea = shape.IsDefaultArea;
            _IsDefaultCount = shape.IsDefaultCount;

            for (int i = 0; i < 4; ++i)
            {
                _Vertices[i] = new XYZ(shape.Points[i].X, shape.Points[i].Y, 0.0);
            }

            if (!Directory.Exists(_TempLocation)) Directory.CreateDirectory(_TempLocation);
        }

        ~MassFactory()
        {
            try
            {
                Directory.Delete(_TempLocation, true);
            }
            catch (Exception ex)
            {
                //TaskDialog.Show("Cleanup Error", ex.Message);
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

                if (!familyDoc.SaveAs(_TempLocation + @"\" + _Name + @".rfa"))
                {
                    throw new Exception("Failed to save family document!");
                }
                
                FamilySymbolSetIterator symbolItor = null;
                Transaction transLoad = new Transaction(familyDoc, "Load Family");
                if (transLoad.Start() == TransactionStatus.Started)
                {
                    FamilyLoadOpt flo = new FamilyLoadOpt();
                    Family family = familyDoc.LoadFamily(_ProjectDoc, flo);
                    symbolItor = family.Symbols.ForwardIterator();
                    transLoad.Commit();
                }
                
                Transaction transPlace = new Transaction(_ProjectDoc, "PlaceFamily");
                if (transPlace.Start() == TransactionStatus.Started)
                {
                    double y = 0.0;

                    symbolItor.MoveNext();
                    while (_Count > 0)
                    {
                        FamilySymbol symbol = symbolItor.Current as FamilySymbol;
                        XYZ location = new XYZ(_X, y, 0.0);
                        FamilyInstance instance = _ProjectDoc.Create.NewFamilyInstance(location, symbol, StructuralType.NonStructural);
                        y += _Y;
                        --_Count;
                    }
                    transPlace.Commit();
                }
                familyDoc.Close(false);

                //File.Delete(_TempLocation + @"\" + _Name + @".rfa");

                _X += _Side + FormFile.xPad;
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
            try
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
                AssignSubCategory(familyDoc, extrusion);

                return extrusion;
            }

            catch (Exception ex)
            {
                TaskDialog.Show("Extrusion Error", ex.Message);

                return null;
            }
        }

        private ModelCurve MakeLine(Document familyDoc, XYZ ptA, XYZ ptB)
        {
            try
            {
                Application app = familyDoc.Application;

                Line line = app.Create.NewLine(ptA, ptB, true);
                XYZ norm = ptA.CrossProduct(ptB);
                if (norm.IsZeroLength()) norm = XYZ.BasisZ;
                Plane plane = app.Create.NewPlane(norm, ptB);
                SketchPlane skplane = familyDoc.FamilyCreate.NewSketchPlane(plane);

                return familyDoc.FamilyCreate.NewModelCurve(line, skplane);
            }

            catch (Exception ex)
            {
                TaskDialog.Show("Line Error", ex.Message);

                return null;
            }
        }

        private void AssignSubCategory(Document familyDoc, GenericForm extrusion)
        {
            try
            {
                Category cat = familyDoc.OwnerFamily.FamilyCategory;
                Category subCat = familyDoc.Settings.Categories.NewSubcategory(cat, _Name);
                subCat.Material = CreateMaterial(familyDoc);
                extrusion.Subcategory = subCat;
            }

            catch (Exception ex)
            {
                TaskDialog.Show("SubCategory Error", ex.Message);
            }
        }

        private Material CreateMaterial(Document familyDoc)
        {
            try
            {
                if (_IsDefaultArea)
                {
                    ElementId matid = Material.Create(familyDoc, _Name);
                    Material mat = familyDoc.GetElement(matid) as Material;
                    Color col = new Color(255,
                                          0,
                                          0);
                    mat.Color = col;
                    mat.Transparency = 0;

                    return mat;
                }
                else if (_IsDefaultCount)
                {
                    ElementId matid = Material.Create(familyDoc, _Name);
                    Material mat = familyDoc.GetElement(matid) as Material;
                    Color col = new Color(0,
                                          0,
                                          255);
                    mat.Color = col;
                    mat.Transparency = 0;

                    return mat;
                }
                else
                {
                    Random r = new Random();
                    ElementId matid = Material.Create(familyDoc, _Name);
                    Material mat = familyDoc.GetElement(matid) as Material;
                    Color col = new Color((byte)r.Next(1, 255),
                                          (byte)r.Next(1, 255),
                                          (byte)r.Next(1, 255));
                    mat.Color = col;
                    mat.Transparency = 50;

                    return mat;
                }
            }

            catch (Exception ex)
            {
                TaskDialog.Show("Material Error", ex.Message);

                return null;
            }
        }
    }
}
