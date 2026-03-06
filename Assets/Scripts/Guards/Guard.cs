using UnityEngine;

public interface IObserver
{
    void Seeing(bool state, Vector3 position);
}
public class Guard : MonoBehaviour, IObserver
{
    [SerializeField] Viewer Viewer;

    float timeTillAlert;
    bool isSeeingPlayer;
    Vector3 lastSeenPlayerPos;

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
            timeTillAlert = timeTillAlert > 0f ? timeTillAlert - Time.deltaTime : 0;
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

}
