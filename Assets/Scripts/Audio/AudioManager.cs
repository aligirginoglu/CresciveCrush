using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; set; }

    [Header("Audio")]
    [SerializeField] private AudioSource collectStart;
    [SerializeField] private AudioSource collectEnd;
    [SerializeField] private AudioSource ballDrop;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public AudioSource CollectStartSound()
    {
        return collectStart;
    }
    public AudioSource CollectEndSound()
    {
        return collectEnd;
    }
    public AudioSource BallDropSound()
    {
        return ballDrop;
    }
}
