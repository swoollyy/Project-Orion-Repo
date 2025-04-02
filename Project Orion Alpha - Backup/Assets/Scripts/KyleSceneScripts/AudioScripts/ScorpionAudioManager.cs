using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorpionAudioManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource walkAudioSource; // Audio source for walking sound
    public AudioSource burrowAudioSource; // Audio source for burrowing sound
    public AudioSource attackAudioSource; // Audio source for attack sound
    public AudioSource damageAudioSource; // Audio source for taking damage sound
    public AudioSource diggingAudioSource;

    public float walkVolume = 0.4f; // Volume level for walking sound
    public float burrowVolume = 0.09f; // Volume level for burrowing sound
    public float attackVolume = .5f; // Volume level for attack sound
    public float damageVolume = 1.0f; // Volume level for damage sound
    public float diggingVolume = 0.4f; // Volume level for damage sound

    public float fadeOutSpeed = 1.0f; // Speed at which audio fades out
    public float movementThreshold = 0.1f; // Minimum movement threshold to trigger walking sound

    private Rigidbody rb; // Reference to the Rigidbody component
    private bool isMoving; // Flag to check if the scorpion is moving
    private float targetWalkVolume; // Target volume for walking sound
    public StateController sc;

    void Start()
    {
        // Validate that all audio sources are assigned correctly
        ValidateAudioSources();

        // Get the Rigidbody component attached to the scorpion
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.Log("Rigidbody not found on the object!");
            return;
        }

        // Initialize the audio source settings
        InitializeAudioSources();
    }

    void Update()
    {
        // Handle the walking sound based on scorpion movement
        HandleWalkingSound();
    }

    private void ValidateAudioSources()
    {
        // Check if all necessary audio sources are assigned, and log errors if not
        if (walkAudioSource == null)
            Debug.LogError("Walk AudioSource not assigned!");
        if (burrowAudioSource == null)
            Debug.LogError("Burrow AudioSource not assigned!");
        if (attackAudioSource == null)
            Debug.LogError("Attack AudioSource not assigned!");
        if (damageAudioSource == null)
            Debug.LogError("Damage AudioSource not assigned!");
    }

    private void InitializeAudioSources()
    {
        // Set initial volume and looping properties for walking audio
        walkAudioSource.volume = 0f;
        walkAudioSource.loop = true;

        // Set volume levels for other audio sources
        burrowAudioSource.volume = burrowVolume;
        attackAudioSource.volume = attackVolume;
        damageAudioSource.volume = damageVolume;
        diggingAudioSource.volume = diggingVolume;
    }

    private void HandleWalkingSound()
    {
        // Check if the scorpion is moving and start or stop the walking sound accordingly
        if (IsScorpionMoving())
        {
            if (!isMoving)
            {
                StartWalking();
            }
        }
        else
        {
            if (isMoving)
            {
                StopWalking();
            }
        }

        // Smoothly adjust the volume of the walking audio towards the target volume
        walkAudioSource.volume = Mathf.Lerp(walkAudioSource.volume, targetWalkVolume, fadeOutSpeed * Time.deltaTime);
    }

    private bool IsScorpionMoving()
    {
        // Determine if the scorpion is moving based on Rigidbody velocity
        return sc.currentState == sc.roamState || sc.currentState == sc.chaseState || sc.currentState == sc.chaseLizardState;
    }

    private void StartWalking()
    {
        // Start playing the walking sound
        isMoving = true;
        targetWalkVolume = walkVolume;

        if (!walkAudioSource.isPlaying)
        {
            walkAudioSource.Play();
        }
    }

    private void StopWalking()
    {
        // Start fading out the walking sound
        isMoving = false;
        targetWalkVolume = 0f;
        Invoke(nameof(StopWalkingAudio), 1f / fadeOutSpeed); // Invoke method to stop audio after fade-out
    }

    private void StopWalkingAudio()
    {
        // Stop the walking audio if it has faded out completely
        if (!isMoving && walkAudioSource.volume <= 0.01f)
        {
            walkAudioSource.Stop();
        }
    }

    public void PlayStartBurrowSound()
    {
        // Play the burrowing sound
        if (burrowAudioSource != null)
        {
            if(!burrowAudioSource.isPlaying)
            burrowAudioSource.Play();
        }
    }
    public void StopBurrowSound()
    {
        if(burrowAudioSource.isPlaying)
            burrowAudioSource.Stop();
    }
    public void PlayDiggingSound()
    {
        // Play the burrowing sound
        if (diggingAudioSource != null)
        {
            diggingAudioSource.Play();
        }
    }
    public void StopDiggingSound()
    {
            diggingAudioSource.Stop();
    }

    public void PlayAttackSound()
    {
        // Play the attack sound
        if (attackAudioSource != null)
        {
            attackAudioSource.Play();
        }
    }


    public void PlayDamageSound()
    {
        // Play the damage sound
        if (damageAudioSource != null)
        {
            damageAudioSource.Play();
        }
    }
}
