using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using PrimeTween;

public class StateController : MonoBehaviour
{
    public IState currentState;
    public RoamState roamState = new RoamState();
    public IdleState idleState = new IdleState();
    public ChaseState chaseState = new ChaseState();
    public PatrolState patrolState = new PatrolState();
    public BurrowState burrowState = new BurrowState();
    public AttackState attackState = new AttackState();
    public TrappedState trappedState = new TrappedState();
    public BurrowAttackState burrowAttackState = new BurrowAttackState();
    public BurrowAttackLizardState burrowAttackLizardState = new BurrowAttackLizardState();
    public BurrowIdleState burrowIdleState = new BurrowIdleState();
    public EndBurrowState endBurrowState = new EndBurrowState();
    public ChaseLizardState chaseLizardState = new ChaseLizardState();
    public AttackLizardState attackLizardState = new AttackLizardState();
    public EnableWeatherState enableWeatherState = new EnableWeatherState();
    public StunnedState stunnedState = new StunnedState();
    public Weather weatherscript;
    public float idleStopIntMin;
    public float idleStopIntMax;
    public float pPointMinXRngValue;
    public float pPointMaxXRngValue;
    public float pPointMinZRngValue;
    public float pPointMaxZRngValue;
    public float minimumRange;
    public NavMeshAgent agent;
    public GameObject[] patrolPoints;
    public int patrolPosition = 0;
    float patrolPosX;
    float patrolPosZ;
    int switchChance = 0;
    public float turnChanceMin;
    public float turnChanceMax;
    int turnChance;
    public float stopInt;
    bool nextTimerStart = false;
    float nextTimer = 0f;
    public LayerMask layerMask;
    public float valueDec;
    public bool startDecline;
    public bool startRise;
    public NavMeshSurface nms;
    public NavMeshSurface nmsBurrow;
    public bool isburrowAtk;
    public bool isburrowAtkLizard;
    public bool eruptNearPlayer;
    public GameObject player;
    RaycastHit rayHit;
    GameObject currentHitObject;
    public GameObject lockedOnObject;
    float currentHitDistance;
    bool isColliding;
    public GameObject burrowTracker;
    public GameObject damageArea;
    public GameObject terrain;
    public float explodeTimer;
    bool explodeTimerStart;
    public bool isCharging;
    float chargeZone;
    public bool inBurAtkRange;
    public float maxChaseDistance;
    public Vector3 burrowLockPosition;
    public bool hasDone;
    public float posX;
    public float posZ;
    public bool hasHitPlayer;
    public float atkCD = 0f;
    int lizardCount = 0;
    int closestCheck = 1;
    public Vector3 closestDistance;
    public GameObject closestDistanceLizard;
    public List<GameObject> currentLizards = new List<GameObject>();
    public Vector3[] sc;
    public List<Vector3> lizardDistances = new List<Vector3>();
    public bool trackingLizards;
    public float debugTimer;
    public bool hasHitLizard;
    public bool removeFromLizardLists;
    public string lizardName;
    public GameObject removedLizard;
    public Vector3 removedLizardDistance;
    public LizardStateController removedLizardCtrl;
    int idleRNG;
    bool chanceToIdle;
    public bool isTrapped;
    public GameObject trappedLizard;
    public GameController gc;
    float getDistanceTimer;
    bool getDistance;
    public bool unhingeTrap;
    public int hp = 3;
    public Animator animator;
    public AnimationEvent anEvent;
    public ScorpionAudioManager scorpAudio;
    public SpawnLizard lizSpawn;
    public Suction succ;
    public TrapObject trapScript;

    public GameObject cube;
    public GameObject tailEnd;

    GameObject poisonBall;

    public AudioSource rumbleAudio;
    public AudioSource lizardRumbleAudio;


    public bool watchCases;
    public bool firstJump;
    public bool lizardInPosition;
    public float timeElapsed;
    public float duration = 2f;


    public GameObject unburrowTrigger;
    public bool triggerUnburrow;

    public bool switchCamToScorp;
    public bool isScaringPlayer;

    public bool poisonedLizard;

    public bool hasBurAtk;
    bool reBurrow;
    public bool doOnce;
    public bool doOnce2;

    bool jump;
    Vector3 lookDirection;

    public GameObject upReference;

    public GameObject camHolder;
    public GameObject scorpionFace;

    public GameObject location1;
    public GameObject location2;

    public LayerMask groundMask;

    public GameObject instParticle;
    public GameObject rumbleParticle;

    public float desiredDuration = .5f;
    public float declineDesiredDuration = 1f;
    private float elapsedTime = 0.0f;
    private float declineElapsedTime = 0.0f;

    private void Awake()
    {
        damageArea.SetActive(false);
    }

    private void Start()
    {
        trackingLizards = true;
        Vector3[] sc = new Vector3[currentLizards.Count];
        lizardDistances = new List<Vector3>(sc);
        currentState = idleState;
    }
    public void NextPoint()
    {

        if (patrolPosition == 0)
        {
            if (Physics.Raycast(patrolPoints[0].transform.position, transform.TransformDirection(Vector3.down), out rayHit, Mathf.Infinity, layerMask, QueryTriggerInteraction.UseGlobal))
            {
                RandomizePoint1();
            }
        }


        if (agent.remainingDistance <= minimumRange && currentState == roamState)
        {
            patrolPosition = 0;
            turnChance = (int)Random.Range(turnChanceMin, turnChanceMax);
            stopInt = Random.Range(idleStopIntMin, idleStopIntMax);
            chanceToIdle = true;
        }
        if (chanceToIdle)
        {
            idleRNG = Random.Range(1, 6);
            chanceToIdle = false;
        }
        if (idleRNG == 5)
        {
            if (!agent.pathPending)
                if (agent.remainingDistance <= minimumRange && currentState != burrowIdleState)
                {
                    ChangeState(idleState);
                    idleRNG = 0;

                }
        }
        else idleRNG = 0;
        if (turnChance == turnChanceMin)
        {
            nextTimerStart = true;
            DirectionalSwitch();
        }
    }
    //method; randomizes patrol point #1
    public void RandomizePoint1()
    {
        patrolPosition++;
        patrolPosX = Random.Range(transform.position.x + pPointMinXRngValue, transform.position.x + pPointMaxXRngValue);
        patrolPosZ = Random.Range(transform.position.z + pPointMinZRngValue, transform.position.z + pPointMaxZRngValue);
        if (currentState == roamState)
        {
            patrolPoints[0].transform.position = new Vector3(patrolPosX, 7f, patrolPosZ);
            if (!agent.pathPending)
                agent.SetDestination(patrolPoints[0].transform.position);
        }
        if (currentState == burrowIdleState)
        {
            patrolPoints[0].transform.position = new Vector3(patrolPosX, -19f, patrolPosZ);
            if (!agent.pathPending)
                agent.SetDestination(patrolPoints[0].transform.position);
        }
    }



    //method; inverts the X value, changing the direction of the creature
    public void DirectionalSwitch()
    {
        switchChance = (int)Random.Range(1f, 3f);
        if (switchChance == 2)
        {
            pPointMinXRngValue *= -1f;
            pPointMaxXRngValue *= -1f;
        }
    }


    //method; forcing a directional switch on the creature
    public void ForceDirSwitch()
    {
        pPointMinXRngValue *= -1f;
        pPointMaxXRngValue *= -1f;
    }

    public void BeginBurrow()
    {
        if (agent.enabled)
            agent.enabled = false;
        nmsBurrow.enabled = true;
    }
    public void EndBurrow()
    {
        if (agent.enabled)
            agent.enabled = false;
        nms.enabled = true;
        startRise = true;
    }
    public void StartDecline()
    {
        valueDec -= .005f;
        transform.position = new Vector3(transform.position.x, (transform.position.y) + valueDec, transform.position.z);
        doOnce2 = false;
        if (transform.position.y <= -19f && isScaringPlayer)
        {
            print("scaring player boy");
            startDecline = false;
            nms.enabled = false;
            nmsBurrow.enabled = true;
            agent.enabled = true;
            //ChangeState(burrowIdleState);
            valueDec = 0;
            transform.eulerAngles = Vector3.zero;
            transform.GetChild(0).transform.eulerAngles = new Vector3(0f, 180f, 0f);
            isburrowAtkLizard = false;
            if (gc.trappedLizard == null)
                gc.EndGlobalEvents();
            gc.scorpEnableTimer = 0f;
            damageArea.SetActive(false);
            gameObject.SetActive(false);
        }
        if (transform.position.y <= -19f && isburrowAtk)
        {
            print("player boy");
            startDecline = false;
            nms.enabled = false;
            nmsBurrow.enabled = true;
            agent.enabled = true;
            ChangeState(burrowAttackState);
            valueDec = 0;
        }
        if (transform.position.y <= -19f && isburrowAtkLizard)
        {
            print("lizard boy");
            startDecline = false;
            nms.enabled = false;
            nmsBurrow.enabled = true;
            agent.enabled = true;
            ChangeState(burrowAttackLizardState);
            valueDec = 0;
        }
        if (transform.position.y <= -19f && eruptNearPlayer)
        {
            print("erupt near player boy");
            startDecline = false;
            nms.enabled = false;
            nmsBurrow.enabled = true;
            agent.enabled = true;
            ChangeState(burrowAttackState);
            isburrowAtkLizard = false;
            valueDec = 0;
        }
    }
    public void StartRise()
    {
        valueDec += .005f;
        transform.position = new Vector3(transform.position.x, (transform.position.y) + valueDec, transform.position.z);
    }
    void Update()
    {



        print("Current State - " + currentState);

        RaycastHit hit;
        if (Physics.Raycast(new Vector3(transform.position.x, 20f, transform.position.z),
            -transform.up, out hit, groundMask))
        {
            unburrowTrigger.transform.position = hit.point;
        }

        if (triggerUnburrow)
        {
            animator.SetTrigger("IsUnburrowing");
            triggerUnburrow = false;
        }

        getDistanceTimer += Time.deltaTime;
        if (isTrapped)
        {
            ChangeState(trappedState);
            isTrapped = false;
        }
        debugTimer += Time.deltaTime;
        posX = player.transform.position.x - transform.position.x;
        posZ = player.transform.position.z - transform.position.z;
        if (posX < 0)
            posX *= -1;
        if (posZ < 0)
            posZ *= -1;
        if (atkCD > 0)
        {
            atkCD -= Time.deltaTime;
        }


        if (currentState != null)
        {
            currentState.UpdateState(this);
        }
        if (currentState == burrowAttackState && !hasBurAtk)
        {
            animator.SetBool("IsBurrowAttacking", true);
            if (burrowTracker.transform.position.x - transform.position.x <= 4f && burrowTracker.transform.position.x - transform.position.x >= -4f)
                if (burrowTracker.transform.position.z - transform.position.z <= 4f && burrowTracker.transform.position.z - transform.position.z >= -4f)
                {
                    if (!isCharging)
                        inBurAtkRange = true;
                }
            if (inBurAtkRange)
            {
                    agent.enabled = false;
                    transform.position = new Vector3(player.transform.position.x, transform.position.y + .5f, player.transform.position.z);
                    if (transform.position.y >= player.transform.position.y - 2f)
                    {
                    if (succ != null)
                    {
                        if (!succ.beginSuckage)
                            isCharging = true;
                    }
                        else
                        isCharging = true;


                    if (!watchCases && lizardInPosition)
                    {
                        eruptNearPlayer = false;
                        isburrowAtkLizard = true;
                        burrowAttackLizardState.climbUp = true;
                        ChangeState(burrowAttackLizardState);
                        Debug.Log("SWITCHING POSITIONS");
                        isCharging = false;
                    }
                }
            }
        }
        print(lookDirection);
        if (isCharging)
        {
            if (!hasDone)
            {
                if (isburrowAtk)
                {
                    burrowLockPosition = player.transform.position;
                    Vector3 burrow = new Vector3(player.transform.position.x, player.transform.position.y + 30, player.transform.position.z); 
                    if (Physics.Raycast(burrow, -transform.up, out hit, Mathf.Infinity, groundMask))
                    {
                        Quaternion rotation = Quaternion.LookRotation(hit.normal);
                        instParticle = Instantiate(rumbleParticle, hit.point, rotation);
                    }
                }

                float RNGRangeX = Random.Range(7f, 15f);
                float RNGRangeZ = Random.Range(7f, 15f);

                print("RNG X " + RNGRangeX);
                print("RNG Z " + RNGRangeZ);

                int posOrNeg = Random.Range(1, 3);

                if (posOrNeg == 1)
                {
                    RNGRangeX = RNGRangeX;
                    RNGRangeZ = RNGRangeZ;
                }
                else if (posOrNeg == 2)
                {
                    RNGRangeX = -RNGRangeX;
                    RNGRangeZ = -RNGRangeZ;
                }


                if (eruptNearPlayer)
                {
                    burrowLockPosition = new Vector3(player.transform.position.x + RNGRangeX, transform.position.y, player.transform.position.z + RNGRangeZ);
                    Vector3 burrowRumble = new Vector3(burrowLockPosition.x, burrowLockPosition.y + 20, burrowLockPosition.z);
                    if (Physics.Raycast(burrowRumble, -transform.up, out hit, Mathf.Infinity, groundMask))
                    {
                        Quaternion rotation = Quaternion.LookRotation(hit.normal);
                        instParticle = Instantiate(rumbleParticle, new Vector3(hit.point.x, hit.point.y + 2, hit.point.z), rotation);
                    }
                }



                print("RNG LOCATION" + burrowLockPosition);

                Tween.ShakeLocalPosition(camHolder.transform, strength: new Vector3(.25f, .25f, .25f), duration: 2.1f, frequency: 40);
                scorpAudio.StopDiggingSound();

                rumbleAudio.Play();

                hasDone = true;
            }
            transform.position = new Vector3(burrowLockPosition.x, burrowLockPosition.y - 3f, burrowLockPosition.z);
            explodeTimerStart = true;
            inBurAtkRange = false;
            jump = true;
        }
        if (explodeTimerStart)
            explodeTimer += Time.deltaTime;
        if (explodeTimer > 2.15f)
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        if (explodeTimer >= 2.2f)
        {
            damageArea.transform.position = transform.position;
            isCharging = false;
            scorpAudio.PlayStartBurrowSound();

            rumbleAudio.Stop();

            if (jump)
            {
                damageArea.SetActive(true);
                Tween.ShakeLocalPosition(camHolder.transform, strength: new Vector3(.4f, .4f, .4f), duration: 2f, frequency: 50);
                scorpAudio.PlayStartBurrowSound();
                GetComponent<Collider>().enabled = false;
                GetComponent<Rigidbody>().AddForce(upReference.transform.up * 21f, ForceMode.VelocityChange);
                jump = false;
            }
            lookDirection = new Vector3(GetComponent<Rigidbody>().velocity.y * -9f, 0f, 0f);
            hasBurAtk = true;
            transform.rotation = Quaternion.Euler(lookDirection);
        }

        if (explodeTimer > 2.4f && explodeTimer < 2.6f)
        {
            damageArea.SetActive(false);
        }

        if (currentState == burrowAttackState)
            if (Physics.Raycast(transform.position, -upReference.transform.up, out hit, 3f, groundMask) && explodeTimer >= 4f)
            {

                transform.eulerAngles = new Vector3(0f, 180f, 0f);
                transform.GetChild(0).GetComponent<RotateToGround>().RotateNow();
                animator.SetBool("IsBurrowAttacking", false);


                if (eruptNearPlayer)
                {
                    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;
                    hasDone = false;
                    hasBurAtk = false;
                    nms.enabled = true;
                    damageArea.SetActive(false);
                    nms.enabled = true;
                    agent.enabled = true;
                    GetComponent<Collider>().enabled = true;
                    explodeTimerStart = false;
                    ChangeState(enableWeatherState);
                }
                else
                {

                    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;
                    hasDone = false;
                    hasBurAtk = false;
                    nms.enabled = true;
                    damageArea.SetActive(false);
                    nms.enabled = true;
                    agent.enabled = true;
                    GetComponent<Collider>().enabled = true;
                    explodeTimerStart = false;
                    print("MEEMOOP");
                    //ChangeState(burrowState);
                }


            }


        /*if (explodeTimer >= 2.2f)
        {
            agent.enabled = true;
            nms.enabled = true;
            ChangeState(chaseState);
            hasDone = false;
            inBurAtkRange = false;
            damageArea.SetActive(false);
            explodeTimer = 0;
            explodeTimerStart = false;
            elapsedTime = 0;
            declineElapsedTime = 0;
        }*/
        if (currentState == endBurrowState)
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out rayHit, Mathf.Infinity, groundMask))
            {
                startRise = false;
                nms.enabled = true;
                nmsBurrow.enabled = false;
                agent.enabled = true;
                ChangeState(roamState);
                valueDec = 0;
            }
        }
        if (getDistanceTimer >= 2.5f)
            getDistance = true;
        if (getDistance)
        {
            for (int i = 0; i < currentLizards.Count; i++)
            {
                if (i == 0)
                    closestDistance = lizardDistances[0];
                if (currentLizards[i] != null)
                {
                    GetDistance(currentLizards[i], i);

                }
                if ((Mathf.Abs(closestDistance.x)) + (Mathf.Abs(closestDistance.z)) > ((Mathf.Abs(lizardDistances[i].x)) + (Mathf.Abs(lizardDistances[i].z))))
                {
                    closestDistance = lizardDistances[i];
                    closestDistanceLizard = currentLizards[i];
                    if (currentState == chaseLizardState)
                    {
                        lockedOnObject = closestDistanceLizard;
                    }
                }

                if (Mathf.Abs(closestDistance.x) < 15f && Mathf.Abs(closestDistance.z) < 15f && debugTimer >= 2f && currentState == roamState)
                {
                    lockedOnObject = closestDistanceLizard;
                    ChangeState(chaseLizardState);
                }
            }
            getDistance = false;
            getDistanceTimer = 0;

        }

        print("burrow atk? " + isburrowAtk);
        print("near player atk? " + eruptNearPlayer);

        if (hasHitPlayer)
        {
            Invoke("BurrowAfterPlayerHit", .5f);
            hasHitPlayer = false;
        }

    }

    public void GetDistance(GameObject obj, int i)
    {
        if (i < currentLizards.Count)
        {
            if (currentLizards[i] != null)
                lizardDistances[i] = obj.transform.position - transform.position;

        }
    }

    public void ChangeState(IState newState)
    {
        if (currentState != null)
        {
            currentState.OnExit(this);
        }
        currentState = newState;
        currentState.OnEnter(this);
    }

    public void BurrowAfterPlayerHit()
    {
        if (currentState != null)
        {
            currentState.OnExit(this);
        }
        currentState = burrowState;
        currentState.OnEnter(this);
    }
    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Lizard")
        {
            unhingeTrap = true;
            for (int i = 0; i < currentLizards.Count; i++)
            {
                if (col.gameObject.name == currentLizards[i].name)
                {
                    lizardName = currentLizards[i].name;
                    removeFromLizardLists = true;
                    isburrowAtkLizard = false;
                    gc.wakeUpScorp = false;
                    RemoveGameObject(i);
                    Destroy(col.gameObject);
                }
            }

            hasHitLizard = true;
        }
    }

    public void OnCollisionStay(Collision col)
    {
        if (col.gameObject.tag == "Player" && !gc.deadScorpion)
        {
            hasHitPlayer = true;
            Tween.ShakeLocalPosition(camHolder.transform, strength: new Vector3(.3f, .3f, .3f), duration: .3f, frequency: 40);
            col.gameObject.GetComponent<CollisionForces>().NormalAttackHitback(transform.forward, 5f, transform.up, .2f);
        }
    }

    public void RemoveGameObject(int integer)
    {
        lizSpawn.isDead = true;
        currentLizards.RemoveAt(integer);
        lizardDistances.RemoveAt(integer);

    }

    public void SpawnGroundParticles(Vector3 position, Quaternion rotation)
    {
        instParticle = Instantiate(rumbleParticle, position, rotation);
    }
    public void DestroyGroundParticles()
    {
        Destroy(instParticle);
    }


    public void UpdateList()
    {
        sc = new Vector3[gc.lizardsFound.Count];
        lizardDistances = new List<Vector3>(sc);
        currentLizards = gc.lizardsFound;
    }

    public void SpawnPoisonBall()
    {
        if (!doOnce)
        {
            poisonBall = Instantiate(cube, tailEnd.transform.position, Quaternion.identity);
            doOnce = true;
        }
    }

    public void ForceHuntPlayer()
    {
        isburrowAtkLizard = true;
        watchCases = true;
        if (currentState != burrowAttackLizardState)
            ChangeState(burrowAttackLizardState);
        if(!doOnce2)
        {
            eruptNearPlayer = false;
            agent.enabled = false;
            transform.position = new Vector3(transform.position.x, -19f, transform.position.z);
            doOnce2 = true;
        }
    }
}


    public interface IState
    {
        public void OnEnter(StateController controller);

        public void UpdateState(StateController controller);

        public void OnHurt(StateController controller);

        public void OnExit(StateController controller);
    }