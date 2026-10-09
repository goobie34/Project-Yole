using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuxScenes : MonoBehaviour
{
    [System.Serializable]
    public struct SceneBatch
    {
        public string[] scenes;
    }

    public SceneBatch[] scenes;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var sceneLoader = SceneLoaderServiceLocator.Instance?.Get();

        StartCoroutine(SceneLoadingAwaiter(sceneLoader?.LoadAuxSceneBatchesSequential(SceneBatchArrayToStringArray(scenes))));
        
    }

    private IEnumerator SceneLoadingAwaiter(IEnumerable<Coroutine> loaders)
    {


        bool gotLoadingscreen = LoadingScreenServiceLocator.Instance.TryGet(out var loadingScreen);

        if(gotLoadingscreen)
            loadingScreen.AddShowLoadingScreen();

        yield return new WaitForSeconds(0.5f);

        foreach (var loader in loaders)
        {
            yield return loader;
        }

        if(gotLoadingscreen)
            loadingScreen.AddHideLoadingScreen();
        
    }

    private string[][] SceneBatchArrayToStringArray(SceneBatch[] batches)
    {
        string[][] string_batches = new string[batches.Length][];

        for (int i = 0; i < batches.Length; i++)
        {
            string_batches[i] = batches[i].scenes;
        }

        return string_batches;
    }
    
}
