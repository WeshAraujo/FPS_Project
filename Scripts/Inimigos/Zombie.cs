using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    private float speed;
    private float speedToRun;
    private float attackSpeed;
    public float timeWait;
    private Animator anim;
    private NavMeshAgent agent;
    private GameObject player;
    private bool attacked;

    private StatusEnemy statusEnemy;
    public bool chamou;
    private AudioSource audioSource;
    public AudioClip[] baseClip;
    public AudioClip[] attackClip;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        statusEnemy = GetComponent<StatusEnemy>();
        audioSource = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        SetCharacteristics();
    }

    // Update is called once per frame
    void Update()
    {
        if(!attacked && !statusEnemy.dead)
        {
            Move();
        }
        if(statusEnemy.dead)
        {
            agent.enabled = false; //tem que desabilitar o agent quando morre para ele nao continuar andando ou atrapalhando em outras coisas
        }
    }
    //IA do Inimigo, simplesmente segue o player onde ele for.
    private void Move()
    {
        agent.SetDestination(player.transform.position);
        if(speed < speedToRun)
        {
            anim.SetBool("Walk", true);
        }
        else
        {
            anim.SetBool("Run", true);
        }
        CheckAttack();
    }
   //Checa a distancia, se tiver perto o suficiente, ataca;
    private void CheckAttack()
    {
        if(!chamou)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < 1.7f)
            {
                StartCoroutine(AttackTime());
            }
        }
        
    }
   //Aplica um tempo Idle depois que atacou
    private IEnumerator AttackTime()
    {
        attacked = true;
        chamou = true;
        anim.SetTrigger("Attack");
        int index = Random.Range(0, attackClip.Length);
        audioSource.PlayOneShot(attackClip[index]);
        anim.SetBool("Walk", false);
        anim.SetBool("Run", false);
        agent.isStopped = true;
        Vector3 target = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
        transform.LookAt(target);
        yield return new WaitForSeconds(timeWait);
        agent.isStopped = false;
        attacked = false;
        chamou = false;
    }
    //Randomiza as caracteristicas do zumbi, como velocidade, tempo de ataque e tempo de espera;
    private void SetCharacteristics()
    {
        speed = Random.Range(1f, 10f);
        speedToRun = 5;
        attackSpeed = Random.Range(1f, 5f);
        timeWait = Random.Range(2f, 10f);

        anim.SetFloat("AttackSpeed", attackSpeed);
        agent.speed = speed;
        //serve para mudar a aceleraçao da animaçao de corrida ou caminhada, depende da velocidade que o inimigo tem.
        if(speed < speedToRun)
        {
            anim.SetFloat("SpeedWalk", speed);
        }
        else
        {
            anim.SetFloat("SpeedRun", speed / 5);
        }
        audioSource.clip = baseClip[Random.Range(0, baseClip.Length)];
        audioSource.Play();
    }
}
