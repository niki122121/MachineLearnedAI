using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using neuralGraphDrawerClass;
using nueralGraphClass;

public class CanvasInfo : MonoBehaviour
{
    [SerializeField] Button openButton;
    [SerializeField] Button infoButton;

    [SerializeField] Image currentUnitImage;
    [SerializeField] Text spriteInnerText;

    [SerializeField] Text inputText;
    [SerializeField] Text outputText;

    [SerializeField] GameObject drawingVertex;
    [SerializeField] GameObject drawingEdge;
    [SerializeField] Vector2 graphPos;
    [SerializeField] List<NeuralGraphDrawer> drawers;

    [SerializeField] GameObject panelContainer;
    [SerializeField] GameObject panel;

    bool isOpen = false;


    int currentUnit;

    public void initializeDraw(NeuralGraph unit)
    {
        GameObject panelAux = Instantiate(panel, Vector3.zero, Quaternion.identity, panelContainer.transform);
        panelAux.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        panelAux.SetActive(false);

        //NeuralGraphDrawer drawerAux = new NeuralGraphDrawer(unit, drawingVertex, drawingEdge, graphPos, offset, panelAux.transform);
        NeuralGraphDrawer drawerAux = ScriptableObject.CreateInstance<NeuralGraphDrawer>();
        drawerAux.init(unit, drawingVertex, drawingEdge, graphPos, panelAux.transform);
        drawers.Add(drawerAux);
    }

    public void resetDraw(NeuralGraph unit, int n)
    {
        drawers[n].clearDraw();
        drawers[n].resetNg(unit);
    }

    public void drawOnPanelAll()
    {
        for(int i=0; i<drawers.Count; i++)
        {
            drawers[i].drawNeuralNetwork();
        }
    }

    public void updateInfo(GameObject unit,int n, string imageName)
    {
        currentUnitImage.GetComponent<Image>().sprite = unit.GetComponent<SpriteRenderer>().sprite;
        currentUnitImage.GetComponent<Image>().color = unit.GetComponent<SpriteRenderer>().color;
        spriteInnerText.text = imageName;

        panelContainer.transform.GetChild(currentUnit).gameObject.SetActive(false);
        panelContainer.transform.GetChild(n).gameObject.SetActive(true);
        currentUnit = n;
    }

    public void testUpdate() { panelContainer.transform.GetChild(0).gameObject.SetActive(true); }

    public void openInfo()
    {
        openButton.gameObject.SetActive(false);
        infoButton.gameObject.SetActive(true);
        isOpen = true;
    }

    public void closeInfo()
    {
        openButton.gameObject.SetActive(true);
        infoButton.gameObject.SetActive(false);
        isOpen = false;
    }

    public void updateInptOutpt(List<float> dist, float output)
    {
        if (isOpen)
        {
            string inputTxt = "";
            for(int i=0; i<dist.Count; i++)
            {
                inputTxt += "input " + i + ": " + dist[i] + "\n";
            }
            inputText.text = inputTxt;
            outputText.text = "output: " + output;
        }
    }

    

    private void Start()
    {
        drawers = new List<NeuralGraphDrawer>();
        currentUnit = 0;
        //closeInfo();
    }
}
