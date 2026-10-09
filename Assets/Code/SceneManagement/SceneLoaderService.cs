using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class SceneLoaderService : MonoBehaviour, ISceneLoaderService
{

    public void Start()
    {
        SceneLoaderServiceLocator.Instance.Register(this);
    }

    public Coroutine StartAuxSceneLoader(string[][] sceneBatches)
    {
        return StartCoroutine
        (
            AwaitCoruitneBatch
            (
                LoadAuxSceneBatchesSequential(sceneBatches)
            )
        );
    }


    public IEnumerable<Coroutine> LoadAuxSceneBatchesSequential(string[][] sceneBatches)
    {
        Debug.Log("SceneLoader ------- Loading Aux Scenes Start");

        for (int i = 0; i < sceneBatches.Length; i++)
        {

            Debug.Log($"SceneLoader - Loading AuxSceneBatch: '{i}'");
            var batchProgress = LoadBatchAux(sceneBatches[i], i);

            yield return StartCoroutine(AwaitCoroutines(batchProgress));
        }

        Debug.Log("SceneLoader ------- Loading Aux Scenes End");
    }

    


    private IEnumerator AwaitCoruitneBatch(IEnumerable<Coroutine> batchLoader)
    {
        foreach (var iterator in batchLoader)
        {
            yield return iterator;
        }
    }
 
    private IEnumerator AwaitCoroutines(Coroutine[] batch)
    {
        foreach (Coroutine i in batch)
        {
            yield return i;
        }
    }

    private Coroutine[] LoadBatchAux(string[] batch, int batchNr)
    {
        Coroutine[] batchLoaders = new Coroutine[batch.Length];

        for(int i = 0; i < batch.Length; i++)
        {
            Debug.Log($"SceneLoader ---- Loading Scene '{batch[i]}', nr {i} of batch {batchNr}");


            batchLoaders[i] = StartCoroutine(LoadSingleAux(batch[i]));
        }

        return batchLoaders;

    }

    private IEnumerator LoadSingleAux(string single)
    {
        LoadSceneParameters parameters = new LoadSceneParameters();
        parameters.loadSceneMode = LoadSceneMode.Additive;
        

        var sceneLoader = SceneManager.LoadSceneAsync(single, parameters);

        while (!sceneLoader.isDone)
        {
            yield return null;
        }

        
    }


    

}
