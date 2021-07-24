namespace Yetibyte.Unity.SpeechRecognition.ModelManagement
{
    public class VoskModelDetails
    {
        public string TechnicalDetails { get; private set; }
        public string Notes { get; private set; }
        public string License { get; private set; }

        public VoskModelDetails(string technicalDetails, string notes, string license)
        {
            TechnicalDetails = technicalDetails;
            Notes = notes;
            License = license;
        }

    }
}
