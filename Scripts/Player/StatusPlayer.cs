using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatusPlayer : MonoBehaviour
{
    public float health;
    private UIHealth uIHealth;
    // Start is called before the first frame update
    void Start()
    {
        uIHealth = GameObject.FindGameObjectWithTag("GameController").GetComponent<UIHealth>();
    }

    public void ApplyDamage(float damage)
    {
        health -= damage;
        if(health <=0)
        {
            SceneManager.LoadScene(0);
        }
        uIHealth.read = true;
    }
}
