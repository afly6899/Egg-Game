using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartNewGame : MonoBehaviour {
	public Button StartButton;

	// Use this for initialization
	void Start () {
		var btn = StartButton.GetComponent<Button> ();
		btn.onClick.AddListener (StartGame);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void StartGame()
	{
		GameLoader.CurrentGame = new GameState();
		GameLoader.CurrentGame.LoadScene("Dialogue");
	}
}
