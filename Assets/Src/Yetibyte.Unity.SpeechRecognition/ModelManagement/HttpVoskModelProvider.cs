using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Yetibyte.Unity.SpeechRecognition.ModelManagement
{
    public class HttpVoskModelProvider : IVoskModelProvider
    {
        private static readonly string[] UNSUPPORTED_MODELS = new[] { "fr-pguyot-zamia-20191016-tdnn_f" };

        private const string MODEL_URL = "https://alphacephei.com/vosk/models";

        private readonly List<IVoskModelInfo> _models = new List<IVoskModelInfo>();

        public VoskModelProviderStatus Status { get; private set; } = VoskModelProviderStatus.Initial;

        public IEnumerable<IVoskModelInfo> Models => _models;

        public string Name => "Http Vosk Model Provider";

        public string Source => "alphacephei.com";

        public static bool IsModelSupported(string modelName)
        {
            return !string.IsNullOrWhiteSpace(modelName) && !UNSUPPORTED_MODELS.Contains(modelName);
        }

        public bool FetchModels()
        {
            _models.Clear();

            Status = VoskModelProviderStatus.Processing;

            UnityWebRequest request = new UnityWebRequest(MODEL_URL, "GET") { downloadHandler = new DownloadHandlerBuffer() };

            var asyncOperation = request.SendWebRequest();

            asyncOperation.completed += op =>
            {
                if (!request.downloadHandler.isDone)
                {
                    Status = VoskModelProviderStatus.Error;
                    request.Dispose();
                    return;
                }

                if (request.responseCode == 200)
                {
                    string responseBody = request.downloadHandler.text;

                    Match tableMatch = Regex.Match(responseBody, @"<table(.|\n)+/table>", RegexOptions.Multiline);

                    string tableXml = tableMatch.Value;

                    XElement tableElement = XElement.Parse(tableXml);

                    string category = string.Empty;

                    foreach (var tableRowElement in tableElement.Element("tbody").Elements("tr"))
                    {
                        string categoryTemp = tableRowElement.Element("td")?.Element("strong")?.Value;

                        if (!string.IsNullOrWhiteSpace(categoryTemp))
                        {
                            category = categoryTemp;
                            continue;
                        }

                        string modelName = tableRowElement.Elements("td").ElementAt(0)?.Element("a")?.Value?.Trim()?.Replace(".zip", string.Empty);

                        if (!IsModelSupported(modelName))
                            continue; // Skip if not supported by this provider.
                        
                        string modelUrl = tableRowElement.Elements("td").ElementAt(0)?.Element("a")?.Attribute("href")?.Value?.Trim();
                        string sizeText = tableRowElement.Elements("td").ElementAt(1)?.Value?.Trim();
                        string technicalDetails = tableRowElement.Elements("td").ElementAt(2)?.Value?.Trim();
                        string notes = tableRowElement.Elements("td").ElementAt(3)?.Value?.Trim();
                        string license = tableRowElement.Elements("td").ElementAt(4)?.Value?.Trim();

                        float modelSize = 0;

                        Match sizeMatch = Regex.Match(sizeText, @"\d*\.?\d*");

                        if (sizeMatch.Success)
                        {
                            modelSize = float.Parse(sizeMatch.Value, new System.Globalization.CultureInfo("en-US"));

                            if (sizeText.ToUpper().Contains("G"))
                            {
                                modelSize *= 1000f;
                            }

                        }

                        VoskModelDetails voskModelDetails = new VoskModelDetails(technicalDetails, notes, license);

                        HttpVoskModelInfo voskModelInfo = new HttpVoskModelInfo(category, modelName, modelUrl, modelSize, voskModelDetails);

                        _models.Add(voskModelInfo);

                    }

                    Status = VoskModelProviderStatus.Done;

                }
                else
                {
                    Status = VoskModelProviderStatus.Error;
                }

                request.Dispose();

            };

            return true;
        }
    }
}
