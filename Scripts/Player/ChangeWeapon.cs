using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeWeapon : MonoBehaviour
{
    public GameObject[] weapons;
    public Animator[] anims;
    public int index;
    private bool canChange = true;
    public Animator anim;
    public float timeToChange;

    public Recoil recoil;

    private StarterAssetsInputs _input;
    // Start is called before the first frame update
    void Start()
    {
        _input = GameObject.FindGameObjectWithTag("Player").GetComponent<StarterAssetsInputs>();
        GetAnims();
        StartCoroutine(ActiveWeapon(index));
    }

    // Update is called once per frame
    void Update()
    {
        if(_input.scroll != 0 && canChange)
        {
            NextWeapon();
        }
    }
    //Pega os animators das armas que o player tem.
    private void GetAnims()
    {
        anims = new Animator[weapons.Length];
        for (int x=0; x < weapons.Length; x++)
        {
            
            anims[x] = weapons[x].GetComponent<Animator>();
        }
        anim = anims[0];
        anim.runtimeAnimatorController = anims[0].runtimeAnimatorController;
    }
    private void NextWeapon()
    {
        StartCoroutine(TimeWait());
        //verifica se o valor do Input é positivo ou negativo, se positivo, proxima arma, se negativo, arma anterior
        float y = _input.scroll;
        if(y > 0)
        {
            if(weapons.Length > 1)
            {
                if(index < weapons.Length - 1)
                {
                    index++;
                    StartCoroutine( ActiveWeapon(index));
                }
            }
        }
        else if(y < 0)
        {
            if (weapons.Length > 1)
            {
                if (index > 0)
                {
                    index--;
                    StartCoroutine(ActiveWeapon(index));
                }
            }
        }
    }
    //Faz uma animacao de troca, espera o tempo de troca, desativa as armas e ativa a que é seguinte e faz uma animaçao para voltar a posicao de Idle
    private IEnumerator ActiveWeapon(int index)
    {
        anim.CrossFadeInFixedTime("Reload", 0.1f);
        yield return new WaitForSeconds(timeToChange);
        for (int x = 0; x < weapons.Length; x++)
        {
            weapons[x].SetActive(false);
        }
        weapons[index].SetActive(true);
        anim = anims[index];
        anim.CrossFadeInFixedTime("ReloadOut", 0.01f);
        GetRecoil(index);
    }
    private IEnumerator TimeWait()
    {
        canChange = false;
        yield return new WaitForSeconds(1f);
        canChange = true;
    }

    //Pega as caracteristicas de Recuo de cada arma e envia para o script responsavel para aplicar o recuo da arma.
    private void GetRecoil(int indexWeapon)
    {
        Weapon instance = weapons[indexWeapon].GetComponent<Weapon>();
        recoil.AimRecoilX = instance.AimRecoilX;
        recoil.AimRecoilY = instance.AimRecoilY;
        recoil.AimRecoilZ = instance.AimRecoilZ;

        //quando nao usa tiro raycast, o recuo é diferente
        recoil.noRaycast_AimRecoilX = instance.noRaycast_AimRecoilX;
        recoil.noRaycast_AimRecoilY = instance.noRaycast_AimRecoilY;
        recoil.noRaycast_AimRecoilZ = instance.noRaycast_AimRecoilZ;

        recoil.recoilX = instance.recoilX;
        recoil.recoilY = instance.recoilY;
        recoil.recoilZ = instance.recoilZ;

        //quando nao usa tiro raycast, o recuo é diferente
        recoil.noRaycast_recoilX = instance.noRaycast_recoilX;
        recoil.noRaycast_recoilY = instance.noRaycast_recoilY;
        recoil.noRaycast_recoilZ = instance.noRaycast_recoilZ;

        recoil.snappiness = instance.snappiness;
        recoil.returSpeed = instance.returSpeed;
    }
}
