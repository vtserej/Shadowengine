using System;
using System.Collections.Generic;
using System.Text;
using Tao.OpenGl; 
using System.Drawing; 

namespace ShadowEngine.OpenGL
{
    public static class OpenGLUtils
    {
        static double[] modelMatrix = new double[16];
        static double[] projMatrix = new double[16];
        static double winx = 0, winy = 0, winz = 0;

        public static Point3D WorldPos()
        {       
            double objx, objy, objz;
            int[] viewport = new int[4];
            
            Gl.glGetDoublev(Gl.GL_MODELVIEW_MATRIX, modelMatrix);
            Gl.glGetDoublev(Gl.GL_PROJECTION_MATRIX, projMatrix);

            Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport);
            winx = viewport[2] * 0.5f + viewport[0];
            winy = viewport[3] * 0.5f + viewport[1];

            Glu.gluUnProject(winx, winy, winz, modelMatrix, projMatrix,
                             viewport, out objx, out objy, out objz);

            return new Point3D((float)objx, (float)objy, (float)objz); 
        }

        public static void Scissor(Rectangle rect)
        {
            Gl.glScissor(rect.X, Globals.ScreenHeight - (rect.Y + rect.Height),
               rect.Width, rect.Height);
        }

        public static void DrawCube(int width, int height, int length, int x, int y, int z, string[] textures)
        {
            //back, top, front, right, left, bottom

            x = x - width / 2;
            z = z - length / 2;

            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, ContentManager.GetTextureByName(textures[0]));

            Gl.glBegin(Gl.GL_QUADS);
            Gl.glNormal3d(-1, 1, 1);
            Gl.glTexCoord2f(1.0f, 1.0f); Gl.glVertex3d(x + width, y, z);
            Gl.glNormal3d(-1, -1, 1);
            Gl.glTexCoord2f(1.0f, 0.0f); Gl.glVertex3d(x + width, y + height, z);
            Gl.glNormal3d(1, -1, 1);
            Gl.glTexCoord2f(0.0f, 0.0f); Gl.glVertex3d(x, y + height, z);
            Gl.glNormal3d(1, 1, 1);
            Gl.glTexCoord2f(0.0f, 1.0f); Gl.glVertex3d(x, y, z);
            Gl.glEnd();

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, ContentManager.GetTextureByName(textures[1]));
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glNormal3d(-1, -1, 1);
            Gl.glTexCoord2f(0.0f, 1.0f); Gl.glVertex3d(x + width, y + height - 0.5f, z);
            Gl.glNormal3d(-1, -1, -1);
            Gl.glTexCoord2f(0.0f, 0.0f); Gl.glVertex3d(x + width, y + height - 0.5f, z + length);
            Gl.glNormal3d(1, -1, -1);
            Gl.glTexCoord2f(1.0f, 0.0f); Gl.glVertex3d(x, y + height - 0.5f, z + length);
            Gl.glNormal3d(1, -1, 1);
            Gl.glTexCoord2f(1.0f, 1.0f); Gl.glVertex3d(x, y + height - 0.5f, z);
            Gl.glEnd();

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, ContentManager.GetTextureByName(textures[2]));
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glNormal3d(1, 1, -1);
            Gl.glTexCoord2f(0.0f, 1.0f); Gl.glVertex3d(x, y, z + length);
            Gl.glNormal3d(1, -1, -1);
            Gl.glTexCoord2f(0.0f, 0.0f); Gl.glVertex3d(x, y + height, z + length);
            Gl.glNormal3d(-1, -1, -1);
            Gl.glTexCoord2f(1.0f, 0.0f); Gl.glVertex3d(x + width, y + height, z + length);
            Gl.glNormal3d(-1, 1, -1);
            Gl.glTexCoord2f(1.0f, 1.0f); Gl.glVertex3d(x + width, y, z + length);
            Gl.glEnd();

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, ContentManager.GetTextureByName(textures[3]));
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glNormal3d(1, -1, 1);
            Gl.glTexCoord2f(0.0f, 0.0f); Gl.glVertex3d(x, y + height, z);
            Gl.glNormal3d(1, -1, -1);
            Gl.glTexCoord2f(1.0f, 0.0f); Gl.glVertex3d(x, y + height, z + length);
            Gl.glNormal3d(1, 1, -1);
            Gl.glTexCoord2f(1.0f, 1.0f); Gl.glVertex3d(x, y, z + length);
            Gl.glNormal3d(1, 1, 1);
            Gl.glTexCoord2f(0.0f, 1.0f); Gl.glVertex3d(x, y, z);
            Gl.glEnd();

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, ContentManager.GetTextureByName(textures[4]));
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glNormal3d(-1, 1, 1);
            Gl.glTexCoord2f(1.0f, 1.0f); Gl.glVertex3d(x + width, y, z);
            Gl.glNormal3d(-1, 1, -1);
            Gl.glTexCoord2f(0.0f, 1.0f); Gl.glVertex3d(x + width, y, z + length);
            Gl.glNormal3d(-1, -1, -1);
            Gl.glTexCoord2f(0.0f, 0.0f); Gl.glVertex3d(x + width, y + height, z + length);
            Gl.glNormal3d(-1, -1, 1);
            Gl.glTexCoord2f(1.0f, 0.0f); Gl.glVertex3d(x + width, y + height, z);
            Gl.glEnd();

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, ContentManager.GetTextureByName(textures[5]));
            Gl.glBegin(Gl.GL_QUADS);

            // Assign the texture coordinates and vertices for the BOTTOM Side
            Gl.glNormal3d(1, 1, 1);
            Gl.glTexCoord2f(1.0f, 0.0f); Gl.glVertex3d(x, y, z);
            Gl.glNormal3d(1, 1, -1);
            Gl.glTexCoord2f(1.0f, 1.0f); Gl.glVertex3d(x, y, z + length);
            Gl.glNormal3d(-1, 1, -1);
            Gl.glTexCoord2f(0.0f, 1.0f); Gl.glVertex3d(x + width, y, z + length);
            Gl.glNormal3d(-1, 1, 1);
            Gl.glTexCoord2f(0.0f, 0.0f); Gl.glVertex3d(x + width, y, z);
            Gl.glEnd();
        }
    }
}
