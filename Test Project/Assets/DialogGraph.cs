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
		get;
		private set;
	}

	public List<string> CurrentChoices 
	{
		get;
		private set;
	}

	private void loadScenes(string sceneFile)
	{
		var lines = System.IO.File.ReadAllLines (sceneFile);
		List<string> currSceneText = null;

		foreach (var line in lines) 
		{
			
		}
	}

	public DialogGraph(string sceneFile)
	{
	}


}