using UnityEngine;
using System.Collections;

public class WreckingBallCollision : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if(Master.instance.wreckingDamage > 0)
		{
			Master.instance.wreckingDamage--;
			SendMessageUpwards("SetHP", 0, SendMessageOptions.RequireReceiver);
		}
	}
}
