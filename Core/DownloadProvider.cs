using BouncyCat.Objects;
using Downloader;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BouncyCat.Services;
/// <summary>
/// 提供一个服务接口，从资源Id生成资源的实际网络地址。下载服务需要实现此接口，并通过依赖注入调用。
/// 按照一定规则生成本地文件名。
/// 最大并发数。
/// </summary>
public interface IDownloadRuleProvider
{
    string GenerateSourceById(string code,string sourceId);
    string GenerateLocalPathByName(string name, string id, string version);
}

