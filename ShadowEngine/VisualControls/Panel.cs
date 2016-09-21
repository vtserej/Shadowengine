using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ShadowEngine;
using ShadowEngine.ContentLoading;
using Tao.OpenGl;

namespace ShadowEngine.VisualControls
{
    public class Panel : Control, IControl
    {
        int texture;
        int drawMode = -1;
        Color color;

        public Panel(Rectangle rectangle, string texture, Aligment aligment)
        {
            base.zOrder = 1;
            base.areaRectangle = rectangle;
            if (aligment == Aligment.Center)
            {
                areaRectangle.X = Globals.ScreenWidthOver2 - areaRectangle.Width / 2;
            }
            this.texture = ContentManager.GetTextureByName(texture);   
        }

        public Panel(Rectangle rectangle, Color color, Aligment aligment)
        {
            base.zOrder = 1;
            base.areaRectangle = rectangle;
            this.color = color;  
            if (aligment == Aligment.Center)
            {
                areaRectangle.X = Globals.ScreenWidthOver2 - areaRectangle.Width / 2;
            }
        }

        public Panel(Rectangle rectangle, Color color)
        {
            base.zOrder = 1;
            base.areaRectangle = rectangle;
            this.color = color;
        }

        public Panel(Rectangle rectangle, Color color, int drawMode)
        {
            base.zOrder = 1;
            this.drawMode = drawMode;  
            base.areaRectangle = rectangle;
            this.color = color;
        }

        public Panel(Rectangle rectangle, Color color, int drawMode, Aligment aligment)
        {
            base.zOrder = 1;
            this.drawMode = drawMode;
            base.areaRectangle = rectangle;
            if (aligment == Aligment.Center)
            {
                areaRectangle.X = Globals.ScreenWidthOver2 - areaRectangle.Width / 2;
            }
            this.color = color;
        }

        public void SetLocation(int x, int y)
        {
            areaRectangle.X = x;
            areaRectangle.Y = y;  
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
            displayList = Gl.glGenLists(1);
            Gl.glNewList(displayList, Gl.GL_COMPILE);
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture);
            if (drawMode != -1)
            {
                //Un cuadrado con lineas
                Sprite.DrawSprite(areaRectangle, color, drawMode); 
            }
            else
                if (this.color.IsEmpty == false)
                {
                    //un cuadrado rellenado con color
                    Sprite.DrawSprite(areaRectangle, color);
                }
                else
                {
                    //una cuadrado con texturas
                    Sprite.DrawSprite(areaRectangle, this.texture);
                }
            Gl.glDisable(Gl.GL_TEXTURE_2D);  
            Gl.glEndList();   
        }

        public void Update(Cursor cursor)
        { }

        public void Draw()
        {
            Gl.glCallList(displayList);
        }
    }
}
