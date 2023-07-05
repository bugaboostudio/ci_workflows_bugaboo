using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0.1f, 3f)]
    public float pitch = 1f;
}

public class SoundManager : MonoBehaviour
{
    public Sound[] sounds;
    public AudioSource audioSource;

    private void Start()
    {
        // Adicionar um componente AudioSource ao GameObject do Sound Manager
        //audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlaySound(string name)
    {
        // Procurar o som pelo nome e reproduzir no AudioSource
        Sound sound = System.Array.Find(sounds, s => s.name == name);
        if (sound != null)
        {
            audioSource.clip = sound.clip;
            audioSource.volume = sound.volume;
            //audioSource.pitch = sound.pitch;
            audioSource.PlayOneShot(audioSource.clip);
        }
        else
        {
            Debug.LogWarning("Sound with name " + name + " not found!");
        }
    }
}
