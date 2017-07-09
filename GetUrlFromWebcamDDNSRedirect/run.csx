using System.Net;
using System.Configuration;

private static string WebcamUrl = ConfigurationManager.AppSettings["WebcamDDNSUrl"];
private static var noRedirectHandler = new HttpClientHandler
{
    AllowAutoRedirect = false
};
private static var client = new HttpClient(noRedirectHandler);

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");
    log.Info($"Sending request to 'WebcamDDNSUrl': {WebcamUrl}");
    using (var response = await client.GetAsync(WebcamUrl, HttpCompletionOption.ResponseHeadersRead)) {
        if (IsRedirectStatus(response.StatusCode)) {
            var locationUri = response.Headers.Location;
            log.Info($"locationUri: {locationUri}");
            return req.CreateResponse(HttpStatusCode.OK, locationUri.Host);
        }
        log.Error("No redirect found! Response Status Code: {response.StatusCode}");
        return req.CreateResponse(HttpStatusCode.InternalServerError);
    }
}

private static bool IsRedirectStatus(HttpStatusCode httpStatusCode) {
    var statusCode = (int)httpStatusCode;
    return statusCode >= 300 && statusCode < 400;
}
