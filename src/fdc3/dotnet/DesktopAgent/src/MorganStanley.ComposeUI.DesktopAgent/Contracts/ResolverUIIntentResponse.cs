namespace MorganStanley.ComposeUI.Fdc3.DesktopAgent.Contracts;

/// <summary>
/// Response type of the request for choosing between intents when multiple intent is available for the specified context when calling the raiseIntentForContext.
/// </summary>
public class ResolverUIIntentResponse
{
    /// <summary>
    /// Indicates that error happened during the execution, e.g.: User cancelled the execution etc.
    /// </summary>
    public string? Error {  get; set; }

    /// <summary>
    /// The chosen intent.
    /// </summary>
    public string? Intent { get; set; }
}
