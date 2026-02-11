namespace Tsa.Submissions.Coding.Contracts.HealthChecks;

public class ServicesStatusResponse
{
    public bool IsHealthy => EvaluateIsHealthy();

    public bool ProblemsServiceIsAlive { get; set; }

    public bool SubmissionsServiceIsAlive { get; set; }

    public bool TestSetsServiceIsAlive { get; set; }

    private bool EvaluateIsHealthy()
    {
        var propertyInfos = GetType().GetProperties().Where(propertyInfo => propertyInfo.CanWrite && propertyInfo.PropertyType == typeof(bool));

        var isAliveList = propertyInfos.Select(propertyInfo => (bool)propertyInfo.GetValue(this)!).ToList();

        return isAliveList.All(isAlive => isAlive);
    }
}
