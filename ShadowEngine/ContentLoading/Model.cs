using System;
using System.Collections.Generic;
using System.Text;

namespace ShadowEngine.ContentLoading
{
    public abstract class Model 
    {
        protected string name;
        protected string file;
        protected float scale = 1;
        protected List<Mesh> meshes = new List<Mesh>();
    }
}
