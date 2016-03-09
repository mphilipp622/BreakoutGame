using UnityEngine;
using System.Collections;

public class createCollider : MonoBehaviour {

	private EdgeCollider2D boundaryCollider, gameOverTrigger;
	public bool gameOver = false;
	float horizontalExtent, vertExtent;
	
	void Start () {
		vertExtent = Camera.main.orthographicSize;
		horizontalExtent = Camera.main.orthographicSize * Screen.width / Screen.height;
		boundaryCollider = gameObject.AddComponent<EdgeCollider2D> ();
		boundaryCollider.points = new Vector2[] {new Vector2(-horizontalExtent, -vertExtent), new Vector2(-horizontalExtent, vertExtent)
			, new Vector2(horizontalExtent, vertExtent), new Vector2(horizontalExtent, -vertExtent)};
		gameOverTrigger = gameObject.AddComponent<EdgeCollider2D> ();
		gameOverTrigger.isTrigger = true;
		gameOverTrigger.points = new Vector2[] {new Vector2(-horizontalExtent, -vertExtent), new Vector2(horizontalExtent, -vertExtent)};
	}

	void OnTriggerEnter2D(Collider2D other) 
	{
		if(other.tag == "Ball")
			Master.instance.gameOver = true;
		else
			Destroy(other.gameObject);
	}
}