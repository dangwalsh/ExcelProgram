using System;

namespace ExcelProgram
{
    public class Shape
    {
        private Point[] _Points = new Point[4];
        private string _Name;

        public Shape(string name, double a)
        {
            double n = Math.Sqrt(a)/2;
            _Points[0].X = n; _Points[0].Y = n;
            _Points[1].X = -n; _Points[1].Y = n;
            _Points[2].X = -n; _Points[2].Y = -n;
            _Points[3].X = n; _Points[3].Y = -n;

            _Name = name;
        }

        public Point[] Points
        {
            get { return _Points; }
        }

        public string Name
        {
            get { return _Name; }
        }
    }
}
