using BouncyCat.Objects;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BouncyCat.Core;

/// <summary>
/// 存储所需Api的地址。
/// </summary>
public static class ApiScope
{
    /// <summary> 应用更新日期/数据库日期和数据库MD5</summary>
    ///<remarks>khd=客户端是否更新 日期对比,wj=数据库日期,检测到新的则下载数据文件覆盖,wjmd5 = 数据库md5值</remarks>
    public const string UpdateInfoUrl = "http://e.haory.cn/e/e336/wenjian/JinQiGengXin";

    /// <summary>
    /// 应用所需的数据库文件，缺省.db后缀。
    /// </summary>
    /// <remarks>需要检验MD5.</remarks>
    public const string DataUrl = "http://e.haory.cn/e/e336/wenjian/WenJian";

}

///<summary>
///发送请求，与API直接交互的功能类。
///必须具有非null的返回。
///</summary>
public static class RequestSender
{
    public static HttpClient client = new();
    /// <summary>
    /// 基本请求方法。
    /// </summary>
    /// <param name="url"></param>
    /// <returns>可能带错误代码的文本。</returns>
    public static async Task<string> Request(string url)
    {
        try
        {
            var res=await client.GetAsync(url);
            return await ValidationHelper.AutoResponse(res);
        }
        catch(Exception ex)
        {
            return $"e:{ex.Message}";
        }
    }
    /// <summary>
    /// 获取更新信息。
    /// </summary>
    /// <param name="url"></param>
    public static async Task<UpdateInfo> GetUpdateInfo(string url)
    {
        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
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



}

/// <summary>
/// 将字符串解析为结构化数据。
/// </summary>
public static class Deserializer
{
    /// <summary>
    /// 提取一定格式的描述文本。
    /// </summary>
    /// <param name="line"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static string ExtractDescription(string line, string key)
    {
        string[] parts = line.Split();
        if (parts.Length == 3)
        {
            if (parts[0] == key)
            {
                return parts[2];
            }
            else
            {
                throw new Exception("输入键与返回顺序不一致。");
            }
        }
        else
        {
            throw new Exception("描述文本的长度错误。");
        }
    }
}

/// <summary>
/// 提供Http请求、键值操作的合法性检验
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// 当状态代码正确且非空时，返回有效文本
    /// </summary>
    /// <param name="response"></param>
    /// <returns>可能带状态码的文本。</returns>
    public static async Task<string> AutoResponse(HttpResponseMessage res)
    {
        try
        {
            if (res.IsSuccessStatusCode)
            {
                string Text = await res.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(Text))
                {
                    return "e:空返回";
                }
                else
                {
                    return Text;
                }
            }
            else
            {
                return "e:请求失败";
            }
        }
        catch
        {
            return "e:解析返回出错";
        }
    }
}