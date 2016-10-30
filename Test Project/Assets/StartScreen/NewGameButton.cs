using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NewGameButton : MonoBehaviour {
	public Button newButton;
	public Canvas newGameCanvas;

	// Use this for initialization
	void Start () {
		newButton.onClick.AddListener(OpenNewGamePopup);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OpenNewGamePopup()
	{
		newGameCanvas.gameObject.SetActive(true);
	}
}
