using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using EmpowerStatusSite.Helpers;
using System.Collections.Specialized;
using System.Net;
using Newtonsoft.Json.Linq;

namespace EmpowerStatusSite.Controllers
{
 [RoutePrefix("api/proxy")]
 public class ProxyController : ApiController
 {
 // Proxy scheduled task actions to target server to avoid CORS from browser
 [HttpPost]
 [Route("scheduledtask/run")]
 public async Task<IHttpActionResult> RunScheduledTask([FromBody] JObject body)
 {
 try
 {
 var server = (string)body["server"];
 var taskName = (string)body["taskName"];
 if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(taskName)) return BadRequest("Invalid payload");

 var response = await HttpClientProvider.Instance.PostAsync($"http://{server}:8081/api/scheduledtask/{Uri.EscapeDataString(taskName)}/run", new StringContent(""));
 return StatusCode(response.IsSuccessStatusCode ? HttpStatusCode.OK : response.StatusCode);
 }
 catch (Exception ex)
 {
 return InternalServerError(ex);
 }
 }

 [HttpPost]
 [Route("scheduledtask/disable")]
 public async Task<IHttpActionResult> DisableScheduledTask([FromBody] JObject body)
 {
 try
 {
 var server = (string)body["server"];
 var taskName = (string)body["taskName"];
 if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(taskName)) return BadRequest("Invalid payload");

 var response = await HttpClientProvider.Instance.PostAsync($"http://{server}:8081/api/scheduledtask/{Uri.EscapeDataString(taskName)}/disable", new StringContent(""));
 return StatusCode(response.IsSuccessStatusCode ? HttpStatusCode.OK : response.StatusCode);
 }
 catch (Exception ex)
 {
 return InternalServerError(ex);
 }
 }

 [HttpPost]
 [Route("scheduledtask/enable")]
 public async Task<IHttpActionResult> EnableScheduledTask([FromBody] JObject body)
 {
 try
 {
 var server = (string)body["server"];
 var taskName = (string)body["taskName"];
 if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(taskName)) return BadRequest("Invalid payload");

 var response = await HttpClientProvider.Instance.PostAsync($"http://{server}:8081/api/scheduledtask/{Uri.EscapeDataString(taskName)}/enable", new StringContent(""));
 return StatusCode(response.IsSuccessStatusCode ? HttpStatusCode.OK : response.StatusCode);
 }
 catch (Exception ex)
 {
 return InternalServerError(ex);
 }
 }

 // Proxy service actions
 [HttpPost]
 [Route("servicestatus/start")]
 public async Task<IHttpActionResult> StartService([FromBody] JObject body)
 {
 try
 {
 var server = (string)body["server"];
 var serviceName = (string)body["serviceName"];
 if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(serviceName)) return BadRequest("Invalid payload");

 var response = await HttpClientProvider.Instance.PostAsync($"http://{server}:8081/api/servicestatus/start?serviceName={Uri.EscapeDataString(serviceName)}", new StringContent(""));
 return StatusCode(response.IsSuccessStatusCode ? HttpStatusCode.OK : response.StatusCode);
 }
 catch (Exception ex)
 {
 return InternalServerError(ex);
 }
 }

 [HttpPost]
 [Route("servicestatus/stop")]
 public async Task<IHttpActionResult> StopService([FromBody] JObject body)
 {
 try
 {
 var server = (string)body["server"];
 var serviceName = (string)body["serviceName"];
 if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(serviceName)) return BadRequest("Invalid payload");

 var response = await HttpClientProvider.Instance.PostAsync($"http://{server}:8081/api/servicestatus/stop?serviceName={Uri.EscapeDataString(serviceName)}", new StringContent(""));
 return StatusCode(response.IsSuccessStatusCode ? HttpStatusCode.OK : response.StatusCode);
 }
 catch (Exception ex)
 {
 return InternalServerError(ex);
 }
 }
 }
}
