using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Google.Cloud.Vision.V1;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zwift.Functions.Users.Models;
using Image = Google.Cloud.Vision.V1.Image;

namespace Zwift.Functions.Users
{
    public static class DetectRoutes
    {
        [FunctionName("DetectRoutes")]
        public static async System.Threading.Tasks.Task Run(
            [BlobTrigger("pending/{name}")]Stream image,
            [Queue(Constants.QUEUE_NAME)] IAsyncCollector<string> applicationQueue,
            string name, ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {image.Length} Bytes");

            var sourceStream = new MemoryStream();
            await image.CopyToAsync(sourceStream);


            var bitmap = new Bitmap(sourceStream);

            var customVisionPredictionClient = new CustomVisionPredictionClient
            {
                ApiKey = "24d9d88ffd39465c90e25a3af135ca40",
                Endpoint = "https://zwiftcustomvision.cognitiveservices.azure.com/"
            };

            sourceStream.Position = 0;

            var response = await customVisionPredictionClient.DetectImageAsync(Guid.Parse("46ff2c6b-1eef-4133-acf9-a9e12b8d8ea2"), "Completed Route",
                sourceStream);

            var computerVisionClient =
                new ComputerVisionClient(new ApiKeyServiceClientCredentials("4865218f4e144b70a87e6476157a3e33"))
                { Endpoint = "https://zwiftcomputervision.cognitiveservices.azure.com/" };

            var routes = new List<string>();

            foreach (var predictionModel in response.Predictions)
            {
                if (predictionModel.TagName == "Completed Route" && predictionModel.Probability > 0.85)
                {
                    var cropped = CropBitmap(bitmap,
                        predictionModel.BoundingBox.Left,
                        predictionModel.BoundingBox.Top,
                        predictionModel.BoundingBox.Width,
                        predictionModel.BoundingBox.Height);

                    var memoryStream = new MemoryStream();
                    //ONLY FOR DEBUG
                    //cropped.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),Guid.NewGuid().ToString()));
                    cropped.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    memoryStream.Position = 0;

                    //https://stackoverflow.com/questions/53367132/where-to-store-files-for-azure-function
                    var path = Path.Combine(executionContext.FunctionAppDirectory, "Zwift-5c2367dfe003.json");

                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",path);

                    Image tmpImage = await Image.FromStreamAsync(memoryStream);
                    var client = await ImageAnnotatorClient.CreateAsync();
                    var tmp = await client.DetectTextAsync(tmpImage);

                    var annotation = tmp.FirstOrDefault();

                    if (annotation?.Description != null) 
                        routes.Add(annotation.Description.Replace("\n", " ").Trim());
                }
            }

            if (routes.Count > 0)
            {
                var user = name.Split("_").First();
                await applicationQueue.AddAsync(JsonConvert.SerializeObject(new MultipleRoutesCompletedModel
                {
                    UserId = user,
                    Routes = routes
                }));
                await applicationQueue.FlushAsync();
            }
        }

        public static System.Drawing.Image CropBitmap(Bitmap bitmap, double cropX, double cropY, double cropWidth, double cropHeight)
        {
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle((int)(cropX * bitmap.Width), (int)(cropY * bitmap.Height), (int)(cropWidth * bitmap.Width), (int)(cropHeight * bitmap.Height));

            return CropImage(bitmap, rect);
        }

        public static Bitmap CropImage(Bitmap img, Rectangle cropArea)
        {
            Bitmap bmp = new Bitmap(cropArea.Width, cropArea.Height);
            using (Graphics gph = Graphics.FromImage(bmp))
            {
                gph.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), cropArea, GraphicsUnit.Pixel);
            }
            return bmp;
        }
    }
}
//https://stackoverflow.com/questions/50794707/how-to-use-azure-custom-vision-service-response-boundingbox-to-plot-shape
//https://www.codingdefined.com/2015/04/solved-bitmapclone-out-of-memory.html