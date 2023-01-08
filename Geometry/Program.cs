using System;
using Geometry;
using Geometry.Plane;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Geometry
{
    public class Program
    {
        static void Main(string[] args)
        {
            TestSegmentContainsPoint();
        }
        public static void TestGeometry()
        {
            Polygon polygon = new Polygon("[{\"x\":379,\"y\":493},{\"x\":449,\"y\":445},{\"x\":489,\"y\":336},{\"x\":96,\"y\":8},{\"x\":3,\"y\":211}]");
            Segment segmentOne = new Segment("[{\"x\":109,\"y\":297},{\"x\":489,\"y\":336}]");
            Segment segmentTwo = new Segment("[{\"x\":96,\"y\":8},{\"x\":109,\"y\":297}]");
            int intersectionsNumber = segmentOne.FindIntersectionsNumber(polygon);
            Console.WriteLine();
        }
        public static void TestLineIntersection()
        {
            Point pointOne = new Point(109, 297);
            Point pointTwo = new Point(489, 336);

            Point pointThree = new Point(3, 211);
            Point pointFour = new Point(379, 493);

            Segment segmentOne = new Segment(pointOne, pointTwo);
            Segment segmentTwo = new Segment(pointThree, pointFour);

            bool intersects = segmentOne.Intersects(segmentTwo);
            Console.WriteLine();
        }
        public static void TestSegmentAndPolygonIntersection()
        {
            Polygon polygon = new Polygon("[{\"x\":386.5,\"y\":436},{\"x\":311.5,\"y\":90},{\"x\":136.5,\"y\":155},{\"x\":133.5,\"y\":296}]");
            Segment segment = new Segment("[{\"x\":136.5,\"y\":155},{\"x\":213.5,\"y\":110}]");
            Point[] intersections = segment.FindIntersections(polygon);
            Console.WriteLine();
        }
        public static void TestSegmentContainsPoint()
        {
            Segment segment = new Segment("[{\"x\":136.5,\"y\":155},{\"x\":213.5,\"y\":110}]");
            Point point = new Point(Convert.ToDecimal(136.5000000000), Convert.ToDecimal(155.0000000000));
            bool contains = segment.ContainsPoint(point);
            Console.WriteLine();
        }
        public static void TestConvexHull()
        {
            PointSet points = new PointSet("[{\"x\":136.5,\"y\":155},{\"x\":311.5,\"y\":90},{\"x\":213.5,\"y\":110},{\"x\":133.5,\"y\":296},{\"x\":386.5,\"y\":436}]");
            string convexHullString = points.FindConvexHull();
            Console.WriteLine(convexHullString);
        }
        
    }
}
