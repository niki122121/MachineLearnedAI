using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using edgeClass;

namespace edgeWrapperClass
{
    public class EdgeWrapper
    {
        private Edge e;
        private Vector2 positionBack;
        private Vector2 positionForw;
        private float bias;


        public EdgeWrapper(Edge e_, Vector2 posb, Vector2 posf, float b){ e = e_; positionBack = posb; positionForw = posf; bias = b; }
        public Edge getE() { return e; }
        public Vector2 getPosB() { return positionBack; }
        public Vector2 getPosF() { return positionForw; }
        public float getBias() { return bias; }

        public override bool Equals(object obj)
        {
            EdgeWrapper vw = obj as EdgeWrapper;
            return vw != null && vw.getE() == this.getE();
        }

        public override int GetHashCode()
        {
            return this.getE().GetHashCode();
        }
    }
}
