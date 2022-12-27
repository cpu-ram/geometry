using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using CustomCollections;
using Newtonsoft.Json;

namespace Geometry.Plane
{
    public partial class PointSet
    {
        private Dictionary<decimal, SortedSet<decimal>> xToYDictionary;
        private Dictionary<decimal, SortedSet<decimal>> yToXDictionary;
        private SortedSet<decimal> xSet;
        private SortedSet<decimal> ySet;

        public PointSet(params Point[] points)
        {
            this.xToYDictionary = new Dictionary<decimal, SortedSet<decimal>>();
            this.yToXDictionary = new Dictionary<decimal, SortedSet<decimal>>();
            this.xSet = new SortedSet<decimal>();
            this.ySet = new SortedSet<decimal>();

            HashSet<Point> pointsSet = new HashSet<Point>(points);

            foreach(Point point in pointsSet)
            {
                AddPoint(point);
            }
        }
        public PointSet(string jsonPoints) : this(Newtonsoft.Json.JsonConvert.DeserializeObject<Point[]>(jsonPoints)) // in progress!
        {
        }

        public void AddPoint(Point entryPoint)
        {
            decimal x = entryPoint.x;
            decimal y = entryPoint.y;

            // checking if the position is occupied
            if(xToYDictionary.ContainsKey(x)
                && yToXDictionary.ContainsKey(y))
            {
                if ((xToYDictionary[x].Contains(y)) ||
                (yToXDictionary[y].Contains(x)))
                {
                    throw new ArgumentOutOfRangeException();
                }
            }

            if (!xToYDictionary.ContainsKey(x))
            {
                xToYDictionary[x] = new SortedSet<decimal>();
            }
            if (!yToXDictionary.ContainsKey(y))
            {
                yToXDictionary[y] = new SortedSet<decimal>();
            }
            xToYDictionary[x].Add(y);
            yToXDictionary[y].Add(x);
            if (!xSet.Contains(x))
            {
                xSet.Add(x);
            }
            if (!ySet.Contains(y))
            {
                ySet.Add(y);
            }
        }
        public void RemovePoint(Point query)
        {
            decimal x = query.x;
            decimal y = query.y;
            // checking 'integrity'
            if ((xToYDictionary[x].Contains(y)) ^
                    (yToXDictionary[y].Contains(x)))
            {
                throw new Exception("Something went wrong");
            }
            // checking if the position is occupied
            if ((!xToYDictionary[x].Contains(y)) ||
                (!yToXDictionary[y].Contains(x)))
            {
                throw new ArgumentOutOfRangeException();
            }

            xToYDictionary[x].Remove(y);
            yToXDictionary[y].Remove(x);
            if (xToYDictionary[x].Count == 0)
            {
                xSet.Remove(x);
            }
            if (yToXDictionary[y].Count == 0)
            {
                xSet.Remove(y);
            }
        }

        public Point GetMaxYPoint()
        {
            decimal maxY = ySet.Max;
            SortedSet<decimal> maxYValues = yToXDictionary[maxY];
            IEnumerator<decimal> enumerator = maxYValues.GetEnumerator();
            enumerator.MoveNext();
            decimal firstAvailableValue = enumerator.Current;
            Point resultPoint = new Point(firstAvailableValue, maxY);
            return resultPoint;
        }
        public Point GetMaxXPoint()
        {
            decimal maxX = xSet.Max;
            SortedSet<decimal> maxXValues = xToYDictionary[maxX];
            IEnumerator<decimal> enumerator = maxXValues.GetEnumerator();
            enumerator.MoveNext();
            decimal firstAvailableValue = enumerator.Current;
            Point resultPoint = new Point(maxX, firstAvailableValue);
            return resultPoint;
        }
        public Point GetMinYPoint()
        {
            decimal minY = ySet.Min;
            SortedSet<decimal> minYValues = yToXDictionary[minY];
            IEnumerator<decimal> enumerator = minYValues.GetEnumerator();
            enumerator.MoveNext();
            decimal firstAvailableValue = enumerator.Current;
            Point resultPoint = new Point(firstAvailableValue, minY);
            return resultPoint;
        }
        public Point GetMinXPoint()
        {
            decimal minX = xSet.Min;
            SortedSet<decimal> maxXValues = xToYDictionary[minX];
            IEnumerator<decimal> enumerator = maxXValues.GetEnumerator();
            enumerator.MoveNext();
            decimal firstAvailableValue = enumerator.Current;
            Point resultPoint = new Point(minX, firstAvailableValue);
            return resultPoint;
        }

        public Point[] GetPoints()
        {
            List<Point> pointsList = new List<Point>();
            foreach(KeyValuePair<decimal,SortedSet<decimal>> keyValuePair in xToYDictionary)
            {
                decimal currentKey = keyValuePair.Key;
                SortedSet<decimal> currentSet = keyValuePair.Value;
                foreach(decimal currentValue in currentSet)
                {
                    Point newPoint = new Point(currentKey, currentValue);
                    pointsList.Add(newPoint);
                }
            }
            Point[] points = pointsList.ToArray();
            return points;
        }
        public override string ToString()
        {
            Point[] allPoints = this.GetPoints();
            string jsonString = System.Text.Json.JsonSerializer.Serialize(allPoints);
            return jsonString;
        }
    }
    public class Point
    {
        private decimal xCoordinate;
        private decimal yCoordinate;

        [Newtonsoft.Json.JsonConstructorAttribute]
        public Point(decimal x, decimal y)
        {
            this.x = Math.Round(x,10);
            this.y = Math.Round(y,10);
        }

        public Point(Point reference)
        {
            this.x = reference.x;
            this.y = reference.y;
        }

        public decimal x { get => xCoordinate; set => xCoordinate = value; }
        public decimal y { get => yCoordinate; set => yCoordinate = value; }

        public bool Equals(Point anotherPoint)
        {
            decimal errorMargin = (decimal)(Math.Pow(10, -10));
            NumeralInterval errorMarginInterval =
                new NumeralInterval(-errorMargin,true, errorMargin, true);
            decimal xDifference = this.x - anotherPoint.x;
            decimal yDifference = this.y - anotherPoint.y;

            bool xDifferenceIsWithinErrorMargin = errorMarginInterval.Contains(xDifference);
            bool yDifferenceIsWithinErrorMargin = errorMarginInterval.Contains(yDifference);

            if (xDifferenceIsWithinErrorMargin && yDifferenceIsWithinErrorMargin)
            {
                return true;
            }
            else return false;
        }
        public override bool Equals(object anotherObject) 
        {
            return this.Equals(anotherObject as Point);
        }
        public override int GetHashCode()
        {
            int resultHashCode = (x.GetHashCode() + y).GetHashCode();
            return resultHashCode;
        }
        public int CompareTo(Point anotherPoint)
        {
            int comparisonByX = (this.x).CompareTo(anotherPoint.x);
            int comparisonByY = (this.y).CompareTo(anotherPoint.y);

            switch (comparisonByX)
            {
                case 1:
                    return 1;
                case 0:
                    switch (comparisonByY)
                    {
                        case 1:
                            return 1;
                        case 0:
                            return 0;
                        case -1:
                            return -1;
                        default:
                            throw new Exception();
                    }
                case -1:
                    return -1;
                default:
                    throw new Exception();
            }
        }
        public QuadrantPosition GetRelativeQuadrantPosition(Point referencePoint)
        {
            QuadrantPosition resultDirection;
            int xComparison = this.x.CompareTo(referencePoint.x);
            int yComparison = this.y.CompareTo(referencePoint.y);
            switch (xComparison)
            {
                case 1:
                    switch (yComparison)
                    {
                        case 1:
                            return QuadrantPosition.I;
                            break;
                        case 0:
                            return QuadrantPosition.xAxisRight;
                            break;
                        case -1:
                            return QuadrantPosition.IV;
                            break;
                        default:
                            throw new Exception();
                    }
                    break;
                case 0:
                    switch (yComparison)
                    {
                        case 1:
                            return QuadrantPosition.yAxisUp;
                            break;
                        case 0:
                            return QuadrantPosition.Equals;
                            break;
                        case -1:
                            return QuadrantPosition.yAxisDown;
                            break;
                        default:
                            throw new Exception();
                    }
                    break;
                case -1:
                    switch (yComparison)
                    {
                        case 1:
                            return QuadrantPosition.II;
                            break;
                        case 0:
                            return QuadrantPosition.xAxisLeft;
                            break;
                        case -1:
                            return QuadrantPosition.IV;
                            break;
                        default:
                            throw new Exception();
                    }
                    break;

                default:
                    throw new Exception();
            }
        }
        public double GetRelativeRotationAngle(Step step)
        {
            double tempRotationAngle;
            double resultRotationAngle;
            Point startingPoint = step.StartingPoint;
            Point endPoint = step.EndPoint;

            double startingPointDirection = this.GetRelativeDirection(startingPoint);
            double endPointDirection = this.GetRelativeDirection(endPoint);
            tempRotationAngle = endPointDirection - startingPointDirection;
            if (tempRotationAngle > (-(Math.PI))
                && tempRotationAngle < Math.PI)
            {
                resultRotationAngle = tempRotationAngle;
            }
            else if (tempRotationAngle > Math.PI)
            {
                resultRotationAngle = tempRotationAngle - (Math.PI * 2);
            }
            else if(tempRotationAngle < -(Math.PI))
            {
                resultRotationAngle = tempRotationAngle + (Math.PI * 2);
            }
            else
            {
                throw new Exception();
            }
            return resultRotationAngle;
        }
        public double GetRelativeDirection(Point reference) //returns an angle
        {
            double resultValue;
            if (this.Equals(reference))
            {
                throw new ArgumentException();
            }

            decimal xDifference = reference.x - this.x;
            decimal yDifference = reference.y - this.y;

            if (xDifference == 0)
            {
                if (this.x > reference.x)
                {
                    resultValue = Math.PI / 2;
                    return resultValue;
                }
                else
                {
                    resultValue = -(Math.PI / 2);
                    return resultValue;
                }
            }
            else if (yDifference == 0)
            {
                if (xDifference > 0)
                {
                    resultValue = 0;
                    return resultValue;
                }
                else if (xDifference < 0)
                {
                    resultValue = Math.PI / 2;
                    return resultValue;
                }
                else
                {
                    throw new Exception();
                }
            }
            else
            {
                double tangent = (double)(yDifference / xDifference);
                if (xDifference < 0)
                {
                    resultValue = Math.Atan(tangent) + Math.PI;
                    return resultValue;
                }
                else if (xDifference > 0)
                {
                    if (yDifference > 0)
                    {
                        resultValue = Math.Atan(tangent);
                        return resultValue;
                    }
                    else if (yDifference < 0)
                    {
                        resultValue = Math.Atan(tangent) + (Math.PI) * 2;
                        return resultValue;
                    }
                    else throw new Exception();
                }
                else
                {
                    throw new Exception();
                }
            }
        }
        public double GetAngle(Segment section)
        {
            double tempAngle;
            double resultAngle;

            Tuple<Point, Point> exteriorPoints = section.GetPoints();
            Point point1 = exteriorPoints.Item1;
            Point point2 = exteriorPoints.Item2;
            double direction1 = point1.GetRelativeDirection(this);
            double direction2 = point2.GetRelativeDirection(this);
            tempAngle = Math.Abs(direction1 - direction2);
            if (tempAngle > Math.PI)
            {
                resultAngle = (2 * Math.PI) - tempAngle;
            }
            else resultAngle = tempAngle;
            return resultAngle;
        }

        public override string ToString()
        {
            string resultString = System.Text.Json.JsonSerializer.Serialize(this);
            return resultString;
        }
    }
    public class Segment
    {
        Point startingPoint;
        Point endPoint;
        public Segment(Point pointOne, Point pointTwo)
        {
            switch (pointOne.CompareTo(pointTwo))
            {
                case 1:
                    startingPoint = pointTwo;
                    endPoint = pointOne;
                    break;
                case 0:
                    startingPoint = pointOne;
                    endPoint = pointTwo;
                    break;
                case -1:
                    startingPoint = pointOne;
                    endPoint = pointTwo;
                    break;
                default:
                    throw new Exception();
            }
        }
        public Segment(string jsonString) // I need to figure out how to refactor this and other things like this!
        {
            Point[] points = new Point[2];
            try
            {
                points = Newtonsoft.Json.JsonConvert.DeserializeObject<Point[]>(jsonString);
            }
            catch(Exception ex)
            {
                throw ex;
            }

            Point pointOne = points[0];
            Point pointTwo = points[1];
            switch (pointOne.CompareTo(pointTwo))
            {
                case 1:
                    startingPoint = pointTwo;
                    endPoint = pointOne;
                    break;
                case 0:
                    startingPoint = pointOne;
                    endPoint = pointTwo;
                    break;
                case -1:
                    startingPoint = pointOne;
                    endPoint = pointTwo;
                    break;
                default:
                    throw new Exception();
            }

        }
        public Point StartingPoint
        {
            get
            {
                return startingPoint;
            }
        }
        public Point EndPoint
        {
            get
            {
                return endPoint;
            }
        }
        internal Tuple<Point, Point> GetPoints()
        {
            return new Tuple<Point, Point>(StartingPoint, EndPoint);
        }
        public bool ContainsEndpoint(Point entryPoint)
        {
            if (StartingPoint.Equals(entryPoint)
                    || EndPoint.Equals(entryPoint))
            {
                return true;
            }
            else return false;
        }
        public bool ContainsPoint(Point entryPoint)
        {
            bool result;
            Line tempLine = new Line(this);
            if (tempLine.ContainsPoint(entryPoint))
            {
                if (entryPoint.y >= StartingPoint.y && entryPoint.y <= EndPoint.y)
                {
                    result = true;
                }
                else result = false;
            }
            else result = false;

            return result;
        }
        public bool Intersects(Segment referenceSegment)
        {
            bool result;

            Line currentSegmentLine = new Line(this);
            Line referenceSegmentLine = new Line(referenceSegment);

            if (currentSegmentLine.Equals(referenceSegmentLine))
            {
                NumeralInterval currentSegmentXInterval = new NumeralInterval
                    (this.StartingPoint.x, true, this.EndPoint.x, true);
                NumeralInterval referenceSegmentXInterval = new NumeralInterval
                    (referenceSegment.StartingPoint.x, true, referenceSegment.EndPoint.x, true);

                if (currentSegmentXInterval.Intersects(referenceSegmentXInterval))
                {
                    return true;
                }
                else return false;
            }
            else
            {
                Point intersectionPoint = currentSegmentLine.FindIntersection(referenceSegmentLine);
                if (intersectionPoint != null)
                {
                    if (this.ContainsPoint(intersectionPoint))
                    {
                        result = true;
                    }
                    else result = false;
                }
                else result = false;
            }
            return result;
        }
        public Point FindIntersection(Segment referenceSegment)
        {
            Point intersection;
            Point linesIntersection = null;
            Line currentSegmentLine = new Line(this);
            Line referenceSegmentLine = new Line(referenceSegment);
            try
            {
                linesIntersection = currentSegmentLine.FindIntersection
                    (referenceSegmentLine);
            }
            catch (ArgumentException)
            {
                intersection = null;
            }
            if (linesIntersection != null)
            {
                if (this.ContainsPoint(linesIntersection) ||
                        referenceSegment.ContainsPoint(linesIntersection))
                {
                    intersection = linesIntersection;
                }
                else intersection = null;
            }
            else intersection = null;

            return intersection;
        }
        public Point[] FindIntersections(Polygon polygon)
        {
            List<Point> tempList = new List<Point>();
            Point[] result;
            HashSet<Point> intersectionsFound = new HashSet<Point>();
            Segment[] polygonSegments = polygon.GetEdges();
            foreach(Segment segment in polygonSegments)
            {
                Point newPoint = this.FindIntersection(segment);
                if(newPoint!=null && !intersectionsFound.Contains(newPoint))
                {
                    intersectionsFound.Add(newPoint);
                    tempList.Add(newPoint);
                }
            }
            result = tempList.ToArray();
            return result;
        }
        public int FindIntersectionsNumber(Polygon polygon)
        {
            int intersectionsNumber;
            Point[] intersections = this.FindIntersections(polygon);
            intersectionsNumber = intersections.Length;
            return intersectionsNumber;
        }

        public decimal Length
        {
            get
            {
                decimal xDifference = EndPoint.x - StartingPoint.x;
                decimal yDifference = EndPoint.y - StartingPoint.y;
                decimal length = (decimal)Math.Sqrt(
                    (double)((xDifference * xDifference) * (yDifference * yDifference)));
                return length;
            }
        }
        public override string ToString()
        {
            string resultString = "{" + startingPoint + "," + endPoint + "}";
            return resultString;
        }
    }
    public class Line
    {
        private LineType type;
        private decimal slope;
        private decimal constantTerm;
        decimal yPosition;
        decimal xPosition;

        public decimal Slope { get => slope; set => slope = value; } 
        public decimal ConstantTerm { get => constantTerm; set => constantTerm = value; }
        public LineType Type { get => type; set => type = value; }

        public Line(decimal slope, decimal constantTerm)
        {
            if (slope == 0) this.Type = LineType.parallelToXAxis;
            else this.Type = LineType.linearFunction;
            this.Slope = slope;
            this.ConstantTerm = constantTerm;
        }
        public Line(Point pointOne, Point pointTwo) : this(new Segment(pointOne, pointTwo))
        {
        }
        public Line(Segment segment)
        {
            Point point1 = segment.StartingPoint;
            Point point2 = segment.EndPoint;
            if (point1.Equals(point2)) throw new ArgumentException();

            decimal xDifference = point2.x - point1.x;
            decimal yDifference = point2.y - point1.y;

            if (xDifference == 0)
            {
                this.Type = LineType.parallelToYAxis;
                this.xPosition = point1.y;
                return;
            }
            if (yDifference == 0)
            {
                this.Type = LineType.parallelToXAxis;
                this.yPosition = point1.x;
                this.Slope = 0;
                this.ConstantTerm = xPosition;
                return;
            }
            if ((xDifference != 0) && (yDifference != 0))
            {
                decimal slope = yDifference / xDifference;
                decimal yInterceptValue = point1.y - (point1.x * slope);
                this.Slope = slope;
                this.ConstantTerm = yInterceptValue;
            }
        }
        private decimal GetValue(decimal x)
        {
            if (Type == LineType.parallelToYAxis)
            {
                throw new Exception();
            }
            decimal resultValue = (Slope * x) + ConstantTerm;
            return resultValue;
        }
        private decimal GetInverseValue(decimal y)
        {
            if (Slope == 0) throw new Exception();

            decimal inverseValue = (y - ConstantTerm) / Slope;
            return inverseValue;
        }
        public QuadrantPosition GetRelativeQuadrantPosition(Point entryPoint)
        {
            QuadrantPosition result;
            Line line = this;
            decimal pointX = entryPoint.x;
            decimal pointY = entryPoint.y;

            int pointIsAboveLine = pointY.CompareTo(line.GetValue(pointX));
            int pointIsToTheRightOfLine = pointX.CompareTo(line.GetInverseValue(pointY));
            if (line.Type == LineType.parallelToXAxis)
            {
                switch (pointIsAboveLine)
                {
                    case 1:
                        result = QuadrantPosition.yAxisUp;
                        break;
                    case -1:
                        result = QuadrantPosition.yAxisDown;
                        break;
                    case 0:
                        result = QuadrantPosition.Equals;
                        break;
                    default:
                        throw new Exception();
                }
                return result;
            }
            if (line.Type == LineType.parallelToYAxis)
            {
                switch (pointIsToTheRightOfLine)
                {
                    case 1:
                        result = QuadrantPosition.xAxisRight;
                        break;
                    case -1:
                        result = QuadrantPosition.xAxisLeft;
                        break;
                    case 0:
                        result = QuadrantPosition.Equals;
                        break;
                    default:
                        throw new Exception();
                }
                return result;
            }

            int slopeIsPositive = line.slope.CompareTo(0);
            switch (pointIsAboveLine)
            {
                case 1:
                    switch (slopeIsPositive)
                    {
                        case 1:
                            result = QuadrantPosition.IV;
                            break;
                        case -1:
                            result = QuadrantPosition.I;
                            break;
                        default:
                            throw new Exception();
                    }
                    break;
                case -1:
                    switch (slopeIsPositive)
                    {
                        case 1:
                            result = QuadrantPosition.II;
                            break;
                        case -1:
                            result = QuadrantPosition.III;
                            break;
                        default:
                            throw new Exception();
                            break;
                    }
                    break;
                case 0:
                    result = QuadrantPosition.Equals;
                    break;
                default:
                    throw new Exception();
            }

            return result;
            
        }
        private double GetAngleFromXAxis()
        {
            double result;
            switch (this.Type)
            {
                case LineType.parallelToYAxis:
                    result = ((Math.PI) / 2);
                    break;
                case LineType.parallelToXAxis:
                    result = 0;
                    break;
                case LineType.linearFunction:
                    result = Math.Atan((double)Slope);
                    break;
                default:
                    throw new Exception();
            }
            return result;
        }
        public Segment BuildHeightFrom(Point startingPoint)
        {
            Segment resultHeight;
            Point pointOne;
            Point pointTwo;
            Point linePoint;
            switch (this.Type)
            {
                case LineType.parallelToXAxis:
                    decimal newXPosition = startingPoint.x;
                    linePoint = new Point
                        (newXPosition, yPosition);
                    resultHeight = new Segment(startingPoint, linePoint);
                    break;
                case LineType.parallelToYAxis:
                    decimal newYPosition = startingPoint.y;
                    linePoint = new Point
                        (newYPosition, xPosition);
                    resultHeight = new Segment(startingPoint, linePoint);
                    break;
                case LineType.linearFunction:
                    Point intersectionPosition =
                        FindHeightIntersectionPosition(startingPoint);
                    resultHeight = new Segment(startingPoint, intersectionPosition);
                    break;
                default:
                    throw new Exception();
            }
            return resultHeight;
        }
        public bool ContainsPoint(Point point)
        {
            switch (this.Type)
            {
                case LineType.parallelToXAxis:
                    if (point.y == this.yPosition) return true;
                    else return false;
                case LineType.parallelToYAxis:
                    if (point.x == this.xPosition) return true;
                    else return false;
                case LineType.linearFunction:
                    decimal valueOfThisLine = this.GetValue(point.x);
                    decimal pointY = point.y;
                    decimal absoluteDifference = Math.Abs(valueOfThisLine - pointY);

                    if (absoluteDifference<Convert.ToDecimal(0.1))
                    {
                        return true;
                    }
                    else return false;
                default:
                    throw new Exception();
            }
        }
        public decimal GetHeightFromPoint(Point point)
        {
            Segment newSection = this.BuildHeightFrom(point);
            decimal resultHeightLength = newSection.Length;
            return resultHeightLength;
        }

        private Point FindHeightIntersectionPosition
            (Point entryPoint)
        {
            Line line = this;
            double pointX = Convert.ToDouble(entryPoint.x);
            double pointY = Convert.ToDouble(entryPoint.y);
            double xDifference;
            double yDifference;
            decimal resultX;
            decimal resultY;
            Point intersectionPosition;

            if (!(line.Type == LineType.linearFunction))
            {

                if (line.Type == LineType.parallelToXAxis)
                {
                    intersectionPosition = new Point(Convert.ToDecimal(pointX), line.yPosition);
                    return intersectionPosition;
                }
                if (line.Type == LineType.parallelToYAxis)
                {
                    intersectionPosition = new Point(line.xPosition, Convert.ToDecimal(pointY));
                    return intersectionPosition;
                }
            }
            QuadrantPosition relativeQuadrantPositionOfPoint = GetRelativeQuadrantPosition(entryPoint);

            int yCoefficient;
            int xCoefficient;
            double yVectorOne;
            double yVectorTwo;
            double xVector;
            switch (relativeQuadrantPositionOfPoint)
            {
                case QuadrantPosition.I:
                    yCoefficient = -1;
                    xCoefficient = -1;
                    break;
                case QuadrantPosition.II:
                    yCoefficient = 1;
                    xCoefficient = -1;
                    break;
                case QuadrantPosition.III:
                    yCoefficient = 1;
                    xCoefficient = 1;
                    break;
                case QuadrantPosition.IV:
                    yCoefficient = -1;
                    xCoefficient = 1;
                    break;
                default:
                    throw new Exception();

            }

            double alphaAngle = Math.Abs(line.GetAngleFromXAxis());
            double alphaAngleSine = Math.Sin(alphaAngle);
            double alphaAngleCosine = Math.Cos(alphaAngle);
            yVectorOne = Math.Abs(pointY - Convert.ToDouble(line.GetValue(Convert.ToDecimal(pointX))));
            double lineCathetus = yVectorOne * alphaAngleSine;
            xVector = lineCathetus * alphaAngleCosine;
            yVectorTwo = lineCathetus * alphaAngleSine;

            xDifference = xCoefficient * xVector;
            yDifference = (yVectorOne - yVectorTwo) * yCoefficient;
            resultX = Convert.ToDecimal(pointX + xDifference);
            resultY = Convert.ToDecimal(pointY + yDifference);
            intersectionPosition = new Point(resultX, resultY);

            return intersectionPosition;
        }

        public Point FindIntersection(Line referenceLine)
        {
            bool thisLineIsParallelToYAxis = this.Type == LineType.parallelToYAxis;
            bool referenceLineIsParallelToYAxis =
                referenceLine.Type == LineType.parallelToYAxis;
            bool eitherLineIsParallelToYAxis = thisLineIsParallelToYAxis
                || referenceLineIsParallelToYAxis;

            decimal intersectionPointX;
            decimal intersectionPointY;
            Point intersectionPoint;
            if (this.Equals(referenceLine))
            {
                throw new ArgumentException();
            }
            else if(eitherLineIsParallelToYAxis)
            {
                if (thisLineIsParallelToYAxis && !referenceLineIsParallelToYAxis)
                {
                    intersectionPointX = this.xPosition;
                    intersectionPointY = referenceLine.GetValue(intersectionPointX);
                    intersectionPoint = new Point(intersectionPointX, intersectionPointY);
                }
                else if (!thisLineIsParallelToYAxis && referenceLineIsParallelToYAxis)
                {
                    intersectionPointX = referenceLine.xPosition;
                    intersectionPointY = this.GetValue(intersectionPointX);
                    intersectionPoint = new Point(intersectionPointX, intersectionPointY);
                }
                else if (thisLineIsParallelToYAxis && referenceLineIsParallelToYAxis)
                {
                    if (this.xPosition == referenceLine.xPosition)
                    {
                        throw new ArgumentException();
                    }
                    else
                    {
                        intersectionPoint = null;
                    }
                }
                else throw new Exception();
            }
            else
            {
                if (this.ConstantTerm == referenceLine.ConstantTerm)
                {
                    intersectionPointY = this.ConstantTerm;
                    intersectionPoint = new Point(0, intersectionPointY);
                }
                else
                {
                    if (this.Slope == referenceLine.Slope)
                    {
                        intersectionPoint = null;
                    }
                    else
                    {
                        intersectionPointX = -((this.ConstantTerm - referenceLine.ConstantTerm) / (this.Slope - referenceLine.Slope));
                        intersectionPointY = this.GetValue(intersectionPointX);
                        intersectionPoint = new Point(intersectionPointX, intersectionPointY);
                    }
                }
            }
                
            return intersectionPoint;
        }
        internal Line BuildPerpendicular(Point intersection)
        {
            Line resultLine;
            Point newPoint;
            decimal sampleLength = 100;
            decimal xDifference;
            decimal yDifference;
            double lineAngle = this.GetAngleFromXAxis();
            double perpendicularAngle;
            double coefficient;
            if (lineAngle > 0)
            {
                coefficient = -1;
            }
            else
            {
                coefficient = 1;
            }
            perpendicularAngle = lineAngle + (Math.PI / 2) * coefficient;
            xDifference = ((decimal)Math.Cos(perpendicularAngle)) * sampleLength;
            yDifference = ((decimal)Math.Sin(perpendicularAngle)) * sampleLength;

            newPoint = new Point(intersection.x + xDifference, intersection.y + yDifference);
            Segment sampleSection = new Segment(newPoint, intersection);
            resultLine = new Line(sampleSection);
            return resultLine;
        }
        public override bool Equals(object obj)
        {
            return this.Equals((Line)(obj));
        }
        public bool Equals(Line referenceLine)
        {
            bool result;
            bool typesAreIncomparable =
                ((this.Type == LineType.parallelToYAxis &&
                referenceLine.Type != LineType.parallelToYAxis)
                || (this.Type != LineType.parallelToYAxis &&
                referenceLine.Type == LineType.parallelToYAxis));
            bool bothLinesAreParallelToYAxis =
                this.Type == LineType.parallelToYAxis
                    && referenceLine.Type == LineType.parallelToYAxis;

            if (this.Type != LineType.parallelToYAxis
                    && referenceLine.Type != LineType.parallelToYAxis)
            {
                if (this.Slope == referenceLine.Slope && this.ConstantTerm == referenceLine.ConstantTerm)
                {
                    result = true;
                }
                else result = false;
            }
            else if (typesAreIncomparable)
            {
                return false;
            }
            else if (bothLinesAreParallelToYAxis)
            {
                if (this.xPosition == referenceLine.xPosition)
                {
                    result = true;
                }
                else result = false;
            }
            else result = false;

            return result;
        }
        public override int GetHashCode()
        {
            int resultHashCode = (Slope.GetHashCode() + ConstantTerm).GetHashCode();
            return resultHashCode;
        }
    }
    public class Polygon
    {
        DLList<Point> vertices;
        public Polygon(params Point[] points)
        {
            if (!InputIsValid(points))
            {
                throw new ArgumentException();
            }
            vertices = new DLList<Point>();
            for (int i = 0; i < points.Length; i++)
            {
                Point currentElement = points[i];
                vertices.AddLast(currentElement);
            }
        }
        public Polygon(string jsonString)
        {
            Point[] points;
            try
            {
                points = Newtonsoft.Json.JsonConvert.DeserializeObject<Point[]>(jsonString);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            vertices = new DLList<Point>();
            for (int i = 0; i < points.Length; i++)
            {
                Point currentElement = points[i];
                vertices.AddLast(currentElement);
            }
        }

        // needs a check for a lack of intersection between edges
        public Segment[] GetEdges()
        {
            List<Segment> edges = new List<Segment>();
            Segment[] resultEdges;
            DLListNode<Point> currentNode = vertices.First;
            Point firstVertice = currentNode.Value;
            DLListNode<Point> lastNode = vertices.Last;
            Point lastVertice = lastNode.Value;
            Segment finishingLink =
                new Segment(lastVertice, firstVertice);

            Point currentValue = currentNode.Value;

            DLListNode<Point> previousNode;
            Point previousValue;
            Segment currentEdge;

            while (currentNode.Next != null)
            {
                previousNode = currentNode;
                previousValue = currentValue;
                currentNode = currentNode.Next;
                currentValue = currentNode.Value;
                currentEdge = new Segment(previousValue, currentValue);
                edges.Add(currentEdge);
            }
            edges.Add(finishingLink);
            resultEdges = edges.ToArray();
            return resultEdges;
        }
        public Point[] GetPoints()
        {
            List<Point> pointsList = new List<Point>();
            foreach (Point vertice in vertices)
            {
                pointsList.Add(vertice);
            }
            Point[] pointsArray = pointsList.ToArray();
            return pointsArray;
        }
        public void AddPoint(Point entry, Point neighborOne, Point neighborTwo)
        {
            DLListNode<Point> currentNode = vertices.First;
            DLListNode<Point> nextNode;
            Point nextPoint;
            Point currentPoint;
            while (currentNode.Next != null)
            {
                nextNode = currentNode.Next;
                currentPoint = currentNode.Value;
                nextPoint = nextNode.Value;

                if(currentPoint.Equals(neighborOne) && nextPoint.Equals(neighborTwo)
                        || currentPoint.Equals(neighborTwo) && nextPoint.Equals(neighborOne))
                {
                    DLListNode<Point> newNode = new DLListNode<Point>(entry, currentNode, nextNode);
                    currentNode.Next = newNode;
                    nextNode.Previous = newNode;
                    return;
                }

                else
                {
                    currentNode = nextNode;
                }
            }
            if (currentNode.Next == null)
            {
                nextNode = vertices.First;
                currentPoint = currentNode.Value;
                nextPoint = nextNode.Value;

                if (currentPoint.Equals(neighborOne) && nextPoint.Equals(neighborTwo)
                        || currentPoint.Equals(neighborTwo) && nextPoint.Equals(neighborOne))
                {
                    DLListNode<Point> newNode = new DLListNode<Point>(entry, currentNode, null);
                    vertices.AddLast(newNode);
                    return;
                }
            }
        }
        public Tuple<Point, Point> GetNeighbors(Point entryPoint)
        {
            if (!this.ContainsPoint(entryPoint))
            {
                throw new ArgumentException("Entry value" +
                    "was not found.");
            }

            Point neighborOne;
            Point neighborTwo;

            DLListNode<Point> currentNode = vertices.First;
            Point currentValue = currentNode.Value;
            while (currentNode != null)
            {
                if (currentValue.Equals(entryPoint))
                {
                    if (currentNode == vertices.First)
                    {
                        neighborOne = vertices.Last.Value;
                        neighborTwo = currentNode.Next.Value;
                    }
                    if (currentNode == vertices.Last)
                    {
                        neighborOne = currentNode.Previous.Value;
                        neighborTwo = vertices.First.Value;
                    }
                    else
                    {
                        neighborOne = currentNode.Previous.Value;
                        neighborTwo = currentNode.Next.Value;
                    }
                    Tuple<Point, Point> resultTuple = new Tuple<Point, Point>
                        (neighborOne, neighborTwo);
                    return resultTuple;
                }
                else continue;
            }
            throw new Exception();
        }
        public bool ContainsPoint(Point entryPoint)
        {
            foreach (Point point in vertices)
            {
                if (point.Equals(entryPoint)) return true;
            }
            return false;
        }
        public bool SurroundsPoint(Point entryPoint)
        {
            double totalRotation = 0;
            Point[] points = this.GetPoints();
            int pointsLength = points.Length;

            double currentStepRotationAngle;
            for (int i = 0; i < pointsLength; i++)
            {
                int nextPointId;
                if (i < (pointsLength - 1))
                {
                    nextPointId = i + 1;
                }
                else if (i == pointsLength - 1)
                {
                    nextPointId = 0;
                }
                else throw new Exception();
                Step tempStep = new Step(points[i], points[nextPointId]);
                currentStepRotationAngle = entryPoint.GetRelativeRotationAngle(tempStep);
                totalRotation += currentStepRotationAngle;
            }
            if (Math.Abs(totalRotation) > Math.PI * 1.75)
            {
                return true;
            }
            else return false;
        }
        public bool InputIsValid(params Point[] points)
        {
            if (points.Length < 3)
            {
                throw new ArgumentException();
            }
            HashSet<Point> pointsSet = new HashSet<Point>();
            for (int i = 0; i < points.Length - 1; i++)
            {
                Point currentElement = new Point(points[i]);
                if (!pointsSet.Contains(currentElement))
                {
                    pointsSet.Add(currentElement);
                }
                else return false;
            }
            return true;
        }
        public override string ToString()
        {
            Point[] points = this.GetPoints();
            string jsonString = System.Text.Json.JsonSerializer.Serialize(points);
            return jsonString;
        }

    }
    public static class Angle
    {
        public static string FactorOfPi(double radianNumber)
        {
            double piFactor = radianNumber / (Math.PI);
            string resultString = piFactor + "*Pi ";
            return resultString;
        }

        static decimal ConvertRadsToDegrees(decimal radianNumber)
        {
            decimal degreesValue;
            decimal radToDegreeRatio = (decimal)360 / ((decimal)(2 * (decimal)Math.PI));
            degreesValue = radianNumber * radToDegreeRatio;
            return degreesValue;
        }
    }
    public class Step
    {
        Point startingPoint;
        Point endPoint;
        public Step(Point startingPoint, Point endPoint)
        {
            this.StartingPoint = startingPoint;
            this.EndPoint = endPoint;
        }

        public Point StartingPoint { get => startingPoint; set => startingPoint = value; }
        public Point EndPoint { get => endPoint; set => endPoint = value; }
        public Segment ToSegment()
        {
            Segment newSegment = new Segment(StartingPoint, EndPoint);
            return newSegment;
        }
        public override string ToString()
        {
            string resultString = "[" + StartingPoint + "->" + EndPoint + "]";
            return resultString;
        }
    }

    public enum LineType
    {
        linearFunction, parallelToYAxis, parallelToXAxis
    }
    public class NumeralInterval
    {
        decimal startingPoint;
        decimal endPoint;
        bool includingStartingPoint;
        bool includingEndPoint;
        bool isEmpty;

        public NumeralInterval(decimal value1, bool includingPoint1, decimal value2, bool includingPoint2)
        {
            decimal precisionLimit = (decimal)(Math.Pow(10, -28));
            int comparison = value1.CompareTo(value2);
            switch (comparison)
            {
                case 1:
                    startingPoint = value2;
                    endPoint = value1;
                    if (!includingStartingPoint)
                    {
                        startingPoint += precisionLimit;
                    }
                    if (!includingEndPoint)
                    {
                        endPoint -= precisionLimit;
                    }

                    break;
                case 0:
                    if (includingPoint1 != includingPoint2)
                    {
                        throw new ArgumentException();
                    }
                    if (includingPoint1 != true)
                    {
                        this.isEmpty = true;
                    }
                    else
                    {
                        this.startingPoint = value1;
                        this.endPoint = value1;
                    }
                    break;
                case -1:
                    startingPoint = value1;
                    endPoint = value2;
                    includingStartingPoint = includingPoint1;
                    includingEndPoint = includingPoint2;
                    if (!includingStartingPoint)
                    {
                        startingPoint += precisionLimit;
                    }
                    if (!includingEndPoint)
                    {
                        endPoint -= precisionLimit;
                    }
                    break;

                default:
                    throw new Exception();
            }
        }
        public bool Contains(decimal query)
        {
            bool result;
            if (query >= startingPoint && query <= endPoint)
            {
                result = true;
            }
            else result = false;
            return result;
        }
        public bool IsEmpty()
        {
            return this.isEmpty;
        }
        public bool Intersects(NumeralInterval referenceInterval)
        {
            bool result;
            if (this.IsEmpty() || referenceInterval.IsEmpty())  
            {
                result = false;
            }

            if (this.endPoint < referenceInterval.startingPoint ||
                    this.startingPoint > referenceInterval.endPoint)
            {
                return false;
            }
            else
            {
                return true;
            }
            return false;
        }
    }
    public enum QuadrantPosition
    {
        I, II, III, IV, xAxisRight, xAxisLeft, yAxisUp, yAxisDown, Equals
    }
    public enum RotationDirection
    {
        clockwise, counterclockwise
    }    

}
