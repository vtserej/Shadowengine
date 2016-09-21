using System;
using System.Collections.Generic;
using System.Text;

namespace ShadowEngine
{
    public interface IColisionable
    {
        List<CollisionPoint> ColitionPoints();

        List<CollisionSegment> ColitionSegments();  
    }
}
