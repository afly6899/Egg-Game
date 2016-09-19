using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class choiceManager : MonoBehaviour {

    public TextAsset choiceFile;
    
    public Canvas optionDisplay;

    int numChoices;

	// Use this for initialization
	void Start () {
        numChoices = 0;
        foreach(string choice in choiceFile.text.Split('\n'))
        {
            Debug.Log(choice);
            AddTextToMenu(choice);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    void AddTextToMenu(string choice)
    {
        numChoices++;
        GameObject newOption = new GameObject();
        newOption.name = "Choice " + numChoices.ToString();
        newOption.transform.SetParent(optionDisplay.transform);
        newOption.AddComponent<Text>().text = choice;
        newOption.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        newOption.GetComponent<Text>().color = Color.black;
        newOption.GetComponent<Text>().font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        newOption.GetComponent<Text>().fontSize = 14;
    }


}
