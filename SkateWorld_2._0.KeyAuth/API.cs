using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace SkateWorld_2._0.KeyAuth;

public class API
{
	//Yep their keyauth information
	//Goto SkateWorld_2.0.UI and goto Main look for Client_PlaySolo (More info about things u can do with this) 
	public static api KeyAuthApp = new api("SkateWorld 2.0", "hKFG6DJkG6", "e0d9747f1540c5a3a82f82a2e9fbf367fc38fb1ff1642c36d7f8b7201c4f6e2b", "1.0");

	public static string SKey3 { get; private set; } = "NzUz";

}
public class api
{
	[DataContract]
	private class response_structure
	{
		[DataMember]
		public bool success { get; set; }

		[DataMember]
		public string sessionid { get; set; }

		[DataMember]
		public string contents { get; set; }

		[DataMember]
		public string response { get; set; }

		[DataMember]
		public string message { get; set; }

		[DataMember]
		public string download { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public user_data_structure info { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public app_data_structure appinfo { get; set; }

		[DataMember]
		public List<msg> messages { get; set; }
	}

	public class msg
	{
		public string message { get; set; }

		public string author { get; set; }

		public string timestamp { get; set; }
	}

	[DataContract]
	private class user_data_structure
	{
		[DataMember]
		public string username { get; set; }

		[DataMember]
		public string ip { get; set; }

		[DataMember]
		public string hwid { get; set; }

		[DataMember]
		public string createdate { get; set; }

		[DataMember]
		public string lastlogin { get; set; }

		[DataMember]
		public List<Data> subscriptions { get; set; }
	}

	[DataContract]
	private class app_data_structure
	{
		[DataMember]
		public string numUsers { get; set; }

		[DataMember]
		public string numOnlineUsers { get; set; }

		[DataMember]
		public string numKeys { get; set; }

		[DataMember]
		public string version { get; set; }

		[DataMember]
		public string customerPanelLink { get; set; }

		[DataMember]
		public string downloadLink { get; set; }
	}

	public class app_data_class
	{
		public string numUsers { get; set; }

		public string numOnlineUsers { get; set; }

		public string numKeys { get; set; }

		public string version { get; set; }

		public string customerPanelLink { get; set; }

		public string downloadLink { get; set; }
	}

	public class user_data_class
	{
		public string username { get; set; }

		public string ip { get; set; }

		public string hwid { get; set; }

		public string createdate { get; set; }

		public string lastlogin { get; set; }

		public List<Data> subscriptions { get; set; }
	}

	public class Data
	{
		public string subscription { get; set; }

		public string expiry { get; set; }

		public string timeleft { get; set; }
	}

	public class response_class
	{
		public bool success { get; set; }

		public string message { get; set; }
	}

	public string name;

	public string ownerid;

	public string secret;

	public string version;

	private string sessionid;

	private string enckey;

	private bool initzalized;

	public app_data_class app_data = new app_data_class();

	public user_data_class user_data = new user_data_class();

	public response_class response = new response_class();

	private json_wrapper response_decoder = new json_wrapper(new response_structure());

	public static bool IsDebugRelease => false;

	public api(string name, string ownerid, string secret, string version)
	{
		if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(ownerid) || string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(version))
		{
			error("Application not setup correctly. Please watch video link found in Program.cs");
			(from x in Process.GetProcesses()
				where x.ProcessName.StartsWith("skate.", StringComparison.OrdinalIgnoreCase)
				select x).ToList().ForEach(delegate(Process x)
			{
				x.Kill();
			});
			Environment.Exit(0);
		}
		this.name = name;
		this.ownerid = ownerid;
		this.secret = secret;
		this.version = version;
	}

	public void init()
	{
		enckey = encryption.sha256(encryption.iv_key());
		string text = encryption.sha256(encryption.iv_key());
		string text2 = req(new NameValueCollection
		{
			["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("init")),
			["ver"] = encryption.encrypt(version, secret, text),
			["hash"] = checksum(Process.GetCurrentProcess().MainModule!.FileName),
			["enckey"] = encryption.encrypt(enckey, secret, text),
			["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(name)),
			["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(ownerid)),
			["init_iv"] = text
		});
		if (text2 == "KeyAuth_Invalid")
		{
			error("Application not found");
			(from x in Process.GetProcesses()
				where x.ProcessName.StartsWith("skate.", StringComparison.OrdinalIgnoreCase)
				select x).ToList().ForEach(delegate(Process x)
			{
				x.Kill();
			});
			Environment.Exit(0);
		}
		text2 = encryption.decrypt(text2, secret, text);
		response_structure response_structure = response_decoder.string_to_generic<response_structure>(text2);
		load_response_struct(response_structure);
		if (response_structure.success)
		{
			load_app_data(response_structure.appinfo);
			sessionid = response_structure.sessionid;
			initzalized = true;
		}
		else if (response_structure.message == "invalidver")
		{
			app_data.downloadLink = response_structure.download;
		}
	}

	public void Checkinit()
	{
		if (!initzalized)
		{
			if (IsDebugRelease)
			{
				error("Not initialized Check if KeyAuthApp.init() does exist");
			}
			else
			{
				error("Please initialize first");
			}
		}
	}

	public void register(string username, string pass, string key)
	{
		Checkinit();
		string value = WindowsIdentity.GetCurrent().User!.Value;
		string text = encryption.sha256(encryption.iv_key());
		string message = req(new NameValueCollection
		{
			["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("register")),
			["username"] = encryption.encrypt(username, enckey, text),
			["pass"] = encryption.encrypt(pass, enckey, text),
			["key"] = encryption.encrypt(key, enckey, text),
			["hwid"] = encryption.encrypt(value, enckey, text),
			["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(sessionid)),
			["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(name)),
			["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(ownerid)),
			["init_iv"] = text
		});
		message = encryption.decrypt(message, enckey, text);
		response_structure response_structure = response_decoder.string_to_generic<response_structure>(message);
		load_response_struct(response_structure);
		if (response_structure.success)
		{
			load_user_data(response_structure.info);
		}
	}

	public void login(string username, string pass)
	{
		Checkinit();
		string value = WindowsIdentity.GetCurrent().User!.Value;
		string text = encryption.sha256(encryption.iv_key());
		string message = req(new NameValueCollection
		{
			["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("login")),
			["username"] = encryption.encrypt(username, enckey, text),
			["pass"] = encryption.encrypt(pass, enckey, text),
			["hwid"] = encryption.encrypt(value, enckey, text),
			["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(sessionid)),
			["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(name)),
			["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(ownerid)),
			["init_iv"] = text
		});
		message = encryption.decrypt(message, enckey, text);
		response_structure response_structure = response_decoder.string_to_generic<response_structure>(message);
		load_response_struct(response_structure);
		if (response_structure.success)
		{
			load_user_data(response_structure.info);
		}
	}

	public void upgrade(string username, string key)
	{
		Checkinit();
		_ = WindowsIdentity.GetCurrent().User!.Value;
		string text = encryption.sha256(encryption.iv_key());
		string message = req(new NameValueCollection
		{
			["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("upgrade")),
			["username"] = encryption.encrypt(username, enckey, text),
			["key"] = encryption.encrypt(key, enckey, text),
			["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(sessionid)),
			["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(name)),
			["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(ownerid)),
			["init_iv"] = text
		});
		message = encryption.decrypt(message, enckey, text);
		response_structure response_structure = response_decoder.string_to_generic<response_structure>(message);
		response_structure.success = false;
		load_response_struct(response_structure);
	}

	public void license(string key)
	{
		Checkinit();
		string value = WindowsIdentity.GetCurrent().User!.Value;
		string text = encryption.sha256(encryption.iv_key());
		string message = req(new NameValueCollection
		{
			["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("license")),
			["key"] = encryption.encrypt(key, enckey, text),
			["hwid"] = encryption.encrypt(value, enckey, text),
			["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(sessionid)),
			["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(name)),
			["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(ownerid)),
			["init_iv"] = text
		});
		message = encryption.decrypt(message, enckey, text);
		response_structure response_structure = response_decoder.string_to_generic<response_structure>(message);
		load_response_struct(response_structure);
		if (response_structure.success)
		{
			load_user_data(response_structure.info);
		}
	}

	public void check()
	{
		Checkinit();
		string text = encryption.sha256(encryption.iv_key());
		string message = req(new NameValueCollection
		{
			["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("check")),
			["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(sessionid)),
			["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(name)),
			["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(ownerid)),
			["init_iv"] = text
		});
		message = encryption.decrypt(message, enckey, text);
		response_structure data = response_decoder.string_to_generic<response_structure>(message);
		load_response_struct(data);
	}

	public void setvar(string var, string data)
	{
		Checkinit();
		string text = encryption.sha256(encryption.iv_key());
		string message = req(new NameValueCollection
		{
			["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("setvar")),
			["var"] = encryption.encrypt(var, enckey, text),
			["data"] = encryption.encrypt(data, enckey, text),
			["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(sessionid)),
			["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(name)),
			["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(ownerid)),
			["init_iv"] = text
		});
		message = encryption.decrypt(message, enckey, text);
		response_structure data2 = response_decoder.string_to_generic<response_structure>(message);
		load_response_struct(data2);
	}

	public string getvar(string var)
	{
		Checkinit();
		string text = encryption.sha256(encryption.iv_key());
		string message = req(new NameValueCollection
		{
			["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("getvar")),
			["var"] = encryption.encrypt(var, enckey, text),
			["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(sessionid)),
			["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(name)),
			["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(ownerid)),
			["init_iv"] = text
		});
		message = encryption.decrypt(message, enckey, text);
		response_structure response_structure = response_decoder.string_to_generic<response_structure>(message);
		load_response_struct(response_structure);
		if (response_structure.success)
		{
			return response_structure.response;
		}
		return null;
	}

	public void ban()
	{
		Checkinit();
		string text = encryption.sha256(encryption.iv_key());
		string message = req(new NameValueCollection
		{
			["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("ban")),
			["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(sessionid)),
			["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(name)),
			["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(ownerid)),
			["init_iv"] = text
		});
		message = encryption.decrypt(message, enckey, text);
		response_structure data = response_decoder.string_to_generic<response_structure>(message);
		load_response_struct(data);
	}

	public string var(string varid)
	{
		Checkinit();
		_ = WindowsIdentity.GetCurrent().User!.Value;
		string text = encryption.sha256(encryption.iv_key());
		string message = req(new NameValueCollection
		{
			["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("var")),
			["varid"] = encryption.encrypt(varid, enckey, text),
			["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(sessionid)),
			["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(name)),
			["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(ownerid)),
			["init_iv"] = text
		});
		message = encryption.decrypt(message, enckey, text);
		response_structure response_structure = response_decoder.string_to_generic<response_structure>(message);
		load_response_struct(response_structure);
		if (response_structure.success)
		{
			return response_structure.message;
		}
		return null;
	}

	public List<msg> chatget(string channelname)
	{
		Checkinit();
		string text = encryption.sha256(encryption.iv_key());
		string message = req(new NameValueCollection
		{
			["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("chatget")),
			["channel"] = encryption.encrypt(channelname, enckey, text),
			["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(sessionid)),
			["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(name)),
			["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(ownerid)),
			["init_iv"] = text
		});
		message = encryption.decrypt(message, enckey, text);
		response_structure response_structure = response_decoder.string_to_generic<response_structure>(message);
		load_response_struct(response_structure);
		if (response_structure.success)
		{
			return response_structure.messages;
		}
		return null;
	}

	public bool chatsend(string msg, string channelname)
	{
		Checkinit();
		string text = encryption.sha256(encryption.iv_key());
		string message = req(new NameValueCollection
		{
			["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("chatsend")),
			["message"] = encryption.encrypt(msg, enckey, text),
			["channel"] = encryption.encrypt(channelname, enckey, text),
			["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(sessionid)),
			["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(name)),
			["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(ownerid)),
			["init_iv"] = text
		});
		message = encryption.decrypt(message, enckey, text);
		response_structure response_structure = response_decoder.string_to_generic<response_structure>(message);
		load_response_struct(response_structure);
		if (response_structure.success)
		{
			return true;
		}
		return false;
	}

	public bool checkblack()
	{
		Checkinit();
		string value = WindowsIdentity.GetCurrent().User!.Value;
		string text = encryption.sha256(encryption.iv_key());
		string message = req(new NameValueCollection
		{
			["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("checkblacklist")),
			["hwid"] = encryption.encrypt(value, enckey, text),
			["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(sessionid)),
			["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(name)),
			["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(ownerid)),
			["init_iv"] = text
		});
		message = encryption.decrypt(message, enckey, text);
		response_structure response_structure = response_decoder.string_to_generic<response_structure>(message);
		load_response_struct(response_structure);
		if (response_structure.success)
		{
			return true;
		}
		return false;
	}

	public string webhook(string webid, string param, string body = "", string conttype = "")
	{
		Checkinit();
		string text = encryption.sha256(encryption.iv_key());
		string message = req(new NameValueCollection
		{
			["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("webhook")),
			["webid"] = encryption.encrypt(webid, enckey, text),
			["params"] = encryption.encrypt(param, enckey, text),
			["body"] = encryption.encrypt(body, enckey, text),
			["conttype"] = encryption.encrypt(conttype, enckey, text),
			["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(sessionid)),
			["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(name)),
			["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(ownerid)),
			["init_iv"] = text
		});
		message = encryption.decrypt(message, enckey, text);
		response_structure response_structure = response_decoder.string_to_generic<response_structure>(message);
		load_response_struct(response_structure);
		if (response_structure.success)
		{
			return response_structure.response;
		}
		return null;
	}

	public byte[] download(string fileid)
	{
		Checkinit();
		string text = encryption.sha256(encryption.iv_key());
		string message = req(new NameValueCollection
		{
			["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("file")),
			["fileid"] = encryption.encrypt(fileid, enckey, text),
			["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(sessionid)),
			["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(name)),
			["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(ownerid)),
			["init_iv"] = text
		});
		message = encryption.decrypt(message, enckey, text);
		response_structure response_structure = response_decoder.string_to_generic<response_structure>(message);
		load_response_struct(response_structure);
		if (response_structure.success)
		{
			return encryption.str_to_byte_arr(response_structure.contents);
		}
		return null;
	}

	public void log(string message)
	{
		Checkinit();
		string text = encryption.sha256(encryption.iv_key());
		req(new NameValueCollection
		{
			["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("log")),
			["pcuser"] = encryption.encrypt(Environment.UserName, enckey, text),
			["message"] = encryption.encrypt(message, enckey, text),
			["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(sessionid)),
			["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(name)),
			["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(ownerid)),
			["init_iv"] = text
		});
	}

	public static string checksum(string filename)
	{
		using MD5 mD = MD5.Create();
		using FileStream inputStream = File.OpenRead(filename);
		return BitConverter.ToString(mD.ComputeHash(inputStream)).Replace("-", "").ToLowerInvariant();
	}

	public static void error(string message)
	{
		Process.Start(new ProcessStartInfo("cmd.exe", "/c start cmd /C \"color b && title Error && echo " + message + " && timeout /t 5\"")
		{
			CreateNoWindow = true,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false
		});
		(from x in Process.GetProcesses()
			where x.ProcessName.StartsWith("skate.", StringComparison.OrdinalIgnoreCase)
			select x).ToList().ForEach(delegate(Process x)
		{
			x.Kill();
		});
		Environment.Exit(0);
	}

	private static string req(NameValueCollection post_data)
	{
		try
		{
			using WebClient webClient = new WebClient();
			byte[] bytes = webClient.UploadValues("https://keyauth.win/api/1.0/", post_data);
			return Encoding.Default.GetString(bytes);
		}
		catch (WebException ex)
		{
			if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.TooManyRequests)
			{
				Thread.Sleep(1000);
				return req(post_data);
			}
			error("Connection failure. Please try again, or contact us for help.");
			(from x in Process.GetProcesses()
				where x.ProcessName.StartsWith("skate.", StringComparison.OrdinalIgnoreCase)
				select x).ToList().ForEach(delegate(Process x)
			{
				x.Kill();
			});
			Environment.Exit(0);
			return "";
		}
	}

	private void load_app_data(app_data_structure data)
	{
		app_data.numUsers = data.numUsers;
		app_data.numOnlineUsers = data.numOnlineUsers;
		app_data.numKeys = data.numKeys;
		app_data.version = data.version;
		app_data.customerPanelLink = data.customerPanelLink;
	}

	private void load_user_data(user_data_structure data)
	{
		user_data.username = data.username;
		user_data.ip = data.ip;
		user_data.hwid = data.hwid;
		user_data.createdate = data.createdate;
		user_data.lastlogin = data.lastlogin;
		user_data.subscriptions = data.subscriptions;
	}

	public string expirydaysleft()
	{
		Checkinit();
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local).AddSeconds(long.Parse(user_data.subscriptions[0].expiry)).ToLocalTime();
		TimeSpan timeSpan = dateTime - DateTime.Now;
		return Convert.ToString(timeSpan.Days + " Days " + timeSpan.Hours + " Hours Left");
	}

	private void load_response_struct(response_structure data)
	{
		response.success = data.success;
		response.message = data.message;
	}
}
