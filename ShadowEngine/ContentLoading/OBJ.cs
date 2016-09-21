using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace ShadowEngine.ContentLoading
{
    /// <summary>
    /// This class loads an OBJ file 
    /// </summary>
    class OBJ : IModelLoader  
    {
        struct Indexes
        {
            public int faceIndex;
            public int textureIndex;
        };

        struct Faces
        {
            public float[] V1;
            public float[] V2;
            public float[] V3;
            public float[] N;
        }

        #region Private Attributes

        char[] invalid = { '#', 'v', 'f', '/', ' '};
        char[] separator = { ' ' };
        Indexes indexes = new Indexes();
        Vertex point = new Vertex();
        int objectCount;
        int prevVertex;
        int prevTexture;
        string[] file;
        TextureCoord textureCoord = new TextureCoord();
        List<Mesh> meshes = new List<Mesh>();
        NumberFormatInfo provider = new NumberFormatInfo();

        #endregion

        public ModelContainer LoadModel(string path)
        {
            this.file = File.ReadAllLines(path);
            provider.CurrencyDecimalSeparator = ".";
            this.objectCount = GetMeshCount();
            for (int i = 0; i < objectCount; i++)
            {
                meshes.Add(LoadMesh());
                if (objectCount > 1)
                {
                    DeleteObject();
                }
            }
            prevTexture = 0;
            prevVertex = 0;
            return new ModelContainer(meshes, "");  
        }

        Mesh LoadMesh()
        {
            MeshIndexed meshIndexed = new MeshIndexed();
            int vertexStart = GetIndex("vertices");
            int vertexCount = (int)GetNumber(file[vertexStart], "#", 5);
            int textureStart = GetIndex("texture");
            int textureCount = (int)GetNumber(file[textureStart], "#", 4);
            meshIndexed.vertexList = new Vertex[vertexCount];
            meshIndexed.textures = new TextureCoord[textureCount];
            int count = GetIndex("object") + 2;
            //obtengo la lista de puntos
            for (int i = 0; i < vertexCount; i++)
            {
                meshIndexed.vertexList[i] = GetVertexPoint(file[i + count]);
            }
            count = GetIndex("vt");
            //obtengo la lista de coordenadas de textura
            for (int i = 0; i < textureCount; i++)
            {
                meshIndexed.textures[i] = GetTexturePoint(file[i + count], 3);
            }
                              
            int facesStart = GetIndex("faces");
            int facesCount = (int)GetNumber(file[facesStart], "#", 5);
            meshIndexed.faceList = new FaceIndexed[facesCount];          
            count = GetIndex("f ");
            int index = 0;
            int s = 0;

            // para obtener los indices de puntos y de texturas
            for (int i = 0; i < facesCount + s; i++)
            {
                if (file[count + i].IndexOf('s') == -1)
                {
                    meshIndexed.faceList[index].vertexIndexes = new int[3];
                    meshIndexed.faceList[index].textureIndexes = new int[3];
                    for (int j = 0; j < 3; j++)
                    {
                        indexes = GetIndexes(file[count + i], j);
                        meshIndexed.faceList[index].vertexIndexes[j] = indexes.faceIndex - prevVertex;
                        meshIndexed.faceList[index].textureIndexes[j] = indexes.textureIndex - prevTexture;
                    }
                    index++;
                }
                else
                {
                    // s significa el material de las caras (no implementado)
                    s++;
                }
            }
            prevVertex += vertexCount;
            prevTexture += textureCount; 
            CalcularNormales(meshIndexed);
            return CopyMesh(meshIndexed);
        }

        Mesh CopyMesh(MeshIndexed meshIndexed)
        {
            Mesh mesh = new Mesh();
            mesh.Faces = new Face[meshIndexed.faceList.Length];  
            for (int i = 0; i < mesh.Faces.Length; i++)
			{
                Vertex[] vertex = new Vertex[3];
                Vector3[] normal = new Vector3[3];  
                for (int j = 0; j < 3; j++)
                {
                    vertex[j] = meshIndexed.vertexList[meshIndexed.faceList[i].vertexIndexes[j]];
                    vertex[j].u = meshIndexed.textures[meshIndexed.faceList[i].textureIndexes[j]].x;
                    vertex[j].v = meshIndexed.textures[meshIndexed.faceList[i].textureIndexes[j]].y;
                    normal[j] = meshIndexed.faceList[i].normal[j];
                }
                mesh.Faces[i].vertex = vertex;
                mesh.Faces[i].normal = normal;   
			}
            return mesh;
        }

        bool HasTexture()
        {
            if (GetIndex("vt") != -1)
            {
                return true;
            }
            else
                return false;
        }

        bool HasNormal()
        {
            if (GetIndex("vn") != -1)
            {
                return true;
            } 
            return false;
        }

        static int[] GetNumberChar(string line, int start)
        { 
            //toma los indices donde empiezan las x,y,z
            int[] numberStarts = new int[6];
            int index = 0;
       
            for (int i = start; i < line.Length  ; i++)
            {               
                if (line[i] == ' ')
                {
                    numberStarts[index] = i;
                    index++;
                }
            }
            //lo ultimos 3 valores son los tamaños de x,y,z
            numberStarts[3] = numberStarts[1] - numberStarts[0];
            numberStarts[4] = numberStarts[2] - numberStarts[1];
            numberStarts[5] = line.Length - numberStarts[2];
            return numberStarts; 
        }

        Indexes GetIndexes(string linea, int pointC)
        {           
            int[] numberStarts = GetNumberChar(linea, 1);
            string points;
            points = linea.Substring(numberStarts[pointC], numberStarts[pointC+3]);
            int separator = points.IndexOf('/');
            indexes.textureIndex = GetTextureIndex(points); 
            points = points.Substring(0, separator);
            indexes.faceIndex = Convert.ToInt32(points, provider) - 1;
            return indexes ;
        }

        int GetTextureIndex(string linea)
        {
            int separator = linea.IndexOf('/');
            linea = linea.Substring(separator + 1);
            separator = linea.IndexOf('/');
            linea = linea.Substring(0, separator);
            return Convert.ToInt32(linea, provider) - 1;
            //le resto uno porque mi arreglo empieza en cero
        }

        Vertex GetVertexPoint(string linea)
        {
            string[] values = linea.Split(separator);
            point.x = Convert.ToSingle(values[2], provider);
            point.y = Convert.ToSingle(values[3], provider);
            point.z = Convert.ToSingle(values[4], provider);
            return point;
        }

        TextureCoord GetTexturePoint(string linea, int start)
        {
            int[] numberStarts = GetNumberChar(linea, start);
            textureCoord.x = Convert.ToSingle(linea.Substring(numberStarts[0], numberStarts[3]), provider);
            textureCoord.y = Convert.ToSingle(linea.Substring(numberStarts[1], numberStarts[4]), provider);    
            return textureCoord;
        }
        
        float GetNumber(string linea, string cadena, int lenght)
        {
            int begin = linea.IndexOf(cadena) + cadena.Length + 1;
            while (begin + lenght > linea.Length || linea[begin + lenght] != ' ')
            {
                lenght--;
            }
            string point = linea.Substring(begin, lenght);
            point = point.Trim(invalid);
            return Convert.ToSingle(point, provider);
        }

        int GetIndex(string cadena)
        {
            int count = 0;
            while (file.Length - 1 != count)
            {
                count++;
                if (this.file[count].IndexOf(cadena,StringComparison.InvariantCulture) != -1)
                {
                    return count;
                }
            }
            return -1;
        }

        int GetIndex(string cadena, int count)
        {
            int index = 0; 
            for (int i = 0; i < this.file.Length; i++)
            {
                if (this.file[i].IndexOf(cadena, StringComparison.InvariantCulture) != -1)
                {
                    index++;
                }
                if (index == count)
                {
                    return i;
                }
            }
            return -1;
        }

        public int GetMeshCount()
        {
            //cuenta cuantos meshes hay en el fichero OBJ
            int count = 0;
            for (int i = 0; i < this.file.Length; i++)
            {
                if (this.file[i].IndexOf("object", StringComparison.InvariantCulture) != -1)
                {
                    count++;
                }
            }
            return count; 
        }

        void DeleteObject()
        {
            int begin = GetIndex("object");
            int end = GetIndex("object", 2);
            if (end == -1)
            {
                end = this.file.Length-1; 
            } 
            if (begin != -1)
            {
                for (int i = 0; i < end - begin; i++)
                {
                    this.file[i + begin] = "";
                }
            }
        }

        bool CheckNormals()
        {
            if (GetIndex("vn") != -1)
            {
                return true;
            }
            return false;
        }

        void CalcularNormales(MeshIndexed objeto)
        {
            float NormalX = 0, NormalY = 0, NormalZ = 0;
            float sumx = 0.0f, sumy = 0.0f, sumz = 0.0f;
            int shared = 0;
            int i, j;
            int policount = objeto.faceList.Length;
            int vertexcount = objeto.vertexList.Length;

            Faces[] Tri = new Faces[policount];

            for (i = 0; i < policount; i++)
            {
                objeto.faceList[i].normal = new Vector3[3];
            }

            for (i = 0; i < Tri.Length; i++)
            {
                Tri[i].V1 = new float[3];
                Tri[i].V2 = new float[3];
                Tri[i].V3 = new float[3];
                Tri[i].N = new float[3];
            }

            float[,] vnormales = new float[vertexcount, 3];

            for (i = 0; i < policount; i++)
            {
                //Vertice 1
                j = objeto.faceList[i].vertexIndexes[0];
                Tri[i].V1[0] = objeto.vertexList[j].x;
                Tri[i].V1[1] = objeto.vertexList[j].y;
                Tri[i].V1[2] = objeto.vertexList[j].z;
                //Vertice 2
                j = objeto.faceList[i].vertexIndexes[1];
                Tri[i].V2[0] = objeto.vertexList[j].x;
                Tri[i].V2[1] = objeto.vertexList[j].y;
                Tri[i].V2[2] = objeto.vertexList[j].z;
                //Vertice 3
                j = objeto.faceList[i].vertexIndexes[2];
                Tri[i].V3[0] = objeto.vertexList[j].x;
                Tri[i].V3[1] = objeto.vertexList[j].y;
                Tri[i].V3[2] = objeto.vertexList[j].z;

                //Calcula el vector Normal
                VectorNormal(Tri[i].V1, Tri[i].V2, Tri[i].V3, out NormalX, out NormalY, out NormalZ);
                //nos retorna un vector unitario, es decir de modulo 1
                Normalizar(ref NormalX, ref NormalY, ref NormalZ);
                //almacena los vectores normales, para cada poligono
                Tri[i].N[0] = NormalX;
                Tri[i].N[1] = NormalY;
                Tri[i].N[2] = NormalZ;
            }

            for (i = 0; i < vertexcount; i++)
            {
                for (j = 0; j < policount; j++)
                {
                    if (objeto.faceList[j].vertexIndexes[0] == i || objeto.faceList[j].vertexIndexes[1] == i || objeto.faceList[j].vertexIndexes[2] == i)
                    {
                        sumx = sumx + Tri[j].N[0];
                        sumy = sumy + Tri[j].N[1];
                        sumz = sumz + Tri[j].N[2];
                        shared++;
                    }
                }
                vnormales[i, 0] = sumx / (float)shared;
                vnormales[i, 1] = sumy / (float)shared;
                vnormales[i, 2] = sumz / (float)shared;

                Normalizar(ref vnormales[i, 0], ref vnormales[i, 1], ref vnormales[i, 2]);

                sumx = 0.0f;
                sumy = 0.0f;
                sumz = 0.0f;
                shared = 0;
            }

            //guardo las normales calculadas en el objeto
            for (i = 0; i < policount; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    objeto.faceList[i].normal[j].X = vnormales[objeto.faceList[i].vertexIndexes[j], 0];
                    objeto.faceList[i].normal[j].Y = vnormales[objeto.faceList[i].vertexIndexes[j], 1];
                    objeto.faceList[i].normal[j].Z = vnormales[objeto.faceList[i].vertexIndexes[j], 2];
                }
            }
        }

        public void VectorNormal(float[] V1, float[] V2, float[] V3, out float NormalX,
                                      out float NormalY, out float NormalZ)
        {
            float Qx, Qy, Qz, Px, Py, Pz;

            Px = V2[0] - V1[0];
            Py = V2[1] - V1[1];
            Pz = V2[2] - V1[2];
            Qx = V3[0] - V1[0];
            Qy = V3[1] - V1[1];
            Qz = V3[2] - V1[2];
            NormalX = Py * Qz - Pz * Qy;
            NormalY = Pz * Qx - Px * Qz;
            NormalZ = Px * Qy - Py * Qx;
        }

        float Modulo(float x, float y, float z)
        {
            float len;
            len = x * x + y * y + z * z;
            return ((float)Math.Sqrt(len));
        }

        public void Normalizar(ref float x, ref float y, ref float z)
        {
            float len;
            len = Modulo(x, y, z);
            len = 1.0f / len;
            x *= len;
            y *= len;
            z *= len;
        }

    }
}
