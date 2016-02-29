using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour {

	private Rigidbody2D ball;
	public float ballSpeedMultiplier = 1f;
	float min_y_velocity = 5.0f;
	float min_x_velocity = 5.0f;
	Material ballMat;
	Light spotlight;

	void Start () 
	{
		ball = GetComponent<Rigidbody2D> ();
		ballMat = GetComponent<SpriteRenderer>().material;
		spotlight = GetComponentInChildren<Light>();
	}

	void Update () 
	{

		if(Master.instance.ballInPlay) //Is Ball moving?
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
		else
		{
			if(Input.GetKeyDown(KeyCode.Mouse0))
			{
				transform.parent = null;
				ball.AddForce (new Vector2(Random.Range(-10 * ballSpeedMultiplier, 10 * ballSpeedMultiplier), 10 * ballSpeedMultiplier), ForceMode2D.Impulse);
				Master.instance.ballInPlay = true;
			}
		}

		if(transform.tag != "DupeBall")
		{
			if(ball.velocity.y > 0 && ball.velocity.x > 0)
				ballMat.SetTextureOffset("_MKGlowTex", new Vector2(Time.timeSinceLevelLoad, Time.timeSinceLevelLoad));
			else if(ball.velocity.y > 0 && ball.velocity.x < 0)
				ballMat.SetTextureOffset("_MKGlowTex", new Vector2(-Time.timeSinceLevelLoad, Time.timeSinceLevelLoad));
			else if(ball.velocity.y < 0 && ball.velocity.x > 0)
				ballMat.SetTextureOffset("_MKGlowTex", new Vector2(Time.timeSinceLevelLoad, -Time.timeSinceLevelLoad));
			else if(ball.velocity.y < 0 && ball.velocity.x < 0)
				ballMat.SetTextureOffset("_MKGlowTex", new Vector2(-Time.timeSinceLevelLoad, -Time.timeSinceLevelLoad));
		}

		//Change layer collision matrix when wrecking ball is active. Make sure ball only hits Trigger colliders on the bricks so that the ball can pass through
		//bricks without stopping.
		if(Master.instance.wreckingDamage > 0)
		{
			Physics2D.IgnoreLayerCollision(8, 10, true);
			Physics2D.IgnoreLayerCollision(8, 11, false);
		}
		else
		{
			Physics2D.IgnoreLayerCollision(8, 10, false);
			Physics2D.IgnoreLayerCollision(8, 11, true);
		}

		if(Master.instance.isWrecking)
			spotlight.intensity = Master.instance.wreckingStacks;
		else
			spotlight.intensity = (float)Master.instance.wreckingDamage;
			

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
