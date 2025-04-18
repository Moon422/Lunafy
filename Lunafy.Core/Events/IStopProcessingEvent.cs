namespace Lunafy.Core.Events;

public partial interface IStopProcessingEvent
{
    /// <summary>
    /// Gets or sets a value whether processing of event publishing should be stopped
    /// </summary>
    bool StopProcessing { get; set; }
}