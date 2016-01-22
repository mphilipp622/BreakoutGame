using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour {

	private Rigidbody2D ball;
	public float ballSpeedMultiplier = 1f;
	float min_y_velocity = 5.0f;
	float min_x_velocity = 5.0f;

	void Start () 
	{
		ball = GetComponent<Rigidbody2D> ();
		ball.AddForce (new Vector2(Random.Range(-10 * ballSpeedMultiplier, 10), 10), ForceMode2D.Impulse);
	}

	void Update () 
	{

		//Check and see if the ball is not moving enough along either X or Y axis. The amount necessary is dictated by min_y_velocity and min_x_velocity variables.
		if((ball.velocity.y < min_y_velocity) && 
			(Mathf.Sign(ball.velocity.y) == 1)) //returns 1 if y velocity is positive or 0
		{
			ball.AddForce(new Vector2(0, 5f));
		} 
		else if((ball.velocity.y > -min_y_velocity) && 
			(Mathf.Sign(ball.velocity.y) == -1)) //returns -1 if y velocity is negative
		{
			ball.AddForce(new Vector2(0, -5f));
		}

		if((ball.velocity.x < min_x_velocity) && 
			(Mathf.Sign(ball.velocity.x) == 1))
		{
			ball.AddForce(new Vector2(5f, 0));
		} 
		else if((ball.velocity.x > -min_x_velocity) && 
			(Mathf.Sign(ball.velocity.x) == -1))
		{
			ball.AddForce(new Vector2(-5f, 0));
		}
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
