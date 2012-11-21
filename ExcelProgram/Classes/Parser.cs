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

        public  List<Shape> ProgramShapes
        {
            get { return _ProgramShapes; }
        }

        public Parser(string filename)
        {
            StreamReader reader = File.OpenText(filename);
            string line;
            string[] a;

            while (null != (line = reader.ReadLine()))
            {
                a = line.Split(_Delimiter).ToArray();

                string s = "";
                double d;
                int n;

                for (int i = 2; i < 4; ++i)
                {
                    if (a[i] != "")
                    {
                        s = a[i];
                        foreach (char c in _Invalid)
                        {
                            s = s.Replace(c.ToString(), "");
                        }

                        if (Double.TryParse(a[8], out d))
                        {
                            if (Int32.TryParse(a[9], out n))
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
