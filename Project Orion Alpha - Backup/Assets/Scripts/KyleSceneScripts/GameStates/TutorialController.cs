using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public GameController gc;
    public TutorialIState currentState;
    public FirstRoomState firstRoomState = new FirstRoomState();
    public SecondRoomState secondRoomState = new SecondRoomState();
    public ThirdRoomState thirdRoomState = new ThirdRoomState();
    public FieldState fieldState = new FieldState();
    public GunDamage gunDmg;
    // Start is called before the first frame update
    void Start()
    {
        ChangeState(firstRoomState);
    }

    // Update is called once per frame
    void Update()
    {
        if(gunDmg != null)
        if (currentState != fieldState)
            gunDmg.enabled = false;
        else gunDmg.enabled = true;
        if (currentState != null)
        {
            currentState.UpdateState(this);
        }
    }

    public void ChangeState(TutorialIState newState)
    {
        if (currentState != null)
        {
            currentState.OnExit(this);
        }
        currentState = newState;
        currentState.OnEnter(this);
    }

}
public interface TutorialIState
{
    public void OnEnter(TutorialController controller);

    public void UpdateState(TutorialController controller);

    public void OnHurt(TutorialController controller);

    public void OnExit(TutorialController controller);
}