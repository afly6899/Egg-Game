using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BackgroundManager : MonoBehaviour {

    public CanvasRenderer canvas;
    public SpriteRenderer background;

	// Use this for initialization
	void Start () {
        float worldScreenHeight = Camera.main.orthographicSize * 2;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        transform.localScale = new Vector3(
            worldScreenWidth / background.sprite.bounds.size.x,
            worldScreenHeight / background.sprite.bounds.size.y, 1);
    }
}
