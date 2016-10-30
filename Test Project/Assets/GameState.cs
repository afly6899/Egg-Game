using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameState 
{
	private List<Scene.DialogOption> choicesMade = new List<Scene.DialogOption>();

	public string CurrentScene
	{
		get;
		private set;
	}

	public void TakeDialogOption(Scene.DialogOption option)
	{
		choicesMade.Add(option);
	}

	public int timesOptionWasTaken(string scene, string choice, string option)
	{
		return choicesMade.Where(c => 
			c.InScene.Name == scene &&
			c.ChoiceName == choice &&
			c.OptionName == option).Count();
	}

	public void LoadScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
		this.CurrentScene = sceneName;
	}
}
