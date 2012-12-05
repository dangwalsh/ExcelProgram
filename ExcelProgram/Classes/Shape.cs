using System;

namespace ExcelProgram
{
    public class Shape
    {
        private Point[] _Points = new Point[4];
        private string _Name;
        private int _Count = 1;
        private double _Area;
        private double _Side = 10.0;
        private bool _IsDefaultArea = false;
        private bool _IsDefaultCount = false;

        public Point[] Points
        {
            get { return _Points; }
        }

        public string Name
        {
            get { return _Name; }
        }

        public double Area
        {
            get { return _Area; }
        }

        public double Side
        {
            get { return _Side; }
        }

        public int Count
        {
            get { return _Count; }
        }

        public bool IsDefaultArea
        {
            get { return _IsDefaultArea; }
        }

        public bool IsDefaultCount
        {
            get { return _IsDefaultCount; }
        }

        public Shape(string name) // name only
        {
            _Points[0].X = 0; _Points[0].Y = 0;
            _Points[1].X = 9.9; _Points[1].Y = 0;
            _Points[2].X = 9.9; _Points[2].Y = 9.9;
            _Points[3].X = 0; _Points[3].Y = 9.9;

            _Name = name;
            _Area = 98.01;
            _Side = 10.0;
            _Count = 1;
            _IsDefaultArea = true;
        }

        public Shape(string name, double a) // area and name
        {
            _Area = a;
            double s = Math.Sqrt(_Area);

            _Points[0].X = 0; _Points[0].Y = 0;
            _Points[1].X = s; _Points[1].Y = 0;
            _Points[2].X = s; _Points[2].Y = s;
            _Points[3].X = 0; _Points[3].Y = s;

            _Name = name;
            _Side = s;
            _Count = 1;
            _IsDefaultCount = true;
        }

        public Shape(string name, double a, int c) // area, name and count
        {
            _Area = a;
            double s = Math.Sqrt(_Area);

            _Points[0].X = 0; _Points[0].Y = 0;
            _Points[1].X = s; _Points[1].Y = 0;
            _Points[2].X = s; _Points[2].Y = s;
            _Points[3].X = 0; _Points[3].Y = s;

            _Name = name;
            _Side = s;
            _Count = c;
        }
    }
}
