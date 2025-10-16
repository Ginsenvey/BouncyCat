using BouncyCat.Core;
using BouncyCat.Objects;
using Windows.Storage;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Media.Protection.PlayReady;
namespace BouncyCat.Services;

public interface IUpdateService 
{
    Task<string> UpdateAsync();
    Task<UpdateInfo> CheckUpdateAsync();
    Task<string> ShouldUpdate();
    Task<bool> Override();
}

public class UpdateService : IUpdateService
{
    private readonly HttpClient _client;
    private readonly string _path;
    private string _hash="0";
    public UpdateService(HttpClient client,string path)

    {
        _client = client;
        _path = path;
    }

    /// <summary>
    /// 获取更新信息。
    /// </summary>
    /// <param name="url"></param>
    public async Task<UpdateInfo> CheckUpdateAsync()
    {
        try
        {
            string url = ApiScope.UpdateInfoUrl;
            HttpResponseMessage response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string fileContent = await response.Content.ReadAsStringAsync();
            string[] lines = fileContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            UpdateInfo Info = new UpdateInfo
            {
                Remark = lines[0],
                ClientVersion = Deserializer.ExtractDescription(lines[1], "khd"),
                DataBaseVersion = Deserializer.ExtractDescription(lines[2], "wj"),
                MD5 = Deserializer.ExtractDescription(lines[3], "wjmd5"),
                Status = true
            };
            return Info;
        }
        catch
        {
            UpdateInfo Info = new UpdateInfo
            {
                Remark = "0",
                ClientVersion = "0",
                DataBaseVersion = "0",
                MD5 = "0",
                Status = false
            };
            return Info;
        }
    }

    /// <summary>
    /// 检查是否应该更新数据
    /// </summary>
    /// <param name="path">本地数据库地址</param>
    ///<remarks>当本地配置中md5返回0时，认为是首次使用，返回false;非0时，与现存文件哈希相匹配。</remarks>
    public async Task<string>ShouldUpdate()
    {
        UpdateInfo info = await CheckUpdateAsync();
        //成功响应，获取MD5
        if (info.Status)
        {
            _hash = info.MD5;  
        }
        else
        {
            return "e:获取更新元数据失败";
        }
        var Set = ApplicationData.Current.LocalSettings;
        string oldHash = ValidationHelper.GetKey(Set, "DataBaseHash");
        if (oldHash == "0") return "1";
        try
        {
            if (File.Exists(_path))
            {
                string md5 = FileHashHelper.GetFileMD5(_path);
                System.Diagnostics.Debug.WriteLine($"本地哈希{md5}");
                System.Diagnostics.Debug.WriteLine(oldHash);
                //仅当在线数据，本地数据和保留的MD5一致时，不需要刷新
                if (oldHash == md5 && (md5 == _hash))
                {
                    return "0";
                }
            }
        }
        catch(Exception ex)
        {
            return $"e:{ex.Message}";
        }
        return "1";
    }

    /// <summary>
    /// 下载并覆写本地文件。
    /// </summary>
    /// <returns>是否成功覆写。</returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<bool> Override()
    {
        try
        {
            string url = ApiScope.DataBaseUrl;
            // 发送GET请求并获取响应流
            HttpResponseMessage response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            // 将响应流写入文件
            using (Stream contentStream = await response.Content.ReadAsStreamAsync())
            using (FileStream fileStream = File.Create(_path))
            {
                await contentStream.CopyToAsync(fileStream);
            }
            //检查覆写后的文件MD5是否与在线数据一致。一致则将该哈希记录。否则返回false.
            try
            {
                if (File.Exists(_path))
                {
                    string md5 = FileHashHelper.GetFileMD5(_path);
                    if (md5 == _hash)
                    {
                        ApplicationData.Current.LocalSettings.Values["DataBaseHash"] = md5;
                        return true;
                    }
                }
            }
            catch{ }
        }
        catch (HttpRequestException e)
        {
            System.Diagnostics.Debug.WriteLine($"请求错误: {e.Message}");
        }
        return false;
    }


    /// <summary>
    /// 主更新函数。
    /// </summary>
    /// <returns>状态码，1表示不需要更新或者已经更新成功。</returns>
    public async Task<string> UpdateAsync()
    {
        string res= await ShouldUpdate();
        if (res=="1")//需要更新
        {
            bool status=await Override();
            if (status)
            {
                return "1";
            }
            else//更新失败
            {
                return "e:更新数据库文件失败";
            }
        }
        else if(res=="0")//无需更新
        {
            return "1";
        }
        else//出错
        {
            return res;
        }
    }
}
