using System;
using System.Collections.Generic;
using System.Text;
using Tao.OpenGl; 

namespace ShadowEngine.ContentLoading
{
    public class Mesh
    {
        #region Private Attributes

        string name;
        int textureName;  
        int displayList;
        
        Face[] faces;       

        #endregion

        #region Properties

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int TextureName
        {
            get { return textureName; }
            set { textureName = value; }
        }

        public int DisplayList
        {
            get { return displayList; }
        }

        public Face[] Faces
        {
            get { return faces; }
            set { faces = value; }
        }

        #endregion

        /// <summary>
        /// This function loads the mesh into an openGL display list
        /// </summary>
        public void Optimize()
        {
            Gl.glPushMatrix(); 
            displayList = Gl.glGenLists(1);
            Gl.glNewList(displayList, Gl.GL_COMPILE);
            this.Draw();
            Gl.glEndList();
            Gl.glPopMatrix(); 
        }

        /// <summary>
        /// This function calls the display list
        /// </summary>
        public void DrawOptimized()
        {
            Gl.glCallList(displayList);
        }

        /// <summary>
        /// This function overrides equality operator
        /// </summary>
        public override bool Equals(object obj)
        {
            if (((Mesh)obj).name == name)
            {
                return true; 
            }
            return false; 
        }

        /// <summary>
        /// This function draws the list of faces of the mesh
        /// </summary>
        public void Draw()
        {
            Gl.glBegin(Gl.GL_TRIANGLES);
            for (int i = 0; i < faces.Length; i++)
            {
                //draw the three points with their texture coordinates
                for (int j = 0; j < 3; j++)
                {
                    Gl.glNormal3f(faces[i].normal[j].X, faces[i].normal[j].Y, faces[i].normal[j].Z);
                    Gl.glTexCoord2f(faces[i].vertex[j].v, faces[i].vertex[j].u);
                    Gl.glVertex3f(faces[i].vertex[j].x, faces[i].vertex[j].y, faces[i].vertex[j].z);
                }
            }
            Gl.glEnd();
        }
    }
}
