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