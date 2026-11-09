using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManagerService : MonoBehaviour, IPlayerManagerService
{
    

    private GameObject m_islandPlayer;
    private GameObject m_boatPlayer;

    public GameObject GetIslandPlayer => m_islandPlayer;
    public GameObject GetBoatPlayer => m_boatPlayer;

    private IEnumerator Start()
    {
        while (this.isActiveAndEnabled)
        {
            PlayerManagerServiceLocator.Instance?.Register(this);
            if (PlayerManagerServiceLocator.Instance != null)
                break;
            yield return new WaitForFixedUpdate();
        }

    }

    private void DespawnIslandPlayer()
    {
        if (m_islandPlayer == null)
            return;

        Destroy(m_islandPlayer);
    }
    private void DespawnBoatPlayer()
    {
        if (m_boatPlayer == null)
            return;

        Destroy(m_boatPlayer);
    }

    public void SetIslandPlayer(GameObject player)
    {
        if (player == m_islandPlayer)
            return;

        DespawnIslandPlayer();

        m_islandPlayer = player;
    }

    public void SetBoatPlayer(GameObject player)
    {
        if (player == m_boatPlayer)
            return;

        DespawnBoatPlayer();


        m_boatPlayer = player;
    }

    
}
