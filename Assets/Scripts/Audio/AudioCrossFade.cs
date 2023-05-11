using UnityEngine;
using System.Collections;

public class AudioCrossFade : MonoBehaviour
{
    [SerializeField] AudioSourceInfo[] audioSources;
    [SerializeField] float transitionSpeed;
    int i = 0;
    AudioSourceInfo currentSource;
    public static AudioCrossFade Instance;

    void Awake()
    {
        if (audioSources.Length == 0)
            return;

        if (Instance != null)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);

            currentSource = audioSources[0];
            currentSource.src.volume = currentSource.maxVolume;
            currentSource.src.Play();
        }
    }

    public void SetExactSourse(int index)
    {
        if (i == index)
            return;

        i = index;
        StartCoroutine(SwitchSourceTo(audioSources[index]));
    }

    [ContextMenu("Next")]
    public void Next()
    {
        if (i >= audioSources.Length - 1)
            return;
        i++;
        StartCoroutine(SwitchSourceTo(audioSources[i]));
    }

    IEnumerator SwitchSourceTo(AudioSourceInfo newSource)
    {
        float t = 0;
        yield return null;
        newSource.src.Play();

        while (true)
        {
            currentSource.src.volume = Mathf.Lerp(currentSource.maxVolume, 0, t);
            newSource.src.volume = Mathf.Lerp(0, newSource.maxVolume, t);

            if (t >= 1)
                break;

            t += Time.deltaTime * transitionSpeed;
            yield return null;
        }
        currentSource.src.Stop();
        currentSource = newSource;
    }

    [System.Serializable]
    class AudioSourceInfo
    {
        public AudioSource src;
        [Range(0,1f)] public float maxVolume;
    }
}
