using MorganStanley.ComposeUI.Logging.Entity;
using System.Text.Json.Serialization;

namespace MorganStanley.ComposeUI.Logging.Entities
{
    [JsonSerializable(typeof(LogData))]
    [JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default)]
    internal partial class LogDataJsonContext : JsonSerializerContext
    {

    }
}

//[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Serialization)] 
// DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull