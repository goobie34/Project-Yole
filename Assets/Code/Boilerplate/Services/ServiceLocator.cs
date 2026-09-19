using UnityEngine;

public class ServiceLocator<T> : Singleton<ServiceLocator<T>> where T : class
{
    private T _service;
    public bool ContainsService() => _service != null;

    public T Get()
    {
        return _service;
    }

    public bool TryGet(out T service)
    {
        service = _service;
        return ContainsService();
    }

    public void Register(T service)
    {
        if (ContainsService())
            return;

        _service = service;
    }

    public void Deregister()
    {
        _service = null;
    }

    public void DeregisterIfThis(T service)
    {
        if (Get().Equals(service))
        {
            Deregister();
        }
    }


}

public class ServiceLocatorPersistent<T> : SingletonPersistent<ServiceLocatorPersistent<T>> where T : class
{
    private T _service;
    public bool ContainsService() => _service != null;

    public T Get()
    {
        return _service;
    }

    public bool TryGet(out T service)
    {
        service = _service;
        return ContainsService();
    }

    public void Register(T service)
    {
        if (ContainsService())
            return;

        _service = service;
    }

    public void Deregister()
    {
        _service = null;
    }

    public void DeregisterIfThis(T service)
    {
        if (Get().Equals(service))
        {
            Deregister();
        }
    }
}

    