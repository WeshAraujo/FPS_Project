using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    private StarterAssetsInputs _input;
    private bool isAiming;

    private Vector3 currentRotation;
    private Vector3 targetRotation;

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

    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        _input = GameObject.FindGameObjectWithTag("Player").GetComponent<StarterAssetsInputs>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        isAiming = _input.aiming;
        //Volta a arma para a posiçao inicial
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returSpeed * Time.deltaTime);
        //Calcula e aplica o recuo de forma Interpolada.
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }
    //calcula a quantidade de recuo que sera aplicado. É chamado dentro do Metodo Fire das armas
    public void RecoilFire()
    {
        if(gameManager.raycast)
        {
            if (isAiming)
            {
                targetRotation += new Vector3(AimRecoilX, Random.Range(-AimRecoilY, AimRecoilY), Random.Range(-AimRecoilZ, AimRecoilZ));
            }
            else
            {
                targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
            }
        }
        else
        {
            if (isAiming)
            {
                targetRotation += new Vector3(noRaycast_AimRecoilX, Random.Range(-noRaycast_AimRecoilY, noRaycast_AimRecoilY), Random.Range(-noRaycast_AimRecoilZ, noRaycast_AimRecoilZ));
            }
            else
            {
                targetRotation += new Vector3(noRaycast_recoilX, Random.Range(-noRaycast_recoilY, noRaycast_recoilY), Random.Range(-noRaycast_recoilZ, noRaycast_recoilZ));
            }
        }
        
    }
}
