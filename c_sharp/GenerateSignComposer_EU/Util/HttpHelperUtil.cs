using Conga.Platform.Extensibility.CustomCode.Library;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Conga.Platform.Extensibility.CustomCode.Library.Interfaces;
using System.Text.Json;
using System;
using System.Threading;

namespace GenerateComposerDocV2
{
    public class HttpHelperUtil : CodeExtensibility
    {

        public HttpHelperUtil()
        {

        }
        public async Task<HttpResponseMessage> DoGet(string API_URL, IHttpHelper httpHelper, ITelemetryHelper _telemetryHelper,CancellationToken cancellationToken)
        {
            ThrowIfCancellationRequested(cancellationToken);

            HttpResponseMessage response = null;

            response = await httpHelper.GetAsync(API_URL);


            using var tele = _telemetryHelper.StartActiveSpan("SendForSign");
            if (response.IsSuccessStatusCode)
            {
                tele.AddLog(response.StatusCode.ToString());
                tele.AddLog("success");
            }
            else
            {
                tele.AddLog("Not Success");
            }
            return response;
        }
        public async Task<HttpResponseMessage> DoPost(string API_URL, string jsonBody, IHttpHelper httpHelper, ITelemetryHelper _telemetryHelper,CancellationToken cancellationToken)
        {
            ThrowIfCancellationRequested(cancellationToken);
            HttpResponseMessage response = null;
            var contentToPost = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            response = await httpHelper.PostAsync(API_URL, contentToPost);
            using var tele = _telemetryHelper.StartActiveSpan("SendForSign");
            if (response.IsSuccessStatusCode)
            {
                tele.AddLog(response.StatusCode.ToString());
                tele.AddLog("success");
            }
            else
            {
                tele.AddLog("Not Success");
                var test  = await response.Content.ReadAsStringAsync();
                tele.AddLog("test  "+JsonSerializer.Deserialize<ErrorResponse>(test));
            }
            return response;

        }

        public async Task<HttpResponseMessage> DoPut(string API_URL, string jsonBody, IHttpHelper httpHelper, ITelemetryHelper _telemetryHelper,CancellationToken cancellationToken)
        {
            ThrowIfCancellationRequested(cancellationToken);
            HttpResponseMessage response = null;
            var contentToPost = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            response = await httpHelper.PutAsync(API_URL, contentToPost);
            using var tele = _telemetryHelper.StartActiveSpan("SendForSign");
            if (response.IsSuccessStatusCode)
            {
                tele.AddLog(response.StatusCode.ToString());
                tele.AddLog("success");
            }
            else
            {
                tele.AddLog("Not Success");
                var test  = await response.Content.ReadAsStringAsync();
                tele.AddLog("test  "+JsonSerializer.Deserialize<ErrorResponse>(test));
            }
            return response;

        }

        public async Task<HttpResponseMessage> DoPostToken(string API_URL, string jsonBody, IHttpHelper httpHelper, ITelemetryHelper _telemetryHelper,CancellationToken cancellationToken)
        {
            ThrowIfCancellationRequested(cancellationToken);
            HttpResponseMessage response = null;
            using var tele = _telemetryHelper.StartActiveSpan("Southern Callback");
            var contentToPost = new StringContent(jsonBody, Encoding.UTF8, "application/x-www-form-urlencoded");
            try{
                response = await httpHelper.PostAsync(API_URL, contentToPost);
                tele.AddLog(response.IsSuccessStatusCode 
                    ? $"Success: {response.StatusCode}" 
                    : $"Failure: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                return response;                         
            } catch(Exception ex) {
                tele.AddLog("Error-- "+ex);
                throw; // Rethrow the exception to be handled by the caller
            }
        }


    }

}
