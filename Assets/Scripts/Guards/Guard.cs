using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public interface IObserver
{
    void Seeing(bool state, Vector3 position);
}
public class Guard : MonoBehaviour, IObserver
{
    [SerializeField] AISensor Viewer;
    [SerializeField] GuardDetectionUI UI;
    public bool IsAlerted   => timeTillAlert == 0f;
    public Vector3 LastSeenPlayerPos => lastSeenPlayerPos;

    public float TIME_TILL_ALERT = 2;
    public float timeTillAlertFloor;
    public float timeTillAlert;
    bool isSeeingPlayer;
    Vector3 lastSeenPlayerPos;



    void Start()
    {
        PlayerStats.Stats.Event_Taunted.AddListener(Taunted);
        PlayerStats.Stats.Event_SecurityRaise.AddListener(RaiseAwareness);
        UI.attachedGuard = this;
        //Should equal a max
        timeTillAlert = TIME_TILL_ALERT;

        isSeeingPlayer = false;

        Viewer.observer = this;

        timeTillAlertFloor = TIME_TILL_ALERT;
    }


    void FixedUpdate()
    {
        if(isSeeingPlayer)
        {
            //If time to alert is not 0 deduct time
            timeTillAlert = timeTillAlert > 0f ? timeTillAlert - Time.deltaTime : 0f;




                
                
        }
        else
        {
            timeTillAlert = timeTillAlert < timeTillAlertFloor ? timeTillAlert + Time.deltaTime : timeTillAlertFloor;
        }

        if(timeTillAlert == 0f)
        {
            PlayerStats.Stats.IncreaseSecurity(.2f);
            //Make less segmeneted
            Vector3 LookHereDumbass = lastSeenPlayerPos;

            LookHereDumbass.y = transform.position.y;
            transform.LookAt(LookHereDumbass);
        }
       
    }

    public void RaiseAwareness(float security, float trash)
    {
        timeTillAlertFloor = TIME_TILL_ALERT - (security / 100);
        Debug.Log(timeTillAlertFloor);
    }

    public void Seeing(bool state, Vector3 position)
    {
        isSeeingPlayer = state;

        if(isSeeingPlayer)
        {
            lastSeenPlayerPos = position;
        }
    }


    public void Taunted()
    {
        if(isSeeingPlayer)
            PlayerStats.Stats.AddAdrenaline(20f);

    }

}
