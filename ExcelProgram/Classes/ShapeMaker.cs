using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace ExcelProgram
{
    public class ShapeMaker
    {
        private DataTable _Data;
        private int _NCol;
        private int _ACol;
        private int _CCol;
        private int _NCnt;
        private int _ACnt;
        private int _CCnt;
        private static List<Shape> _Shapes;
        private int _Incrementer = 2;
        private string _Invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

        public static List<Shape> Shapes
        {
            get { return _Shapes; }
        }

        public ShapeMaker(DataTable data, DataColumn n, DataColumn a, DataColumn c)
        {
            _Data = data;
            _NCol = n.Ordinal;
            _ACol = a.Ordinal;
            _CCol = c.Ordinal;
            _NCnt = FormTable.NCount;
            _ACnt = FormTable.ACount;
            _CCnt = FormTable.CCount;
            _Shapes = new List<Shape>();

            foreach (DataRow row in _Data.Rows)
            {
                for (int i = _NCol; i < _NCol + _NCnt + 1; ++i)
                {
                    string name = row[i].ToString();
                    if (name != "")
                    {
                        foreach (char ch in _Invalid)
                        {
                            name = name.Replace(ch.ToString(), " ");
                        }

                        if (_Shapes.Any(shape => shape.Name == name))
                        {
                            name += " " + _Incrementer;
                            ++_Incrementer;
                        }

                        for (int j = _ACol; j < _ACol + _ACnt + 1; ++j)
                        {
                            double area;
                            if (Double.TryParse(row[j].ToString(), out area))
                            {
                                if (area == 0) continue;
                                for (int k = _CCol; k < _CCol + _CCnt + 1; ++k)
                                {
                                    int count;
                                    if (Int32.TryParse(row[k].ToString(), out count))
                                    {
                                        if (count == 0) continue;
                                        _Shapes.Add(new Shape(name, area, count));
                                    }
                                    else
                                    {
                                        _Shapes.Add(new Shape(name, area));
                                    }
                                }
                            }
                            else
                            {
                                _Shapes.Add(new Shape(name));
                            }
                        }
                    }
                }
            }
        }
    }
}
