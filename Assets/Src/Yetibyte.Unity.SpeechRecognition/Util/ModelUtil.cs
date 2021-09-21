using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Yetibyte.Unity.SpeechRecognition.Util
{
    public static class ModelUtil
    {

        public const string MODEL_VALID_CHECK_DIR_NAME = "ivector";

        public static string GetAbsoluteModelPathByRelativePath(string modelPath) => string.IsNullOrWhiteSpace(modelPath) ? null : System.IO.Path.Combine(Application.dataPath, modelPath);

        public static bool ModelPathExists(string modelPath)
        {
            string absolutePath = ModelUtil.GetAbsoluteModelPathByRelativePath(modelPath);

            return IsValidModelDirectory(absolutePath);

        }

        public static bool IsValidModelDirectory(string path)
        {
            if (!System.IO.Directory.Exists(path))
                return false;

            var subDirs = System.IO.Directory.EnumerateDirectories(path);

            bool isValid = subDirs.Any(d => System.IO.Path.GetFileName(d).ToLower() == MODEL_VALID_CHECK_DIR_NAME);

            return isValid;
        }

    }
}
