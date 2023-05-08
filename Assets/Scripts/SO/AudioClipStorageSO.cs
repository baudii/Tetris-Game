using UnityEngine;

[CreateAssetMenu(fileName = "Clip data", menuName = "SO")]
public class AudioClipStorageSO : ScriptableObject
{
    [SerializeField] public AudioClipData[] clips;
}

[System.Serializable]
public class AudioClipData
{
    public AudioClip clip;
    public string name;
}