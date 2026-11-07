using UnityEngine;

public class PlayerManagerServiceLocator : ServiceLocator<IPlayerManagerService>
{
    
}

public interface IPlayerManagerService
{
    GameObject GetIslandPlayer { get; }
    GameObject GetBoatPlayer { get; }

    public void SetIslandPlayer(GameObject player);

    public void SetBoatPlayer(GameObject player);
}