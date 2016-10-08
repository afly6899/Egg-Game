using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogGraph
{
	private Dictionary<string, Scene> scenes;
	private string currScene;

	public string CurrentSpeaker
	{
		get;
		private set;
	}

	public string CurrentText
	{
		get
		{
			return this.scenes [currScene].CurrentDialog;	
		}
	}

	public List<string> CurrentChoices 
	{
		get;
		private set;
	}

	public bool HasChoices
	{
		get
		{
			return CurrentChoices != null;
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


}