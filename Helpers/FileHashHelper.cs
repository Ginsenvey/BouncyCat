using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 检验文件有效性的功能类。
/// </summary>
public static class FileHashHelper
{
    /// <summary>
    /// 计算文件的MD5哈希值
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>MD5哈希值的十六进制字符串</returns>
    public static string GetFileMD5(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"文件不存在: {filePath}");

        using (var md5 = MD5.Create())
        using (var stream = File.OpenRead(filePath))
        {
            byte[] hashBytes = md5.ComputeHash(stream);
            return Convert.ToHexStringLower(hashBytes);
        }
    }

    /// <summary>
    /// 异步计算文件的MD5哈希值
    /// </summary>
    /// <returns>MD5字符串。</returns>
    public static async Task<string> GetFileMD5Async(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"文件不存在: {filePath}");

        using (var md5 = MD5.Create())
        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
        {
            byte[] hashBytes = await md5.ComputeHashAsync(stream);
            return Convert.ToHexStringLower(hashBytes);
        }
    }
}