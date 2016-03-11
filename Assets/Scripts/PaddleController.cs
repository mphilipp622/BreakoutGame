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
	Animator anim;

	LineRenderer outline;

	void Start () {
		anim = GetComponent<Animator>();
		sprite = GetComponent<SpriteRenderer> ();
		spriteWidth = sprite.sprite.bounds.size.x * transform.localScale.x;
		currentPosition = transform.position;
		horizontalCam = Camera.main.orthographicSize * Screen.width / Screen.height;
		//outline = GetComponent<LineRenderer>();
		//outline.SetWidth(GetComponent<SpriteRenderer>().bounds.extents.y * 2, GetComponent<SpriteRenderer>().bounds.extents.y * 2);
		//outline.SetPosition(0, new Vector3(GetComponent<SpriteRenderer>().bounds.min.x, transform.position.y, -1.0f));
		//outline.SetPosition(1, new Vector3(GetComponent<SpriteRenderer>().bounds.max.x, transform.position.y, -1.0f));
	}

	void Update () {
		if (Input.GetAxis ("Mouse X") != 0)
			MovePaddle ();

		if(Master.instance.isStretched)
			spriteWidth = sprite.sprite.bounds.size.x * transform.localScale.x;
		else
			spriteWidth = sprite.sprite.bounds.size.x * transform.localScale.x;

		transform.position = currentPosition;

		if (transform.position.x - (spriteWidth / 2) <= -horizontalCam)
			hittingLeft = true;
		else if (transform.position.x + (spriteWidth / 2) >= horizontalCam)
			hittingRight = true;

		//outline.SetPosition(0, new Vector3(GetComponent<SpriteRenderer>().bounds.min.x, transform.position.y, -1.0f));
		//outline.SetPosition(1, new Vector3(GetComponent<SpriteRenderer>().bounds.max.x, transform.position.y, -1.0f));

	}

	void MovePaddle(){
		if (hittingLeft) 
		{
			currentPosition.x = -horizontalCam + spriteWidth/2;
			if (Input.GetAxis ("Mouse X") > 0) 
			{
				currentPosition.x += (Input.GetAxis ("Mouse X")) * movementSpeed * Time.deltaTime;
				hittingLeft = false;
			}
		} else if (hittingRight) 
		{
			currentPosition.x = horizontalCam - spriteWidth / 2;
			if (Input.GetAxis ("Mouse X") < 0) 
			{
				currentPosition.x += (Input.GetAxis ("Mouse X")) * movementSpeed * Time.deltaTime;
				hittingRight = false;
			}
		}
		else if(!hittingLeft || !hittingRight)
			currentPosition.x += (Input.GetAxis ("Mouse X")) * movementSpeed * Time.deltaTime;
	}

	public void SetHittingLeft()
	{
		hittingLeft = true;
	}

	public void SetHittingRight()
	{
		hittingRight = true;
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if(coll.gameObject.tag == "Energy")
			anim.SetTrigger("GrabbedEnergy");
	}
}
