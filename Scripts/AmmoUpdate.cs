using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUpdate : MonoBehaviour
{
    public Text ammoText;
    
    

    public void UpdateAmmoText(int currentBullets, int TotalBullets)
    {
        ammoText.text = currentBullets + " / " + TotalBullets;
    }
}
