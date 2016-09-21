using System;
using System.Collections.Generic;
using System.Text;
using Tao.OpenGl;
using System.IO; 
using ShadowEngine.ContentLoading; 

namespace ShadowEngine
{
    static public class Collision
    {
        #region private attributes

        static List<CollisionPoint> colitionPoints = new List<CollisionPoint>();
        static List<CollisionSegment> colitionSegments = new List<CollisionSegment>();
        static int updateFreq = 10;
        static int counter;
        static bool ghostMode;
        static int id;

        #endregion

        #region Properties

        public static bool GhostMode
        {
            get { return Collision.ghostMode; }
            set { Collision.ghostMode = value; }
        }

        #endregion

        public static int GenerateCollitionId()
        {
            return (id++);
        }

        public static void AddCollisionPointList(List<CollisionPoint> colitionPointList)
        {
            Collision.colitionPoints.AddRange(colitionPointList);     
        }

        public static void AddCollisionSegmentList(List<CollisionPoint> colitionSegmentList)
        {
            Collision.colitionPoints.AddRange(colitionSegmentList );
        }
        
        public static void AddCollisionPoint(CollisionPoint point)
        {
            colitionPoints.Add(point);  
        }

        public static void AddCollisionSegment(CollisionSegment segment)
        {
            colitionSegments.Add(segment);    
        }

        public static void AddCollisionSegment(Point3D first, Point3D second, float colitionDistance)
        {
            Segment cSegment = new Segment(first, second);
            CollisionSegment cCollisionSegment = new CollisionSegment();
            cCollisionSegment.segment = cSegment;
            cCollisionSegment.ColitionDistance = colitionDistance;
            cCollisionSegment.Enabled(true);   
            colitionSegments.Add(cCollisionSegment);
        }

        public static void AddCollisionModel(GeometryModel m)
        {
            colitionPoints.AddRange(m.ColitionPoints);
            colitionSegments.AddRange(m.ColitionSegments);   
        }

        public static void Update(Point3D camaraPos)
        {
            counter++;
            if (counter == updateFreq)
            {
                counter = 0;
            }
        }

        public static void EnableCollisionSegment(int id, bool enabled)
        {
            foreach (var item in colitionSegments)
            {
                if (item.objectID == id)
                {
                    item.Enabled(enabled); 
                }
            }
        }

        public static Point3D CheckCollisionWithMove(Point3D camaraPos)
        {
            if (!ghostMode) //if colitions are not disabled
            {
                foreach (var item in colitionSegments)
                {
                    if (item.segment.DistToSegment(camaraPos) < item.ColitionDistance)
                    {
                        //there was a collition and we return the posible move
                        return item.segment.PosibleMove(Globals.CamaraInc, Globals.CamaraYaw, camaraPos);
                    }
                }
                foreach (var item in colitionPoints)
                {
                    if (item.point.DistToPoint(camaraPos) < item.ColitionDistance)
                    {
                        return new Point3D(-1000, 0, 0);
                    }
                } 
            }            
             return new Point3D(-1000, 0, 0);; 
        }

        public static bool CheckCollision(Point3D camaraPos)
        {
            if (!ghostMode) //if colitions are not disabled
            {
                foreach (var item in colitionSegments)
                {
                    if (item.segment.DistToSegment(camaraPos) < item.ColitionDistance)
                    {
                        return true; 
                    }
                }
                foreach (var item in colitionPoints)
                {
                    if (item.point.DistToPoint(camaraPos) < item.ColitionDistance)
                    {
                        return true; 
                    }
                }
            }
            return false; 
        }

        public static void SaveColition(string filename, string line)
        {
            if (!File.Exists(filename))
            {
                FileStream f = File.Create(filename);
                f.Close(); 
            }
            string[] fileLine = File.ReadAllLines(filename);
            string[] fileNew = new string[fileLine.Length + 1];
            fileNew[0] = line;
            fileLine.CopyTo(fileNew, 1);
            File.WriteAllLines(filename, fileNew);    
        }

        public static GeometryModel LoadGeometryModel(string filename)
        {
            return new GeometryModel(); 
        }

        public static void DrawColissions()
        {
            Gl.glPushAttrib(Gl.GL_LINE_BIT);
            Gl.glLineWidth(3); 
            Gl.glBegin(Gl.GL_LINES);
            foreach (var item in colitionSegments)
            {
                Gl.glVertex3f(item.segment.first.x, 1, item.segment.first.y);
                Gl.glVertex3f(item.segment.second.x, 1, item.segment.second.y);
            } 
            Gl.glEnd();
            Gl.glPopAttrib();  
        }
    }
}
