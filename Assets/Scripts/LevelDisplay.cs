using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelDisplay : MonoBehaviour {

	Text level;
	// Use this for initialization
	void Start () {
		level = GetComponent<Text>();
	}

	// Update is called once per frame
	void Update () {
		level.text = "Level: " + Master.instance.GetMilestone().ToString();
		//time.text = Time.realtimeSinceStartup.ToString();
	}
}
