using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : ScriptableObject
{
    public static PlayerStats Stats;

    //Should be private
    public float adrenaline;
    public float adrenaline_max;

    [SerializeField] float Adrenaline_Absolute_Max;
    [SerializeField] float Adrenaline_Absolute_Min;
    [SerializeField] float Rate_Change_Adr = 1f;

    [HideInInspector] public bool taunting;

    [System.NonSerialized] public UnityEvent<float, float> Event_AdrenalineValueChanged;
    [System.NonSerialized] public UnityEvent Event_Taunted;

    private void OnEnable()
    {
        //Theoretical Defaults
        adrenaline = 0;
        adrenaline_max = 100f;

        taunting = false;

        Stats = this;

        if (Event_AdrenalineValueChanged == null)
            Event_AdrenalineValueChanged = new UnityEvent<float, float>();

        if (Event_Taunted == null)
            Event_Taunted = new UnityEvent();


    }

    private void OnDisable()
    {
        Event_AdrenalineValueChanged.RemoveAllListeners();
        Event_AdrenalineValueChanged = null;

        Event_Taunted.RemoveAllListeners();
        Event_Taunted = null;
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
}
