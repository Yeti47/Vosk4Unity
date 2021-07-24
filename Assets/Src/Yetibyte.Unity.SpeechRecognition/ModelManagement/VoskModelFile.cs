using System.Linq;
using System.IO;

namespace Yetibyte.Unity.SpeechRecognition.ModelManagement
{
    public class VoskModelFile
    {
        private float _size = -1;

        public string Path { get; }

        public float Size
        {
            get
            {
                if (_size < 0)
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(Path);
                    _size = dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(f => f.Length / 1000f / 1000f);
                }

                return _size;
            }
        }
        public string Name => System.IO.Path.GetFileName(Path);

        public VoskModelFile(string path)
        {
            Path = path ?? string.Empty;
        }

    }
}
