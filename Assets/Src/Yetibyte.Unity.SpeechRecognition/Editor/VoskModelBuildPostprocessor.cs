using System;
using System.Linq;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Yetibyte.Unity.SpeechRecognition.Util;

namespace Yetibyte.Unity.SpeechRecognition.Editor
{
    public class VoskModelBuildPostprocessor : UnityEditor.Build.IPostprocessBuildWithReport
    {
        private const string BUILD_PIPELINE_PREFIX = "Vosk4Unity - Build Pipeline";
        private const string META_FILE_EXTENSION = ".meta";

        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            if(report.summary.platform == UnityEditor.BuildTarget.StandaloneWindows || report.summary.platform == UnityEditor.BuildTarget.StandaloneWindows64)
            {
                string dataDirectoryName = Application.productName + "_Data";

                string dataPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(report.summary.outputPath), dataDirectoryName);

                string modelPath = VoskModelManagerSettings.GetOrCreateSettings().AbsoluteModelDirectoryPath;

                if(System.IO.Directory.Exists(modelPath))
                {
                    string outputModelPath = System.IO.Path.Combine(dataPath, System.IO.Path.GetFileName(modelPath));

                    Debug.Log($"{BUILD_PIPELINE_PREFIX}: Copying Vosk models from path '{modelPath}' to output path'{outputModelPath}'...");
                    
                    try
                    {
                        DirectoryUtil.Copy(modelPath, outputModelPath, f => f.Extension.ToLower() != META_FILE_EXTENSION);
                    }
                    catch(Exception ex)
                    {
                        Debug.LogError($"{BUILD_PIPELINE_PREFIX}: Failed to copy Vosk models to output directory. Error: {ex.Message}");
                        return;
                    }

                    Debug.Log($"{BUILD_PIPELINE_PREFIX}: Vosk models copied successfully.");

                }
                else
                {
                    Debug.Log($"{BUILD_PIPELINE_PREFIX}: Vosk model path '{modelPath}' does not exist. Will not copy models to output directory.");
                }


            }
            else
            {
                UnityEngine.Debug.LogWarning("Vosk4Unity currently only supports Windows target platforms. Speech recognition will not work properly in the built application.");
            }
            
        }
    }
}
