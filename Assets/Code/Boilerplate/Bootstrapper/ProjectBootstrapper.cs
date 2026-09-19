using UnityEngine;


public static class ProjectBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        Debug.Log("ProjectBootstrapper: Loading Systems, start  <--------");

        Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("Systems")));

        Debug.Log("ProjectBootstrapper: Loading Systems, end    <--------");
    }
}