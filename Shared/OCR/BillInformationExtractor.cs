using System.Buffers.Text;
using Tesseract;

namespace ExpenditureTrackerWeb.Shared.OCR
{
    public interface IBillInformationExtractor
    {
        public string BillInformationOCRExtractor(byte[] imageBytes);
    }
    public class BillInformationExtractor : IBillInformationExtractor
    {
        public string BillInformationOCRExtractor(byte[] imageBytes)
        {
            string tessDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");

            using (var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default))
            using (var img = Pix.LoadFromMemory(imageBytes))
            using (var page = engine.Process(img))
            {
                return page.GetText();
            }
        }
    }
}
