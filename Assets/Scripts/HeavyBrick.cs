using UnityEngine;
using System.Collections;

public class HeavyBrick : MonoBehaviour {

	private int hp = 3;
	private SpriteRenderer brickSprite;
	[SerializeField]
	private Sprite newSprite;
	
	void Start () {
		brickSprite = GetComponent<SpriteRenderer> ();
	}
	
	void Update () {
		switch (hp) {
		case 0: 
			//Master.instance.RemoveBrick(transform, transform.GetComponent<Renderer>());
			Destroy (gameObject);
			break;
		case 1: 
			brickSprite.color = Color.red;
			break;
		case 2: 
			brickSprite.sprite = newSprite;
			break;
		}

		if(transform.position.y <= Master.instance.paddle.position.y)
			Master.instance.gameOver = true;
	}
	
	void OnCollisionEnter2D(Collision2D collision)
	{
		hp--;
	}
}
