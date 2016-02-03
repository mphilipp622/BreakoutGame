using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimeDisplay : MonoBehaviour {

	Text time;
	// Use this for initialization
	void Start () {
		time = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		time.text = Time.timeSinceLevelLoad.ToString();
	}
}
