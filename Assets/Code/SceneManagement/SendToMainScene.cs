using UnityEngine;
using UnityEngine.SceneManagement;

public class SendToMainScene : MonoBehaviour
{
    private void Awake()
    {

        SceneManager.MoveGameObjectToScene(gameObject,SceneManager.GetActiveScene());
    }
}
