using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGun : MonoBehaviour
{

    [Range(.001f, 1f)]
    public float amount;
    [Range(1f, 30f)]
    public float frequency;
    [Range(0, 100f)]
    public float smooth;

    //sprint
    [Range(.001f, 1f)]
    public float sprintamount;
    [Range(1f, 30f)]
    public float sprintfrequency;
    [Range(0, 100f)]
    public float sprintsmooth;

    float sinAmountY;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
}
