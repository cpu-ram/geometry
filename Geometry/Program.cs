using System;
using Geometry;
using Geometry.BiDimentional;

namespace Geometry
{
    public class Program
    {
        static void Main(string[] args)
        {
            TestConvexHull();
        }
        static void TestTrigonometry()
        {
        }
        public static void TestGeometry()
        {
            decimal height = (decimal)((3 * Math.Sqrt(3)) / 2);
            Plane plane = new Plane();
            Point point1 = new Point(10, 10);
            Point point2 = new Point(110, 70);
            Line newLine = new Line(point1, point2);
            Console.WriteLine(newLine);
        }
        public static void TestConvexHull()
        {
            Plane plane = new Plane();
            Point[] points = new Point[14];

            points[0] = new Point(20, 70);
            points[1] = new Point(70, 80);
            points[2] = new Point(110, 70);
            points[3] = new Point(30, 70);
            points[4] = new Point(30, 60);
            points[5] = new Point(90, 60);
            points[6] = new Point(60, 50);
            points[7] = new Point(40, 40);
            points[8] = new Point(20, 20);
            points[9] = new Point(50, 20);
            points[10] = new Point(80, 20);
            points[11] = new Point(100, 30);
            points[12] = new Point(10, 10);
            points[13] = new Point(-10, -10);

            foreach (Point point in points)
            {
                plane.AddPoint(point);
            }
            plane.FindConvexHull();
        }
        
    }
}
