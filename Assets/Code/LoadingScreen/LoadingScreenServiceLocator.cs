using UnityEngine;

public class LoadingScreenServiceLocator : ServiceLocatorPersistent<ILoadingScreenService>
{
    
}

public interface ILoadingScreenService
{
    public void AddShowLoadingScreen();
    public void AddHideLoadingScreen();
}