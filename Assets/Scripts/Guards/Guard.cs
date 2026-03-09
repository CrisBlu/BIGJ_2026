using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public interface IObserver
{
    void Seeing(bool state, Vector3 position);
}
public class Guard : MonoBehaviour, IObserver
{
    [SerializeField] AISensor Viewer;
    public bool IsAlerted   => timeTillAlert == 0f;
    public Vector3 LastSeenPlayerPos => lastSeenPlayerPos;

    float timeTillAlert;
    bool isSeeingPlayer;
    Vector3 lastSeenPlayerPos;

    void Awake()
    {
        PlayerStats.Stats.Event_Taunted.AddListener(Taunted);
    }


    void Start()
    {
        //Should equal a max
        timeTillAlert = 2f;

        isSeeingPlayer = false;

        Viewer.observer = this;
    }


    void FixedUpdate()
    {
        if(isSeeingPlayer)
        {
            //If time to alert is not 0 deduct time
            timeTillAlert = timeTillAlert > 0f ? timeTillAlert - Time.deltaTime : 0;



            //Increase Player Adrenaline
            if (PlayerStats.Stats)
                PlayerStats.Stats.ShiftAdrenaline();
                
                
        }
        else
        {
            timeTillAlert = timeTillAlert < 2f ? timeTillAlert + Time.deltaTime : 2f;
        }

        if(timeTillAlert == 0f)
        {
            Debug.Log("Wait a minute, at " + lastSeenPlayerPos);
        }
       
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
