using System.ServiceModel;

namespace Autodor.Modules.Orders.Infrastructure.Extensions;

/// <summary>
/// Extension methods for WCF/SOAP client handling.
/// </summary>
public static class WCFExtensions
{
    /// <summary>
    /// Safely closes a WCF client with proper handling of faulted state.
    ///
    /// WCF clients require special disposal handling:
    /// - If the client is in Faulted state, calling Close() or Dispose() will throw an exception
    /// - In Faulted state, we must call Abort() instead
    /// - For all other states, we use Task.Factory.FromAsync to properly close the connection asynchronously
    /// - If close fails, we fall back to Abort() to ensure cleanup
    /// </summary>
    /// <typeparam name="T">The type of WCF client that implements ICommunicationObject.</typeparam>
    /// <param name="client">The WCF client to close.</param>
    /// <returns>A ValueTask representing the asynchronous close operation.</returns>
    public static async ValueTask CloseClientAsync<T>(this T client)
        where T : ICommunicationObject
    {
        try
        {
            if (client.State != CommunicationState.Faulted)
            {
                // Use Task.Factory.FromAsync for proper APM-to-TAP conversion
                await Task.Factory.FromAsync(
                    client.BeginClose(null, null),
                    client.EndClose);
            }
            else
            {
                client.Abort();
            }
        }
        catch
        {
            client.Abort();
        }
    }
}
