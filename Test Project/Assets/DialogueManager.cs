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
    public Text choiceSize;
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
                Scene newScene = new Scene(scene.TrimEnd().Split('\n'));
                scenesByName[newScene.sceneName] = newScene;
            }
        }
    }

    // Will update the system every cycle!
    void Update()
    {

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
    List<string[]> dialogueText;
    List<GameObject> choiceOptions;

    Text choiceSize;
    Canvas choiceDisplay;
    private string[] v;

    // Scene(): constructor for Scene that takes in the array of text for a scene, does all the parsing!
    public Scene (string[] sceneText)
    {
        for (int i=0; i<sceneText.Length; i++) { Debug.Log(sceneText[i]); }

        sceneName = sceneText[0].Substring(0, 2); // "[Scene " goes up to index 6, so name should start at 7 -> now has reformatted to just be __], so start at 0
        //string[] parsedLine;
        //List<string> goToSceneNames = new List<string>();
        //for (int i=1; i<sceneText.Length; i++)
        //{
        //    parsedLine = sceneText[i].Split(':');
        //    if (parsedLine[0][0] == '[') // could either be a choice option or just a goto statement
        //    {
        //        // parsedLine[0] = [Goto Scene __] -> Split by ' ' = [ [Goto, Scene, __] ] -> [2] = __] -> Substring(0,2) = __
        //        goToSceneNames.Add(parsedLine[0].Split(' ')[2].Substring(0,2));

        //        if (parsedLine.Length == 2) // is a choice option cause it has Answer in it
        //        {
        //            // parsedLine = [[Goto Scene __], Answer]
        //            AddTextToMenu(parsedLine[1]);
        //        }
        //    }
        //    else if (parsedLine.Length == 3) // will be 3 if ChoiceTrigger!
        //    {
        //        // parsedLine = [ ChoiceTrigger, Name, Question ] -> only want Name and Question
        //        hasChoices = true;
        //        choiceText = new string[2];
        //        choiceText[0] = parsedLine[1];
        //        choiceText[1] = parsedLine[2];
        //    }
        //    else
        //    {
        //        dialogueText.Add(parsedLine);
        //    }
        //}
        //if (goToSceneNames.Count == 1) // if there were no choices... then the only node to go to is the next?
        //{
        //    hasChoices = false;
        //}

    }

    // AddTextToMenu: creates a Text object for a choice by creating a new GameObject with a Text component
    void AddTextToMenu(string choice)
    {
        numChoices++;
        choiceSize.text += "\n";
        GameObject newOption = new GameObject();
        newOption.name = "Choice " + numChoices.ToString();
        newOption.transform.SetParent(choiceDisplay.transform);
        newOption.AddComponent<Text>().text = choice;
        newOption.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        newOption.GetComponent<Text>().color = Color.black;
        newOption.GetComponent<Text>().font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        newOption.GetComponent<Text>().fontSize = 26;
        choiceOptions.Add(newOption);
    }
}