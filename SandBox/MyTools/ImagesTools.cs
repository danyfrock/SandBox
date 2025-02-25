using Microsoft.Extensions.Configuration;
using Tesseract;

namespace SandBox.MyTools
{
    public static class ImagesTools
    {

        public static string ExtractTextFromImage(string imagePath)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("MyTools/appsettings.json")
                .Build();

            string tessDataPath = configuration["Tesseract:TessDataPath"] ?? string.Empty;
            ////string tessDataPath = @"C:\Users\daniel.lorenzi\.nuget\packages\tesseract\5.2.0\tessdata"; // Chemin vers le dossier tessdata

            using (var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(imagePath)) // Utilisation de Leptonica pour charger l'image
                {
                    using (var page = engine.Process(img))
                    {
                        return page.GetText();
                    }
                }
            }
        }
    }//classe
}//namespace
