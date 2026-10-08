using System.Collections.Generic;
using UnityEngine;

public class SceneLoaderServiceLocator : ServiceLocatorPersistent<ISceneLoaderService>
{
    
}



public interface ISceneLoaderService
{
    public IEnumerable<Coroutine> LoadAuxSceneBatchesSequential(string[][] sceneBatches);

    public Coroutine StartAuxSceneLoader(string[][] sceneBatches);
}
