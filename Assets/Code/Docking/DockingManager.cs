using UnityEngine;

public class DockingManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject IslandPlayerPrefab;
    public GameObject PlayerBoatPrefab;
    public GameObject DockedBoatPrefab;

    [Header("Managerd Game Objects")]
    public GameObject DockedBoat;

    [Header("Dock Variables")]
    public Transform[] dockingPositions;
    public Transform GetCurrentDockPos => dockingPositions != null ? dockingPositions[dockIndex] : transform;

    public Transform playerSpawPos;



    [Header("Docked Boat Data")]
    public int dockIndex;
    public bool reversed;

    public bool testBtn;
    private bool toggle;

    private void Start()
    {
        DockedBoat = GameObject.Instantiate(DockedBoatPrefab);

        DockedBoat.transform.parent = GetCurrentDockPos;
        DockedBoat.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        DockedBoat.SetActive(false);
    }

    private void StartIslandState()
    {

        if (GameStatesLocator.Instance == null) return;

        if (!GameStatesLocator.Instance.TryGet(out var gameStates)) return;

        if (PlayerManagerServiceLocator.Instance == null) return;

        if (!PlayerManagerServiceLocator.Instance.TryGet(out var playerManager)) return;

        gameStates.EnterState("ISLAND");


        playerManager.SetBoatPlayer(null);

        SpawnDockedBoat();

        SpawnPlayer();


    }

    private void EndIslandState()
    {
        if (GameStatesLocator.Instance == null) return;

        if (!GameStatesLocator.Instance.TryGet(out var gameStates)) return;

        if (PlayerManagerServiceLocator.Instance == null) return;

        if (!PlayerManagerServiceLocator.Instance.TryGet(out var playerManager)) return;

        gameStates.EnterState("BOAT");

        playerManager.SetIslandPlayer(null);
        DockedBoat.SetActive(false);

        SpawnPlayerBoat();

        
    }

    private void SpawnPlayerBoat()
    {
        var newPlayer = GameObject.Instantiate(PlayerBoatPrefab);

        newPlayer.transform.position = DockedBoat.transform.position;
        newPlayer.transform.rotation = DockedBoat.transform.rotation;
    }

    private void SpawnDockedBoat()
    {
        DockedBoat.SetActive(true);
        DockedBoat.transform.parent = GetCurrentDockPos;
        DockedBoat.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    private void SpawnPlayer()
    {
        var player = GameObject.Instantiate(IslandPlayerPrefab);
        player.transform.position = playerSpawPos.position;
        player.transform.rotation = playerSpawPos.rotation;


    }

    private void FixedUpdate()
    {
        if (testBtn)
        {
            testBtn = false;

            if(toggle)
            {
                StartIslandState();
            }
            else
            {
                EndIslandState();
            }

            toggle = !toggle;

        }
    }

    private void OnDrawGizmos()
    {
        GizmosDrawDockPositions();

        DrawGizmosPlayerPos();

        Gizmos.color = Color.white;
        Gizmos.matrix = Matrix4x4.identity;


    }


    private void GizmosDrawDockPositions()
    {
        if (dockingPositions == null)
            return;

        for (int i = 0; i < dockingPositions.Length; i++)
        {
            if (dockingPositions[i] == null)
                continue;

            Gizmos.matrix = dockingPositions[i].localToWorldMatrix;

            Gizmos.color = i == dockIndex ? Color.green : Color.red;

            Gizmos.DrawSphere(Vector3.zero, 1);
            Gizmos.DrawSphere(Vector3.forward, 0.5f);
        }
    }

    private void DrawGizmosPlayerPos()
    {
        if(playerSpawPos == null) return;

        Gizmos.matrix = playerSpawPos.localToWorldMatrix;
        Gizmos.color = Color.cyan;

        Gizmos.DrawCube(Vector3.zero, Vector3.one);
    }
}
