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

    public Transform GetTransformAtIndex(int index)
    {
        index = Mathf.Clamp(index, 0, dockingPositions.Length);

        return dockingPositions[index];
    }

    public Transform GetCurrentDockPos => dockingPositions != null ? dockingPositions[dockIndex] : transform;

    public Transform playerSpawPos;



    [Header("Docked Boat Data")]
    public int dockIndex;
    public bool reversed;

    public bool testBtn;
    private bool toggle;

    private Quaternion GetFlipper => reversed ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;

    private void Start()
    {
        DockedBoat = GameObject.Instantiate(DockedBoatPrefab);

        DockedBoat.transform.parent = GetCurrentDockPos;
        DockedBoat.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        DockedBoat.SetActive(false);
    }

    public void StartIslandState()
    {

        if (GameStatesLocator.Instance == null) return;

        if (!GameStatesLocator.Instance.TryGet(out var gameStates)) return;

        if (PlayerManagerServiceLocator.Instance == null) return;

        if (!PlayerManagerServiceLocator.Instance.TryGet(out var playerManager)) return;

        toggle = false;

        gameStates.EnterState("ISLAND");


        playerManager.SetBoatPlayer(null);

        SpawnDockedBoat();

        SpawnPlayer();


    }

    public void EndIslandState()
    {
        if (GameStatesLocator.Instance == null) return;

        if (!GameStatesLocator.Instance.TryGet(out var gameStates)) return;

        if (PlayerManagerServiceLocator.Instance == null) return;

        if (!PlayerManagerServiceLocator.Instance.TryGet(out var playerManager)) return;

        toggle = true;


        gameStates.EnterState("BOAT");

        playerManager.SetIslandPlayer(null);

        if (DockedBoat != null)
            Destroy(DockedBoat);
        
        SpawnPlayerBoat();

        
    }

    private void SpawnPlayerBoat()
    {
        var newPlayer = GameObject.Instantiate(PlayerBoatPrefab);

        newPlayer.transform.position = GetCurrentDockPos.position;
        newPlayer.transform.rotation = GetFlipper * GetCurrentDockPos.rotation;

        if (WaveServiceLocator.Instance == null)
            return;
    }

    private void SpawnDockedBoat()
    {
        if(DockedBoat != null)
        {
            Destroy(DockedBoat);
        }

        DockedBoat = Instantiate(DockedBoatPrefab, GetCurrentDockPos);


        DockedBoat.transform.SetLocalPositionAndRotation(Vector3.zero, GetFlipper);
    }

    private void SpawnPlayer()
    {
        var player = GameObject.Instantiate(IslandPlayerPrefab);
        player.transform.position = playerSpawPos.position;
        player.transform.rotation = playerSpawPos.rotation;


    }

    
    public void GetClosestDockingIndex(Transform target, out int index)
    {
        index = -1;
        

        if(dockingPositions == null)
            return ;

        if (dockingPositions.Length == 0)
            return;

        int min = 0;
        float minDist = Vector3.Distance(target.position, dockingPositions[0].position);

        for(int i = 1; i < dockingPositions.Length; i++)
        {
            var linearDist = Vector3.Distance( target.position, dockingPositions[i].position);

            if (linearDist < minDist)
            {
                min = i;
                minDist = linearDist;
            }
        }
        index = min;
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
