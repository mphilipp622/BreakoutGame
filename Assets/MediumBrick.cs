using UnityEngine;
using System.Collections;

public class MediumBrick : MonoBehaviour {

	private int hp = 2;
	private SpriteRenderer brickSprite;

	void Start () {
		brickSprite = GetComponent<SpriteRenderer> ();
	}

	void Update () {
		if (hp == 0)
		{
			//Master.instance.RemoveBrick(transform, transform.GetComponent<Renderer>());
			Destroy (gameObject);
		}

		if(transform.position.y <= Master.instance.paddle.position.y)
			Master.instance.gameOver = true;
	}

	void OnCollisionEnter2D(Collision2D collision){
		hp--;
		brickSprite.color = Color.red;
	}
}
