using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoItem : MonoBehaviour
{
    public enum AmmoType { pistol, rifle, Full}
    public AmmoType ammoType = AmmoType.pistol;
    public AudioClip ammoClip;
    public float rotateSpeed;

    private ChangeWeapon changeWeapon;
    private GameManager gameManager;
    private AmmoUpdate ammoUpdate;
    private AudioSource audioSource;
    private GameObject mesh;
    private Weapon weapon;
    private bool gotAmmo;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        ammoUpdate = GameObject.FindGameObjectWithTag("GameController").GetComponent<AmmoUpdate>();
        changeWeapon = gameManager.changeWeapon;
        mesh = transform.GetChild(0).gameObject;
        switch(ammoType)
        {
            case AmmoType.pistol:
                weapon = gameManager.pistol;
                break;
            case AmmoType.rifle:
                weapon = gameManager.m4;
                break;
        }
    }
    private void Update()
    {
        mesh.transform.Rotate(Vector3.forward * rotateSpeed, Space.Self);
    }

    //quando entrar em colisao com o player, addiciona municao as armas.
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && !gotAmmo)
        {
           
            switch (ammoType)
            {
                case AmmoType.pistol:
                    gotAmmo = true;
                    mesh.SetActive(false);
                    weapon.totalBullets += Random.Range(24, 36);
                    //if para só atualizar as municoes no hud se estiver com a arma certa selecionada, nesse caso Pistola
                    if (changeWeapon.index == 0)
                    {
                        ammoUpdate.UpdateAmmoText(weapon.bulletsInMagazine, weapon.totalBullets);
                    }
                    Destroy(gameObject, ammoClip.length + 0.2f);
                    break;
                case AmmoType.rifle:
                    gotAmmo = true;
                    mesh.SetActive(false);
                    weapon.totalBullets += Random.Range(15, 20);
                    //if para só atualizar as municoes no hud se estiver com a arma certa selecionada, nesse caso Fuzil
                    if (changeWeapon.index == 1)
                    {
                        ammoUpdate.UpdateAmmoText(weapon.bulletsInMagazine, weapon.totalBullets);
                    }
                    Destroy(gameObject, ammoClip.length + 0.2f);
                    break;

                    //essa é a municao que fica no campo de treino, tambem aumenta a vida.
                case AmmoType.Full:
                    weapon = gameManager.pistol;
                    weapon.totalBullets = 100;
                    weapon = gameManager.m4;
                    weapon.totalBullets = 100;
                    other.gameObject.GetComponent<StatusPlayer>().health = 100f;
                    if (changeWeapon.index == 1)
                    {
                        weapon = gameManager.m4;
                        ammoUpdate.UpdateAmmoText(weapon.bulletsInMagazine, weapon.totalBullets);
                    }
                    else
                    {
                        weapon = gameManager.pistol;
                        ammoUpdate.UpdateAmmoText(weapon.bulletsInMagazine, weapon.totalBullets);
                    }
                    break;
            }
            
            audioSource.PlayOneShot(ammoClip);
            
        }
    }
}
