using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource audioSourcePrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void PlayAudioClip(AudioClip audioClip, Transform SpawnTransform, float volume = 1f)
    {
        AudioSource audioSource = Instantiate(audioSourcePrefab, SpawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayRandomAudioClip(AudioClip[] audioClip, Transform SpawnTransform, float volume = 1f)
    {
        int randomIndex = Random.Range(0, audioClip.Length);
        AudioSource audioSource = Instantiate(audioSourcePrefab, SpawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip[randomIndex];
        audioSource.volume = volume;
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }
}
