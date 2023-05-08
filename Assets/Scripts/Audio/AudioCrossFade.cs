using UnityEngine;
using System.Collections;

public class AudioCrossFade : MonoBehaviour
{
    [SerializeField] AudioSource[] audioSources;
    [SerializeField] float transitionSpeed;
    int i = 0;
    AudioSource currentSource;
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
            currentSource.Play();
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

    IEnumerator SwitchSourceTo(AudioSource newSource)
    {
        float t = 0;
        yield return null;
        newSource.Play();

        while (true)
        {
            currentSource.volume = Mathf.Lerp(1, 0, t);
            newSource.volume = Mathf.Lerp(0, 1, t);

            if (t >= 1)
                break;

            t += Time.deltaTime * transitionSpeed;
            yield return null;
        }
        currentSource.Stop();
        currentSource = newSource;
    }

}
