using System;
using System.Runtime.InteropServices;
using Tao.OpenGl;

namespace ShadowEngine.OpenGL
{
    public class OpenGLControl
    {
        #region Data Types

        // Layer types
        enum LayerTypes : sbyte
        {
            PFD_MAIN_PLANE = 0,
            PFD_OVERLAY_PLANE = 1,
            PFD_UNDERLAY_PLANE = -1,
        }

        // Pixel types
        enum PixelTypes : byte
        {
            PFD_TYPE_RGBA = 0,
            PFD_TYPE_COLORINDEX = 1,
        }

        // PIXELFORMATDESCRIPTOR flags
        [Flags]
        enum PFD_Flags : uint
        {
            PFD_DOUBLEBUFFER = 0x00000001,
            PFD_STEREO = 0x00000002,
            PFD_DRAW_TO_WINDOW = 0x00000004,
            PFD_DRAW_TO_BITMAP = 0x00000008,
            PFD_SUPPORT_GDI = 0x00000010,
            PFD_SUPPORT_OPENGL = 0x00000020,
            PFD_GENERIC_FORMAT = 0x00000040,
            PFD_NEED_PALETTE = 0x00000080,
            PFD_NEED_SYSTEM_PALETTE = 0x00000100,
            PFD_SWAP_EXCHANGE = 0x00000200,
            PFD_SWAP_COPY = 0x00000400,
            PFD_SWAP_LAYER_BUFFERS = 0x00000800,
            PFD_GENERIC_ACCELERATED = 0x00001000,
            PFD_SUPPORT_DIRECTDRAW = 0x00002000,
        }

        // Structure of PIXELFORMATDESCRIPTOR used by ChoosePixelFormat()
        [StructLayout(LayoutKind.Sequential)]
        struct PIXELFORMATDESCRIPTOR
        {
            public ushort nSize;
            public ushort nVersion;
            public uint dwFlags;
            public byte iPixelType;
            public byte cColorBits;
            public byte cRedBits;
            public byte cRedShift;
            public byte cGreenBits;
            public byte cGreenShift;
            public byte cBlueBits;
            public byte cBlueShift;
            public byte cAlphaBits;
            public byte cAlphaShift;
            public byte cAccumBits;
            public byte cAccumRedBits;
            public byte cAccumGreenBits;
            public byte cAccumBlueBits;
            public byte cAccumAlphaBits;
            public byte cDepthBits;
            public byte cStencilBits;
            public byte cAuxBuffers;
            public sbyte iLayerType;
            public byte bReserved;
            public uint dwLayerMask;
            public uint dwVisibleMask;
            public uint dwDamageMask;
        }

        #endregion

        #region Private Attributes

        // External Win32 libraries
        const string KER_DLL = "kernel32.dll";	// Import library for Kernel on Win32
        const string OGL_DLL = "opengl32.dll";	// Import library for OpenGL on Win32
        const string GDI_DLL = "gdi32.dll";		// Import library for GDI on Win32
        const string USR_DLL = "user32.dll";	// Import library for User on Win32

        // Class variables
        static uint m_hDC = 0;					// Display device context
        static uint m_hRC = 0;					// OpenGL rendering context
        static bool haveMultisample;
        static bool haveTextRectangle;
        static bool haveAnisotropicFiltering;

        const int WGL_SAMPLE_BUFFERS_ARB = 0x2041;
        const int WGL_SAMPLES_ARB = 0x2042;
        
        //Projection values
        static float proportion;
        static float fov = 45;
        static float zNear = 0.05f;
        static float zFar = 300;

        #endregion

        #region Properties

        public static bool HaveAnisotropicFiltering
        {
            get { return OpenGLControl.haveAnisotropicFiltering; }
        }

        public static bool HaveMultisample
        {
            get { return OpenGLControl.haveMultisample; }
        }

        #endregion

        #region Imported functions

        // Define all of the exported functions from unmanaged Win32 DLLs

        // kernel32.dll unmanaged Win32 DLL
        [DllImport(KER_DLL)]
        public static extern uint LoadLibrary(string lpFileName);

        // opengl32.dll unmanaged Win32 DLL
        [DllImport(OGL_DLL)]
        static extern uint wglGetCurrentContext();
        [DllImport(OGL_DLL)]
        static extern bool wglMakeCurrent(uint hdc, uint hglrc);
        [DllImport(OGL_DLL)]
        static extern uint wglCreateContext(uint hdc);
        [DllImport(OGL_DLL)]
        static extern int wglDeleteContext(uint hglrc);


        // gdi32.dll unmanaged Win32 DLL
        [DllImport(GDI_DLL)]
        unsafe static extern int ChoosePixelFormat(uint hdc, PIXELFORMATDESCRIPTOR* ppfd);
        [DllImport(GDI_DLL)]
        unsafe static extern int SetPixelFormat(uint hdc, int iPixelFormat,
                                                              PIXELFORMATDESCRIPTOR* ppfd);

        // user32.dll unmanaged Win32 DLL
        [DllImport(USR_DLL)]
        static extern uint GetDC(uint hWnd);
        [DllImport(USR_DLL)]
        static extern int RelaseDC(uint hWnd, uint hDC);

        #endregion

        ~OpenGLControl()
        {
            if (m_hRC != 0)
                wglDeleteContext(m_hRC);
        }

        public static unsafe bool OpenGLInit(ref uint handle, int width, int height, ref string error)
        {
            Globals.ScreenHeight = height;
            Globals.ScreenWidth = width;
            // Explicitly load the OPENGL32.DLL library
            uint m_hModuleOGL = LoadLibrary(OGL_DLL);

            // Retrieve a handle to display device context for client area of specified window
            uint hdc = GetDC((uint)handle);

            PFD_Flags BitFlags = new PFD_Flags();
            BitFlags |= PFD_Flags.PFD_DRAW_TO_WINDOW;
            BitFlags |= PFD_Flags.PFD_SUPPORT_OPENGL;
            BitFlags |= PFD_Flags.PFD_DOUBLEBUFFER;


            #region Inicialización del PIXELFORMATDESCRIPTOR

            PIXELFORMATDESCRIPTOR pfd;
            pfd.nSize = (ushort)sizeof(PIXELFORMATDESCRIPTOR);
            pfd.nVersion = 1;
            pfd.dwFlags = (uint)BitFlags;
            pfd.iPixelType = (byte)PixelTypes.PFD_TYPE_RGBA;
            pfd.cColorBits = 24;
            pfd.cRedBits = 0;
            pfd.cRedShift = 0;
            pfd.cGreenBits = 0;
            pfd.cGreenShift = 0;
            pfd.cBlueBits = 0;
            pfd.cBlueShift = 0;
            pfd.cAlphaBits = 0;
            pfd.cAlphaShift = 0;
            pfd.cAccumBits = 0;
            pfd.cAccumRedBits = 0;
            pfd.cAccumGreenBits = 0;
            pfd.cAccumBlueBits = 0;
            pfd.cAccumAlphaBits = 0;
            pfd.cDepthBits = 16;
            pfd.cStencilBits = 1;
            pfd.cAuxBuffers = 0;
            pfd.iLayerType = (sbyte)LayerTypes.PFD_MAIN_PLANE;
            pfd.bReserved = 0;
            pfd.dwLayerMask = 0;
            pfd.dwVisibleMask = 0;
            pfd.dwDamageMask = 0;

            #endregion

            // Match an appropriate pixel format 
            int iPixelformat;
            if ((iPixelformat = ChoosePixelFormat(hdc, &pfd)) == 0)
                return false;

            // Sets the pixel format
            if (SetPixelFormat(hdc, iPixelformat, &pfd) == 0)
                return false;

            // Create a new OpenGL rendering contex
            m_hRC = wglCreateContext(hdc);

            if (m_hRC == 0)
            {
                error = "HRC = 0";
                return false;
            }

            m_hDC = hdc;
            handle = hdc;

            // Make the OpenGL rendering context the current rendering context
            if (!wglMakeCurrent(m_hDC, m_hRC))
            {
                error = "Could not make current";
                return false;
            }

            SetOpenGL();
            Init(width, height);
            CheckAnisotropicFiltering(); 

            return true;
        }

        static public void SetOpenGL()
        {
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthFunc(Gl.GL_LEQUAL);
            Gl.glClearDepth(1.0f);
            Gl.glHint(Gl.GL_POINT_SMOOTH_HINT, Gl.GL_NICEST);
            Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);
            Gl.glHint(Gl.GL_POLYGON_SMOOTH_HINT, Gl.GL_NICEST);
            Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);
            Gl.glClearColor(0.2f, 0.5f, 1.0f, 1.0f);
        }

        static public bool OpenlGlutInit(int width, int height)
        {
            Globals.ScreenHeight = height;
            Globals.ScreenWidth = width;
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_ALPHA | Glut.GLUT_DOUBLE |
                                     Glut.GLUT_DEPTH);
            Glut.glutInitWindowSize(width, height);
            Glut.glutInitWindowPosition(0, 0);
            Glut.glutCreateWindow("");
            Init(width, height);
            CheckMultiSampling();
            CheckTextureRectangle();
            CheckAnisotropicFiltering();
            return true;
        }

        static public void Init(int width, int height)
        {
            Globals.ScreenHeight = height;
            Globals.ScreenWidth = width;
            proportion = width / (float)height;
            Globals.ScreenProportion = proportion;  
            Gl.glViewport(0, 0, width, height);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(fov, proportion, zNear, zFar);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
        }

        static void CheckMultiSampling()
        {
            int s;
            Gl.glGetIntegerv(WGL_SAMPLES_ARB, out s);
            if (Glut.glutExtensionSupported("GL_ARB_multisample") != 0 || s < 1)
            {
                haveMultisample = false;
            }
        }

        static void CheckTextureRectangle()
        {
            if (Glut.glutExtensionSupported("GL_EXT_texture_rectangle") == 0)
            {
                haveTextRectangle = false;
            }
        }

        static void CheckAnisotropicFiltering()
        {
            if (Glut.glutExtensionSupported("GL_EXT_texture_filter_anisotropic") == 0)
            {
                haveAnisotropicFiltering = false;
            }
            else
            {
                haveAnisotropicFiltering = true;
            }
        }
    }
}
