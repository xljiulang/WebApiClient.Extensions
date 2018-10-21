# WebApiClient.Extensions
[WebApiClient](https://github.com/dotnetcore/WebApiClient)项目的第三方扩展：[DependencyInjection](https://github.com/aspnet/DependencyInjection)、[HttpClientFactory](https://github.com/aspnet/HttpClientFactory)、[SteeltoeOSS.Discovery](https://github.com/SteeltoeOSS/Discovery)、[MessagePack](https://github.com/neuecc/MessagePack-CSharp)、[Protobuf](https://github.com/mgravell/protobuf-net)


### 1 DependencyInjection扩展

#### 1.1 Nuget
PM> `install-package WebApiClient.Extensions.DependencyInjection`
<br/>支持 netstandard2.0 

#### 1.2 使用方法
> 声明远程http服务的的WebApiClient调用接口

```c#
[HttpHost("https:/localhost:5000")]
public interface IValuesApi : IHttpApi
{
    [HttpGet("api/values")]
    ITask<string[]> GetAsync();

    [HttpGet("api/values/{id}")]
    ITask<string> GetAsync(int id);
}
```

> Startup相关配置

```c#
// This method gets called by the runtime. Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services)
{
    services.AddHttpApi<IValuesApi>().ConfigureHttpApiConfig((c,p) =>
    {
        c.HttpHost = new Uri("http://localhost:9999/");
        c.FormatOptions.DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
        c.LoggerFactory = p.GetRequiredService<ILoggerFactory>();
    });
    ...
}
```

> Controller

```c#
public class HomeController : Controller
{
    public async Task<string> Index([FromServices]IValuesApi api, int id = 0)
    {
        var values = await api.GetValuesAsync();
        var value = await api.GetValuesAsync(id);
        return "ok";
    }
}
```

### 2 HttpClientFactory扩展

#### 2.1 Nuget
PM> `install-package WebApiClient.Extensions.HttpClientFactory`
<br/>支持 netstandard2.0 

#### 2.2 使用方法
> 声明远程http服务的的WebApiClient调用接口

```c#
[HttpHost("https:/localhost:5000")]
public interface IValuesApi : IHttpApi
{
    [HttpGet("api/values")]
    ITask<string[]> GetAsync();

    [HttpGet("api/values/{id}")]
    ITask<string> GetAsync(int id);
}
```

> Startup相关配置

```c#
// This method gets called by the runtime. Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services)
{   
    services.AddHttpApiTypedClient<IValuesApi>((c, p) =>
    {
        c.HttpHost = new Uri("http://localhost:9999/");
        c.FormatOptions.DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
        c.LoggerFactory = p.GetRequiredService<ILoggerFactory>();
    });
    ...
}
```

> Controller

```c#
public class HomeController : Controller
{
    public async Task<string> Index([FromServices]IValuesApi api, int id = 0)
    {
        var values = await api.GetAsync();
        var value = await api.GetAsync(id);
        return "ok";
    }
}
```
### 3 DiscoveryClient扩展

#### 3.1 Nuget
PM> `install-package WebApiClient.Extensions.DiscoveryClient`
<br/>支持 netstandard2.0 

#### 3.2 使用方法
> 声明微服务的WebApiClient调用接口

```c#
[HttpHost("http://VALUES")]
public interface IValuesApi : IHttpApi
{
    [HttpGet("api/values")]
    ITask<string[]> GetAsync();

    [HttpGet("api/values/{id}")]
    ITask<string> GetAsync(int id);
}
```

> Startup相关配置

```c#
// This method gets called by the runtime. Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services)
{
    services.AddDiscoveryClient(Configuration);
    services.AddDiscoveryTypedClient<IValuesApi>((c, p) =>
    {        
        c.FormatOptions.DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
        c.LoggerFactory = p.GetRequiredService<ILoggerFactory>();
    });
    ...
}


// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    ...
    app.UseDiscoveryClient();
}
```

> Controller

```c#
public class HomeController : Controller
{
    public async Task<string> Index([FromServices]IValuesApi api, int id = 0)
    {
        var values = await api.GetAsync();
        var value = await api.GetAsync(id);
        return "ok";
    }
}
```
 
### 4 MessagePack扩展

#### 4.1 Nuget
PM> `install-package WebApiClient.Extensions.MessagePack `
<br/>支持 netstandard1.6 / net4.5 

#### 4.2 使用方法
> 声明远程http服务的的WebApiClient调用接口

```c#
[MessagePackReturn]
[HttpHost("https:/localhost:5000")]
public interface IUsersApi : IHttpApi
{
    [HttpGet("api/users/{id}")]
    ITask<UserInfo> GetAsync(int id);
    
    [HttpPut("api/users")]
    ITask<bool> PutAsync([MessagePackContent] UserInfo value);
}
```

> `asp.net core`服务端MessagePack相关配置

```c#
// This method gets called by the runtime. Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services)
{
    ...
    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddMvcOptions(o =>
    {
        o.OutputFormatters.Add(new MessagePackOutputFormatter(ContractlessStandardResolver.Instance));
        o.InputFormatters.Add(new MessagePackInputFormatter(ContractlessStandardResolver.Instance));
    });
}



class MessagePackInputFormatter : InputFormatter
{
    private readonly IFormatterResolver resolver;

    private static readonly StringSegment mediaType = new StringSegment("application/x-msgpack");

    public MessagePackInputFormatter(IFormatterResolver resolver)
    {
        this.resolver = resolver ?? MessagePackSerializer.DefaultResolver;
        this.SupportedMediaTypes.Add(new Microsoft.Net.Http.Headers.MediaTypeHeaderValue(mediaType));
    }

    public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
    {
        var body = context.HttpContext.Request.Body;
        var result = MessagePackSerializer.NonGeneric.Deserialize(context.ModelType, body, resolver);
        return InputFormatterResult.SuccessAsync(result);
    }
}

class MessagePackOutputFormatter : OutputFormatter
{
    private readonly IFormatterResolver resolver;

    private static readonly StringSegment mediaType = new StringSegment("application/x-msgpack");

    public MessagePackOutputFormatter(IFormatterResolver resolver)
    {
        this.resolver = resolver ?? MessagePackSerializer.DefaultResolver;
        this.SupportedMediaTypes.Add(new Microsoft.Net.Http.Headers.MediaTypeHeaderValue(mediaType));
    }

    public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
    {
        if (context.ObjectType != typeof(object))
        {
            MessagePackSerializer.NonGeneric.Serialize(context.ObjectType, context.HttpContext.Response.Body, context.Object, resolver);
        }
        else if (context.Object == null)
        {
            context.HttpContext.Response.Body.WriteByte(MessagePackCode.Nil);
        }
        else
        {
            MessagePackSerializer.NonGeneric.Serialize(context.Object.GetType(), context.HttpContext.Response.Body, context.Object, resolver);
        }

        context.ContentType = mediaType;
        return Task.CompletedTask;
    }
}
```
 

### 5 Protobuf扩展

#### 4.1 Nuget
PM> `install-package WebApiClient.Extensions.Protobuf`
<br/>支持 netstandard1.3 / net4.5 

#### 4.2 使用方法
> 声明远程http服务的的WebApiClient调用接口

```c#
[ProtobufReturn]
[HttpHost("https:/localhost:5000")]
public interface IUsersApi : IHttpApi
{
    [HttpGet("api/users/{id}")]
    ITask<UserInfo> GetAsync(int id);
    
    [HttpPut("api/users")]
    ITask<bool> PutAsync([ProtobufContent] UserInfo value);
}
```

> `asp.net core`服务端MessagePack相关配置

```c#
// This method gets called by the runtime. Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services)
{
    ...
    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddMvcOptions(o =>
    {
        o.OutputFormatters.Add(new ProtobufOutputFormatter());
        o.InputFormatters.Add(new ProtobufInputFormatter());
    });
}


class ProtobufInputFormatter : InputFormatter
{
    private static readonly StringSegment mediaType = new StringSegment("application/x-protobuf");

    public ProtobufInputFormatter()
    {
        this.SupportedMediaTypes.Add(new Microsoft.Net.Http.Headers.MediaTypeHeaderValue(mediaType));
    }

    public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
    {
        var body = context.HttpContext.Request.Body;
        var model = Serializer.NonGeneric.Deserialize(context.ModelType, body);
        return InputFormatterResult.SuccessAsync(model);
    }
}

class ProtobufOutputFormatter : OutputFormatter
{
    private static readonly StringSegment mediaType = new StringSegment("application/x-protobuf");

    public ProtobufOutputFormatter()
    {
        this.SupportedMediaTypes.Add(new Microsoft.Net.Http.Headers.MediaTypeHeaderValue(mediaType));
    }

    public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
    {
        var body = context.HttpContext.Response.Body;
        Serializer.NonGeneric.Serialize(body, context.Object);
        context.ContentType = mediaType;
        return Task.CompletedTask;
    }
}
```
 
