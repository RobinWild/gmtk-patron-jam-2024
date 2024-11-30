using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySFX : MonoBehaviour {

	public AudioClip audio;
	public float volume = 1;

	void Start () {
		SoundManager.PlaySFX(audio, volume);
	}
}
