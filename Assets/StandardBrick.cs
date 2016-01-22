using UnityEngine;
using System.Collections;

public class StandardBrick : MonoBehaviour {

	void OnCollisionEnter2D(Collision2D collision){
		//if (collision.gameObject.tag == "Ball")
		Master.instance.RemoveBrick(transform);
		Destroy (gameObject);
	}

	void Update()
	{
		if(transform.position.y <= Master.instance.paddle.position.y)
			Master.instance.gameOver = true;
	}
	/*void OnCollisionStay2D(Collision2D coll){
		Debug.Log ("Collision Stay");
		if (coll.gameObject.tag == "Brick") {
			Debug.Log("Collide Brick");
			transform.position = new Vector2 (Random.Range (Camera.main.orthographicSize * Screen.width / Screen.height, 
			                                             (Camera.main.orthographicSize * Screen.width / Screen.height) * -1),
			                                 Random.Range (Camera.main.orthographicSize / 2, Camera.main.orthographicSize));
	
		}
	}

	void OnCollisionExit2D(Collision2D coll){
		Debug.Log ("Collision Exit");
		if (coll.gameObject.tag == "Brick") {
			Debug.Log("Collide Brick Exit");
			transform.position = new Vector2 (Random.Range (Camera.main.orthographicSize * Screen.width / Screen.height, 
			                                                (Camera.main.orthographicSize * Screen.width / Screen.height) * -1),
			                                  Random.Range (Camera.main.orthographicSize / 2, Camera.main.orthographicSize));
			
		}
	}*/
}
