using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    [Header("ID")]
    [SerializeField] int idTag;
    //float timeAlive;

    [Header("AI")]
    [SerializeField] bool Ai_controlled;
    [SerializeField] float detLeng;

    [Header("Unit Settings")]
    [SerializeField] float speed;
    [SerializeField] float rotSpeed;
    [SerializeField] Vector2 startPos;

    float controller;
    bool alive;

    Vector3 dirr;
    float[] distances = new float[5];
    Vector3[] detector = new Vector3[5];

    private class Links
    {
        public float[] link = new float[5];
    }

    private class MachineLearning
    {
        private Links links;

        public MachineLearning()
        {
            links = new Links();
        }

        public void structureInit()
        {
            for (int i = 0; i < 5; i++)
            {
                links.link[i] = Random.Range(-1.0f, 1.0f);
            }
        }

        public void structureModify(float amount)
        {

            for (int i = 0; i < 5; i++)
            {
                float primaryParams = Random.Range(0.0f, amount);
                float secondaryParams = Random.Range(-primaryParams, primaryParams);
                links.link[i] = links.link[i] + secondaryParams;

                /*links.R_link[i] = Mathf.Clamp(links.R_link[i] + R_secondaryParams, 0.0f, 1.0f);
                links.L_link[i] = Mathf.Clamp(links.L_link[i] + L_secondaryParams, 0.0f, 1.0f);*/
            }


        }

        public void printStructure()
        {
            Debug.Log(links.link[0] + "   " + links.link[1] + "   " + links.link[2] + "   " + links.link[3] + "   " + links.link[4]);
        }

        public void loadData(int id)
        {
            if (File.Exists(@"c:\Users\Niki Kalamov\Desktop\ML_Values\ML" + id + ".txt"))
            {
                string json = File.ReadAllText(@"c:\Users\Niki Kalamov\Desktop\ML_Values\ML" + id + ".txt");
                links = JsonUtility.FromJson<Links>(json);
            }
            else
            {
                structureInit();
            }
        }

        public void saveData(int id)
        {
            string json = JsonUtility.ToJson(links);
            File.WriteAllText(@"c:\Users\Niki Kalamov\Desktop\ML_Values\ML" + id + ".txt", json);
        }

        public float finalOutput(float[] inpt, float leng)
        {
            float[] input = new float[5];
            for (int i = 0; i < 5; i++)
            {
                input[i] = (inpt[i] / leng); //normalizar para 0 -> 1
            }

            float finalResult = (input[0] * links.link[0] + input[1] * links.link[1] + input[2] * links.link[2] + input[3] * links.link[3] + input[4] * links.link[4]); //normalizarlo a 0->1

            //Debug.Log(inter1 + "  " + inter2 + "  " + inter3 + "  " + inter4 + "  " + inter5 + "  ");

            return finalResult;

        }

        /*public void saveData()
        {
            string json = JsonUtility.ToJson(this);
            File.WriteAllText(@"c:\Users\Niki Kalamov\Desktop\SaveGame.txt", json);
        }*/
    }

    MachineLearning ml;

    void Start()
    {

        alive = true;
        //timeAlive = 0.0f;

        startPos = transform.position;
        transform.rotation = Quaternion.Euler(0, 0, -90.0f);
        dirr = new Vector3(1.0f, 0, 0);

        ml = new MachineLearning();
        //transform.position += dirr * Time.deltaTime;

        detector[0] = new Vector3(0, -1.0f, 0);
        detector[1] = new Vector3(0.6f, -0.8f, 0);
        detector[2] = new Vector3(1.0f, 0, 0);
        detector[3] = new Vector3(0.6f, 0.8f, 0);
        detector[4] = new Vector3(0, 1.0f, 0);

        for (int i = 0; i < 5; i++)
        {
            detector[i] = detector[i] * detLeng;
            distances[i] = detLeng;
        }

        controller = 0.0f;

        ml.loadData(idTag);
        //ml.structureModify();
        //ml.saveData(idTag);

    }

    void Update()
    {
        if (alive)
        {

            //timeAlive += Time.deltaTime;

            transform.position += (dirr * speed) * Time.deltaTime;


            dirr = Quaternion.Euler(0, 0, -controller * rotSpeed) * dirr;
            transform.Rotate(0, 0, -controller * rotSpeed);

            RaycastHit2D hit;
            for (int i = 0; i < 5; i++)
            {

                detector[i] = Quaternion.Euler(0, 0, -controller * rotSpeed) * detector[i];
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
                controller = ml.finalOutput(distances, detLeng);
                //Debug.Log(controller);
            }
            else
            {
                controller = Input.GetAxis("Horizontal");
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("obst"))
        {
            alive = false;
            SceneManager.LoadScene(1);
            //Debug.Log(points.Count);
        }
        else if (other.CompareTag("end"))
        {
            alive = false;
            SceneManager.LoadScene(1);
        }

    }

}

