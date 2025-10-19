using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public Slider healthSLider;
    [SerializeField] public Slider delayedHealthSlider;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;

    private void Update()
    {
        transform.rotation = mainCamera.transform.rotation;
        transform.position = target.position + offset;
    }

    public void SetSlider(float health)
    {
        healthSLider.value = health;
       
    }

    public void SetSliderMax(float health)
    {
        healthSLider.maxValue = health;
        delayedHealthSlider.maxValue = health;
        SetSlider(health);
    }

}
