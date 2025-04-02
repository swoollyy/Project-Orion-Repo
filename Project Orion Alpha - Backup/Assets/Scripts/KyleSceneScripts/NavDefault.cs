using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavDefault : MonoBehaviour
{
    public float pPointMinXRngValue;
    public float pPointMaxXRngValue;
    public float pPointMinZRngValue;
    public float pPointMaxZRngValue;
    public float minimumRangeToPoint;
    public float lurkTurnTime;
    public float idleStopIntMin;
    public float idleStopIntMax;
    public float maxChaseDistance;
    public NavMeshLinkInstance nmld;
    public NavMeshAgent agent;
    public GameObject[] patrolPoints;
    public GameObject player;
    public GameObject enemy;
    public GameObject cube;
    public AudioSource soundSource;
    public Movement mMent;
    public LayerMask layerMask;
    public debug dBug;
    public Vector3 endChase;
    public bool inMovementState = false;
    public bool inLurkState = false;
    public bool hasChased = false;
    int patrolPosition = 0;
    int patrolCount = 0;
    public float maxPatrolCount;
    int turnChance;
    public float turnChanceMin;
    public float turnChanceMax;
    int switchChance = 0;
    bool timerStart = false;
    bool sTimerStart = false;
    bool nextTimerStart = false;
    bool lurkComplete;
    bool lurk;
    float rotate;
    float random;
    float timer = 0f;
    float surveyTimer = 0f;
    float nextTimer = 0f;
    float turner;
    float patrolPosX;
    float patrolPosZ;
    float stopInt;
    Vector3 reference;
    NavMeshPath optPath;


    // Start is called before the first frame update
    void Start()
    {
        //patrol; maximum amt of times creature stands still & rotates around its surroundings until it continues moving
        maxPatrolCount = GetComponent<float>();

        //movement; minimum and maximum amount of attempts it takes to go in the opposite x value
        turnChanceMin = GetComponent<float>();
        turnChanceMax = GetComponent<float>();

        //movement; amount of time it takes for the creature to continue moving after stopping
        idleStopIntMin = GetComponent<float>();
        idleStopIntMax = GetComponent<float>();

        //movement; controls the X & Z position of the patrol points
        pPointMinXRngValue = GetComponent<float>();
        pPointMaxXRngValue = GetComponent<float>();
        pPointMinZRngValue = GetComponent<float>();
        pPointMaxZRngValue = GetComponent<float>();

        //movement; the amount of time in between looking around for the player
        lurkTurnTime = GetComponent<float>();

        //movement; minimum value that enemy needs to be within to the patrol point
        minimumRangeToPoint = GetComponent<float>();

        //movement; maximum value the enemy will chase the player
        maxChaseDistance = GetComponent<float>();

        //movement; initiates the position of patrol points on runtime
        patrolPosX = Random.Range(transform.position.x * pPointMinXRngValue, transform.position.x * pPointMaxXRngValue);
        patrolPosZ = Random.Range(transform.position.z * pPointMinZRngValue, transform.position.z * pPointMaxZRngValue);

        //movement; begins the patrol point randomization
        RandomizePoint1();

        //variable assign; assigns declared speed, acceleration, and turnSpeed to navMesh
        agent.speed = 6f;
        agent.acceleration = 17f;
        agent.angularSpeed = 2700f;
    }

    //method; responsible for assigning the next set of patrol point positions
    void NextPoint()
    {
        if (patrolPosition == 1)
            RandomizePoint2();
        if (patrolPosition == 2)
            RandomizePoint1();
        if (patrolPosition >= 2)
            patrolPosition = 0;
        agent.SetDestination(patrolPoints[patrolPosition].transform.position);
        patrolPosition++;
    }


    //method; randomizes patrol point #1
    void RandomizePoint1()
    {
        patrolPosX = Random.Range(transform.position.x + pPointMinXRngValue, transform.position.x + pPointMaxXRngValue);
        patrolPosZ = Random.Range(transform.position.z + pPointMinZRngValue, transform.position.z + pPointMaxZRngValue);
        if (inMovementState)
            patrolPoints[0].transform.position = new Vector3(patrolPosX, 2f, patrolPosZ);
        inMovementState = true;
    }


    //method; randomizes patrol point #2
    void RandomizePoint2()
    {
        patrolPosX = Random.Range(transform.position.x + pPointMinXRngValue, transform.position.x + pPointMaxXRngValue);
        patrolPosZ = Random.Range(transform.position.z + pPointMinZRngValue, transform.position.z + pPointMaxZRngValue);
        if (inMovementState)
            patrolPoints[1].transform.position = new Vector3(patrolPosX, 2f, patrolPosZ);
        inMovementState = true;
    }


    //method; inverts the X value, changing the direction of the creature
    void DirectionalSwitch()
    {
        switchChance = (int)Random.Range(1f, 3f);
        if (switchChance == 2)
        {
            pPointMinXRngValue *= -1f;
            pPointMaxXRngValue *= -1f;
        }
    }


    //method; forcing a directional switch on the creature
    void ForceDirSwitch()
    {
        pPointMinXRngValue *= -1f;
        pPointMaxXRngValue *= -1f;
    }

    //method; confirms that the player chase is over
    void EndChase()
    {
        hasChased = true;
        cube.transform.position = endChase;
    }


    // Update is called once per frame
    void Update()
    {
        NavMeshHit hit;
        RaycastHit rayHit;
        //tracks the distance between creature and player
        float posX = player.transform.position.x - transform.position.x;
        float posZ = player.transform.position.z - transform.position.z;
        if (posX < 0)
            posX *= -1;
        if (posZ < 0)
            posZ *= -1;

        //if player is within this distance of the enemy, chase the players position exactly
        if (!mMent.sneak)
            if ((posX < 24 && (posZ < 25) && posX + posZ < 32) || (posZ < 24 && (posX < 25) && posZ + posZ < 32))
            {
                dBug.chase = true;
                inMovementState = true;
            }

        //detection radius while player is sneaking; will lurk if player is within radius
        if (mMent.sneak)
            if ((posX < 10 && (posZ < 10) && posX + posZ < 15) || (posZ < 10 && (posX < 10) && posX + posZ < 15))
            {
                agent.speed = 9f;
                agent.acceleration = 19f;
                lurk = true;
                agent.isStopped = true;
                hasChased = true;
                inLurkState = true;

            }

        //sound detection; minimum values
        if ((posX < 35 || posZ < 35) && posX + posZ < 30 && soundSource.isPlaying)
        {
            if (!mMent.sneak)
            {
                lurk = false;
                agent.isStopped = false;
                agent.speed = 9f;
                agent.acceleration = 19f;
                agent.SetDestination(player.transform.position - transform.position * .06f);
                hasChased = true;
                print("solo");
                inLurkState = false;
            }
        }
        //sound detection; maximum values
        if (posX + posZ < 50 && posX + posZ > 30 && (posX < 35 && posZ < 35) && soundSource.isPlaying)
        {
            if (!mMent.sneak)
            {
                lurk = false;
                agent.isStopped = false;
                agent.speed = 9f;
                agent.acceleration = 19f;
                agent.SetDestination(player.transform.position - transform.position * .06f);
                hasChased = true;
                print("cumbo");
                inLurkState = false;
            }
        }

        //if creature detects ground underneath, start movement
        if (!Physics.Raycast(patrolPoints[0].transform.position, transform.TransformDirection(Vector3.down), out rayHit, Mathf.Infinity, layerMask, QueryTriggerInteraction.UseGlobal))
        {
            RandomizePoint1();
        }

        if (!Physics.Raycast(patrolPoints[1].transform.position, transform.TransformDirection(Vector3.down), out rayHit, Mathf.Infinity, layerMask, QueryTriggerInteraction.UseGlobal))
        {
            RandomizePoint2();
        }

        //if creature is within minimumRangeToPoint, begin idle & direction behavior
        if (agent.remainingDistance <= minimumRangeToPoint)
        {
            turnChance = (int)Random.Range(turnChanceMin, turnChanceMax);
            stopInt = Random.Range(idleStopIntMin, idleStopIntMax);

            //if turnChance equals the minimum value, begin directional change
            if (turnChance == turnChanceMin)
            {
                nextTimerStart = true;
                DirectionalSwitch();
            }

            //adds an additional chance to change direction
            if (turnChance == turnChanceMax || turnChance == turnChanceMax - 1)
            {
                nextTimerStart = true;
                ForceDirSwitch();
            }

            //if the creature has finished chasing, begin lurking
            if (hasChased)
            {
                lurk = true;
                agent.isStopped = true;
                inLurkState = true;
            }

            if (nextTimerStart)
                nextTimer += Time.deltaTime;

            if (nextTimer >= stopInt)
            {
                NextPoint();
                nextTimerStart = false;
                nextTimer = 0;
            }
        }

        //if the creature is lurking, it will stop and rotate around it to scout for the player
        if (lurk && !dBug.chase)
        {
            timerStart = true;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, random, 0f), Time.deltaTime * 3f);
            inMovementState = false;
            inLurkState = true;
        }


        if (timerStart)
        {
            timer += Time.deltaTime;
            timerStart = false;
        }

        //after this amount of time, turn a new direction
        if (timer > lurkTurnTime)
        {
            patrolCount++;
            timer = 0;
            random = Random.Range(0f, 360f);
        }

        //if the creatures turned the maxPatrolCount or detects the player and is chasing, ends the lurk state
        if (patrolCount >= maxPatrolCount || dBug.chase)
        {
            lurk = false;
            hasChased = false;
            agent.isStopped = false;
            lurkComplete = true;
            patrolCount = 0;
            timer = 0;
            inLurkState = false;
            agent.speed = 6f;
            agent.acceleration = 17f;
            inMovementState = true;
        }

        //the creature is chasing the player
        if (dBug.chase)
        {
            agent.speed = 9f;
            agent.acceleration = 19f;
            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
            lurkComplete = false;
            if (NavMesh.FindClosestEdge(transform.position, out hit, NavMesh.AllAreas))
            {
                Debug.DrawRay(hit.position, Vector3.up, Color.green);
            }
        }

        //while chasing, if the player gets past these values, approximate a location close to the player and begin lurk
        if (dBug.chase && player.transform.position.x - transform.position.x > maxChaseDistance)
        {
            endChase = player.transform.position - transform.forward * 10;
            dBug.chase = false;
            agent.speed = 9f;
            agent.acceleration = 19f;
            EndChase();
            agent.SetDestination(endChase);
        }
        if (dBug.chase && player.transform.position.x - transform.position.x < -maxChaseDistance)
        {
            endChase = player.transform.position - transform.forward * 10;
            dBug.chase = false;
            agent.speed = 9f;
            agent.acceleration = 19f;
            EndChase();
            agent.SetDestination(endChase);

        }
        if (dBug.chase && player.transform.position.z - transform.position.z > maxChaseDistance)
        {
            endChase = player.transform.position - transform.forward * 10;
            dBug.chase = false;
            agent.speed = 9f;
            agent.acceleration = 19f;
            EndChase();
            agent.SetDestination(endChase);

        }
        if (dBug.chase && player.transform.position.z - transform.position.z < -maxChaseDistance)
        {
            endChase = player.transform.position - transform.forward * 10;
            dBug.chase = false;
            agent.speed = 9f;
            agent.acceleration = 19f;
            EndChase();
            agent.SetDestination(endChase);

        }
    }
}
