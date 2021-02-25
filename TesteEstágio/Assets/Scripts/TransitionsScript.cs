using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TransitionsScript : MonoBehaviour
{
    public string[] transitions;
    public char[] transitionMarkers;
    public GameObject[] items;
    public bool printTransition;
    public TextMeshProUGUI transitionText;
    public int numMarkers;

    // Start is called before the first frame update
    void Start()
    {
        transitions = new string[10];
        transitionMarkers = new char[10];
        numMarkers = 0;
        for (int i=0; i<10; i++) {
            transitions[i] = "";
            transitionMarkers[i] = '#';
        }
        items = new GameObject[10];
    }

    // Update is called once per frame
    void Update()
    {
        if (printTransition) {
            int i = 0;
            while (transitions[i] != "") {
                i++;
            }
            char transitionMarker = '#';
            for (int j=0; j<10; j++) {
                if(items[j]) {
                    if (items[j].tag == "Knives") {transitions[i] += "Faca";}
                    else if (items[j].tag == "Hammers") {transitions[i] += "Martelo";}
                    else if (items[j].tag == "Axes") {transitions[i] += "Machado";}
                    else if (items[j].tag == "Ropes") {transitions[i] += "Corda";}
                    else if (items[j].tag == "Wood") {transitions[i] += "Madeira";}
                    if (items[j+1]) {transitions[i] += " + ";}
                }
            }
            for (int j=0;j<10;j++) {
                if (i!=j) {
                    if (transitions[j] != "") {
                        transitionMarker = transitionMarkers[j];
                        transitions[i] += " = " + transitionMarker;
                        if (transitions[i] == transitions[j]) {
                            transitionMarkers[i] = transitionMarker;
                            break;
                        } else {
                            int ind = transitions[i].IndexOf("=");
                            transitions[i].Remove(ind-1, 4);
                            transitionMarker = '#';
                        }
                    }
                }
            }
            if (transitionMarker == '#') {
                if (numMarkers == 0) {transitionMarker = 'A';}
                else if (numMarkers == 1) {transitionMarker = 'B';}
                else if (numMarkers == 2) {transitionMarker = 'C';}
                else if (numMarkers == 3) {transitionMarker = 'D';}
                else if (numMarkers == 4) {transitionMarker = 'E';}
                else if (numMarkers == 5) {transitionMarker = 'F';}
                else if (numMarkers == 6) {transitionMarker = 'G';}
                else if (numMarkers == 7) {transitionMarker = 'H';}
                else if (numMarkers == 8) {transitionMarker = 'I';}
                else if (numMarkers == 9) {transitionMarker = 'J';}
                numMarkers++;
                transitions[i] += " = " + transitionMarker;
                transitionMarkers[i] = transitionMarker;
            }
            printTransition = false;
            items = new GameObject[10];
            string str = "";
            for (int j=0; j<10; j++) {
                str += transitions[j];
                str += "\n";
            }
            transitionText.text = str;
        }
    }
}

