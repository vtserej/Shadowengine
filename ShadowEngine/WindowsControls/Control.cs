using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace ShadowEngine.WindowsControls
{
    public delegate void OnClick();
    public delegate string Function();
    public delegate string FunctionParameter(object objeto);


    public abstract class Control
    {
        protected string accesibleName = "null";
        protected OnClick onClick;
        protected Rectangle areaRectangle;
        protected Color fontColor = Color.White;
        protected int zOrder;
        protected int displayList;
        protected int fontId;
        protected bool discard;

        public string Name
        {
            get { return accesibleName; }
        }
    }
}
