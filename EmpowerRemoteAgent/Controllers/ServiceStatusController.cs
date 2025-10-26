using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.ServiceProcess;

namespace EmpowerRemoteAgent.Controllers
{
 public class ServiceStatusController : ApiController
 {
 [HttpGet]
 [Route("api/servicestatus/get")]
 public IHttpActionResult GetServiceStatus(string serviceName)
 {
 try
 {
 string decodedServiceName = System.Net.WebUtility.UrlDecode(serviceName);
 ServiceController[] services = ServiceController.GetServices()
 .Where(s => s.ServiceName.Contains(decodedServiceName))
 .ToArray();

 var serviceStatuses = services.ToDictionary(
 s => s.ServiceName,
 s => s.Status.ToString()
 );
 return Ok(serviceStatuses);
 }
 catch (System.Exception ex)
 {
 return InternalServerError(ex);
 }
 }
 [HttpPost]
 [Route("api/servicestatus/start")]
 public IHttpActionResult StartService(string serviceName)
 {
 try
 {
 string decodedServiceName = System.Net.WebUtility.UrlDecode(serviceName);
 ServiceController sc = new ServiceController(decodedServiceName);

 if (sc.Status == ServiceControllerStatus.Stopped)
 {
 sc.Start();
 sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
 return Ok($"Service '{decodedServiceName}' started successfully.");
 }
 else
 {
 return BadRequest($"Service '{decodedServiceName}' is already running or in a state that cannot be started.");
 }
 }
 catch (System.Exception ex)
 {
 return InternalServerError(ex);
 }
 }
 [HttpPost]
 [Route("api/servicestatus/stop")]
 public IHttpActionResult StopService(string serviceName)
 {
 try
 {
 string decodedServiceName = System.Net.WebUtility.UrlDecode(serviceName);
 ServiceController sc = new ServiceController(decodedServiceName);

 if (sc.Status == ServiceControllerStatus.Running)
 {
 sc.Stop();
 sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
 return Ok($"Service '{decodedServiceName}' stopped successfully.");
 }
 else
 {
 return BadRequest($"Service '{decodedServiceName}' is already stopped or in a state that cannot be stopped.");
 }
 }
 catch (System.Exception ex)
 {
 return InternalServerError(ex);
 }
 }
 }
}