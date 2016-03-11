using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CooldownAnimation : MonoBehaviour {

	Image image;
	//[SerializeField]
	//float red = 1.0f, green = 0f;

	void Start () 
	{
		image = GetComponent<Image>();
	}
	
	void Update () 
	{
		switch(gameObject.name)
		{
		case "CooldownImage 0":
			if(Time.timeSinceLevelLoad < Master.instance.GetCooldownTime(0))
			{
				//red -= Time.deltaTime / Master.instance.GetBaseCooldown(0);
				//	green += Time.deltaTime / Master.instance.GetBaseCooldown(0);
				//	image.material.SetColor("_MKGlowTexColor", new Color(red, green, 0, 0));
				image.material.SetColor("_MKGlowTexColor", new Color(1, 0, 0, 0f));
			}
			//	image.material.SetColor("_MKGlowTexColor", new Color(Mathf.Lerp(1, 0, Time.timeSinceLevelLoad / Master.instance.GetBaseCooldown(0)), Mathf.Lerp(0, 1, Time.timeSinceLevelLoad / Master.instance.GetBaseCooldown(0)), 0, 0));
			else if(Time.timeSinceLevelLoad >= Master.instance.GetCooldownTime(0))
			{
				image.material.SetColor("_MKGlowTexColor", new Color(0, 1, 0, 0f));
				//	green = 0f;
				//	red = 1f;
			}
			break;
		case "CooldownImage 1":
			if(Time.timeSinceLevelLoad < Master.instance.GetCooldownTime(1))
			{
				//red -= Time.deltaTime / Master.instance.GetBaseCooldown(0);
				//	green += Time.deltaTime / Master.instance.GetBaseCooldown(0);
				//	image.material.SetColor("_MKGlowTexColor", new Color(red, green, 0, 0));
				image.material.SetColor("_MKGlowTexColor", new Color(1, 0, 0, 0f));
			}
			//	image.material.SetColor("_MKGlowTexColor", new Color(Mathf.Lerp(1, 0, Time.timeSinceLevelLoad / Master.instance.GetBaseCooldown(0)), Mathf.Lerp(0, 1, Time.timeSinceLevelLoad / Master.instance.GetBaseCooldown(0)), 0, 0));
			else if(Time.timeSinceLevelLoad >= Master.instance.GetCooldownTime(1))
			{
				image.material.SetColor("_MKGlowTexColor", new Color(0, 1, 0, 0f));
				//	green = 0f;
				//	red = 1f;
			}
			break;
		case "CooldownImage 2":
			if(Time.timeSinceLevelLoad < Master.instance.GetCooldownTime(2))
			{
				//red -= Time.deltaTime / Master.instance.GetBaseCooldown(0);
				//	green += Time.deltaTime / Master.instance.GetBaseCooldown(0);
				//	image.material.SetColor("_MKGlowTexColor", new Color(red, green, 0, 0));
				image.material.SetColor("_MKGlowTexColor", new Color(1, 0, 0, 0f));
			}
			//	image.material.SetColor("_MKGlowTexColor", new Color(Mathf.Lerp(1, 0, Time.timeSinceLevelLoad / Master.instance.GetBaseCooldown(0)), Mathf.Lerp(0, 1, Time.timeSinceLevelLoad / Master.instance.GetBaseCooldown(0)), 0, 0));
			else if(Time.timeSinceLevelLoad >= Master.instance.GetCooldownTime(2))
			{
				image.material.SetColor("_MKGlowTexColor", new Color(0, 1, 0, 0f));
				//	green = 0f;
				//	red = 1f;
			}
			break;
		case "CooldownImage 3":
			if(Time.timeSinceLevelLoad < Master.instance.GetCooldownTime(3))
			{
				//red -= Time.deltaTime / Master.instance.GetBaseCooldown(0);
				//	green += Time.deltaTime / Master.instance.GetBaseCooldown(0);
				//	image.material.SetColor("_MKGlowTexColor", new Color(red, green, 0, 0));
				image.material.SetColor("_MKGlowTexColor", new Color(1, 0, 0, 0f));
			}
			//	image.material.SetColor("_MKGlowTexColor", new Color(Mathf.Lerp(1, 0, Time.timeSinceLevelLoad / Master.instance.GetBaseCooldown(0)), Mathf.Lerp(0, 1, Time.timeSinceLevelLoad / Master.instance.GetBaseCooldown(0)), 0, 0));
			else if(Time.timeSinceLevelLoad >= Master.instance.GetCooldownTime(3))
			{
				image.material.SetColor("_MKGlowTexColor", new Color(0, 1, 0, 0f));
				//	green = 0f;
				//	red = 1f;
			}
			break;
		}


	}
}