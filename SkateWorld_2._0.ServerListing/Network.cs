using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SkateWorld_2._0.ServerListing;

public static class Network
{
	private static readonly HttpClient client = new HttpClient();

	public static string SKey4 { get; private set; } = "Nzc4MjE3QQ==";


	public static async Task<HttpResponseMessage> Post(string url, Dictionary<string, string> values)
	{
		try
		{
			FormUrlEncodedContent content = new FormUrlEncodedContent(values);
			return await client.PostAsync(url, content);
		}
		catch
		{
			return null;
		}
	}
}
