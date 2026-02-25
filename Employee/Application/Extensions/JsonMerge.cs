using System.Text.Json.Nodes;

public static class JsonMerge
{
    // Merge: patch overrides target; null => remove key.
    public static JsonNode Merge(JsonNode? target, JsonNode? patch)
    {
        if (patch is null) return target ?? new JsonObject();

        // If patch is not object -> replace entirely
        if (patch is not JsonObject patchObj)
            return patch; // scalars/arrays replace target

        var result = (target?.DeepClone() as JsonObject) ?? new JsonObject();

        foreach (var kv in patchObj)
        {
            var key = kv.Key;
            var patchVal = kv.Value;

            if (patchVal is null)
            {
                // null means remove the key
                result.Remove(key);
                continue;
            }

            var hasTarget = result.TryGetPropertyValue(key, out var targetVal);

            if (patchVal is JsonObject || patchVal is JsonArray)
            {
                // If both are objects -> deep merge; arrays -> replace by default
                if (patchVal is JsonObject && targetVal is JsonObject)
                    result[key] = Merge(targetVal, patchVal);
                else
                    result[key] = patchVal.DeepClone(); // replace (clone to avoid parent conflict)
            }
            else
            {
                // scalar -> replace (clone to avoid parent conflict)
                result[key] = patchVal.DeepClone();
            }
        }

        return result;
    }
}