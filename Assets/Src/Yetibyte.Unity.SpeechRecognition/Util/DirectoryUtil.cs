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
        public static void Copy(string sourceDirectory, string targetDirectory, Predicate<FileInfo> filePredicate = null)
        {
            DirectoryInfo sourceDirectoryInfo = new DirectoryInfo(sourceDirectory);
            DirectoryInfo targetDirectoryInfo = new DirectoryInfo(targetDirectory);

            CopyAll(sourceDirectoryInfo, targetDirectoryInfo, filePredicate);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target, Predicate<FileInfo> filePredicate = null)
        {
            filePredicate = filePredicate ?? (f => true);

            Directory.CreateDirectory(target.FullName);

            foreach (FileInfo fi in source.GetFiles().Where(f => filePredicate.Invoke(f)))
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

    }
}
