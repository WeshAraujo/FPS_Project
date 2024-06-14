using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZombieBattle : MonoBehaviour
{
    public GameObject[] zombiePrefab;
    public Transform[] instanceLocal;
    public Text scoreText;
    public float score;
    public float timeToSpawnNextZombie;

    public bool isRead;
    public static ZombieBattle instance;

    public int conter;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isRead)
        {
            StartCoroutine(CooldownZombie());
        }
    }
    private void ChooseZombie()
    {
        int local = Random.Range(0, instanceLocal.Length);
        int typeZombie = Random.Range(0, zombiePrefab.Length);
        GameObject zombie = Instantiate(zombiePrefab[typeZombie], instanceLocal[local].position, instanceLocal[local].rotation);
    }
    private IEnumerator CooldownZombie()
    {
        isRead = false;
        ChooseZombie();
        SetUp();
        yield return new WaitForSeconds(timeToSpawnNextZombie);
        isRead = true;
    }
    private void SetUp()
    {
        conter++;
        if(conter >3)
        {
            conter = 0;
            timeToSpawnNextZombie -= 0.1f;
        }
        if(timeToSpawnNextZombie < 3f)
        {
            timeToSpawnNextZombie = 3.1f;
        }
    }
    public void StartZombie()
    {
        isRead = true;
        scoreText.enabled = true;
        score = 0;
        timeToSpawnNextZombie = 5f;
        conter = 0;
        UpdateScore(0);
    }
    public void UpdateScore(float sc)
    {
        score += sc;
        scoreText.text = "Score " + score; 
    }
}
