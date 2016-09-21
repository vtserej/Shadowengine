using System.Drawing;
using Tao.OpenGl;
using ShadowEngine.VisualControls;

namespace ShadowEngine
{
    public enum AltKey
    {
        None, Alt, Ctrol, Shift
    };

    /// <summary>
    /// This class provides inputs functions to glut's dll
    /// </summary>
    static public class Input
    {
        #region Private atributes

        static char key;
        static int delta;
        static bool pressed;
        static bool returned = false;
        static bool cursorDown = true; 
        static Cursor cursor = new Cursor();
        static AltKey altKey;
        static int specialKey;
        static Point position;
        
        #endregion

        #region Properties

        public static Cursor Cursor
        {
            get
            {
                return Input.cursor;
            }
        }

        public static int Delta
        {
            get
            {
                int r = Input.delta;
                Input.delta = 0; 
                return r;
            }
            set { Input.delta = value; }
        }

        public static bool CursorDown
        {
            get
            {
                if (cursorDown == true)
                {
                    cursorDown = false;
                    return true; 
                }
                return false;  
            }
        }

        public static bool Pressed
        {
            get { return Input.pressed; }
            set { Input.pressed = value; }
        }

        public static int SpecialKey
        {
            get
            {
                if (pressed)
                {
                    return Input.specialKey;
                }
                return -1;
            }
        }

        public static char Key
        {
            set 
            {
                key = value; 
            }
 
            get 
            {
                if (pressed)
                {
                    return key;    
                }
                return ' '; 
            }
        }

        public static char KeyDownChar
        {
            get
            {
                if (pressed == true && returned == false)
                {
                    returned = true; 
                    return key;
                }
                return ' ';
            }
        }

        public static AltKey AltKey
        {
            get { return Input.altKey; }
        }

        #endregion

        static public void SpecialKeyDown(int key, int x, int y)
        {
            pressed = true;
            specialKey = key;
            switch (Glut.glutGetModifiers())
            {
                case Glut.GLUT_ACTIVE_ALT:
                    {
                        altKey = AltKey.Alt;
                        break;
                    }
                case Glut.GLUT_ACTIVE_CTRL:
                    {
                        altKey = AltKey.Ctrol;
                        break;
                    }
                case Glut.GLUT_ACTIVE_SHIFT:
                    {
                        altKey = AltKey.Shift;
                        break;
                    }
                default:
                    {
                        altKey = AltKey.None;
                        break;
                    }
            } 
        }

        static public void SpecialKeyUp(int key, int x, int y)
        {
            pressed = false;
            returned = false;
            altKey = AltKey.None;  
        }

        static public void KeyDown(byte b, int i, int j)
        {
            key = (char)b;
            pressed = true;         
        }

        static public void KeyUp(byte b, int i, int j)
        {
            pressed = false;
            returned = false;  
        }

        static public void MouseState(int button, int state, int x, int y)
        {
            if (state ==  Glut.GLUT_UP)
            {
                cursorDown = true; 
            }
            cursor.State = state;
            cursor.Button = button;   
        }

        static public void MousePos(int x , int y)
        {
            position.X = x;
            position.Y = y;
            cursor.Position = position;
        }

        static public void CenterMouse()
        {
            WinApi.SetCursorPos(Globals.ScreenWidthOver2 + Globals.FormLeft, Globals.ScreenHeightOver2 + Globals.FormTop);
            Input.cursor.Position = new Point(Globals.ScreenWidthOver2, Globals.ScreenHeightOver2);
   
        }
    }
}
