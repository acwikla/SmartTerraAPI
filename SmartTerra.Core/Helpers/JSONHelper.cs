using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartTerra.Core.Helpers
{
    public static class JSONHelper
    {
        public static string ToJSON(this object obj)
        {
            var jsonString = JsonSerializer.Serialize(obj);
            return jsonString;
        }
    }
}
