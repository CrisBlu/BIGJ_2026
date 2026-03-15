using UnityEngine;
using UnityEngine.UI;

public class HitpointsUI : MonoBehaviour
{
    [SerializeField] GameObject HitpointsParent;
    [SerializeField] GameObject Icon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Populate(PlayerStats.Stats.Hitpoints_Max);

        PlayerStats.Stats.Event_DamageTaken.AddListener(Populate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Populate(int amount)
    {
        foreach(Transform child in HitpointsParent.transform)
        {
            Destroy(child.gameObject);
        }
        for(int i = 0; i < amount; i++)
        {
            GameObject hitpoint = Instantiate(Icon, HitpointsParent.transform);
            hitpoint.transform.localPosition = new Vector3(i * 150, 0, 0);

        }
    }
}
