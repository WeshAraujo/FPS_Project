using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    
    public enum ShootMode { Raycast, Bullet }
    [Header("First Step")]
    public ShootMode shootMode = ShootMode.Raycast;
    public Transform shootMiddlePoint;
    public Transform shootLocalPoint;
    public GameObject bulletHole;
    public GameObject bloodPrefab;
    public Recoil recoil;
    [Header("Effects")]
    public ParticleSystem smokeEffect;
    public ParticleSystem fireEffect;

    [Header("Weapons Atribute")]
    public float damage;
    [Tooltip("A distancida máxima que a bala vai chegar (só se aplica ao modo Raycast)")]
    public float range = 100f;
    public int totalBullets;
    public int magazineSize = 30;
    public int bulletsInMagazine;
    public float fireRate = 0.1f;
    public float fireTime;
    public float reloadTime = 2f;
    public float reloadConter;

    //hipFire Recoil
    [Header("HipFire Recoil")]
    [SerializeField] public float recoilX;
    [SerializeField] public float recoilY;
    [SerializeField] public float recoilZ;

    [Space(5)]

    [SerializeField] public float noRaycast_recoilX;
    [SerializeField] public float noRaycast_recoilY;
    [SerializeField] public float noRaycast_recoilZ;

    //Aim Recoil
    [Header("Aim Recoil")]
    [SerializeField] public float AimRecoilX;
    [SerializeField] public float AimRecoilY;
    [SerializeField] public float AimRecoilZ;

    [Space(5)]

    [SerializeField] public float noRaycast_AimRecoilX;
    [SerializeField] public float noRaycast_AimRecoilY;
    [SerializeField] public float noRaycast_AimRecoilZ;

    [Space(5)]

    public float snappiness; //velocidade com que o pulo da arma é dado
    public float returSpeed; //velocidade com que a arma volta para o lugar

    public bool isReloading;

    [Header("Aim")]
    public Vector3 aimPos;
    public float aimSpeed;
    private Vector3 originalPos;

    [Header("Audios")]
    public AudioClip fireClip;
    public AudioClip reloadClip;
    public AudioClip emptyClip;
    public AudioClip[] metalRicochetClip;
    private AudioSource source;
    private bool canPlayEmpty = true;

    [Header("Bullet Mode Especificidades")]
    public GameObject bulletPrefab;
    private GameObject player;
    private StarterAssetsInputs _input; //É o script que tem todos os métodos de Input;
    private Animator anim;
    private FPSController fPSController;
    private AmmoUpdate ammoUpdate;
    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        _input = player.GetComponent<StarterAssetsInputs>();
        bulletsInMagazine = magazineSize;
        anim = GetComponent<Animator>();
        originalPos = transform.localPosition;
        fPSController = player.GetComponent<FPSController>();
        source = player.GetComponent<AudioSource>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        ammoUpdate = GameObject.FindGameObjectWithTag("GameController").GetComponent<AmmoUpdate>();
        ammoUpdate.UpdateAmmoText(bulletsInMagazine, totalBullets);
    }
    private void OnEnable()
    {
        ammoUpdate = GameObject.FindGameObjectWithTag("GameController").GetComponent<AmmoUpdate>();
        ammoUpdate.UpdateAmmoText(bulletsInMagazine, totalBullets);
    }

    // Update is called once per frame
    void Update()
    {
        //Verifica se Input do Tiro esta sendo apertado
        if(_input.shooting)
        {
            if(bulletsInMagazine > 0)
            {
                Fire();
            }
            
        }
        //recarrega a arma automaticamente se as municoes acabarem
        if(bulletsInMagazine <= 0 && !isReloading )
        {
            Reload();
        }
        //recarrega a arma caso aperte o botao correspondente
        if(_input.reload && !isReloading && magazineSize != bulletsInMagazine)
        {
            Reload();
        }

        //Calcula o tempo que a arma pode atirar novamente logo apos ter atirado uma vez
        if(fireTime < fireRate)
        {
            fireTime += Time.deltaTime;
        }
        
        if (isReloading)
        {
            reloadConter += Time.deltaTime;
        }
        if (reloadConter > reloadTime)
        {
            isReloading = false;
            _input.reload = false;
            anim.CrossFadeInFixedTime("ReloadOut", 0.1f);
            reloadConter = 0;
        }

        ToAim();
    }
    private void Fire()
    {
        //Se qualquer uma dessas condicoes nao forem atendidas, nao será executado o tiro
        if(fireTime < fireRate || isReloading || bulletsInMagazine <= 0 || _input.sprint || gameManager.paused)
        {
            return;
        }
        source.PlayOneShot(fireClip);

        switch (shootMode)
        {
            //verifica colisao, se for inimigo, instancia sangue, se for o Target de treino, solta som de metal, se for qualquer outra coisa, apenas instancia uma textura de impacto;
            case ShootMode.Raycast: 
                RaycastHit hit;
                if(Physics.Raycast(shootMiddlePoint.position, shootMiddlePoint.transform.forward, out hit, range))
                {
                    if(hit.transform.CompareTag("Enemy"))
                    {
                        GameObject blood = Instantiate(bloodPrefab, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal), hit.transform);
                        hit.transform.GetComponent<StatusEnemy>().ApplyDamage(damage);
                    }
                    else if(hit.transform.CompareTag("Target"))
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
                break;
                //Apenas instancia a municao e ajusta o dano da municao. Nesse modo de tiro, a origen do spawn do projetil é diferente, apenas por questão de estética
            case ShootMode.Bullet:
                if(_input.aiming)
                {
                    GameObject bullet = Instantiate(bulletPrefab, shootMiddlePoint.position, shootMiddlePoint.rotation);
                    bullet.GetComponent<Bullet>().damage = damage;
                    Destroy(bullet, 5f);
                }
                else
                {
                    GameObject bullet = Instantiate(bulletPrefab, shootLocalPoint.position, shootLocalPoint.rotation);
                    bullet.GetComponent<Bullet>().damage = damage;
                    Destroy(bullet, 5f);
                }
                break;
        }
        recoil.RecoilFire();
        anim.CrossFadeInFixedTime("Fire", 0.01f);
        bulletsInMagazine--;
        fireTime = 0f;

        smokeEffect.Play();
        fireEffect.Play();

        ammoUpdate.UpdateAmmoText(bulletsInMagazine, totalBullets);
    }
    private void Reload()
    {
        if(totalBullets <= 0 || gameManager.paused)
        {
            if(canPlayEmpty && _input.shooting)
            {
                StartCoroutine(PlayEmpytSound());
            }
            return;
        }
        source.PlayOneShot(reloadClip);
        isReloading = true;
        int bulletsToLoad = magazineSize - bulletsInMagazine;
        int bulletsToDeduct;
        if(totalBullets >= bulletsToLoad)
        {
            bulletsToDeduct = bulletsToLoad;
        }
        else
        {
            bulletsToDeduct = totalBullets;
        }
        totalBullets -= bulletsToDeduct;
        bulletsInMagazine += bulletsToDeduct;

        anim.CrossFadeInFixedTime("Reload", 1f);
        ammoUpdate.UpdateAmmoText(bulletsInMagazine, totalBullets);
    }
    //leva o objeto ao centro da camera para simular que está mirando.
    public void ToAim()
    {
        if(_input.aiming && !isReloading)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, aimPos, Time.deltaTime * aimSpeed);
            anim.SetBool("Aim", true);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPos, Time.deltaTime * aimSpeed *2);
            anim.SetBool("Aim", false);
        }
    }

    private IEnumerator PlayEmpytSound()
    {
        canPlayEmpty = false;
        source.PlayOneShot(emptyClip);
        yield return new WaitForSeconds(fireRate);
        canPlayEmpty = true;
    }
}
