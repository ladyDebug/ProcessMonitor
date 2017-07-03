using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessMonitor
{
    public class HttpListenerService
    {
        private const string GetConst = "GET";
        private readonly string _httpPrefix;
        private readonly HttpListener _listener;

        private readonly ConcurrentDictionary<string, WebSocket> _wsDictionary =
            new ConcurrentDictionary<string, WebSocket>();

        private IHttpProcess _httpHttpProcess;

        public HttpListenerService(string wsprefixes, string httpPrefix)
        {
            if (!HttpListener.IsSupported)
            {
                return;
            }
            _listener = new HttpListener();
            _httpPrefix = httpPrefix;
            _listener.Prefixes.Add(wsprefixes);
            _listener.Prefixes.Add(_httpPrefix);
            if (_httpPrefix.EndsWith("/"))
            {
                _httpPrefix = _httpPrefix.Remove(_httpPrefix.Length - 1);
            }
        }

        public void StartListen()
        {
            _listener.Start();
            Console.WriteLine("Listening");
            while (true)
            {
                ThreadPool.QueueUserWorkItem(ProcessContext, _listener.GetContext());
            }
        }

        public void StopListen()
        {
            _listener.Stop();
        }

        public async void SendMessageToWebsocket(string message)
        {
            if (_wsDictionary.Count == 0)
            {
                return;
            }
            var buffer = Encoding.UTF8.GetBytes(message);
            var removedItem =
                _wsDictionary.Where(i => i.Value.State != WebSocketState.Open)
                    .ToList();
            foreach (var item in removedItem)
            {
                WebSocket rItem;
                _wsDictionary.TryRemove(item.Key, out rItem);
            }
            foreach (var ws in _wsDictionary.Values)
            {
                if (ws.State == WebSocketState.Open)
                {
                    try
                    {
                        await
                            ws.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text,
                                true,
                                CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("State: {0},  {1}", ws.State, ex.Message);
                    }
                }
            }
        }

        public void SetGetAction(IHttpProcess httpHttpProcess)
        {
            _httpHttpProcess = httpHttpProcess;
        }

        private void ProcessContext(Object obj)
        {
            var context = (HttpListenerContext) obj;
            var request = context.Request;
            if (request != null)
            {
                Console.WriteLine(request.Url.ToString());
                if (request.IsWebSocketRequest)
                {
                    RegisterWsContext(context);
                }
                else
                {
                    if (_httpHttpProcess == null)
                    {
                        Console.WriteLine("Was not set IHttpProcess");
                        return;
                    }
                    if (!CheckHttpUrl(request))
                    {
                        context.Response.StatusCode = 404;
                        context.Response.Close();
                        return;
                    }

                    var httpResult = _httpHttpProcess.GetProcessInfo();
                    HandleHttpRequest(context, httpResult);
                }
            }
        }

        private void HandleHttpRequest(HttpListenerContext httpListenerContext, string result)
        {
            var response = httpListenerContext.Response;
            response.AddHeader("Access-Control-Allow-Origin","*");
            var buffer = Encoding.UTF8.GetBytes(result);
            response.ContentLength64 = buffer.Length;
            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        private bool CheckHttpUrl(HttpListenerRequest request)
        {
            if (request.HttpMethod != GetConst)
            {
                Console.WriteLine("Wrong http request");
                return false;
            }
            var url = request.Url.ToString();
            
            if (request.Url.ToString().EndsWith("/"))
            {
                url =  url.Remove(url.Length - 1);
            }
            if (url == _httpPrefix) return true;
            Console.WriteLine("Wrong get request url");
            return false;
        }

        private async void RegisterWsContext(HttpListenerContext httpListenerContext)
        {
            var webSocketContext = await OpenWebSocket(httpListenerContext);
            var webSocket = webSocketContext.WebSocket;
            if (webSocket.State == WebSocketState.Open)
            {
                _wsDictionary.TryAdd(webSocketContext.SecWebSocketKey, webSocket);
            }
        }

        private async Task<WebSocketContext> OpenWebSocket(HttpListenerContext httpListenerContext)
        {
            WebSocketContext webSocketContext = null;
            try
            {
                webSocketContext = await httpListenerContext.AcceptWebSocketAsync(null);
            }
            catch (Exception e)
            {
                httpListenerContext.Response.StatusCode = 500;
                httpListenerContext.Response.Close();
                Console.WriteLine("Exception: {0}", e);
            }
            return webSocketContext;
        }
    }
}