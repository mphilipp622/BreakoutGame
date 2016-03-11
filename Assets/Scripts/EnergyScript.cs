using UnityEngine;
using System.Collections;

public class EnergyScript : MonoBehaviour {

	void Start () 
	{
		
	}

	void Update () 
	{
		transform.position = new Vector2(transform.position.x, transform.position.y - (Time.deltaTime * 2.0f));
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.tag == "Player")
		{
			if(Master.instance.energy < Master.instance.maxEnergy)
				Master.instance.energy += 20;
			
			Destroy(gameObject);
		}


	}
}
