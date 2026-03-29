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

    [SerializeField] float securityValue;

    [System.NonSerialized] public UnityEvent<float, float> Event_AdrenalineValueChanged;
    [System.NonSerialized] public UnityEvent Event_Taunted;
    [System.NonSerialized] public UnityEvent<int> Event_DamageTaken;
    [System.NonSerialized] public UnityEvent<float, float> Event_SecurityRaise;

    private void OnEnable()
    {
        //Theoretical Defaults
        adrenaline = 0;
        adrenaline_max = Adrenaline_Absolute_Min;

        securityValue = 0;


        taunting = false;

        Stats = this;

        hitpoints = Hitpoints_Max;

        if (Event_AdrenalineValueChanged == null)
            Event_AdrenalineValueChanged = new UnityEvent<float, float>();

        if (Event_Taunted == null)
            Event_Taunted = new UnityEvent();

        if (Event_DamageTaken == null)
            Event_DamageTaken = new UnityEvent<int>();

        if (Event_SecurityRaise == null)
            Event_SecurityRaise = new UnityEvent<float, float>();

    }

    private void OnDisable()
    {
        Event_AdrenalineValueChanged.RemoveAllListeners();
        Event_AdrenalineValueChanged = null;

        Event_Taunted.RemoveAllListeners();
        Event_Taunted = null;

        Event_DamageTaken.RemoveAllListeners();
        Event_DamageTaken = null;

        Event_SecurityRaise.RemoveAllListeners();
        Event_SecurityRaise = null;
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

    public void IncreaseSecurity(float value)
    {
        securityValue += value;
        adrenaline_max = securityValue + Adrenaline_Absolute_Min;
        adrenaline_max = Mathf.Clamp(adrenaline_max, 0, Adrenaline_Absolute_Max);

        Event_SecurityRaise.Invoke(securityValue, adrenaline_max);

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
