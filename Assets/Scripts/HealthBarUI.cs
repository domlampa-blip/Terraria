using UnityEngine;
using UnityEngine.UI; // Nutné pro práci se Sliderem

public class HealthBarUI : MonoBehaviour
{
    public Slider slider;
    public PlayerController player;

    void Start()
    {
        if (player != null)
        {
            // Nastavíme maximum podle zdraví z LivingEntity
            slider.maxValue = player.maxHealth;
            slider.value = player.maxHealth;
        }
    }

    void Update()
    {
        if (player != null)
        {
            // Prùbìžnì aktualizujeme bar podle HP hrdiny
            slider.value = player.GetCurrentHealth();
        }
    }
}