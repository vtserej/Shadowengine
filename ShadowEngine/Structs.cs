using System;
using System.Collections.Generic;
using System.Text;

namespace ShadowEngine
{
    public struct Point3D
    {
        public float x, y, z;

        public Point3D(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z; 
        }

        public float DistToPoint(Point3D other)
        {
            return (float)Math.Sqrt((x - other.x) * (x - other.x) + (y - other.y) * (y - other.y));
        }

        /// <summary>
        /// This function  calculates 2D distances between two points
        /// <summary>
        public static float DistPointToPoint(Point3D first, Point3D second)
        {
            return (float)Math.Sqrt((first.x - second.x) * (first.x - second.x) + (first.y - second.y) * (first.y - second.y));
        }
    }

    public struct Segment
    {
        public Point3D first;
        public Point3D second;
        float angleSegment;
        float m1, m2;
        float n1;

        public Segment(Point3D first, Point3D second)
        {
            this.first = first;
            this.second = second;

            //evito que la m de la funcion sea 0 o indefinida
            if (first.y == second.y)
            {
                first.y += 0.001f; 
            }
            if (second.x == first.x)
            {
                second.x += 0.001f;  
            }
            //calculate the pendient of the function a the perpendicular
            //pendient
            m1 = (second.y - first.y) / (second.x - first.x);
            m2 = -1 / m1;
            //calculate the y intersection
            n1 = first.y - m1 * first.x;
            //calculate the angle with the x axis
            this.angleSegment = Helper.RadToDegree(Math.Atan((float)m1));  
        }

        /// <summary>
        /// This function  calculates the distance between a point
        /// and a segment
        /// <summary>
        public float DistToSegment(Point3D other)
        {
            //calculate the distances to each point
            //float dist1 = Point3D.DistPointToPoint(other, first);
            //float dist2 = Point3D.DistPointToPoint(other, second);

            //calculate the two linear functions
            float n2 = other.y - m2 * other.x;   //y = m2x + n   m2 = -1/m1

            //calculate the intersection point(i.p.)
            Point3D intersectionPoint = new Point3D();
            intersectionPoint.x = (n2 - n1) / (m1 - m2);
            intersectionPoint.y = m1 * intersectionPoint.x + n1;
            float d = Point3D.DistPointToPoint(intersectionPoint, other);

            //if the i.p. is contained in the rect segment return d else 
            //the minimun value between dist1 and dist2
            if ((intersectionPoint.x < first.x && intersectionPoint.x > second.x) ||
                (intersectionPoint.x > first.x && intersectionPoint.x < second.x))
            {
                return d;
            }
            else
                return 1000;//Math.Min(dist1, dist2);    
        }

        /// <summary>
        /// If you hit against a wall this method will give you and alternative point
        /// to move if there is one
        /// <summary>
        public Point3D PosibleMove(float inc, float angle, Point3D pos)
        {
            float xInc = 0;
            float yInc = 0;

            if (angle <= 90 && angle > 270)
            {
                xInc = inc * -(float)Math.Cos((float)angleSegment);
            }
            else
            {
                xInc = inc * (float)Math.Cos((float)angleSegment);
            }
            if (angle < 180 && angle > 0)
            {
                yInc = -inc * (float)Math.Sin((float)angleSegment);
            }
            else
            {
                yInc = inc * (float)Math.Sin((float)angleSegment);
            }
            return new Point3D(pos.x + xInc, pos.y + yInc, 0);
        }
    }

    public struct CollisionPoint
    {      
        public int objectID;
        public bool enabled;
        public Point3D point;
        float colitionDistance;

        public float ColitionDistance
        {
            get
            {
                if (enabled)
                {
                    return colitionDistance;
                }
                else
                {
                    return -1;
                }
            }
            set { colitionDistance = value; }
        }
    }

    public struct CollisionSegment
    {
        public int objectID;
        private bool enabled;
        public Segment segment;
        float colitionDistance;

        public void Enabled(bool enabled)
        {
            this.enabled = enabled; 
        }

        public float ColitionDistance
        {
            get {
                if (enabled)
                {
                    return colitionDistance;
                }
                else
                {
                    return -1;
                }
            }
            set { colitionDistance = value; }
        }
    }

    public struct Vector2
    {
        int x, y;

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public Vector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public struct Vector3
    {
        float x, y, z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public void Normalize()
        {
            float len = (float)Math.Sqrt(x * x + y * y + z * z);
            len = 1.0f / len;
            this.x *= len;
            this.y *= len;
            this.z *= len;
        }

        public static Vector3 Cross(Vector3 v1, Vector3 v2)
        {
            float x = ((v1.Y * v2.Z) - (v1.Z * v2.Y));
            float y = ((v1.Z * v2.X) - (v1.X * v2.Z));
            float z = ((v1.X * v2.Y) - (v1.Y * v2.X));
            return new Vector3(x, y, z);
        }

        public static Vector3 operator *(Vector3 first, int other)
        {
            return new Vector3(first.X * other, first.Y * other, first.Z * other);
        }

        public static Vector3 operator /(Vector3 first, ushort other)
        {
            return new Vector3(first.X / other, first.Y / other, first.Z / other);
        }

        public static Vector3 operator +(Vector3 first, Vector3 other)
        {
            return new Vector3(first.X + other.X, first.Y + other.Y, first.Z + other.Z);
        }

        public static Vector3 operator -(Vector3 first, Vector3 other)
        {
            return new Vector3(first.X - other.X, first.Y - other.Y, first.Z - other.Z);
        }

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public float Z
        {
            get { return z; }
            set { z = value; }
        }
    }

    public struct Matrix
    {
        float[,] values;

        public Matrix(float[,] values)
        {
            this.values = values;
        }

        public Matrix(float m11, float m12, float m13, float m14, float m21,
           float m22, float m23, float m24, float m31, float m32, float m33,
           float m34, float m41, float m42, float m43, float m44)
        {
            values = new float[4, 4];
            values[0, 0] = m11;
            values[0, 1] = m12;
            values[0, 2] = m13;
            values[0, 3] = m14;
            values[1, 0] = m21;
            values[1, 1] = m22;
            values[1, 2] = m23;
            values[1, 3] = m24;
            values[2, 0] = m31;
            values[2, 1] = m32;
            values[2, 2] = m33;
            values[2, 3] = m34;
            values[3, 0] = m41;
            values[3, 1] = m42;
            values[3, 2] = m43;
            values[3, 3] = m44;
        }

        public static Matrix Identity()
        {
            return new Matrix(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
        }

        public static Matrix CreateTranslation(float x, float y, float z)
        {
            return new Matrix(new float[4, 4]);
        }

        public static Matrix CreateScale(float x, float y, float z)
        {
            return new Matrix(new float[4, 4]);
        }

        public static Matrix CreateFromAxisAngle(Vector3 v, float r)
        {
            return new Matrix(new float[4, 4]);
        }

        public static Matrix operator *(Matrix first, Matrix other)
        {
            float[,] values = new float[4, 4];
            values[0, 0] = first.values[0, 0] * other.values[0, 0];
            values[0, 1] = first.values[1, 0] * other.values[0, 1];
            values[0, 2] = first.values[2, 0] * other.values[0, 2];
            values[0, 3] = first.values[3, 0] * other.values[0, 3];

            values[1, 0] = first.values[0, 1] * other.values[1, 0];
            values[1, 1] = first.values[1, 1] * other.values[1, 1];
            values[1, 2] = first.values[2, 1] * other.values[1, 2];
            values[1, 3] = first.values[3, 1] * other.values[1, 3];

            values[2, 0] = first.values[0, 2] * other.values[2, 0];
            values[2, 1] = first.values[1, 2] * other.values[2, 1];
            values[2, 2] = first.values[2, 2] * other.values[2, 2];
            values[2, 3] = first.values[3, 2] * other.values[2, 3];

            values[3, 0] = first.values[0, 3] * other.values[3, 0];
            values[3, 1] = first.values[1, 3] * other.values[3, 1];
            values[3, 2] = first.values[2, 3] * other.values[3, 2];
            values[3, 3] = first.values[3, 3] * other.values[3, 3];
            return new Matrix(values);
        }
    }
}
