using UnityEngine;

public class ExtractionPoint : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.name == "GOLD")
        {
            Debug.Log("WIN!!");
        }
    }
}
