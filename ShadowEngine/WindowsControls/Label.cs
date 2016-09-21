using System;
using System.Collections.Generic;
using System.Text;
using Tao.OpenGl;  
using System.Drawing;

namespace ShadowEngine.WindowsControls
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

        public Label(string text, Vector2 pos, int font)
        {
            base.zOrder = 0;
            this.textData = text;
            this.pos = pos;
            base.fontId = font;
        }

        public Label(Function function, Vector2 pos, int font)
        {
            myFunction = function;
            base.zOrder = 0;
            this.textData = "";
            this.pos = pos;
            base.fontId = font;
        }

        public Label(Function function,Rectangle rec, int font, Aligment aligment)
        {
            myFunction = function;
            this.rectangle = rec;
            base.zOrder = 0;
            this.textData = "";
            base.fontId = font;
            this.aligment = aligment;
            this.pos.X = rec.X;
            this.pos.Y = rec.Y;   
        }

        public Label(string text, Rectangle rec, int font, Aligment aligment)
        {
            this.rectangle = rec;
            base.zOrder = 0;
            this.textData = text;
            base.fontId = font;
            this.aligment = aligment;
            this.pos.X = rec.X;
            this.pos.Y = rec.Y;
        }

        public Label(FunctionParameter func, object objeto, Rectangle rec, int font, Aligment aligment)
        {
            functionParameter = func;
            this.objeto = objeto; 
            this.rectangle = rec;
            base.zOrder = 0;
            this.textData = "";
            base.fontId = font;
            this.aligment = aligment;
            this.pos.X = rec.X;
            this.pos.Y = rec.Y;
        }

        public Label(Function function, Rectangle rec, int font, Aligment aligment, bool bold)
        {
            myFunction = function;
            this.rectangle = rec;
            base.zOrder = 0;
            this.textData = "";
            base.fontId = font;
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
                this.left = (rectangle.Width / 2 - GLFont.GlSize(fontId, textData) / 2);
            }
            else
            {
                if (aligment == Aligment.Center && bold == true)
                {
                    //this.left = (rectangle.Width / 2 - Sprite.FontBoldWidth(textFont, textData) / 2);
                }
            }
            if (aligment == Aligment.Right)
            {
                this.left = (rectangle.Width - GLFont.GlSize(fontId, textData));
            }
        }

        public void Draw()
        {
            if (bold)
            {
               // Sprite.DrawBoldText(pos.X + left, pos.Y, textData, textFont);
            }
            else
            {
                GLFont.DrawText(pos.X + left, pos.Y, textData, fontId, fontColor);
            } 
        }
    }
}
