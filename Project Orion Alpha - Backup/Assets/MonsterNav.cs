using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using PrimeTween;

public class MonsterNav : MonoBehaviour
{

    public LayerMask groundMask;
    public LayerMask waterMask;
    public LayerMask movementMask;
    public LayerMask playerAndOilMask;

    Vector3 hitpoint;

    NavMeshAgent agent;
    NavMeshPath path;

    public GameObject sphere;
    public GameObject eyes;

    public Transform sphereFloor;
    public Transform sphereMid;
    public Transform sphereTop;
    public Transform mapCenter;

    public GameObject player;
    public GameObject camHolder;
    public GameObject oilLocked;
    public BiteCollision colliderObject;

    public bool isOnLand;
    public bool isOnWater;
    public bool isInWater;

    Vector3 randomPosition;
    Vector3 randomPositionLand;
    Vector3 lastPosition;
    Vector3 nextLocation;

    Vector3 jumpStartPosition;
    float jumpProgress;
    Vector3 closestLocation;
    float closestDistance;

    public Vector3 currentRotation;
    public Vector3 desiredRotation;

    bool waterLocFound;
    public bool waterJump;
    public bool isWaterJumping;
    public bool isLandJumping;

    public bool isAbleToJump;
    bool hasHitPlayer;

    bool lockToPlayer;
    public bool lockToOilpool;
    public bool readyToJump;
    bool locationFound;

    public Vector3 moveDirection;
    public Vector3 rightLock;

    public Vector3 closestDirection;

    public Animator animator;

    public float yOffset;

    float timeElapsed;

    float timer;
    float moveTimer;
    float jumpToWaterTimer;
    float jumpToLandTimer;
    float retreatTimer;
    float randomGrowlTimer;
    float randomBreathTimer;
    float lockTimer;
    float animBlendTimer;

    float waterJumpTime;
    float landJumpTime;

    bool doOnceLand;
    bool doOnceWater;
    bool doOnce;
    bool doOnce2;
    bool doOnce3;
    bool doOnceSaveRotation;
    bool doOnceLockPlayer;
    bool doOnceGrowlSFX;
    bool doOnceBreathSFX;
    bool notDetectingPlayerNear;
    bool doOnceStartTimer;
    bool doOnceLockOil;

    Vector3 savedRotation;
    Vector3 lastTargetPosition;

    public bool forceJump;
    public bool monsterBaited;

    int rng;
    float growlRNG;
    float breathRNG;

    public water waterScript;
    public LeverCheck leverCheck;
    public GameControllerLevel2 gc;

    [Header("Player Settings")]
    public float value;

    [Header("Jump Timer Values")]
    public float minValue;
    public float maxValue;

    public List<GameObject> topFloor = new List<GameObject>();
    public List<GameObject> middleFloor = new List<GameObject>();
    public List<GameObject> bottomFloor = new List<GameObject>();

    [Header("Audio")]
    public GameObject jumpOutAudio;
    public AudioSource swimAudio;
    public AudioSource walkSFX;
    public AudioSource growlSFX;
    public AudioSource detectSFX;
    public AudioSource breathSFX;
    public AudioSource breathSFX2;
    public AudioSource biteSFX;
    public AudioSource postBiteSFX;
    public AudioSource waterImpactSFX;
    public AudioSource jumpToWaterSFX;
    public AudioSource deathCrySFX;




    // Start is called before the first frame update
    void Start()
    {
        agent = transform.GetChild(0).GetComponent<NavMeshAgent>();
        path = new NavMeshPath();

        lastPosition = transform.position;

        animBlendTimer = 2.5f;

        waterJumpTime = Random.Range(minValue, maxValue);
        landJumpTime = Random.Range(minValue, maxValue);
    }

    // Update is called once per frame
    void Update()
    {

        if (doOnceStartTimer && !notDetectingPlayerNear)
        {
            lockTimer += Time.deltaTime;
            print("wed " + lockTimer);
        }
        if (lockTimer >= 12.2f)
        {
            doOnceStartTimer = false;
            lockTimer = 0;
        }

        print("wededed " + lockTimer);

        if(isOnLand)
        {
            if(!waterJump)
                agent.enabled = true;
            if (!lockToOilpool)
            DetectPlayer();
            readyToJump = false;
        }
        if (!isAbleToJump && !isWaterJumping && !readyToJump)
        {
            jumpProgress = 0f;
            IsOnLandOrWater();
        }
        if (isOnLand && !waterJump && !lockToPlayer && !leverCheck.finalSegment && !lockToOilpool)
        {
            agent.updateRotation = false;
            readyToJump = false;
            print("enabling agent");
            agent.enabled = true;
            jumpToWaterTimer += Time.deltaTime;

            if(!waterJump && isOnLand)
            if(jumpToWaterTimer >= waterJumpTime)
            {
                FindClosestDirection();
                jumpToWaterTimer = 0;
            }





        }

        if (agent.enabled && !agent.pathPending && !isWaterJumping && !lockToPlayer)
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                agent.ResetPath();
                RandomizePointOnLand();
            }


        //monster rotation fix
        if (agent.enabled)
        {
            if (agent.velocity.normalized.magnitude > .4f)
            {
                transform.GetChild(0).right = agent.velocity.normalized;
                rightLock = transform.GetChild(0).right;
            }
            else if (agent.velocity.normalized.magnitude <= .4f)
            {
                transform.GetChild(0).right = rightLock;
            }
        }


        if(isOnLand)
        {
            if (!doOnceGrowlSFX)
            {
                growlRNG = Random.Range(4f, 17f);
                doOnceGrowlSFX = true;
            }
            if(!doOnceBreathSFX)
            {
                breathRNG = Random.Range(2f, 8f);
                doOnceBreathSFX = true;
            }
            randomGrowlTimer += Time.deltaTime;
            randomBreathTimer += Time.deltaTime;
            if(randomGrowlTimer > growlRNG)
            {
                if (!growlSFX.isPlaying)
                    growlSFX.Play();
                doOnceGrowlSFX = false;
                randomGrowlTimer = 0;
            }
            if(randomBreathTimer > breathRNG)
            {
                int rng = Random.Range(1, 3);
                if (rng == 1)
                {
                    if (!breathSFX.isPlaying)
                        breathSFX.Play();
                }
                else
                    if (!breathSFX2.isPlaying)
                    breathSFX2.Play();
                doOnceBreathSFX = false;
                randomBreathTimer = 0;
            }
        }

        if (isOnWater || isInWater)
        {
             
                if (!readyToJump)
                RandomizePointInWater();
        }

        if ((isOnWater || isInWater) && isAbleToJump)
        {
            jumpToLandTimer += Time.deltaTime;
            transform.GetChild(0).localPosition = Vector3.zero;

        }

        if (jumpToLandTimer >= landJumpTime)
        {
            readyToJump = true;
            if(!doOnce2)
            {
                rng = Random.Range(1, 3);
                doOnce2 = true;
            }
            JumpOnLand();
        }



        if (isOnLand && waterJump && !leverCheck.finalSegment)
        {
            if(!doOnce)
            {
                transform.position = transform.GetChild(0).position;
                transform.GetChild(0).localPosition = Vector3.zero;
                rng = Random.Range(1, 3);

                doOnce = true;
            }
            JumpToWater();
            hasHitPlayer = false;
        }

        if (lockToPlayer && !hasHitPlayer)
        {
            LockPlayer();
        }
        if (isOnLand && lockToOilpool && leverCheck.finalSegment)
        {
            LockOilPool();
        }
        if (hasHitPlayer)
        {
            if (leverCheck.finalSegment)
                agent.ResetPath();
            retreatTimer += Time.deltaTime;
            if(retreatTimer >= .5f && !leverCheck.finalSegment)
            {
                if (!postBiteSFX.isPlaying)
                    postBiteSFX.Play();
                FindClosestDirection();
                retreatTimer = 0;

            }
            else if(retreatTimer >= 4f)
            {
                agent.SetDestination(player.transform.position);
                hasHitPlayer = false;
                retreatTimer = 0f;
            }
        }

        if(leverCheck.finalSegment && !isOnLand)
        {
            rng = 1;
            readyToJump = true;
            JumpOnLand();
        }
        if (forceJump && !isOnLand)
        {
            rng = 1;
            readyToJump = true;
            JumpOnLand();
        }


    }

    public void RandomizePointOnLand()
    {
        print("enabling agent");
        agent.enabled = true;
        agent.speed = 5f;

        float random = 1f;

        if (!animator.GetBool("IsIdle"))
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsIdle", true);
            animator.SetBool("IsSwimming", false);
        }

        if (swimAudio.isPlaying)
            swimAudio.Stop();

        RaycastHit hit;
        NavMeshHit navHit;

        do
        {
            int rngInt = Random.Range(1, 5);
            walkSFX.Stop();
            if (rngInt == 1)
            {
                randomPositionLand = new Vector3(mapCenter.transform.position.x + Random.Range(0f, 42f), transform.position.y + 2f, mapCenter.transform.position.z + Random.Range(0f, 50f));
            }
            else if (rngInt == 2)
            {
                randomPositionLand = new Vector3(mapCenter.transform.position.x + Random.Range(-42f, 0f), transform.position.y + 2f, mapCenter.transform.position.z + Random.Range(-50f, 0f));
            }
            else if (rngInt == 3)
            {
                randomPositionLand = new Vector3(mapCenter.transform.position.x + Random.Range(0f, 42f), transform.position.y + 15f, mapCenter.transform.position.z + Random.Range(-50f, 0f));
            }
            else
                randomPositionLand = new Vector3(mapCenter.transform.position.x + Random.Range(-42f, 0f), transform.position.y + 2f, mapCenter.transform.position.z + Random.Range(-50f, 0f));
            if (Physics.Raycast(randomPositionLand, -transform.up, out hit, Mathf.Infinity, groundMask))
                hitpoint = hit.point;
            random = Random.Range(3f, 5f);
        }
        while
            (!NavMesh.SamplePosition(hitpoint, out navHit, Mathf.Infinity, NavMesh.AllAreas) &&
            !NavMesh.Raycast(transform.position, hitpoint, out navHit, NavMesh.AllAreas) && !NavMesh.CalculatePath(transform.position, hitpoint, NavMesh.AllAreas, path) && path.status == NavMeshPathStatus.PathComplete && agent.remainingDistance < 2f);

        moveDirection = navHit.position;
        moveTimer += Time.deltaTime;


        if (moveTimer > random)
        {
            agent.SetDestination(moveDirection);
            if(!walkSFX.isPlaying)
            walkSFX.Play();
            if (!animator.GetBool("IsWalking"))
            {
                animator.SetBool("IsWalking", true);
                animator.SetBool("IsIdle", false);
                animator.SetBool("IsSwimming", false);
            }
            moveTimer = 0;
        }



    }
    public void RandomizePointInWater()
    {
        print("DOING THIS");

        randomPosition = (player.transform.position + Random.insideUnitSphere * 40f);
        lockToPlayer = false;
        if (!waterLocFound)
        {
            transform.eulerAngles = savedRotation;
            print("booyahh" + savedRotation);
            if (!swimAudio.isPlaying)
                swimAudio.Play();
            if (walkSFX.isPlaying)
                walkSFX.Stop();
            timeElapsed = 0f;
            timer = 0f;
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(randomPosition.x, randomPosition.y + 100, randomPosition.z), -transform.up, out hit, Mathf.Infinity, waterMask))
                hitpoint = hit.point;
            nextLocation = new Vector3(hitpoint.x, hitpoint.y - 10f, hitpoint.z);

            Vector3 direction = (nextLocation - transform.position).normalized;
            if (Vector3.Distance(transform.position, nextLocation) > 15f)
            waterLocFound = true;
        }
        if(waterLocFound)
        {
            if (nextLocation.y > hitpoint.y)
                nextLocation.y = hitpoint.y;

            Vector3 waterLevel = new Vector3(transform.position.x, transform.position.y + 100, transform.position.z);

            if(Physics.Raycast(waterLevel, -transform.up, out RaycastHit hit, Mathf.Infinity, waterMask))
            {
                if(hit.point.y < transform.position.y)
                {
                    transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
                }
            }
            if (!animator.GetBool("IsSwimming"))
            {
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsIdle", false);
                animator.SetBool("IsSwimming", true);
            }
            timer += Time.deltaTime;
            if(timer >= 1.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextLocation, Time.deltaTime * value);
                Vector3 moveDirection = transform.position - lastPosition;
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 3 * Time.deltaTime);
            }

            if (Vector3.Distance(transform.position, nextLocation) < 15f && Vector3.Distance(transform.position, nextLocation) > 14f)
            if(!doOnceSaveRotation)
                {
                    savedRotation = transform.eulerAngles;
                    print("booyah" + savedRotation);
                    doOnceSaveRotation = true;
                }
            if (Vector3.Distance(transform.position, nextLocation) < 7f)
            {
                doOnceSaveRotation = false;
                waterLocFound = false;
            }

            lastPosition = transform.position;

        }
        

    }

    private void IsOnLandOrWater()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 5f, movementMask))
        {
            if (hit.transform.tag == "Terrain" || hit.transform.tag == "Floor")
            {
                isOnWater = false;
                isOnLand = true;
                if(!doOnceLand)
                {
                    doOnceWater = false;
                    swimAudio.Stop();
                    print("enabling agent");
                    agent.enabled = true;
                    doOnceLand = true;
                }
            }
        }

        if (Physics.Raycast(transform.position, -transform.up, out hit, 7f, movementMask))
        if (hit.transform.tag == "Water")
        {
            isOnLand = false;
            isOnWater = true;
            if (!doOnceWater)
            {
                doOnceLand = false;
                    transform.eulerAngles = Vector3.zero;
                    walkSFX.Stop();
                    transform.GetChild(0).right = transform.forward;
                    transform.position = transform.GetChild(0).position;
                    transform.GetChild(0).localPosition = Vector3.zero;
                    agent.enabled = false;
                doOnceWater = true;
            }
            if (!animator.GetBool("IsSwimming"))
            {
                animator.SetBool("IsSwimming", true);
                animator.SetBool("IsWalking", false);
            }
        }

        if (isInWater)
        {
            isOnLand = false;
            isOnWater = true;
            if (!doOnceWater)
            {
                doOnceLand = false;
                transform.eulerAngles = Vector3.zero;
                transform.GetChild(0).right = transform.forward;
                transform.position = transform.GetChild(0).position;
                transform.GetChild(0).localPosition = Vector3.zero;
                agent.enabled = false;
                doOnceWater = true;
            }
            if (!animator.GetBool("IsSwimming"))
            {
                animator.SetBool("IsSwimming", true);
                animator.SetBool("IsWalking", false);
            }
        }
    }

    void JumpOnLand()
    {



        float speed = 1f;


        if (rng == 1)
        {
            if (!animator.GetBool("IsLandJumping"))
            {
                animator.SetBool("IsSwimming", false);

                if (swimAudio.isPlaying)
                    swimAudio.Stop();

                Instantiate(jumpOutAudio, transform.position, Quaternion.identity);


                animator.SetBool("IsLandJumping", true);
            }




            if (jumpProgress < 2f)
            {
                isLandJumping = true;

                jumpProgress += Time.deltaTime * speed;
                if (waterScript.isAtFloor)
                {
                    float height = .5f;
                    Vector3 mid = Vector3.MoveTowards(transform.position, sphereFloor.position, jumpProgress);
                    mid.y += height * Mathf.Sin(.5f * jumpProgress * Mathf.PI);
                    transform.position = mid;
                }
                else if(waterScript.isAtMiddle)
                {
                    float height = .5f;
                    Vector3 mid = Vector3.MoveTowards(transform.position, sphereMid.position, jumpProgress);
                    mid.y += height * Mathf.Sin(.5f * jumpProgress * Mathf.PI);
                    transform.position = mid;
                }
                else
                {
                    float height = 1f;
                    Vector3 mid = Vector3.MoveTowards(transform.position, sphereTop.position, jumpProgress);
                    mid.y += height * Mathf.Sin(.5f * jumpProgress * Mathf.PI);
                    transform.position = mid;
                }

            }
            else
            {
                waterJumpTime = Random.Range(minValue, maxValue);
                jumpToLandTimer = 0f;
                forceJump = false;
                isAbleToJump = false;
                isLandJumping = false;
                isOnLand = true;
                isOnWater = false;
                readyToJump = false;
                agent.enabled = true;
                doOnce2 = false;
                animator.SetBool("IsLandJumping", false);
                jumpProgress = 0f;
            }
        }
        else
        {
            readyToJump = false;
            doOnce2 = false;
            jumpToLandTimer = 0f;
        }
    }

    void FindClosestDirection()
    {
        waterJump = true;


    }

    void JumpToWater()
    {
        //agent.enabled = false;
        float height = 1f;
        float speed = 1f;
        RaycastHit hit;

        if (lockToPlayer)
            waterJump = false;

        print("BOYS");

        if (rng == 1)
        {
            if (Physics.Raycast(transform.position, -transform.up, out hit, Mathf.Infinity, groundMask) && agent.enabled && !lockToPlayer)
            {
                if (hit.transform.gameObject.name == "Box016" || hit.transform.gameObject.name == "Box050")
                    agent.SetDestination(ClosestLocationFound(topFloor));
                else if (hit.transform.gameObject.name == "Box015" || hit.transform.gameObject.name == "Box045")
                    agent.SetDestination(ClosestLocationFound(middleFloor));
                else
                    agent.SetDestination(ClosestLocationFound(bottomFloor));
                if(!walkSFX.isPlaying)
                walkSFX.Play();

                if (!animator.GetBool("IsWalking"))
                {
                    animator.SetBool("IsWalking", true);
                    animator.SetBool("IsIdle", false);
                    animator.SetBool("IsSwimming", false);
                }
                agent.speed = 13;


            }
            if (Vector3.Distance(closestLocation, transform.GetChild(0).transform.position) < 5f)
            {
                isWaterJumping = true;
                if (!jumpToWaterSFX.isPlaying)
                    jumpToWaterSFX.Play();
                walkSFX.Stop();
                agent.enabled = false;
            }
            if (jumpProgress < 2f && isWaterJumping)
            {
                if(!agent.enabled)
                jumpProgress += Time.deltaTime * speed;
                Vector3 previousPosition = transform.position;
                if(!doOnce3)
                {
                    transform.position = transform.GetChild(0).position;
                    Vector3 direction = (transform.position + (transform.position - mapCenter.position));
                    direction.y = -20f;
                    closestDirection = direction;
                    print(direction + "gyatt");
                    doOnce3 = true;
                }
                transform.GetChild(0).localPosition = Vector3.zero;
                Vector3 mid = Vector3.MoveTowards(transform.position, closestDirection, jumpProgress);
                mid.y += height * Mathf.Sin(.5f * jumpProgress * Mathf.PI);

                transform.position = mid;

                Vector3 movementDirection = (mid - previousPosition);
                Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // Smooth rotation
                transform.GetChild(0).right = transform.forward;
            }
            else if(jumpProgress > 2f)
            {
                isOnLand = false;
                isOnWater = true;
                jumpToWaterTimer = 0f;
                readyToJump = false;
                if(!waterImpactSFX.isPlaying)
                waterImpactSFX.Play();
                landJumpTime = Random.Range(minValue, maxValue);
                transform.eulerAngles = Vector3.zero;
                transform.GetChild(0).right = transform.forward;
                transform.position = transform.GetChild(0).position;
                transform.GetChild(0).localPosition = Vector3.zero;
                doOnce = false;
                isWaterJumping = false;
                isAbleToJump = false;
                hasHitPlayer = false;
                doOnceLockPlayer = false;
                walkSFX.Stop();
                locationFound = false;
                waterJump = false;

                jumpProgress = 0f;
            }
        }
        else
        {
            waterJump = false;
            doOnce = false;
        }
    }

    void DetectPlayer()
    {
        RaycastHit[] hits = Physics.SphereCastAll(eyes.transform.position, 8f, transform.GetChild(0).right, 40f);
        RaycastHit[] oilHits = Physics.SphereCastAll(eyes.transform.position, 8f, transform.GetChild(0).right, 40f, playerAndOilMask);
        RaycastHit hitt;
        foreach(RaycastHit hit in hits)
        {
            if(hit.transform.gameObject.tag == "Player")
            {
                Vector3 direction = (hit.transform.position - eyes.transform.position).normalized;
                if (Physics.Raycast(eyes.transform.position, direction, out hitt, Mathf.Infinity))
                {
                    Debug.DrawRay(eyes.transform.position, direction * hitt.distance, Color.red, 2f);
                    if(hitt.transform.gameObject.tag == "Player")
                    {
                        if (!animator.GetBool("IsWalking"))
                        {
                            animator.SetBool("IsWalking", true);
                            animator.SetBool("IsIdle", false);
                            animator.SetBool("IsSwimming", false);
                        }
                        if (!doOnceLockPlayer && !lockToOilpool)
                        {
                            agent.SetDestination(player.transform.position);
                            agent.ResetPath();
                            if (!walkSFX.isPlaying)
                                walkSFX.Play();
                            if (!detectSFX.isPlaying)
                                detectSFX.Play();
                            lastTargetPosition = player.transform.position;
                            animator.Play("TrappedInitial", 1, 0f);
                            animator.SetLayerWeight(1, .29f);
                            animBlendTimer = 2.5f;
                            lockToPlayer = true;
                            doOnceLockPlayer = true;

                        }
                        print("wededededed");
                        lockTimer = 0;
                        notDetectingPlayerNear = true;
                        doOnceStartTimer = false;

                    }
                    else

                    {
                        notDetectingPlayerNear = false;
                        lockTimer += Time.deltaTime;
                            print("wed " + lockTimer);
                    }
                }

            }
            else if(player.GetComponent<PlayerMovementLevel2>().isWalking || player.GetComponent<PlayerMovementLevel2>().isSprinting)
            {
                print("doodee" + Vector3.Distance(eyes.transform.position, player.transform.position));
                    if(Vector3.Distance(eyes.transform.position, player.transform.position) < 18)
                    {
                        if (!animator.GetBool("IsWalking"))
                        {
                            animator.SetBool("IsWalking", true);
                            animator.SetBool("IsIdle", false);
                            animator.SetBool("IsSwimming", false);
                        }
                        if (!doOnceLockPlayer && !lockToOilpool)
                        {
                            agent.SetDestination(player.transform.position);
                            agent.ResetPath();
                            if (!walkSFX.isPlaying)
                                walkSFX.Play();
                            if (!detectSFX.isPlaying)
                                detectSFX.Play();
                            lastTargetPosition = player.transform.position;
                            animator.Play("TrappedInitial", 1, 0f);
                            animator.SetLayerWeight(1, .29f);
                            animBlendTimer = 2.5f;
                            lockToPlayer = true;
                            doOnceLockPlayer = true;

                        }
                        print("wededededed");
                        lockTimer = 0;
                        notDetectingPlayerNear = true;
                        doOnceStartTimer = false;
                    }
            }
            else
            {
                if (!doOnceStartTimer && doOnceLockPlayer)
                {
                    doOnceStartTimer = true;
                }
            }
        }

        foreach(RaycastHit hit in oilHits)
        {
                        if (hit.transform.gameObject.tag == "OilPool")
            {
                lockToOilpool = true;
                if(!doOnceLockOil)
                {
                    detectSFX.Play();
                    doOnceLockOil = true;
                }
                lockToPlayer = false;
                oilLocked = hit.transform.gameObject;
            }
        }

    }

    void LockPlayer()
    {

        NavMeshHit navHit;

        animBlendTimer -= Time.deltaTime;
        agent.speed = 5;

        if (animBlendTimer > 0 && animBlendTimer < .58f)
        {
            agent.SetDestination(player.transform.position);
            animator.SetLayerWeight(1, animBlendTimer / 2);
        }

        //agent.speed = 5f;
        agent.stoppingDistance = 1f;
        if (Vector3.Distance(player.transform.position, lastTargetPosition) > 2f && !hasHitPlayer && lockTimer < 22f)
        {
            if (!animator.GetBool("IsWalking"))
            {
                animator.SetBool("IsWalking", true);
                animator.SetBool("IsIdle", false);
                animator.SetBool("IsSwimming", false);
            }
            agent.SetDestination(player.transform.position);
            if(!walkSFX.isPlaying)
            walkSFX.Play();
            lastTargetPosition = player.transform.position;
        }


        if(agent.hasPath && !agent.pathPending)
        if (agent.remainingDistance < 8.9f && colliderObject.distanceToPlayer <= 3.4f)
        {
            if (!animator.GetBool("IsBiting"))
            {
                    walkSFX.Stop();
                    if (!biteSFX.isPlaying)
                        biteSFX.Play();
                agent.ResetPath();
                animator.SetBool("IsBiting", true);
            }
        }
            else
                animator.SetBool("IsBiting", false);

        if (lockTimer >= 12f)
        {
            print("wedwedwed");
            doOnceLockPlayer = false;
            lockToPlayer = false;
        }

        if (colliderObject.collided)
        {
            print("WOW DUDE");
            animator.SetBool("IsBiting", false);
            player.GetComponent<CollisionForces2>().NormalAttackHitback(transform.GetChild(0).right, 12f, transform.GetChild(0).up, 4f);
            Tween.ShakeLocalPosition(camHolder.transform, strength: new Vector3(.4f, .4f, .4f), duration: .5f, frequency: 40);
            player.GetComponent<PlayerHealth>().TakeDamage(34f);



            if (!leverCheck.finalSegment)
            {
                agent.ResetPath();
                lockToPlayer = false;
            }
            hasHitPlayer = true;

        }

    }

    void LockOilPool()
    {

        NavMeshHit navHit;

        agent.speed = 4f;
        agent.stoppingDistance = 1f;
        agent.SetDestination(oilLocked.transform.position);



        if (Vector3.Distance(transform.GetChild(0).position, oilLocked.transform.position) < 2f)
        {
            if (!detectSFX.isPlaying && !monsterBaited)
                detectSFX.Play();
            agent.ResetPath();
            monsterBaited = true;
        }
        else
            agent.SetDestination(oilLocked.transform.position);

    }

    public void ForceLandJump()
    {
        forceJump = true;
    }

    public Vector3 ClosestLocationFound(List<GameObject> list)
    {
        for(int i = 0; i < list.Count; i++)
        {
            if(i == 0)
            {
                closestDistance = Vector3.Distance(list[i].transform.position, transform.position);
                closestLocation = list[i].transform.position;
            }
            if (Vector3.Distance(list[i].transform.position, transform.position) < closestDistance)
            {
                closestDistance = Vector3.Distance(list[i].transform.position, transform.position);
                closestLocation = list[i].transform.position;
            }
        }
        locationFound = true;
        print("enabling closest location: " + closestLocation);
        return closestLocation;
    }


}
