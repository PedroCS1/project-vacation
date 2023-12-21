using UnityEngine;

public abstract class ServiceBase : MonoBehaviour
{
    public void InitializeService()
    {
        Initialize();
    }

    protected abstract void Initialize();
}
