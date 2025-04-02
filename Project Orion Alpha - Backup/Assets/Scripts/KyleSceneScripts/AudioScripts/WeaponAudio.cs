using UnityEngine;

public class WeaponFireAudio : MonoBehaviour
{
    public AudioSource audioSource; // Audio source to play the firing sound
    public float holdThreshold = 2.1f; // Time in seconds to hold before playing the full clip

    private bool isFiring;
    private float holdTime;
    private bool playFullClip;

    void Start()
    {
        // Initialize the audio source
        audioSource.loop = false; // Ensure the clip does not loop
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) // Left mouse button held down
        {
            holdTime += Time.deltaTime;
            if (holdTime >= holdThreshold)
            {
                playFullClip = true;
            }

            if (!isFiring)
            {
                StartFiring();
            }
        }
        else if (Input.GetMouseButtonUp(0) && isFiring) // Left mouse button released while firing
        {
            StopFiring();
        }
    }

    private void StartFiring()
    {
        isFiring = true;
        audioSource.Play();
    }

    private void StopFiring()
    {
        isFiring = false;
        if (!playFullClip && audioSource.isPlaying)
        {
            audioSource.Stop(); // Stop the audio if it is still playing and hold threshold not met
        }
        else if (playFullClip)
        {
            // Let the audio play out fully
            playFullClip = false;
        }
        holdTime = 0f; // Reset hold time
    }
}
