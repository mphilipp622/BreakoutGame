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
			Destroy (gameObject);
	}

	void OnCollisionEnter2D(Collision2D collision){
		hp--;
		brickSprite.color = Color.red;
	}
}
