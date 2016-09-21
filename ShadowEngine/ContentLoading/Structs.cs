using System;
using System.Collections.Generic;
using System.Text;

namespace ShadowEngine.ContentLoading
{
    public struct Vertex
    {
        public float x, y, z;  //point coordenates
        public float u, v;     //texture coordinates
    }

    public struct Face
    {
        public Vertex[] vertex;
        public Vector3[] normal;
    }

    public struct MeshIndexed
    {
        public FaceIndexed[] faceList;
        public Vertex[] vertexList;
        public TextureCoord[] textures;
        public Vector3[] normals;
        public string meshName;
        public string textureName;

        public void Allocate(uint nVerts, uint nFaces)
        {
            vertexList = new Vertex[nVerts];
            faceList = new FaceIndexed[nFaces];
            normals = new Vector3[nFaces * 3];
            textures = new TextureCoord[nVerts * 2]; 
        }

        public void SetVertexPosition(uint i, Vector3 v)
        {
            Vertex vertex = new Vertex();
            vertex.x = v.X;
            vertex.y = v.Y;
            vertex.z = v.Z;
            vertexList[i] = vertex;  
        }

        public void SetFaceIndicies(uint i, int a, int b, int c)
        {
            int[] indicies = { a, b, c };
            faceList[i].vertexIndexes = indicies;
        }

        public void SetVertexNormal(uint i, Vector3 v)
        {
            normals[i] = v; 
        }
    }

    public struct FaceIndexed
    {
        public int[] vertexIndexes;
        public int[] textureIndexes;
        public Vector3[] normal;
    }

    public struct TextureCoord
    {
        public float x;
        public float y;
    };
}
