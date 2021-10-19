using System;
using System.Collections;
using System.Collections.Generic;
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
            if (Method == "echo")
            {
                if (Date == null)
                {
                    return new Response("4 Missing Resource", null);
                }
                else
                {
                    if (Body == null)
                    {
                        return new Response("4 Missing Body", null);
                    }
                    else
                    {
                        try
                        {
                            Int32.Parse(Date);
                            return new Response("4", Body);
                        }
                        catch (Exception)
                        {
                            return new Response("4 Illegal Date", Body);
                        }
                    }
                }
            }
            else
            {   
                if (Date == null)
                {
                    if (Method == null)
                    {
                        return new Response("4 Missing Date, Missing Method", null);
                    }
                    else
                    {
                        return new Response("4 Missing Date", null);
                    }
                }
                else if (Method == null)
                {
                    return new Response("4 Missing Method", null);
                }
                else
                {
                    try
                    {
                        Int32.Parse(Date);
                    }
                    catch (Exception)
                    {
                        return new Response("4 Illegal Date", null);
                    }

                    if (Method == "create")
                    {
                        if (Path == "/api/categories")
                        {
                            if (Body == null)
                            {
                                return new Response("4 Missing Body", null);
                            }
                            else
                            {
                                try
                                {
                                    var category = JsonSerializer.Deserialize<Category>(Body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                                    CategoryTable.Add(category);
                                    return new Response("2", category.ToJson());
                                }
                                catch (Exception)
                                {
                                    return new Response("4 Illegal Body", "The Body doesn't correspond to a Json Category");
                                }
                                
                            }
                        }
                        else
                        {
                            if (Body == null)
                            {
                                return new Response("4 Missing Resource, Missing Body", null);
                            }
                            else
                            {
                                if (Path == null)
                                    return new Response("4 Missing Resource, Missing Path", null);
                                return new Response("4 Bad Request", null);
                            }
                        }
                    }
                    else if (Method == "read")
                    {
                        if (Path == "/api/categories")
                        {
                            return new Response("1 Ok", CategoryTable.Read());
                        }
                        else
                        {
                            if (Path == null)
                            {
                                return new Response("4 Missing Resource, Missing Path", null);
                            }
                            else if (Path.Length < 16)
                            {
                                return new Response("4 Bad Request", null);
                            }

                            var subPath = Path.Substring(0, 16);
                            try
                            {
                                var number = Int32.Parse(Path.Substring(16));
                                if (subPath == "/api/categories/")
                                {
                                    if (CategoryTable.Exist(number))
                                        return new Response("1 Ok", CategoryTable.Read(number));
                                    else
                                        return new Response("5 Not Found", null);
                                }
                                else
                                {
                                    return new Response("4 Bad Request", null);
                                }
                            }
                            catch (Exception)
                            {
                                return new Response("4 Bad Request", null);
                            }
                        }
                    }
                    else if (Method == "update")
                    {
                        if (Body == null)
                        {
                            if (Path == null)
                            {
                                return new Response("4 Missing Body, Missing Path, Missing Resource", null);
                            }
                            else
                            {
                                return new Response("4 Missing Body, Missing Resource", null);
                            }
                        }
                        else
                        {
                            if (Path == null)
                            {
                                return new Response("4 Missing Path, Missing Resource", null);
                            }
                            else if (Path.Length < 16)
                            {
                                return new Response("4 Bad Request", null);
                            }
                            else
                            {

                                var subPathUp = Path.Substring(0, 16);
                                try
                                {
                                    var numberUp = Int32.Parse(Path.Substring(16));
                                    if (subPathUp == "/api/categories/")
                                    {
                                        try
                                        {
                                            var category = JsonSerializer.Deserialize<Category>(Body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                                            if (category.Id == numberUp)
                                            {
                                                if (CategoryTable.Exist(numberUp))
                                                {
                                                    CategoryTable.Update(category);
                                                    return new Response("3 Updated", category.ToJson());
                                                }
                                                else
                                                {
                                                    return new Response("4 Bad Request", null);
                                                }
                                            }
                                            else
                                            {
                                                return new Response("5 Not Found", null);
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            return new Response("4 Illegal Body", "The Body doesn't correspond to a Json Category");
                                        }
                                    }
                                    else
                                    {
                                        return new Response("4 Bad Request", null);
                                    }
                                }
                                catch (Exception)
                                {
                                    return new Response("4 Bad Request", null);
                                }
                            }
                        }
                    }
                    else if (Method == "delete")
                    {
                        if (Path == null)
                            return new Response("4 Missing Resource, Missing Path", null);
                        else if (Path.Length < 16)
                            return new Response("4 Bad Request", null);

                        var subPathDel = Path.Substring(0, 16);
                        try
                        {
                            var numberDel = Int32.Parse(Path.Substring(16));
                            if (subPathDel == "/api/categories/")
                            {
                                if (CategoryTable.Exist(numberDel))
                                {
                                    CategoryTable.Delete(numberDel);
                                    return new Response("1 Ok", null);
                                }
                                else
                                {
                                    return new Response("5 Not Found", null);
                                }
                                
                            }
                            else
                            {
                                return new Response("4 Bad Request", null);
                            }
                        }
                        catch (Exception)
                        {
                            return new Response("4 Bad Request", null);
                        }
                    }
                    else
                    {
                        return new Response("4 Illegal Method", null);
                    }
                }
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
            var categories = new List<Category>();
            foreach (DictionaryEntry cat in CategoryHashtable)
            {
                categories.Insert(0,(Category)cat.Value);
            }

            return JsonSerializer.Serialize<List<Category>>(categories, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
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

        public static bool Exist(int i)
        {
            return CategoryHashtable.ContainsKey(i);
        }
    }

    class Response
    {
        public string Status { get; set; }
        public string Body { get; set; }

        public Response(string s, string? b)
        {
            Status = s;
            if (b != null)
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
            //Create the first Categories
            var c1 = new Category("Beverages");
            var c2 = new Category("Condiments");
            var c3 = new Category("Confections");
            CategoryTable.Add(c1);
            CategoryTable.Add(c2);
            CategoryTable.Add(c3);

            //Configure and start the server
            var server = new TcpListener(IPAddress.Loopback, 5000);
            server.Start();
            Console.WriteLine("Server Started");

            //Loop to get multiple requests
            while (true)
            {
                var client = new NetworkClient(server.AcceptTcpClient());
                Console.WriteLine("\nNew Client accepted");

                var message = client.Read();

                Console.WriteLine($"Client message '{message}'");

                var request = JsonSerializer.Deserialize<Request>(message, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    
                var response = request.Manage();

                Console.WriteLine("Returning : " + response.ToJson());
                client.Write(response.ToJson());
            }

        }
    }
}
