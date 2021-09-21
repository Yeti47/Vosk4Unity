using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Unity.SpeechRecognition.Util
{
    public static class DirectoryUtil
    {
        
        public static void CopyDirectory(DirectoryInfo source, DirectoryInfo target, Predicate<FileInfo> filePredicate = null) {

            filePredicate = filePredicate ?? (f => true);

            Directory.CreateDirectory(target.FullName);

            foreach (FileInfo fileInfo in source.GetFiles().Where(f => filePredicate.Invoke(f)))
            {
                string outPath = Path.Combine(target.FullName, fileInfo.Name);
                fileInfo.CopyTo(outPath, true);
            }

            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories()) {

                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyDirectory(diSourceSubDir, nextTargetSubDir, filePredicate);
            }
        }

        public static void CopyDirectory(string sourceDirectory, string targetDirectory, Predicate<FileInfo> filePredicate = null) {

            DirectoryInfo sourceDirectoryInfo = new DirectoryInfo(sourceDirectory);
            DirectoryInfo targetDirectoryInfo = new DirectoryInfo(targetDirectory);

            CopyDirectory(sourceDirectoryInfo, targetDirectoryInfo, filePredicate);
        }

    }
}
