using System;
using System.Collections.Generic;
using System.Text;
using Tao.OpenGl;

namespace ShadowEngine.OpenGL
{
    /// <summary>
    /// This class provides the methods to handle OpenGL lighting
    /// </summary>
    public static class Lighting
    {
        #region Private attributes

        static float[] materialAmbient = { 0.5F, 0.5F, 0.5F, 1.0F };
        static float[] materialDiffuse = { 0.2f, 0.7f, 0.9f, 1.0f };
        static float[] materialSpecular = { 1.0F, 1.0F, 1.0F, 1.0F };
        static float[] materialShininess = { 60.0F };
        static float[] ambientLightPosition = { 5F, -15F, -10F, 1.0F };
        static float[] lightAmbient = { 0.85F, 0.85F, 0.85F, 0.0F };

        #endregion

        #region Properties

        public static float[] MaterialAmbient
        {
            get { return Lighting.materialAmbient; }
            set { Lighting.materialAmbient = value; }
        }

        public static float[] MaterialDiffuse
        {
            get { return Lighting.materialDiffuse; }
            set { Lighting.materialDiffuse = value; }
        }

        public static float[] MaterialSpecular
        {
            get { return Lighting.materialSpecular; }
            set { Lighting.materialSpecular = value; }
        }

        public static float[] MaterialShininess
        {
            get { return Lighting.materialShininess; }
            set { Lighting.materialShininess = value; }
        }

        public static float[] AmbientLightPosition
        {
            get { return Lighting.ambientLightPosition; }
            set { Lighting.ambientLightPosition = value; }
        }

        public static float[] LightAmbient
        {
            get { return Lighting.lightAmbient; }
            set { Lighting.lightAmbient = value; }
        }


        #endregion
        
         public static void SetupLighting()
        {
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, materialAmbient);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, materialDiffuse);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, materialSpecular);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, materialShininess);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, ambientLightPosition);
            Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_AMBIENT, lightAmbient);

            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_LIGHT0);
            Gl.glEnable(Gl.GL_COLOR_MATERIAL);
            Gl.glColorMaterial(Gl.GL_FRONT, Gl.GL_AMBIENT_AND_DIFFUSE);
            Gl.glShadeModel(Gl.GL_SMOOTH);
        }
    }
}
