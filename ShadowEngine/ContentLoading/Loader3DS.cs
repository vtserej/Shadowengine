using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;


namespace ShadowEngine.ContentLoading
{
    /// <summary>
    /// This class loads an OBJ file 
    /// </summary>
    public class Loader3DS : IModelLoader 
    {   
        #region constants

        //
        // Constants
        //
        public const uint kTextureFilenameSize = 256;
        public const uint kObjectNameSize = 200;
        public const uint kHandlerHashSize = 131;
        public const uint kFileContextMaxDepth = 16;
        public const uint kMaxStringLength = 255;

        //
        // Chunk definitions
        //
        private const ushort kChunkEmpty = 0x0000;

        private const ushort kMagic3ds = 0x4d4d;
        private const ushort kMagicS = 0x2d2d;
        private const ushort kMagicL = 0x2d3d;
        // mli file
        private const ushort kMagicLib = 0x3daa;
        private const ushort kMagicMat = 0x3dff;
        // prj file
        private const ushort kMagicC = 0xc23d;
        // start of actual objs
        private const ushort kChunkObjects = 0x3d3d;

        private const ushort kVersionMax = 0x0002;
        private const ushort kVersionKF = 0x0005;
        private const ushort kVersionMesh = 0x3d3e;

        private const ushort kColor3f = 0x0010;
        private const ushort kColor24 = 0x0011;
        private const ushort kColorLin24 = 0x0012;
        private const ushort kColorLin3f = 0x0013;
        private const ushort kIntPercent = 0x0030;
        private const ushort kFloatPercent = 0x0031;
        private const ushort kMasterScale = 0x0100;
        private const ushort kImageFile = 0x1100;
        private const ushort kAmbLight = 0x2100;

        // object chunks
        private const ushort kNamedObject = 0x4000;
        private const ushort kObjMesh = 0x4100;
        private const ushort kObjLight = 0x4600;
        private const ushort kObjCamera = 0x4700;

        private const ushort kMeshVerts = 0x4110;
        private const ushort kVertexFlags = 0x4111;
        private const ushort kMeshFaces = 0x4120;
        private const ushort kMeshMaterial = 0x4130;
        private const ushort kMeshTexVert = 0x4140;
        private const ushort kMeshSmoothGroup = 0x4150;
        private const ushort kMeshXFMatrix = 0x4160;
        private const ushort kMeshColorInd = 0x4165;
        private const ushort kMeshTexInfo = 0x4170;
        private const ushort kHeirarchy = 0x4F00;

        private const ushort kLightSpot = 0x4620;


        private const ushort kViewportLayout = 0x7001;
        private const ushort kViewportData = 0x7011;
        private const ushort kViewportData3 = 0x7012;
        private const ushort kViewportSize = 0x7020;


        // material chunks
        private const ushort kMat = 0xAFFF;
        private const ushort kMatName = 0xA000;
        private const ushort kMatAmb = 0xA010;
        private const ushort kMatDiff = 0xA020;
        private const ushort kMatSpec = 0xA030;
        private const ushort kMatShin = 0xA040;
        private const ushort kMatShinPow = 0xA041;
        private const ushort kMatTransparency = 0xA050;
        private const ushort kMatTransFalloff = 0xA052;
        private const ushort kMatRefBlur = 0xA053;
        private const ushort kMatEmis = 0xA080;
        private const ushort kMatTwoSided = 0xA081;
        private const ushort kMatTransAdd = 0xA083;
        private const ushort kMatSelfIlum = 0xA084;
        private const ushort kMatWireOn = 0xA085;
        private const ushort kMatWireThickness = 0xA087;
        private const ushort kMatFaceMap = 0xA088;
        private const ushort kMatTransFalloffIN = 0xA08A;
        private const ushort kMatSoften = 0xA08C;
        private const ushort kMatShader = 0xA100;
        private const ushort kMatTexMap = 0xA200;
        private const ushort kMatTexFLNM = 0xA300;
        private const ushort kMatTexTile = 0xA351;
        private const ushort kMatTexBlur = 0xA353;
        private const ushort kMatTexUscale = 0xA354;
        private const ushort kMatTexVscale = 0xA356;
        private const ushort kMatTexUoffset = 0xA358;
        private const ushort kMatTexVoffset = 0xA35A;
        private const ushort kMatTexAngle = 0xA35C;
        private const ushort kMatTexCol1 = 0xA360;
        private const ushort kMatTexCol2 = 0xA362;
        private const ushort kMatTexColR = 0xA364;
        private const ushort kMatTexColG = 0xA366;
        private const ushort kMatTexColB = 0xA368;

        // keyframe chunks

        // start of keyframe info
        private const ushort kChunkKeyFrame = 0xB000;
        private const ushort kAmbientNodeTag = 0xB001;
        private const ushort kObjectNodeTag = 0xB002;
        private const ushort kCameraNodeTag = 0xB003;
        private const ushort kTargetNodeTag = 0xB004;
        private const ushort kLightNodeTag = 0xB005;
        private const ushort kLTargetNodeTag = 0xB006;
        private const ushort kSpotlightNodeTag = 0xB007;

        private const ushort kKeyFrameSegment = 0xB008;
        private const ushort kKeyFrameCurtime = 0xB009;
        private const ushort kKeyFrameHdr = 0xB00A;
        private const ushort kKeyFrameNodeHdr = 0xB010;
        private const ushort kKeyFrameDummyName = 0xB011; // when tag is $$$DUMMY
        private const ushort kKeyFramePrescale = 0xB012;
        private const ushort kKeyFramePivot = 0xB013;
        private const ushort kBoundingBox = 0xB014;
        private const ushort kMorphSmooth = 0xB015;
        private const ushort kKeyFramePos = 0xB020;
        private const ushort kKeyFrameRot = 0xB021;
        private const ushort kKeyFrameScale = 0xB022;
        private const ushort kKeyFrameFov = 0xB023;
        private const ushort kKeyFrameRoll = 0xB024;
        private const ushort kKeyFrameCol = 0xB025;
        private const ushort kKeyFrameMorph = 0xB026;
        private const ushort kKeyFrameHot = 0xB027;
        private const ushort kKeyFrameFall = 0xB028;
        private const ushort kKeyFrameHide = 0xB029;
        private const ushort kKeyFrameNodeID = 0xB030;

        private const ushort kChunkEnd = 0xffff;



        private const byte AB = 0x04;
        private const byte BC = 0x02;
        private const byte CA = 0x01;
        private const byte UWRAP = 0x08;
        private const byte VWRAP = 0x10;

        #endregion

        #region Data types definition

        public struct Vert
        {
            public float x, y, z;
        };

        public class FileContext
        {
            public BinaryReader reader;
            public ChunkHeader chunk;
            public ChunkHeader[] stack;
            public ushort sp;

            public FileContext(BinaryReader input)
            {
                reader = input;
                stack = new ChunkHeader[kFileContextMaxDepth];
            }

            bool SeekSet(long where)
            {
                long pos = reader.BaseStream.Seek(where, SeekOrigin.Begin);
                return pos == where;
            }

            bool SeekRelative(long howfar)
            {
                long current = reader.BaseStream.Position;
                long pos = reader.BaseStream.Seek(howfar, SeekOrigin.Current);
                return howfar == (pos - current);
            }

            public long Position()
            {
                return reader.BaseStream.Position;
            }

            // operations on the stream
            public byte PopByte()
            {
                return reader.ReadByte();
            }

            public ushort PopWord()
            {
                ushort i = reader.ReadUInt16();
                return i;
            }

            public uint PopDword()
            {
                uint i = reader.ReadUInt32();
                return i;
            }

            public float PopFloat()
            {
                float f = reader.ReadSingle();
                return f;
            }

            public bool ReadChunk()
            {
                bool popped = false;
                uint pos = (uint)Position();

                // if we are requesting to read a chunk but we are at the end
                // of a super chunk (inside of a 'subhandler') pop the previous
                // chunk info and return false so that the subhandler will return
                // control
                while (sp > 0 && pos >= stack[sp - 1].off + stack[sp - 1].len)
                {
                    _PopChunk();
                    popped = true;
                }

                if (popped)
                {
                    return false;
                }

                // if the last header is valid and it's data
                // has not been fully consumed push the last header
                if (chunk.id != 0 && pos < chunk.off + chunk.len)
                {
                    _PushChunk();
                }

                if (!IsEOF())
                {
                    // read the new chunk header
                    chunk.id = PopWord();
                    chunk.len = PopDword();

                    if (chunk.id == 0 && IsEOF())
                    {
                        return false;
                    }

                    // save off the offset
                    chunk.off = (int)pos;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void SkipChunk()
            {
                uint pos = (uint)Position();
                if (pos < chunk.off + chunk.len)
                {
                    SeekSet(chunk.off + chunk.len);
                }
            }

            void _PushChunk()
            {
                Debug.Assert(sp < kFileContextMaxDepth);
                stack[sp++] = chunk;
            }

            void _PopChunk()
            {
                Debug.Assert(sp > 0);
                chunk = stack[--sp];
#if MAXLOADER_DEBUG
                ModelLoader.WriteDebug("FileContext::_PopChunk(): SP: [{0:d02}], ID: 0x{1:x04}, OFF: {2:d5}, LEN: {3:d5}",
                    sp, chunk.id, chunk.off, chunk.len);
#endif
            }

            // Convert chunk tag id to string
            string _GetName(ushort id)
            {
                return "<unknown>";
            }

            public string PopString()
            {
                byte c = 0;
                int i = 0;
                StringBuilder buf = new StringBuilder((int)kMaxStringLength);

                while (i < kMaxStringLength)
                {
                    c = PopByte();
                    if (0 == c)
                    {
                        break;
                    }
                    else
                    {
                        buf.Append((char)c);
                    }
                }

                // read any remaining bytes (incase len is not enough to read the entire string)
                while (c != 0)
                {
                    c = PopByte();
                }

                return buf.ToString();
            }

            bool IsEOF()
            {
                return reader.BaseStream.Position >= reader.BaseStream.Length;
            }
        }

        interface IChunkHandler
        {
            bool ProcessChunk(FileContext ctx);
        }

        public class MeshObject : IChunkHandler
        {
            public string name;
            public ushort mnv;
            public float[] mc;
            public Matrix lmat;
            public Vertex[] verts;
            public ushort nVerts;
            public Face3S[] faces;
            public ushort nFaces;
            public List<FaceMaterial> faceMaterials;

            public MeshObject(string objname)
            {
                name = objname;
                lmat = Matrix.Identity();
                faceMaterials = new List<FaceMaterial>();
            }

            public bool ProcessChunk(FileContext ctx)
            {
                while (ctx.ReadChunk())
                {
                    switch (ctx.chunk.id)
                    {
                        case kMeshVerts:
                            {
                                nVerts = ctx.PopWord();
                                verts = new Vertex[nVerts];

                                for (ushort i = 0; i < nVerts; i++)
                                {
                                    // flip y and z
                                    verts[i] = new Vertex();
                                    verts[i].x = ctx.PopFloat();
                                    verts[i].z = ctx.PopFloat();
                                    verts[i].y = ctx.PopFloat();
                                }
                            } break;
                        case kMeshFaces:
                            {
                                nFaces = ctx.PopWord();
                                faces = new Face3S[nFaces];

                                // Read all the faces
                                for (ushort i = 0; i < nFaces; i++)
                                {
                                    // wind faces CCW
                                    faces[i].c = ctx.PopWord();
                                    faces[i].b = ctx.PopWord();
                                    faces[i].a = ctx.PopWord();

                                    faces[i].flags = ctx.PopWord();
                                    faces[i].material = 0;
                                }
                            }
                            break;
                        case kMeshMaterial:
                            {
                                // Material mapping Info.
                                FaceMaterial fmat = new FaceMaterial();
                                fmat.name = ctx.PopString();
                                fmat.ne = ctx.PopWord();
                                fmat.faces = new short[fmat.ne];

                                // read all faces (actually indices to the face list)
                                for (ushort i = 0; i < fmat.ne; ++i)
                                {
                                    fmat.faces[i] = (short)ctx.PopWord();
                                }

                                faceMaterials.Add(fmat);
                            } break;
                        case kMeshTexVert:
                            {
                                // Material mapping coords
                                int i;
                                ushort imnv, off;
                                imnv = mnv = ctx.PopWord(); // No. of mapping coords
                                mc = new float[mnv * 2];

                                // Read mapping coords
                                // These are actually texture coords for vertices.
                                for (i = 0; i < imnv; i++)
                                {
                                    off = (ushort)(i * 2);
                                    mc[off + 0] = ctx.PopFloat(); // u 
                                    mc[off + 1] = ctx.PopFloat(); // v
                                }
                            }
                            break;
                        case kMeshXFMatrix:
                            {
                                // Local transformation matrix
                                int i, j;
                                float[][] mat = new float[4][];
                                for (i = 0; i < 4; i++)
                                {
                                    for (j = 0; j < 3; j++)
                                    {
                                        mat[i] = new float[4];
                                        mat[i][j] = ctx.PopFloat();
                                    }
                                }

                                lmat = new Matrix(mat[0][0], mat[0][1], mat[0][2], mat[0][3],
                                                     mat[1][0], mat[1][1], mat[1][2], mat[1][3],
                                                     mat[2][0], mat[2][1], mat[2][2], mat[2][3],
                                                     mat[3][0], mat[3][1], mat[3][2], mat[3][3]);
                            }
                            break;
                        case kMeshSmoothGroup:
                            {
                                for (ushort i = 0; i < nFaces; ++i)
                                {
                                    faces[i].smooth = (ushort)ctx.PopDword();
                                }
                            }
                            break;
                        default:
                            ctx.SkipChunk();
                            break;
                    }
                }
                return true;
            }
        }

        class FileHeader
        {
            ushort magic;
            byte[] reserved = new byte[10];
            uint version;

            public void Read(BinaryReader input)
            {
                magic = input.ReadUInt16();
                reserved = input.ReadBytes(10);
                version = input.ReadUInt32();

                if (magic != kMagic3ds)
                {
                    throw new Exception("Invalid magic in 3ds file header: 0x" + magic.ToString("X8"));
                }

                if (version != 3)
                {
                    throw new Exception("Invalid 3DS file version: 0x " + version.ToString("X8"));
                }
            }
        };

        public struct ChunkHeader
        {
            public ushort id;
            public uint len;
            public int off;
        };

        public struct Face3S
        {
            public ushort a, b, c;
            public ushort flags;
            public ushort material;
            public ushort smooth;
        };

        public class FaceMaterial : IChunkHandler
        {
            public string name;
            public ushort ne;
            public short[] faces;

            public FaceMaterial()
            {
            }

            public bool ProcessChunk(FileContext ctx)
            {
                // read all of the face material chunks available
                while (ctx.ReadChunk())
                {
                    ctx.SkipChunk();
                }
                return true;
            }
        }

        class Material : IChunkHandler
        {
            public string name;
            public Color24 ambient;
            public Color24 diffuse;
            public Color24 specular;
            public List<TextureMap> textures;

            public Material()
            {
                ambient = new Color24();
                diffuse = new Color24();
                specular = new Color24();
                textures = new List<TextureMap>();
            }

            public bool ProcessChunk(FileContext ctx)
            {
                while (ctx.ReadChunk())
                {
                    switch (ctx.chunk.id)
                    {
                        case kNamedObject:      // (object block
                            ctx.sp = 1;
                            return false; 
                            break;
                        case kMatName:
                            name = ctx.PopString();
                            break;
                        case kMatAmb:
                            ambient.ProcessChunk(ctx);
                            break;
                        case kMatDiff:
                            diffuse.ProcessChunk(ctx);
                            break;
                        case kMatSpec:
                            specular.ProcessChunk(ctx);
                            break;
                        case kMatTexMap:
                            {
                                TextureMap tex = new TextureMap();
                                if (tex.ProcessChunk(ctx))
                                {
                                    textures.Add(tex);
                                }
                                break; 
                            }

                        default:
                            ctx.SkipChunk();
                            break;
                    }
                }

                return true;
            }
        }

        class TextureMap : IChunkHandler
        {
            public string name;
            public float u;
            public float v;
            public float uoff;
            public float voff;
            public float rot;

            public TextureMap()
            {
            }

            public bool ProcessChunk(FileContext ctx)
            {
                while (ctx.ReadChunk())
                {
                    switch (ctx.chunk.id)
                    {
                        case kMatTexFLNM:
                            name = ctx.PopString();
                            break;
                        case kMatTexUscale:
                            u = ctx.PopFloat();
                            break;
                        case kMatTexVscale:
                            v = ctx.PopFloat();
                            break;
                        case kMatTexUoffset:
                            uoff = ctx.PopFloat();
                            break;
                        case kMatTexVoffset:
                            voff = ctx.PopFloat();
                            break;
                        case kMatTexAngle:
                            rot = ctx.PopFloat();
                            break;
                        default:
                            ctx.SkipChunk();
                            break;
                    }
                }

                return true;
            }
        }

        class Color24 : IChunkHandler
        {
            public byte r;
            public byte g;
            public byte b;

            public bool ProcessChunk(FileContext ctx)
            {
                if (ctx.chunk.id == kColor24)
                {
                    r = ctx.PopByte();
                    g = ctx.PopByte();
                    b = ctx.PopByte();
                }

                // in the case when this was deferred (material colors)
                while (ctx.ReadChunk())
                {
                    switch (ctx.chunk.id)
                    {
                        case kColor24:
                            r = ctx.PopByte();
                            g = ctx.PopByte();
                            b = ctx.PopByte();
                            break;
                        default:
                            ctx.SkipChunk();
                            break;
                    }
                }

                return true;
            }
        }

        class Color3f : IChunkHandler
        {
            public float r;
            public float g;
            public float b;

            public bool ProcessChunk(FileContext ctx)
            {
                if (ctx.chunk.id == kColor3f)
                {
                    r = ctx.PopFloat();
                    g = ctx.PopFloat();
                    b = ctx.PopFloat();
                }

                // in the case when this was deferred (material colors)
                while (ctx.ReadChunk())
                {
                    switch (ctx.chunk.id)
                    {
                        case kColor3f:
                            r = ctx.PopFloat();
                            g = ctx.PopFloat();
                            b = ctx.PopFloat();
                            break;
                        default:
                            ctx.SkipChunk();
                            break;
                    }
                }

                return true;
            }
        }

        #endregion

        #region private attributes

        float _masterScale;
        uint _meshVersion;

        List<MeshObject> _meshes;
        List<Material> _materials;
        List<TextureMap> _textures;

        #endregion

        public Loader3DS()
        {
            _meshes = new List<MeshObject>();
            _materials = new List<Material>();
            _textures = new List<TextureMap>();
        }

        public ModelContainer LoadModel(string path)
        {
            List<MeshIndexed> _meshesIndexed = new List<MeshIndexed>(); 
            Loader3DS loader = new Loader3DS();
            loader.Load(new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)));
            loader.LoadAllMeshes(_meshesIndexed, 3.0f);
            List<Mesh> meshes = new List<Mesh>();
            foreach (var item in _meshesIndexed)
            {
                meshes.Add(CopyMesh(item));   
            }

            return new ModelContainer(meshes, path);
        }

        Mesh CopyMesh(MeshIndexed meshIndexed)
        {
            Mesh mesh = new Mesh();
            mesh.Name = meshIndexed.meshName;       
            mesh.Faces = new Face[meshIndexed.faceList.Length];
            for (int i = 0; i < mesh.Faces.Length; i++)
            {
                Vertex[] vertex = new Vertex[3];
                Vector3[] normal = new Vector3[3];
                for (int j = 0; j < 3; j++)
                {
                    vertex[j] = meshIndexed.vertexList[meshIndexed.faceList[i].vertexIndexes[j]];
                    normal[j] = meshIndexed.normals[meshIndexed.faceList[i].vertexIndexes[j]];
                    vertex[j].u =  meshIndexed.textures[meshIndexed.faceList[i].vertexIndexes[j]].x;
                    vertex[j].v =  meshIndexed.textures[meshIndexed.faceList[i].vertexIndexes[j]].y;
                }
                mesh.Faces[i].vertex = vertex;
                mesh.Faces[i].normal = normal;
            }
            return mesh;
        }

        private void ResetObjects()
        {
            _meshes.Clear();
            _materials.Clear();
            _textures.Clear();

            _masterScale = 1.0f;
            _meshVersion = 0;
        }

        private void ReadObjects(FileContext ctx)
        {
            string name = "<unnamed>";

            ResetObjects();

            while (ctx.ReadChunk())
            {
                switch (ctx.chunk.id)
                {
                  case kNamedObject:      // (object block
                        name = ctx.PopString();
                        break;
                    case kObjMesh:
                        {
                            MeshObject mesh = new MeshObject(name);
                            if (mesh.ProcessChunk(ctx))
                            {
                                _meshes.Add(mesh);
                            }
                        }
                        break;
                    case kMat:      // material object
                        {
                            Material mat = new Material();
                            if (mat.ProcessChunk(ctx))
                            {
                                _materials.Add(mat);
                            }
                        }
                        break;
                    case kVersionMesh:
                        _meshVersion = ctx.PopDword();
                        break;
                    case kMasterScale:
                        _masterScale = ctx.PopFloat();
                        break;
                    case kChunkKeyFrame:
                        ReadKeyFrameData(ctx);
                        break;
                    default:
                        ctx.SkipChunk();
                        break;
                }
            }
        }

        private void ReadKeyFrameData(FileContext ctx)
        {
            while (ctx.ReadChunk())
            {
                switch (ctx.chunk.id)
                {
                    case kKeyFrameHdr:
                        {
                            ushort revision = ctx.PopWord();
                            string cstr = ctx.PopString();
                            uint animlen = ctx.PopDword();
                            Loader3DS.WriteDebug("KeyFrame Header: rev: 0x{0:x04}, animlen: 0x{1:x08}, cstr: {2}", revision, animlen, cstr);
                        }
                        break;
                    case kKeyFrameSegment:
                        {
                            uint start = ctx.PopDword();
                            uint end = ctx.PopDword();
                            Loader3DS.WriteDebug("KeyFrame Segment: start: 0x{0:x08}, end: 0x{1:x08}", start, end);
                        }
                        break;
                    case kKeyFrameCurtime:
                        {
                            uint curtime = ctx.PopDword();
                            Loader3DS.WriteDebug("KeyFrame curtime: 0x{0:x08}", curtime);
                        }
                        break;
                    case kObjectNodeTag:
                        ReadObjectNode(ctx);
                        break;
                    default:
                        ctx.SkipChunk();
                        break;
                }
            }
        }

        private void ReadObjectNode(FileContext ctx)
        {
            string name = "<unnamed>";

            while (ctx.ReadChunk())
            {
                switch (ctx.chunk.id)
                {
                    case kKeyFrameNodeID:
                        {
                            ushort nodeID = ctx.PopWord();
                            Loader3DS.WriteDebug("KeyFrame ID: 0x{0:x04}", nodeID);
                        }
                        break;
                    case kKeyFrameNodeHdr:
                        {
                            name = ctx.PopString();
                            ushort flags1 = ctx.PopWord();
                            ushort flags2 = ctx.PopWord();
                            ushort heirarchy = ctx.PopWord();
                            Loader3DS.WriteDebug("KeyFrame NodeHdr: {0}, flags1: 0x{1:x04}, flags2: 0x{2:x04}, heirarchy: 0x{3:x04}", name, flags1, flags2, heirarchy);
                        }
                        break;
                    case kKeyFramePos:
                        {
                            ushort flags = ctx.PopWord();
                            ushort[] unk = new ushort[4];
                            unk[0] = ctx.PopWord();
                            unk[1] = ctx.PopWord();
                            unk[2] = ctx.PopWord();
                            unk[3] = ctx.PopWord();
                            ushort keys = ctx.PopWord();
                            ushort unk2 = ctx.PopWord();
                            ushort framenum = ctx.PopWord();
                            uint unk3 = ctx.PopDword();
                            float x = ctx.PopFloat();
                            float y = ctx.PopFloat();
                            float z = ctx.PopFloat();

                            Loader3DS.WriteDebug("KeyFrame POS: flags: 0x{0:x04}, keys: 0x{1:x04}, frame: 0x{2:x04}", flags, keys, framenum);
                            Loader3DS.WriteDebug("KeyFrame POS: x: {0:f2}, y: {1:f2}, z: {2:f2}", x, y, z);
                            Loader3DS.WriteDebug("KeyFrame POS: unk[{0},{1},{2},{3}], unk2: {4}, unk3: {5}", unk[0], unk[1], unk[2], unk[3], unk2, unk3);

                            if (framenum == 0 && name != String.Empty)
                            {
                                MeshObject mesh;
                                if (LookupMeshByName(name, out mesh))
                                {
                                    Matrix tranmtx = Matrix.CreateTranslation(-x, -y, -z);
                                    //mesh.lmat *= tranmtx;
                                }
                            }
                        }
                        break;
                    case kKeyFramePivot:
                        {
                            float x = ctx.PopFloat();
                            float y = ctx.PopFloat();
                            float z = ctx.PopFloat();
                            Loader3DS.WriteDebug("KeyFrame Pivot: x: {0:f2}, y: {1:f2}, z: {2:f2}", x, y, z);

                            if (name != String.Empty)
                            {
                                MeshObject mesh;
                                if (LookupMeshByName(name, out mesh))
                                {
                                    Matrix tranmtx = Matrix.CreateTranslation(-x, -y, -z);
                                    mesh.lmat *= tranmtx;
                                }
                            }
                        }
                        break;
                    case kKeyFrameRot:
                        {
                            ushort flags = ctx.PopWord();
                            ushort[] unk = new ushort[4];
                            unk[0] = ctx.PopWord();
                            unk[1] = ctx.PopWord();
                            unk[2] = ctx.PopWord();
                            unk[3] = ctx.PopWord();
                            ushort keys = ctx.PopWord();
                            ushort unk2 = ctx.PopWord();
                            ushort framenum = ctx.PopWord();
                            uint unk3 = ctx.PopDword();
                            float r = ctx.PopFloat();
                            float x = ctx.PopFloat();
                            float y = ctx.PopFloat();
                            float z = ctx.PopFloat();


                            if (framenum == 0 && name != String.Empty)
                            {
                                MeshObject mesh;
                                 if (LookupMeshByName(name, out mesh))
                                {
                                    Matrix rotmtx = Matrix.CreateFromAxisAngle(new Vector3(x, y, z), r);
                                    mesh.lmat *= rotmtx;
                                }
                            }
                        }
                        break;
                    case kKeyFrameScale:
                        {
                            ushort flags = ctx.PopWord();
                            ushort[] unk = new ushort[4];
                            unk[0] = ctx.PopWord();
                            unk[1] = ctx.PopWord();
                            unk[2] = ctx.PopWord();
                            unk[3] = ctx.PopWord();
                            ushort keys = ctx.PopWord();
                            ushort unk2 = ctx.PopWord();
                            ushort framenum = ctx.PopWord();
                            uint unk3 = ctx.PopDword();
                            float x = ctx.PopFloat();
                            float y = ctx.PopFloat();
                            float z = ctx.PopFloat();

                            if (framenum == 0 && name != String.Empty)
                            {
                                MeshObject mesh;
                                if (LookupMeshByName(name, out mesh))
                                {
                                    Matrix sclmtx = Matrix.CreateScale(1 / x, 1 / y, 1 / z);
                                    //mesh.lmat *= sclmtx;;
                                }
                            }
                        }
                        break;
                    case kKeyFrameFov:
                    case kKeyFrameRoll:
                    case kKeyFrameCol:
                    case kKeyFrameMorph:
                    default:
                        ctx.SkipChunk();
                        break;
                }
            }
        }

        private bool LookupMeshByName(string name, out MeshObject outMesh)
        {
            outMesh = null;

            foreach (MeshObject mesh in _meshes)
            {
                if (mesh.name == name)
                {
                    outMesh = mesh;
                    return true;
                }
            }

            return false;
        }

        private bool LookupMaterialByName(string name, out Material outMaterial)
        {
            outMaterial = null;

            foreach (Material mat in _materials)
            {
                if (mat.name == name)
                {
                    outMaterial = mat;
                    return true;
                }
            }

            return false;
        }

        public void Load(BinaryReader reader)
        {
            // char buf[6] = ".PMF";
            // short ver = 0x0002;
            // Debug.Assert(filename);
            FileContext ctx = new FileContext(reader);
            FileHeader header = new FileHeader();

            header.Read(reader);

            while (ctx.ReadChunk())
            {
                switch (ctx.chunk.id)
                {
                    case kChunkEnd:
                        break;
                    case kChunkObjects:
                        ReadObjects(ctx);
                        break;
                    default:
                        break;
                }
            }
        }

        public void LoadMesh(string name, MeshIndexed mesh, float scale)
        {
            MeshObject meshobj = null;

            for (int i = 0; i < _meshes.Count; ++i)
            {
                meshobj = _meshes[i];   
                if (meshobj.name == name)
                {
                    LoadMesh((uint)i, mesh, scale, false);
                }
            }

            if (null == meshobj)
            {
                throw new Exception("No such named mesh object: " + name);
            }
        }

        public void LoadAllMeshes(List<MeshIndexed> outMeshes, float scale)
        {
            uint i;

            for (i = 0; i < _meshes.Count; ++i)
            {
                MeshIndexed mesh = new MeshIndexed();
                mesh = LoadMesh(i, mesh, scale, false);
                outMeshes.Add(mesh);
            }
        }

        public MeshIndexed LoadMesh(uint i, MeshIndexed meshIndexed, float scale, bool applySmoothingGroups)
        {           
            Debug.Assert(i < _meshes.Count);

            MeshObject meshobj = _meshes[(int)i];

            meshIndexed.meshName = meshobj.name;   
 
            meshIndexed.Allocate(meshobj.nVerts, meshobj.nFaces);

            // scale and transform the mesh object
            for (i = 0; i < meshobj.nVerts; ++i)
            {
                Vector3 v = new Vector3(meshobj.verts[i].x * -1, meshobj.verts[i].y, meshobj.verts[i].z); 
                meshIndexed.SetVertexPosition(i, v);
                meshIndexed.textures[i].y = meshobj.mc[i * 2];
                meshIndexed.textures[i].x = meshobj.mc[(i * 2) + 1];
            }

            // Buffers for calculating smoothing groups
            ushort[] sharedFaces = new ushort[meshIndexed.vertexList.Length];
            Vector3[] summedNormals = new Vector3[meshIndexed.vertexList.Length];
            Vector3 v1, v2, normal;

            for (i = 0; i < meshobj.nFaces; ++i)
            {
                Face3S f = meshobj.faces[i];

                // copy face information
                meshIndexed.SetFaceIndicies(i, meshobj.faces[i].a, meshobj.faces[i].b, meshobj.faces[i].c);

                // get the normal with the cross product of two verts
                // on the same face

                Vector3 a = new Vector3(meshobj.verts[f.a].x, meshobj.verts[f.a].y, meshobj.verts[f.a].z);
                Vector3 b = new Vector3(meshobj.verts[f.b].x, meshobj.verts[f.b].y, meshobj.verts[f.b].z);
                Vector3 c = new Vector3(meshobj.verts[f.c].x, meshobj.verts[f.c].y, meshobj.verts[f.c].z);

                v1 = b - c;
                v2 = a - b;

                // calculate the normal, sum it in the normal
                // array and 
                normal = Vector3.Cross(v1, v2) * -1;
                normal.Normalize();

                meshIndexed.SetVertexNormal(f.a, normal);
                meshIndexed.SetVertexNormal(f.b, normal);
                meshIndexed.SetVertexNormal(f.c, normal);

                sharedFaces[f.a]++;
                sharedFaces[f.b]++;
                sharedFaces[f.c]++;

                summedNormals[f.a] += normal;
                summedNormals[f.b] += normal;
                summedNormals[f.c] += normal;
            }


            if (applySmoothingGroups)
            {
                // apply the smoothing groups
                for (int iv = 0; iv < meshIndexed.vertexList.Length; ++iv)
                {
                    // get the weighted normal
                    Vector3 n = summedNormals[iv];
                    n = n / sharedFaces[iv];

                    // renorm
                    n.Normalize();

                    meshIndexed.SetVertexNormal(i, n);
                }
            }

            // Lookup and assign materials groups, this will convert the material
            // group to something that can be rendered quickly and has to happen
            // after we have already loaded all the face->vert indexes
            foreach (FaceMaterial material in meshobj.faceMaterials)
            {
                Material mat;
                if (LookupMaterialByName(material.name, out mat))
                {
                    meshIndexed.textureName = mat.name;    
            //        mesh.AddMaterialGroup(
            //            material.faces,
            //            new Color(mat.ambient.r, mat.ambient.g, mat.ambient.b),
            //            new Color(mat.diffuse.r, mat.diffuse.g, mat.diffuse.b),
            //            new Color(mat.specular.r, mat.diffuse.g, mat.diffuse.b));
                }
            }
            return meshIndexed; 
        }

        protected static void WriteDebug(string line, params object[] args)
        {
            string formatted = String.Format(line, args);
            Debug.WriteLine(formatted);
        }
    }
}
