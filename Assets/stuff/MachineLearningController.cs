using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

namespace Pathfinding
{
    using Pathfinding.Util;
    public class MachineLearningController : MonoBehaviour
    {
        [Header("General Settings")]
        [SerializeField] float mutationBonus;
        [SerializeField] float totalTime;
        [SerializeField] float timeAlive;

        [Header("Advanced Mutation")]
        [SerializeField] bool enableAdvancedMutation;

        [Header("Units")]
        [SerializeField] List<GameObject> units = new List<GameObject>();
        [SerializeField] int remaining;

        [Header("Aditional Game Objects")]
        [SerializeField] GameObject cameraControl;
        [SerializeField] GameObject canvas;

        int unitsCount;
        int generation;
        bool finalCheckInProcess;
        distanceCalculatorAStar aStar;

        int cameraTracker;

        public void hasCollided()
        {
            int temporalRemaining = 0;
            foreach (GameObject unit in units)
            {
                PlayerController pControlerAux = unit.GetComponent<PlayerController>();
                if (pControlerAux.isAlive())
                {
                    temporalRemaining++;
                }
                else
                {
                    if (pControlerAux.getDistanceToEnd() == -99.0f)
                    {
                        aStar.queuePath(unit);
                    }
                }
            }
            remaining = temporalRemaining;
            canvas.GetComponent<CanvasText>().setAlive(remaining);

        }

        public void collideCurrent()
        {
            PlayerController unitAux = units[cameraTracker].GetComponent<PlayerController>();

            if(unitAux.isAlive()){
                unitAux.setAlive(false);
                hasCollided();
            }

            resetCamera();
        }

        public void collideAll()
        {
            foreach (GameObject unit in units)
            {
                unit.GetComponent<PlayerController>().setAlive(false);
            }
            hasCollided();
        }

        public GameObject getFirstUnit()
        {
            return units[units.Count - 1];
        }

        public void resetCamera()
        {
            if (remaining > 0)
            {
                if (cameraTracker >= unitsCount-1) { cameraTracker = 0; }
                else { cameraTracker++; }

                //Debug.Log(cameraTracker+"  ||  "+ unitsCount);
                if (units[cameraTracker].GetComponent<PlayerController>().isAlive())
                {
                    cameraControl.GetComponent<CameraController>().setTarget(units[cameraTracker]);
                    canvas.GetComponent<CanvasInfo>().updateInfo(units[cameraTracker],cameraTracker, units[cameraTracker].GetComponent<PlayerController>().name);
                }
                else
                {
                    resetCamera();
                }
            }
            else
            {
                Debug.Log("All potential targets are dead");
            }
        }

        public void firstCamera()
        {
            cameraTracker = 0;
            cameraControl.GetComponent<CameraController>().setTarget(units[cameraTracker]);
            canvas.GetComponent<CanvasInfo>().updateInfo(units[cameraTracker],cameraTracker, units[cameraTracker].GetComponent<PlayerController>().name);
        }

        public void lastCamera()
        {
            cameraTracker = unitsCount - 1;
            cameraControl.GetComponent<CameraController>().setTarget(units[cameraTracker]);
            canvas.GetComponent<CanvasInfo>().updateInfo(units[cameraTracker],cameraTracker, units[cameraTracker].GetComponent<PlayerController>().name);
        }

        void Start()
        {
            finalCheckInProcess = false;

            aStar = GetComponent<distanceCalculatorAStar>();

            remaining = units.Count;
            unitsCount = units.Count;
            for (int i = 0; i < unitsCount; i++)
            {
                for (int j = i + 1; j < unitsCount; j++) {
                    Physics2D.IgnoreCollision(units[i].GetComponent<PolygonCollider2D>(), units[j].GetComponent<PolygonCollider2D>());
                }

                //units[i].GetComponent<PlayerController>().loadUnit();
            }
            if (cameraControl != null)
                cameraControl.GetComponent<CameraController>().setTarget(units[0]);

            cameraTracker = 0;

            generation = 0;
            if (File.Exists(Application.dataPath + "\\ML_Values\\ML_Main.txt"))
            {
                string json = File.ReadAllText(Application.dataPath + "\\ML_Values\\ML_Main.txt");
                string[] auxLoader = JsonConvert.DeserializeObject<string[]>(json);

                generation = int.Parse(auxLoader[0]) + 1;
                enableAdvancedMutation = bool.Parse(auxLoader[5]);
            }
            Debug.Log("Start of Gen: " + generation + "____________________________________________________________________________________");

            timeAlive = totalTime;

            canvas.GetComponent<CanvasText>().setGen(generation);
            canvas.GetComponent<CanvasText>().setAlive(units.Count);
            canvas.GetComponent<CanvasText>().setTime(totalTime);

            for (int i = 0; i < unitsCount; i++)
            {
                canvas.GetComponent<CanvasInfo>().initializeDraw(units[i].GetComponent<PlayerController>().GetNeuralGraph());
            }
            canvas.GetComponent<CanvasInfo>().drawOnPanelAll();

            canvas.GetComponent<CanvasInfo>().updateInfo(units[0],0,  units[cameraTracker].GetComponent<PlayerController>().name);
            //canvas.GetComponent<CanvasText>().resetGen();
        }

        private void sortSaveResetAll()
        {
            Debug.Log("End of Gen: " + generation);

            units.Sort(delegate (GameObject a, GameObject b) {
                float auxA = a.GetComponent<PlayerController>().getDistanceToEnd();
                float auxB = b.GetComponent<PlayerController>().getDistanceToEnd();
                return (auxA.CompareTo(auxB));
            });

            for (int i = 0; i < unitsCount; i++)
            {
                if (i < unitsCount / 2)
                {
                }
                else
                {
                    units[i].GetComponent<PlayerController>().replaceModify(units[i - unitsCount / 2].GetComponent<PlayerController>().getIdTag(), mutationBonus);
                }
                units[i].GetComponent<PlayerController>().saveUnit();
                units[i].GetComponent<PlayerController>().resetUnit();
            }

            remaining = units.Count;

            string[] auxSaver = new string[6]; auxSaver[0] = generation.ToString(); auxSaver[1] = units[0].name; auxSaver[2] = units[unitsCount - 1].name;
            auxSaver[3] = totalTime.ToString(); auxSaver[4] = unitsCount.ToString(); auxSaver[5] = enableAdvancedMutation.ToString();
            string json = JsonConvert.SerializeObject(auxSaver, Formatting.Indented);
            File.WriteAllText(Application.dataPath + "\\ML_Values\\ML_Main.txt", json);

            generation++;

            if (cameraControl != null)
                cameraControl.GetComponent<CameraController>().setTarget(units[0]);


            aStar.resetQueue();

            finalCheckInProcess = false;

            cameraTracker = 0;

            for (int i = 0; i < unitsCount; i++)
            {
                canvas.GetComponent<CanvasInfo>().resetDraw(units[i].GetComponentInChildren<PlayerController>().GetNeuralGraph(), i);
            }
            canvas.GetComponent<CanvasInfo>().drawOnPanelAll();

            canvas.GetComponent<CanvasInfo>().updateInfo(units[0],0,  units[cameraTracker].GetComponent<PlayerController>().name);

            canvas.GetComponent<CanvasText>().setGen(generation);
            canvas.GetComponent<CanvasText>().setTime(totalTime);
            canvas.GetComponent<CanvasText>().setAlive(unitsCount);
            Debug.Log("Start of Gen: " + generation + "____________________________________________________________________________________");

            timeAlive = totalTime;
        }
        
        private IEnumerator lastCheckRec()
        {
            finalCheckInProcess = true;
            yield return new WaitUntil(() => aStar.queueIsDone() == true);
            sortSaveResetAll();
            /*
            if (i < units.Count)
            {
                if (units[i].GetComponent<PlayerController>().getDistanceToEnd() == -99.0f)
                {
                    Debug.Log("added to queue:  " + units[i].name); args  : int i
                    aStar.queuePath(units[i]);
                }
                StartCoroutine(lastCheckRec(i + 1));
            }
            else
            {
                sortSaveResetAll();
            }*/
        }

        private void Update()
        {
            timeAlive -= Time.deltaTime;

            if(timeAlive > 0)
            canvas.GetComponent<CanvasText>().setTime(timeAlive);

            if (timeAlive <= 0.0f)
            {
                collideAll();
            }
            else
            {
                canvas.GetComponent<CanvasInfo>().updateInptOutpt(units[cameraTracker].GetComponent<PlayerController>().getDistances(), units[cameraTracker].GetComponent<PlayerController>().getController());
            }

            if (remaining <= 0)
            {
                if(!finalCheckInProcess)
                StartCoroutine(lastCheckRec());
            }

        }

    }
}
