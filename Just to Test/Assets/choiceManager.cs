using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class choiceManager : MonoBehaviour {

    public TextAsset choiceFile;
    
    public Canvas optionDisplay;

    int numChoices;
    string[] choices;
    List<GameObject> choiceOptions;

	// Use this for initialization
	void Start () {
        numChoices = 0;
        choiceOptions = new List<GameObject>();
        choices = choiceFile.text.Split('\n');

        // testing for how system responds to having more GameObjects than needed
        for (int i=0; i<1; i++) { AddTextToMenu(i.ToString()); }

        DisplayChoices();

        //foreach(string choice in choiceFile.text.Split('\n'))
        //{
        //    Debug.Log(choice);
        //    AddTextToMenu(choice);
        //}
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
        choiceOptions.Add(newOption);
    }

    void DisplayChoices()
    {
        // want a Text GameObject for each choice
        // for GameObjects that are already there, want to reuse
        // for new GameObjects that are needed, want to create
        int targetCount = choices.Length;
        int i;

        // if targetCount is the min, then only up to targetCount number of GameObjects will be set -> need to dectivate the others
        // if choiceOptions.Count is the min, then only choiceOption.Count number of choices will be prepared -> need to make more GameObjects
        // if they're equal, then we're good!
        Debug.Log("targetCount = " + targetCount + ", numChoiceOptions = " + choiceOptions.Count);
        for (i = 0; i < Mathf.Min(targetCount, choiceOptions.Count); i++)
        {
            Debug.Log("Game Object #" + i + 1 + " is changing text from \'" + choiceOptions[i].GetComponent<Text>().text + "\' to \'" + choices[i] + "\'");
            choiceOptions[i].GetComponent<Text>().text = choices[i];
        }

        for (; i < targetCount; i++)
        {
            AddTextToMenu(choices[i]);
        }

        for (; i < choiceOptions.Count; i++)
        {
            choiceOptions[i].SetActive(false);
        }
    }
}
