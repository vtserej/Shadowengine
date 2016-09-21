using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.IO;  

namespace ShadowEngine.ContentLoading
{
    public class GeometryModel
    {
        List<CollisionPoint> colitionPoints = new List<CollisionPoint>();
        List<CollisionSegment> colitionSegments = new List<CollisionSegment>();
        static NumberFormatInfo provider = new NumberFormatInfo();

        #region properties

        public  List<CollisionPoint> ColitionPoints
        {
            get { return colitionPoints; }
            set { colitionPoints = value; }
        }

        public  List<CollisionSegment> ColitionSegments
        {
            get { return colitionSegments; }
            set { colitionSegments = value; }
        }

        #endregion

        public GeometryModel()
        {
            provider.NumberDecimalSeparator = ",";
        }

        public  void AddCollisionPointList(List<CollisionPoint> colitionPointList)
        {
            colitionPoints.AddRange(colitionPointList);
        }

        public void AddCollisionSegmentList(List<CollisionPoint> colitionSegmentList)
        {
            colitionPoints.AddRange(colitionSegmentList);
        }

        public  void AddCollisionPoint(CollisionPoint point)
        {
            colitionPoints.Add(point);
        }

        public  void AddCollisionSegment(CollisionSegment segment)
        {
            colitionSegments.Add(segment);
        }

        public  void AddCollisionSegment(Point3D first, Point3D second, float colitionDistance)
        {
            Segment cSegment = new Segment(first, second);
            CollisionSegment cCollisionSegment = new CollisionSegment();
            cCollisionSegment.segment = cSegment;
            cCollisionSegment.ColitionDistance = colitionDistance;
            cCollisionSegment.Enabled(true);
            colitionSegments.Add(cCollisionSegment);
        }

        public static GeometryModel LoadGeometryFile(string gFile)
        {
            GeometryModel m = new GeometryModel();  
            string[] lines = File.ReadAllLines(gFile);
            string[] numbers;
            char[] separator = {' '};
            for (int i = 0; i < lines.Length; i++)
            {
                numbers = lines[i].Split(separator);
                float x1 = (float)Convert.ToDouble(numbers[0], provider);
                float y1 = (float)Convert.ToDouble(numbers[1], provider);
                float x2 = (float)Convert.ToDouble(numbers[2], provider);
                float y2 = (float)Convert.ToDouble(numbers[3], provider);
                m.AddCollisionSegment(new Point3D(x1, y1, 0), new Point3D(x2, y2, 0), 0.3f); 
            }
            return m;
        }
    }
}
