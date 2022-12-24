using System;
using System.Text;
using System.Collections.Generic;
using CustomCollections;

namespace Geometry.Plane
{
    public partial class PointSet
    {
        public string FindConvexHull()
        {
            Point maxY = GetMaxYPoint();
            Point maxX = GetMaxXPoint();
            Point minY = GetMinYPoint();
            Point minX = GetMinXPoint();

            Point[] extremePointsArray = new Point[4];
            extremePointsArray[0] = maxY;
            extremePointsArray[1] = maxX;
            extremePointsArray[2] = minY;
            extremePointsArray[3] = minX;

            HashSet<Point> pointsSet = new HashSet<Point>();
            foreach (Point point in extremePointsArray)
            {
                if (!pointsSet.Contains(point))
                {
                    pointsSet.Add(point);
                }
            }
            int pointsSetCount = pointsSet.Count;
            Point[] uniqueExtremePointsArray = new Point[pointsSetCount];
            int counter = 0;
            foreach (Point point in pointsSet)
            {
                uniqueExtremePointsArray[counter] = point;
                counter++;
            }

            Polygon resultPolygon = new Polygon(uniqueExtremePointsArray);
            ExpandPolygon(ref resultPolygon, this.xToYDictionary);
            return resultPolygon.ToString();

            bool ExpandPolygon(ref Polygon polygon, Dictionary<decimal, SortedSet<decimal>> xToYDictionary)
            {
                bool continueExpanding = true;
                while (continueExpanding)
                {
                    continueExpanding = false;
                    Segment[] edges = polygon.GetEdges();
                    for (int i = 0; i < edges.Length; i++)
                    {
                        Segment currentEdge = edges[i];
                        if (ExpandEdge(currentEdge, ref polygon, xToYDictionary))
                        {
                            continueExpanding = true;
                        }
                    }
                }

                return false;
            }
            bool ExpandEdge(Segment edge, ref Polygon polygon,
                Dictionary<decimal, SortedSet<decimal>> xToYDictionary)
            {
                Point endPointOne = edge.StartingPoint;
                Point endPointTwo = edge.EndPoint;
                Point[] pointsOutsideEdge = FindPointsPastEdge(edge, polygon);

                if (pointsOutsideEdge.Length > 0)
                {
                    Point newPoint = FindMostRemovedPoint(edge, pointsOutsideEdge);
                    if (newPoint != null)
                    {
                        polygon.AddPoint(newPoint, endPointOne, endPointTwo);
                        return true;
                    }
                    else return false;
                }
                else return false;
            }
            Point[] FindPointsPastEdge(Segment edge, Polygon polygon)
            {
                List<Point> foundPoints = new List<Point>();
                Point[] resultPoints;

                Tuple<Point, Point> edgeVertices = edge.GetPoints();
                Tuple<decimal, decimal> xLimits =
                    CreateOrderedDecimalTuple(edgeVertices.Item1.X, edgeVertices.Item2.X);
                Tuple<decimal, decimal> yLimits =
                    CreateOrderedDecimalTuple(edgeVertices.Item1.Y, edgeVertices.Item2.Y);
                SortedSet<decimal> xPositionsWithinRange =
                    xSet.GetViewBetween(xLimits.Item1, xLimits.Item2);

                IEnumerator<decimal> enumerator = xPositionsWithinRange.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    decimal currentX = enumerator.Current;
                    SortedSet<decimal> currentYSet = xToYDictionary[currentX];
                    SortedSet<decimal> filteredYSet =
                        currentYSet.GetViewBetween(yLimits.Item1, yLimits.Item2);
                    foreach (decimal currentY in filteredYSet)
                    {
                        Point currentPoint = new Point(currentX, currentY);

                        if (!polygon.ContainsPoint(currentPoint)
                                && !polygon.SurroundsPoint(currentPoint)
                                    && !edge.ContainsEndpoint(currentPoint))
                        {
                            Segment pathFromSide1 = new Segment(edge.StartingPoint, currentPoint);
                            Segment pathFromSide2 = new Segment(edge.EndPoint, currentPoint);
                            int intersectionsNumber1 =
                                pathFromSide1.FindIntersectionsNumber(polygon);
                            int intersectionsNumber2 =
                                pathFromSide2.FindIntersectionsNumber(polygon);
                            if (intersectionsNumber1 == 1 && intersectionsNumber2 == 1)
                            {
                                foundPoints.Add(currentPoint);
                            }
                        }
                    }
                }
                resultPoints = foundPoints.ToArray();
                return resultPoints;

                static Tuple<decimal, decimal> CreateOrderedDecimalTuple(decimal entryOne, decimal entryTwo)
                {
                    Tuple<decimal, decimal> resultTuple;
                    if (entryOne > entryTwo)
                    {
                        resultTuple = new Tuple<decimal, decimal>(entryTwo, entryOne);
                        return resultTuple;
                    }
                    else
                    {
                        resultTuple = new Tuple<decimal, decimal>(entryOne, entryTwo);
                        return resultTuple;
                    }
                }
            }
            Point FindMostRemovedPoint(Segment edge, Point[] pointsArray)
            {
                if (pointsArray.Length == 0)
                {
                    throw new ArgumentException();
                }
                Line edgeLine = new Line(edge);

                Point mostRemovedPoint = pointsArray[0];
                decimal greatestHeight = edgeLine.GetHeightFromPoint(mostRemovedPoint);
                Point currentPoint;
                decimal currentHeight;
                for (int i = 1; i < pointsArray.Length; i++)
                {
                    currentPoint = pointsArray[i];
                    currentHeight = edgeLine.GetHeightFromPoint(currentPoint);
                    if (currentHeight > greatestHeight)
                    {
                        mostRemovedPoint = currentPoint;
                        greatestHeight = currentHeight;
                    }
                }
                return mostRemovedPoint;
            }
        }

    }
}
