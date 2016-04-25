using UnityEngine;
using System.Collections;

public class MusicSingleton : MonoBehaviour {

	public static MusicSingleton instance;
	AudioSource music;
	bool movePitchUp = false;
	float originalValue = 1;

	void Awake() {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			instance = this;
		}
		DontDestroyOnLoad(this.gameObject);

		music = GetComponent<AudioSource>(); //This needs to be in awake in order for other objects in game to read the AudioSource properly
	}

	void Start()
	{
	}

	void Update()
	{
		if(movePitchUp)
		{
			music.pitch += 0.01f * (Time.deltaTime * 2);
			if(music.pitch >= originalValue + .05f)
			{
				music.pitch = originalValue + .05f;
				movePitchUp = false;
			}
		}

		if(Master.instance.gameOver)
		{
			music.pitch -= 0.005f;
			if(music.pitch <= 0)
			{
				//music.Stop();
				music.pitch = 0f;
			}
		}
	}

	public void SetPitch()
	{
		originalValue = music.pitch;
		movePitchUp = true;
	}

	public void ResetPitch() //Used for resetting our music whenever we restart the game.
	{
		movePitchUp = false;
		music.Stop();
		music.pitch = 1f;
		music.Play();
	}
}
