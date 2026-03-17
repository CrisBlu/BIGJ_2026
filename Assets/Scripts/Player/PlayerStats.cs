using UnityEngine;
using UnityEngine.Events;

using UnityEngine.SceneManagement;

public class PlayerStats : ScriptableObject
{
    public static PlayerStats Stats;

    [SerializeField] public int Hitpoints_Max = 3;
    private int hitpoints;

    //Should be private
    public float adrenaline;
    public float adrenaline_max;

    [SerializeField] float Adrenaline_Absolute_Max;
    [SerializeField] float Adrenaline_Absolute_Min;
    [SerializeField] float Rate_Change_Adr = 1f;

    [HideInInspector] public bool taunting;

    [System.NonSerialized] public UnityEvent<float, float> Event_AdrenalineValueChanged;
    [System.NonSerialized] public UnityEvent Event_Taunted;
    [System.NonSerialized] public UnityEvent<int> Event_DamageTaken;

    private void OnEnable()
    {
        //Theoretical Defaults
        adrenaline = 0;
        adrenaline_max = 100f;

        taunting = false;

        Stats = this;

        hitpoints = Hitpoints_Max;

        if (Event_AdrenalineValueChanged == null)
            Event_AdrenalineValueChanged = new UnityEvent<float, float>();

        if (Event_Taunted == null)
            Event_Taunted = new UnityEvent();

        if (Event_DamageTaken == null)
            Event_DamageTaken = new UnityEvent<int>();


    }

    private void OnDisable()
    {
        Event_AdrenalineValueChanged.RemoveAllListeners();
        Event_AdrenalineValueChanged = null;

        Event_Taunted.RemoveAllListeners();
        Event_Taunted = null;

        Event_DamageTaken.RemoveAllListeners();
        Event_DamageTaken = null;
    }

    public bool ShiftAdrenaline(float multiplier = 1f)
    {
        adrenaline += Rate_Change_Adr * multiplier;

        if (adrenaline <= 0 || adrenaline >= adrenaline_max)
        {
            adrenaline = Mathf.Clamp(adrenaline, 0, adrenaline_max);
            return false;
        }

        
        Event_AdrenalineValueChanged.Invoke(adrenaline, adrenaline_max);
        return true;
    }

    public void AddAdrenaline(float value)
    {
        adrenaline += value;

        adrenaline = Mathf.Clamp(adrenaline, 0, adrenaline_max);



        Event_AdrenalineValueChanged.Invoke(adrenaline, adrenaline_max);
    }

    public void DealDamage()
    {
        if(hitpoints <= 0)
        {

            SceneManager.LoadScene(0);
        }

        hitpoints -= 1;

        Event_DamageTaken.Invoke(hitpoints);
    }
}
