using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using vertexClass;
using edgeClass;

namespace nueralGraphClass
{
    public class NeuralGraph
    {
        private List<Vertex> vertexes;
        private int _inputs;
        private int _outputs;

        public NeuralGraph()
        {
            vertexes = new List<Vertex>();
        }
        public Vertex getOutVertex() { return vertexes[vertexes.Count - _outputs]; }
        public Vertex getOutVertex(int n) { return vertexes[vertexes.Count - _outputs + n]; }

        public List<Vertex> getVertexes() { return vertexes; }
        public int getInCount() { return _inputs; }
        public int getOutCount() { return _outputs; }

        public Vertex addVertex(Vertex v)
        {
            vertexes.Insert((vertexes.Count - _outputs), v);
            return v;
        }
        public Vertex addVertex(Vertex v, bool isInput, bool IsOutput)  //inputs.Add(v);    //outputs.Add(v);
        {
            if (isInput) { vertexes.Insert(_inputs, v); _inputs++; }
            if (IsOutput) { vertexes.Add(v); _outputs++; }
            if (!isInput && !IsOutput) { addVertex(v); }
            return v;
        }
        public void removeVertex(Vertex v)
        {
            vertexes.Remove(v);
            v = null;
        }
        public void removeVertex(int n)
        {
            Vertex v = vertexes[n];
            removeVertex(v);
        }

        private bool isLinked(Vertex backward, Vertex forward)
        {
            foreach(Edge e in backward.getForwVertexes())
            {
                if (e.getForward() == forward)
                    return true;
            }
            return false;
        }
        public void link(Vertex backward, Vertex forward)
        {
            Edge link = new Edge(forward);
            backward.addForwardEdge(link);
        }
        public void link(Vertex backward, Vertex forward, float weight)
        {
            Edge link = new Edge(forward, weight);
            backward.addForwardEdge(link);
        }
        public void link(Vertex backward, Vertex forward, float weight, float checker)
        {
            forward.checkSetValue(checker);
            if (backward.getValue() != checker && !isLinked(backward, forward))
            {
                Edge link = new Edge(forward, weight);
                backward.addForwardEdge(link);
            }
        }

        public void structureInit(float variation)
        {
            foreach(Vertex v in vertexes)
            {
                foreach(Edge backwEdge in v.getForwVertexes())
                {
                    backwEdge.setWeight(Random.Range(-variation, variation));
                }
            }
        }

        public void structureInitTest()
        {
            foreach (Vertex v in vertexes)
            {
                foreach (Edge backwEdge in v.getForwVertexes())
                {
                    backwEdge.setWeight(1.0f);
                }
            }
        }

        public void structureModify(float variation)
        {
            foreach (Vertex v in vertexes)
            {
                foreach (Edge backwEdge in v.getForwVertexes())
                {
                    float primaryParams = Random.Range(0.0f, variation);
                    float secondaryParams = Random.Range(-primaryParams, primaryParams);

                    backwEdge.setWeight(backwEdge.getWeight() + secondaryParams);
                }
            }
        }

        private void removeRandomVertex()
        {
            removeVertex(Random.Range(_inputs, vertexes.Count - _inputs - _outputs));
        }

        private void removeRandomEdge()
        {
            int randVertex = Random.Range(_inputs, vertexes.Count - _inputs - _outputs);
            int randEdge = Random.Range(0, vertexes[randVertex].getForwVertexes().Count);

            vertexes[randVertex].getForwVertexes().RemoveAt(randEdge);
        }
        public void deMutate(float deLink, float deVertex)  //use spearingly
        {
            float rand = Random.Range(0.0f, 1.0f);
            float trueDeLink = Mathf.Clamp(deLink, 0, 1);
            float trueDeVertex = Mathf.Clamp(deVertex, 0, 1);
            if (trueDeVertex > rand) { removeRandomVertex(); }
            else if (trueDeLink > rand) { removeRandomEdge(); }
        }

        public float vertexRandomLink(Vertex toLink, float variation, int maxLinksIfVertex, float checker)
        {
            int randVertexes = Random.Range(1, maxLinksIfVertex + 1);
            randVertexes = Random.Range(1, randVertexes);

            float coinToss = Random.Range(0.0f, 1.0f);
            int other = 0;

            float trueChecker = checker;
            for (int i = 0; i < randVertexes; i++)
            {
                if (coinToss <= 0.5f)
                {
                    other = Random.Range(0, vertexes.Count);
                    link(vertexes[other], toLink, variation, trueChecker);
                }
                else
                {
                    other = Random.Range(0, vertexes.Count);
                    link(toLink, vertexes[other], variation, -trueChecker);
                }
                trueChecker++;
            }
            return trueChecker;
        }
        private void mutateRecursive(float variation, float vertexChance, int maxLinksIfVertex, Vertex firstVertex, Vertex secondVertex, float recursiveChecker, float vertexChecker)
        {
            float vChance = Mathf.Clamp(vertexChance, 0, 1);
            float randomP = Random.Range(0.0f, 1.0f);
            if (vChance < randomP)
            {
                float trueVariation = Random.Range(0.0f, variation);
                trueVariation = Random.Range(-trueVariation, trueVariation);
                link(firstVertex, secondVertex, trueVariation, 1);
            }
            else
            {
                Vertex toAdd = new Vertex("newIntermediate");
                link(firstVertex, toAdd, Random.Range(-variation, variation), recursiveChecker);
                float vertexCheckTracker = vertexRandomLink(toAdd, variation, maxLinksIfVertex, vertexChecker);
                addVertex(toAdd);
                mutateRecursive(variation, vertexChance, maxLinksIfVertex, toAdd, secondVertex, recursiveChecker + 1, vertexCheckTracker);
            }
        }
        public void mutate(float variation, float vertexChance, int maxLinksIfVertex)
        {
            int firstV = Random.Range(0, vertexes.Count);
            int secV = Random.Range(0, vertexes.Count - 1);
            if (secV >= firstV) { secV++; }
            mutateRecursive(variation, vertexChance, maxLinksIfVertex, vertexes[firstV], vertexes[secV], 1.5f, 10);
        }

        public void input(List<float> inpts, float detLeng)
        {
            if (_inputs == inpts.Count)
            {
                for (int i = 0; i < _inputs; i++)
                {
                    vertexes[i].setValue(inpts[i]/detLeng);
                }
            }
            else
            {
                Debug.Log("input mismatch: " + _inputs + " != " + inpts.Count);
            }
        }

        public float output()
        {
            float aux = getOutVertex().getValue();
            getOutVertex().setValue(0.0f);
            return aux;
        }

        public float output(int n)
        {
            float aux = getOutVertex(n).getValue();
            getOutVertex(n).setValue(0.0f);
            return aux;
        }

        public Vertex findByName(string n)
        {
            return vertexes.Find(s => s.getName().Contains(n));
        }

        public List<Vertex> findAllByName(string n)
        {
            return vertexes.FindAll(s => s.getName().Contains(n));
        }

        public float getWeightBetween(Vertex v1, Vertex v2)
        {
            if (v1 == v2)
            {
                return 1234.567f;
            }
            else
            {
                foreach (Edge e in v1.getForwVertexes())
                {
                    if(e.getForward() == v2)
                    {
                        return e.getWeight();
                    }
                }
                return 1234.567f; //in case no fowrwads nodes exist
            }
        }

        public void printGraph()
        {
            Debug.Log("Graph Length:  " + vertexes.Count + "_______________________________________________________________");
            foreach (Vertex v in vertexes)
            {
                Debug.Log("________________________________________________________________________________");
                v.printVertex();
                Debug.Log("________________________________________________________________________________");
            }
            Debug.Log("End Print  ______________________________________________________________________");
        }

        /*
        public void printEdgeValues()
        {
            Debug.Log("______________________________________________________________________________________");
            foreach (Vertex v in vertexes)
            {
                foreach (Edge e in v.getBackwVertexes())
                {
                    Debug.Log("vertexes: " + e.getBackward().getName() + " | " + e.getForward().getName() + "          wieght: "+e.getWeight());
                }
            }
            Debug.Log("______________________________________________________________________________________");
        }*/

    }
}
