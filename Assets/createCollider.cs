using UnityEngine;
using System.Collections;

public class createCollider : MonoBehaviour {
	
	private EdgeCollider2D boundaryCollider, gameOverTrigger;
	public bool gameOver = false;
	
	void Start () {
		float vertExtent = Camera.main.orthographicSize;
		float horizontalExtent = Camera.main.orthographicSize * Screen.width / Screen.height;
		boundaryCollider = gameObject.AddComponent<EdgeCollider2D> ();
		boundaryCollider.points = new Vector2[] {new Vector2(-horizontalExtent, -vertExtent), new Vector2(-horizontalExtent, vertExtent)
			, new Vector2(horizontalExtent, vertExtent), new Vector2(horizontalExtent, -vertExtent)};
		gameOverTrigger = gameObject.AddComponent<EdgeCollider2D> ();
		gameOverTrigger.isTrigger = true;
		gameOverTrigger.points = new Vector2[] {new Vector2(-horizontalExtent, -vertExtent), new Vector2(horizontalExtent, -vertExtent)};
	}

	void OnTriggerEnter2D(Collider2D other) {
		Time.timeScale = 0;
		GameObject.Find ("Menu UI").GetComponent<GameOverMenuScript> ().gameOver = true;
	}
}