using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEnemy : MonoBehaviour
{
    public float health;
    public bool dead;
    private Animator anim;
    private AudioSource audioSource;
    public AudioClip deathClip;
    private DropOnDie dropScript;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        health = Random.Range(10, 100);
        audioSource = GetComponent<AudioSource>();
        dropScript = GetComponent<DropOnDie>();
    }

   //Responsável por aplicar dano no Inimigo, é chamado pelas classes "Weapon" ou "Bullet" Depende do tipo de tiro que esta sendo usado.
    public void ApplyDamage(float damage)
    {
        health -= damage;
        ZombieBattle.instance.UpdateScore(10);
        if(health <=0 && !dead)
        {
            dead = true;
            dropScript.DropItem();
            anim.SetTrigger("Death");
            Destroy(gameObject, 10f);
            ZombieBattle.instance.UpdateScore(100);
            audioSource.Stop();
            audioSource.PlayOneShot(deathClip);
        }
    }
}
