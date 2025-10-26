using System;
using System.Net.Http;

namespace EmpowerStatusSite.Helpers
{
 public static class HttpClientProvider
 {
 private static readonly Lazy<HttpClient> _lazyClient = new Lazy<HttpClient>(() =>
 {
 var client = new HttpClient();
 client.Timeout = TimeSpan.FromSeconds(30);
 client.DefaultRequestHeaders.Add("User-Agent", "EmpowerStatusSite/1.0");
 return client;
 });

 public static HttpClient Instance => _lazyClient.Value;
 }
}
