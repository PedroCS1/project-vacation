using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenService : ServiceBase
{
    [SerializeField] private GameObject _defaultLoadingScreen;
    protected override void Initialize()
    {
        Debug.Log("Initialized PopUp Service");
        _defaultLoadingScreen.SetActive(false);
    }
    public void ShowDefaultLoadingScreen()
    {
        _defaultLoadingScreen.SetActive(true);
    }

    public void HideLoadingScreen()
    {
        _defaultLoadingScreen.SetActive(false);
    }
    
}
