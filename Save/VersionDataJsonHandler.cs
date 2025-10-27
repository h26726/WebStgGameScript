using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class VersionDataJsonHandler
{
    #region Vector Converters
    public class Vector2Converter : JsonConverter<Vector2>
    {
        public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x"); writer.WriteValue(value.x);
            writer.WritePropertyName("y"); writer.WriteValue(value.y);
            writer.WriteEndObject();
        }

        public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            float x = obj["x"]?.Value<float>() ?? 0f;
            float y = obj["y"]?.Value<float>() ?? 0f;
            return new Vector2(x, y);
        }
    }

    public class Vector3Converter : JsonConverter<Vector3>
    {
        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x"); writer.WriteValue(value.x);
            writer.WritePropertyName("y"); writer.WriteValue(value.y);
            writer.WritePropertyName("z"); writer.WriteValue(value.z);
            writer.WriteEndObject();
        }

        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            float x = obj["x"]?.Value<float>() ?? 0f;
            float y = obj["y"]?.Value<float>() ?? 0f;
            float z = obj["z"]?.Value<float>() ?? 0f;
            return new Vector3(x, y, z);
        }
    }
    #endregion

    private static JsonSerializerSettings GetSerializerSettings()
    {
        return new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Converters = new List<JsonConverter>
            {
                new Vector2Converter(),
                new Vector3Converter()
            },
            TypeNameHandling = TypeNameHandling.Auto
        };
    }

    #region Wrapper class for compatibility
    [Serializable]
    private class VersionDataListWrapper
    {
        public List<VersionData> Items = new List<VersionData>();
    }
    #endregion

    public static void SaveVersionDataList(string filePath,List<VersionData> versionDatas)
    {
        try
        {
            var settings = GetSerializerSettings();
            // 可以直接存 List 或包裝成 Items
            var wrapper = new VersionDataListWrapper { Items = versionDatas };
            var json = JsonConvert.SerializeObject(wrapper, settings);
            File.WriteAllText(filePath, json);
            Debug.Log($"VersionData saved to: {filePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save VersionData: {ex}");
        }
    }

    public static List<VersionData> LoadVersionDataList(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"File not found: {filePath}");
                return new List<VersionData>();
            }

            var json = File.ReadAllText(filePath);
            var settings = GetSerializerSettings();

            // 嘗試先解析成陣列
            try
            {
                var list = JsonConvert.DeserializeObject<List<VersionData>>(json, settings);
                if (list != null) return list;
            }
            catch
            {
                // 如果失敗，可能是包裝成 Items 的物件
            }

            // 嘗試解析成 Wrapper
            var wrapper = JsonConvert.DeserializeObject<VersionDataListWrapper>(json, settings);
            return wrapper?.Items ?? new List<VersionData>();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load VersionData: {ex}");
            return new List<VersionData>();
        }
    }
}
