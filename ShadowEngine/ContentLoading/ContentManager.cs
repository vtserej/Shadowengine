using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ShadowEngine.ContentLoading; 

namespace ShadowEngine
{
    public class ContentManager
    {
        #region Private Atributes

        //general;
        static string contentRoot;

        //textures
        static Dictionary<string, int> textures = new Dictionary<string, int>();
        static Dictionary<string, string> texturesAnisotropic = new Dictionary<string, string>();
        static List<string> textureDirs = new List<string>();
        static string textureDir;
      
        //models
        static Dictionary<string, ModelContainer> models = new Dictionary<string, ModelContainer>();
        static IModelLoader modelLoader = new Loader3DS();
        static List<string> modelDirs = new List<string>();
        static string modelDir;
        
        //geometry
        static Dictionary<string, GeometryModel> geometrys = new Dictionary<string, GeometryModel>();   
        static List<string> geometryDirs = new List<string>();   
        static string geometryDir;


        #endregion

        #region Properties

        public static string ContentRoot
        {
            get { return ContentManager.contentRoot; }
            set { ContentManager.contentRoot = value; }
        }

        #endregion

        #region Texture Functions

        /// <summary>
        /// This method loads all the texture path from a specified folder
        /// </summary>
        static public void SetTextureList(string texDir)
        {
            textureDir = Globals.ApplicationPath + texDir;
            string[] textArrayJPG = Directory.GetFiles(textureDir, "*.jpg");
            string[] textArrayBMP = Directory.GetFiles(textureDir, "*.bmp");
            string[] textArrayTGA = Directory.GetFiles(textureDir, "*.tga");
            textureDirs.AddRange(textArrayBMP);  
            textureDirs.AddRange(textArrayJPG);
            textureDirs.AddRange(textArrayTGA);   
        }

        /// <summary>
        /// This method specifies which textures will be filtered by
        /// anisotropic filtering
        /// </summary>
        public static void SetAnisotropic(string textureName)
        {
            textureName = textureDir + textureName; 
            texturesAnisotropic.Add(textureName, textureName);  
        }

        /// <summary>
        /// Returns true if the texture is intended to be rendered anisotropically
        /// </summary>
        public static bool IsAnisotropic(string textureName)
        {
            if (texturesAnisotropic.ContainsKey(textureName)) return true;
            return false;
        }

        /// <summary>
        /// This method returns the openGL id for the texture wanted
        /// </summary>
        public static int GetTextureByName(string textureName)
        {
            textureName = textureDir + textureName;

            if (textures.ContainsKey(textureName))
            {
                return textures[textureName];
            }

            return 0;
        }

        /// <summary>
        /// This method load all the textures
        /// </summary>
        public static void LoadTextures()
        {
            foreach (var item in textureDirs)
            {
                textures.Add(item, Texture.LoadSingleTexture(item));      
            }
        }

        #endregion

        #region Model Functions

        /// <summary>
        /// This method loads all the models path from a specified folder
        /// </summary>
        public static void SetModelList(string modDir)
         {
             modelDir = Globals.ApplicationPath + modDir;
             string[] objects = Directory.GetFiles(modelDir);
             modelDirs.AddRange(objects);            
        }

        /// <summary>
        /// This method loads a single model
        /// </summary>
        public static ModelContainer LoadModel(string file)
        {
            return modelLoader.LoadModel(file);
        }

        /// <summary>
        /// This method loads all the models
        /// </summary>
        public static void LoadModels()
        {
            foreach (string item in modelDirs)
            {
                models.Add(item, ContentManager.LoadModel(item));    
            }
        }

        /// <summary>
        /// This method returns the model whoose name has been specified
        /// </summary>
        public static ModelContainer GetModelByName(string name)
        {
            name = modelDir + name;
            return models[name]; 
        }

        #endregion

        #region Geometry Functions

        public static void SetGeometryList(string gDir)
        {
            geometryDir = Globals.ApplicationPath + gDir;
            string[] objects = Directory.GetFiles(geometryDir);
            geometryDirs.AddRange(objects);  
        }

        public static void LoadGeometry()
        {
            foreach (string item in geometryDirs)
            {
                geometrys.Add(item, GeometryModel.LoadGeometryFile(item));  
            }                        
        }

        public static GeometryModel GetGeometryByName(string name)
        {
            name = geometryDir + name;
            return geometrys[name]; 
        }

        #endregion
    }
}
