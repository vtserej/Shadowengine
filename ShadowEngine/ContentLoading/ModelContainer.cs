using System;
using System.Collections.Generic;
using System.Text;
using Tao.OpenGl; 

namespace ShadowEngine.ContentLoading
{
    public class ModelContainer : Model 
    {
        string fileName;

        #region Properties

        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public string FileName
        {
            get { return fileName; }
        }

        public List<Mesh> GetMeshes
        {
            get { return base.meshes; }
        }

        #endregion

        public ModelContainer(List<Mesh> meshes, string fileName)
        {
            base.meshes = meshes;
            this.fileName = fileName;
        }

        public void RemoveMeshesWithName(string name)
        {
            for (int i = 0; i < meshes.Count; i++)
            {
                if (meshes[i].Name.Contains(name))
                {
                    meshes.RemoveAt(i);
                    i--;
                }
            }
        }

        public void RemoveMeshByName(string name)
        {
            for (int i = 0; i < meshes.Count; i++)
            {
                if (meshes[i].Name == name)
                {
                    meshes.RemoveAt(i);
                    i--;
                }
            }
        }

        public Mesh GetMeshWithName(string name)
        {
            foreach (var item in meshes)
            {
                if (item.Name == name)
                {
                    return item; 
                }
            }
            return null; 
        }

        public void CreateDisplayList()
        {
            foreach (var item in meshes)
            {
                item.Optimize();  
            }
        }

        public void DrawWithTextures()
        {
            foreach (var item in meshes)
            {
                Gl.glEnable(Gl.GL_TEXTURE_2D);  
                if (item.Name.Length > 5)
                    {
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, ContentManager.GetTextureByName(item.Name.Substring(0, 6) + ".jpg"));
                    }
                    else
                    {
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, ContentManager.GetTextureByName(item.Name + ".jpg"));
                    }
                
                item.DrawOptimized(); 
            }
        }

        public void Draw()
        {
            foreach (var item in meshes)
            {
                item.Draw();  
            }
        }

        public void DrawDisplayList()
        {
            Gl.glPushMatrix();
            Gl.glScalef(scale, scale, scale);    
            foreach (var item in meshes)
            {
                item.DrawOptimized(); 
            }
            Gl.glPopMatrix(); 
        }
    }
}
