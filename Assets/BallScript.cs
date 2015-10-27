using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour {

	private Rigidbody2D ball;
	public float ballSpeedMultiplier = 1f;

	void Start () {
		ball = GetComponent<Rigidbody2D> ();
		ball.AddForce (new Vector2(Random.Range(-10 * ballSpeedMultiplier, 10), 10), ForceMode2D.Impulse);
	}

	void Update () {
	
	}

	/*void OnCollisionEnter2D (Collision2D collision){
		if (collision.gameObject.tag == "Player") {
			if(transform.position.x < collision.collider.bounds.center.x){

				ball.velocity = new Vector2(Random.Range(-10, -5) * ballSpeedMultiplier, 10);

			}
			else if(transform.position.x > collision.collider.bounds.center.x){

				ball.velocity = new Vector2(Random.Range(5, 10) * ballSpeedMultiplier, 10);

			}
		}
	}*/
}
