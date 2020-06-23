using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ngComputerVision.DTOModels;
using ngComputerVision.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ngComputerVision.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ImageController : Controller
    {
        static string subscriptionKey;
        static string endpoint;
        static string uriBase;


        string[] GeneralRepairKeyWords = new string[] { "HAMMER", "SAW", "NAIL" };
        string[] HVACKeyWords = new string[] {  "FAN", "AIR",  "CONDITIONING", "HEAT", "HVAC", "AIR CONDITIONER", "APPLIANCE",
                                                "UNIT", "LEAK", "LEAKING", "COOL", "COOLING", "HEATING",
                                                "HOT", "AC", "thermostat", "RTU", "HUMIDITY", "WARM", "HUMID", "EMS",
                                                "VENT", "VENTILATION", "ROOF", "TOP", "REFRIGERANT", "BROKEN", "REPAIR",
                                                "CLOCK", "WHITE", "REMOTE", "CONTROL", "OUTDOOR", "BUILDING", "REFRIGERATOR", "MICROWAVE",
        "ELECTRIC FAN", "DIGITAL CLOCK"};
        string[] JanitorialKeyWords = new string[] { "JANITOR", "FLOOR", "BROOM", "dirty", "WET", "MOP" };
        string[] LighhtingKeyWords = new string[] { "LIGHT", "BULB", "FLUORESCENT", "SIGN", "LAMP", "MERCURY", "tube" };
        string[] LocksmithKeyWords = new string[] { "LOCK", "DOOR", "LOCKSMITH", "KEY" };
        string[] PlumbingKeyWords = new string[] { "SINK FAUCET", "FAUCET", "PLUMBER", "PIPE", "SINK", "WATER", "BATHROOM", "TOILET", "INDOOR",
                                                    "PLUMBING", "RESTROOM"};
        string[] PestControlKeyWords = new string[] { "INSECT", "RAT", "MOUSE", "HONEYCOMB", "ANT", "BEE", "ANIMAL", "MAMMAL", "RODENT", "BIRD", "NEST",
                                                    "HIVE" };
        string[] AutoDoorsgKeyWords = new string[] { "DOOR", "AUTODOOR", "TRANSPARENT", "glass", "building" };
        string[] ElectricalKeyWords = new string[] { "WIRE", "ELECTRICITY", "CIRCUIT", "WIRING", "CABLE", "ELECTRONICS" };
        string[] FireSafetyKeyWords = new string[] { "fire", "EXTINGUISHER", "SAFETY" };



        public ImageController()
        {
            subscriptionKey = "37b5da280e9b4042b0aa43c434ca00f0";
            endpoint = "https://visionwo.cognitiveservices.azure.com/";
            uriBase = endpoint + "vision/v2.0/describe?maxCandidates=2";
        }

        [HttpPost]

        public async Task<OcrResultDTO> Post([FromBody] myImg image)
        {
            StringBuilder sb = new StringBuilder();
            OcrResultDTO ocrResultDTO = new OcrResultDTO();
            //  var baseURLImage = "https://www.fmpilot2.com/Attachment/IFM/AAR-PV08/WEB-1721300/1[20200616_182215543].jpg";
            try
            {
                if (!string.IsNullOrEmpty(image.filename))
                {

                    string JSONResult = await ReadTextFromStreamAzureUrl(image.filename);
                    //string JSONResult2 = await ReadTextFromStreamAWS(ImageUrl);

                    ImageDescription imgDescAzure = JsonConvert.DeserializeObject<ImageDescription>(JSONResult);
                    // var AWSList = JsonConvert.DeserializeObject<List<string>>(JSONResult2);
                    ImageAnalysis imageAnalysis = JsonConvert.DeserializeObject<ImageAnalysis>(JSONResult);
                    TagResult TagResult = JsonConvert.DeserializeObject<TagResult>(JSONResult);
                    //OcrResult ocrResult = JsonConvert.DeserializeObject<OcrResult>(JSONResult);
                    var AzureList = new List<string>();
                    if (imageAnalysis.Description != null)
                    {
                        foreach (var item in imageAnalysis.Description.Tags)
                        {
                            AzureList.Add(item);
                        }
                        var selectedRTRCAzure = GetWinner(AzureList);
                        //    var selectedRTRCAWS = GetWinner(AWSList);


                        sb.Append("*********************Azure*********************** ");
                        sb.Append("\n");
                        sb.Append(selectedRTRCAzure);
                        sb.Append("\n");

                        sb.Append("**********************AWS************************ ");
                        sb.Append("\n");
                        //   sb.Append(selectedRTRCAWS);
                        sb.Append("\n");



                        ocrResultDTO.DetectedText = sb.ToString();
                    }
                    else
                    {

                        ocrResultDTO.DetectedText = "Cannot process this image";
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


        private string GetWinner(List<string> listToEvaluate)
        {
            StringBuilder sb = new StringBuilder();

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


            if (listToEvaluate.Count > 0)
            {
                foreach (var taggy in listToEvaluate)
                {
                    if (GeneralRepairKeyWords.Contains(taggy.ToUpper())) GeneralRepairScore++;
                    if (HVACKeyWords.Contains(taggy.ToUpper())) HVACScore++;
                    if (JanitorialKeyWords.Contains(taggy.ToUpper())) JanitorialScore++;
                    if (LighhtingKeyWords.Contains(taggy.ToUpper())) LighhtingScore++;
                    if (LocksmithKeyWords.Contains(taggy.ToUpper())) LocksmithScore++;
                    if (PlumbingKeyWords.Contains(taggy.ToUpper())) PlumbingScore++;
                    if (PestControlKeyWords.Contains(taggy.ToUpper())) PestControlScore++;
                    if (AutoDoorsgKeyWords.Contains(taggy.ToUpper())) AutoDoorsgScore++;
                    if (ElectricalKeyWords.Contains(taggy.ToUpper())) ElectricalScore++;
                    if (FireSafetyKeyWords.Contains(taggy.ToUpper())) FireSafetyScore++;
                    sb.Append(taggy);
                    sb.Append(" - ");
                }
            }

            var ppp = new p
            {
                GeneralRepairScore = GeneralRepairScore,
                HVACScore = HVACScore,
                JanitorialScore = JanitorialScore,
                LighhtingScore = LighhtingScore,
                LocksmithScore = LocksmithScore,
                PlumbingScore = PlumbingScore,
                PestControlScore = PestControlScore,
                AutoDoorsgScore = AutoDoorsgScore,
                ElectricalScore = ElectricalScore,
                FireSafetyScore = FireSafetyScore
            };

            var winner = ppp.GetType().GetFields().OrderByDescending(f => f.GetValue(ppp)).First().Name;
            var selectedRTRC = "None";
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

            sb.Append("==========>");
            sb.Append(selectedRTRC);
            return sb.ToString();
        }

        static async Task<string> ReadTextFromStreamAzure(byte[] byteData)
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
        static async Task<string> ReadTextFromStreamAWS(byte[] byteData)
        {
            try
            {
                using (var imageCient = new AmazonRekognitionClient())
                {
                    var stream = new MemoryStream(byteData);

                    //if using .NET Core, make sure to use await keyword and async method
                    var detectResponses = await imageCient.DetectLabelsAsync(new DetectLabelsRequest
                    {
                        MinConfidence = 75,
                        Image = new Image
                        {
                            Bytes = stream
                        }
                    });

                    var contentString = new List<string>();
                    foreach (var item in detectResponses.Labels)
                    {
                        contentString.Add(item.Name);
                    }

                    var json = JsonConvert.SerializeObject(contentString);
                    return json;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }


        static async Task<string> ReadTextFromStreamAzureUrl(string imgUrl)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                string requestParameters = "language=unk&detectOrientation=true";
                string uri = uriBase + "?" + requestParameters;
                HttpResponseMessage response;

                using (var webClient = new System.Net.WebClient())
                {
                    byte[] byteData = webClient.DownloadData(imgUrl);



                    using (ByteArrayContent content = new ByteArrayContent(byteData))
                    {
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        response = await client.PostAsync(uri, content);
                    }
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





}
