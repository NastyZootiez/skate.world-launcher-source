using System;
using System.Security.Cryptography;
using System.Text;
using SkateWorld_2._0.Client;
using SkateWorld_2._0.KeyAuth;

namespace SkateWorld_2._0.ServerListing;

public static class Encryption
{
	public static string SKey1 { get; private set; } = "NEQ2MzU=";


	public static string Decrypt(string encodedStr, string key)
	{
		AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider();
		string text = Game.ReturnSuper(Encoding.UTF8.GetString(Convert.FromBase64String(key)), Encoding.UTF8.GetString(Convert.FromBase64String(SKey1)), Encoding.UTF8.GetString(Convert.FromBase64String(Game.SKey2))).Replace("EXE", Encoding.UTF8.GetString(Convert.FromBase64String(API.SKey3)));
		try
		{
			aesCryptoServiceProvider.Key = Encoding.UTF8.GetBytes(text + Encoding.UTF8.GetString(Convert.FromBase64String(Network.SKey4)));
			string[] array = encodedStr.Split(':');
			byte[] array3 = (aesCryptoServiceProvider.IV = FromHexString(array[0]));
			byte[] array4 = FromHexString(array[1]);
			return Encoding.UTF8.GetString(aesCryptoServiceProvider.CreateDecryptor().TransformFinalBlock(array4, 0, array4.Length));
		}
		catch (Exception value)
		{
			Console.WriteLine(value);
			return null;
		}
	}

	public static byte[] FromHexString(string hexString)
	{
		byte[] array = new byte[hexString.Length / 2];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
		}
		return array;
	}
}
