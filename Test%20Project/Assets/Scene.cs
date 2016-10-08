using UnityEngine;
using System.Collections.Generic;

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
//  1) create helpful comments on what's happening in the code (for each section or statement?)
//  2) should the Scene class hold the text for the choices? Or should the DialogueManager?

class Scene
{
    int currentDialogue;
    bool hasChoices;
    bool isDone;
    string sceneName;
    List<string> choices; // TEMP!
    List<string[]> dialogueText; // holding a list of [Name, Dialogue]

    // Scene(): constructor for Scene that takes in the array of text for a scene, does all the parsing!
    public Scene(string[] sceneText, ref List<string> potentialConnections)
    {
        // Initializing all variables
        currentDialogue = 0;
        isDone = false;
        hasChoices = false;
        choices = new List<string>();
        dialogueText = new List<string[]>();
		Debug.Log ("Creating new scene: " + sceneText [0]);
		sceneName = sceneText[0].Substring(0, sceneText[0].IndexOf(']'));

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
        return dialogueText[currentDialogue - 1];
    }
}