using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

using UnityEngine.SceneManagement;

using nueralGraphClass;
using vertexClass;
using edgeClass;

using Unity.Jobs;

namespace Pathfinding
{
    using Pathfinding.Util;
    public class PlayerController : MonoBehaviour
    {
        [Header("ID")]
        [SerializeField] int idTag;
        //float maxTime;
        //[SerializeField] float timeAlive;
        [SerializeField] float distanceToEnd;

        [Header("AI")]
        [SerializeField] bool Ai_controlled;
        [SerializeField] float detLeng;
        [SerializeField] bool drawGizmos;

        [Header("Unit Settings")]
        [SerializeField] float speed;
        [SerializeField] float rotSpeed;
        Vector2 startPos;

        float controller;
        bool alive;

        Vector3 dirr;
        List<float> distances;
        List<Vector3> detector;

        MachineLearningController mlc;
        NeuralGraph neuralNetwork;

        public void iniUnit()
        {
            //initialization
            neuralNetwork = new NeuralGraph();

            for (int i = 0; i < 5; i++)
            {
                neuralNetwork.addVertex(new Vertex("input " + i), true, false);
            }

            for (int i = 0; i < 4; i++)
            {
                neuralNetwork.addVertex(new Vertex("intermediate_1 " + i));
            }

            for (int i = 0; i < 4; i++)
            {
                neuralNetwork.addVertex(new Vertex("intermediate_2 " + i));
            }

            for (int i = 0; i < 4; i++)
            {
                neuralNetwork.addVertex(new Vertex("intermediate_3 " + i));
            }

            neuralNetwork.addVertex(new Vertex("output"), false, true);

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

        public void saveUnit()
        {
            int size = neuralNetwork.getVertexes().Count;
            int inputSize = neuralNetwork.getInCount();
            int outputSize = neuralNetwork.getOutCount();
            float[,] aux = new float[size + 1, size];

            aux[0, 0] = size;
            aux[0, 1] = inputSize;
            aux[0, 2] = size - (inputSize + outputSize);
            aux[0, 3] = outputSize;
            for (int m = 0; m < size; m++)
            {
                for (int n = m + 1; n < size; n++)
                {
                    aux[m + 1, n] = neuralNetwork.getWeightBetween(neuralNetwork.getVertexes()[m], neuralNetwork.getVertexes()[n]);
                }
            }
            string json = JsonConvert.SerializeObject(aux, Formatting.Indented);
            //File.WriteAllText(@"c:\Users\Niki Kalamov\Desktop\ML_Values\ML" + idTag + ".txt", json);
            File.WriteAllText(Application.dataPath + "\\ML_Values\\ML" + idTag + ".txt", json);
        }

        public void loadUnit(int id)
        {
            if (File.Exists(Application.dataPath + "\\ML_Values\\ML" + id + ".txt"))
            {
                string json = File.ReadAllText(Application.dataPath + "\\ML_Values\\ML" + id + ".txt");
                float[,] aux = JsonConvert.DeserializeObject<float[,]>(json);

                neuralNetwork = new NeuralGraph();
                for (int n = 0; n < (int)aux[0, 1]; n++)
                {
                    neuralNetwork.addVertex(new Vertex("input " + n), true, false);
                }
                for (int n = 0; n < (int)aux[0, 2]; n++)
                {
                    neuralNetwork.addVertex(new Vertex("intermediate " + n));
                }
                for (int n = 0; n < (int)aux[0, 3]; n++)
                {
                    neuralNetwork.addVertex(new Vertex("output " + n), false, true);
                }
                for (int m = 0; m < (int)aux[0, 0]; m++)
                {
                    for (int n = m + 1; n < (int)aux[0, 0]; n++)
                    {
                        if (Mathf.Abs(aux[m + 1, n] - 1234.567f) < 0.01f)
                        {
                            //do nothing there is no link
                        }
                        else
                        {
                            neuralNetwork.link(neuralNetwork.getVertexes()[m], neuralNetwork.getVertexes()[n], aux[m + 1, n]);
                        }
                    }
                }
            }
            else
            {
                iniUnit();
                neuralNetwork.structureInit(0.75f);
            }
        }

        void initStart()
        {
            alive = true;
            distanceToEnd = -99.0f;

            startPos = transform.position;
            transform.rotation = Quaternion.Euler(0, 0, 0.0f);
            dirr = new Vector3(0, 1.0f, 0);

            detector = new List<Vector3>();
            distances = new List<float>();

            neuralNetwork = new NeuralGraph();
            //transform.position += dirr * Time.deltaTime;

            detector.Add(new Vector3(-1.0f, 0, 0)); detector.Add(new Vector3(-0.71f, 0.71f, 0)); detector.Add(new Vector3(0, 1.0f, 0)); detector.Add(new Vector3(0.71f, 0.71f, 0)); detector.Add(new Vector3(1.0f, 0, 0));

            for (int i = 0; i < detector.Count; i++)
            {
                detector[i] = detector[i] * detLeng;
                distances.Add(detLeng);
            }

            controller = 0.0f;

            loadUnit(idTag);
            //ml.structureModify();
            //ml.saveData(idTag);
        }

        void Start()
        {
            mlc = GameObject.Find("MACHINE LEARNING").GetComponent<MachineLearningController>();
            initStart();
        }

        void FixedUpdate()
        {
            if (alive)
            {
                /*
                timeAlive += Time.deltaTime;
                if (timeAlive > mlc.totalTime)
                {
                    alive = false;
                    mlc.hasCollided();
                }*/

                transform.position += (dirr * speed) * Time.deltaTime;


                dirr = Quaternion.Euler(0, 0, -controller * rotSpeed) * dirr;
                transform.Rotate(0, 0, -controller * rotSpeed);

                RaycastHit2D hit;
                for (int i = 0; i < detector.Count; i++)
                {

                    detector[i] = Quaternion.Euler(0, 0, -controller * rotSpeed) * detector[i];
                    if(drawGizmos)
                    Debug.DrawRay(transform.position, detector[i], Color.red);

                    hit = Physics2D.Raycast(transform.position, detector[i], detLeng, 1 << 8);
                    if (hit.collider != null)
                    {
                        //Debug.Log("hit");
                        distances[i] = hit.distance;
                    }
                    else
                    {
                        //Debug.Log("nothing");
                        distances[i] = detLeng;
                    }

                }

                if (Ai_controlled)
                {
                    neuralNetwork.input(distances, detLeng);
                    controller = Mathf.Clamp(neuralNetwork.output(),-1.0f,1.0f); //could cause problems (remove if needed)
                    //Debug.Log(controller);
                }
                else
                {
                    controller = Input.GetAxis("Horizontal");
                }
            }
        }


        public int getIdTag()
        {
            return idTag;
        }

        public NeuralGraph GetNeuralGraph()
        {
            return neuralNetwork;
        }

        public float getDetLeng()
        {
            return detLeng;
        }

        public float getController()
        {
            return controller;
        }

        public bool isAlive()
        {
            return alive;
        }

        public void setAlive(bool al)
        {
            alive = al;
        }

        /*
        public float getMaxTime()
        {
            return maxTime;
        }
        */
        public Vector2 getPos()
        {
            return transform.position; // + (timeAlive / 3.0f);
        }

        public List<float> getDistances()
        {
            return distances;
        }

        public float getDistanceToEnd()
        {
            return distanceToEnd;
        }

        public void setDistanceToEnd(float d)
        {
            distanceToEnd = d;
        }

        public void resetUnit()
        {
            transform.position = startPos;
            initStart();
        }

        public void replaceModify(int replacerTag, float amount)
        {
            loadUnit(replacerTag);
            neuralNetwork.structureModify(amount);
            saveUnit();
        }

        public void justModify(float amount)
        {
            neuralNetwork.structureModify(amount);
            saveUnit();
        }

        public void printUnit()
        {
            neuralNetwork.printGraph();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("obst"))
            {
                alive = false;
                mlc.hasCollided();
                //Debug.Log(points.Count);
            }
            else if (other.CompareTag("end"))
            {
                alive = false;
                mlc.hasCollided();
            }
            //printUnit();
        }
    }
}