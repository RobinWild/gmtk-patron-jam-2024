using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	// Static instance
	static SoundManager _instance;
	public static SoundManager GetInstance(){
		if (!_instance){
			GameObject soundManager = new GameObject("SoundManager");
			_instance = soundManager.AddComponent<SoundManager>();
			_instance.Initialize();
		}
		return _instance;
	}

	const float MaxVolume_BGM = 1f;
	const float MaxVolume_SFX = 1f;
	static float CurrentVolumeNormalized_BGM = 1f;
	static float CurrentVolumeNormalized_SFX = 1f;
	static bool isMuted = false;

	List<AudioSource> sfxSources;
	AudioSource bgmSource;

	void Initialize(){
		// Add our bgm sound source
		bgmSource = gameObject.AddComponent<AudioSource>();
		bgmSource.loop = true;
		bgmSource.playOnAwake = false;
		bgmSource.volume = GetBGMVolume();
		DontDestroyOnLoad(gameObject);

		
	}

	// ========= Volume Getters =========

	static float GetBGMVolume()
	{
		CurrentVolumeNormalized_BGM = PlayerPrefsManager.GetMusicVolume();
		if (CurrentVolumeNormalized_BGM <= 0f)
		{
			// Default to a non-zero volume if not set
			CurrentVolumeNormalized_BGM = 1f;
		}
		return isMuted ? 0f : MaxVolume_BGM * CurrentVolumeNormalized_BGM;
	}

	public static float GetSFXVolume()
	{
		CurrentVolumeNormalized_SFX = PlayerPrefsManager.GetMasterVolume();
		if (CurrentVolumeNormalized_SFX <= 0f)
		{
			// Default to a non-zero volume if not set
			CurrentVolumeNormalized_SFX = 1f;
		}
		return isMuted ? 0f : MaxVolume_SFX * CurrentVolumeNormalized_SFX;
	}

	// ========= BGM Utils =========
	void FadeBGMOut (float fadeDuration){
		SoundManager soundMan = GetInstance();
		float delay = 0f;
		float toVolume = 0f;

		if (soundMan.bgmSource.clip == null){
			Debug.LogError("Error: Could not fade BGM out as BGM AudioSource has no currently playing clip.");
		}
	}

	void FadeBGMIn (AudioClip bgmClip, float delay, float fadeDuration){
		SoundManager soundMan = GetInstance();
		soundMan.bgmSource.clip = bgmClip;
		soundMan.bgmSource.Play();

		float toVolume = GetBGMVolume();

		StartCoroutine(FadeBGM(toVolume, delay, fadeDuration));
	}

	IEnumerator FadeBGM (float fadeToVolume, float delay, float duration){
		yield return new WaitForSeconds (delay);

		SoundManager soundMan = GetInstance();
		float elapsed = 0f;
		while (duration > 0) {
			float t = (elapsed / duration);
			float volume = Mathf.Lerp(0f, fadeToVolume*CurrentVolumeNormalized_BGM, t);
			soundMan.bgmSource.volume = volume;

			elapsed += Time.deltaTime;
			yield return 0;
		}
	}

	// ========= BGM Functions =========
	public static void PlayBGM (AudioClip bgmClip, bool fade, float fadeDuration){
		SoundManager soundMan = GetInstance();

		if (fade) {
			if (soundMan.bgmSource.isPlaying) {
				// Fade out, then switch and fade in
				soundMan.FadeBGMOut(fadeDuration/2);
				soundMan.FadeBGMIn(bgmClip, fadeDuration / 2, fadeDuration);
			} else {
				// Just fade in
				float delay = 0f;
				soundMan.FadeBGMIn(bgmClip, delay, fadeDuration);
			}
		} else {
			// Play immediately
			soundMan.bgmSource.volume = GetBGMVolume();
			soundMan.bgmSource.clip = bgmClip;
			soundMan.bgmSource.Play();
		}
	}

	public static void StopBGM(bool fade, float fadeDuration){
		SoundManager soundMan = GetInstance();
		if (soundMan.bgmSource.isPlaying) {
			// Fade out then switch and fade im
			if (fade) {
				soundMan.FadeBGMOut(fadeDuration);
			} else {
				soundMan.bgmSource.Stop();
			}
		}
	}

	// ========= SFX Utils =========
	AudioSource GetSFXSource() {
		// Sound up a new sfx sound source for each new sfx clip
		AudioSource sfxSource = gameObject.AddComponent<AudioSource>();
		sfxSource.loop = false;
		sfxSource.playOnAwake = false;
		sfxSource.volume = GetSFXVolume();

		if (sfxSources == null){
			sfxSources = new List<AudioSource>();
		}

		sfxSources.Add(sfxSource);

		return sfxSource;
	}

	IEnumerator RemoveSFXSource(AudioSource sfxSource){
		yield return new WaitForSeconds(sfxSource.clip.length);
		sfxSources.Remove(sfxSource);
		Destroy(sfxSource);
	}

	IEnumerator RemoveSFXSourceFixedLength(AudioSource sfxSource, float length){
		yield return new WaitForSeconds(length);
		sfxSources.Remove(sfxSource);
		Destroy(sfxSource);
	}

	// ========= SFX Functions =========
	public static void PlaySFX(AudioClip sfxClip, float volume){
		SoundManager soundMan = GetInstance();
		AudioSource source = soundMan.GetSFXSource();
		source.volume = GetSFXVolume() * volume;
		source.clip = sfxClip;
		source.Play();

		soundMan.StartCoroutine(soundMan.RemoveSFXSource(source));
	}

	public static void PlaySFXRandomized(AudioClip sfxClip, float volume, float minRange, float maxRange){
		SoundManager soundMan = GetInstance();
		AudioSource source = soundMan.GetSFXSource();
		source.volume = GetSFXVolume() * volume;
		source.clip = sfxClip;
		source.pitch = Random.Range(minRange, maxRange);
		source.Play();

		soundMan.StartCoroutine(soundMan.RemoveSFXSource(source));
	}

	public static void PlaySFXPitch(AudioClip sfxClip, float volume, float semitone){
		SoundManager soundMan = GetInstance();
		AudioSource source = soundMan.GetSFXSource();
		source.volume = GetSFXVolume() * volume;
		source.clip = sfxClip;
		source.pitch = 1f * Mathf.Pow(1.05946f, semitone);
		source.Play();
		soundMan.StartCoroutine(soundMan.RemoveSFXSource(source));
	}

	public static void PlaySFXFixedDuration(AudioClip sfxClip, float volume, float duration){
		SoundManager soundMan = GetInstance();
		AudioSource source = soundMan.GetSFXSource();
		source.volume = GetSFXVolume() * volume;
		source.clip = sfxClip;
		source.loop = true;
		source.Play();

		soundMan.StartCoroutine(soundMan.RemoveSFXSourceFixedLength(source, duration));
	}

	public static void PlaySFX3D(AudioClip sfxClip, float volume, Vector3 position, float pitch = 0, float maxAudibleDistance = 10f)
    {
		if (sfxClip == null) return;
		Camera mainCamera = Camera.main;
		if (mainCamera == null)
		{
			Debug.LogError("No main camera found in the scene.");
			return;
		}
		
		float cameraZDepth = Mathf.Abs(mainCamera.transform.position.z);
		maxAudibleDistance += cameraZDepth;

        float distance = Vector3.Distance(position, Camera.main.transform.position);
		if (distance > maxAudibleDistance) return;
        float distanceVolume = Mathf.Clamp01(1f - (distance / (maxAudibleDistance)));
        SoundManager soundMan = GetInstance();
        AudioSource source = soundMan.GetSFXSource();
        source.volume = GetSFXVolume() * volume;

        source.volume *= distanceVolume;

        source.clip = sfxClip;
        source.spatialBlend = .5f;
        source.minDistance = 1f;
        source.maxDistance = maxAudibleDistance;

        // Set the position of the AudioSource to the sound source's position
        source.transform.position = position;

        // Set the pitch of the AudioSource
		source.pitch = 1f * Mathf.Pow(1.05946f, pitch);

        source.Play();

        soundMan.StartCoroutine(soundMan.RemoveSFXSource(source));
    }

	// ========== Volume Control Functions ==========
	public static void DisableSoundImmediate() {
		SoundManager soundMan = GetInstance();
		soundMan.StopAllCoroutines();
		if (soundMan.sfxSources != null){
			foreach (AudioSource source in soundMan.sfxSources){
				source.volume = 0;
			}
		}
		soundMan.bgmSource.volume = 0f;
		isMuted = true;
	}

	public static void EnableSoundImmediate(){
		SoundManager soundMan = GetInstance();
		if (soundMan.sfxSources != null){
			foreach (AudioSource source in soundMan.sfxSources){
				source.volume = GetSFXVolume();
			}
		}
		soundMan.bgmSource.volume = GetBGMVolume();
		isMuted = false;
	}

	public static void SetGlobalVolume(float newVolume){
		CurrentVolumeNormalized_BGM = newVolume;
		CurrentVolumeNormalized_SFX = newVolume;
		AdjustSoundImmediate();
	}

	public static void SetSFXVolume(float newVolume){
		CurrentVolumeNormalized_SFX = newVolume;
		AdjustSoundImmediate();
	}

	public static void SetBGMVolume(float newVolume){
		CurrentVolumeNormalized_BGM = newVolume;
		AdjustSoundImmediate();
	}

	public static void AdjustSoundImmediate(){
		SoundManager soundMan = GetInstance();
		if (soundMan.sfxSources != null){
			foreach (AudioSource source in soundMan.sfxSources){
				source.volume = GetSFXVolume();
			}
		}
		Debug.Log ("BGM Volume: " + GetBGMVolume());
		soundMan.bgmSource.volume = GetBGMVolume();
		Debug.Log ("BGM Volume is now: " + GetBGMVolume());
	}
}
