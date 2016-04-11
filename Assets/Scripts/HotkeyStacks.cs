using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HotkeyStacks : MonoBehaviour {

	Text stackText;

	void Start () 
	{
		stackText = GetComponent<Text>();
	}

	void Update () 
	{
		switch(gameObject.name)
		{
		case "Stacks 0":
			stackText.text = Master.instance.GetSkillStacks(0).ToString();
			break;
		case "Stacks 1":
			stackText.text = Master.instance.GetSkillStacks(1).ToString();
			break;
		case "Stacks 2":
			stackText.text = Master.instance.GetSkillStacks(2).ToString();
			break;
		case "Stacks 3":
			stackText.text = Master.instance.GetSkillStacks(3).ToString();
			break;
		}
	}
}
