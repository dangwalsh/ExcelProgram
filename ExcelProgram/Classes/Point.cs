using System;

namespace ExcelProgram
{
    public struct Point
    {
        private double _X;
        private double _Y;

        public double X
        {
            get { return _X; }
            set { _X = value; }
        }
        public double Y
        {
            get { return _Y; }
            set { _Y = value; }
        }
    }
}
