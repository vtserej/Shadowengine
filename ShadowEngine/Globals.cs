using System;
using System.Collections.Generic;
using System.Text;

namespace ShadowEngine
{
    static public class Globals
    {
        static int formLeft; 
        static int formTop;
        static int screenWidth;      
        static int screenHeight;
        static int screenWidthOver2;
        static int screenHeightover2;
        static string applicationPath;
        static float camaraInc = 0.1f;
        static float camaraYaw = 0;
        static float camaraYawInc = 0.1f;
        static float camaraPitchInc = 0.1f;
        static float screenProportion;

        public static int FormLeft
        {
            get { return Globals.formLeft; }
            set { Globals.formLeft = value; }
        }

        public static int FormTop
        {
            get { return Globals.formTop; }
            set { Globals.formTop = value; }
        }

        public static float ScreenProportion
        {
            get { return Globals.screenProportion; }
            set { Globals.screenProportion = value; }
        }

        public static float CamaraInc
        {
            get { return Globals.camaraInc; }
            set { Globals.camaraInc = value; }
        }

        public static float CamaraYaw
        {
            get { return Globals.camaraYaw; }
            set { Globals.camaraYaw = value; }
        }

        public static string ApplicationPath
        {
            get { return Globals.applicationPath; }
            set { Globals.applicationPath = value; }
        }

        public static int ScreenWidth
        {
            get { return Globals.screenWidth; }
            set 
            {
                Globals.screenWidth = value;
                screenWidthOver2 = screenWidth / 2; 
            }
        }

        public static int ScreenHeight
        {
            get { return Globals.screenHeight; }
            set 
            { 
                Globals.screenHeight = value;
                screenHeightover2 = screenHeight / 2;   
            }
        }

        static public int ScreenWidthOver2
        {
            get { return screenWidthOver2; }
        }

        static public int ScreenHeightOver2
        {
            get { return screenHeightover2; }
        }
    }
}
