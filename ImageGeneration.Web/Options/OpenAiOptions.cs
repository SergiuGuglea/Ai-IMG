namespace ImageGeneration.Web.Options
{
    public class OpenAiOptions
    {
        public const string SettingName = "OpenAi";
        public required string ApiKey { get; set; }
    }
}
