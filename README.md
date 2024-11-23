### 说明

  MagicCode MagicApi is a dynamic webapi middleware for .net core,and it has added SwaggerGen by default.

  use MagicApi,you can focus on logic design without considering how to configure the web api verbs or routes just by inherit the imagiccode to the service class or set the [MagicApi] to the service class
﻿
魔码MagicApi是.net core的动态WebApi包，默认已添加SwaggerGen。
使用魔码MagicApi，你可以关注于逻辑的实现，而无需考虑如何配置Webapi的请求方法、路由模板，所需要做的仅仅是服务继承IMagicApi或者给服务添加MagicApi特性。

### 约定

控制器名自动去除以ApplicationService(s)，AppService(s)结尾的内容，例如：WechatAppService 得出的是wechat

方法约定：

  1、谓词：

| 谓词 | 方法名开始词 |
|----|----|
|GET|get |
|POST|post,create,add,insert|
|PUT|put,update,save|
|DELETE|delete,remove|
|PATCH|patch|

  2、路由：

  方法名剔除上方开始词及Async等关键词后剩余部分

路由约定：
   
   路由模板为“api/控制名/方法名/参数”，方法名如果等于谓词约定中的开始词则模板为：“api/控制名/参数”

### 使用

包安装
```
Install-Package MagicCode.MagicApi -Version 1.0.0
```

引用
```
services.AddMvc()
  .AddMagicApi();
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