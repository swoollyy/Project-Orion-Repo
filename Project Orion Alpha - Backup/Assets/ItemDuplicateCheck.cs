using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDuplicateCheck : MonoBehaviour
{

    public List<string> collectedItemTags = new List<string>();
    public List<int> collectedItemCount = new List<int>();

    int notDupeIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void RecordTag(string tag)
    {
        if (!CheckForDups(tag))
        {
            notDupeIndex++;
            print("booooo");
            collectedItemTags[notDupeIndex] = tag;
            collectedItemCount[notDupeIndex] = 1;
            return;
        }
        else
        {
            collectedItemCount[CheckIndex(tag)]++;
            return;
        }
    }

    public bool CheckForDups(string dupTag)
    {
        for (int i = 0; i < collectedItemTags.Count; i++)
        {
            if (dupTag == collectedItemTags[i])
            {
                return true;
                break;
            }
        }
        return false;
    }

    public int CheckIndex(string tag)
    {
        for (int i = 0; i < collectedItemTags.Count; i++)
        {
            if (tag == collectedItemTags[i])
            {
                return i;
                break;
            }
        }
        return -1;
    }

    public bool CheckForMult(string tag)
    {

        int index = -1;

        for (int i = 0; i < collectedItemTags.Count; i++)
        {
            if (tag == collectedItemTags[i])
            {
                index = i;
                break;
            }
        }

        if(collectedItemCount[index] > 1)
        {
            DecreaseItemCount(index);
            return true;
        }
        else
        {
            DecreaseItemCount(index);
            return false;
        }
    }

    public void DecreaseItemCount(int index)
    {
        collectedItemCount[index]--;
        if (collectedItemCount[index] == 0)
        {
            collectedItemTags[index] = null;
            notDupeIndex--;
        }
        return;
    }

}
