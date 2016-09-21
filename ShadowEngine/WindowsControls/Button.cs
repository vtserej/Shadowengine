using System;
using Tao.OpenGl;
using System.Drawing;

namespace ShadowEngine.WindowsControls
{
    public class Button : Control, IControl 
    {
        bool isPressed;
        string text = "";
        string textHoverS, texPressedS, texNormalS;
        int textHover, texPressed, texNormal, texCurrent;
        int textWidth;
        Vector2 textPosition = new Vector2(0, 0);
         

        public Button(Rectangle rectangle, string text, OnClick onClick)
        {
            base.zOrder = 0; 
            this.text = text;
            this.areaRectangle = rectangle;
            base.onClick = onClick;
            texNormalS = "normal.jpg";
            texPressedS = "pressed.jpg";
            textHoverS = "hover.jpg";
            texCurrent = texNormal;
            textWidth = GLFont.GlSize("verdana", 18, text);
            textPosition = new Vector2(areaRectangle.X + (areaRectangle.Width - textWidth) / 2,
                                        areaRectangle.Y + ((areaRectangle.Height - 15) / 2) + 14);
            this.fontId  = GLFont.GetFontId("verdana", 18);   
        }

        public Button(Rectangle rectangle, OnClick onClick, string texHover, 
                      string texPressed, string texNormal, int fontId)
        {
            base.zOrder = 0;
            this.areaRectangle = rectangle;
            this.onClick = onClick;
            this.texNormalS = texNormal;
            this.texPressedS = texPressed;
            this.textHoverS = texHover;
            this.fontId = fontId;
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
                if (cursor.State == CursorState.Down)
                {
                    isPressed = true;
                    texCurrent = texPressed;
                }
                else

                    if (isPressed)
                    {
                        if (cursor.State == CursorState.UP)
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
            Sprite.DrawSprite(areaRectangle, texCurrent);
            GLFont.DrawText(textPosition.X, textPosition.Y, text, fontId, fontColor);  
        }
    }
}
