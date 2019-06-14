using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    public float health = 50f;
    public Text healthTXT;

    public void Awake()
    {
        healthTXT.text = health.ToString();
    }

    public void  Damage(float amount)
    {
        healthTXT.text = health.ToString();
        health -= amount;
        if (health <= 0f)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
