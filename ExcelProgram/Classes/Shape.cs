using System;

namespace ExcelProgram
{
    public class Shape
    {
        private Point[] _Points = new Point[4];
        private string _Name;
        private int _Count = 1;
        private double _Side = 10.0;

        public Point[] Points
        {
            get { return _Points; }
        }

        public string Name
        {
            get { return _Name; }
        }

        public double Side
        {
            get { return _Side; }
        }

        public int Count
        {
            get { return _Count; }
        }

        //public Shape(string name) // name only
        //{
        //    _Points[0].X = 0; _Points[0].Y = 0;
        //    _Points[1].X = 10; _Points[1].Y = 0;
        //    _Points[2].X = 10; _Points[2].Y = 10;
        //    _Points[3].X = 0; _Points[3].Y = 10;

        //    _Name = name;
        //}

        //public Shape(string name, double a) // area and name
        //{
        //    double n = Math.Sqrt(a);

        //    _Points[0].X = 0; _Points[0].Y = 0;
        //    _Points[1].X = n; _Points[1].Y = 0;
        //    _Points[2].X = n; _Points[2].Y = n;
        //    _Points[3].X = 0; _Points[3].Y = n;

        //    _Name = name;
        //    _Side = n;
        //}

        // make three separate overloaded constructors rather than default values
        // and add data property to color default
        public Shape(string n = "Default", double a = 100.0, int c = 1) 
        {
            double s = Math.Sqrt(a);

            _Points[0].X = 0; _Points[0].Y = 0;
            _Points[1].X = s; _Points[1].Y = 0;
            _Points[2].X = s; _Points[2].Y = s;
            _Points[3].X = 0; _Points[3].Y = s;

            _Name = n;
            _Side = s;
            _Count = c;
        }
    }
}
