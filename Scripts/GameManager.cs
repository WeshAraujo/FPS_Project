using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private StarterAssetsInputs _input;
    public FPSController controller;
    
    public Weapon[] weapons;
    public GameObject[] mirasM4;
    private int indexMira;
    
    public AudioClip zombieAmbienceClip;
    private AudioSource audioSource;

    public bool paused;
    public GameObject panelPause;
    public Text movement;
    public Text shootType;

    //tipos de movimentacao e tiro
    [HideInInspector] public bool raycast = true;
    [HideInInspector] public bool arcade = true;


    [Header("Referencias para outros scripts")]
    public Weapon pistol;
    public Weapon m4;
    public ChangeWeapon changeWeapon;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        _input = GameObject.FindGameObjectWithTag("Player").GetComponent<StarterAssetsInputs>();
        UI_Update();
        //SetCursorState(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(_input.paused)
        {
            Pause();
        }
    }
    private void Pause()
    {
        paused = !paused;
        _input.cursorInputForLook = !paused;
        panelPause.SetActive(paused);
        if (paused)
        {
            SetCursorState(false);
            Selectable selectable = panelPause.GetComponentInChildren<Selectable>(); // Obtenha o primeiro objeto selecionável dentro do novo painel
            if (selectable != null)
            {
                selectable.Select(); // Defina o foco de entrada para o objeto selecionável
            }
            Time.timeScale = 0;
            
            
        }
        else
        {
            Time.timeScale = 1;
            
            SetCursorState(true);
        }
        _input.paused = false;
    }
    public void ChangeMovimentation()
    {
        arcade = !arcade;
        if(arcade)
        {
            controller.moveType = FPSController.MoveType.Arcade;
        }
        else
        {
            controller.moveType = FPSController.MoveType.Real;
        }
        UI_Update();
    }
    public void ChangeFireType()
    {
        raycast = !raycast;
        if(raycast)
        {
            for(int x=0; x<weapons.Length; x++)
            {
                weapons[x].shootMode = Weapon.ShootMode.Raycast;
            }
        }
        else
        {
            for (int x = 0; x < weapons.Length; x++)
            {
                weapons[x].shootMode = Weapon.ShootMode.Bullet;
            }
        }
        UI_Update();
    }
    public void ChangeMira()
    {
        for(int x=0; x< mirasM4.Length;x++)
        {
            mirasM4[x].SetActive(false);
        }
        if(indexMira < mirasM4.Length - 1)
        {
            indexMira++;
        }
        else
        {
            indexMira = 0;
        }
        mirasM4[indexMira].SetActive(true);
    }
    public void UI_Update()
    {
        if(arcade)
        {
            movement.text = "Arcade Movement";
        }
        else
        {
            movement.text = "Semi-Arcade Movement";
        }
        if(raycast)
        {
            shootType.text = "Shooting in raycast";
        }
        else
        {
            shootType.text = "Shooting bullet spawn";
        }
    }
    public void StartZombie()
    {
        ZombieBattle.instance.StartZombie();
        audioSource.clip = zombieAmbienceClip;
        audioSource.Play();
        Pause();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
