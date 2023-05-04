using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image healthbarSprite;
    [SerializeField] private float reduceSpeed = 2;
    private float target = 1;
    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        //replace target here with 'healthbarSprite.fillAmount' if the update function below starts giving errrors (and delete the update function)
        target = currentHealth / maxHealth;
    }

    // This update function is meant to give healthbars a smooth transition when changing hp values
    private void Update()
    {
        healthbarSprite.fillAmount = Mathf.MoveTowards(healthbarSprite.fillAmount, target, reduceSpeed * Time.deltaTime);
    }
}

