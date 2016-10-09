using UnityEngine;
using UnityEngine.UI;

using System;
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

	public int NumChoices
	{
		get
		{
			return (this.CurrentChoices == null ? 0 : this.CurrentChoices.Count);
		}
	}

	public bool HasChoices
	{
		get
		{
			return this.NumChoices == 0;
		}
	}

	private void loadScenes(TextAsset sceneFile)
	{
		var lines = sceneFile.text.Split (Environment.NewLine);
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
		this.currScene = lines[0].Substring("[Scene ".Length);
		this.currScene = this.currScene.Substring(0, this.currScene.IndexOf(']'));
	}

	public DialogGraph(TextAsset sceneFile, string startScene = null)
	{
		loadScenes (sceneFile);
		this.currScene = startScene ?? this.currScene;
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