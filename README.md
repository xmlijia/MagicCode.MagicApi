### 说明

  MagicCode MagicApi is a dynamic webapi middleware for .net core,and it has added SwaggerGen by default.

  use MagicApi,you can focus on logic design without considering how to configure the web api verbs or routes just by inherit the imagiccode to the service class or set the [MagicApi] to the service class
﻿
  魔码MagicApi是.net core的动态WebApi包，默认已添加SwaggerGen。

  使用魔码MagicApi，你可以关注于逻辑的实现，而无需考虑如何配置Webapi的请求方法、路由模板，所需要做的仅仅是服务继承IMagicApi或者给服务添加MagicApi特性。

### 配置 Configure

  配置选项可以设定api路由的基础路由、类名末尾自动去除词汇、http请求方法匹配规则。

  MagicApiOptions类
```
 public class MagicApiOptions
    {
        public string BaseRoute { get; set; } = "api"; //路由基础路径
        //待去除类名后缀词汇
        public string[] RemoveClassSuffixWords { get; set; } = new string[]
        {
            "AppService","AppServices","ApplicationService","ApplicationServices"
        };
        //http请求方法与Action名开头匹配关系
        public Dictionary<string, string[]> VerbPreWordMapper { get; set; } = new Dictionary<string, string[]>
        {
             {"GET",new string[]{"get"} },
            {"POST",new string[]{ "post", "create", "add", "insert"} } ,
            {"PUT",new string[]{ "put", "update", "save" } },
            {"DELETE",new string[]{ "delete", "remove" } },
            {"PATCH",new string[]{ "patch" } }
        };
        //可在app.UseMagicApi中配置
        public IMagicApiRouteParserProvider RouteParserProvider { get; set; }
    }
```

  配置：
```json
"MagicCode": {
    "MagicApi": {
      "BaseRoute": "api1",
      "SuffixServiceName": [ "AppService", "AppServices", "ApplicationService", "ApplicationServices" ],
      "VerbPreWordMapper": {

        "GET": [ "get" ],
        "POST": [ "post", "create", "add", "insert" ],
        "PUT": [ "put", "update", "save" ],
        "DELETE": [ "delete", "remove" ],
        "PATCH": [ "patch" ]
      }
    }
  }
```
### 约定

控制器名默认自动去除以ApplicationService(s)，AppService(s)结尾的内容（可通过配置SuffixServiceName为具体需要的后缀），例如：WechatAppService 得出的是wechat

方法约定：

  1、谓词：方法名以 MagicApiOptions.VerbPreWordMapper 值部分匹配即使用其键值名称方法。
 默认匹配表：
 | 谓词 | 方法名开始词 |
|----|----|
|GET|get |
|POST|post,create,add,insert|
|PUT|put,update,save|
|DELETE|delete,remove|
|PATCH|patch|


  2、路由：方法名剔除上方开始词及Async等关键词后剩余部分

路由约定： 路由模板为“api/控制名/方法名/参数”，方法名如果等于谓词约定中的开始词则模板为：“api/控制名/参数”



### 使用

包安装
```
Install-Package MagicCode.MagicApi -Version 1.0.0
```

引用
```
services.AddMvc()
  .AddMagicApi()
  .AddMagicfulResult() //可选，使用RESTful风格的Api结果
  ; 

app.UseMagicApi();
//或 app.UseMagicApi(o=>{o.RouteParserProvider=new CustomsRouteParserProvider();}) //CustomsRouteParserProvider为继承了IMagicApiRouteParserProvider的类
```

服务定义：

```C# 
public class HelloService : IMagicApi
{
    public string Get(){
      return "hello";
    }
}
```
或者 

```C#
[MagicApi]
public class HelloService
{
    public string Get(){
      return "hello";
    }
} 
```