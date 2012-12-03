using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;

namespace ExcelProgram
{
    public class ModelFactory
    {
        private const string _template = @"L:\4_Revit\Imperial Templates\Generic Model.rft";
        private const string _tempfile = @"C:\famtemp";
        private Document _projectDoc;
        private string _name;
        private double _area;
        private XYZ[] _vert = new XYZ[4];
        private int _count;
        private double _side;
        private static double _X = 0.0;
        private double _Y;
        private const int _padding = 4;
        private bool _isDefaultArea;
        private bool _isDefaultCount;
        private ReferenceArray _width;
        private ReferenceArray _height;
        private ReferenceArray _leftCon;
        private ReferenceArray _botCon;
        private ReferenceArray _rightCon;
        private ReferenceArray _topCon;
        FamilyType _type;
        View _view;

        public ModelFactory(Document projectDoc, Shape shape)
        {
            _projectDoc = projectDoc;
            _name = shape.Name;
            _count = shape.Count;
            _side = shape.Side;
            _Y = _side + FormFile.yPad;
            _isDefaultArea = shape.IsDefaultArea;
            _isDefaultCount = shape.IsDefaultCount;

            for (int i = 0; i < 4; ++i)
            {
                _vert[i] = new XYZ(shape.Points[i].X, shape.Points[i].Y, 0.0); 
            }

            _area = Math.Pow(_side, 2);

            if (!Directory.Exists(_tempfile)) Directory.CreateDirectory(_tempfile);
        }

        ~ModelFactory()
        {
            try
            {
                Directory.Delete(_tempfile, true);
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
                Document familyDoc = _projectDoc.Application.NewFamilyDocument(_template);
                if (null == familyDoc)
                {
                    throw new Exception("Failed to open the family document");
                }
                SetView(familyDoc);
                BuildFamily(familyDoc);

                if (!familyDoc.SaveAs(_tempfile + @"\" + _name + @".rfa"))
                {
                    throw new Exception("Failed to save family document!");
                }

                FamilySymbolSetIterator symbolItor = null;
                Transaction transLoad = new Transaction(familyDoc, "Load Family");
                if (transLoad.Start() == TransactionStatus.Started)
                {
                    FamilyLoadOpt flo = new FamilyLoadOpt();
                    Family family = familyDoc.LoadFamily(_projectDoc, flo);
                    symbolItor = family.Symbols.ForwardIterator();
                    transLoad.Commit();
                }

                Transaction transPlace = new Transaction(_projectDoc, "PlaceFamily");
                if (transPlace.Start() == TransactionStatus.Started)
                {
                    double y = 0.0;

                    symbolItor.MoveNext();
                    while (_count > 0)
                    {
                        FamilySymbol symbol = symbolItor.Current as FamilySymbol;
                        XYZ location = new XYZ(_X, y, 0.0);
                        FamilyInstance instance = _projectDoc.Create.NewFamilyInstance(location, symbol, StructuralType.NonStructural);
                        y += _Y;
                        --_count;
                    }
                    transPlace.Commit();
                }
                familyDoc.Close(false);

                _X += _side + FormFile.xPad;
            }

            catch (Exception ex)
            {
                TaskDialog.Show("Load Error", ex.Message);
            }
        }
        private void SetView(Document familyDoc)
        {
            FilteredElementCollector col = new FilteredElementCollector(familyDoc);
            col.OfCategory(BuiltInCategory.OST_Views).WhereElementIsNotElementType().ToElements();

            foreach (View v in col)
            {
                if (v.ViewType == ViewType.FloorPlan) _view = v;
            }
        }

        private void BuildFamily(Document familyDoc)
        {
            try
            {
                Transaction tR = new Transaction(familyDoc, "Set References");
                if (tR.Start() == TransactionStatus.Started)
                {
                    SetReferences(familyDoc);
                    tR.Commit();
                }
                Extrusion extrusion = null;
                Transaction tE = new Transaction(familyDoc, "Create Extrusion");
                if (tE.Start() == TransactionStatus.Started)
                {
                    extrusion = CreateExtrusion(familyDoc);
                    tE.Commit();
                }
                if (null != extrusion)
                {
                    Transaction tC = new Transaction(familyDoc, "Set Constraints");
                    if (tC.Start() == TransactionStatus.Started)
                    {
                        SetConstraints(familyDoc, extrusion);
                        tC.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Build Error", ex.Message);
            }
        }

        private void SetReferences(Document familyDoc)
        {
            try
            {
                FilteredElementCollector col = new FilteredElementCollector(familyDoc);
                col.OfClass(typeof(ReferencePlane)).WhereElementIsNotElementType().ToElements();

                foreach (ReferencePlane rplane in col)
                {
                    if (rplane.Name == @"Center (Left/Right)")
                    {
                        _rightCon = new ReferenceArray();
                        _leftCon = new ReferenceArray();
                        _width = new ReferenceArray();
                        _leftCon.Append(rplane.Reference);
                        _width.Append(rplane.Reference);

                        ReferencePlane newplane = familyDoc.FamilyCreate.NewReferencePlane(_vert[1], _vert[2], new XYZ(0, 0, 1), _view);
                        _rightCon.Append(newplane.Reference);
                        _width.Append(newplane.Reference);
                    }
                    else if (rplane.Name == @"Center (Front/Back)")
                    {
                        _topCon = new ReferenceArray();
                        _botCon = new ReferenceArray();
                        _height = new ReferenceArray();
                        _botCon.Append(rplane.Reference);
                        _height.Append(rplane.Reference);
                        ReferencePlane newplane = familyDoc.FamilyCreate.NewReferencePlane(_vert[2], _vert[3], new XYZ(0, 0, 1), _view);
                        _topCon.Append(newplane.Reference);
                        _height.Append(newplane.Reference);
                    }
                }
            }

            catch (Exception ex)
            {
                TaskDialog.Show("Reference Error", ex.Message);
            }
        }

        private Extrusion CreateExtrusion(Document familyDoc)
        {
            try
            {
                // familyDoc used to be _uiapp
                Plane plane = familyDoc.Application.Create.NewPlane(new XYZ(0.0, 0.0, 1.0),
                                                                    new XYZ(0.0, 0.0, 0.0)
                                                                    );

                SketchPlane s_plane = familyDoc.FamilyCreate.NewSketchPlane(plane);
                ReferenceArray ra = new ReferenceArray();
                CurveArray profile = new CurveArray();
                CurveArrArray caa = new CurveArrArray();

                int i = 0;
                while (i < 3)
                {
                    profile.Append(familyDoc.Application.Create.NewLineBound(_vert[i], _vert[i + 1]));
                    ++i;
                }
                profile.Append(familyDoc.Application.Create.NewLineBound(_vert[i], _vert[i - i]));
                caa.Append(profile);
                Extrusion extrusion = familyDoc.FamilyCreate.NewExtrusion(true, caa, s_plane, 10.0);
                AssignSubCategory(familyDoc, extrusion);
                Line line = familyDoc.Application.Create.NewLine(_vert[0], _vert[1], true);
                ConstructParam(familyDoc, _width, line, "Width");
                line = familyDoc.Application.Create.NewLine(_vert[1], _vert[2], true);
                ConstructParam(familyDoc, _height, line, "Height");
                SetFormula(familyDoc);


                return extrusion;
            }

            catch (Exception ex)
            {
                TaskDialog.Show("Extrusion Error", ex.Message);
                return null;
            }
        }

        private void SetConstraints(Document familyDoc, Extrusion extrusion)
        {
            CurveArrArray curvesArr = new CurveArrArray();
            curvesArr = extrusion.Sketch.Profile;

            foreach (CurveArray ca in curvesArr)
            {
                CurveArrayIterator itor = ca.ForwardIterator();
                itor.Reset();
                itor.MoveNext();
                Line l = itor.Current as Line;
                _rightCon.Append(l.Reference);
                itor.MoveNext();
                l = itor.Current as Line;
                _topCon.Append(l.Reference);
                itor.MoveNext();
                l = itor.Current as Line;
                _leftCon.Append(l.Reference);
                l = itor.Current as Line;
                _botCon.Append(l.Reference);
            }
            ReferenceArrayArray conArray = new ReferenceArrayArray();

            conArray.Append(_rightCon);
            conArray.Append(_topCon);
            conArray.Append(_leftCon);
            conArray.Append(_botCon);

            ConstructConstraint(familyDoc, _rightCon);
            ConstructConstraint(familyDoc, _topCon);
            ConstructConstraint(familyDoc, _leftCon);
        }

        private void ConstructConstraint(Document familyDoc, ReferenceArray ra)
        {
            Reference ref1 = null;
            Reference ref2 = null;
            int i = 1;
            foreach (Reference r in ra)
            {
                if (i == 1)
                {
                    ref1 = r;
                }
                else
                {
                    ref2 = r;
                }
                ++i;
            }
            Autodesk.Revit.DB.Dimension alignment = familyDoc.FamilyCreate.NewAlignment(_view, ref1, ref2);
        }

        private void ConstructParam(Document familyDoc, ReferenceArray ra, Line line, string label)
        {
            Autodesk.Revit.DB.Dimension dim = familyDoc.FamilyCreate.NewDimension(_view, line, ra);
            FamilyParameter param = familyDoc.FamilyManager.AddParameter(label, BuiltInParameterGroup.PG_CONSTRAINTS, ParameterType.Length, true);
            dim.Label = param;
        }

        private void SetFormula(Document familyDoc)
        {
            _type = familyDoc.FamilyManager.NewType(_name);
            familyDoc.FamilyManager.AddParameter("Area", BuiltInParameterGroup.PG_CONSTRAINTS, ParameterType.Area, true);
            FamilyParameter param = familyDoc.FamilyManager.get_Parameter("Area");
            if (null != param)
            {
                familyDoc.FamilyManager.Set(param, _area);
            }
            param = familyDoc.FamilyManager.get_Parameter("Height");
            if (null != param)
            {
                familyDoc.FamilyManager.SetFormula(param, @"Area / Width");
            }
        }

        private void AssignSubCategory(Document familyDoc, GenericForm extrusion)
        {
            try
            {
                Category cat = familyDoc.OwnerFamily.FamilyCategory;
                Category subCat = familyDoc.Settings.Categories.NewSubcategory(cat, _name);
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
            FillPatternElement fill = null;
            FilteredElementCollector collector = new FilteredElementCollector(familyDoc);
            ICollection<FillPatternElement> fillPatternElements = collector.OfClass(typeof(FillPatternElement)).Cast<FillPatternElement>().ToList();

            foreach (FillPatternElement fillPattern in fillPatternElements)
            {
                if (fillPattern.Name == "Solid fill")
                {
                    fill = fillPattern;
                }
            }

            try
            {
                if (_isDefaultArea)
                {
                    ElementId matid = Material.Create(familyDoc, _name);
                    Material mat = familyDoc.GetElement(matid) as Material;
                    Color col = new Color(255,
                                          0,
                                          0);
                    mat.Color = col;
                    mat.Transparency = 50;
                    mat.SurfacePattern = fill;
                    mat.SurfacePatternColor = col;
                    mat.CutPattern = fill;
                    mat.CutPatternColor = col;

                    return mat;
                }
                else if (_isDefaultCount)
                {
                    ElementId matid = Material.Create(familyDoc, _name);
                    Material mat = familyDoc.GetElement(matid) as Material;
                    Color col = new Color(0,
                                          0,
                                          255);
                    mat.Color = col;
                    mat.Transparency = 50;
                    mat.SurfacePattern = fill;
                    mat.SurfacePatternColor = col;
                    mat.CutPattern = fill;
                    mat.CutPatternColor = col;

                    return mat;
                }
                else
                {
                    Random r = new Random();
                    ElementId matid = Material.Create(familyDoc, _name);
                    Material mat = familyDoc.GetElement(matid) as Material;
                    Color col = new Color((byte)r.Next(1, 255),
                                          (byte)r.Next(1, 255),
                                          (byte)r.Next(1, 255));
                    mat.Color = col;
                    mat.Transparency = 0;
                    mat.SurfacePattern = fill;
                    mat.SurfacePatternColor = col;
                    mat.CutPattern = fill;
                    mat.CutPatternColor = col;

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
