using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour {

	Text scoreText;

	void Start () 
	{
		scoreText = GetComponent<Text>();
	}

	void Update () 
	{
		scoreText.text = Master.instance.GetScore().ToString();
	}
}
