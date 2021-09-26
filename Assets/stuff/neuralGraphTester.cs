using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using Newtonsoft.Json;

using nueralGraphClass;
using vertexClass;
using edgeClass;

public class neuralGraphTester : MonoBehaviour
{
    NeuralGraph neuralNetwork;
    [SerializeField] int minColumns;
    [SerializeField] int maxColumns;
    /*
    public void iniUnit()
    {
        //initialization
        neuralNetwork = new NeuralGraph(new Vector2Int(5,5));

        for (int i = 0; i < 5; i++)
        {
            neuralNetwork.addVertex(new Vertex("input " + i, new Vector2Int(0, i)), true, false);
        }

        for (int i = 0; i < 4; i++)
        {
            neuralNetwork.addVertex(new Vertex("intermediate_1 " + i, new Vector2Int(1, i)));
        }

        for (int i = 0; i < 4; i++)
        {
            neuralNetwork.addVertex(new Vertex("intermediate_2 " + i, new Vector2Int(2, i)));
        }

        for (int i = 0; i < 4; i++)
        {
            neuralNetwork.addVertex(new Vertex("intermediate_3 " + i, new Vector2Int(3, i)));
        }

        neuralNetwork.addVertex(new Vertex("output", new Vector2Int(4, 0)), false, true);

        //linking (normal link based on position in list)
        List<Vertex> nVertexes = neuralNetwork.getVertexes();

        for (int i = 0; i < 5; i++)
        {
            for (int j = 5; j < 9; j++)
            {
                neuralNetwork.link(nVertexes[i], nVertexes[j]);
            }
        }

        for (int i = 5; i < 9; i++)
        {
            for (int j = 9; j < 13; j++)
            {
                neuralNetwork.link(nVertexes[i], nVertexes[j]);
            }
        }

        for (int i = 9; i < 13; i++)
        {
            for (int j = 13; j < 17; j++)
            {
                neuralNetwork.link(nVertexes[i], nVertexes[j]);
            }
        }

        for (int i = 13; i < 17; i++)
        {
            neuralNetwork.link(nVertexes[i], neuralNetwork.getOutVertex());
        }
    }

    public void iniUnitAM()
    {
        //initialization
        neuralNetwork = new NeuralGraph();
        int inputNumber = 5;
        int outputNumber = 1;
        int maxRowNumber = Mathf.Max(inputNumber, outputNumber);
        int columnNumber = Random.Range(minColumns, maxColumns+1);
        neuralNetwork.setMaxDimensinsX(columnNumber + 2);

        for (int i = 0; i < inputNumber; i++)
        {
            neuralNetwork.addVertex(new Vertex("input " + i, new Vector2Int(0, i)), true, false);
        }
        for (int i = 0; i < columnNumber; i++)
        {
            int intermediatesPerColumn = Random.Range(2, 6);
            maxRowNumber = Mathf.Max(maxRowNumber, intermediatesPerColumn);
            for (int j = 0; j < intermediatesPerColumn; j++)
            {
                Vertex auxVV = new Vertex("intermediate_" + i + j, new Vector2Int(i+1, j));
                neuralNetwork.addVertex(auxVV);
            }
        }
        for (int i = 0; i < outputNumber; i++)
        {
            neuralNetwork.addVertex(new Vertex("output", new Vector2Int(1+columnNumber,i)), false, true);
        }
        neuralNetwork.setMaxDimensinsY(maxRowNumber);

        //linking
        float checkTracker = 999.3f;
        for(int i=0; i< neuralNetwork.getVertexes().Count; i++)
        {
            checkTracker = neuralNetwork.vertexRandomLink(neuralNetwork.getVertexes()[i], 0.0f, 8, checkTracker);
        }
    }

    public void iniUnitTest()
    {
        neuralNetwork = new NeuralGraph(new Vector2Int(4,4));
        for (int i = 0; i < 4; i++) { neuralNetwork.addVertex(new Vertex("input " + i, new Vector2Int(0,i)), true, false); }
        for (int i = 0; i < 3; i++) { neuralNetwork.addVertex(new Vertex("intermediate_1 " + i, new Vector2Int(1, i)));  }
        for (int i = 0; i < 3; i++) { neuralNetwork.addVertex(new Vertex("intermediate_2 " + i, new Vector2Int(2, i))); }
        for (int i = 0; i < 2; i++) { neuralNetwork.addVertex(new Vertex("output" + i, new Vector2Int(3, i)), false, true); }

        List<Vertex> nVertexes = neuralNetwork.getVertexes();
        neuralNetwork.link(nVertexes[0], nVertexes[4]); neuralNetwork.link(nVertexes[0], nVertexes[5]);
        neuralNetwork.link(nVertexes[1], nVertexes[6]);
        neuralNetwork.link(nVertexes[2], nVertexes[4]); neuralNetwork.link(nVertexes[2], nVertexes[6]);
        neuralNetwork.link(nVertexes[3], nVertexes[5]);

        neuralNetwork.link(nVertexes[4], nVertexes[7]);
        neuralNetwork.link(nVertexes[5], nVertexes[8]); neuralNetwork.link(nVertexes[5], nVertexes[9]);
        neuralNetwork.link(nVertexes[6], nVertexes[8]);

        neuralNetwork.link(nVertexes[7], nVertexes[10]); neuralNetwork.link(nVertexes[7], nVertexes[11]);
        neuralNetwork.link(nVertexes[8], nVertexes[11]);
        neuralNetwork.link(nVertexes[9], nVertexes[10]);
    }

    public void saveUnit()
    {
        int size = neuralNetwork.getVertexes().Count;
        int inputSize = neuralNetwork.getInCount();
        int outputSize = neuralNetwork.getOutCount();
        float[,] aux = new float[size + 3, size];

        aux[0, 0] = size;
        aux[0, 1] = inputSize;
        aux[0, 2] = size - (inputSize + outputSize);
        aux[0, 3] = outputSize;
        aux[0, 4] = neuralNetwork.getMaxDimensins().x;
        aux[0, 5] = neuralNetwork.getMaxDimensins().y;
        for (int n = 0; n < size; n++)
        {
            aux[1, n] = neuralNetwork.getVertexes()[n].getInternalPos().x;
            aux[2, n] = neuralNetwork.getVertexes()[n].getInternalPos().y;
        }
        for (int m = 0; m < size; m++)
        {
            for (int n = 0; n < size; n++)
            {
                aux[m + 3, n] = neuralNetwork.getWeightBetween(neuralNetwork.getVertexes()[m], neuralNetwork.getVertexes()[n]);
            }
        }
        string json = JsonConvert.SerializeObject(aux, Formatting.Indented);
        //File.WriteAllText(@"c:\Users\Niki Kalamov\Desktop\ML_Values\ML" + idTag + ".txt", json);
        File.WriteAllText(@"c:\Users\spabi\Desktop\ML_Values\ML" + 9999 + ".txt", json);
    }

    public void loadUnit()
    {
        if (File.Exists(@"c:\Users\spabi\Desktop\ML_Values\ML" + 9999 + ".txt"))
        {
            string json = File.ReadAllText(@"c:\Users\spabi\Desktop\ML_Values\ML" + 9999 + ".txt");
            float[,] aux = JsonConvert.DeserializeObject<float[,]>(json);

            neuralNetwork = new NeuralGraph(new Vector2Int((int)aux[0,4], (int)aux[0,5]));
            for (int n = 0; n < (int)aux[0, 1]; n++)
            {
                neuralNetwork.addVertex(new Vertex("input " + n, new Vector2Int((int)aux[1, n], (int)aux[2,n])), true, false);
            }
            for (int n = 0; n < (int)aux[0, 2]; n++)
            {
                neuralNetwork.addVertex(new Vertex("intermediate " + n, new Vector2Int((int)aux[1, n+ (int)aux[0, 1]], (int)aux[2, n+ (int)aux[0, 1]])));
            }
            for (int n = 0; n < (int)aux[0, 3]; n++)
            {
                neuralNetwork.addVertex(new Vertex("output " + n, new Vector2Int((int)aux[1, n+(int)(aux[0,1] + aux[0,2])], (int)aux[2, n + (int)(aux[0, 1] + aux[0, 2])])), false, true);
            }
            for (int m = 0; m < (int)aux[0, 0]; m++)
            {
                for (int n = 0; n < (int)aux[0, 0]; n++)
                {
                    if (Mathf.Abs(aux[m + 3, n] - 1234.567f) < 0.01f)
                    {
                        //do nothing there is no link
                    }
                    else
                    {
                        neuralNetwork.link(neuralNetwork.getVertexes()[m], neuralNetwork.getVertexes()[n], aux[m + 3, n]);
                    }
                }
            }
        }
        else
        {
            iniUnitAM();
            neuralNetwork.structureInit(0.75f);
        }
    }



    void Start()
    {
        iniUnitTest();
        //neuralNetwork.structureInit(0.75f);  
        //neuralNetwork.mutate(0.2f, 0.9f, 7);
        //saveUnit();
        //loadUnit();

        //neuralNetwork.addRandomEdge(0.75f);
        //neuralNetwork.mutate(0.2f, 0.1f, 7); //good for now
        //neuralNetwork.deMutate(0.99f, 0.2f);

        GameObject.Find("Canvas").GetComponent<CanvasInfo>().initializeDraw(neuralNetwork);
        GameObject.Find("Canvas").GetComponent<CanvasInfo>().drawOnPanelAll();
        GameObject.Find("Canvas").GetComponent<CanvasInfo>().testUpdate();
        //neuralNetwork.printGraph();
    }*/
}
