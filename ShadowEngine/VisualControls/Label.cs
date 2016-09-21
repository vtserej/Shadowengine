using System;
using System.Collections.Generic;
using System.Text;
using Tao.OpenGl;  
using System.Drawing;
using ShadowEngine;

namespace ShadowEngine.VisualControls
{
    public class Label : Control, IControl
    {
        #region private atributes

        string textData;
        Vector2 pos;
        Function myFunction;
        FunctionParameter functionParameter;
        Aligment aligment = Aligment.Left;
        Rectangle rectangle;
        object objeto; 
        bool bold;
        int left;

        #endregion

        public Label(string text, Vector2 pos, IntPtr font)
        {
            base.zOrder = 0;
            this.textData = text;
            this.pos = pos;
            base.textFont = font;
        }

        public Label(string text, Vector2 pos, IntPtr font,Color fontColor)
        {
            base.zOrder = 0;
            this.textData = text;
            this.pos = pos;
            base.textFont = font;
            base.fontColor = fontColor; 
        }

        public Label(Function function, Vector2 pos, IntPtr font)
        {
            myFunction = function;
            base.zOrder = 0;
            this.textData = "";
            this.pos = pos;
            base.textFont = font; ;
        }

        public Label(Function function,Rectangle rec, IntPtr font, Aligment aligment)
        {
            myFunction = function;
            this.rectangle = rec;
            base.zOrder = 0;
            this.textData = "";
            base.textFont = font;
            this.aligment = aligment;
            this.pos.X = rec.X;
            this.pos.Y = rec.Y;   
        }

        public Label(string text, Rectangle rec, IntPtr font, Aligment aligment)
        {
            this.rectangle = rec;
            base.zOrder = 0;
            this.textData = text;
            base.textFont = font;
            this.aligment = aligment;
            this.pos.X = rec.X;
            this.pos.Y = rec.Y;
        }

        public Label(FunctionParameter func, object objeto, Rectangle rec, IntPtr font, Aligment aligment)
        {
            functionParameter = func;
            this.objeto = objeto; 
            this.rectangle = rec;
            base.zOrder = 0;
            this.textData = "";
            base.textFont = font;
            this.aligment = aligment;
            this.pos.X = rec.X;
            this.pos.Y = rec.Y;
        }

        public Label(Function function, Rectangle rec, IntPtr font, Aligment aligment, bool bold)
        {
            myFunction = function;
            this.rectangle = rec;
            base.zOrder = 0;
            this.textData = "";
            base.textFont = font;
            this.aligment = aligment;
            this.pos.X = rec.X;
            this.pos.Y = rec.Y;
            this.bold = bold;  
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
        {

        }

        public void Update(Cursor cursor)
        {
            if (myFunction != null)
            {
                textData = myFunction.Invoke();
            }
            else
            {
                if (functionParameter != null)
                {
                    textData = functionParameter(objeto);
                }
            }
            if (aligment == Aligment.Center && bold == false)
            {
                pos = new Vector2(areaRectangle.X + (areaRectangle.Width - Sprite.FontWidth(textFont, textData)) / 2,
                            areaRectangle.Y + ((areaRectangle.Height - 10) / 2) + 14);
            }
            else
            {
                if (aligment == Aligment.Center && bold == true)
                {
                    this.left = (rectangle.Width / 2 - Sprite.FontBoldWidth(textFont, textData) / 2);
                }
            }
            if (aligment == Aligment.Right)
            {
                this.left = (rectangle.Width - Sprite.FontWidth(textFont, textData));
            }
        }

        public void Draw()
        {
            if (bold)
            {
                Sprite.DrawBoldText(pos.X + left, pos.Y, textData, textFont);
            }
            else
            {
                Sprite.DrawText(pos.X + left, pos.Y, textData, textFont, fontColor);
            } 
        }
    }
}
