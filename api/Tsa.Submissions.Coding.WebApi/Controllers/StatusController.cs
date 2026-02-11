using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tsa.Submissions.Coding.Contracts.HealthChecks;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class StatusController : ControllerBase
{
    private readonly List<IPingableService> _pingableServices = [];

    public StatusController(IEnumerable<IPingableService> pingableServices)
    {
        _pingableServices.AddRange(pingableServices);
    }

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServicesStatusResponse>> Get(CancellationToken cancellationToken = default)
    {
        var concurrentBag = new ConcurrentBag<Tuple<string, bool>>();

        await Parallel.ForEachAsync(_pingableServices, cancellationToken, async (pingableService, pingCancellationToken) =>
        {
            bool isAlive;
            try
            {
                isAlive = await pingableService.PingAsync(pingCancellationToken);
            }
            catch (Exception)
            {
                isAlive = false;
            }

            concurrentBag.Add(new Tuple<string, bool>(pingableService.ServiceName, isAlive));
        });

        var servicesStatusResponseType = typeof(ServicesStatusResponse);
        var servicesStatusResponse = new ServicesStatusResponse();

        foreach (var tuple in concurrentBag)
        {
            var propertyInfo = servicesStatusResponseType.GetProperty($"{tuple.Item1}ServiceIsAlive");

            if (propertyInfo == null) continue;

            propertyInfo.SetValue(servicesStatusResponse, tuple.Item2);
        }

        return servicesStatusResponse.IsHealthy
            ? Ok(servicesStatusResponse)
            : StatusCode(StatusCodes.Status500InternalServerError, servicesStatusResponse);
    }
}
