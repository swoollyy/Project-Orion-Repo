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
    public GameObject oilLocked;

    public bool isOnLand;
    public bool isOnWater;
    public bool isInWater;

    Vector3 randomPosition;
    Vector3 randomPositionLand;
    Vector3 lastPosition;
    Vector3 nextLocation;

    Vector3 jumpStartPosition;
    float jumpProgress;

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

    float waterJumpTime;
    float landJumpTime;

    bool doOnceLand;
    bool doOnceWater;
    bool doOnce;
    bool doOnce2;

    public bool forceJump;

    int rng;

    public water waterScript;
    public LeverCheck leverCheck;
    public GameControllerLevel2 gc;

    [Header("Player Settings")]
    public float value;

    [Header("Jump Timer Values")]
    public float minValue;
    public float maxValue;


    // Start is called before the first frame update
    void Start()
    {
        agent = transform.GetChild(0).GetComponent<NavMeshAgent>();
        path = new NavMeshPath();

        lastPosition = transform.position;

        waterJumpTime = Random.Range(minValue, maxValue);
        landJumpTime = Random.Range(minValue, maxValue);
    }

    // Update is called once per frame
    void Update()
    {



        if(!lockToPlayer)
        DetectPlayer();
        if (!isAbleToJump && !waterJump && !readyToJump)
        {
            jumpProgress = 0f;
            IsOnLandOrWater();
        }
        if (isOnLand && !isAbleToJump && !lockToPlayer)
        {
            agent.updateRotation = false;
            agent.enabled = true;
            jumpToWaterTimer += Time.deltaTime;

            if(!waterJump && isOnLand)
            if(jumpToWaterTimer >= waterJumpTime)
            {
                FindClosestDirection();
                jumpToWaterTimer = 0;
            }

            if(agent.enabled && !agent.pathPending)
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                agent.ResetPath();
                RandomizePointOnLand();
            }

            //monster rotation fix
            if(agent.enabled)
            {
                if (agent.velocity.normalized.magnitude > .4f)
                {
                    transform.GetChild(0).right = agent.velocity.normalized;
                    rightLock = transform.GetChild(0).right;
                }
                else if (agent.velocity.normalized.magnitude <= .4f)
                    transform.GetChild(0).right = rightLock;
            }


        }
        if(isOnWater || isInWater && !readyToJump)
        {
            RandomizePointInWater();
        }

        if((isOnWater || isInWater) && isAbleToJump)
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
            agent.ResetPath();
            LockPlayer();
            lockToPlayer = false;
        }
        if (isOnLand && lockToOilpool)
        {
            LockOilPool();
        }
        if (hasHitPlayer)
        {
            if (leverCheck.finalSegment)
                agent.ResetPath();
            retreatTimer += Time.deltaTime;
            if(retreatTimer >= 1.5f && !leverCheck.finalSegment)
            {
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
        agent.enabled = true;
        agent.speed = 5f;

        float random = 1f;

        if (!animator.GetBool("IsIdle"))
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsIdle", true);
            animator.SetBool("IsSwimming", false);
        }



        RaycastHit hit;
        NavMeshHit navHit;

        do
        {
            int rngInt = Random.Range(1, 5);

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
            !NavMesh.Raycast(transform.position, hitpoint, out navHit, NavMesh.AllAreas) && !NavMesh.CalculatePath(transform.position, hitpoint, NavMesh.AllAreas, path) && path.status == NavMeshPathStatus.PathComplete);

        moveDirection = navHit.position;
        moveTimer += Time.deltaTime;


        if (moveTimer > random)
        {
            agent.SetDestination(moveDirection);
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

        if (!waterLocFound)
        {
            timeElapsed = 0f;
            timer = 0f;
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(randomPosition.x, randomPosition.y + 100, randomPosition.z), -transform.up, out hit, Mathf.Infinity, waterMask))
                hitpoint = hit.point;
            nextLocation = new Vector3(hitpoint.x, hitpoint.y - 10f, hitpoint.z);
            if (Vector3.Distance(transform.position, nextLocation) > 15f)
            waterLocFound = true;
        }
        if(waterLocFound)
        {
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
            }
            Vector3 moveDirection = transform.position - lastPosition;
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 3 * Time.deltaTime);
            if (Vector3.Distance(transform.position, nextLocation) < 7f)
            {
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
        float height = 1f;
        float speed = 1f;


        if (rng == 1)
        {

            if (jumpProgress < 2f)
            {
                isLandJumping = true;

                jumpProgress += Time.deltaTime * speed;
                if (waterScript.isAtFloor)
                {
                    Vector3 mid = Vector3.MoveTowards(transform.position, sphereFloor.position, jumpProgress);
                    mid.y += height * Mathf.Sin(.5f * jumpProgress * Mathf.PI);
                    transform.position = mid;
                }
                else if(waterScript.isAtMiddle)
                {
                    Vector3 mid = Vector3.MoveTowards(transform.position, sphereMid.position, jumpProgress);
                    mid.y += height * Mathf.Sin(.5f * jumpProgress * Mathf.PI);
                    transform.position = mid;
                }
                else
                {
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
                doOnce2 = false;
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
        Vector3 direction = transform.position - mapCenter.position;
        direction.y -= 20f;
        closestDirection = transform.position + direction;
        waterJump = true;


    }

    void JumpToWater()
    {
        agent.enabled = false;
        float height = .5f;
        float speed = 1f;

        print("BOYS");

        if (rng == 1)
        {
            if (jumpProgress < 2f)
            {
                isWaterJumping = true;
                jumpProgress += Time.deltaTime * speed;
                Vector3 previousPosition = transform.position;
                transform.GetChild(0).localPosition = Vector3.zero;
                Vector3 mid = Vector3.MoveTowards(transform.position, closestDirection, jumpProgress);
                sphere.transform.position = closestDirection;
                mid.y += height * Mathf.Sin(.5f * jumpProgress * Mathf.PI);


                transform.position = mid;

                Vector3 movementDirection = (mid - previousPosition);
                Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // Smooth rotation
                transform.GetChild(0).right = transform.forward;
            }
            else
            {
                isOnLand = false;
                isOnWater = true;
                jumpToWaterTimer = 0f;
                readyToJump = false;
                landJumpTime = Random.Range(minValue, maxValue);
                transform.eulerAngles = Vector3.zero;
                transform.GetChild(0).right = transform.forward;
                transform.position = transform.GetChild(0).position;
                transform.GetChild(0).localPosition = Vector3.zero;
                doOnce = false;
                isWaterJumping = false;
                isAbleToJump = false;
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
        RaycastHit hit;

        if(Physics.SphereCast(eyes.transform.position, 10f, transform.GetChild(0).right, out hit, 40f, playerAndOilMask))
        {
            if(hit.transform.gameObject.tag == "Player")
            lockToPlayer = true;
        }

        if(Physics.SphereCast(eyes.transform.position, 15f, transform.GetChild(0).right, out hit, 40f, playerAndOilMask))
        {
                        if (hit.transform.gameObject.tag == "OilPool")
            {
                lockToOilpool = true;
                oilLocked = hit.transform.gameObject;
            }
        }

    }

    void LockPlayer()
    {

        NavMeshHit navHit;

        agent.speed = 5f;
        agent.stoppingDistance = 2f;
            agent.SetDestination(player.transform.position);

        if(Vector3.Distance(transform.GetChild(0).position, player.transform.position) < 8f && agent.enabled)
        {
            print("WOW DUDE");
            player.GetComponent<CollisionForces2>().NormalAttackHitback(transform.GetChild(0).right, 9f, transform.GetChild(0).up, 3f);
            player.GetComponent<PlayerHealth>().TakeDamage(34f);

            if(!leverCheck.finalSegment)
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
            agent.ResetPath();
        }
        else
            agent.SetDestination(oilLocked.transform.position);

    }

    public void ForceLandJump()
    {
        forceJump = true;
    }


}
