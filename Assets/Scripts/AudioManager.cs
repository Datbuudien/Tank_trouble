using UnityEngine;
public class AudioManager
{
    public static void Play2DOneShot(AudioClip clip, float volume)
    {
        if(clip == null)
        {
            Debug.LogError("Clip is null");
            return;
        }
        GameObject go = new GameObject("OneShot2DAudio");
        AudioSource src = go.AddComponent<AudioSource>();
        src.playOnAwake = false; 
        src.spatialBlend =0f; 
        src.clip = clip;
        src.volume = volume;
        src.spatialBlend = 0f;
        src.Play();
        Object.Destroy(go, clip.length);
    }
    
}
