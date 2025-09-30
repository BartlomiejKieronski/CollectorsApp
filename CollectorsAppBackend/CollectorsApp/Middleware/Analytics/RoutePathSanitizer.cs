using CollectorsApp.Models.APILogs;
using Microsoft.AspNetCore.Routing.Patterns;
using System.Text;

namespace CollectorsApp.Middleware.Analytics;

internal static class RoutePathSanitizer
{
    public static APILog RemoveRouteParamValues(APILog log, HttpContext context)
    {
        // Prefer exact pattern reconstruction from endpoint metadata when available.
        if (context.GetEndpoint() is RouteEndpoint routeEndpoint && routeEndpoint.RoutePattern is RoutePattern pattern)
        {
            var rebuilt = RebuildFromPattern(pattern);
            if (!string.IsNullOrEmpty(rebuilt))
            {
                log.RequestPath = rebuilt;
                return log;
            }
        }

        // Fallback heuristic (legacy) if we cannot access metadata pattern (should be rare).
        var path = context.Request.Path.HasValue ? context.Request.Path.Value! : string.Empty;
        if (string.IsNullOrEmpty(path))
        {
            log.RequestPath = path;
            return log;
        }

        var routeValues = context.Request.RouteValues;
        var valueToKey = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in routeValues)
        {
            if (kvp.Value is null) continue;
            var key = kvp.Key;
            if (string.Equals(key, "controller", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(key, "action", StringComparison.OrdinalIgnoreCase))
                continue;
            var valString = kvp.Value.ToString();
            if (!string.IsNullOrWhiteSpace(valString) && !valueToKey.ContainsKey(valString))
            {
                valueToKey[valString] = key;
            }
        }

        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        for (int i = 0; i < segments.Length; i++)
        {
            var seg = segments[i];
            if (valueToKey.TryGetValue(seg, out var paramName))
            {
                segments[i] = "{" + paramName + "}";
            }
        }
        log.RequestPath = "/" + string.Join('/', segments);
        return log;
    }

    private static string RebuildFromPattern(RoutePattern pattern)
    {
        // Build path from pattern segments replacing parameters with {parameterName}
        // Strip constraints / policies / defaults to avoid leaking values.
        if (pattern.PathSegments.Count == 0) return "/"; // root
        var sb = new StringBuilder();
        foreach (var segment in pattern.PathSegments)
        {
            sb.Append('/');
            if (segment.Parts.Count == 1)
            {
                var part = segment.Parts[0];
                switch (part)
                {
                    case RoutePatternLiteralPart lit:
                        sb.Append(lit.Content);
                        break;
                    case RoutePatternParameterPart param:
                        sb.Append('{').Append(param.Name).Append('}');
                        break;
                    default:
                        sb.Append(part.ToString());
                        break;
                }
            }
            else
            {
                // Segment composed of multiple parts (rare, like 'file.{ext}').
                foreach (var part in segment.Parts)
                {
                    switch (part)
                    {
                        case RoutePatternLiteralPart lit:
                            sb.Append(lit.Content);
                            break;
                        case RoutePatternParameterPart param:
                            sb.Append('{').Append(param.Name).Append('}');
                            break;
                        default:
                            sb.Append(part.ToString());
                            break;
                    }
                }
            }
        }
        return sb.Length == 0 ? "/" : sb.ToString();
    }
}