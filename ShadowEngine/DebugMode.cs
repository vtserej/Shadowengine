using System;
using System.Collections.Generic;
using System.Text;
using Tao.OpenGl;
using System.Drawing;

namespace ShadowEngine
{
    static public class DebugMode
    {
        public static void WriteCamaraPos(int x, int y)
        {
            Point3D point = OpenGL.OpenGLUtils.WorldPos();
            Sprite.Begin();
            string text = point.x.ToString() + " " + point.y.ToString() + " " + point.z.ToString();
            Sprite.DrawText(x, y, text, Glut.GLUT_BITMAP_TIMES_ROMAN_24, Color.Red);
            Sprite.End();
        }

        public static void WriteDirectionVectors()
        {
            throw new NotImplementedException(); 
        }
    }
}
