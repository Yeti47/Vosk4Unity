using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System.IO;

namespace Yetibyte.Unity.SpeechRecognition.ModelManagement
{
    public class HttpVoskModelInfo : IVoskModelInfo
    {
        private const string DOWNLOAD_FILE_EXTENSION = ".zip";

        public string Category { get; private set; }
        public string Name { get; private set; }

        public string Url { get; private set; }

        public float Size { get; private set; }

        public VoskModelDetails Details { get; private set; }

        public event EventHandler<VoskModelImportEventArgs> ImportComplete;

        public HttpVoskModelInfo(string category, string name, string url, float size, VoskModelDetails details)
        {
            Category = category;
            Name = name;
            Url = url;
            Size = size;
            Details = details;
        }

        public virtual bool Import(string targetPath, out string message)
        {
            message = string.Empty;

            string fullPath = Path.Combine(UnityEngine.Application.dataPath, targetPath);
            string fullFilePath = Path.Combine(fullPath, this.Name + DOWNLOAD_FILE_EXTENSION);

            if (Directory.Exists(Path.Combine(fullPath, Path.GetFileNameWithoutExtension(fullFilePath))))
            {
                message = $"Model '{Name}' already exists in model path.";
                return false;
            }

            UnityWebRequest request = new UnityWebRequest(Url, "GET") { downloadHandler = new DownloadHandlerBuffer() };
            
            UnityWebRequestAsyncOperation asyncOperation;

            try
            {
                asyncOperation = request.SendWebRequest();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
                request.Dispose();
                message = ex.Message;
                return false;
            }
            asyncOperation.completed += op =>
            {
                bool isErr = false;
                string errMsg = string.Empty;

                try
                {
                    File.WriteAllBytes(fullFilePath, request.downloadHandler.data);

                    ZipFileExtractor zipFileExtractor = ZipFileExtractor.Create();
                    zipFileExtractor.ExtractToDirectory(fullFilePath, fullPath);

                    //System.IO.Compression.ZipFile.ExtractToDirectory(fullFilePath, fullPath);

                    File.Delete(fullFilePath);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogException(ex);
                    isErr = true;
                    errMsg = ex.Message;
                }

                isErr |= (request.responseCode < 200 || request.responseCode > 299);

                if (isErr && string.IsNullOrWhiteSpace(errMsg))
                    errMsg = "Error processing the request.";

                request.Dispose();

                OnImportComplete(errMsg, isErr);
            };

            return true;

        }

        protected virtual void OnImportComplete(string message, bool isError)
        {
            var handler = ImportComplete;
            handler?.Invoke(this, new VoskModelImportEventArgs(this, message, isError));
        }

    }
}
