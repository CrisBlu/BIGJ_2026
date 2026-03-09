using UnityEngine;
using UnityEngine.UI;
public class GuardDetectionUI : MonoBehaviour
{
    public Slider timeSlider;
    public float increaseRate = 0.25f;
    public GameObject MainCharacter;
    public AISensor guardScript;

    public Image flashImage;
    public float flashSpeed = 3f;

    private bool isFlashing = false;

    void Start()
    {
        if (timeSlider != null)
        {
            timeSlider.value = timeSlider.minValue;
            timeSlider.gameObject.SetActive(false);
        }

        if (flashImage != null)
            flashImage.gameObject.SetActive(false);
    }

    void Update()
    {
        if (timeSlider == null || guardScript == null) return;

        bool inSight = guardScript.Objects.Contains(MainCharacter);

        if (inSight)
        {
            timeSlider.gameObject.SetActive(true);
            timeSlider.value += increaseRate * Time.deltaTime;

            if (timeSlider.value >= timeSlider.maxValue)
            {
                timeSlider.value = timeSlider.maxValue;
                timeSlider.gameObject.SetActive(false);
                isFlashing = true;
                flashImage.gameObject.SetActive(true);
            }
        }
        else
        {
            if (isFlashing)
            {
                isFlashing = false;
                flashImage.gameObject.SetActive(false);
            }

            timeSlider.value -= increaseRate * Time.deltaTime;

            if (timeSlider.value <= timeSlider.minValue)
            {
                timeSlider.value = timeSlider.minValue;
                timeSlider.gameObject.SetActive(false);
            }
            else
            {
                timeSlider.gameObject.SetActive(true);
            }
        }

        if (isFlashing && flashImage != null)
        {
            float alpha = Mathf.Abs(Mathf.Sin(Time.time * flashSpeed));
            Color c = flashImage.color;
            c.a = alpha;
            flashImage.color = c;
        }
    }
}
