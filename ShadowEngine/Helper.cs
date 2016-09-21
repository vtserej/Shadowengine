using System;
using System.Collections.Generic;
using System.Text;

namespace ShadowEngine
{
    public static class Helper
    {
        #region Private atributes

        static Random randomizer = new Random();
        const double piOver2 = Math.PI / 2;
        const double piOver4 = Math.PI / 4;
        const double div1 = Math.PI / 180;
        const double div2 = 180 / Math.PI;

        #endregion 

        /// <summary>
        /// This function  returns pi over 2,this is done this way to
        /// improve some calculations
        /// <summary>
        public static double PiOver2()
        {
            return piOver2; 
        }

        /// <summary>
        /// This function  returns pi over 4,this is done this way to
        /// improve some calculations
        /// <summary>
        public static double PiOver4()
        {
            return piOver4; 
        }

        /// <summary>
        /// This function random returns 1 or -1
        /// <summary>
        static public int Alternator()
        {
            return (int)Math.Pow((-1), randomizer.Next());
        }

        /// <summary>
        /// Converts from degree to radian
        /// <summary>
        static public float DegreeToRad(double angle)
        {
            return (float)(angle * div1);
        }

        /// <summary>
        /// Converts from radian to degree
        /// <summary>
        static public float RadToDegree(double angle)
        {
            return (float)(angle * div2);
        }

        /// <summary>
        /// This function comapares two arrays of bytes
        /// </summary>
        public static bool Compare(byte[] array1, byte[] array2)
        {
            int length1 = array1.Length;
            int length2 = array2.Length;
            if (length1 != length2)
            {
                return false;
            }
            // if continue the 2 lenghts are the same
            for (int i = 0; i < length1; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
