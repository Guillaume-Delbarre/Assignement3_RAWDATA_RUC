using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Utilities;

namespace ServerWebAPI
{
    class Category
    {
        public static int nextId = 1;

        [JsonPropertyName("cid")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        public Category(string name)
        {
            Name = name;
            Id = nextId++;
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize<Category>(this, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

    }

    class Request
    {
        public string Method { get; set; }
        public string Path { get; set; }
        public string Date { get; set; }
        public string Body { get; set; }

        public Response Manage()
        {
            if (Method == "create")
            {
                if (Path == "/api/categories")
                {
                    var category = JsonSerializer.Deserialize<Category>(Body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    CategoryTable.Add(category);
                    return new Response(2, category.ToJson());
                }
                else
                {
                    return new Response(4, "Missing Path");
                }
            } 
            else if (Method == "read")
            {
                if (Path == "/api/categories")
                {
                    return new Response(1, CategoryTable.Read());
                }
                else
                {
                    if (Path == null || Path.Length < 16)
                    {
                        return new Response(4, "Missing Path");
                    }

                    var subPath = Path.Substring(0, 16);
                    try
                    {
                        var number = Int32.Parse(Path.Substring(16));
                        if (subPath == "/api/categories/")
                        {
                            return new Response(1, CategoryTable.Read(number));
                        }
                        else
                        {
                            return new Response(4, "Bad Request");
                        }
                    }
                    catch (Exception e)
                    {
                        return new Response(4, "Bad Request");
                    }
                }
            }
            else if (Method == "update")
            {
                if (Path == null || Path.Length < 16)
                {
                    return new Response(4, "Missing Path");
                }
                
                var subPath = Path.Substring(0, 16);
                //var number = Int32.Parse(Path.Substring(16));

                if (subPath == "/api/categories/")
                {
                    CategoryTable.Update(JsonSerializer.Deserialize<Category>(Body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
                    return new Response(1, "Updated");
                }
                else
                {
                    return new Response(3, "Bad Request");
                }
            }
            else if (Method == "delete")
            {
                if (Path == null || Path.Length < 16)
                {
                    return new Response(4, "Missing Path");
                }

                var subPath = Path.Substring(0, 16);
                try
                {
                    var number = Int32.Parse(Path.Substring(16));
                    if (subPath == "/api/categories/")
                    {
                        CategoryTable.Delete(number);
                        return new Response(1, "Deleted");
                    }
                    else
                    {
                        return new Response(3, "Updated");
                    }
                }
                catch (Exception e)
                {
                    return new Response(4, "Bad Request");
                }
            }
            else if (Method == "echo")
            {
                if (Date == null)
                {
                    return new Response(4, "Missing Date");
                }
                else
                {
                    return new Response(4, Body);
                }
            }
            else
            {
                return new Response(4, "Missing Method");
            }
        }
    }

    static class CategoryTable
    {
        public static Hashtable CategoryHashtable = new Hashtable();

        public static void Add(Category c)
        {
            CategoryHashtable.Add(c.Id, c);
        }

        public static void Delete(int i)
        {
            CategoryHashtable.Remove(i);
        }

        public static void Update(Category c)
        {
            CategoryHashtable[c.Id] = c;
        }

        public static string Read()
        {
            var ret = "[";
            foreach (Category cat in CategoryHashtable)
            {
                ret += cat.ToJson() + ",";
            }
            ret = ret.Remove(ret.Length - 1);
            ret += "]";
            return ret;
        }

        public static string Read(int i)
        {
            if (CategoryHashtable.ContainsKey(i))
            {
                Category category = (Category)CategoryHashtable[i];
                return category.ToJson();
            }
            else
            {
                return "{}";
            }
        }
    }

    class Response
    {
        public string Status { get; set; }
        public string Body { get; set; }

        public Response(int s, string b)
        {
            Status = s.ToString();
            Body = b;
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize<Response>(this, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }

    class ServerWebAPI
    {
        static void Main(string[] args)
        {
            //Configure and start the server
            var server = new TcpListener(IPAddress.Loopback, 5000);
            server.Start();
            Console.WriteLine("Server Started");

            Hashtable categoryHashTable = new Hashtable();

            //Loop to get multiple requests
            while (true)
            {
            var client = new NetworkClient(server.AcceptTcpClient());
                Console.WriteLine("Client accepted");

                var message = client.Read();

                Console.WriteLine($"Client message '{message}'");

                var request = JsonSerializer.Deserialize<Request>(message, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                var response = request.Manage();

                Console.WriteLine(request.ToString());
                client.Write(response.ToJson());
            }

        }
    }
}
