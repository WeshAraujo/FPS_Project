using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOnDie : MonoBehaviour
{
    public GameObject pistolAmmo;
    public GameObject rifleAmmo;

    //Quando o inimigo morre é chamado Pelo Script StatusEnemy
    public void DropItem()
    {
        //faz um sorteio e um calculo para probabilidade de dropar algo e se dropar, a probabilidade de ser um item ou outro
        int probability = Random.Range(0, 10);
        if(probability < 4)
        {
            int probabilityAmmo = Random.Range(0, 10);
            if(probabilityAmmo < 7)
            {
                GameObject pistolAmmoGO = Instantiate(pistolAmmo, transform.position, transform.rotation);
            }
            else
            {
                GameObject rifleAmmoGO = Instantiate(rifleAmmo, transform.position, transform.rotation);
            }
        }
    }
}
