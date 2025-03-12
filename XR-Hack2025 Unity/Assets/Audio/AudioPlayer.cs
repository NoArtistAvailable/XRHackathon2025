using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] AudioClip Sfx;
            void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void playSfx()
    {
        audioSource.PlayOneShot(Sfx);
    }

    // Update is called once per frame

}
