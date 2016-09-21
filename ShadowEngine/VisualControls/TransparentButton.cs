using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Tao.OpenGl;

namespace ShadowEngine.VisualControls
{
    public class TransparentButton : Control, IControl 
    {
        bool isPressed;
        string text = "";
        string textHoverS, texPressedS, texNormalS;
        int textHover, texPressed, texNormal, texCurrent;
        int textWidth;
        Vector2 textPosition = new Vector2(0, 0);
        float transparency = 1;
        int textDisplayList;


        public TransparentButton(Rectangle rectangle, string text, OnClick onClick, float transparency)
        {
            base.zOrder = 0;
            this.text = text;
            this.areaRectangle = rectangle;
            base.onClick = onClick;
            texNormalS = "normal.jpg";
            texPressedS = "pressed.jpg";
            textHoverS = "hover.jpg";
            texCurrent = texNormal;
            textWidth = Sprite.FontWidth(Glut.GLUT_BITMAP_HELVETICA_18, text);
            textPosition = new Vector2(areaRectangle.X + (areaRectangle.Width - textWidth) / 2,
                                        areaRectangle.Y + ((areaRectangle.Height - 15) / 2) + 14);
            this.textFont = Glut.GLUT_BITMAP_HELVETICA_18;

        }

        public TransparentButton(Rectangle rectangle, OnClick onClick, string texHover,
                      string texPressed, string texNormal, IntPtr textFont, string text)
        {
            base.zOrder = 0;
            this.areaRectangle = rectangle;
            this.onClick = onClick;
            this.texNormalS = texNormal;
            this.texPressedS = texPressed;
            this.textHoverS = texHover;
            this.textFont = textFont;
            this.text = text;
            textWidth = Sprite.FontWidth(Glut.GLUT_BITMAP_HELVETICA_18, text);
            textPosition = new Vector2(areaRectangle.X + (areaRectangle.Width - textWidth) / 2,
                                        areaRectangle.Y + ((areaRectangle.Height - 15) / 2) + 14);
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
        {
            textHover = ContentManager.GetTextureByName(textHoverS);
            texPressed = ContentManager.GetTextureByName(texPressedS);
            texNormal = ContentManager.GetTextureByName(texNormalS);
            texCurrent = texNormal;
            textDisplayList = Sprite.GetTextDisplayList(textPosition.X, textPosition.Y, text, textFont);  
        }

        public void Update(Cursor cursor)
        {
            if (areaRectangle.Contains(cursor.Position))
            {
                texCurrent = textHover;
                if (cursor.State == Glut.GLUT_DOWN)
                {
                    isPressed = true;
                    texCurrent = texPressed;
                }
                else

                    if (isPressed)
                    {
                        if (cursor.State == Glut.GLUT_UP)
                        {
                            isPressed = false;
                            texCurrent = texNormal;
                            base.onClick.Invoke();
                        }
                    }
            }
            else
            {
                isPressed = false;
                texCurrent = texNormal;
            }
        }

        public void Draw()
        {
            //Gl.glColor4f(1, 1, 1, transparency);  
            Sprite.DrawAlphaSprite(areaRectangle, texCurrent); 
            Sprite.DrawDisplayListText(textDisplayList);   
        }
    }
}
