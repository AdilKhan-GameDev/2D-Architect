using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace _2D_Graphics.Model
{
    public class ShapeObject
    {
        private List<Point> shapeCords = new List<Point>();
        private double area;
        private Point center;
        private double numberOfLines;
        private double[] linesLengths;
        private string name;
        private SolidColorBrush color;
        private Polygon shape;
        private List<Control> controls;

        public List<Point> ShapeCords { get => shapeCords; set => shapeCords = value; }
        public double Area { get => area; set => area = value; }
        public Point Center { get => center; set => center = value; }
        public double NumberOfLines { get => numberOfLines; set => numberOfLines = value; }
        public double[] LinesLengths { get => linesLengths; set => linesLengths = value; }
        public string Name { get => name; set => name = value; }
        public SolidColorBrush Color { get => color; set => color = value; }
        public Polygon Shape { get => shape; set => shape = value; }
        public List<Control> Controls { get => controls; set => controls = value; }
    }

}
