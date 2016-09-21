using System;
using System.Collections.Generic;
using System.Text;

namespace ShadowEngine.ContentLoading
{
    public interface IModelLoader
    {
        ModelContainer LoadModel(string modelPath);
    }
}
