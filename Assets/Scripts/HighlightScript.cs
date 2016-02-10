using UnityEngine;
using System.Collections;

public class HighlightScript : MonoBehaviour {

	SpriteRenderer renderer;
	float alpha;
	bool goUp = true;
	Color newColor;

	void Start () 
	{
		renderer = GetComponent<SpriteRenderer>();

		newColor = new Color (renderer.color.r, renderer.color.g, renderer.color.b, 0f);
	}

	void Update () 
	{
		if(alpha >= .2f)
			goUp = false;
		else if(alpha <= 0f)
			goUp = true;
		
		if(goUp)
			alpha += 0.1f * 0.2f;
		else if(!goUp)
			alpha -= 0.1f * 0.2f;
		
		newColor.a = alpha;
		renderer.color = newColor;
	}
}
