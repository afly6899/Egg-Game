using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;


public class DialogueManager : MonoBehaviour {

    // Input Parameters:
    // nameText (Text) -> Text UI that displays the name of the speaker
    // choiceSize (Text) -> Text UI of x number of \n characters, where x = number of choices
    // dialogueText (Text) -> Text UI that displays the dialogue of the speaker
    // choiceDisplay (Canvas) -> Canvas UI that holds the Text UI for the choices (is parent to choiceSize)
    // originalFile (TextAsset) -> Text File that contains all the scene information
    public Text nameText;
    //public Text choiceSize;
    public Text dialogueText;
    public Canvas choiceDisplay;
	public TextAsset originalFile;
	
    Dictionary<string, Scene> scenesByName;


    // To initialize all the stuffs
	void Start()
    {
        scenesByName = new Dictionary<string, Scene>();
        string[] scenes = Regex.Split(originalFile.text, "\\[Scene ");
        Debug.Log(scenes.Length.ToString());
        foreach (string scene in scenes)
        {
            if (scene.Length > 0)
            {
                Scene newScene = new Scene(scene.TrimEnd().Split('\n'), ref choiceDisplay);
                scenesByName[newScene.sceneName] = newScene;
            }
        }
    }

    // Will update the system every cycle!
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {

        }
    }
}

//  hi
//      

// Scene (class) -> Class to hold the scene dialogue, choices, and next Scenes
//      - takes in the text for that scene as an array of lines formatted as either __] (used to be [Scene __]) or
//          Name:Dialogue  or  ChoiceTrigger:Name:Question  or  [Goto Scene __]:Answer
//      - __] (used to be [Scene __]) -> save the name of the scene (the __ part)
//      - Name:Dialogue -> simply parse the line into [Name, Dialogue]
//      - ChoiceTrigger:Name:Dialogue -> parse the line, but only save the [Name, Dialogue] part
//      - [Goto Scene __]:Answer -> parse the line, but then do...
//          ~ call AddTextToMenu and pass in Answer
//          ~ add the scene name to list of goto scene names
//          ~ at end of everything, return the list of scene names

// STILL NEED TO DO:
//  1) create an Update function that can read in Input, specifically up, down arrows for choices and the enter key to 
//      continue the dialogue or select a choice
//  2) have the Scene know when it's making a choice and make sure it deals with the two kinds of returns differently
//  3) find a way to display the choices and change the formatting of the choice based on what's selected? <- or leave this to the Dialogue Manager?
//      a) maybe if leaving this to Dialogue Manager, have a function that returns a reference to the choiceOptions list?

class Scene
{
    int numChoices;
    bool hasChoices;
    public string sceneName; // may remove public and have a getName function instead... we'll have to see
    string[] choiceText;
    List<string[]> dialogueText; // holding a list of [Name, Dialogue]
    List<string> choices; // TEMP!
    List<GameObject> choiceOptions;
    List<string> goToSceneNames;

    Text choiceSize;
    Canvas choiceDisplay;
    private string[] v;

    // Scene(): constructor for Scene that takes in the array of text for a scene, does all the parsing!
    public Scene (string[] sceneText, ref Canvas optionDisplay)
    {
        // Initializing all variables
        numChoices = 0;
        hasChoices = false;
        dialogueText = new List<string[]>();
        choices = new List<string>();
        choiceOptions = new List<GameObject>();
        goToSceneNames = new List<string>();
        


        sceneName = sceneText[0].Substring(0, 2); // "[Scene " goes up to index 6, so name should start at 7 -> now has reformatted to just be __], so start at 0
        Debug.Log(sceneName);

        string[] parsedLine;
        for (int i=1; i<sceneText.Length; i++)
        {
            parsedLine = sceneText[i].Split(':');

            // first: has [Goto Scene A1] condition
            if (parsedLine[0][0] == '[') // could either be a choice option or just a goto statement
            {
                //Debug.Log("Could be choice or goto statement: " + parsedLine.Length);
                // parsedLine[0] = [Goto Scene __] -> Split by ' ' = [ [Goto, Scene, __] ] -> [2] = __] -> Substring(0,2) = __
                // save the scene name in the goToSceneNames list
                goToSceneNames.Add(parsedLine[0].Split(' ')[2].Substring(0,2));
                // if there's a choice afterwards, save it as well
                if (parsedLine.Length == 2) // is a choice option cause it has Answer in it
                {
                    // parsedLine = [[Goto Scene __], Answer]
                    // TEMP {
                    choices.Add(parsedLine[1]);
                    // }
                    AddTextToMenu(parsedLine[1], ref optionDisplay);
                }
            }
            else if (parsedLine.Length == 3) // will be 3 if ChoiceTrigger!
            {
                string[] temp = { parsedLine[1], parsedLine[2] };
                dialogueText.Add(temp);
                // parsedLine = [ ChoiceTrigger, Name, Question ] -> only want Name and Question
                hasChoices = true;
            }
            else
            {
                dialogueText.Add(parsedLine);
            }
        }

    }

    // AddTextToMenu: creates a Text object for a choice by creating a new GameObject with a Text component
    void AddTextToMenu(string choice, ref Canvas optionDisplay)
    {
        numChoices++;
        GameObject newOption = new GameObject();
        newOption.name = "Choice " + numChoices.ToString();
        newOption.transform.SetParent(optionDisplay.transform);
        newOption.transform.localScale = new Vector3(1, 1, 1);
        newOption.AddComponent<Text>().text = choice;
        newOption.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
        newOption.GetComponent<Text>().color = Color.black;
        newOption.GetComponent<Text>().font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        newOption.GetComponent<Text>().fontSize = 24;
        choiceOptions.Add(newOption);
    }
}