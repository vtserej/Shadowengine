using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Tao.OpenGl;
using ShadowEngine.ContentLoading;

namespace ShadowEngine.VisualControls
{
    public class Cursor : Control
    {
        #region private attributes

        int state;
        int button;
        Point position;
        int currentTexture;
        ECursor currentCursor;
        Dictionary<ECursor, int> cursors = new Dictionary<ECursor, int>();

        #endregion

        #region properties

        public ECursor CurrentCursor
        {
            get { return currentCursor; }
            set 
            {
                currentCursor = value;
                currentTexture = cursors[currentCursor]; 
            }
        }

        public Point Position
        {
            get { return position; }
            set { position = value; }
        } 

        public int State
        {
            get { return state; }
            set { state = value; }
        }

        public int Button
        {
            get { return button; }
            set { button = value; }
        }

        #endregion

        public Cursor()
        {
            state = Glut.GLUT_UP;
            areaRectangle = new Rectangle(0, 0, 32, 32); 
        }

        public void AddCursor(ECursor cname, string cursorName)
        {
            cursors.Add(cname, ContentManager.GetTextureByName(cursorName));     
        }

        public void Update()
        {
            if (currentCursor == ECursor.Arrow)
            {
                areaRectangle.X = position.X;
                areaRectangle.Y = position.Y;
            }
            else
            {
                areaRectangle.X = position.X - 16;
                areaRectangle.Y = position.Y - 16;
            }    
        }

        public void Draw()
        {
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);  
            Gl.glEnable(Gl.GL_BLEND);	
            Sprite.Begin();
            Sprite.DrawSprite(areaRectangle, currentTexture);   
            Sprite.End();
            Gl.glDisable(Gl.GL_BLEND);  
        }
    }
}
