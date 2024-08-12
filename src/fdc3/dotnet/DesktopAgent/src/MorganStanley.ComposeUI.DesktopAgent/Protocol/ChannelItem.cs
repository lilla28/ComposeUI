
using System.Text.Json.Serialization;
using Finos.Fdc3;

namespace MorganStanley.ComposeUI.Fdc3.DesktopAgent.Protocol;

/// <summary>
/// Class used for parsing the UserChannel from the user channel set config file.
/// </summary>
public class ChannelItem
{
    /// <summary>
    /// Unique identifier of the channel.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Type of the channel.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ChannelType Type { get; set; }

    /// <summary>
    /// Metadata specific to the channel.
    /// </summary>
    public IDisplayMetadata DisplayMetadata { get; set; }
}