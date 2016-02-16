using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HotkeyGraphics : MonoBehaviour 
{
	[SerializeField]
	GameObject[] hotkeys;
	[SerializeField]
	Image[] icons;
	int currentSelection = 0;

	void Start () 
	{

		for(int i = 0; i < hotkeys.Length; i++)
		{
			if(hotkeys[i] == null)
				Debug.LogError("ASSIGN A GAME OBJECT TO INDEX " + i + " of 'hotkeys' ARRAY ON 'HOTKEYS' GAMEOBJECT");
		}
		for(int i = 0; i < icons.Length; i++)
		{
			if(icons[i] == null)
				Debug.LogError("ASSIGN AN ICON TO INDEX " + i + " of 'icons' ARRAY ON 'HOTKEYS' GAMEOBJECT");
		}

		SetHotkeyIcons();
		ChangeActiveHotkey(Master.instance.GetCurrentHotkey());
	}

	void Update () 
	{
		if(currentSelection != Master.instance.GetCurrentHotkey())
		{
			ChangeActiveHotkey(Master.instance.GetCurrentHotkey());
			currentSelection = Master.instance.GetCurrentHotkey();
		}
	}

	void ChangeActiveHotkey(int num)
	{
		for(int i = 0; i < hotkeys.Length; i++)
		{
			if(i == num)
				hotkeys[i].gameObject.SetActive(false);
			else
				hotkeys[i].gameObject.SetActive(true);
		}
	}

	void SetHotkeyIcons()
	{
		for(int i = 0; i < icons.Length; i++)
		{
			icons[i].sprite = Master.instance.GetSkillIcons(i);
		}
	}
}
