using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderService : MonoBehaviour
{

    public void Start()
    {
        string[][] batches = new string[][] 
        {
            new string[] {"1.1","1.2","1.3"},
            new string[] {"2.1","2.2","2.3"},
            new string[] {"3.1","3.2","3.3"},
        };
        StartCoroutine(TestSequentialBatches(batches));
    }

    public IEnumerator TestSequentialBatches(string[][] batches)
    {
        for(int i = 0; i < batches.Length; i++)
        {
            var batchProgress = LoadBatch(batches[i]);

            yield return StartCoroutine(AwaitBatch(batchProgress));
        }
    }

    public IEnumerator AwaitBatch(Coroutine[] batch)
    {
        foreach (Coroutine i in batch)
        {
            yield return i;
        }
    }

    public Coroutine[] LoadBatch(string[] batch)
    {
        Coroutine[] batchLoaders = new Coroutine[batch.Length];

        for(int i = 0; i < batch.Length; i++)
        {
            batchLoaders[i] = StartCoroutine(LoadSingle(batch[i]));
        }

        return batchLoaders;

    }

    public IEnumerator LoadSingle(string single)
    {
        yield return new WaitForSeconds(Random.Range(5, 20));

        Debug.Log(single);
    }


    public void LoadAuxScene(string sceneName)
    {
        
        
        
    }

}
