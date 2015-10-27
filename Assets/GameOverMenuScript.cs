using UnityEngine;
using System.Collections;

public class GameOverMenuScript : MonoBehaviour {

	GameObject gameOverPanel;
	public bool gameOver = false;
	// Use this for initialization
	void Start () {
		gameOverPanel = transform.FindChild ("GameOverPanel").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if (gameOver) {
			gameOverPanel.SetActive(true);
		}
	}
}
