using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vertexClass;
using edgeClass;

namespace vertexWrapperClass
{
    public class VertexWrapper
    {
        private Vertex v;
        private Vector2 pos;

        public VertexWrapper(Vertex v_, Vector2 pos_) { v = v_; pos = pos_; }
        public Vector2 getPos() { return pos; }
        public Vertex getV() { return v; }

        public override bool Equals(object obj)
        {
            VertexWrapper vw = obj as VertexWrapper;
            return vw != null && vw.getV() == this.getV();
        }

        public override int GetHashCode()
        {
            return this.getV().GetHashCode();
        }
    }
}
