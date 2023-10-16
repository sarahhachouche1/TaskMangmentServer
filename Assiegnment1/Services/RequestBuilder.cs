using Library;
using Newtonsoft.Json;
using System.Collections;
using Newtonsoft.Json.Linq;
using config = ServerSide.Configurations.AppConfiguration;
using System.Reflection;

namespace ServerSide.Services
{
    public class RequestBuilder
    {
        public void SetUpFileWatcher(string path)
        {
            var watcher = new FileWatcher(path);
            watcher.OnFileChanged += HandleRequest;
        }
        public async Task HandleRequest( string content)
        {
             Request request = new Request();
             Response response = new Response();
            try
            {
                request = ParseRequest(content);
                response = await GetResponseAsync(request);
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex);
            }
            finally
            {
                await WriteResponse($@"{request.SenderUsername}\{request.RequestId}", response); 
            }

        }
        internal async Task<Response> GetResponseAsync(Request request)
        {
            Response response = new Response();
            try
            {
                response.Content = await RouteRequestAsync(request);
                response.StatusCode = 200;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.StatusCode = 501;
                response.IsSuccess = false;
            }
            return response;
        }

        internal  Request ParseRequest(string request)
        {
            var deserializedRequest = JsonConvert.DeserializeObject<Request>(request);

            if (deserializedRequest == null)
            {
                throw new InvalidOperationException("The request message could not be parsed.");
            }
            return deserializedRequest;
        }

        internal  async Task<object?> RouteRequestAsync(Request request)
        {
            string[] url = ParseUrl(request);

            object instance = CreateInstance(url[0]);

            MethodInfo method = GetMethodInfo(instance, url[1]);

             var parameters = ExtractParam(request.Content, method);

            object[] param = parameters.ToArray();

            var content = method.Invoke(instance, param);

            if (content is Task task)
            {
                await task;

                if (task.GetType().IsGenericType)
                {
                    return ((dynamic)task).Result;
                }
            }
            return content;
        }

        internal string[] ParseUrl(Request request)
        {
            var parsedUri = request.Uri?.Split('/');

            if (parsedUri == null)
            {
                throw new ArgumentException("Invalid URI");
            }
            return parsedUri;

        }

        internal object CreateInstance(string className)
        {
            try
            {
                string namespaceName = "ServerSide.Services";
                string fullClassName = $"{namespaceName}.{className}";

                Assembly assembly = Assembly.GetExecutingAssembly();
                Type type = assembly.GetType(fullClassName, true, true);

                if (type == null)
                {
                    throw new InvalidOperationException($"Type '{fullClassName}' could not be found.");
                }

                var constructor = type.GetConstructors().FirstOrDefault();

                if (constructor == null)
                {
                    throw new InvalidOperationException($"No public constructors found for type '{fullClassName}'.");
                }

                var parameters = constructor.GetParameters()
                    .Select(p => config.ConfigureServices().GetService(p.ParameterType))
                    .ToArray();

                var instance = Activator.CreateInstance(type, parameters);

                return instance;
            }
            catch (Exception ex)
            { 
                throw;
            }
        }

        internal MethodInfo GetMethodInfo(object instance, string methodName)
        {
            Type type = instance.GetType();

            MethodInfo method = type.GetMethod(methodName);

            if (method == null)
            {
                throw new ArgumentException("No such method: " + methodName);
            }

            return method;
        }

        public ArrayList ExtractParam(Dictionary<string, object> parameters, MethodInfo method)
        {
            ArrayList array = new ArrayList();
            try
            {
                ParameterInfo[] param = method.GetParameters();

                foreach (var paramInfo in param)
                {
                    Type type = paramInfo.ParameterType;
                    if (parameters[paramInfo.Name] is JObject jObjectParameter)
                    {
                        object deserializedParameter = JsonConvert.DeserializeObject(jObjectParameter.ToString(), type);
                        array.Add(deserializedParameter);
                    }
                    else
                    {
                        array.Add(Convert.ChangeType(parameters[paramInfo.Name], type));
                    }
                  
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return array;

        }

        private async Task WriteResponse(string filePath, Response response)
        {
            // var _responseFolderPath = AppConfiguration.GetSetting("");
            // Console.WriteLine(_responseFolderPath);
            Console.WriteLine(filePath);
            string directoryPath = @"C:\Root\Responses";

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string fullPath = Path.Combine(directoryPath, filePath);
            try
            {
                
                if (!Directory.Exists(fullPath))
                {
                    Console.WriteLine(fullPath);
                    Directory.CreateDirectory(fullPath);
                }
                string serializedResponse = JsonConvert.SerializeObject(response);

                await File.WriteAllTextAsync(fullPath, serializedResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while attempting to write to {fullPath}: {ex.Message}");
            }
        }
    }
}

