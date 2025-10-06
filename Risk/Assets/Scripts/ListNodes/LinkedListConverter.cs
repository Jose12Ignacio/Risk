using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class LinkedListConverter<T> : JsonConverter<LinkedList<T>>
{
    public override void WriteJson(JsonWriter writer, LinkedList<T> value, JsonSerializer serializer)
    {
        // Normalmente no necesitas serializar, solo deserializar.
        serializer.Serialize(writer, value);
    }

    public override LinkedList<T> ReadJson(JsonReader reader, Type objectType, LinkedList<T> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        LinkedList<T> list = new LinkedList<T>();
        JToken token = JToken.Load(reader);

        if (token.Type != JTokenType.Object)
            return list; // Si no es un objeto, devolvemos lista vacía

        var current = token["head"];
        while (current != null && current.Type == JTokenType.Object)
        {
            T data = current["data"].ToObject<T>(serializer);
            list.Add(data);
            current = current["next"];
        }

        return list;
    }
}