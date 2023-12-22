using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServiceManager : MonoBehaviour
{
    public static ServiceManager Instance { get; private set; }
    [SerializeField] private ServiceBase[] allServices;

    public AudioService GameAudio { get; private set; }
    public LoadingScreenService LoadingScreen { get; private set; }

    private void Awake()
    {
        Initialize();
        GoToMainMenu();
    }

    private static void GoToMainMenu()
    {
        SceneManager.LoadScene(SceneNames.MAIN_MENU);
    }

    private void Initialize()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeServices();
    }

    public void InitializeServices()
    {
        foreach (var prefab in allServices)
        {
            GameObject serviceObject = Instantiate(prefab, gameObject.transform).gameObject;
            ServiceBase service = serviceObject.GetComponent<ServiceBase>();
            AssignService(service);
            service.InitializeService();
        }
    }

    private void AssignService(ServiceBase service)
    {
        switch (service)
        {
            case AudioService audioService:
                GameAudio = audioService;
                break;
            case LoadingScreenService loadingService:
                LoadingScreen = loadingService;
                break;
            default:
                break;
        }
    }
}
