using UnityEngine;

public class WindServiceLocator : ServiceLocator<IWindService>
{

}



public interface IWindService
{
    public void EvaluateWind(Vector3 worldPos, float time, out Vector3 windDirection, out float windMagnitude);
}