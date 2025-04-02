using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public AudioSource audioSource; // Audio source to play the sound
    public float walkPitch = 1.0f; // Normal pitch for walking
    public float sprintPitch = 1.5f; // Higher pitch for sprinting
    public float startDelay = 2f; // Delay before starting the footsteps
    public float fadeOutSpeed = .5f; // Speed of volume fade-out when stopping

    private Rigidbody rigidbody; // Reference to Rigidbody component
    private bool isWalking;
    private float targetVolume;
    public AudioClip footStepSand;
    public AudioClip footStepMetal;
    public AudioClip chosenClip;
    public AudioClip metalLand;
    public AudioClip sandLand;
    public AudioSource sprintBreath;
    public AudioSource jumpBreath;
    public AudioSource jumpLand;
    public AudioSource poisonSFX;
    private bool isSprinting;
    public PlayerMovement mMent;
    public PlayerHealthLevel1 pHealth;
    public GameController gc;
    int onlyOnce;
    int onlyOnceJump;
    float timer;

    public float fadeOuter;
    float distCovered;

    public OpeningScene opScene;



    void Start()
    {
        // Initialize variables
        rigidbody = GetComponent<Rigidbody>();

        // Assign the clip to the audio source
        audioSource.loop = true; // Set the clip to loop
        audioSource.volume = 0f; // Start with volume at 0
        audioSource.pitch = walkPitch; // Set initial pitch for walking
        targetVolume = 0f;
    }

    void Update()
    {
        Ray ray;
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 15f))
        {
            if (hit.transform.gameObject.tag == "Room" && !opScene.startSequence)
            {
                chosenClip = footStepMetal;
                audioSource.clip = chosenClip;
                gc.tutorialMixer.TransitionTo(1f);
            }
            else if (hit.transform.gameObject.tag == "Terrain")
            {
                chosenClip = footStepSand;
                audioSource.clip = chosenClip;
            }

            if (gc.tutController.currentState == gc.tutController.fieldState)
                gc.arenaMixer.TransitionTo(1f);

        }

        if(pHealth.isPoisoned)
        {
            if(!poisonSFX.isPlaying)
                poisonSFX.Play();
        }
        else poisonSFX.Stop();

        if (!mMent.isJumping)
        {
            onlyOnceJump = 0;
        }
        if(mMent.hasJumped && mMent.grounded)
        {
            if (hit.transform.gameObject.tag == "Room")
            {
                jumpLand.Stop();
                PlayLandAudio(metalLand);
                mMent.hasJumped = false;
            }
            else if (hit.transform.gameObject.tag == "Terrain")
            {
                jumpLand.Stop();
                PlayLandAudio(sandLand);
                mMent.hasJumped = false;
            }
        }
        if (IsPlayerMoving() && mMent.grounded)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, 5 * Time.deltaTime);
            if (!isWalking)
            {
                StartWalkingWithDelay();
            }
            else StopWalking();
            // Dynamically adjust pitch based on whether the player is sprinting
            audioSource.pitch = isSprinting ? sprintPitch : walkPitch;
        }
        if(!IsPlayerMoving() || !mMent.grounded)
        {
            StopWalkingAudio();
        }

        // Smoothly adjust volume toward the target volume
    }

    private bool IsPlayerMoving()
    {
        // Check the magnitude of the rigidbody's velocity
        return Input.GetAxisRaw("Vertical") > 0 || Input.GetAxisRaw("Horizontal") > 0; // Adjust threshold as needed
    }

    private bool IsPlayerSprinting()
    {
        // Check if the sprint input is active (e.g., Shift key or gamepad button)
        return Input.GetKey(KeyCode.LeftShift); // Replace with your sprint input key
    }


    private void StartWalkingWithDelay()
    {

        // Randomize the starting position of the audio clip
        audioSource.time = Random.Range(0f, chosenClip.length);

        // Start the audio after a delay
        Invoke(nameof(StartAudio), startDelay);
        isWalking = true;
    }

    private void StartAudio()
    {
        isWalking = false;
        distCovered = targetVolume;
        if (mMent.grounded) // Ensure the player is still walking and grounded after the delay
        {
            audioSource.Play();
            if (chosenClip == footStepSand)
                targetVolume = .7f; // Fade in to full volume
            else targetVolume = 1f;
        }

    }

    private void StopWalking()
    {
        distCovered = Time.deltaTime - 5f;
        targetVolume = .15f; // Fade out to silence
        audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, distCovered / 50f);

        // Stop the audio once the volume fades out
        Invoke(nameof(StopAudio), 1f / fadeOutSpeed); // Adjust delay based on fadeOutSpeed
    }

    private void StopWalkingAudio()
    {

        targetVolume = 0f; // Fade out to silence
        audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, Time.deltaTime);

        // Stop the audio once the volume fades out
        Invoke(nameof(StopAudio), 1f / fadeOutSpeed); // Adjust delay based on fadeOutSpeed
    }


    private void StopAudio()
    {
        if (!isWalking && audioSource.volume <= 0.01f)
        {
            audioSource.Stop();
        }
    }
    private void PlayRunBreatheOnce()
    {
        onlyOnce++;
        if (onlyOnce == 1)
            sprintBreath.Play();
    }
    private void PlayLandAudio(AudioClip audio)
    {
        jumpLand.clip = audio;
        if(!jumpLand.isPlaying)
        jumpLand.Play();
    }
}
