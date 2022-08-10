using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using SkateWorld_2._0.Client;
using SkateWorld_2._0.KeyAuth;
using SkateWorld_2._0.Properties;

namespace SkateWorld_2._0.UI;

public class Login : Window, IComponentConnector
{
	private WebClient client = new WebClient();

	private List<string> result;

	public static readonly DependencyProperty VisibilyUserLoggedIn = DependencyProperty.Register("UserLoggedIn", typeof(bool), typeof(Login), new PropertyMetadata(false));

	internal Login loginwindow;

	internal Grid TopBar;

	internal Button Exit;

	internal Grid LoginForm;

	internal TextBox LoginUsername;

	internal PasswordBox LoginPassword;

	internal CheckBox SaveDetails;

	internal Button LoginBtn;

	internal Button CreateAccount;

	internal Grid RegisterForm;

	internal TextBox RegisterUsername;

	internal PasswordBox RegisterPassword;

	internal PasswordBox RegisterPasswordVerify;

	internal Button RegisterBtn;

	internal Button BackBtn;

	internal Label StatusText;

	private bool _contentLoaded;

	public bool UserLoggedIn
	{
		get
		{
			return (bool)GetValue(VisibilyUserLoggedIn);
		}
		set
		{
			SetValue(VisibilyUserLoggedIn, value);
		}
	}

	public Login()
	{
		InitializeComponent();
	}

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
		Game.CheckForPrograms();
		RegisterUsername.MaxLength = 15;
		LoginUsername.MaxLength = 15;
		result = client.DownloadString("https://www.cs.cmu.edu/~biglou/resources/bad-words.txt").Split(new char[1] { '\n' }).ToList();
		API.KeyAuthApp.init();
		if (!API.KeyAuthApp.response.success)
		{
			MessageBox.Show(API.KeyAuthApp.response.message);
			Environment.Exit(0);
		}
		ShowGrid(LoginForm);
		API.KeyAuthApp.check();
		if (SkateWorld_2._0.Properties.Settings.Default.SaveDetails)
		{
			LoginUsername.Text = SkateWorld_2._0.Properties.Settings.Default.Username;
			LoginPassword.Password = SkateWorld_2._0.Properties.Settings.Default.Password;
			SaveDetails.IsChecked = SkateWorld_2._0.Properties.Settings.Default.SaveDetails;
		}
		DataObject.AddPastingHandler(LoginUsername, PasteHandler);
		DataObject.AddPastingHandler(RegisterUsername, PasteHandler);
	}

	private void ShowGrid(Grid g)
	{
		LoginForm.Visibility = Visibility.Hidden;
		RegisterForm.Visibility = Visibility.Hidden;
		g.Visibility = Visibility.Visible;
	}

	private void SaveLoginDetails()
	{
		SkateWorld_2._0.Properties.Settings.Default.Username = LoginUsername.Text;
		SkateWorld_2._0.Properties.Settings.Default.Password = LoginPassword.Password;
		SkateWorld_2._0.Properties.Settings.Default.SaveDetails = true;
		SkateWorld_2._0.Properties.Settings.Default.Save();
	}

	private async void LoginToKeyAuth()
	{
		API.KeyAuthApp.login(LoginUsername.Text, LoginPassword.Password);
		if (API.KeyAuthApp.response.success)
		{
			if (SaveDetails.IsChecked.Value)
			{
				SaveLoginDetails();
			}
			StatusText.Content = "Status: Logged in.";
			API.KeyAuthApp.log("User: " + API.KeyAuthApp.user_data.username + " Logged In!");
			new Main().Show();
			Hide();
		}
		else
		{
			StatusText.Content = "Status: " + API.KeyAuthApp.response.message;
		}
	}

	private bool UserExists()
	{
		//using this u can force verify
		//I dont even
		if (client.DownloadString("https://r5reloaded.com/skatesdk/api/userexists.php?name=" + RegisterUsername.Text).Contains("User Successfully Verified"))
		{
			return true;
		}
		return false;
	}

	private bool IsNameBanned(string name)
	{
		if (result.Contains(name))
		{
			return true;
		}
		return false;
	}

	private async Task<string> GetKey()
	{
		HttpClient httpClient = new HttpClient();
		//Im pretty sure this php is made from a public keyauth one
		//anyways u can still write a exploit to spam accounts
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "https://r5reloaded.com/skatesdk/api/createkey.php");
		ProductInfoHeaderValue item = new ProductInfoHeaderValue("user-agent", "SkateWorldAPI");
		httpRequestMessage.Headers.UserAgent.Add(item);
		return await (await httpClient.SendAsync(httpRequestMessage)).Content.ReadAsStringAsync();
	}

	private bool DoPasswordsMatch()
	{
		if (RegisterPassword.Password == RegisterPasswordVerify.Password)
		{
			return true;
		}
		return false;
	}

	private async void RegisterUser()
	{
		//Yes I know there dumbasses
		//and they said Skaterverse would crash cause its winfroms smh
		if (!UserExists())
		{
			if (DoPasswordsMatch())
			{
				if (IsAlphabetic(RegisterUsername.Text))
				{
					if (!IsNameBanned(RegisterUsername.Text))
					{
						if (RegisterUsername.Text.Length >= 3)
						{
							if (!string.IsNullOrWhiteSpace(RegisterUsername.Text))
							{
								if (RegisterPassword.Password.Length >= 5)
								{
									api keyAuthApp = API.KeyAuthApp;
									string text = RegisterUsername.Text;
									string password = RegisterPassword.Password;
									keyAuthApp.register(text, password, await GetKey());
									if (API.KeyAuthApp.response.success)
									{
										StatusText.Content = "Status: Account Registered, Please Login.";
										await Task.Delay(1000);
										LoginUsername.Text = RegisterUsername.Text;
										ShowGrid(LoginForm);
										RegisterUsername.Text = "";
										RegisterPassword.Password = "";
										RegisterPasswordVerify.Password = "";
									}
									else
									{
										StatusText.Content = "Status: Failed to register account.";
									}
								}
								else
								{
									StatusText.Content = "Status: Pick a longer password.";
								}
							}
							else
							{
								StatusText.Content = "Status: Username cant be blank.";
							}
						}
						else
						{
							StatusText.Content = "Status: Username must be greater then three letters.";
						}
					}
					else
					{
						StatusText.Content = "Status: Pick a different username.";
					}
				}
				else
				{
					StatusText.Content = "Status: Special characters are not allowed.";
				}
			}
			else
			{
				StatusText.Content = "Status: Passwords don't match.";
			}
		}
		else
		{
			StatusText.Content = "Status: Username Taken.";
			MessageBox.Show("Failed: Username Taken!");
		}
	}

	private void CreateAccount_Click(object sender, RoutedEventArgs e)
	{
		ShowGrid(RegisterForm);
	}

	private void LoginBtn_Click(object sender, RoutedEventArgs e)
	{
		LoginToKeyAuth();
	}

	private void RegisterBtn_Click(object sender, RoutedEventArgs e)
	{
		RegisterUser();
	}

	private void BackBtn_Click(object sender, RoutedEventArgs e)
	{
		ShowGrid(LoginForm);
		RegisterUsername.Text = "";
		RegisterPassword.Password = "";
		RegisterPasswordVerify.Password = "";
	}

	private void Exit_Click(object sender, RoutedEventArgs e)
	{
		Environment.Exit(0);
	}

	private bool IsAlphabetic(string s)
	{
		return new Regex("^[a-zA-Z0-9]+$").IsMatch(s);
	}

	private void PasteHandler(object sender, DataObjectPastingEventArgs e)
	{
		bool flag = false;
		if (e.DataObject.GetDataPresent(typeof(string)))
		{
			string s = e.DataObject.GetData(typeof(string)) as string;
			if (IsAlphabetic(s))
			{
				flag = true;
			}
		}
		if (!flag)
		{
			e.CancelCommand();
		}
	}

	private void LoginUsername_PreviewKeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Space)
		{
			e.Handled = true;
		}
	}

	private void LoginUsername_PreviewTextInput(object sender, TextCompositionEventArgs e)
	{
		if (!IsAlphabetic(e.Text))
		{
			e.Handled = true;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "6.0.5.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/SkateWorld-2.0;component/ui/login.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "6.0.5.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			loginwindow = (Login)target;
			loginwindow.Loaded += Window_Loaded;
			break;
		case 2:
			TopBar = (Grid)target;
			break;
		case 3:
			Exit = (Button)target;
			Exit.Click += Exit_Click;
			break;
		case 4:
			LoginForm = (Grid)target;
			break;
		case 5:
			LoginUsername = (TextBox)target;
			LoginUsername.PreviewKeyDown += LoginUsername_PreviewKeyDown;
			LoginUsername.PreviewTextInput += LoginUsername_PreviewTextInput;
			break;
		case 6:
			LoginPassword = (PasswordBox)target;
			break;
		case 7:
			SaveDetails = (CheckBox)target;
			break;
		case 8:
			LoginBtn = (Button)target;
			LoginBtn.Click += LoginBtn_Click;
			break;
		case 9:
			CreateAccount = (Button)target;
			CreateAccount.Click += CreateAccount_Click;
			break;
		case 10:
			RegisterForm = (Grid)target;
			break;
		case 11:
			RegisterUsername = (TextBox)target;
			RegisterUsername.PreviewKeyDown += LoginUsername_PreviewKeyDown;
			RegisterUsername.PreviewTextInput += LoginUsername_PreviewTextInput;
			break;
		case 12:
			RegisterPassword = (PasswordBox)target;
			break;
		case 13:
			RegisterPasswordVerify = (PasswordBox)target;
			break;
		case 14:
			RegisterBtn = (Button)target;
			RegisterBtn.Click += RegisterBtn_Click;
			break;
		case 15:
			BackBtn = (Button)target;
			BackBtn.Click += BackBtn_Click;
			break;
		case 16:
			StatusText = (Label)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
