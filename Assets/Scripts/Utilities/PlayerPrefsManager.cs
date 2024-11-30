using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsManager {

    const string MASTER_VOLUME_KEY = "master_volume";
    const string MUSIC_VOLUME_KEY = "music_volume";

    public static void SetMasterVolume(float volume){
        if (volume >= 0f && volume <= 1f) {
            PlayerPrefs.SetFloat (MASTER_VOLUME_KEY, volume);
        } else {
            Debug.LogError ("Master volume out of range");
        }
    }

    public static float GetMasterVolume(){
        return PlayerPrefs.GetFloat(MASTER_VOLUME_KEY);
    }

    public static void SetMusicVolume(float volume){
        if (volume >= 0f && volume <= 1f) {
            PlayerPrefs.SetFloat (MUSIC_VOLUME_KEY, volume);
        } else {
            Debug.LogError ("Music volume out of range");
        }
    }

    public static float GetMusicVolume(){
        return PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY);
    }
}
