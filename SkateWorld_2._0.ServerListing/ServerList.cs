using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SkateWorld_2._0.ServerListing;

public static class ServerList
{
	//Yes now this were things get interesting
	//You can just take this and make a simple exploit and spam their serverlist
	//until they change it of cource I might update it
	private static string CommKey = "1337";

	public static string ServerListAddress = "http://144.126.131.181:8869";

	public static string IPKey { get; private set; } = "U0tBVEUxNjY1NDZBV09STERBNzIzNEVYRQ==";


	public static ServerListing? serverListing { get; private set; } = null;


	public static int playercount { get; set; } = 0;


	public static int playercapacity { get; set; } = 0;


	public static async Task<ServerListing[]> GetList()
	{
		HttpResponseMessage httpResponseMessage = await Network.Post(ServerListAddress + "/list", new Dictionary<string, string> { { "serverKey", CommKey } });
		if (httpResponseMessage != null && httpResponseMessage.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<ServerListing[]>(((object)((JObject)JsonConvert.DeserializeObject(await httpResponseMessage.Content.ReadAsStringAsync())).get_Item("servers")).ToString());
		}
		return null;
	}
}
