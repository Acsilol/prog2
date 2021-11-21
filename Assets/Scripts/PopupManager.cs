using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public GameObject popup;
    public GameObject ubuntuButton;
    

    private void Start()
    {
        popup.SetActive(false);
        ubuntuButton.SetActive(false);
    }

    public void ShowPopup() 
    {
        popup.SetActive(true);
    }

    public void HidePopup() 
    {
        popup.SetActive(false);
    }

    public void ShowUbuntu() 
    {
        ubuntuButton.SetActive(true);
    }
}
