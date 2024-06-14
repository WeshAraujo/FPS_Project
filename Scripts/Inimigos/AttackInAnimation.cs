using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackInAnimation : MonoBehaviour
{
    public float damage;
    public Transform attackPos;
    public float attackRadius;


    //É chamado no frame exato da animaçao
    public void AttackPlayer()
    {
        Collider[] col = Physics.OverlapSphere(attackPos.position, attackRadius);
        foreach(Collider coll in col)
        {
            if(coll.gameObject.CompareTag("Player"))
            {
                Vector3 target = new Vector3(transform.position.x, coll.gameObject.transform.position.y, transform.position.z);
                transform.LookAt(target);
                coll.GetComponent<StatusPlayer>().ApplyDamage(damage);
                Debug.Log("Pegou");
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(attackPos.position, attackRadius);
    }
}
