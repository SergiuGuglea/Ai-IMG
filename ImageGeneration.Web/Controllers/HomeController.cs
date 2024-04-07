using ImageGeneration.Web.Constants;
using ImageGeneration.Web.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Images;
using OpenAI_API.Models;

namespace ImageGeneration.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly OpenAiOptions openAiOptions;

        public HomeController(IOptions<OpenAiOptions> openAiOptions)
        {
            this.openAiOptions = openAiOptions.Value;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GeneratePaintingPrompt(string paintTheme, string shortDesc)
        {
            string PROMPT = $@"You are an really good painter, I want to provide you a short description about a painting and the painting theme, you will give me back a prompt to create a painting using details I provided. The painting short description is: ""{shortDesc}"", the painting theme is ""{paintTheme}"".  Return the result as single line json string having this form {{""Title"": ""[put your title here]"", ""Description"":[Put the painting description to be generated here]}}";


            ChatRequest chatRequest = new ChatRequest()
            {
                Model = Model.ChatGPTTurbo,
                Temperature = 0.1,
                MaxTokens = 500,
                ResponseFormat = ChatRequest.ResponseFormats.JsonObject,
                Messages = [new ChatMessage(ChatMessageRole.System, PROMPT)]
            };

            OpenAIAPI api = new(openAiOptions.ApiKey);

            var paintingPrompt = await api.Chat.CreateChatCompletionAsync(chatRequest);

            string formatted = paintingPrompt.ToString().Replace("{{", "{").Replace("}}", "}").Replace("\\n", Environment.NewLine).Replace("\\t", "");

            TempData["PaintPrompt"] = formatted;

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GenerateImages(string model, string e2ImgSize, string e3ImgSize, string quality, string prompt)
        {
            OpenAIAPI api = new(openAiOptions.ApiKey);

            ImageGenerationRequest? imgReq = null;

            if (model == AIModels.DALLE2)
            {
                imgReq = new ImageGenerationRequest(prompt, 2, ConvertSizeFromString(e2ImgSize)) { Model = Model.DALLE2, ResponseFormat = ImageResponseFormat.Url };
            }

            if (model == AIModels.DALLE3)
            {
                imgReq = new ImageGenerationRequest(prompt, 1, ConvertSizeFromString(e3ImgSize)) { Model = Model.DALLE3, ResponseFormat = ImageResponseFormat.Url, Quality = quality };
            }

            var img = await api.ImageGenerations.CreateImageAsync(imgReq);

            if (img.Data.Count == 1)
            {
                TempData["Img1"] = img.Data[0].Url;
            }

            if (img.Data.Count == 2)
            {
                TempData["Img1"] = img.Data[0].Url;
                TempData["Img2"] = img.Data[1].Url;
            }

            return RedirectToAction(nameof(Index));
        }

        private ImageSize ConvertSizeFromString(string size) =>
           size switch
           {
               "256x256" => ImageSize._256,
               "512x512" => ImageSize._512,
               "1024x1024" => ImageSize._1024,
               "1024x1792" => ImageSize._1024x1792,
               "1792x1024" => ImageSize._1792x1024,
               _ => throw new ArgumentException("Invalid enum value for size", nameof(size)),
           };
    }
}
