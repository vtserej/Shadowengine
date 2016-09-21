using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Tao.OpenGl;

namespace ShadowEngine.VisualControls
{
    public class Message : Control, IControl
    {
        #region Private Atributes

        string textData;
        int time;
        Vector2 pos;
        IntPtr font;
        Color textColor;
        int number;
        static int mutex;
        int textBack;

        #endregion

        public Message(string text, int time, IntPtr font, Color color, int half, int texture)
        {
            mutex++;
            number = mutex;
            base.zOrder = 0;
            this.textData = text;
            this.time = time;
            this.font = font;
            this.textColor = color;
            areaRectangle.X = half - Sprite.FontWidth(font, text) / 2 - 15;
            areaRectangle.Y = Globals.ScreenHeightOver2 - 55;
            areaRectangle.Width = Sprite.FontWidth(font, text) + 30;
            areaRectangle.Height = 35;
            pos = new Vector2(half - Sprite.FontWidth(font, text) / 2, 354);
            textBack = texture;
        }

        public bool Discard()
        {
            return base.discard;
        }

        public int ZOrder()
        {
            return zOrder;
        }

        public string AccesibleName()
        {
            return accesibleName;
        }

        public void Create()
        { }

        public void Update(Cursor cursor)
        {
            if (time > 0 && mutex == number)
            {
                time--;
            }
        }

        public void Draw()
        {
            if (time > 0 && mutex == number)
            {
                Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);  // Enable Alpha Blending (disable alpha testing)
                Gl.glEnable(Gl.GL_BLEND);
                Sprite.DrawSprite(areaRectangle, textBack);
                Gl.glDisable(Gl.GL_BLEND);  
                if (textColor != null)
                {
                    Gl.glColor3ub(textColor.R, textColor.G, textColor.B);
                }
                Sprite.DrawText(pos.X, pos.Y, textData, font);
                Gl.glColor3f(1, 1, 1);
            }
        }
    }
}
