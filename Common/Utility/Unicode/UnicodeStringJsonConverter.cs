using System.Text.Json;
using System.Text;
using System;
using System.Text.Json.Serialization;
using System.Linq;

namespace XDCommon.Utility
{
    public class UnicodeStringJsonConverter : JsonConverter<UnicodeString>
    {
        public override UnicodeString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var bytes = Encoding.UTF8.GetBytes(reader.GetString() ?? "\0").SelectMany(b => new byte[] { 0, b });
            return new UnicodeString(bytes);
        }

        public override void Write(Utf8JsonWriter writer, UnicodeString unicodeString, JsonSerializerOptions options)
        {
            writer.WriteStringValue(unicodeString.ToString());
        }    
    }
}