using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace ExcelProgram
{
    public class Parser
    {
        private static char[] _Delimiter = new char[] { ',' };
        private List<Shape> _ProgramShapes = new List<Shape>();

        public List<Shape> ProgramShapes
        {
            get { return _ProgramShapes; }
        }

        public Parser(string filename)
        {
            StreamReader reader = File.OpenText(filename);
            string line;
            string[] a;

            // this loop will need more complexity once we have the actual csv format
            // for now it only reads a name and corresponding area
            while (null != (line = reader.ReadLine()))
            {
                a = line.Split(_Delimiter).ToArray();
                double n;
                bool result = Double.TryParse(a[1], out n);
                if (result)
                {
                    _ProgramShapes.Add(new Shape(a[0], n));
                }
            }
        }
    }
}
