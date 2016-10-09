using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class DialogueGUI : MonoBehaviour
{

    // this is temp; just to declare max num of choices you can make
    public int maxChoices;

    public TextAsset textfile;
    public GameObject dialoguePanel;
    public Text nameText;
    public Text dialogueText;
    public Canvas choiceDisplay;

    DialogGraph dialogueGraph;
    List<GameObject> choiceOptions;

    int selectedChoice;

    // Use this for initialization
    void Start()
    {
		
        //dialogueGraph = new DialogGraph(textfile, textfile.text.Substring(textfile.text.IndexOf(' ')+1, textfile.text.IndexOf(']')));
		dialogueGraph = new DialogGraph(textfile);
        choiceOptions = new List<GameObject>();
        AddTextToMenu();

        dialoguePanel.SetActive(true);
        nameText.text = dialogueGraph.CurrentSpeaker;
        dialogueText.text = dialogueGraph.CurrentText;
        if (dialogueGraph.HasChoices) { SetChoiceButtons(dialogueGraph.CurrentChoices); }
        selectedChoice = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (selectedChoice > -1)
            {
                selectedChoice = -1;
                dialogueGraph.nextDialogue(dialogueGraph.CurrentChoices[selectedChoice]);
                HideChoiceButtons(); // this means that once a choice is made, the choices are automatically deactivated
            }
            else
            {
                dialogueGraph.nextDialogue();
                if(dialogueGraph.HasChoices)
                {
                    selectedChoice = 0;
                    SetChoiceButtons(dialogueGraph.CurrentChoices);
                }
            }
            nameText.text = dialogueGraph.CurrentSpeaker;
            dialogueText.text = dialogueGraph.CurrentText;
        }
        if (selectedChoice > -1) { 
        if (Input.GetKeyDown(KeyCode.UpArrow) && selectedChoice > 0)
            {
                ChangeSelectionText(--selectedChoice, -1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && selectedChoice < dialogueGraph.CurrentChoices.Count-1)
            {
                ChangeSelectionText(++selectedChoice, 1);
            }
        }
    }

    void ChangeSelectionText(int newChoice, int changedBy)
    {
        choiceOptions[newChoice - changedBy].GetComponent<Text>().fontStyle = FontStyle.Normal;
        choiceOptions[newChoice].GetComponent<Text>().fontStyle = FontStyle.Bold;
    }

    void HideChoiceButtons()
    {
        for (int i = 0; i < maxChoices; i++)
        {
            choiceOptions[i].SetActive(false);
        }
    }

    void SetChoiceButtons(List<string> choices)
    {
		for (int i = 0; i < choices.Count(); i++)
        {
            choiceOptions[i].SetActive(true);
            choiceOptions[i].GetComponent<Text>().text = choices[i];
        }
    }

    void AddTextToMenu()
    {
        for (int i = 0; i < maxChoices; i++)
        {
            GameObject newOption = new GameObject();
            newOption.name = "Choice " + (i + 1).ToString();
            newOption.transform.SetParent(choiceDisplay.transform);
            newOption.transform.localScale = new Vector3(1, 1, 1);
            newOption.AddComponent<Text>().text = " ";
            newOption.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
            newOption.GetComponent<Text>().color = Color.black;
            newOption.GetComponent<Text>().font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            newOption.GetComponent<Text>().fontSize = 24;
            newOption.SetActive(false);
            choiceOptions.Add(newOption);
        }
    }
}
