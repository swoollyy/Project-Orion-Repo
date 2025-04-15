using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapObject : MonoBehaviour
{
    public Mesh closedTrap;
    public Mesh openedTrap;
    public StateController sc;
    public bool isOpen;
    public bool isClosed;
    bool hasTrappedOnce;
    public AudioSource trapAudio;
    public AudioClip trappedAudio;
    public AudioClip releasedAudio;
    // Start is called before the first frame update
    void Start()
    {
        hasTrappedOnce = false;
        trapAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sc.unhingeTrap)
        {
            isOpen = true;
            isClosed = false;
            gameObject.GetComponent<MeshFilter>().mesh = openedTrap;
            sc.unhingeTrap = false;
            GetComponent<Item>().canBeCollected = true;
        }
    }
    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Enemy" && sc.gc.canTrapScorpion && !hasTrappedOnce)
        {
            col.gameObject.GetComponent<StateController>().ChangeState(sc.trappedState);
            gameObject.GetComponent<MeshFilter>().mesh = closedTrap;
            trapAudio.clip = trappedAudio;
            trapAudio.Play();
            isClosed = true;
            GetComponent<Item>().canBeCollected = false;
            isOpen = false;
            hasTrappedOnce = true;
        }
        if(col.gameObject.tag == "Lizard" && !hasTrappedOnce)
        {
            col.gameObject.GetComponent<LizardStateController>().isTrapped = true;
            gameObject.GetComponent<MeshFilter>().mesh = closedTrap;
            trapAudio.clip = trappedAudio;
            trapAudio.Play();
            isClosed = true;
            GetComponent<Item>().canBeCollected = false;
            isOpen = false;
            hasTrappedOnce = true;
        }
    }
    void OnTriggerExit(Collider col)
    {

    }
}
