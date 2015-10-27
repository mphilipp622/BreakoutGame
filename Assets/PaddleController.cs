using UnityEngine;
using System.Collections;

public class PaddleController : MonoBehaviour {

	[SerializeField]
	private float movementSpeed = 20f;
	private Vector2 currentPosition;
	private float spriteWidth;
	private SpriteRenderer sprite;
	public bool hittingLeft, hittingRight;
	private float horizontalCam;

	void Start () {
		sprite = GetComponent<SpriteRenderer> ();
		spriteWidth = sprite.sprite.bounds.size.x * transform.localScale.x;
		currentPosition = transform.position;
		horizontalCam = Camera.main.orthographicSize * Screen.width / Screen.height;
	}

	void Update () {
		if (Input.GetAxis ("Mouse X") != 0)
			MovePaddle ();

		transform.position = currentPosition;

		if (transform.position.x - (spriteWidth / 2) <= -horizontalCam)
			hittingLeft = true;
		else if (transform.position.x + spriteWidth / 2 >= horizontalCam)
			hittingRight = true;

	}

	void MovePaddle(){
		if (hittingLeft) {
			currentPosition.x = -horizontalCam + spriteWidth/2;
			if (Input.GetAxis ("Mouse X") > 0) {
				currentPosition.x += (Input.GetAxis ("Mouse X")) * movementSpeed * Time.deltaTime;
				hittingLeft = false;
			}
		} else if (hittingRight) {
			currentPosition.x = horizontalCam - spriteWidth / 2;
			if (Input.GetAxis ("Mouse X") < 0) {
				currentPosition.x += (Input.GetAxis ("Mouse X")) * movementSpeed * Time.deltaTime;
				hittingRight = false;
			}
		}
		else if(!hittingLeft || !hittingRight)
			currentPosition.x += (Input.GetAxis ("Mouse X")) * movementSpeed * Time.deltaTime;
	}
}
