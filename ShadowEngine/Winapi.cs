using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;   

namespace ShadowEngine
{
    public class WinApi
    {
        [DllImport("user32.dll")]
        public static extern void SetCursorPos(int x, int y);

        #region native code to load fonts

        [DllImport("opengl32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool wglUseFontBitmaps(uint hdc, uint first, uint count, uint listBase);

        [DllImport("gdi32.dll")]
        public static extern uint SelectObject(uint hdc, uint obj);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(uint hObject);

        #endregion
    }
}
