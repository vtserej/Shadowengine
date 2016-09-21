using System;
using System.Collections.Generic;
using Tao.OpenGl;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace ShadowEngine.ContentLoading
{
    /// <summary>
    /// This class provides the methods to handle OpenGL textures
    /// </summary>
    public static class Texture
    {
        #region Private atributes

        static TextureFiltering textFiltering = TextureFiltering.Trilinear;
        static TextureImage texture = new TextureImage();
        static float maximumAnisotropy = -1;
        static Regex rJPG = new Regex(".jpg");
        static Regex rBMP = new Regex(".bmp");
        static Regex rTGA = new Regex(".tga");

        #endregion

        #region Data types

        enum TexureSize //POT(Power of two) NPOT(non power of two)
        { NPOT, POT }

        enum TextureType
        { TGA, JPG, BMP }

        struct TextureImage
        {
            public byte[] imageData;				// Image Data (Up To 32 Bits)
            public int bpp;							// Image Color Depth In Bits Per Pixel.
            public int width;						// Image Width
            public int height;						// Image Height
            public int texID;						// Texture ID Used To Select A Texture
        }

        #endregion

        #region Properties

        public static TextureFiltering TextFiltering
        {
            get { return textFiltering; }
            set { textFiltering = value; }
        }

        #endregion      

        /// <summary>
        /// Static Constructor
        /// </summary>
        static Texture()
        {
            Gl.glGetFloatv(Gl.GL_MAX_TEXTURE_MAX_ANISOTROPY_EXT, out  maximumAnisotropy);
        }

        /// <summary>
        /// This method loads one texture
        /// </summary>
        static public int LoadSingleTexture(string path)
        {
            TextureType tex = ClasifyTexture(path);
            switch (tex)
            {
                case TextureType.TGA:
                    return LoadTGA(path);  

                case TextureType.JPG:
                    return LoadTexture(path);

                case TextureType.BMP:
                    return LoadTexture(path);

            }
            return -1;
        }

        /// <summary>
        /// This method performs a low level load of a texture
        /// </summary>
        static int LoadTexture(string path)
        {
            int[] texture = new int[1];

            Gl.glGenTextures(1, texture);
            Gl.glEnable(Gl.GL_TEXTURE_2D);

            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
          
            return TextureOne(path, 0, texture);
        }

        /// <summary>
        /// Enables the adecuate texture filtering
        /// </summary>
        static void EnableFiltering(TextureFiltering texFilt, TexureSize texSize)
        {
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);

            if (texSize == TexureSize.NPOT)
            {
                switch (texFilt)
                {
                    case TextureFiltering.Linear:
                        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST_MIPMAP_NEAREST);
                        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
                        break;
                    case TextureFiltering.Bilinear:
                        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR_MIPMAP_NEAREST);
                        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
                        break;
                    case TextureFiltering.Trilinear:
                        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR_MIPMAP_LINEAR);
                        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
                        break;
                    case TextureFiltering.Anisotropy:
                        Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAX_ANISOTROPY_EXT, maximumAnisotropy);
                        break;
                    default:
                        break;
                } 
            }
            else
            {
                switch (texFilt)
                {
                    case TextureFiltering.Linear:
                        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST);
                        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);
                        break;
                    case TextureFiltering.Bilinear:
                        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST);
                        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
                        break;
                    case TextureFiltering.Trilinear:
                        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
                        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
                        break;
                    case TextureFiltering.Anisotropy:
                        Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAX_ANISOTROPY_EXT, maximumAnisotropy);
                        break;
                    default:
                        break;
                } 
            }
        }

        /// <summary>
        /// This method performs a low level load of a texture
        /// </summary>
        static int TextureOne(string s, int i, int[] texture)
        {
            Bitmap bitmap = new Bitmap(s);

            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bitmapData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
            System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //esto es para asegurar que no se mueva del lugar de memoria el bitmap.

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture[i]);

            if (ContentManager.IsAnisotropic(s) && maximumAnisotropy > 0)
                Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAX_ANISOTROPY_EXT, maximumAnisotropy);	// anisotropic Filtered
            else
                EnableFiltering(textFiltering, TexureSize.NPOT);

            Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, 3, bitmap.Width, bitmap.Height, Gl.GL_BGR_EXT, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);
  

            // este codigo es si quiero diferenciar que las texturas NPOT se
            //cargen con mimapping y las POT(Power Of Two) normalmente
            /*
            if (bitmap.Width != bitmap.Height || Math.Log(bitmap.Width, 2) - (int)Math.Log(bitmap.Width, 2) != 0)
            {
                if (EngineContent.IsAnisotropic(s) && maximumAnisotropy > 0)
                    Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAX_ANISOTROPY_EXT, maximumAnisotropy);	// anisotropic Filtered
                else
                    EnableFiltering(textFiltering, TexureSize.NPOT);  
                
                Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, (int)Gl.GL_RGB8, bitmap.Width, bitmap.Height, Gl.GL_BGR_EXT, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);
            }
            else
            {
                if (EngineContent.IsAnisotropic(s) && maximumAnisotropy > 0)
                    Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAX_ANISOTROPY_EXT, maximumAnisotropy);	// anisotropic Filtered
                else
                    EnableFiltering(textFiltering, TexureSize.POT);  
                
                Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, 3, bitmap.Width, bitmap.Height, 0, Gl.GL_BGR, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);
            }
            */
            bitmap.UnlockBits(bitmapData);
            bitmap.Dispose();
            return texture[i];
        }

        /// <summary>
        /// This method clasify the type of texture to be loaded
        /// </summary>
        static TextureType ClasifyTexture(string name)
        {
            if (rJPG.Match(name).Success)
            {
                return TextureType.JPG;  
            }
            if (rTGA.Match(name).Success)
            {
                return TextureType.TGA;  
            }
            return TextureType.BMP;
        }

        /// <summary>
        /// This function checks and load the TGA selected 
        /// </summary>
        static int LoadTGA(string filename)
        {
            byte[] uTGACompare = { 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0 };	 // Uncompressed TGA Header
            byte[] cTGACompare = { 0, 0, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0 };	 // Compressed TGA Header
            byte[] TGACompare = new byte[12];	    // Used To Compare TGA Header

            FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read);	// Open The TGA File
            file.Read(TGACompare, 0, 12);
            if (Helper.Compare(TGACompare, uTGACompare))
            {
                return LoadUncompressedTGA(file, filename);
            }
            else
            {
                if (Helper.Compare(TGACompare, uTGACompare))
                {
                    return LoadCompressedTGA(file);
                }
            }
            file.Close();
            return 0;
        }

        /// <summary>
        /// This function load an uncompressed TGA
        /// </summary>
        static int LoadUncompressedTGA(FileStream file, string name)
        {
            byte[] header = new byte[6];			// First 6 Useful Bytes From The Header
            int bytesPerPixel;						// Holds Number Of Bytes Per Pixel Used In The TGA File
            int imageSize;						    // Used To Store The Image Size When Setting Aside Ram
            int temp;							    // Temporary Variable
            int type = Gl.GL_RGBA;					// Set The Default GL Mode To RBGA (32 BPP)

            if (file == null || file.Read(header, 0, 6) != 6)
            //falta la comparacion de memcmp
            {
                if (file == null)
                    return -1;
                else
                {
                    file.Close();
                    return -1;
                }
            }
            texture.width = header[1] * 256 + header[0];		// Determine The TGA Width	(highbyte*256+lowbyte)
            texture.height = header[3] * 256 + header[2];		// Determine The TGA Height	(highbyte*256+lowbyte)

            if (texture.width <= 0 || texture.height <= 0 || (header[4] != 24 && header[4] != 32))				// Is The TGA 24 or 32 Bit?
            {
                file.Close();
                return -1;
            }
            texture.bpp = header[4];					// Grab The TGA's Bits Per Pixel (24 or 32)
            bytesPerPixel = texture.bpp / 8;			// Divide By 8 To Get The Bytes Per Pixel
            imageSize = texture.width * texture.height * bytesPerPixel;	// Calculate The Memory Required For The TGA Data 
            texture.imageData = new byte[imageSize];			// Reserve Memory To Hold The TGA Data
            if (imageSize == 0 || file.Read(texture.imageData, 0, imageSize) != imageSize)
            {
                if (texture.imageData != null)
                    texture.imageData = null;

                file.Close();
                return -1;
            }
            for (int i = 0; i < imageSize; i += bytesPerPixel)			// Loop Through The Image Data
            {									// Swaps The 1st And 3rd Bytes ('R'ed and 'B'lue)
                temp = texture.imageData[i];					// Temporarily Store The Value At Image Data 'i'
                texture.imageData[i] = texture.imageData[i + 2];		// Set The 1st Byte To The Value Of The 3rd Byte
                texture.imageData[i + 2] = (byte)temp;				// Set The 3rd Byte To The Value In 'temp' (1st Byte Value)
            }

            file.Close();
            // Build A Texture From The Data
            int[] textureArray = new int[1];
            textureArray[0] = texture.texID;
            Gl.glGenTextures(1, textureArray);		// Generate OpenGL texture IDs

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureArray[0]);				// Bind Our Texture
            if (texture.bpp == 24)							// Was The TGA 24 Bits
            {
                type = Gl.GL_RGB;							// If So Set The 'type' To GL_RGB
            }
            
            //if the texture is intended to be rendered anisotropically
            if (ContentManager.IsAnisotropic(name) && maximumAnisotropy != 0)
            {
                Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAX_ANISOTROPY_EXT, maximumAnisotropy);	// anisotropic Filtered
            }

            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);	// Linear Filtered
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);	// Linear Filtered
            Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, 4, texture.width, texture.height, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, texture.imageData);
            
            //check for NPOT textures
            //if (texture.width != texture.height || Math.Log(texture.width, 2) - (int)Math.Log(texture.width, 2) != 0)
            //{
            //    Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);	// Linear Filtered
            //    Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);	// Linear Filtered
            //    Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, (int)Gl.GL_RGB8, texture.width, texture.height, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, texture.imageData);
            //}
            //else
            //{
            //    Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);	// Linear Filtered
            //    Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);	// Linear Filtered
            //    Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, type, texture.width, texture.height, 0, type, Gl.GL_UNSIGNED_BYTE, texture.imageData);
            //}

            texture.texID = textureArray[0];
            return texture.texID;                
        }

        /// <summary>
        /// This function load a RLE(Run Time Encoded) TGA
        /// </summary>
        static int LoadCompressedTGA(FileStream file)
        {
            //        if(fread(tga.header, sizeof(tga.header), 1, fTGA) == 0)
            //        {
            //            //...Error code here...
            //        }
            byte[] header = new byte[6];			// First 6 Useful Bytes From The Header
            file.Read(header, 0, 6);
            texture.width = header[1] * 256 + header[0];
            texture.height = header[3] * 256 + header[2];
            texture.bpp = header[4];
            //        tga.Width	= texture->width;
            //        tga.Height	= texture->height;
            //        tga.Bpp	= texture->bpp;
            //        if((texture->width <= 0) || (texture->height <= 0) || ((texture->bpp != 24) && (texture->bpp !=32)))
            //        {
            //            //...Error code here...
            //        }								
            //        tga.bytesPerPixel	= (tga.Bpp / 8);
            //        tga.imageSize		= (tga.bytesPerPixel * tga.Width * tga.Height);

            //    // Allocate Memory To Store Image Data
            //    texture->imageData	= (GLubyte *)malloc(tga.imageSize);
            //    if(texture->imageData == NULL)			// If Memory Can Not Be Allocated...
            //    {
            //        //...Error code here...
            //        return false;				// Return False
            //    }

            //    GLuint pixelcount = tga.Height * tga.Width;	// Number Of Pixels In The Image
            //    GLuint currentpixel	= 0;			// Current Pixel We Are Reading From Data
            //    GLuint currentbyte	= 0;			// Current Byte We Are Writing Into Imagedata
            //    // Storage For 1 Pixel
            //    GLubyte * colorbuffer = (GLubyte *)malloc(tga.bytesPerPixel); 

            //    do						// Start Loop
            //    {
            //    GLubyte chunkheader = 0;			// Variable To Store The Value Of The Id Chunk
            //    if(fread(&chunkheader, sizeof(GLubyte), 1, fTGA) == 0)	// Attempt To Read The Chunk's Header
            //    {
            //        //...Error code...
            //        return false;				// If It Fails, Return False
            //    }

            //    if(chunkheader < 128)				// If The Chunk Is A 'RAW' Chunk
            //    {
            //        chunkheader++;				// Add 1 To The Value To Get Total Number Of Raw Pixels


            //    // Start Pixel Reading Loop
            //    for(short counter = 0; counter < chunkheader; counter++)
            //    {
            //        // Try To Read 1 Pixel
            //        if(fread(colorbuffer, 1, tga.bytesPerPixel, fTGA) != tga.bytesPerPixel)
            //        {
            //            ...Error code...
            //            return false;			// If It Fails, Return False
            //        }

            //    texture->imageData[currentbyte] = colorbuffer[2];		// Write The 'R' Byte
            //    texture->imageData[currentbyte + 1	] = colorbuffer[1];	// Write The 'G' Byte
            //    texture->imageData[currentbyte + 2	] = colorbuffer[0];	// Write The 'B' Byte
            //    if(tga.bytesPerPixel == 4)					// If It's A 32bpp Image...
            //    {
            //        texture->imageData[currentbyte + 3] = colorbuffer[3];	// Write The 'A' Byte
            //    }
            //    // Increment The Byte Counter By The Number Of Bytes In A Pixel
            //    currentbyte += tga.bytesPerPixel;
            //    currentpixel++;					
            //// Increment The Number Of Pixels By 1  

            //    else						// If It's An RLE Header
            //    {
            //        chunkheader -= 127;			// Subtract 127 To Get Rid Of The ID Bit


            // //The we attempt to read the next color value.   


            //    // Read The Next Pixel
            //    if(fread(colorbuffer, 1, tga.bytesPerPixel, fTGA) != tga.bytesPerPixel)
            //    {
            //        //...Error code...
            //        return false;				// If It Fails, Return False
            //    }   

            //    // Start The Loop
            //    for(short counter = 0; counter < chunkheader; counter++)
            //    {
            //        // Copy The 'R' Byte
            //        texture->imageData[currentbyte] = colorbuffer[2];
            //        // Copy The 'G' Byte
            //        texture->imageData[currentbyte + 1	] = colorbuffer[1];
            //        // Copy The 'B' Byte
            //        texture->imageData[currentbyte + 2	] = colorbuffer[0];
            //        if(tga.bytesPerPixel == 4)		// If It's A 32bpp Image
            //        {
            //            // Copy The 'A' Byte
            //            texture->imageData[currentbyte + 3] = colorbuffer[3];
            //        }
            //        currentbyte += tga.bytesPerPixel;	// Increment The Byte Counter
            //        currentpixel++;				// Increment The Pixel Counter  

            //        while(currentpixel < pixelcount);	// More Pixels To Read? ... Start Loop Over
            //        fclose(fTGA);				// Close File
            return 0;				// Return Success
        }
    }
}
