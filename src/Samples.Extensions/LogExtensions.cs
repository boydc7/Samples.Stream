using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Samples.Extensions;

public static class LogExtensions
{
    public static void Exception(this ILogger log, Exception ex, string baseMsg = "",
                                 [CallerMemberName] string memberName = null)
    {
        ex ??= new ApplicationException("Unknown Exception Object Reference");

        var msg = ToLogMessage(ex, baseMsg, memberName);

        log.LogError(ex, msg);
    }

    public static string ToLogMessage(this Exception ex,
                                      string customMessage = null, [CallerMemberName] string memberName = null)
    {
        if (ex == null)
        {
            return null;
        }

        var msg = string.Concat("!!! EXCEPTION !!! ",
                                customMessage.IsNullOrEmpty()
                                    ? string.Concat("Caller [", memberName.Coalesce("N/A"), "]")
                                    : customMessage,
                                " :: Type [", ex.GetType(),
                                "] :: Message [", ex.Message, "]");

        return msg;
    }
}
