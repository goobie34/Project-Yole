using FMODUnity;
using UnityEngine;
[RequireComponent(typeof(DockingManager))]
public class DockMusicTrigger : MonoBehaviour
{
    [SerializeField] private EventReference dockedMusic;
    [SerializeField] private EventReference undockedMusic;
    DockingManager dockingManager;

    private void Awake()
    {
        dockingManager = GetComponent<DockingManager>();
    }
    private void OnEnable()
    {
        dockingManager.EVENT_DockBoat.AddListener(Dock);
        dockingManager.EVENT_UndockBoat.AddListener(Undock);
    }

    private void OnDisable()
    {
        dockingManager.EVENT_DockBoat.RemoveListener(Dock);
        dockingManager.EVENT_UndockBoat.RemoveListener(Undock);
    }
    public void Dock() { MusicManager.Instance.ChangeMusic(dockedMusic); Debug.Log("DOCK MUSIC"); }
    public void Undock() { MusicManager.Instance.ChangeMusic(undockedMusic); Debug.Log("UNDOCK MUSIC"); }
}
