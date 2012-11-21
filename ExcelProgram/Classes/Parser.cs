using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace ExcelProgram
{
    public class Parser
    {
        private char[] _Delimiter = new char[] { '\t' };
        private List<Shape> _ProgramShapes = new List<Shape>();
        private string _Invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        private int _Incrementer = 2;
        private int _nCol;
        private int _aCol;
        private int _cCol;

        public  List<Shape> ProgramShapes
        {
            get { return _ProgramShapes; }
        }

        public Parser(string filename, int ncol, int acol, int ccol)
        {
            _nCol = ncol;
            _aCol = acol;
            _cCol = ccol;

            StreamReader reader = File.OpenText(filename);
            string line;
            string[] a;

            while (null != (line = reader.ReadLine()))
            {
                a = line.Split(_Delimiter).ToArray();

                string s = "";
                double d;
                int n;

                for (int i = _nCol; i < _nCol + 2; ++i)
                {
                    if (a[i] != "")
                    {
                        s = a[i];
                        foreach (char c in _Invalid)
                        {
                            s = s.Replace(c.ToString(), "");
                        }

                        if (_ProgramShapes.Any(shape => shape.Name == s))
                        {
                            s += " " + _Incrementer;
                            ++_Incrementer;
                        }

                        if (Double.TryParse(a[_aCol], out d))
                        {
                            if (Int32.TryParse(a[_cCol], out n))
                            {
                                _ProgramShapes.Add(new Shape(s, d, n));
                            }
                            else
                            {
                                _ProgramShapes.Add(new Shape(s, d));
                            }
                        }
                        else
                        {
                            _ProgramShapes.Add(new Shape(s));
                        }
                        break;
                    }
                }                
            }
        }
    }
}
