using System.Linq;

namespace Tsa.Submissions.Coding.WebApi.Models;

public class ServicesStatusModel
{
    public bool IsHealthy => EvaluateIsHealthy();

    public bool ProblemsServiceIsAlive { get; set; }

    public bool SubmissionsServiceIsAlive { get; set; }

    public bool TestSetsServiceIsAlive { get; set; }

    private bool EvaluateIsHealthy()
    {
        var propertyInfos = GetType().GetProperties().Where(_ => _.CanWrite && _.PropertyType == typeof(bool));

        var isAliveList = propertyInfos.Select(propertyInfo => (bool)propertyInfo.GetValue(this)!).ToList();

        return isAliveList.All(isAlive => isAlive);
    }
}
