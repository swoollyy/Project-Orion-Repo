using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener2 : MonoBehaviour
{
    public GameObject door;
    public GameObject doorOpenReference;
    public GameObject doorCloseReference;
    Vector3 openReference;
    Vector3 closeReference;
    bool closeDoor;
    public TutorialController tutorialStateMachine;
    public AudioSource doorAudio;
    int playOnce;

    public ParticleSystem doorPart;


    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (closeDoor)
            CloseDoor();
    }
    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player" && (tutorialStateMachine.currentState == tutorialStateMachine.thirdRoomState || tutorialStateMachine.currentState == tutorialStateMachine.fieldState))
        {
            closeDoor = false;
            OpenDoor();
        }
    }
    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player" && (tutorialStateMachine.currentState == tutorialStateMachine.thirdRoomState || tutorialStateMachine.currentState == tutorialStateMachine.fieldState))
        {
            closeDoor = true;
        }
    }
    void OpenDoor()
    {
        playOnce++;
        if (playOnce == 1)
        {
            doorPart.Play();
            doorAudio.Play();
        }
        door.transform.position = Vector3.SmoothDamp(door.transform.position, doorOpenReference.transform.position, ref openReference, 50 * Time.deltaTime);


    }
    void CloseDoor()
    {
        playOnce = 0;
        door.transform.position = Vector3.SmoothDamp(door.transform.position, doorCloseReference.transform.position, ref openReference, 50 * Time.deltaTime);
    }
}
