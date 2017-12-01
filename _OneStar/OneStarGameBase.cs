using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class OneStarGameBase : MonoBehaviour {

	public class User {
		public string email, username; 
		public User(){}
		public User(string e, string n){ email = e; username = n; }
	}
	//Load on IDE.
	public UI ui;
	public UILogin uiLogin;

	public DatabaseReference reference;

	public string gameId;

	public void setDataOnFirebase( ){
		print ("["+ui.user.text+"] "+ui.id.text+": "+ui.score.text);

		User user = new User(ui.id.text, ui.score.text);
		string json = JsonUtility.ToJson(user);
		reference.Child("users").Child(ui.user.text).SetRawJsonValueAsync(json);

	}

	public void getDataFromFirebase( ){
		reference.Child("users").GetValueAsync().ContinueWith(task => {
			if (task.IsFaulted) {
				print("ERRO: Não foi possive listar dados.");
			}
			else
				if (task.IsCompleted) {
					DataSnapshot snapshot = task.Result;
					print("snapshot.GetRawJsonValue: "+snapshot.GetRawJsonValue());

					User[] user = jsonToUser(snapshot.GetRawJsonValue());

					print("USERS: " + user.Length);
					string temp = "";
					for (int c = 0; c < user.Length; c++)
						temp+="["+(c+1)+"º] "+user[c].email +"\t"+ user[c].username+System.Environment.NewLine;
					ui.setLeaders(temp);
				}
		});
	}
		

	public User[] jsonToUser(string json){
		//Iserir pontos para o SPLIT
		json = json.Replace ("},", "}~");
		json = json.Replace ("\":{", "\"~{");
		//Deletar primeira e ultima chave ('{' e '}')
		json = json.Remove (0, 1);
		json = json.Remove (json.Length-1,1);
		//Realizo o SPLIT
		string[] s = json.Split ('~');

		User[] user = new User [s.Length/2];
		for (int c = 0; c < user.Length; c++) user [c] = JsonUtility.FromJson<User> (s [c * 2 + 1]);
		return user;
	}

	public void createAccount(){

		uiLogin.status.text = "CREATEACCOUNT()..." ;

		Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

		auth.CreateUserWithEmailAndPasswordAsync(uiLogin.name.text, uiLogin.password.text).ContinueWith(task => {
			if (task.IsCanceled) {
				Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
				uiLogin.status.text += "SignInWithEmailAndPasswordAsync was canceled.";
				return;
			}
			if (task.IsFaulted) {
				Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
				uiLogin.status.text += "CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception;
				return;
			}

			// Firebase user has been created.
			Firebase.Auth.FirebaseUser newUser = task.Result;
			//Debug.LogFormat("Firebase user created successfully: {0} ({1})",newUser.DisplayName, newUser.UserId);

			uiLogin.status.text += "Firebase user created successfully: "+newUser.DisplayName+" ("+newUser.UserId+")";


			print("DisplayName: "+newUser.DisplayName);
			print("Email: "+newUser.Email);
			print("IsAnonymous: "+newUser.IsAnonymous);
			print("IsEmailVerified: "+newUser.IsEmailVerified);
			print("PhoneNumber: "+newUser.PhoneNumber);
			print("PhotoUrl: "+newUser.PhotoUrl);
			print("ProviderData: "+newUser.ProviderData);
			print("ProviderId: "+newUser.ProviderId);
			print("UserId: "+newUser.UserId);


		});
	
	}

	public void loginAccount(){
		//LOGINS ATIVOS: "kurtyebsb@gmail.com", "1senhaforte2SENHAFORTE3"

		uiLogin.status.text = "LOGINACCOUNT()..." ;

		Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

		auth.SignInWithEmailAndPasswordAsync(uiLogin.name.text, uiLogin.password.text).ContinueWith(task => {
			if (task.IsCanceled) {
				Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
				uiLogin.status.text += "SignInWithEmailAndPasswordAsync was canceled.";
				return;
			}
			if (task.IsFaulted) {
				Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
				uiLogin.status.text += "SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception;
				return;
			}

			Firebase.Auth.FirebaseUser newUser = task.Result;
			Debug.LogFormat("User signed in successfully: {0} ({1})",
				newUser.DisplayName, newUser.UserId);

			uiLogin.status.text += "User signed in successfully: "+newUser.DisplayName+" ("+newUser.UserId+")";
		});
	}

	public void loginWithFacebook(){
		//FIREBASE 
			//Projeto: TesteA
			//App 1 no Projeto: com.DivisaoZero.OneStarGameBase
			//URI de redirecionamento de OAuth para o Facebook: https://testea-f9de1.firebaseapp.com/__/auth/handler
		//FACEBOOK
			//Versão da API: v2.11
			//ID do Aplicativo: 189407058291283
			//Chave Secreta do Aplicativo: 4a859d69b53e2fe6494f486576eacf09
			//Acessado em: https://developers.facebook.com/tools/accesstoken/
		//User Tokem [ela expira]: EAACsQ8RtalMBANPZAaoyWOCQ66CfUsZBbr8YrcukSOiO68PwqwQjZA7pwhAGbv6YDqaDDYCEdhWKONo3zuojqn9BYTBNikZBqZA719twospDpdpp7IhyupjN6Q0h6wlDMIqGfsCkNDXEppZA0khudnHZCv1jogm5t9Yg3pnrF9pfg0iPZBRP7fJuDsd5133FVgkZD
				//App Token: 189407058291283|B47y5x6RuE33dL9VGm6lcLlicp8

		uiLogin.status.text = "LOGIN WITH FACEBOOK..." ;

		//auth é o gateway para todas as chamadas de API.
		Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

		//Pergunta em: https://stackoverflow.com/questions/46866013/unity3d-firebase-facebook-login?newreg=c68fe261a86e4712b819c2bea2eb65e8
		//Dica de: https://developers.facebook.com/docs/facebook-login/access-tokens/?locale=pt_BR#apptokens



		Firebase.Auth.Credential credential = Firebase.Auth.FacebookAuthProvider.GetCredential( accessToken );
	  	auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
	  		if (task.IsCanceled) {
	    		Debug.LogError("SignInWithCredentialAsync was canceled.");
				uiLogin.status.text += "SignInWithCredentialAsync was canceled.";
	    		return;
		  	}
		  	if (task.IsFaulted) {
	    		Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
				uiLogin.status.text += "SignInWithCredentialAsync encountered an error: " + task.Exception;
				uiLogin.status.text += " >> Data: "+ task.Exception.Data;
				uiLogin.status.text += " >> HelpLink: "+ task.Exception.HelpLink;
				uiLogin.status.text += " >> InnerException: "+ task.Exception.InnerException;
				uiLogin.status.text += " >> InnerExceptions: "+ task.Exception.InnerExceptions;
				uiLogin.status.text += " >> Message: "+ task.Exception.Message;
				uiLogin.status.text += " >> Source: "+ task.Exception.Source;
				uiLogin.status.text += " >> StackTrace: "+ task.Exception.StackTrace;
				uiLogin.status.text += " >> TargetSite: "+ task.Exception.TargetSite;
				uiLogin.status.text += " >> ToString(): "+ task.Exception.ToString();
	    		return;
		  	}

		  	Firebase.Auth.FirebaseUser newUser = task.Result;
	  		Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
			uiLogin.status.text += "User signed in successfully: "+newUser.DisplayName+" ("+newUser.UserId+")";


			Firebase.Auth.FirebaseUser user = auth.CurrentUser;
			if (user != null) {
				string name = user.DisplayName;
				string email = user.Email;
				System.Uri photo_url = user.PhotoUrl;
				// The user's Id, unique to the Firebase project.
				// Do NOT use this value to authenticate with your backend server, if you
				// have one; use User.TokenAsync() instead.
				string uid = user.UserId;
			}
		});

	}

	void Start() {
		// Set these values before calling into the realtime database.
		// É uma configuração para acesso público;
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://testea-f9de1.firebaseio.com"); 

		// Get the root reference location of the database.
		//Para gravar dados no Database, você precisa de uma instância de DatabaseReference.
		reference = FirebaseDatabase.DefaultInstance.RootReference;

		//loginAccount ();

		uiLogin.status.text = "START" ;

	}


}
