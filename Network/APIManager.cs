using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Net.Http;
using System.Security.Cryptography;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Amazon.S3;
using Amazon;
using Amazon.S3.Model;
using System.IO;
using Amazon.CognitoIdentity;
using Amazon.Runtime;
using MindPlus;
using System.Threading.Tasks;

public class NetworkMessage
{
    public NetCommand command;
    public HttpResponseMessage response;
    public string body;
    public NetworkMessage(NetCommand command, HttpResponseMessage response, string body)
    {
        this.command = command;
        this.response = response;
        this.body = body;
    }
}
public class APIManager : MonoBehaviour
{

    public interface IEventHandler
    {

    }

    private List<IEventHandler> eventHandlers = new List<IEventHandler>();
    private Queue<NetworkMessage> queue = new Queue<NetworkMessage>();
    private object obj = new object();
    private IAmazonS3 _s3Client;
    private AWSCredentials _credentials;

    private AWSCredentials Credentials
    {
        get
        {
            if (_credentials == null)
                _credentials = new CognitoAWSCredentials(Address.S3Cognito, RegionEndpoint.APNortheast2);
            return _credentials;
        }
    }

    private IAmazonS3 Client
    {
        get
        {
            if (_s3Client == null)
            {
                _s3Client = new AmazonS3Client(Credentials, RegionEndpoint.APNortheast2);
            }
            //test comment
            return _s3Client;
        }
    }
    public void Initialize()
    {
        UnityInitializer.AttachToGameObject(this.gameObject);
        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
    }
    private void Update()
    {
        if (queue.Count <= 0)
        {
            return;
        }

        NetworkMessage message = queue.Dequeue();
        List<IEventHandler> result = new List<IEventHandler>();
        foreach (var eventHandler in eventHandlers)
        {
            if (message.command.IsCompareIEventHandler(eventHandler))
            {
                result.Add(eventHandler);
            }
        }

        if (message.response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            GetErrorCode(message);
            message.command.Error(result, message);
        }
        else
        {
            message.command.Execute(result, message);
        }
    }

    public void ResisterEvent(IEventHandler eventHandler)
    {
        this.eventHandlers.Add(eventHandler);
    }
    public void UnResisterEvent(IEventHandler eventHandler)
    {
        this.eventHandlers.Remove(eventHandler);
    }
    public void ClearEvent()
    {
        this.eventHandlers.Clear();
    }
    public static int GetErrorCode(NetworkMessage networkMessage)
    {

        JObject keyValues = JObject.Parse(networkMessage.body);
        Debug.Log("GetErrorCode : " + networkMessage.body);
        int code = -1;
        if (keyValues.ContainsKey("error"))
        {
            string value = keyValues["error"].ToString();
            switch (value)
            {
                case "BadRequest":
                    code = 1001;
                    break;
                case "KeyNotFound":
                    code = 1002;
                    break;
                case "UserAlreadyExists":
                    code = 1003;
                    break;
                case "InvalidPassword":
                    code = 1004;
                    break;
                case "PageNotFound":
                    code = 1005;
                    break;
                case "ForbiddenRoom":
                    code = 1006;
                    break;
                case "RoomIsFull":
                    code = 1007;
                    break;
                case "RoomIsClosed":
                    code = 1008;
                    break;
                case "KeyAlreadyExists":
                    code = 1009;
                    break;
                case "NotEnoughCoin":
                    code = 1010;
                    break;
                case "NotEnoughHeart":
                    code = 1011;
                    break;
            }
            Debug.LogError(networkMessage.command.ID + " ErrorCode[" + code + "] " + value);
        }
        return code;
    }
    private async Task SendNetWorkMessage(HttpClient client, HttpRequestMessage request, NetCommand command)
    {
        HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
        string responseBody = await response.Content.ReadAsStringAsync();
        lock (obj)
        {
            //Debug.Log("======================Response================" + command.ID);
            queue.Enqueue(new NetworkMessage(command, response, responseBody));
        }
    }
    private async Task SendRequestBody(HttpClient client, HttpRequestMessage request, NetCommand command, JObject requestBody)
    {
        using (var stringContent = new StringContent(requestBody.ToString(Formatting.None), Encoding.UTF8, "application/json"))
        {
            request.Content = stringContent;

            HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
            string responseBody = await response.Content.ReadAsStringAsync();
            lock (obj)
            {
                //Debug.Log("======================Response================" + command.ID);
                queue.Enqueue(new NetworkMessage(command, response, responseBody));
            }
        }
    }
    /// <summary>
    /// HTTP.Get 메소드는 requsetBody를 가질 수 없음
    /// </summary>
    /// <param name="command"></param>
    /// <param name="key"></param>
    public async void GetAsync(NetCommand command, string key)
    {
        StringBuilder httpPath = new StringBuilder();
        httpPath.Append(Address.AWSDev);
        httpPath.Append(key);

        using (var client = new HttpClient())
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, httpPath.ToString()))
            {
                await SendNetWorkMessage(client, request, command);
            }
        }
    }

    public async void PostAsync(NetCommand command, string key, JObject requestBody = null)
    {
        StringBuilder httpPath = new StringBuilder();
        httpPath.Append(Address.AWSDev);
        httpPath.Append(key);

        using (var client = new HttpClient())
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, httpPath.ToString()))
            {
                if (requestBody != null) await SendRequestBody(client, request, command, requestBody);
                else await SendNetWorkMessage(client, request, command);
            }
        }
    }

    public async void PutAsync(NetCommand command, string key, JObject requestBody = null)
    {
        StringBuilder httpPath = new StringBuilder();
        httpPath.Append(Address.AWSDev);
        httpPath.Append(key);

        using (var client = new HttpClient())
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, httpPath.ToString()))
            {
                if (requestBody != null) await SendRequestBody(client, request, command, requestBody);
                else await SendNetWorkMessage(client, request, command);
            }
        }
    }

    public async void DeleteAsync(NetCommand command, string key, JObject requestBody = null)
    {
        StringBuilder httpPath = new StringBuilder();
        httpPath.Append(Address.AWSDev);
        httpPath.Append(key);

        using (var client = new HttpClient())
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, httpPath.ToString()))
            {
                if (requestBody != null) await SendRequestBody(client, request, command, requestBody);
                else await SendNetWorkMessage(client, request, command);
            }
        }
    }

    public string CalcHMACSHA256(string data)
    {
        string secretKey = "MindPlus$1234";
        HMACSHA256 hashObject = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
        byte[] signature = hashObject.ComputeHash(Encoding.UTF8.GetBytes(data));
        return System.Convert.ToBase64String(signature);
    }

    public void DownLoadTextureBytes(string ID, Action<byte[]> onComplete = null)
    {
        Client.GetObjectAsync(Address.ThumbnailBucket, ID, (response) =>
        {
            if (response.Response.ResponseStream != null)
            {
                byte[] bytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        byte[] buf = new byte[1024];
                        count = response.Response.ResponseStream.Read(buf, 0, 1024);
                        ms.Write(buf, 0, count);
                    }
                    while (response.Response.ResponseStream.CanRead && count > 0);
                    bytes = ms.ToArray();
                }
                if (bytes != null)
                {
                    onComplete?.Invoke(bytes);
                }
                if (response.Exception != null)
                {
                    Debug.LogError(string.Format("\n receieved error {0}", response.Response.HttpStatusCode.ToString()));
                }
            }
        });
    }

    public void DownLoadTexture(string ID, Action<Sprite> onComplete = null)
    {
        Client.GetObjectAsync(Address.ThumbnailBucket, ID, (response) =>
        {
            if (response.Response.ResponseStream != null)
            {
                byte[] bytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        byte[] buf = new byte[1024];
                        count = response.Response.ResponseStream.Read(buf, 0, 1024);
                        ms.Write(buf, 0, count);
                    }
                    while (response.Response.ResponseStream.CanRead && count > 0);
                    bytes = ms.ToArray();
                }
                if (bytes != null)
                {
                    Sprite sprite = Util.ConvertBytes(bytes);
                    onComplete?.Invoke(sprite);
                }
                if (response.Exception != null)
                {
                    Debug.LogError(string.Format("\n receieved error {0}", response.Response.HttpStatusCode.ToString()));
                }

            }
        });
    }

    public void UploadTextrue(Stream stream, string uploadKey, Action<Sprite> onComplete = null)
    {
        PutObjectRequest request = new PutObjectRequest()
        {
            BucketName = Address.ThumbnailBucket,
            Key = uploadKey,
            InputStream = stream,

        };

        Client.PutObjectAsync(request, (response) =>
        {
            if (response.Exception == null)
            {
                Debug.Log(string.Format("object {0} posted to bucket {1}", response.Request.Key, response.Request.BucketName));
                stream.Close();
                onComplete?.Invoke(null);
            }
            else
            {
                Debug.LogError(string.Format("Exception while posting the result object receieved error \n{0}", response.Exception.Message));
            }
        });
    }


    public void UploadCSVFile(Stream stream, string uploadKey, Action onComplete = null)
    {
        PutObjectRequest request = new PutObjectRequest()
        {
            BucketName = Address.HealthBucket,
            Key = uploadKey,
            InputStream = stream,
        };

        Client.PutObjectAsync(request, (response) =>
        {
            if (response.Exception == null)
            {
                Debug.Log(string.Format("object {0} posted to bucket {1}", response.Request.Key, response.Request.BucketName));
            }
            else
            {
                Debug.LogError(string.Format("Exception while posting the result object receieved error \n{0}", response.Exception.Message));
            }
            onComplete?.Invoke();
        });
    }

    public void UploadTextrue(string path, string uploadKey, Action<Sprite> onComplete = null)
    {
        byte[] bytes = File.ReadAllBytes(path);

        Sprite sprite = Util.ConvertBytes(bytes);

        FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

        PutObjectRequest request = new PutObjectRequest()
        {
            BucketName = Address.ThumbnailBucket,
            Key = uploadKey,
            InputStream = stream,

        };

        Client.PutObjectAsync(request, (response) =>
        {
            if (response.Exception == null)
            {
                Debug.Log(string.Format("object {0} posted to bucket {1}", response.Request.Key, response.Request.BucketName));
                onComplete?.Invoke(sprite);
            }
            else
            {
                Debug.LogError(string.Format("Exception while posting the result object receieved error \n{0}", response.Exception.Message));
            }
        });
    }
    private void GetBucketList()
    {
        Client.ListBucketsAsync(new ListBucketsRequest(), (response) =>
        {
            if (response.Exception == null)
            {
                Debug.Log("Got Response \nPrinting now \n");
                response.Response.Buckets.ForEach((s3b) =>
                {
                    Debug.Log(string.Format("bucket = {0}, created date = {1} \n", s3b.BucketName, s3b.CreationDate));
                });
            }
            else
            {
                Debug.Log("Got Exception \n" + response.Exception.Message);
            }
        });
    }
    private void GetObjects()
    {
        ListObjectsRequest request = new ListObjectsRequest()
        {
            BucketName = Address.ThumbnailBucket
        };
        Client.ListObjectsAsync(request, (response) =>
        {

            if (response.Exception == null)
            {
                Debug.Log("Got Response \nPrinting now \n");
                response.Response.S3Objects.ForEach((o) =>
                {
                    Debug.Log(string.Format("{0}\n", o.Key));
                });
            }
            else
            {
                Debug.Log("Got Exception \n" + response.Exception.Message);
            }
        });
    }
    private void DeleteObject()
    {
        List<KeyVersion> objects = new List<KeyVersion>();
        objects.Add(new KeyVersion()
        {
            Key = "users/Moon12/Moon12.jpg"
        });

        var request = new DeleteObjectsRequest()
        {
            BucketName = Address.ThumbnailBucket,
            Objects = objects
        };

        Client.DeleteObjectsAsync(request, (responseObj) =>
        {
            if (responseObj.Exception == null)
            {
                Debug.Log("Got Response \n \n");

                Debug.Log(string.Format("deleted objects \n"));

                responseObj.Response.DeletedObjects.ForEach((dObj) =>
                {
                    Debug.Log(dObj.Key);
                });
            }
            else
            {
                Debug.Log("Got Exception \n" + responseObj.Exception.Message);
            }
        });
    }
}
