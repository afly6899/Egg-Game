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

public class Scene
{
	public class DialogueLine
	{
		public string Speaker;
		public string Dialogue;
		public Dictionary<string, string> Options; // maps the text of the option to the name of the scene to which the option takes you
		public string Goto;

		public DialogueLine()
		{
			this.Speaker = "";
			this.Dialogue = "";
			this.Options = null;
			this.Goto = null;
		}
	}

    private int currentDialogue;
	private List<DialogueLine> dialogueText; // holding a list of [Name, Dialogue]

	public string Name
	{
		get;
		private set;
	}

	public DialogueLine CurrentDialogue
	{
		get
		{
			return this.dialogueText [this.currentDialogue];
		}
	}

	public bool IsDone
	{
		get
		{
			return this.currentDialogue >= this.dialogueText.Count;
		}
	}

    // Scene(): constructor for Scene that takes in the array of text for a scene, does all the parsing!
	public Scene(List<string> sceneText)
    {
        currentDialogue = 0;
		dialogueText = new List<DialogueLine>();
		this.Name = sceneText [0].Substring ("[Scene ".Length);
		this.Name = this.Name.Substring (0, this.Name.IndexOf (']'));

		for (int i = 1; i < sceneText.Count; i++)
		{
			if (sceneText [i] == null || sceneText [i].Trim () == "")
				continue;
			
			string []parsedLine = sceneText[i].Split(':');
			DialogueLine currLine = new DialogueLine();

			if (parsedLine[0].Trim() == "ChoiceTrigger")
			{
				currLine.Speaker = parsedLine[1];
				currLine.Dialogue = parsedLine[2];

				currLine.Options = new Dictionary<string, string>();

				for (++i; i < sceneText.Count && sceneText[i].StartsWith("[Goto Scene "); i++)
				{
					string next = sceneText[i].Substring("[Goto Scene ".Length);
					next = next.Substring(0, next.IndexOf(']'));

					string choiceText = sceneText[i].Substring(sceneText[i].IndexOf(':') + 1);
					currLine.Options.Add(next, choiceText);
				}

				this.dialogueText.Add(currLine);
				break;
			}
			else if (parsedLine[0].Trim().StartsWith("[Goto Scene "))
			{
				currLine.Goto = sceneText[i].Substring("[Goto Scene ".Length);
				currLine.Goto = currLine.Goto.Substring(0, currLine.Goto.IndexOf(']'));

				this.dialogueText.Add(currLine);
			}
			else
			{
				currLine.Speaker = parsedLine[0];
				currLine.Dialogue = parsedLine[1];

				this.dialogueText.Add(currLine);
			}
		}
    }

    public DialogueLine nextDialogue()
    {
		if (this.currentDialogue < this.dialogueText.Count - 1)
			this.currentDialogue++;
		return this.CurrentDialogue;
    }
}