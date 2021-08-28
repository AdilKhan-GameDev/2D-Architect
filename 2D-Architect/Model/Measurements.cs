using _2D_Graphics.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace _2D_Graphics.Model
{
    public class Measurements
    {

        static Color colorfromswatch = Color.FromArgb(128,255,155,155);
        //Calculate the length of a Given Line using 2 points
        public static double LineDistance(Point p1, Point p2)
        {
            double distance = 0;
            distance = Point.Subtract(p2, p1).Length;
            return distance;
        }

        //Calculate the Area of ShapeObject
        public static double GetArea(ShapeObject obj)
        {
            int n = obj.ShapeCords.Count;
            double area = 0.0;
            // Calculate value of shoelace formula 
            int j = n - 1;
            for (int i = 0; i < n; i++)
            {
                area += (obj.ShapeCords[j].X + obj.ShapeCords[i].X) * (obj.ShapeCords[j].Y - obj.ShapeCords[i].Y);
                // j is previous vertex to i 
                j = i;
            }
            // Return absolute value 
            return Math.Abs(area / 2.0);

        }

        //Calculate the Center Point of ShapeObject
        public static Point GetCenter(ShapeObject obj)
        {
            Point center = new Point();
            int n = obj.ShapeCords.Count;
            double x = 0;
            double y = 0;
            for (int i = 0; i < n-1; i++)
            {
                center = obj.ShapeCords[i];
                x += center.X;
                y += center.Y;
            }

            center.X = x / (n-1);
            center.Y = y / (n-1);


            return center;
        }

        //...
        //public static void FillPolygonPoint(ShapeObject sobj)
        //{

        //    // Create solid brush.
        //    System.Drawing.SolidBrush blueBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Aqua);
        //    // Draw polygon to screen.
        //    System.Drawing.Point[] parray = new System.Drawing.Point[sobj.ShapeCords.Count];
        //    for (int i= 0; i < sobj.ShapeCords.Count;i++)
        //    {
        //        System.Drawing.Point p = new System.Drawing.Point(Convert.ToInt32(sobj.ShapeCords[i].X), Convert.ToInt32(sobj.ShapeCords[i].Y));
        //        parray[i] = p;
        //    }

        //    // Draw polygon to screen.
        //    System.Drawing.Graphics.FillPolygon(blueBrush,parray);
        //}
        public static void SetColor(Color color)
        {
            //ColorSwatch win = new ColorSwatch();
            //win.Show();
            colorfromswatch = color;
        }

        public static SolidColorBrush GetColor()
        {
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = colorfromswatch;
            return mySolidColorBrush;
        }








    }
}
