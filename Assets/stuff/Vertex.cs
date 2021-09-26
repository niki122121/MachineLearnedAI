using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using edgeClass;

namespace vertexClass
{
    public class Vertex
    {
        private string name;
        private float value;
        private List<Edge> forwVertexes;

        public Vertex()
        {
            name = "";
            value = 0.0f;
            forwVertexes = new List<Edge>();
        }
        public Vertex(string n)
        {
            name = n;
            value = 0.0f;
            forwVertexes = new List<Edge>();
        }

        public string getName()
        {
            return name;
        }
        public void setName(string n)
        {
            name = n;
        }

        public float getValue()
        {
            return value;
        }
        public void setValue(float v)
        {
            value = v;
            //onSetValue();
            //change value of forward vertexes if it has fwv
            if (forwVertexes.Count > 0)
            {
                foreach (Edge forwardEdge in forwVertexes)
                {
                    Vertex auxForw = forwardEdge.getForward();
                    auxForw.setValue(auxForw.getValue() + (getValue() * forwardEdge.getWeight())); //there are negative values so dont normalize them
                }
                value = 0.0f;
            }
        }
        /*
        private void onSetValue()
        {
            if (forwVertexes.Count > 0)
            {
                foreach (Edge forwardEdge in forwVertexes)
                {
                    Vertex auxForw = forwardEdge.getForward();
                    auxForw.setValue(auxForw.getValue() + (getValue() * forwardEdge.getWeight())); //there are negative values so dont normalize them
                }
                value = 0.0f;
            }
        }*/

        public void checkSetValue(float check)
        {
            value = check;
            if (forwVertexes.Count > 0)
            {
                foreach (Edge forwardEdge in forwVertexes)
                {
                    Vertex auxForw = forwardEdge.getForward();
                    auxForw.checkSetValue(check);
                }
            }
        }

        public List<Edge> getForwVertexes()
        {
            return forwVertexes;
        }
        public void setForward(List<Edge> fv)
        {
            forwVertexes = fv;
        }
        public void addForwardEdge(Edge fe)
        {
            forwVertexes.Add(fe);
        }
        public void removeForwardEdge(Edge fe)
        {
            forwVertexes.Remove(fe);
        }

        public bool isForwardVertex(Vertex fv)
        {
            bool check = false;
            foreach (Edge fedge in forwVertexes)
            {
                if (fedge.getForward() == fv)
                {
                    check = true;
                }
            }
            return check;
        }

        public void printVertex()
        {
            Debug.Log(name.ToUpper() + "  ||  value:  " + value);
            Debug.Log("-----------------------------");
            Debug.Log("Forward Links(" + forwVertexes.Count+"): ");
            foreach (Edge forwVertex in forwVertexes)
            {
                Debug.Log(forwVertex.getForward().getName() + "  ||  value:  " + forwVertex.getForward().getValue() + "  ||  edgeWeight:  " + forwVertex.getWeight());
            }
            Debug.Log("-----------------------------");
        }

    }
}
