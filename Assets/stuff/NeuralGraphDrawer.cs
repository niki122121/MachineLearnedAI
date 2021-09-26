using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using nueralGraphClass;
using vertexClass;
using edgeClass;

namespace neuralGraphDrawerClass
{
    public class NeuralGraphDrawer : ScriptableObject
    {
        private NeuralGraph neuralGraph;
        private GameObject drawingVertex;
        private GameObject drawingEdge;
        private Vector2 graphPos;
        private Vector2 offset;
        private Transform parentT;

        public void init(NeuralGraph ng,GameObject drV, GameObject drE, Vector2 gp, Transform p)
        {
            neuralGraph = ng;
            drawingVertex = drV;
            drawingEdge = drE;
            graphPos = gp;
            //offset.x = 250 / ng.getMaxDimensins().x;
            offset.y = -18;
            parentT = p;
        }
        
        public void resetNg(NeuralGraph ng)
        {
            neuralGraph = ng;
        }

        private void drawVertex(Vector2 pos)
        {
            GameObject auxVertex = Instantiate(drawingVertex, new Vector3(0, 0, 0), Quaternion.identity, parentT.transform);
            auxVertex.GetComponent<RectTransform>().localPosition = pos;
        }

        /*private void drawVertex(VertexWrapper v)
        {
            GameObject auxVertex = Instantiate(drawingVertex, new Vector3(0, 0, 0), Quaternion.identity, parentT.transform);
            auxVertex.GetComponent<RectTransform>().localPosition = v.getPos();
        }*/

        private void drawEdge(Vector2 pos, Quaternion rot, float width, float bias)
        {
            GameObject auxEdge = Instantiate(drawingEdge, new Vector3(0, 0, 0), Quaternion.identity, parentT.transform);
            auxEdge.GetComponent<RectTransform>().localPosition = pos;
            auxEdge.GetComponent<RectTransform>().localRotation = rot;
            auxEdge.GetComponent<RectTransform>().sizeDelta = new Vector3(width, 0.75f);
            auxEdge.GetComponent<RectTransform>().GetChild(0).GetComponent<RectTransform>().localPosition = new Vector2(width/2, 0);

            Color biasColor;
            if (bias >= 0) { biasColor = new Color(1, bias / 2, bias / 2, 1); }
            else { biasColor = new Color(-bias / 2, -bias / 2, 1, 1); }
            auxEdge.GetComponent<Image>().color = biasColor;
            auxEdge.GetComponent<RectTransform>().GetChild(0).GetComponent<Image>().color = biasColor;
        }

        private void conectVertexes(Vector2 v1, Vector2 v2, float bias)
        {
            Vector2 disVector = v2 - v1;
            float distance = disVector.magnitude;
            float dheight = v2.y - v1.y;
            //float angle = Mathf.Asin((dheight / 2) / (distance / 2));
            float angle = Mathf.Atan2(disVector.y , disVector.x);
            angle = angle * (180 / Mathf.PI);

            drawEdge(v1 + (disVector / 2), Quaternion.Euler(0, 0, angle), distance-10, bias);
        }
        
        public void numberOfColumns(out int columnNumber, out int vertexPerColumn, out int remainers)
        {
            int intermediates = neuralGraph.getVertexes().Count - neuralGraph.getInCount() - neuralGraph.getOutCount();
            columnNumber = (intermediates - 1) / 5 + 1;
            vertexPerColumn = intermediates / columnNumber;
            remainers = intermediates % columnNumber;
        }

        public void drawNeuralNetwork()
        {
            List<Vector2> posList = new List<Vector2>(neuralGraph.getVertexes().Count);
            for(int i=0; i<neuralGraph.getInCount(); i++)
            {
                drawVertex(new Vector2(0,i) * offset + graphPos);
                posList.Add(new Vector2(0, i));
            }

            int columnNumber;
            int vertexPerColumn;
            int remainers;
            numberOfColumns(out columnNumber, out vertexPerColumn,out remainers);
            int vertexTracker = 0;

            for(int i=0; i<columnNumber; i++)
            {
                for(int j=0; j<vertexPerColumn; j++)
                {
                    drawVertex(new Vector2(i, j) * offset + graphPos);
                    posList.Add(new Vector2(i, j));
                }
                if(remainers > 0)
                {
                    drawVertex(new Vector2(i, vertexPerColumn) * offset + graphPos);
                    posList.Add(new Vector2(i, vertexPerColumn));
                    remainers--;
                }
            }

            for(int i=0; i<neuralGraph.getVertexes().Count; i++)
            {

            }


            /*

            for (int i = neuralGraph.getInCount(); i < neuralGraph.getVertexes().Count -  neuralGraph.getOutCount(); i++)
            {
                drawVertex(new Vector2(0, i) * offset + graphPos);
            }


            foreach (Vertex v in neuralGraph.getVertexes())
            {
                drawVertex(v.getInternalPos() * offset + graphPos);
            }

            foreach (Vertex v in neuralGraph.getVertexes())
            {
                //drawVertex(v.getInternalPos() * offset + graphPos);
                foreach(Edge e in v.getForwVertexes())
                {
                    conectVertexes(v.getInternalPos() * offset + graphPos, e.getForward().getInternalPos() * offset + graphPos, e.getWeight());
                }
            }*/
        }


        public void clearDraw()
        {
            foreach(Transform t in parentT)
            {
                Destroy(t.gameObject);
            }
        }
    }
}
