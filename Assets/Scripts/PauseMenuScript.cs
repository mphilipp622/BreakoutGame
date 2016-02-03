using UnityEngine;
using System.Collections;

public class PauseMenuScript : MonoBehaviour {

	GameObject pausePanel;
	// Use this for initialization
	void Start () {
		pausePanel = transform.FindChild ("Pause_Menu_Panel").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape) && Time.timeScale > 0) {
			Time.timeScale = 0;
			pausePanel.SetActive (true);
		} else if (Input.GetKeyDown (KeyCode.Escape) && Time.timeScale == 0) {
			Time.timeScale = 1;
			pausePanel.SetActive (false);
		}
	}
}
