﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Master : MonoBehaviour {

	[SerializeField]
	private Transform[] bricks;
	public List<Transform> spawnedBricks = new List<Transform>();
	private Rect[] spawnGrid;
//	private Rect spawnArea;
	public Texture2D texture;
	private GUIStyle style = new GUIStyle();
	private Vector2 randomSpawn, organizedSpawn;
	public int numberOfBricks;
	private Rect lastRect;
	public float rowSpace, columnSpace;
	private SpriteRenderer brickRender;

	[SerializeField]
	float spawnTime;
	float nextSpawn;

	public static Master instance = null; //Singleton
	public bool gameOver = false;
	public Transform paddle;

    public int energy;
    public List<Renderer> brickRenderers = new List<Renderer>(); //used for color outlines
    public List<int> brickInt = new List<int>(); //also used for color outlines
    //public List<BrickData> brickData = new List<BrickData>();
    public bool newSpawn = false;
    int index = 0;

	public Material[] outlineMats;

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
	}

	void Start () {
		paddle = GameObject.Find("Paddle").transform;
		spawnedBricks = new List<Transform>();
		brickRender = bricks [0].GetComponent<SpriteRenderer> ();
		spawnGrid = new Rect[numberOfBricks];
        energy = 100;
		texture = new Texture2D((int) bricks[0].GetComponent<SpriteRenderer>().sprite.textureRect.width, (int) bricks[0].GetComponent<SpriteRenderer>().sprite.textureRect.height);
		//texture = new Texture2D((int)bricks[0].GetComponent<SpriteRenderer>().sprite.textureRect.width, (int)bricks[0].GetComponent<SpriteRenderer>().sprite.textureRect.height);
		//spawnArea = new Rect (Camera.main.pixelRect.position, new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight/2));

		SpawnNewBricks();

		//ParentBricks();
	}
	
	void Update()
	{
		if(Time.timeSinceLevelLoad >= nextSpawn)
		{
			MoveBricks();
			SpawnNewBricks();
		}

		if(gameOver)
			GameOver();


	}
	void OnGUI()
	{
		
		/*for(int i = 0; i < brickRenderers.Count; i++)
		{
			switch(spawnedBricks[i].GetComponent<BrickScript>().GetHP())
			{
			case 1:
				for ( int a = 0; a < bricks[0].GetComponent<SpriteRenderer>().bounds.size.x; a++)
				{
					for ( int b = 0; b < bricks[0].GetComponent<SpriteRenderer>().bounds.size.y; b++)
					{
						Debug.Log((int) spawnedBricks[i].GetComponent<SpriteRenderer>().bounds.min.x + a);
						Debug.Log((int) spawnedBricks[i].GetComponent<SpriteRenderer>().bounds.min.y + b);
						texture.SetPixel((int) spawnedBricks[i].GetComponent<SpriteRenderer>().bounds.min.x + a, 
										 (int) spawnedBricks[i].GetComponent<SpriteRenderer>().bounds.min.y + b, Color.green);
					}
				}
				texture.Apply();
				style.normal.background = texture;
				GUI.Box(spawnGrid[i], texture, style);
				break;
			case 2:
				for ( int a = 0; a < bricks[0].GetComponent<SpriteRenderer>().bounds.size.x; a++)
				{
					for ( int b = 0; b < bricks[0].GetComponent<SpriteRenderer>().bounds.size.y; b++)
					{
						texture.SetPixel(a, b, Color.yellow);
					}
				}
				texture.Apply();
				style.normal.background = texture;
				GUI.Box(spawnGrid[i], texture, style);
				break;
			case 3:
				for ( int a = 0; a < bricks[0].GetComponent<SpriteRenderer>().bounds.size.x; a++)
				{
					for ( int b = 0; b < bricks[0].GetComponent<SpriteRenderer>().bounds.size.y; b++)
					{
						texture.SetPixel(a, b, Color.red);
					}
				}
				texture.Apply();
				style.normal.background = texture;
				GUI.Box(spawnGrid[i], texture, style);
				break;
			}

		}*/
		//texture.SetPixel(512, 512, Color.green);

			
		//texture.Apply();
	//	style.normal.background = texture;
		//GUI.Box(new Rect(0, 0, 512, 512), texture, style);
		/*for(int i = 0; i < spawnedBricks.Count; i++)
		{
			//GUI.Box(spawnGrid[i], texture);
			DrawQuad(new Rect(spawnedBricks[i].position.x, spawnedBricks[i].position.y, brickRenderers[i].bounds.size.x, brickRenderers[i].bounds.size.y), Color.green);
		}
*/
		//for (int c = 0; c < spawnGrid.Length; c++) {
		//	GUI.Box (spawnGrid [c], texture, style);
			//DrawQuad(spawnGrid[c], Color.green);
		//}
	}

	//Move bricks down by a row
	void MoveBricks()
	{
		for(int i = 0; i < spawnedBricks.Count; i++)
		{
			spawnedBricks[i].position = new Vector2(spawnedBricks[i].position.x, spawnedBricks[i].position.y - brickRender.bounds.size.y);
			
		}

	}

	void SpawnNewBricks()
	{
		Rect spawnRect = new Rect(new Vector2((Camera.main.orthographicSize * Screen.width/Screen.height)*-1 + bricks[0].GetComponent<SpriteRenderer>().bounds.extents.x, 
			Camera.main.orthographicSize - bricks[0].GetComponent<SpriteRenderer>().bounds.extents.y),
			new Vector2(bricks[0].GetComponent<SpriteRenderer>().bounds.size.x,
				bricks[0].GetComponent<SpriteRenderer>().bounds.size.y));

		lastRect.position = new Vector2 (spawnRect.position.x - brickRender.bounds.size.x, spawnRect.position.y - rowSpace);

		for (int b = 0; b < spawnGrid.Length; b++) 
		{
			if(lastRect.position.x + brickRender.bounds.size.x > Camera.main.ScreenToWorldPoint(Camera.main.pixelRect.max).x)
			{
				spawnGrid[b] = new Rect(new Vector2(spawnRect.position.x + columnSpace, lastRect.position.y - brickRender.bounds.size.y - rowSpace), 
					new Vector2(brickRender.bounds.size.x, brickRender.bounds.size.y));
				//Debug.Log("Max Reached. Make New Row");
			}
			else if(lastRect.position.x + brickRender.bounds.size.x < Camera.main.ScreenToWorldPoint(Camera.main.pixelRect.max).x)
			{
				spawnGrid[b] = new Rect(new Vector2(lastRect.position.x + brickRender.bounds.size.x + columnSpace, lastRect.position.y), 
					new Vector2(brickRender.bounds.size.x, brickRender.bounds.size.y));
				//Debug.Log ("Not Hitting Max");
			}
			lastRect = spawnGrid[b];
		}
		//Debug.Log (spawnRect.position);
		//Debug.Log (spawnRect.max);

		//Instanttiate New Bricks
		for(int i = 0; i < numberOfBricks; i++)
		{
            //int outlineColor = Random.Range(1, 3);
            //int hp = Random.Range(1, 4);
			//Transform newBrick = Instantiate(bricks[Random.Range (0, bricks.Length)], randomSpawn, Quaternion.identity) as Transform;
            
			spawnedBricks.Add(Instantiate(bricks[Random.Range (0, bricks.Length)], spawnGrid[i].position, Quaternion.identity) as Transform);
			spawnedBricks[i].gameObject.layer = LayerMask.NameToLayer("Default");
          //  spawnedBricks[index].GetComponent<BrickScript>().SetIndex(index);
           // brickData.Add(new BrickData(outlineColor, Instantiate(bricks[Random.Range(0, bricks.Length)], spawnGrid[i].position, Quaternion.identity) as Transform));
          //  brickRenderers.Add(spawnedBricks[index].GetComponent<Renderer>());
           // spawnedBricks[index].GetComponent<BrickScript>().SetHP(hp);
           // brickInt.Add(spawnedBricks[index].GetComponent<BrickScript>().GetHP());


            index = spawnedBricks.Count;
			/*if(spawnedBricks[i].position.x < Camera.main.pixelRect.xMax){
				//Debug.Log("True");
				spawnRect = new Rect (new Vector2(newBrick.GetComponent<SpriteRenderer>().bounds.max.x + newBrick.GetComponent<SpriteRenderer>().bounds.extents.x,
			                                 	  newBrick.GetComponent<SpriteRenderer>().bounds.max.y - newBrick.GetComponent<SpriteRenderer>().bounds.extents.y),
			                     	 			  newBrick.GetComponent<SpriteRenderer>().bounds.size);
			}*/
			/*else if(spawnedBricks[i].position.x > Camera.main.pixelRect.xMax){
			Debug.Log("Not True");
			spawnRect = new Rect (new Vector2((Camera.main.orthographicSize * Screen.width/Screen.height)*-1 + bricks[0].GetComponent<SpriteRenderer>().bounds.extents.x, 
				Camera.main.orthographicSize - (bricks[0].GetComponent<SpriteRenderer>().bounds.extents.y * 2)),
				new Vector2(bricks[0].GetComponent<SpriteRenderer>().bounds.size.x,
					bricks[0].GetComponent<SpriteRenderer>().bounds.size.y));
		}*/
		}
		
		nextSpawn = Time.timeSinceLevelLoad + spawnTime;
        newSpawn = true;
		
		//Parent all the new bricks to the Master Object
		//for (int i = 0; i < spawnedBricks.Count; i++)
		//{
		//	if(spawnedBricks[i].parent == null)
		//		spawnedBricks[i].parent = transform;
		//}

		/*for(int i = 0; i < numberOfBricks; i++)
		{
			randomSpawn = new Vector2(Random.Range(Camera.main.orthographicSize * Screen.width/ Screen.height, 
				(Camera.main.orthographicSize * Screen.width/Screen.height) * -1),
				Random.Range (0, Camera.main.orthographicSize));
			organizedSpawn = new Vector2(spawnRect.x, spawnRect.y);
			//Transform newBrick = Instantiate(bricks[Random.Range (0, bricks.Length)], randomSpawn, Quaternion.identity) as Transform;
			Transform newBrick = Instantiate(bricks[Random.Range (0, bricks.Length)], spawnGrid[i].position, Quaternion.identity) as Transform;
			newBrick.SetParent(gameObject.transform);

			spawnedBricks.Add(newBrick);
			/*if(spawnedBricks[i].position.x < Camera.main.pixelRect.xMax){
					//Debug.Log("True");
					spawnRect = new Rect (new Vector2(newBrick.GetComponent<SpriteRenderer>().bounds.max.x + newBrick.GetComponent<SpriteRenderer>().bounds.extents.x,
				                                 	  newBrick.GetComponent<SpriteRenderer>().bounds.max.y - newBrick.GetComponent<SpriteRenderer>().bounds.extents.y),
				                     	 			  newBrick.GetComponent<SpriteRenderer>().bounds.size);
				}*/
			/*else if(spawnedBricks[i].position.x > Camera.main.pixelRect.xMax){
			Debug.Log("Not True");
			spawnRect = new Rect (new Vector2((Camera.main.orthographicSize * Screen.width/Screen.height)*-1 + bricks[0].GetComponent<SpriteRenderer>().bounds.extents.x, 
				Camera.main.orthographicSize - (bricks[0].GetComponent<SpriteRenderer>().bounds.extents.y * 2)),
				new Vector2(bricks[0].GetComponent<SpriteRenderer>().bounds.size.x,
					bricks[0].GetComponent<SpriteRenderer>().bounds.size.y));
		}
		}*/
		//texture.wrapMode = TextureWrapMode.Repeat;
		//texture.Apply ();
		//style = new GUIStyle ();
		//style.normal.background = texture;

		/*for (int i = 0; i < spawnedBricks.Count; i++){
					SpriteRenderer currentRenderer = spawnedBricks[i].GetComponent<SpriteRenderer>();
					for(int a = 0; a < spawnedBricks.Count; a++){
						if(spawnedBricks[i] == spawnedBricks[a]){
							continue;
						}
						SpriteRenderer subRenderer = spawnedBricks[a].GetComponent<SpriteRenderer>();
						if(currentRenderer.bounds.Intersects(subRenderer.bounds)){
							Debug.Log("spawnedBricks " + i + " Intersects " + "spawnedBricks " + a);
							spawnedBricks[i].position = new Vector2(Random.Range(Camera.main.orthographicSize * Screen.width/ Screen.height, 
							                                                     (Camera.main.orthographicSize * Screen.width/Screen.height) * -1),
							                                        Random.Range (Camera.main.orthographicSize/2, Camera.main.orthographicSize));
							a = 0;
						}
					}
				}*/
	}

    //Remove Bricks from List when they're destroyed
    public void RemoveBrick(Transform brickToDestroy)
	{
       // int index = spawnedBricks.FindIndex(brickToDestroy);
		spawnedBricks.Remove(brickToDestroy);
       // brickRenderers.Remove(brickToDestroy.GetComponent<Renderer>());
       // brickRenderers.Remove(brickToDestroyRenderer);
       // brickInt.RemoveAt(brickIndex);
	}

   /* public void RemoveBrick(int brickIndex)
    {
        // int index = spawnedBricks.FindIndex(brickToDestroy);
        spawnedBricks.RemoveAt(brickIndex);
        brickRenderers.RemoveAt(brickIndex);
        // brickRenderers.Remove(brickToDestroyRenderer);
        brickInt.RemoveAt(brickIndex);
    }*/

    void GameOver()
	{
		Time.timeScale = 0;
	}

    public void GetRenderer()
    {
        foreach (Transform brick in spawnedBricks)
        {
            brickRenderers.Add(brick.GetComponent<Renderer>());
        }
    }
}
