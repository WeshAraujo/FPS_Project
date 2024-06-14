using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealth : MonoBehaviour
{
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Image bloodImg;
    [SerializeField] private Image redImage;
    [SerializeField] private int speed;
    private StatusPlayer statusPlayer;
    [HideInInspector] public bool read = false;
    // Start is called before the first frame update
    void Start()
    {
        healthBarImage.color = Color.green;
        statusPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<StatusPlayer>();
    }

    private void LateUpdate()
    {
        float healthPercent = (float) statusPlayer.health / 100;
        healthBarImage.fillAmount = Mathf.Lerp(healthBarImage.fillAmount, healthPercent, Time.deltaTime * speed);

        if (statusPlayer.health <= 20)
        {
            healthBarImage.color = Color.red;
        }
        else
        {
            healthBarImage.color = Color.green;
        }
        StartCoroutine(DanoUi());
    }
    public IEnumerator DanoUi()
    {
        if (read)
        {
            bloodImg.fillAmount = Mathf.Lerp(bloodImg.fillAmount, 1, Time.deltaTime * speed);
            redImage.enabled = true;
            yield return new WaitForSeconds(2f);
            redImage.enabled = false;
            bloodImg.fillAmount = Mathf.Lerp(bloodImg.fillAmount, 0, Time.deltaTime * speed);
            read = false;
        }
        
    }
}
