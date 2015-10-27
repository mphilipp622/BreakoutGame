using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnBricks : MonoBehaviour {

	[SerializeField]
	private Transform[] bricks;
	private List<Transform> spawnedBricks = new List<Transform>();
	private Rect[] spawnGrid;
	private Rect spawnArea;
	public Texture2D texture;
	private GUIStyle style;
	private Vector2 randomSpawn, organizedSpawn;
	public int numberOfBricks = 20;
	private Rect lastRect;
	public float rowSpace, columnSpace;
	private SpriteRenderer brickRender;

	void Start () {
		brickRender = bricks [0].GetComponent<SpriteRenderer> ();
		spawnGrid = new Rect[numberOfBricks];
		spawnArea = new Rect (Camera.main.pixelRect.position, new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight/2));

		//texture.wrapMode = TextureWrapMode.Repeat;
		//texture.Apply ();
		//style = new GUIStyle ();
		//style.normal.background = texture;
		Rect spawnRect = new Rect(new Vector2((Camera.main.orthographicSize * Screen.width/Screen.height)*-1 + bricks[0].GetComponent<SpriteRenderer>().bounds.extents.x, 
		                                      Camera.main.orthographicSize - bricks[0].GetComponent<SpriteRenderer>().bounds.extents.y),
		                          new Vector2(bricks[0].GetComponent<SpriteRenderer>().bounds.size.x,
		                                      bricks[0].GetComponent<SpriteRenderer>().bounds.size.y));
		lastRect = spawnRect;
		lastRect.position = new Vector2 (spawnRect.position.x - brickRender.bounds.size.x, spawnRect.position.y - rowSpace);
		for (int b = 0; b < spawnGrid.Length; b++) {
			if(lastRect.position.x + brickRender.bounds.size.x > Camera.main.ScreenToWorldPoint(Camera.main.pixelRect.max).x){
				spawnGrid[b] = new Rect(new Vector2(spawnRect.position.x + columnSpace, lastRect.position.y - brickRender.bounds.size.y - rowSpace), 
				                        new Vector2(brickRender.bounds.size.x, brickRender.bounds.size.y));
				Debug.Log("Max Reached. Make New Row");
			}
			else if(lastRect.position.x + brickRender.bounds.size.x < Camera.main.ScreenToWorldPoint(Camera.main.pixelRect.max).x){
				spawnGrid[b] = new Rect(new Vector2(lastRect.position.x + brickRender.bounds.size.x + columnSpace, lastRect.position.y), 
			                        new Vector2(brickRender.bounds.size.x, brickRender.bounds.size.y));
				Debug.Log ("Not Hitting Max");
			}
			lastRect = spawnGrid[b];
		}
		//Debug.Log (spawnRect.position);
		//Debug.Log (spawnRect.max);
		for(int i = 0; i < numberOfBricks; i++){
			randomSpawn = new Vector2(Random.Range(Camera.main.orthographicSize * Screen.width/ Screen.height, 
			                                       (Camera.main.orthographicSize * Screen.width/Screen.height) * -1),
			                          Random.Range (0, Camera.main.orthographicSize));
			organizedSpawn = new Vector2(spawnRect.x, spawnRect.y);
			//Transform newBrick = Instantiate(bricks[Random.Range (0, bricks.Length)], randomSpawn, Quaternion.identity) as Transform;
			Transform newBrick = Instantiate(bricks[Random.Range (0, bricks.Length)], spawnGrid[i].position, Quaternion.identity) as Transform;

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
			}*/
		}
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

	void Update () {
	
	}

	void OnGUI(){
		//for (int c = 0; c < spawnGrid.Length; c++) {
		//	GUI.Box (spawnGrid [c], "  ");
		//}
	}
}
