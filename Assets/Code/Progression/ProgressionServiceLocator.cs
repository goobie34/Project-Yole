using UnityEngine;

public class ProgressionServiceLocator : ServiceLocator<IProgressionService>
{
    
}

public interface IProgressionService
{

    public void SetPoints(int points);
    public int GetCurrentPoints();

    public void IncrementPoints(int diff);

    public int GetCurrentProgressionIndex();


}
