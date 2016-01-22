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
			SendMessageUpwards("RemoveBrick", transform);
			Destroy (gameObject);
			break;
		case 1: 
			brickSprite.color = Color.red;
			break;
		case 2: 
			brickSprite.sprite = newSprite;
			break;
		}
	}
	
	void OnCollisionEnter2D(Collision2D collision){
		hp--;
	}
}
