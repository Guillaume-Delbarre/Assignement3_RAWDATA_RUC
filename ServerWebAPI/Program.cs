using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ServerWebAPI
{
    class Category
    {
        private static int nextId;

        [JsonPropertyName("cid")]
        public int cid { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        public Category(string n)
        {
            Name = n;
            cid = nextId++;
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize<Category>(this, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        public Category FromJson(string element)
        {
            return JsonSerializer.Deserialize<Category>(element, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }


    }


    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

        }
    }
}
