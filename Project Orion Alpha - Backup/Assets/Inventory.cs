using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Inventory : MonoBehaviour
{
    public List<GameObject> myInventory = new List<GameObject>(5);
    public List<GameObject> weaponInventory = new List<GameObject>(2);
    public Image[] inventorySlots = new Image[5];
    public Image[] weaponSlots = new Image[2];
    public TMP_Text[] inventorySlotName = new TMP_Text[5];
    public TMP_Text[] weaponSlotName = new TMP_Text[2];

    public WhatAmILookingAt waila;
    public MoveGun gunBob;
    public PlayerMovement playerMovement;
    public GameController gc;

    public int totalCollectedItems = -1;
    public int totalCollectedWeapons = -1;

    public KeyCode weaponSlot1 = KeyCode.Alpha1;
    public KeyCode weaponSlot2 = KeyCode.Alpha2;
    public KeyCode throwKey = KeyCode.F;


    public GameObject knifeItem;
    public GameObject knifeHands;
    public GameObject gunItem;
    public GameObject gunHands;
    public GameObject itemHands;
    public GameObject leftHand;
    public GameObject currentHeldItem;
    public GameObject currentHeldWeapon;
    public GameObject throwArea;

    public bool isHoldingGun;
    public bool isHoldingKnife;
    public bool isHoldingItem;
    public bool isHoldingFruit;
    public bool isHoldingLizard;
    public bool isHoldingWeapon;
    public bool isThrowingItem;
    public bool isThrowingWeapon;



    public bool collectedGun;
    public bool collectedKnife;

    public bool hasPickedUpKnife;
    public bool hasPickedUpGun;
    bool holdSlot1;

    public bool hasTamed;
    public GameObject lizzyFriend;
    public Animator gunAnim;
    public Animator knifeAnim;

    Camera cam;

    public AudioSource equipSFX;

    public bool removeTrap;

    public GameObject burrowTrack;

    void Start()
    {
        GetComponent<BoxCollider>().enabled = false;
        cam = Camera.main;
    }
    void Update()
    {
        burrowTrack.transform.position = new Vector3(transform.position.x, -18f, transform.position.z);

        if (gunItem.GetComponent<Item>().isCollected)
        {
            isHoldingGun = gunHands.activeSelf;
            gunAnim.enabled = true;
        }
        else
        {
            isHoldingGun = false;
            gunAnim.enabled = false;
        }
        if (knifeItem.GetComponent<Item>().isCollected)
        {
            isHoldingKnife = knifeHands.activeSelf;
            knifeAnim.enabled = true;
        }
        else
        {
            isHoldingKnife = false;
            knifeAnim.enabled = false;
        }
        if (waila.currentObject != null && Input.GetKeyDown(gc.pickupKey))
        {
            if (waila.currentObject.tag == "Gun")
                hasPickedUpGun = true;
            else if (waila.currentObject.tag == "Knife")
                hasPickedUpKnife = true;
            else if (waila.currentObject.name == "Trap")
            {
                if (waila.currentObject.GetComponent<TrapObject>().isOpen)
                    AddItem(waila.currentObject);
                return;
            }
            else if (isHoldingItem)
                return;

            if(waila.currentObject.GetComponent<Item>() != null)
            AddItem(waila.currentObject);

        }

        if (Input.GetKeyDown(weaponSlot1) && !isHoldingLizard)
        {
            if(isHoldingItem)
            {
                for(int i = 0; i < myInventory.Count; i++)
                {
                    if(currentHeldItem.name == myInventory[i].name)
                    {
                        PutAwayItem(i);
                        break;
                    }
                }
            }
            if(weaponInventory[0].tag == "Knife")
            {
                if (!isHoldingGun && !isHoldingKnife)
                    HoldKnife();
                else if (isHoldingGun)
                {
                    PutAwayGun();
                    HoldKnife();
                }
                else PutAwayKnife();
            }
            else if (weaponInventory[0].tag == "Gun")
            {
                if (!isHoldingGun && !isHoldingKnife)
                    HoldGun();
                else if (isHoldingKnife)
                {
                    PutAwayKnife();
                    HoldGun();
                }
                else PutAwayGun();
            }

        }
        if (Input.GetKeyDown(weaponSlot2) && !isHoldingLizard)
        {
            if (isHoldingItem)
            {
                for (int i = 0; i < myInventory.Count; i++)
                {
                    if (currentHeldItem.name == myInventory[i].name)
                    {
                        PutAwayItem(i);
                        break;
                    }
                }
            }
            if (weaponInventory[1].tag == "Knife")
            {
                if (!isHoldingGun && !isHoldingKnife)
                    HoldKnife();
                else if (isHoldingGun)
                {
                    PutAwayGun();
                    HoldKnife();
                }
                else PutAwayKnife();
            }
            else if (weaponInventory[1].tag == "Gun")
            {
                if (!isHoldingGun && !isHoldingKnife)
                    HoldGun();
                else if (isHoldingKnife)
                {
                    PutAwayKnife();
                    HoldGun();
                }
                else PutAwayGun();
            }

        }


        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (myInventory[0] != null)
            {
                if (isHoldingGun)
                {
                    PutAwayGun();
                }
                else if (isHoldingKnife)
                {
                    PutAwayKnife();
                }
                HoldItem(0);
            }

        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (myInventory[1] != null)
            {
                if (isHoldingGun)
                {
                    PutAwayGun();
                }
                else if (isHoldingKnife)
                {
                    PutAwayKnife();
                }
                HoldItem(1);
            }

        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (myInventory[2] != null)
            {
                if (isHoldingGun)
                {
                    PutAwayGun();
                }
                else if (isHoldingKnife)
                {
                    PutAwayKnife();
                }
                HoldItem(2);
            }

        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (myInventory[3] != null)
            {
                if (isHoldingGun)
                {
                    PutAwayGun();
                }
                else if (isHoldingKnife)
                {
                    PutAwayKnife();
                }
                HoldItem(3);
            }
        }

        print("collected items? " + totalCollectedItems);

        if(currentHeldItem != null)
        if (currentHeldItem.tag == "Lizard")
            isHoldingLizard = true;
        else
            isHoldingLizard = false;




        if (removeTrap)
        {
            for(int i = 0; i < myInventory.Count; i++)
            {
                if (myInventory[i].name == "Trap" || myInventory[i].name == "Trap(Clone)")
                {
                    RemoveItem(i);
                    removeTrap = false;
                    break;
                }
            }
        }



        if (Input.GetKeyDown(gc.throwKey) && isHoldingWeapon)
        {
            ThrowWeapon(currentHeldWeapon);
        }
        else if(Input.GetKeyDown(gc.throwKey) && isHoldingItem)
        {
            ThrowItem(currentHeldItem);
        }



        if (!knifeHands.activeSelf && !gunHands.activeSelf)
            isHoldingWeapon = false;


    }

    public void AddItem(GameObject obj)
    {
        obj.GetComponent<Item>().isCollected = true;
        obj.transform.parent = leftHand.transform;

        if (hasPickedUpGun)
        {
            collectedGun = true;
            totalCollectedWeapons++;
            weaponInventory[totalCollectedWeapons] = obj;
            if(isHoldingKnife)
            {
                PutAwayKnife();
                HoldGun();
            }
            else
                HoldGun();
            hasPickedUpGun = false;
        }
        else if(hasPickedUpKnife)
        {
            collectedKnife = true;
            totalCollectedWeapons++;
                weaponInventory[totalCollectedWeapons] = obj;
            if(isHoldingGun)
            {
                PutAwayGun();
                HoldKnife();
            }
            else
                HoldKnife();
                hasPickedUpKnife = false;
        }
        else if(obj.GetComponent<Item>().canBeCollected)
        {

            if (!GetComponent<ItemDuplicateCheck>().CheckForDups(obj.tag))
            {
                totalCollectedItems++;
            }

            GetComponent<ItemDuplicateCheck>().RecordTag(obj.tag);


            if (obj.tag == "Trap")
            {
                obj.GetComponent<MeshCollider>().enabled = false;
                obj.GetComponent<TrapObject>().enabled = false;
            }
            if(obj.tag == "Lizard")
            {
                obj.GetComponent<Collider>().enabled = false;
                myInventory[totalCollectedItems] = obj;
                inventorySlotName[totalCollectedItems].text = obj.name;
                HoldItem(totalCollectedItems);
            }
            myInventory[totalCollectedItems] = obj;
            inventorySlotName[totalCollectedItems].text = obj.name;
            if(obj.tag != "Lizard")
            PutAwayItem(totalCollectedItems);
        }
    }

    public void HoldGun()
    {
            gunHands.SetActive(true);
            gunItem.SetActive(false);
            gunHands.transform.localPosition = new Vector3(0.028f, -0.631f, 0f);
            //gunHands.transform.localScale = new Vector3(.83f, .83f, .83f);
            //gun.GetComponent<Rigidbody>().useGravity = false;
            isHoldingWeapon = true;
            isHoldingGun = true;
            currentHeldWeapon = gunHands;
            GetComponent<BoxCollider>().enabled = true;
            GetComponent<BoxCollider>().size = new Vector3(.31f, .28f, .68f);
            itemHands.SetActive(false);
            if (isHoldingItem)
            {
                for (int i = 0; i < myInventory.Count; i++)
                {
                    if (myInventory[i].activeSelf)
                        PutAwayItem(i);
                    break;
                }
            }

    }
    public void HoldKnife()
    {
            if (!equipSFX.isPlaying)
                equipSFX.Play();
            knifeItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            knifeHands.SetActive(true);
            knifeItem.SetActive(false);
            knifeHands.transform.localPosition = new Vector3(0f, -0.63f, 0.28f);
            knifeItem.transform.localPosition = new Vector3(-.206f, -0.501f, 0.988f);
            knifeItem.GetComponent<Rigidbody>().useGravity = false;
            isHoldingWeapon = true;
            isHoldingKnife = true;
            currentHeldWeapon = knifeItem;
            GetComponent<BoxCollider>().enabled = true;
            GetComponent<BoxCollider>().size = new Vector3(1f, .5f, .4f);
            itemHands.SetActive(false);
            if (isHoldingItem)
            {
                for (int i = 0; i < myInventory.Count; i++)
                {
                    if (myInventory[i].activeSelf)
                        PutAwayItem(i);
                    break;
                }
            }

    }
    public void HoldItem(int i)
    {
            bool sameIndex = false;
            if (currentHeldItem != null)
                for (int j = 0; j < myInventory.Count; j++)
                {
                    if (myInventory[j] != null)
                        if (myInventory[j].activeSelf)
                        {
                            PutAwayItem(j);
                            if (j == i)
                                sameIndex = true;
                            break;
                        }
                }
            if (!sameIndex)
            {
                if (myInventory[i].name == "Trap")
                    myInventory[i].transform.localScale = new Vector3(.151f, .151f, .151f);
                else if (myInventory[i].name == "Wistiria")
                {
                    myInventory[i].transform.localScale = new Vector3(0.2088065f, 0.2088065f, 0.2088065f);
                    isHoldingFruit = true;
                }

                myInventory[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                itemHands.SetActive(true);
                myInventory[i].SetActive(true);
                inventorySlotName[i].text = myInventory[i].name;
                myInventory[i].transform.localPosition = new Vector3(-0.12f, 0.106f, -0.043f);
                currentHeldItem = myInventory[i];
            if(currentHeldItem.tag == "Lizard")
            {
                if (isHoldingGun)
                    PutAwayGun();
                else if(isHoldingKnife)
                    PutAwayKnife();
            }
            isHoldingItem = true;
            }

    }

    /*public void GunBob()
    {
        Vector3 inputVector = new Vector3(Input.GetAxisRaw("Vertical"), 0f, Input.GetAxisRaw("Horizontal"));
        if (inputVector.magnitude > 0)
        {
            Vector3 pos = Vector3.zero;
            if (playerMovement.walkBob)
            {

            }
            else if (playerMovement.crouchBob)
            {
                gunBob.amount *= .5f;
                gunBob.frequency *= .5f;
                gunBob.smooth *= .5f;
            }
            else if (playerMovement.sprintBob)
            {
                gunBob.amount = gunBob.sprintamount;
                gunBob.frequency = gunBob.sprintfrequency;
                gunBob.smooth = gunBob.sprintsmooth;
            }
            pos.y += Mathf.Lerp(pos.y, Mathf.Sin(Time.time * gunBob.frequency) * gunBob.amount, gunBob.smooth * Time.deltaTime);
            pos.x += Mathf.Lerp(pos.x, Mathf.Cos(Time.time * gunBob.frequency / 2f) * gunBob.amount, gunBob.smooth * Time.deltaTime);
            gun.transform.localPosition = new Vector3(0.3427277f + pos.x, -0.2718974f + pos.y, 0.65f);
        }
    }*/
    public void PutAwayGun()
    {
        gunHands.SetActive(false);
        isHoldingGun = false;
        GetComponent<BoxCollider>().enabled = false;
        currentHeldWeapon = null;
    }
    public void PutAwayKnife()
    {
        equipSFX.Stop();
        knifeHands.SetActive(false);
        isHoldingKnife = false;
        GetComponent<BoxCollider>().enabled = false;
        currentHeldWeapon = null;

    }
    public void PutAwayItem(int i)
    {
        if(myInventory[i].GetComponent<ColliderEnable>() != null)
        myInventory[i].GetComponent<ColliderEnable>().disableCollider = true;
        myInventory[i].SetActive(false);
        itemHands.SetActive(false);
        isHoldingItem = false;
        isHoldingFruit = false;
        currentHeldItem = null;
    }

    public void RemoveItem(int i)
    {
        if (!GetComponent<ItemDuplicateCheck>().CheckForMult(currentHeldItem.tag))
        {
            currentHeldItem.SetActive(false);
            myInventory[i] = null;
            inventorySlotName[i].text = "";

            int itemIndex = 0;

            itemIndex = i;
            totalCollectedItems = itemIndex - 1;


            currentHeldItem = null;
            isHoldingItem = false;
            return;
        }
        else
        {
            print("you still have some!");
            return;
        }

    }

    public void ThrowItem(GameObject obj)
    {

        GameObject instObj = Instantiate(obj, obj.transform.position, obj.transform.rotation);
        int DefaultLayer = LayerMask.NameToLayer("Default");

        for (int i = 0; i < myInventory.Count; i++)
        {
            if(instObj.tag == myInventory[i].tag)
            {
                print("wowow");
                RemoveItem(i);
                break;
            }
        }

        if(obj.tag == "Lizard")
        {
            if(gc.scorpEnableTimer < gc.wakeUpTime)
                gc.scorpEnableTimer = 0;
            instObj.name = "Sang Sang (Dead)";
            isHoldingLizard = false;
        }

        isThrowingItem = true;
        instObj.GetComponent<Item>().isCollected = false;
        instObj.GetComponent<Rigidbody>().useGravity = true;
        instObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        instObj.GetComponent<Collider>().enabled = true;
        if (instObj.GetComponent<Animator>() != null)
        {
            instObj.GetComponent<Animator>().enabled = false;
        }
        instObj.layer = DefaultLayer;
        instObj.GetComponent<Rigidbody>().AddForce(Camera.main.transform.up * 1f + Camera.main.transform.forward * 3f, ForceMode.Impulse);

    }
    public void ThrowWeapon(GameObject obj)
    {
        GameObject thrownWeapon;
        bool throwingKnife = false;
        bool throwingGun = false;
        int weaponIndex = 0;
        int DefaultLayer = LayerMask.NameToLayer("Default");

        isThrowingWeapon = true;



        if (obj.tag == "Knife")
        {
            thrownWeapon = knifeItem;
            throwingKnife = true;
            throwingGun = false;
        }
        else
        {
            thrownWeapon = gunItem;
            throwingGun = true;
            throwingKnife = false;
        }

        weaponIndex = weaponInventory.IndexOf(thrownWeapon);
        totalCollectedWeapons = weaponIndex - 1;


        if (throwingGun)
        {
            GetComponent<BoxCollider>().enabled = false;
            gunHands.SetActive(false);
            gunItem.SetActive(true);
        }
        else
        {
            knifeHands.SetActive(false);
            knifeItem.SetActive(true);
        }

        thrownWeapon.transform.parent = null;
        int removedSlot = 0;
        for (int i = 0; i < weaponInventory.Count; i++)
        {
            if (weaponInventory[i].tag == thrownWeapon.tag)
            {
                removedSlot = i;
                break;
            }
        }
        weaponInventory[removedSlot] = null;
        weaponSlotName[removedSlot].text = null;
        thrownWeapon.GetComponent<Item>().isCollected = false;
        thrownWeapon.GetComponent<Rigidbody>().useGravity = true;
        thrownWeapon.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        if (thrownWeapon.GetComponent<Animator>() != null)
        {
            thrownWeapon.GetComponent<Animator>().enabled = false;
        }
        thrownWeapon.layer = DefaultLayer;
        thrownWeapon.GetComponent<Rigidbody>().AddForce(Camera.main.transform.up * 1f + Camera.main.transform.forward * 3f, ForceMode.Impulse);
    }
}
