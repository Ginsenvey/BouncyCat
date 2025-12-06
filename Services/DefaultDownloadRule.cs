using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BouncyCat.Services;
class DownloadRule : IDownloadRuleProvider
{
    private readonly string _localPath;
    private readonly string _baseUrl;
    public DownloadRule(string localPath, string baseUrl) {
        _localPath = localPath;
        _baseUrl = baseUrl;
    }

    string IDownloadRuleProvider.GenerateLocalPathByName(string name, string id, string version)
    {
        return $"{_localPath}/{name}";
    }

    string IDownloadRuleProvider.GenerateSourceById(string code, string sourceId)
    {
        return $"{_baseUrl}code={code}&id={sourceId}&type=json";
    }
}
