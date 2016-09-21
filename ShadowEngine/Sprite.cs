using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Tao.OpenGl;

namespace ShadowEngine
{
    /// <summary>
    /// This class provides 2D render operations
    /// <summary>
    static public class Sprite
    {
        static int beginList;
        static int endList;

        static public void Create()
        {
            beginList = Gl.glGenLists(1);
            Gl.glNewList(beginList, Gl.GL_COMPILE);
            Gl.glDisable(Gl.GL_DEPTH_TEST);
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glDisable(Gl.GL_DITHER);
            Gl.glPushMatrix();
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPushMatrix();
            Gl.glLoadIdentity();
            Gl.glOrtho(0, Globals.ScreenWidth, Globals.ScreenHeight, 0, -1, 1);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Gl.glEndList();

            endList = Gl.glGenLists(1);
            Gl.glNewList(endList, Gl.GL_COMPILE);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPopMatrix();
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPopMatrix();
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_DITHER);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glEndList(); 
        }

        static public void Begin()
        {
            Gl.glCallList(beginList);  
        }

        static public void End()
        {
           Gl.glCallList(endList);  
        }
        
        static public void DrawSprite(Rectangle rectangle, int texture)
        {
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture);  

            Gl.glBegin(Gl.GL_QUADS);

            Gl.glTexCoord2f(0, 1);
            Gl.glVertex2f(rectangle.X, rectangle.Y + rectangle.Height);

            Gl.glTexCoord2f(1, 1);
            Gl.glVertex2f(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);

            Gl.glTexCoord2f(1, 0);
            Gl.glVertex2f(rectangle.X + rectangle.Width, rectangle.Y);

            Gl.glTexCoord2f(0, 0);
            Gl.glVertex2f(rectangle.X, rectangle.Y);

            Gl.glEnd();

            Gl.glDisable(Gl.GL_TEXTURE_2D); 
        }

        static public void DrawSprite(Rectangle rectangle, Color color)
        {
            Gl.glBegin(Gl.GL_QUADS);

            Gl.glColor3ub(color.R, color.G, color.B);

            Gl.glVertex2f(rectangle.X, rectangle.Y + rectangle.Height);

            Gl.glVertex2f(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);

            Gl.glVertex2f(rectangle.X + rectangle.Width, rectangle.Y);

            Gl.glVertex2f(rectangle.X, rectangle.Y);

            Gl.glColor3f(1, 1, 1);

            Gl.glEnd();
        }

        static public void DrawSprite(Rectangle rectangle, Color color, int DrawMode)
        {
            Gl.glColor3ub(color.R, color.G, color.B);
            
            Gl.glBegin(DrawMode);

            Gl.glVertex2f(rectangle.X, rectangle.Y + rectangle.Height);

            Gl.glVertex2f(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);

            Gl.glVertex2f(rectangle.X + rectangle.Width, rectangle.Y);

            Gl.glVertex2f(rectangle.X, rectangle.Y);

            Gl.glVertex2f(rectangle.X, rectangle.Y + rectangle.Height);

            Gl.glColor3f(1, 1, 1);

            Gl.glEnd();
        }

        static public void DrawSprite(Rectangle rectangle, Color color, float lineWidth)
        {
            Gl.glPushAttrib(Gl.GL_LINE_BIT);

            Gl.glLineWidth(lineWidth);

            Gl.glColor3ub(color.R, color.G, color.B);

            Gl.glBegin(Gl.GL_LINE_STRIP);

            Gl.glVertex2f(rectangle.X, rectangle.Y + rectangle.Height);

            Gl.glVertex2f(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);

            Gl.glVertex2f(rectangle.X + rectangle.Width, rectangle.Y);

            Gl.glVertex2f(rectangle.X, rectangle.Y);

            Gl.glVertex2f(rectangle.X, rectangle.Y + rectangle.Height);

            Gl.glColor3f(1, 1, 1);

            Gl.glEnd();

            Gl.glPopAttrib();
        }

        static public void DrawAlphaSprite(Rectangle rectangle, int texture)
        {
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture);  
            
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glEnable(Gl.GL_BLEND);          

            Gl.glBegin(Gl.GL_QUADS);

            Gl.glTexCoord2f(0, 1);
            Gl.glVertex2f(rectangle.X, rectangle.Y + rectangle.Height);

            Gl.glTexCoord2f(1, 1);
            Gl.glVertex2f(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);

            Gl.glTexCoord2f(1, 0);
            Gl.glVertex2f(rectangle.X + rectangle.Width, rectangle.Y);

            Gl.glTexCoord2f(0, 0);
            Gl.glVertex2f(rectangle.X, rectangle.Y);

            Gl.glEnd();

            Gl.glDisable(Gl.GL_BLEND);
            Gl.glDisable(Gl.GL_TEXTURE_2D);
        }

        static public void DrawAlphaSprite(Rectangle rectangle, Color color, float alphaValue)
        {
            Gl.glBegin(Gl.GL_QUADS);

            Gl.glColor4f(color.R / 255f, color.G / 255f, color.B / 255f, alphaValue);

            Gl.glVertex2f(rectangle.X, rectangle.Y + rectangle.Height);

            Gl.glVertex2f(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);

            Gl.glVertex2f(rectangle.X + rectangle.Width, rectangle.Y);

            Gl.glVertex2f(rectangle.X, rectangle.Y);

            Gl.glColor4f(1, 1, 1, 1);

            Gl.glEnd();
        }

        static public void DrawText(int x, int y, string text, IntPtr font)
        {
            Gl.glPushAttrib(Gl.GL_CURRENT_BIT);
            Gl.glColor3f(1, 1, 1); 
            Gl.glRasterPos2f(x, y);
            int length = text.Length;
            for (int i = 0; i < length; i++)
            {
                Glut.glutBitmapCharacter(font, text[i]);
            }
            Gl.glPopAttrib();
        }

        static public Bitmap DrawTextBox(Rectangle areaRectangle,string text)
        {
            Rectangle r;
            if (text.Length < 90)
            {
                r = new Rectangle(0, 0, areaRectangle.Width, areaRectangle.Height);
            }
            else
            {
                r = new Rectangle(0, 0, areaRectangle.Width, (int)(areaRectangle.Height * 1.5f));
            }
            
            Bitmap b = new Bitmap(r.Width, r.Height);
            using (Graphics g = Graphics.FromImage(b))
            {
                Rectangle border = new Rectangle(0, 0, r.Width - 1, r.Height - 1);   
                g.FillRectangle(Brushes.Beige, r);
                g.DrawRectangle(Pens.Black, border);
                g.DrawString(text, new Font("verdana", 12), Brushes.Black, r);
            }
            b.RotateFlip(RotateFlipType.RotateNoneFlipY);   
            return b;
        }

        static public void DrawDisplayListText(int text)
        {
            Gl.glCallList(text);  
        }

        static public int GetTextDisplayList(int x, int y, string text, IntPtr font)
        {
            int textList = Gl.glGenLists(1);
            Gl.glNewList(textList, Gl.GL_COMPILE);
            Gl.glPushAttrib(Gl.GL_CURRENT_BIT);
            Gl.glColor3f(1, 1, 1);
            Gl.glRasterPos2f(x, y);
            int length = text.Length;
            for (int i = 0; i < length; i++)
            {
                Glut.glutBitmapCharacter(font, text[i]);
            }
            Gl.glPopAttrib();
            Gl.glEndList();
            return textList; 
        }

        static public void DrawText(int x, int y, string text, IntPtr font, Color color)
        {
            Gl.glColor3ub(color.R, color.G, color.B);
            Gl.glRasterPos2f(x, y);
            for (int i = 0; i < text.Length; i++)
            {
                Glut.glutBitmapCharacter(font, text[i]);
            }
            Gl.glColor3f(1, 1, 1); 
        }

        static public void DrawBoldText(int x, int y, string text, IntPtr font)
        {
            Gl.glRasterPos2f(x, y);
            for (int i = 0; i < text.Length; i++)
            {
                Glut.glutStrokeCharacter(font, text[i]);
            }
        }

        static public void DrawBoldCenterText(int y, string text, IntPtr font)
        {
            int x = Glut.glutStrokeLength(font, text);
            x = Globals.ScreenWidthOver2 - x / 2;
            Gl.glRasterPos2f(x, y);
            for (int i = 0; i < text.Length; i++)
            {
                Glut.glutBitmapCharacter(font, text[i]);
            }
        }

        static public void DrawCenterText(int y, string text, IntPtr font)
        {
            int x = Glut.glutBitmapLength(font, text);
            x = Globals.ScreenWidthOver2 - x / 2;
            Gl.glRasterPos2f(x, y);
            for (int i = 0; i < text.Length; i++)
            {
                Glut.glutBitmapCharacter(font, text[i]);
            }
        }

        static public void DrawCenterText(int y, string text, IntPtr font,Color color)
        {
            Gl.glColor3ub(color.R, color.G, color.B);
            int x = Glut.glutBitmapLength(font, text);
            x = Globals.ScreenWidthOver2 - x / 2;
            Gl.glRasterPos2f(x, y);
            for (int i = 0; i < text.Length; i++)
            {
                Glut.glutBitmapCharacter(font, text[i]);
            }
            Gl.glColor3f(1, 1, 1); 
        }

        static public int FontWidth(IntPtr font, string text)
        {
            return Glut.glutBitmapLength(font, text);
        }

        static public int FontBoldWidth(IntPtr font, string text)
        {
            return Glut.glutStrokeLength(font, text);
        }
    }
}
