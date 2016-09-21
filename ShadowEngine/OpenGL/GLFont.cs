using System;
using System.Collections.Generic;
using System.Text;
using Tao.OpenGl;
using System.Drawing; 

namespace ShadowEngine
{
    static public class GLFont
    {
        static int baseList;		                // Base Display List For The Font Set
        static Dictionary<string, int> fonts = new Dictionary<string, int>();
        static Dictionary<string, Font> managedFonts = new Dictionary<string, Font>();
        static Graphics g;
        static StringFormat f; 

        #region constants
        
        const uint FALSE = 0;
        const int FW_BOLD = 700;
        const int ANSI_CHARSET = 0;
        const int DEFAULT_CHARSET = 1;
        const int OUT_TT_PRECIS = 4;
        const int CLIP_DEFAULT_PRECIS = 16;
        const int ANTIALIASED_QUALITY = 4;
        
        #endregion

        static GLFont()
        {
            g = Graphics.FromImage(new Bitmap(1024, 768));
        }

        static public void BuildFont(uint HDC, string fontName, int size)	// Build Our Bitmap Font
        {
            uint font;					// Windows Font ID
            uint oldfont;				// Used For Good House Keeping

            baseList = Gl.glGenLists(219);				              // Storage For 219 Characters
            font = (uint)new Font(fontName, size, GraphicsUnit.Pixel).ToHfont();   
            oldfont = WinApi.SelectObject(HDC, font);		          // Selects The Font We Want
            WinApi.wglUseFontBitmaps(HDC, 32, 219, (uint)baseList);   // Builds 219 Characters Starting At Character 32
            WinApi.SelectObject(HDC, oldfont);				          // Selects The Font We Want
            WinApi.DeleteObject(font);				                  // Delete The Font
            fonts.Add(fontName.ToLower() + "_" + size, baseList);
            managedFonts.Add(fontName.ToLower() + "_" + size, new Font(fontName, size));         
        }

        static public void KillFont()						    // Delete The Font List
        {
            Gl.glDeleteLists(baseList, 219);				// Delete All 96 Characters ( NEW )
        }

        static public int GetFontId(string fontName, int size)
        {
            return fonts[fontName + "_" + size];   
        }

        static public int GlSize(string font, int size, string text)	// Custom GL "Print" Routine
        {
            return (int)(g.MeasureString(text, managedFonts[font + "_" + size]).Width * 0.837f);
        }

        static public int GlSize(int font, string text)	// Custom GL "Print" Routine
        {
            Gl.glColorMask(0, 0, 0, 0); 
            GlPrint(font, text, 0, 0);
            int[] coordinates = new int[3];
            Gl.glGetIntegerv(Gl.GL_CURRENT_RASTER_POSITION, coordinates);
            Gl.glColorMask(1, 1, 1, 1);
            return coordinates[0];
        }

        static public void GlPrint(string fontName, int size, string text, int x, int y)	// Custom GL "Print" Routine
        {
            string font_size = fontName.ToLower() + "_" + size.ToString();
            baseList = fonts[font_size];
            Gl.glRasterPos2f(x, y);
            Gl.glPushAttrib(Gl.GL_LIST_BIT);		// Pushes The Display List Bits		( NEW )
            Gl.glListBase(baseList - 32);			// Sets The Base Character to 32	( NEW )
            Gl.glCallLists(text.Length, Gl.GL_UNSIGNED_BYTE, text);	// Draws The Display List Text	( NEW )
            Gl.glPopAttrib();						// Pops The Display List Bits	( NEW )
        }

        static public void DrawCenterText(int y, string text, string font,int size)
        {
            int x = GlSize(font, size, text);
            string font_size = font.ToLower() + "_" + size.ToString();
            baseList = fonts[font_size];
            x = Globals.ScreenWidthOver2 - (x / 2);
            Gl.glRasterPos2f(x, y);
            Gl.glPushAttrib(Gl.GL_LIST_BIT);		// Pushes The Display List Bits		( NEW )
            Gl.glListBase(baseList - 32);			// Sets The Base Character to 32	( NEW )
            Gl.glCallLists(text.Length, Gl.GL_UNSIGNED_BYTE, text);	// Draws The Display List Text	( NEW )
            Gl.glPopAttrib();						// Pops The Display List Bits	( NEW )
        }

        static public void GlPrint(int fontId, string text, int x, int y)	// Custom GL "Print" Routine
        {
            baseList = fontId; 
            Gl.glRasterPos2f(x, y);
            Gl.glPushAttrib(Gl.GL_LIST_BIT);		// Pushes The Display List Bits		( NEW )
            Gl.glListBase(baseList - 32);			// Sets The Base Character to 32	( NEW )
            Gl.glCallLists(text.Length, Gl.GL_UNSIGNED_BYTE, text);	// Draws The Display List Text	( NEW )
            Gl.glPopAttrib();						// Pops The Display List Bits	( NEW )
        }

        static public void DrawText(int x, int y, string text, int fontId, Color color)
        {
            Gl.glColor3ub(color.R, color.G, color.B);
            baseList = fontId;
            Gl.glRasterPos2f(x, y);
            Gl.glPushAttrib(Gl.GL_LIST_BIT);		// Pushes The Display List Bits		( NEW )
            Gl.glListBase(baseList - 32);			// Sets The Base Character to 32	( NEW )
            Gl.glCallLists(text.Length, Gl.GL_UNSIGNED_BYTE, text);	// Draws The Display List Text	( NEW )
            Gl.glPopAttrib();						// Pops The Display List Bits	( NEW )
            Gl.glColor3f(1, 1, 1); 
        }
    }
}
