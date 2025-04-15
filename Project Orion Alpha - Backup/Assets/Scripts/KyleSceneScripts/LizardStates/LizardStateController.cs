using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class LizardStateController : MonoBehaviour
{
    public LizardIState currentState;
    public LizardIdleState idleState = new LizardIdleState();
    public LizardRoamState roamState = new LizardRoamState();
    public LizardChaseState chaseState = new LizardChaseState();
    public LizardScurryState scurryState = new LizardScurryState();
    public LizardFriendState friendState = new LizardFriendState();
    public LizardTrappedState trappedState = new LizardTrappedState();
    public LizardStunnedState stunnedState = new LizardStunnedState();
    public LizardConfusedState confusedState = new LizardConfusedState();
    public float pPointMinXRngValue;
    public float pPointMaxXRngValue;
    public float pPointMinZRngValue;
    public float pPointMaxZRngValue;
    public int idleStopIntMin;
    public int idleStopIntMax;
    public float minimumRange;
    public NavMeshAgent agent;
    public GameController gc;
    public Renderer rend;
    public GameObject[] patrolPoints;
    public int patrolPosition = 0;
    float patrolPosX;
    float patrolPosZ;
    int switchChance = 0;
    bool nextTimerStart = false;
    public float turnChanceMin;
    public float turnChanceMax;
    int turnChance;
    public int stopInt;
    public LayerMask layerMask;
    public LayerMask playerMask;
    public LayerMask groundMask;
    public GameObject player;
    public GameObject enemy;
    public Inventory playerInv;
    Vector3 reference;
    public Material defaultMat;
    public Material friendMat;
    RaycastHit rayHit;
    public float posX;
    public float posZ;
    public float posXenemy;
    public float posZenemy;
    int lizardCount = 0;
    int closestCheck = 1;
    public Vector3 closestDistance;
    public GameObject closestDistanceLizard;
    public List<GameObject> currentLizards = new List<GameObject>();
    public Vector3[] sc;
    public List<Vector3> lizardDistances = new List<Vector3>();
    public LizardStateController[] ctrl;
    public List<LizardStateController> lizardCtrl = new List<LizardStateController>();
    bool trackingLizards;
    public bool isScurrying;
    public StateController scorpController;


    public bool isTrapped;
    public bool isTamed;
    public bool isLookingAtPlayer;

    public float lookDistance;

    float getDistanceTimer;
    bool getDistance = true;

    public bool airbourne;

    public bool fleeFromEnemy;
    public bool fleeFromPlayer;

    public bool longBreath;

    public Animator animator;

    public GameObject upReference;

    public Vector3 lockedPos;
    public bool lockPosition;

    public bool staminaCooldownStart;
    public float staminaCooldown;

    public ParticleSystem breathParticle;
    public ParticleSystem hardBreathParticle;
    public ParticleSystem confusedParticle;
    public ParticleSystem tameParticle;
    public ParticleSystem fleeParticle;

    public float scurrySpeed = 5.5f;
    public float scurryAccel = 30f;

    public bool hasBeenShot;
    public bool hasBeenTamed;

    public bool tiredScurry;

    public float playerDetectionDistance;
    public bool willFleeFromPlayer;
    public bool willFleeFromAgressiveness;
    public bool willSlowApproach;


    public StatusToPlayer currentStatus;
    public enum StatusToPlayer
    {
        Scared,
        Cautious,
        Friendly,
        Tamed
    }

    private void Start()
    {
        staminaCooldown = -1f;
        ChangeState(roamState);
        rend.material = defaultMat;
        LizardStateController[] ctrl = new LizardStateController[gc.lizardsFound.Count];
        lizardCtrl = new List<LizardStateController>(ctrl);
        sc = new Vector3[gc.lizardsFound.Count];
        lizardDistances = new List<Vector3>(sc);

        currentStatus = StatusToPlayer.Scared;
    }
    public void NextPoint()
    {

        if (patrolPosition == 0)
        {
            RandomizePoint1();
        }
        if (!Physics.Raycast(patrolPoints[0].transform.position, transform.TransformDirection(Vector3.down), out rayHit, Mathf.Infinity, layerMask, QueryTriggerInteraction.UseGlobal))
        {
            RandomizePoint1();
        }

        if(!agent.pathPending)
        if (agent.remainingDistance <= minimumRange)
        {
            patrolPosition = 0;
            turnChance = (int)Random.Range(turnChanceMin, turnChanceMax);
            stopInt = Random.Range(idleStopIntMin, idleStopIntMax);
        }


        if (turnChance == turnChanceMin)
        {
            nextTimerStart = true;
            DirectionalSwitch();
        }
        if (turnChance == turnChanceMax || turnChance == turnChanceMax - 1)
        {
            nextTimerStart = true;
            ForceDirSwitch();
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
            patrolPoints[0].transform.position = new Vector3(patrolPosX, 2f, patrolPosZ);
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
    void Update()
    {
        print("state - " + currentState);

            

        if (staminaCooldownStart)
        {
            staminaCooldown -= Time.deltaTime;
        }

        if (staminaCooldown <= 0)
        {
            scurrySpeed = 5.5f;
            scurryAccel = 30f;
            staminaCooldownStart = false;
            tiredScurry = false;
        }
        else
        {
            tiredScurry = true;
            scurrySpeed = 3f;
            scurryAccel = 12f;
        }


        getDistanceTimer += Time.deltaTime;
        if (isTrapped)
        {
            ChangeState(trappedState);
            gc.wakeUpScorp = true;
            isTrapped = false;
        }

        RaycastHit hit;

        if (hasBeenTamed)
        {
            tameParticle.Play();
            currentStatus = StatusToPlayer.Tamed;
            hasBeenTamed = false;
        }

        switch (currentStatus)
        {
            case StatusToPlayer.Scared:
                {
                    lookDistance = 15f;
                    willFleeFromPlayer = true;
                    playerDetectionDistance = 6f;
                    break;
                }
            case StatusToPlayer.Cautious:
                {
                    lookDistance = 5f;
                    willFleeFromPlayer = true;
                    playerDetectionDistance = 3f;
                    willSlowApproach = true;
                    break;
                }
            case StatusToPlayer.Friendly:
                {
                    willFleeFromPlayer = false;
                    willFleeFromAgressiveness = true;
                    break;
                }
            case StatusToPlayer.Tamed:
                {
                    willFleeFromPlayer = false;
                    ChangeState(friendState);
                    isTamed = true;
                    break;
                }
        }


        RaycastHit playerHit;

        if (Physics.SphereCast(transform.position, 1f, transform.forward, out playerHit, lookDistance, playerMask))
        {
            isLookingAtPlayer = true;
        }


        print("status " + currentStatus);





        if (stopInt == idleStopIntMin)
        {
            lockedPos = transform.position;
            ChangeState(idleState);
            stopInt = 0;
        }
        else if(lockPosition)
        {
            lockedPos = transform.position;
            lockPosition = false;
        }


        if (hasBeenShot)
        {
            GetComponent<LizardHealth>().TakeDamage(105f);
            hasBeenShot = false;
        }
        if (getDistanceTimer >= 2.5f)
            getDistance = true;
        if(getDistance)
        {
            for (int i = 0; i < currentLizards.Count; i++)
            {
                if (currentLizards[i] != null)
                {
                    GetDistance(currentLizards[i], i);

                }
            }
            getDistance = false;
            getDistanceTimer = 0;
        }
        for (int i = 0; i < currentLizards.Count; i++)
        {
            if (currentLizards[i] != null)
                lizardCtrl[i] = currentLizards[i].GetComponent<LizardStateController>();
        }
        posX = player.transform.position.x - transform.position.x;
        posZ = player.transform.position.z - transform.position.z;

        if(enemy.activeSelf)
        posXenemy = enemy.transform.position.x - transform.position.x;
        posZenemy = enemy.transform.position.z - transform.position.z;

        if (currentState != null)
        {
            currentState.UpdateState(this);
        }
        if(currentState == scurryState)
        {
            isScurrying = true;
        }

    }
    public void ChangeState(LizardIState newState)
    {
        if (currentState != null)
        {
            currentState.OnExit(this);
        }
        currentState = newState;
        currentState.OnEnter(this);
    }

    public void GetDistance(GameObject obj, int i)
    {
        if (i < currentLizards.Count)
        {
            if (currentLizards[i] != null)
                lizardDistances[i] = obj.transform.position - transform.position;

        }
    }


    public void UpdateList()
    {
        ctrl = new LizardStateController[gc.lizardsFound.Count];
        lizardCtrl = new List<LizardStateController>(ctrl);
        sc = new Vector3[gc.lizardsFound.Count];
        lizardDistances = new List<Vector3>(sc);
        currentLizards = gc.lizardsFound;
    }


}
public interface LizardIState
{
    public void OnEnter(LizardStateController controller);

    public void UpdateState(LizardStateController controller);

    public void OnHurt(LizardStateController controller);

    public void OnExit(LizardStateController controller);
}