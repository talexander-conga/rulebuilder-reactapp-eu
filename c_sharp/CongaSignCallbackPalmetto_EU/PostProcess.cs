using Conga.Platform.Extensibility.CustomCode.Library;
using Conga.Platform.Extensibility.CustomCode.Library.ServiceHooks;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;
using System.Dynamic;
using System.Collections.Generic;
using System.Xml.XPath;
using System.Net.Http;
using System.Text;

namespace CongaSignCallbackPalmetto
{
    public class PostProcess : CodeExtensibility
    {

        public async void Callbackmethod(dynamic actionParams,CancellationToken cancellationToken){
            ThrowIfCancellationRequested(cancellationToken);
            var _telemetryHelper = base.GetTelemetryHelper();
            var httpHelper = GetHttpHelper(new Conga.Platform.Extensibility.CustomCode.Library.Models.InternalApiAttributes());
            using var tele = _telemetryHelper.StartActiveSpan("CongaSignCallbackPalmetto");
            try{
                tele.AddLog("Conga Sign Callback Called");
                var paramsElement = (JsonElement)actionParams;
                //tele.AddLog(postBody.name + " " + postBody.packageId);
                var name = paramsElement.GetProperty("name").GetString();
                var packageId=paramsElement.GetProperty("packageId").GetString();
                tele.AddLog(name + " " + packageId);
                HttpHelperUtil httpUtil = new HttpHelperUtil();
                
                if (name == "PACKAGE_COMPLETE")
                {
                    var dbHelper = this.GetDataHelper();
                    DataAccess dataAccess = new(dbHelper);
                    var contractRecord = dataAccess.GetContract(packageId,cancellationToken).Result.FirstOrDefault();
                    tele.AddLog("contractRecord >>> " + JsonSerializer.Serialize<ContractQueryModel>(contractRecord));

                    var callGetPackageAPIResponse = await httpUtil.DoGet($"{Constants.CoreAppsUrl}/api/sign/v1/cs-packages/" + packageId, httpHelper, _telemetryHelper,cancellationToken);
                    var callGetPackageAPIResponseResult = await callGetPackageAPIResponse.Content.ReadAsStringAsync();
                    tele.AddLog("Get Package" + callGetPackageAPIResponseResult);
                    var SignedDocumentId = JsonSerializer.Deserialize<GetPackageResponse>(callGetPackageAPIResponseResult).documents[1].id;
                    tele.AddLog("signed doc id "+SignedDocumentId);
                    var callGetDocumentAPIResponse = await httpUtil.DoGet($"{Constants.CoreAppsUrl}/api/sign/v1/cs-packages/" + packageId+"/documents/"+SignedDocumentId+"/pdf", httpHelper, _telemetryHelper,cancellationToken);
                    var callGetDocumentAPIResponseResult = await callGetDocumentAPIResponse.Content.ReadAsByteArrayAsync();
                    tele.AddLog("Get Document --- " + callGetDocumentAPIResponseResult);

                    var fileContent = new ByteArrayContent(callGetDocumentAPIResponseResult);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                    Dictionary<String,String> docAttributes = new(){
                        {"DocumentType","SupportingDocument"}
                    };
                    var docAttributesJson = JsonSerializer.Serialize(docAttributes);
                    string jsonObject = JsonSerializer.Serialize(docAttributes);
                    var jsonContent = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                    var formData = new MultipartFormDataContent
                    {
                       { new StringContent(docAttributesJson), "documentAttributes" },
                        { fileContent, "file",contractRecord.Name+" For "+contractRecord.Account.Name+".pdf"}
                    };
                    var postResultFile = await httpHelper.PostAsync("/api/clm/v1/contracts/"+contractRecord.Id+"/documents", formData);
                    var postResultFileResult = await postResultFile.Content.ReadAsStringAsync();
                    tele.AddLog(postResultFileResult);

                }
            }
            catch (Exception ex)
            {
                tele.AddLog("Error--- " + ex);
            }
        }
    }
}
