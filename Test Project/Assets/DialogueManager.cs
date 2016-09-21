using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;


public class DialogueManager : MonoBehaviour
{

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
    Dictionary<string, string[]> sceneConnections;

    List<GameObject> choiceOptions;

    string currentSceneName;

    // To initialize all the stuffs
    void Start()
    {
        scenesByName = new Dictionary<string, Scene>();
        sceneConnections = new Dictionary<string, string[]>();
        choiceOptions = new List<GameObject>();
        string[] scenes = Regex.Split(originalFile.text, "\\[Scene ");
        Debug.Log(scenes.Length.ToString());
        foreach (string scene in scenes)
        {
            if (scene.Length > 0)
            {
                List<string> potentialConnections = new List<string>();
                Scene newScene = new Scene(scene.TrimEnd().Split('\n'), ref potentialConnections);
                scenesByName[newScene.sceneName] = newScene;
                // sceneConnections[newScene.sceneName] = potentialConnections.ToArray(); // -> potentialConnections has the choice dialogue, not the scene names!
                sceneConnections[newScene.sceneName] = newScene.goToSceneNames.ToArray();
            }
        }
        currentSceneName = "A0";
    }

    // Will update the system every cycle!
    void Update()
    {
        nameText.text = currentSceneName;
        dialogueText.text = HelpfulFunctions.arrayToString(sceneConnections[currentSceneName]);
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (scenesByName[currentSceneName].hasChoices)
            {
                DisplayChoices();
            }
            else
            {
                HideChoices();
            }
            currentSceneName = sceneConnections[currentSceneName][0];
        }
    }

    // Sets up the choice display stuff - already assuming we did the check for having choices
    void DisplayChoices()
    {
        // want a Text GameObject for each choice
        // for GameObjects that are already there, want to reuse
        // for new GameObjects that are needed, want to create
        int targetCount = sceneConnections[currentSceneName].Length;
        string[] choicesToDisplay = scenesByName[currentSceneName].choices.ToArray();
        int i;

        // if targetCount is the min, then only up to targetCount number of GameObjects will be set -> need to dectivate the others
        // if choiceOptions.Count is the min, then only choiceOption.Count number of choices will be prepared -> need to make more GameObjects
        // if they're equal, then we're good!
        for (i=0; i< Mathf.Min(targetCount, choiceOptions.Count); i++)
        {
            choiceOptions[i].SetActive(true);
            choiceOptions[i].GetComponent<Text>().text = choicesToDisplay[i];
        }
        
        for (; i < targetCount ;i++)
        {
            AddTextToMenu(choicesToDisplay[i], i + 1);
        }
    }

    void HideChoices()
    {
        for (int i=0; i < choiceOptions.Count; i++)
        {
            choiceOptions[i].SetActive(false);
        }
    }

    // AddTextToMenu: creates a Text object for a choice by creating a new GameObject with a Text component
    void AddTextToMenu(string choice, int choiceNumber)
    {
        GameObject newOption = new GameObject();
        newOption.name = "Choice " + choiceNumber.ToString();
        newOption.transform.SetParent(choiceDisplay.transform);
        newOption.transform.localScale = new Vector3(1, 1, 1);
        newOption.AddComponent<Text>().text = choice;
        newOption.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
        newOption.GetComponent<Text>().color = Color.black;
        newOption.GetComponent<Text>().font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        newOption.GetComponent<Text>().fontSize = 24;
        choiceOptions.Add(newOption);
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
    public bool hasChoices; // may remove public and have a getIfHasChoices function instead
    public string sceneName; // may remove public and have a getName function instead... we'll have to see
    string[] choiceText;
    List<string[]> dialogueText; // holding a list of [Name, Dialogue]
    public List<string> choices; // TEMP!; maybe remove public and have a getChoices function instead?
    List<GameObject> choiceOptions;
    public List<string> goToSceneNames; // same as above

    Text choiceSize;
    Canvas choiceDisplay;
    private string[] v;

    // Scene(): constructor for Scene that takes in the array of text for a scene, does all the parsing!
    public Scene(string[] sceneText, ref List<string> potentialConnections)
    {
        // Initializing all variables
        numChoices = 0;
        hasChoices = false;
        choices = new List<string>();
        dialogueText = new List<string[]>();
        goToSceneNames = new List<string>();



        sceneName = sceneText[0].Substring(0, 2); // "[Scene " goes up to index 6, so name should start at 7 -> now has reformatted to just be __], so start at 0
        Debug.Log(sceneName);

        string[] parsedLine;
        for (int i = 1; i < sceneText.Length; i++)
        {
            parsedLine = sceneText[i].Split(':');

            // first: has [Goto Scene A1] condition
            if (parsedLine[0][0] == '[') // could either be a choice option or just a goto statement
            {
                //Debug.Log("Could be choice or goto statement: " + parsedLine.Length);
                // parsedLine[0] = [Goto Scene __] -> Split by ' ' = [ [Goto, Scene, __] ] -> [2] = __] -> Substring(0,2) = __
                // save the scene name in the goToSceneNames list
                goToSceneNames.Add(parsedLine[0].Split(' ')[2].Substring(0, 2));
                // if there's a choice afterwards, save it as well
                if (parsedLine.Length == 2) // is a choice option cause it has Answer in it
                {
                    // parsedLine = [[Goto Scene __], Answer]
                    choices.Add(parsedLine[1]);
                    potentialConnections.Add(parsedLine[1]);
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


}