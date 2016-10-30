using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

public class GameLoader : MonoBehaviour {
	public static GameState CurrentGame;
	private const string SAVE_FILE = "save.ev";

	public static string SAVE_FILE_PATH()
	{
		return Path.Combine(Application.persistentDataPath, SAVE_FILE);
	}

	// Use this for initialization
	void Start()
	{
		
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}

	public static void SaveGame()
	{
		BinaryFormatter bf = new BinaryFormatter();

		using (FileStream saveStream = File.Create(SAVE_FILE_PATH()))
		{
			bf.Serialize(saveStream, CurrentGame);
		}
	}

	public static void LoadGame()
	{
		BinaryFormatter bf = new BinaryFormatter();

		using (FileStream loadStream = File.Open(SAVE_FILE_PATH(), FileMode.Open))
		{
			CurrentGame = (GameState)bf.Deserialize(loadStream);
		}

		CurrentGame.LoadScene(CurrentGame.CurrentScene);
	}
}
