using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogGraph
{
	private Dictionary<string, Scene> scenes;
	private string currScene;

	public Scene CurrentScene
	{
		get
		{
			return this.scenes [currScene];
		}
	}

	public string CurrentSpeaker
	{
		get
		{
			return this.CurrentScene.CurrentDialogue.Speaker;
		}
	}

	public string CurrentText
	{
		get
		{
			return this.CurrentScene.CurrentDialogue.Dialogue;
		}
	}

	public List<string> CurrentChoices 
	{
		get
		{
			return this.CurrentScene.CurrentDialogue.Options.Keys;
		}
	}

	public bool HasChoices
	{
		get
		{
			return this.CurrentChoices != null;
		}
	}

	private void loadScenes(string sceneFile)
	{
		var lines = System.IO.File.ReadAllLines (sceneFile);
		List<string> currSceneText = null;

		Scene s = null;

		foreach (var line in lines) 
		{
			if (line.StartsWith ("[Scene ") && currSceneText != null) 
			{
				s = new Scene (currSceneText);
				this.scenes.Add (s.Name, s);
				currSceneText = new List<string> ();
			}

			currSceneText.Add (line);
		}

		s = new Scene(currSceneText);
		this.scenes.Add(s.Name, s);

	}

	public DialogGraph(string sceneFile, string startScene)
	{
		loadScenes (sceneFile);
		this.currScene = startScene;
	}

	public void nextDialogue(string choice = null)
	{
		if (!this.HasChoices && this.CurrentScene.CurrentDialogue.Goto == null)
			this.CurrentScene.nextDialogue ();
		else if (this.HasChoices)
			this.currScene = this.CurrentScene.CurrentDialogue.Options [choice];

		if (this.CurrentScene.CurrentDialogue.Goto != null)
			this.currScene = this.CurrentScene.CurrentDialogue.Goto;
	}
}