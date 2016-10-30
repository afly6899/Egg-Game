using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnableLoadButton : MonoBehaviour {
	public Button LoadButton;

	// Use this for initialization
	void Start () {
		this.LoadButton.enabled = System.IO.File.Exists(GameLoader.SAVE_FILE_PATH());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void loadGame()
	{
		GameLoader.LoadGame();
	}
}
