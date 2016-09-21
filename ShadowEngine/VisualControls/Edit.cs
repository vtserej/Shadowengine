using System;
using System.Collections.Generic;
using System.Text;
using ShadowEngine;
using ShadowEngine.Sound;
using Tao.OpenGl;
using System.Drawing;

namespace ShadowEngine.VisualControls
{
    public class Edit : Control, IControl
    {
        string text = "";
        bool pressed;
        int maxLenght;
        int textWidht;
        Vector2 textPosition;
        byte lastKey;
        int cursorCount;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public Edit(Vector2 pos, int width, int height, string name, string text)
        {
            base.areaRectangle = new Rectangle(pos.X, pos.Y, width, height);
            base.zOrder = 0;
            maxLenght = 15;
            textWidht = Sprite.FontWidth(Glut.GLUT_BITMAP_HELVETICA_12, text);
            textPosition = new Vector2(areaRectangle.X + 5,
                                        areaRectangle.Y + ((areaRectangle.Height - 15) / 2) + 15);
            accesibleName = name;
            this.text = text;
        }

        public string AccesibleName()
        {
            return base.accesibleName;
        }

        public bool Discard()
        {
            return base.discard;
        }

        public int ZOrder()
        {
            return base.zOrder;
        }

        public void Create()
        { }

        public void Update(Cursor cursor)
        {
            cursorCount += 2;
            byte key = (byte)Input.Key;
            if (Input.Pressed == false)
            {
                pressed = true;
            }
            if (lastKey != key || pressed == true)
            {
                if (((key > 96 && key < 123) || (key > 64 && key < 91)) && text.Length < maxLenght)
                {
                    text += (char)key;
                    pressed = false;
                }
                else
                    switch (key)
                    {
                        case 8: //backspace key
                            {
                                if (text.Length > 0)
                                {
                                    text = text.Remove(text.Length - 1);
                                    pressed = false;
                                }
                                break;
                            }
                        case 32:
                            {
                                if (Input.Pressed == true)
                                {
                                    text += " ";
                                    pressed = false;
                                }
                                break;
                            }
                        default:
                            break;
                    }
            }
            lastKey = key;
        }

        public void Draw()
        {
            Gl.glScissor(areaRectangle.X, Globals.ScreenHeight - (areaRectangle.Y + areaRectangle.Height),
                areaRectangle.Width, areaRectangle.Height);
            Sprite.DrawSprite(areaRectangle, Color.Black);
            //border of the control 
            Sprite.DrawSprite(areaRectangle, Color.White, Gl.GL_LINE_STRIP);
            Gl.glEnable(Gl.GL_SCISSOR_TEST); 
            if (cursorCount > 48)
            {
                Sprite.DrawText(textPosition.X, textPosition.Y, text + "|", Glut.GLUT_BITMAP_HELVETICA_18);
                if (cursorCount == 96)
                {
                    cursorCount = 0;
                }
            }
            else
            {
                Sprite.DrawText(textPosition.X, textPosition.Y, text, Glut.GLUT_BITMAP_HELVETICA_18);
            }
            Gl.glDisable(Gl.GL_SCISSOR_TEST);
        }
    }
}
