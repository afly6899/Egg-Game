using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;


public class DialogueManager : MonoBehaviour
{

    // Input Parameters:
    // nameText (Text) -> Text UI that displays the name of the speaker
    // dialogueText (Text) -> Text UI that displays the dialogue of the speaker
    // choiceDisplay (Canvas) -> Canvas UI that holds the Text UI for the choices (is parent to choiceSize)
    // originalFile (TextAsset) -> Text File that contains all the scene information
    public Text nameText;
    public Text dialogueText;
    public Canvas choiceDisplay;
    public TextAsset originalFile;

    Dictionary<string, Scene> scenesByName;
    Dictionary<string, string[]> sceneConnections;

    List<GameObject> choiceOptions;

    int chosenChoice;
    bool isLastScene;
    bool scenesContinue;
    bool isMakingChoice;
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
                scenesByName[newScene.getSceneName()] = newScene;
                sceneConnections[newScene.getSceneName()] = potentialConnections.ToArray();
            }
        }

        chosenChoice = 0;
        currentSceneName = scenes[1].Substring(0,2);
        isLastScene = !(sceneConnections[currentSceneName].Length > 0);
        isMakingChoice = false;
        scenesContinue = true;

        nextDialogue(scenesByName[currentSceneName].getDialogue());
    }

    // Will update the system every cycle!
    void Update()
    {
        if (scenesContinue) // only takes in input if the scene is still going on
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (isLastScene && scenesByName[currentSceneName].getIsDone())
                {
                    scenesContinue = false;
                }

                if (isMakingChoice)
                {
                    isMakingChoice = false;
                    // select the choice?
                    nextScene(chosenChoice);
                    nextDialogue(scenesByName[currentSceneName].getDialogue());
                    HideChoices(); // this means that once a choice is made, the choices are automatically deactivated
                }
                else
                {
                    if (scenesByName[currentSceneName].getIsDone()) // if the current scene is done: check if have choice, else keep the scene going
                    {
                        if (scenesByName[currentSceneName].getHasChoice()) // if this scene has choices, then want to display them since the scene is done
                        {
                            chosenChoice = 0;
                            isMakingChoice = true;
                            DisplayChoices();
                        }
                        else // if the scene doesn't have choices, then move on to the next scene
                        {
                            nextScene();
                            if (!isLastScene)
                            {
                                nextDialogue(scenesByName[currentSceneName].getDialogue());
                            }
                        }
                    }
                    else
                    {
                        nextDialogue(scenesByName[currentSceneName].getDialogue());
                    }
                }

            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) && chosenChoice > 0)
            {
                chosenChoice--;
                ChangeSelectionText(chosenChoice, -1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && chosenChoice < sceneConnections[currentSceneName].Length-1)
            {
                chosenChoice++;
                ChangeSelectionText(chosenChoice, 1);
            }
        }
    }

    // Sets up the choice display stuff - already assuming we did the check for having choices
    void DisplayChoices()
    {
        // want a Text GameObject for each choice
        // for GameObjects that are already there, want to reuse
        // for new GameObjects that are needed, want to create
        int targetCount = sceneConnections[currentSceneName].Length;
        string[] choicesToDisplay = scenesByName[currentSceneName].getChoices();
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
        chosenChoice = 0;
        ChangeSelectionText(0, 0);
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

    void ChangeSelectionText(int newChoice, int changedBy)
    {
        choiceOptions[newChoice-changedBy].GetComponent<Text>().fontStyle = FontStyle.Normal;
        choiceOptions[newChoice].GetComponent<Text>().fontStyle = FontStyle.Bold;
    }

    void nextScene(int chosenScene = 0)
    {
        if (sceneConnections[currentSceneName].Length > 0)
        {
            currentSceneName = sceneConnections[currentSceneName][chosenScene];
        }
        else
        {
            isLastScene = true;
        }
    }

    void nextDialogue(string[] text)
    {
        nameText.text = text[0];
        dialogueText.text = text[1];
    }
}
    




























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
    int currentDialogue;
    bool hasChoices; // may remove public and have a getIfHasChoices function instead
    bool isDone;
    string sceneName; // may remove public and have a getName function instead... we'll have to see
    List<string> choices; // TEMP!; maybe remove public and have a getChoices function instead?
    List<string[]> dialogueText; // holding a list of [Name, Dialogue]

    Text choiceSize;
    Canvas choiceDisplay;
    private string[] v;

    // Scene(): constructor for Scene that takes in the array of text for a scene, does all the parsing!
    public Scene(string[] sceneText, ref List<string> potentialConnections)
    {
        // Initializing all variables
        currentDialogue = 0;
        isDone = false;
        hasChoices = false;
        choices = new List<string>();
        dialogueText = new List<string[]>();



        sceneName = sceneText[0].Substring(0, sceneText[0].Length-2);
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
                potentialConnections.Add(parsedLine[0].Split(' ')[2].Substring(0, 2));
                // if there's a choice afterwards, save it as well
                if (parsedLine.Length == 2) // is a choice option cause it has Answer in it
                {
                    // parsedLine = [[Goto Scene __], Answer]
                    choices.Add(parsedLine[1]);
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

    public bool getIsDone() { return isDone; }
    public bool getHasChoice() { return hasChoices; }
    public string getSceneName() { return sceneName; }
    public string[] getChoices() { return choices.ToArray(); }

    public string[] getDialogue() // sets isDone to true if this is the last line of dialogue
    {
        currentDialogue++;
        if (currentDialogue == dialogueText.Count)
        {
            isDone = true;
        }
        return dialogueText[currentDialogue-1];
    }
}