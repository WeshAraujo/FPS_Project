using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    public GameObject bloodPrefab;
    public GameObject bulletHole;
    public float force;
    public AudioClip[] metalRicochetClip;
    [HideInInspector] public float damage;
    private Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        rb.AddForce(transform.forward * force, ForceMode.Impulse);
    }

    //verifica colisao, se for inimigo, instancia sangue, se for o Target de treino, solta som de metal, se for qualquer outra coisa, apenas instancia uma textura de impacto;
    private void OnCollisionEnter(Collision collision)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.transform.forward, out hit, 1f))
        {
            Debug.Log(hit.transform.name);
            
            if(hit.transform.CompareTag("Enemy"))
            {
                GameObject blood = Instantiate(bloodPrefab, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal), hit.transform);
                hit.transform.GetComponent<StatusEnemy>().ApplyDamage(damage);
            }
            else if (hit.transform.CompareTag("Target"))
            {
                hit.transform.GetComponent<Animator>().SetTrigger("Hit");
                hit.transform.GetComponent<AudioSource>().PlayOneShot(metalRicochetClip[Random.Range(0, metalRicochetClip.Length)]);
                GameObject impact = Instantiate(bulletHole, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal), hit.transform);
            }
            else
            {
                GameObject impact = Instantiate(bulletHole, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal), hit.transform);
            }
            //GameObject sparks = Instantiate(bulletHole, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
        }
        Destroy(gameObject);
    }
}
