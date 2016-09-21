using System;
using System.Collections.Generic;
using System.Text;
using Tao.OpenGl;
using System.Drawing;

namespace ShadowEngine.OpenGL
{
    /// <summary>
    /// This class provides the methods to handle OpenGL fog
    /// </summary>
    static public class Fog
    {
        static public void SetupFog(int filter, float density, Color color, float start, float end)
        {
            int[] fogMode = { Gl.GL_EXP, Gl.GL_EXP2, Gl.GL_LINEAR };
            float[] fogColor = { color.R / 255f, color.G / 255f, color.B / 255f, 0.5f };
            Gl.glFogi(Gl.GL_FOG_MODE, fogMode[filter]);
            Gl.glFogfv(Gl.GL_FOG_COLOR, fogColor);
            Gl.glFogf(Gl.GL_FOG_DENSITY, density);
            Gl.glHint(Gl.GL_FOG_HINT, Gl.GL_DONT_CARE);
            Gl.glFogf(Gl.GL_FOG_START, start);
            Gl.glFogf(Gl.GL_FOG_END, end);
        }
    }
}
