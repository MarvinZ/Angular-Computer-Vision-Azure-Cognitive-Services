using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using ngComputerVision.Models;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using ngComputerVision.DTOModels;
using System.Linq;

namespace ngComputerVision.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class OCRController : Controller
    {
        static string subscriptionKey;
        static string endpoint;
        static string uriBase;

        public OCRController()
        {
            subscriptionKey = "37b5da280e9b4042b0aa43c434ca00f0";
            endpoint = "https://visionwo.cognitiveservices.azure.com/";
            uriBase = endpoint + "vision/v2.0/describe?maxCandidates=2";
        }

        [HttpPost, DisableRequestSizeLimit]
        public async Task<OcrResultDTO> Post()
        {
            var GeneralRepairKeyWords = new string[] { "hammer", "saw", "nail" };
            var HVACKeyWords = new string[] {  "fan", "air",  "conditioning", "heat", "HVAC", 
                                                "unit", "leak", "leaking", "cool", "heat", "cooling", "heating", 
                                                "hot", "AC", "thermostat", "RTU", "humidity", "warm", "humid", "EMS", 
                                                "vent", "ventilation", "roof", "top", "refrigerant", "broken", "repair",
                                                "clock", "white", "remote", "control", "outdoor", "building", "refrigerator", "microwave",  };
            var JanitorialKeyWords = new string[] { "janitor", "floor", "broom", "dirty", "wet", "mop" };
            var LighhtingKeyWords = new string[] { "light", "bulb", "fluorescent", "sign", "lamp", "mercury", "tube" };
            var LocksmithKeyWords = new string[] { "lock", "door", "locksmith", "key" };
            var PlumbingKeyWords = new string[] { "faucet", "plumber", "pipe", "sink", "water" };
            var PestControlKeyWords = new string[] { "insect", "rat", "mouse", "honeycomb", "ant", "bee", "animal", "mammal", "rodent", "bird"};
            var AutoDoorsgKeyWords = new string[] { "door", "autodoor", "transparent", "glass", "building" };
            var ElectricalKeyWords = new string[] { "wire", "electricity", "circuit" };
            var FireSafetyKeyWords = new string[] { "fire", "extinguisher", "safety" };

            var GeneralRepairScore = 0;
            var HVACScore = 0;
            var JanitorialScore = 0;
            var LighhtingScore = 0;
            var LocksmithScore = 0;
            var PlumbingScore = 0;
            var PestControlScore = 0;
            var AutoDoorsgScore = 0;
            var ElectricalScore = 0;
            var FireSafetyScore = 0;

      






        StringBuilder sb = new StringBuilder();
            OcrResultDTO ocrResultDTO = new OcrResultDTO();
            try
            {
                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[Request.Form.Files.Count - 1];

                    if (file.Length > 0)
                    {
                        var memoryStream = new MemoryStream();
                        file.CopyTo(memoryStream);
                        byte[] imageFileBytes = memoryStream.ToArray();
                        memoryStream.Flush();

                        string JSONResult = await ReadTextFromStream(imageFileBytes);

                        ImageDescription imgDesc = JsonConvert.DeserializeObject<ImageDescription>(JSONResult);
                        ImageAnalysis imageAnalysis = JsonConvert.DeserializeObject<ImageAnalysis>(JSONResult);
                        TagResult TagResult = JsonConvert.DeserializeObject<TagResult>(JSONResult);
                        OcrResult ocrResult = JsonConvert.DeserializeObject<OcrResult>(JSONResult);

                        if (imageAnalysis.Description.Tags.Count > 0)
                        {
                            foreach (var taggy in imageAnalysis.Description.Tags)
                            {
                                if (GeneralRepairKeyWords.Contains(taggy)) GeneralRepairScore++;
                                if (HVACKeyWords.Contains(taggy)) HVACScore++;
                                if (JanitorialKeyWords.Contains(taggy)) JanitorialScore++;
                                if (LighhtingKeyWords.Contains(taggy)) LighhtingScore++;
                                if (LocksmithKeyWords.Contains(taggy)) LocksmithScore++;
                                if (PlumbingKeyWords.Contains(taggy)) PlumbingScore++;
                                if (PestControlKeyWords.Contains(taggy)) PestControlScore++;
                                if (AutoDoorsgKeyWords.Contains(taggy)) AutoDoorsgScore++;
                                if (ElectricalKeyWords.Contains(taggy)) ElectricalScore++;
                                if (FireSafetyKeyWords.Contains(taggy)) FireSafetyScore++;
                                sb.Append(taggy);
                                sb.Append(' ');
                            }
                        }
                        if (imageAnalysis.Description.Captions.Count > 0)
                        {
                            foreach (var capi in imageAnalysis.Description.Captions)
                            {
                                sb.Append("---");
                                sb.Append(capi.Text);
                                sb.Append(' ');
                            }
                        }
                        var ppp = new p();
                        ppp.GeneralRepairScore = GeneralRepairScore;
                        ppp.HVACScore = HVACScore;
                        ppp.JanitorialScore = JanitorialScore;
                        ppp.LighhtingScore = LighhtingScore;
                        ppp.LocksmithScore = LocksmithScore;
                        ppp.PlumbingScore = PlumbingScore;
                        ppp.PestControlScore = PestControlScore;
                        ppp.AutoDoorsgScore = AutoDoorsgScore;
                        ppp.ElectricalScore = ElectricalScore;
                        ppp.FireSafetyScore = FireSafetyScore;

           

                        var winner = ppp.GetType().GetFields().OrderByDescending(f => f.GetValue(ppp)).First().Name;
                        var selectedRTRC = "Unable to decide...";
                        switch (winner)
                        {


                            case "GeneralRepairScore":
                                selectedRTRC = "General Repair";
                                break;
                            case "HVACScore":
                                selectedRTRC = "HVAC";
                                break;
                            case "JanitorialScore":
                                selectedRTRC = "Janitorial";
                                break;
                            case "LighhtingScore":
                                selectedRTRC = "Lighting";
                                break;
                            case "LocksmithScore":
                                selectedRTRC = "Locksmith";
                                break;
                            case "PlumbingScore":
                                selectedRTRC = "Plumbing";
                                break;
                            case "PestControlScore":
                                selectedRTRC = "Pest Control";
                                break;
                            case "AutoDoorsgScore":
                                selectedRTRC = "Autodoors";
                                break;
                            case "ElectricalScore":
                                selectedRTRC = "Electrical";
                                break;
                            case "FireSafetyScore":
                                selectedRTRC = "Fire Safety";
                                break;
                            default:
                                break;


                        }

                        sb.Append("------------------------------------------->");
                        sb.Append(selectedRTRC);
                        sb.Append(' ');

                        

                        //if (!ocrResult.Language.Equals("unk"))
                        //{
                        //    foreach (OcrLine ocrLine in ocrResult.Regions[0].Lines)
                        //    {
                        //        foreach (OcrWord ocrWord in ocrLine.Words)
                        //        {
                        //            sb.Append(ocrWord.Text);
                        //            sb.Append(' ');
                        //        }
                        //        sb.AppendLine();
                        //    }
                        //}
                        //else
                        //{
                        //    sb.Append("This language is not supported.");
                        //}
                        ocrResultDTO.DetectedText = sb.ToString();
                        ocrResultDTO.Language = ocrResult.Language;
                    }
                }
                return ocrResultDTO;
            }
            catch
            {
                ocrResultDTO.DetectedText = "Error occurred. Try again";
                ocrResultDTO.Language = "unk";
                return ocrResultDTO;
            }
        }

        static async Task<string> ReadTextFromStream(byte[] byteData)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                string requestParameters = "language=unk&detectOrientation=true";
                string uri = uriBase + "?" + requestParameters;
                HttpResponseMessage response;

                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response = await client.PostAsync(uri, content);
                }

                string contentString = await response.Content.ReadAsStringAsync();
                string result = JToken.Parse(contentString).ToString();
                return result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        [HttpGet]
        public async Task<List<AvailableLanguageDTO>> GetAvailableLanguages()
        {
            string endpoint = "https://api.cognitive.microsofttranslator.com/languages?api-version=3.0&scope=translation";
            var client = new HttpClient();
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(endpoint);
                var response = await client.SendAsync(request).ConfigureAwait(false);
                string result = await response.Content.ReadAsStringAsync();

                AvailableLanguage deserializedOutput = JsonConvert.DeserializeObject<AvailableLanguage>(result);

                List<AvailableLanguageDTO> availableLanguage = new List<AvailableLanguageDTO>();

                foreach (KeyValuePair<string, LanguageDetails> translation in deserializedOutput.Translation)
                {
                    AvailableLanguageDTO language = new AvailableLanguageDTO();
                    language.LanguageID = translation.Key;
                    language.LanguageName = translation.Value.Name;

                    availableLanguage.Add(language);
                }
                return availableLanguage;
            }
        }
    }


   public  struct p
    {
        public int GeneralRepairScore, HVACScore, JanitorialScore, LighhtingScore, LocksmithScore,
            PlumbingScore, PestControlScore, AutoDoorsgScore, ElectricalScore, FireSafetyScore;
    }


}
