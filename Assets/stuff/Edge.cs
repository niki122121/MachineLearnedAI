using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vertexClass;

namespace edgeClass
{
    public class Edge
    {
        private float weight;
        private Vertex forward;
        //private Vertex backward;

        public Edge()
        {
            weight = 0.0f;
        }
        public Edge(Vertex f)
        {
            weight = 0.0f;
            forward = f;
            //backward = b;
        }

        public Edge(Vertex f, float w)
        {
            weight = w;
            forward = f;
            //backward = b;
        }

        public float getWeight()
        {
            return weight;
        }
        public void setWeight(float w)
        {
            weight = w;
        }

        public Vertex getForward()
        {
            return forward;
        }
        public void setForward(Vertex f)
        {
            forward = f;
        }

        /*
        public Vertex getBackward()
        {
            return backward;
        }
        public void setBackward(Vertex b)
        {
            backward = b;
        }
        */
    }
}
