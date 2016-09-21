using System;
using Tao.OpenGl;
using System.Drawing;
using ShadowEngine.Sound; 

namespace ShadowEngine.VisualControls
{
    public class Button : Control, IControl 
    {
        bool isPressed;
        string text = "";
        string textHoverS, texPressedS, texNormalS;
        int textHover, texPressed, texNormal, texCurrent;
        int textWidth;
        Vector2 textPosition = new Vector2(0, 0);
        Vector2 buttonPos;
         

        public Button(Rectangle rectangle, string text, OnClick onClick)
        {
            buttonPos.X = rectangle.X;
            buttonPos.Y = rectangle.Y;   
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

        public Button(Rectangle rectangle, string text, OnClick onClick,Color fontColor)
        {
            buttonPos.X = rectangle.X;
            buttonPos.Y = rectangle.Y;
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
            this.fontColor = fontColor;  
        }

        public Button(Rectangle rectangle, OnClick onClick, string texHover, 
                      string texPressed, string texNormal, IntPtr textFont)
        {
            buttonPos.X = rectangle.X;
            buttonPos.Y = rectangle.Y;   
            base.zOrder = 0;
            this.areaRectangle = rectangle;
            this.onClick = onClick;
            this.texNormalS = texNormal;
            this.texPressedS = texPressed;
            this.textHoverS = texHover; 
            this.textFont = textFont;
            this.fontColor = Color.White;   
        }

        public bool Discard()
        {
            return base.discard; 
        }

        public int ZOrder()
        {
            return zOrder;
        }

        public Vector2 GetPos()
        {
            return buttonPos; 
        }

        public void SetPos(Vector2  vect)
        {
            buttonPos = vect; 
            areaRectangle.X = vect.X;
            areaRectangle.Y = vect.Y;  
            textPosition = new Vector2(areaRectangle.X + (areaRectangle.Width - textWidth) / 2,
                                        areaRectangle.Y + ((areaRectangle.Height - 15) / 2) + 14);
        }

        public Rectangle GetRectangle()
        {
            return areaRectangle; 
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
                            if (onClick != null)
                            {
                                AudioPlayback.PlayOne("click.wav");  
                                base.onClick.Invoke();
                            } 
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
            Sprite.DrawSprite(areaRectangle, texCurrent);
            Sprite.DrawText(textPosition.X, textPosition.Y, text, textFont, fontColor);  
        }
    }
}
