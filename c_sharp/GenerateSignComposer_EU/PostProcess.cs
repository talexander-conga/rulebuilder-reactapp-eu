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

namespace GenerateComposerDocV2
{
  public class PostProcess : CodeExtensibility, IDataChangeServiceHooks
  {
    [Obsolete]
    public Task OnDataChangeAsync(DataChangeEvent dataChangeEvent)
    {
      throw new NotImplementedException();
    }

    public class MyObj
    {
      public string test { get; set; }
      public decimal amount { get; set; }
    }

    //Generate Document and sign using Composer API & Conga Sign API for Palmetto
    public async Task<string> generateandsigncomposerdoc(dynamic actionParams, CancellationToken cancellationToken)
    {
      //Add this check in every data transformation logic
      ThrowIfCancellationRequested(cancellationToken);
      var httpHelper = GetHttpHelper(new Conga.Platform.Extensibility.CustomCode.Library.Models.InternalApiAttributes());
      var httpUtil = new HttpHelperUtil();
      var _telemetryHelper = base.GetTelemetryHelper();
      using var tele = _telemetryHelper.StartActiveSpan("GenerateComposerDoc palmetto");
      tele.AddLog("GenerateComposerDoc Starts Palmetto");
      var paramsElement = (JsonElement)actionParams;
      var RecordIdData = paramsElement.GetProperty("recordId").GetString();
      tele.AddLog("RecordIDData --- " + RecordIdData);
      var dbHelper = this.GetDataHelper();
      DataAccess dataAccess = new(dbHelper);
      HashSet<string> clauseNames = new HashSet<string>();
      string clausesToQuery = "";
      var queryTermList = dataAccess.GetQueryTerm(cancellationToken).Result;
      tele.AddLog("queryTermList " + JsonSerializer.Serialize(queryTermList));
      var templateId = Constants.TemplateId_MSA;
      HashSet<string> templateIdList = new HashSet<string>();
      var agreementRecord = dataAccess.GetAgreementById(RecordIdData, cancellationToken).Result.FirstOrDefault();
      tele.AddLog("Agreement " + JsonSerializer.Serialize(agreementRecord));
      List<SignerDefination> signerDefinationList3 = new List<SignerDefination>();
      SignerDefination signerDefination3 = new SignerDefination();
      bool hasSigner3=false;

      //Gather Template Ids and ClauseNames
      foreach (var queryTerm in queryTermList)
      {
        ThrowIfCancellationRequested(cancellationToken);
        if (!string.IsNullOrEmpty(queryTerm.CNGCU_Expression_Condition_c))
        {
          var agreement = dataAccess.GetAgreement(RecordIdData, queryTerm.CNGCU_Expression_Condition_c, cancellationToken).Result.FirstOrDefault();
          tele.AddLog("Agreement " + JsonSerializer.Serialize(agreement));
          if (agreement != null)
          {
            if (queryTerm.CNGCU_Type_c != null && queryTerm.CNGCU_Type_c == "Template")
            {

              templateIdList.Add(queryTerm.CNGCU_TemplateId_c);
            }
            else if(queryTerm.CNGCU_Type_c != null && queryTerm.CNGCU_Type_c == "Clause")
            {
              string[] clauses = queryTerm.CNGCU_Clauses_c.Split(',').Select(x => $"'{x.Trim()}'").ToArray();
              clauseNames.UnionWith(clauses);
            }
            else if (queryTerm.CNGCU_Type_c != null && queryTerm.CNGCU_Type_c == "Signer")
            {
               
                signerDefination3.email = agreementRecord?.Installer_Email_c;
                signerDefination3.firstName = agreementRecord?.Installer_Name_c.Split(' ')[0];
                signerDefination3.lastName = agreementRecord?.Installer_Name_c.Split(' ')[1];
                signerDefination3.SignerName = agreementRecord?.Installer_Name_c;
                signerDefination3.title="Installer";
                signerDefinationList3.Add(signerDefination3);
                hasSigner3=true;
            }
          }
        }
      }
      tele.AddLog("template Id List" + JsonSerializer.Serialize(templateIdList));

      if (clauseNames.Count > 0)
      {
        clausesToQuery = "(" + string.Join(",", clauseNames) + ")";
      }
      tele.AddLog($"Clauses {clausesToQuery}"); 
      List<TemplateQueryModel> clauseList = new List<TemplateQueryModel>();

      //Query Clause Details
      if (clauseNames.Count > 0)
      {
        clauseList = dataAccess.GetTemplate(clausesToQuery, cancellationToken).Result;
        tele.AddLog("clauseList " + JsonSerializer.Serialize(clauseList));
      }


     

      var accountRecord = dataAccess.GetAccount(agreementRecord?.Account?.Id, cancellationToken).Result.FirstOrDefault();
      tele.AddLog("accountRecord >>> " + JsonSerializer.Serialize(accountRecord));
      var agreementContactRecord = dataAccess.GetContact(agreementRecord?.PrimaryContact?.Id, cancellationToken).Result.FirstOrDefault();
      tele.AddLog("contactRecord >>> " + JsonSerializer.Serialize(agreementContactRecord));

      var ownerUserData = await httpUtil.DoGet($"{Constants.InstanceURL}/api/user-management/v1/users/" + agreementRecord.RecordOwner.Id, httpHelper, _telemetryHelper, cancellationToken);
      var ownerUserDataResponseResult = await ownerUserData.Content.ReadAsStringAsync();
      var userData = JsonSerializer.Deserialize<UserQueryModel>(ownerUserDataResponseResult).Data;
      //Create Composer Merge Data
      Dictionary<String, dynamic> DataValues = new(){
                {"Name",agreementRecord.Name},
                {"CustomerName_c",agreementRecord.CustomerName_c},
                {"ContractStartDate",agreementRecord.ContractStartDate.ToString("MMMM dd, yyyy")},
                {"ContractEndDate",agreementRecord.ContractEndDate.ToString("MMMM dd, yyyy")},
                {"CustomerState_c",agreementRecord.CustomerState_c},
                {"CustomerStreetAddress_c",agreementRecord.CustomerStreetAddress_c},
                {"CNGCU_Billing_City",accountRecord?.BillingCity},
                {"CustomerZipCode_c",agreementRecord.CustomerZipCode_c},
                {"TotalContractValue",agreementRecord.TotalContractValue?.Value},
                {"Installer_Name_c",agreementRecord?.Installer_Name_c},
                {"HasSigner3",hasSigner3},
                {"Signer1","{{esl:Signer1:Signature}}"},
                {"Signer2","{{esl:Signer2:Signature}}"},
                {"Signer3","{{esl:Signer3:Signature}}"},
                {"Signer1Name","{{esl:Signer1:SignerName}}"},
                {"Signer1Date","{{esl:Signer1:SigningDate}}"},
                {"Signer1Title","{{esl:Signer1:SignerTitle}}"},
                {"Signer2Name","{{esl:Signer2:SignerName}}"},
                {"Signer2Date","{{esl:Signer2:SigningDate}}"},
                {"Signer2Title","{{esl:Signer2:SignerTitle}}"},
                {"Signer3Name","{{esl:Signer3:SignerName}}"},
                {"Signer3Date","{{esl:Signer3:SigningDate}}"},
                {"Signer3Title","{{esl:Signer3:SignerTitle}}"},
                {"Clauses",clauseList},
                {"RecordOwner",agreementRecord.RecordOwner?.Name}
            };
      dynamic composerBody = new ExpandoObject();
      dynamic composerLegacy = new ExpandoObject();
      dynamic composerOutput = new ExpandoObject();
      composerLegacy.Ofn = "RLS_Test_composer";
      composerLegacy.RM = "0";
      composerLegacy.DefaultPDF = "1";
      composerBody.LegacyOptions = composerLegacy;
      composerBody.JsonData = JsonSerializer.Serialize(DataValues);
      tele.AddLog("jsondata --" + JsonSerializer.Serialize(DataValues));
      composerOutput.generateDocumentDownload = true;
      composerBody.output = composerOutput;

      List<Dictionary<String, String>> composerTemplateList = new List<Dictionary<string, string>>();

      if (templateIdList.Count == 0)
      {
        templateIdList.Add(templateId);
      }
      foreach (var template in templateIdList)
      {
        ThrowIfCancellationRequested(cancellationToken);
        Dictionary<String, String> composerTemplate = new(){
                {"IntegrationName","conga"},
                {"fileId",template}
            };
        composerTemplateList.Add(composerTemplate);
      }


      composerBody.TemplateSources = composerTemplateList;
      var composerBodySerialized = JsonSerializer.Serialize(composerBody);
      tele.AddLog(composerBodySerialized);


      //Call Composer API
      var callComposerMergeAPIResponse = await httpUtil.DoPost($"{Constants.CoreAppsUrl}/api/ingress/v1/Merge", composerBodySerialized, httpHelper, _telemetryHelper,cancellationToken);
      var callComposerMergeAPIResponseResult = await callComposerMergeAPIResponse.Content.ReadAsStringAsync();
      var correlationId = JsonSerializer.Deserialize<ComposerMergeQueryModel>(callComposerMergeAPIResponseResult).correlationId;
      tele.AddLog("correlationId " + correlationId);

      //Get Document Generation Status
      var callMergeStatusAPIResponse = await httpUtil.DoGet($"{Constants.CoreAppsUrl}/api/status/v1/status/" + correlationId, httpHelper, _telemetryHelper,cancellationToken);
      var callMergeStatusAPIResponseResult = await callMergeStatusAPIResponse.Content.ReadAsStringAsync();
      var statusMessage = JsonSerializer.Deserialize<MergeStatusResponse>(callMergeStatusAPIResponseResult).message;
      var statusFlag = false;
      while (!statusFlag && statusMessage == "Pending")
      {
        ThrowIfCancellationRequested(cancellationToken);
        await Task.Delay(2000);
        callMergeStatusAPIResponse = await httpUtil.DoGet($"{Constants.CoreAppsUrl}/api/status/v1/status/" + correlationId, httpHelper, _telemetryHelper,cancellationToken);
        callMergeStatusAPIResponseResult = await callMergeStatusAPIResponse.Content.ReadAsStringAsync();
        statusMessage = JsonSerializer.Deserialize<MergeStatusResponse>(callMergeStatusAPIResponseResult).message;
        if (statusMessage == "Completed")
        {
          statusFlag = true;
        }
      }
      tele.AddLog(statusMessage);
      if (statusFlag == true)
      {
        //Get Composer Generated Document File
        var getTransactionAPIResponse = await httpUtil.DoGet($"{Constants.CoreAppsUrl}/api/presigned/v1/presignedservice/" + correlationId, httpHelper, _telemetryHelper,cancellationToken);
        var getTransactionAPIResponseResult = await getTransactionAPIResponse.Content.ReadAsStringAsync();
        var transactionId = JsonSerializer.Deserialize<TransactionResponse>(getTransactionAPIResponseResult).preSignedCreateResponseModel.fileInfoDetailsList[0].transactionId;
        var getFileAPIResponse = await httpUtil.DoGet($"{Constants.CoreAppsUrl}/api/presigned/v1/presignedservice/" + correlationId + "/" + transactionId, httpHelper, _telemetryHelper,cancellationToken);
        var getFileAPIResponseResult = await getFileAPIResponse.Content.ReadAsByteArrayAsync();


        // Creating Data For Conga sign
        List<SignerDefination> signerDefinationList1 = new List<SignerDefination>();
        List<SignerDefination> signerDefinationList2 = new List<SignerDefination>();
        SignerDefination signerDefination1 = new SignerDefination();
        signerDefination1.email = agreementContactRecord?.Email;
        signerDefination1.firstName = agreementContactRecord?.FirstName;
        signerDefination1.lastName = agreementContactRecord?.LastName;
        signerDefination1.SignerName = agreementContactRecord.FirstName + " " + agreementContactRecord.LastName;
        signerDefinationList1.Add(signerDefination1);
        SignerDefination signerDefination2 = new SignerDefination();
        signerDefination2.email = userData?.Email;
        signerDefination2.firstName = userData?.FirstName;
        signerDefination2.lastName = userData?.LastName;
        signerDefination2.SignerName = userData?.FirstName + " " + userData?.LastName;
        signerDefinationList2.Add(signerDefination2);

        dynamic CloneBody = new ExpandoObject();
        dynamic senderData = new ExpandoObject();
        dynamic senderEmail = new ExpandoObject();
        dynamic documentBody = new ExpandoObject();
        dynamic roleBody1 = new ExpandoObject();
        dynamic roleBody2 = new ExpandoObject();
        dynamic roleBody3 = new ExpandoObject();
        documentBody.extract = true;
        documentBody.extractionTypes = new List<String> { "TEXT_TAGS" };
        documentBody.index = 0;
        documentBody.name = "Contract Document - " + agreementRecord.Name;
        roleBody1.id = "Signer1";
        roleBody1.type = "SIGNER";
        roleBody1.name = "Signer1";
        roleBody1.index = 0;
        roleBody1.signers = signerDefinationList1;
        roleBody2.id = "Signer2";
        roleBody2.type = "SIGNER";
        roleBody2.name = "Signer2";
        roleBody2.index = 0;
        roleBody2.signers = signerDefinationList2;
        if (hasSigner3)
        {
          roleBody3.id = "Signer3";
          roleBody3.type = "SIGNER";
          roleBody3.name = "Signer3";
          roleBody3.index = 0;
          roleBody3.signers = signerDefinationList3;
        }
        senderData.senderVisible = false;
        CloneBody.name = "Palmetto Document";
        senderEmail.email = userData.Email;
        CloneBody.sender = senderEmail;
        CloneBody.data = senderData;
        CloneBody.autocomplete = true;
        CloneBody.notarized = false;
        CloneBody.description = "";
        CloneBody.language = "en";
        CloneBody.timezoneId = "GMT";
        CloneBody.due = null;
        CloneBody.visibility = "ACCOUNT";
        CloneBody.type = "PACKAGE";
        CloneBody.status = "SENT";
        if (hasSigner3)
        {
         CloneBody.roles = new object[] { roleBody1,roleBody2,roleBody3 };
        }
        else
        {
          CloneBody.roles = new object[] { roleBody1,roleBody2 };
        }
        

        CloneBody.documents = new object[] { documentBody };
        var Json = JsonSerializer.Serialize(CloneBody);
        tele.AddLog("json2" + Json);

        var fileContent = new ByteArrayContent(getFileAPIResponseResult);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
        var formData = new MultipartFormDataContent{
                                    { fileContent, "File",agreementRecord.Name+" For "+agreementRecord?.Account?.Name+".pdf"},
                                    {new StringContent(Json),"payload"}
                                };
        //Create Conga sign Package
        var postSignApiResponse = await httpHelper.PostAsync($"{Constants.CoreAppsUrl}/api/sign/v1/cs-packages", formData);
        tele.AddLog(postSignApiResponse.StatusCode.ToString());
        var postSignApiResponseResult = await postSignApiResponse.Content.ReadAsStringAsync();
        tele.AddLog(JsonSerializer.Serialize(postSignApiResponseResult));
        var packageId = JsonSerializer.Deserialize<CloneTemplateResponse>(postSignApiResponseResult).id;
        tele.AddLog(packageId);
        
        //Fetch Signing URL
        var getSigningUrlResponse = await httpHelper.GetAsync($"{Constants.CoreAppsUrl}/api/sign/v1/cs-packages/{packageId}/roles/Signer1/signingUrl");
        tele.AddLog(getSigningUrlResponse.StatusCode.ToString());
        var signingUrl = await getSigningUrlResponse.Content.ReadAsStringAsync();
        signingUrl = signingUrl.Substring(1, signingUrl.Length - 2);
        tele.AddLog("signing url " + signingUrl);


        Dictionary<string, Object> agreementToUpdate = new()
                                {
                                    { "Id", RecordIdData },
                                    {"CNGCU_Package_Id_c", packageId}
                                };
        var updatedAgreement = dataAccess.UpdateAgreement(agreementToUpdate, cancellationToken);

        var signingUrlRedirection = new Dictionary<string, dynamic>
            {
                {
                    "respAction",
                    new List<Dictionary<string,dynamic>>
                    {
                        new Dictionary<string, dynamic>
                        {
                            {"type","navigate"},
                            {"data",new Dictionary<string,dynamic>
                            {
                                 {"url",signingUrl},
                                 {"message","Redirecting..."}
                            }
                            }
                        }
                    }
                }
            };
        return signingUrl;
      }
      return "";

    }

    public async Task<string> generateandsigncomposerdocInternal(dynamic actionParams, CancellationToken cancellationToken)
    {
      //Add this check in every data transformation logic
      ThrowIfCancellationRequested(cancellationToken);
      var httpHelper = GetHttpHelper(new Conga.Platform.Extensibility.CustomCode.Library.Models.InternalApiAttributes());
      var httpUtil = new HttpHelperUtil();
      var _telemetryHelper = base.GetTelemetryHelper();
      using var tele = _telemetryHelper.StartActiveSpan("GenerateComposerDoc palmetto");
      tele.AddLog("GenerateComposerDoc Internal Starts");
      var paramsElement = (JsonElement)actionParams;
      var RecordIdData = paramsElement.GetProperty("recordId").GetString();
      tele.AddLog("RecordIDData --- " + RecordIdData);
      var dbHelper = this.GetDataHelper();
      DataAccess dataAccess = new(dbHelper);
      HashSet<string> clauseNames = new HashSet<string>();
      string clausesToQuery = "";
      var queryTermList = dataAccess.GetQueryTerm(cancellationToken).Result;
      tele.AddLog("queryTermList " + JsonSerializer.Serialize(queryTermList));
      var templateId = "";
      HashSet<string> templateIdList = new HashSet<string>();

      //Gather Template Ids and ClauseNames
      foreach (var queryTerm in queryTermList)
      {
        ThrowIfCancellationRequested(cancellationToken);
        if (!string.IsNullOrEmpty(queryTerm.CNGCU_Expression_Condition_c))
        {
          var agreement = dataAccess.GetAgreement(RecordIdData, queryTerm.CNGCU_Expression_Condition_c, cancellationToken).Result.FirstOrDefault();
          tele.AddLog("Agreement " + JsonSerializer.Serialize(agreement));
          if (agreement != null)
          {
            if (queryTerm.CNGCU_Type_c != null && queryTerm.CNGCU_Type_c == "Template")
            {

              // templateIdList.Add(queryTerm.CNGCU_TemplateId_c);
            }
            else if (queryTerm.CNGCU_Type_c != null && queryTerm.CNGCU_Type_c == "Clause")
            {
              string[] clauses = queryTerm.CNGCU_Clauses_c.Split(',').Select(x => $"'{x.Trim()}'").ToArray();
              clauseNames.UnionWith(clauses);
            }
          }
        }
      }
      tele.AddLog("template Id List" + templateIdList);

      if (clauseNames.Count > 0)
      {
        clausesToQuery = "(" + string.Join(",", clauseNames) + ")";
      }
      tele.AddLog($"Clauses {clausesToQuery}");
      List<TemplateQueryModel> clauseList = new List<TemplateQueryModel>();

      //Query Clause Details
      if (clauseNames.Count > 0)
      {
        clauseList = dataAccess.GetTemplate(clausesToQuery, cancellationToken).Result;
        tele.AddLog("clauseList " + JsonSerializer.Serialize(clauseList));
      }


      var agreementRecord = dataAccess.GetAgreementById(RecordIdData, cancellationToken).Result.FirstOrDefault();
      tele.AddLog("Agreement " + JsonSerializer.Serialize(agreementRecord));
      if (agreementRecord.Internal_User_c != null)
      {
        var internalUserData = await httpUtil.DoGet($"{Constants.InstanceURL}/api/user-management/v1/users/" + agreementRecord.Internal_User_c.Id, httpHelper, _telemetryHelper, cancellationToken);
        var internalUserDataResponseResult = await internalUserData.Content.ReadAsStringAsync();
        var userData = JsonSerializer.Deserialize<UserQueryModel>(internalUserDataResponseResult).Data;

      var ownerUserData = await httpUtil.DoGet($"{Constants.InstanceURL}/api/user-management/v1/users/" + agreementRecord.RecordOwner.Id, httpHelper, _telemetryHelper, cancellationToken);
      var ownerUserDataResponseResult = await ownerUserData.Content.ReadAsStringAsync();
      var ownerData = JsonSerializer.Deserialize<UserQueryModel>(ownerUserDataResponseResult).Data;

        var accountRecord = dataAccess.GetAccount(agreementRecord?.Account?.Id, cancellationToken).Result.FirstOrDefault();
        tele.AddLog("accountRecord >>> " + JsonSerializer.Serialize(accountRecord));
        var agreementContactRecord = dataAccess.GetContact(agreementRecord?.PrimaryContact?.Id, cancellationToken).Result.FirstOrDefault();
        tele.AddLog("contactRecord >>> " + JsonSerializer.Serialize(agreementContactRecord));

        //Create Composer Merge Data
        Dictionary<String, dynamic> DataValues = new(){
                {"Name",agreementRecord.Name},
                {"CustomerName_c",agreementRecord.CustomerName_c},
                {"ContractStartDate",agreementRecord.ContractStartDate.ToString("MMMM dd, yyyy")},
                {"ContractEndDate",agreementRecord.ContractEndDate.ToString("MMMM dd, yyyy")},
                {"CustomerState_c",agreementRecord.CustomerState_c},
                {"CustomerStreetAddress_c",agreementRecord.CustomerStreetAddress_c},
                {"CNGCU_Billing_City",accountRecord?.BillingCity},
                {"CustomerZipCode_c",agreementRecord.CustomerZipCode_c},
                {"TotalContractValue",agreementRecord.TotalContractValue?.Value},
                {"Signer2","{{esl:Signer2:Signature}}"},
                // {"InputText", "{{esl:Signer1:textfield:size(654,43),Maxlen(300)}}"},
                // {"Signer1", "{{esl:Signer1:Signature}}"},
                // {"Signer1Name","{{esl:Signer1:SignerName}}"},
                // {"Signer1Date","{{esl:Signer1:SigningDate}}"},
                // {"Signer1Title","{{esl:Signer1:SignerTitle}}"},
                {"Signer2Name","{{esl:Signer2:SignerName}}"},
                {"Signer2Date","{{esl:Signer2:SigningDate}}"},
                {"Signer2Title","{{esl:Signer2:SignerTitle}}"},
                {"Clauses",clauseList},
                {"CNGCU_Record_Owner_Name",agreementRecord.RecordOwner?.Name},
            };
        dynamic composerBody = new ExpandoObject();
        dynamic composerLegacy = new ExpandoObject();
        dynamic composerOutput = new ExpandoObject();
        composerLegacy.Ofn = "RLS_Test_composer";
        composerLegacy.RM = "0";
        composerLegacy.DefaultPDF = "1";
        composerBody.LegacyOptions = composerLegacy;
        if (agreementRecord != null)
        {
          if (agreementRecord?.CNGCU_Template_Selection_c == "Internal Contract Template")
          {
            templateIdList.Add(Constants.InternalContractTemplate);
          }
          else
          {
            templateIdList.Add(Constants.InternalContractTemplate2);
          }
          if(agreementRecord?.CNGCU_Configurations_c == "Pattern B")
          {
            DataValues.Add("InputText", "{{esl:Signer1:textfield:size(654,43),Maxlen(300)}}");
            DataValues.Add("Signer1", "{{esl:Signer1:Signature}}");
          }
        }
        composerBody.JsonData = JsonSerializer.Serialize(DataValues);
        tele.AddLog("jsondata --" + JsonSerializer.Serialize(DataValues));
        composerOutput.generateDocumentDownload = true;
        composerBody.output = composerOutput;

        List<Dictionary<String, String>> composerTemplateList = new List<Dictionary<string, string>>();

        // if (templateIdList.Count == 0)
        // {
        //   templateIdList.Add(templateId);
        // }
        foreach (var template in templateIdList)
        {
          ThrowIfCancellationRequested(cancellationToken);
          Dictionary<String, String> composerTemplate = new(){
                {"IntegrationName","conga"},
                {"fileId",template}
            };
          composerTemplateList.Add(composerTemplate);
        }


        composerBody.TemplateSources = composerTemplateList;
        var composerBodySerialized = JsonSerializer.Serialize(composerBody);
        tele.AddLog(composerBodySerialized);


        //Call Composer API
        var callComposerMergeAPIResponse = await httpUtil.DoPost($"{Constants.CoreAppsUrl}/api/ingress/v1/Merge", composerBodySerialized, httpHelper, _telemetryHelper, cancellationToken);
        var callComposerMergeAPIResponseResult = await callComposerMergeAPIResponse.Content.ReadAsStringAsync();
        var correlationId = JsonSerializer.Deserialize<ComposerMergeQueryModel>(callComposerMergeAPIResponseResult).correlationId;
        tele.AddLog("correlationId " + correlationId);

        //Get Document Generation Status
        var callMergeStatusAPIResponse = await httpUtil.DoGet($"{Constants.CoreAppsUrl}/api/status/v1/status/" + correlationId, httpHelper, _telemetryHelper, cancellationToken);
        var callMergeStatusAPIResponseResult = await callMergeStatusAPIResponse.Content.ReadAsStringAsync();
        var statusMessage = JsonSerializer.Deserialize<MergeStatusResponse>(callMergeStatusAPIResponseResult).message;
        var statusFlag = false;
        while (!statusFlag && statusMessage == "Pending")
        {
          ThrowIfCancellationRequested(cancellationToken);
          await Task.Delay(2000);
          callMergeStatusAPIResponse = await httpUtil.DoGet($"{Constants.CoreAppsUrl}/api/status/v1/status/" + correlationId, httpHelper, _telemetryHelper, cancellationToken);
          callMergeStatusAPIResponseResult = await callMergeStatusAPIResponse.Content.ReadAsStringAsync();
          statusMessage = JsonSerializer.Deserialize<MergeStatusResponse>(callMergeStatusAPIResponseResult).message;
          if (statusMessage == "Completed")
          {
            statusFlag = true;
          }
        }
        tele.AddLog(statusMessage);
        if (statusFlag == true)
        {
          //Get Composer Generated Document File
          var getTransactionAPIResponse = await httpUtil.DoGet($"{Constants.CoreAppsUrl}/api/presigned/v1/presignedservice/" + correlationId, httpHelper, _telemetryHelper, cancellationToken);
          var getTransactionAPIResponseResult = await getTransactionAPIResponse.Content.ReadAsStringAsync();
          var transactionId = JsonSerializer.Deserialize<TransactionResponse>(getTransactionAPIResponseResult).preSignedCreateResponseModel.fileInfoDetailsList[0].transactionId;
          var getFileAPIResponse = await httpUtil.DoGet($"{Constants.CoreAppsUrl}/api/presigned/v1/presignedservice/" + correlationId + "/" + transactionId, httpHelper, _telemetryHelper, cancellationToken);
          var getFileAPIResponseResult = await getFileAPIResponse.Content.ReadAsByteArrayAsync();

          List<SignerDefination> signerDefinationList1 = new List<SignerDefination>();
          List<SignerDefination> signerDefinationList2 = new List<SignerDefination>();
          SignerDefination signerDefination1 = new SignerDefination();
          SignerDefination signerDefination2 = new SignerDefination();
          signerDefination2.email = agreementRecord?.CNGCU_Customer_Email_c;
          signerDefination2.firstName = agreementRecord?.CustomerName_c.Split(' ')[0];
          signerDefination2.lastName = agreementRecord?.CustomerName_c.Split(' ')[1];
          signerDefination2.SignerName = agreementRecord?.CustomerName_c;
          signerDefinationList2.Add(signerDefination2);




          // Creating Data For Conga sign

          dynamic CloneBody = new ExpandoObject();
          dynamic senderData = new ExpandoObject();
          dynamic senderEmail = new ExpandoObject();
          dynamic documentBody = new ExpandoObject();
          dynamic roleBody1 = new ExpandoObject();
          dynamic roleBody2 = new ExpandoObject();
          documentBody.extract = true;
          documentBody.extractionTypes = new List<String> { "TEXT_TAGS" };
          documentBody.index = 0;
          documentBody.name = "Contract Document - " + agreementRecord?.Name;

          roleBody2.id = "Signer2";
          roleBody2.type = "SIGNER";
          roleBody2.name = "Signer2";
          roleBody2.index = 1;
          roleBody2.signers = signerDefinationList2;
          senderData.senderVisible = false;
          CloneBody.name = "Palmetto Document";
          senderEmail.email = ownerData.Email;
          CloneBody.sender = senderEmail;
          CloneBody.data = senderData;
          CloneBody.autocomplete = true;
          CloneBody.notarized = false;
          CloneBody.description = "";
          CloneBody.language = "en";
          CloneBody.timezoneId = "GMT";
          CloneBody.due = null;
          CloneBody.visibility = "ACCOUNT";
          CloneBody.type = "PACKAGE";
          CloneBody.status = "SENT";
          if (agreementRecord?.CNGCU_Configurations_c == "Pattern B")
          {
            roleBody1.id = "Signer1";
            roleBody1.type = "SIGNER";
            roleBody1.name = "Signer1";
            roleBody1.index = 0;
            roleBody1.signers = signerDefinationList1;

            signerDefination1.email = userData?.Email;
            signerDefination1.firstName = userData?.FirstName;
            signerDefination1.lastName = userData?.LastName;
            signerDefination1.SignerName = userData?.FirstName + " " + userData?.LastName;
            signerDefinationList1.Add(signerDefination1);
            CloneBody.roles = new object[] { roleBody1, roleBody2 };

          }
          else
          {
            CloneBody.roles = new object[] { roleBody2 };

          }
          CloneBody.documents = new object[] { documentBody };
          var Json = JsonSerializer.Serialize(CloneBody);
          tele.AddLog("json2" + Json);

          var fileContent = new ByteArrayContent(getFileAPIResponseResult);
          fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
          var formData = new MultipartFormDataContent{
                                    { fileContent, "File",agreementRecord.Name+" For "+agreementRecord?.Account?.Name+".pdf"},
                                    {new StringContent(Json),"payload"}
                                };
          //Create Conga sign Package
          var postSignApiResponse = await httpHelper.PostAsync($"{Constants.CoreAppsUrl}/api/sign/v1/cs-packages", formData);
          tele.AddLog(postSignApiResponse.StatusCode.ToString());
          var postSignApiResponseResult = await postSignApiResponse.Content.ReadAsStringAsync();
          tele.AddLog(JsonSerializer.Serialize(postSignApiResponseResult));
          var packageId = JsonSerializer.Deserialize<CloneTemplateResponse>(postSignApiResponseResult).id;
          tele.AddLog(packageId);

          //Fetch Signing URL
          var signingUrl = "";
          if (agreementRecord?.CNGCU_Configurations_c == "Pattern B")
          {
            var getSigningUrlResponse = await httpHelper.GetAsync($"{Constants.CoreAppsUrl}/api/sign/v1/cs-packages/{packageId}/roles/Signer1/signingUrl");
            tele.AddLog(getSigningUrlResponse.StatusCode.ToString());
            signingUrl = await getSigningUrlResponse.Content.ReadAsStringAsync();
          }
          else
          {
            var getSigningUrlResponse = await httpHelper.GetAsync($"{Constants.CoreAppsUrl}/api/sign/v1/cs-packages/{packageId}/roles/Signer2/signingUrl");
            tele.AddLog(getSigningUrlResponse.StatusCode.ToString());
            signingUrl = await getSigningUrlResponse.Content.ReadAsStringAsync();
          }
          signingUrl = signingUrl.Substring(1, signingUrl.Length - 2);
          tele.AddLog("signing url " + signingUrl);


          Dictionary<string, Object> agreementToUpdate = new()
                                {
                                    { "Id", RecordIdData },
                                    {"CNGCU_Package_Id_c", packageId}
                                };
          var updatedAgreement = dataAccess.UpdateAgreement(agreementToUpdate, cancellationToken);

          var signingUrlRedirection = new Dictionary<string, dynamic>
            {
                {
                    "respAction",
                    new List<Dictionary<string,dynamic>>
                    {
                        new Dictionary<string, dynamic>
                        {
                            {"type","navigate"},
                            {"data",new Dictionary<string,dynamic>
                            {
                                 {"url",signingUrl},
                                 {"message","Redirecting..."}
                            }
                            }
                        }
                    }
                }
            };
          return signingUrl;
        }

      }
      return "";
    }

  
    public Dictionary<string, dynamic> ToasterMesage(string messageType, string message,CancellationToken cancellationToken)
    {
      ThrowIfCancellationRequested(cancellationToken);
      var result = new Dictionary<string, dynamic> {
          {
            "respAction",
            new List < Dictionary < string,
            dynamic >> {
              new Dictionary < string,
              dynamic > {
                {
                  "type","showMessage"
                },
                {
                  "data",
                  new Dictionary < string,
                  string > {
                    {
                      "messageType",messageType
                    },
                    {
                      "message",message
                    }
                  }
                }
              }
            }
          }
        };
      return result;
    }
  }
}
