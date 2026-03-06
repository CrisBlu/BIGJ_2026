using UnityEngine;

public class Viewer : MonoBehaviour
{
    [HideInInspector] public IObserver observer;
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            observer.Seeing(true, other.transform.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            observer.Seeing(false, Vector3.negativeInfinity);
        }
    }
}
