using prjZeiWei.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace prjZeiWei.Controllers
{
    public class LobbyController : Controller
    {
		WebClient client = new WebClient();
		JavaScriptSerializer serializer = new JavaScriptSerializer();

		// GET: Lobby
		public ActionResult Index(string fToken="", string fid="")
        {
			//剛登入進來的用戶記住token跟id
			if (fToken != "" & fid != "")
			{
				Session["fToken"] = fToken;
				Session["fid"] = fid;
			}
			//驗證是否是已登入的用戶 如不是導回首頁
			else
			{
				if (Session["fToken"] == null || Session["fid"] == null)
					return RedirectToAction("Index", "Home");
				else
					fid = Session["fid"].ToString();
			}
			ViewBag.fid = fid + ".jpg";
			client.Encoding = Encoding.UTF8;
			client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
			var body = client.DownloadString("http://localhost:7717/api/lobby/selectAllMemeber?memberId=" + fid );
			//TODO:反序列化
			var list = serializer.Deserialize<List<LobbyTemple>>(body);
			//取得會員暱稱供後續聊天功能使用
			foreach (var item in list)
			{
				if(item.errorMessage == "未輸入命盤資料")
					return RedirectToAction("MemberData", "Home", new { mid = fid });
				if (item.id == fid + ".jpg")
					Session["userName"] = item.name;
			}
			//比對登入會員與其他異性的速配指數
			list = setMatchLevel(fid+".jpg", list);
			
			return View(list);
        }

		public ActionResult Life()
		{
			//未登入的用戶導回首頁
			if (Session["fToken"] == null || Session["fid"] == null)
				return RedirectToAction("Index", "Home");
			string life = "";
			string move = "";
			string company = "";
			string money = "";
			string love = "";
			string friend = "";
			//初始化WebClient並呼叫WebAPI
			client.Encoding = Encoding.UTF8;
			client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
			var body = client.DownloadString("http://localhost:7717/api/lobby/getLife?fid=" + Session["fid"].ToString());
			//TODO:反序列化
			var list = serializer.Deserialize<List<LifePage>>(body);
			foreach (var member in list)
			{
				life = member.life;
				move = member.move;
				company = member.company;
				money = member.money;
				love = member.love;
				friend = member.friend;
			}
			ViewBag.Life = life;
			ViewBag.Move = move;
			ViewBag.Company = company;
			ViewBag.Money = money;
			ViewBag.Love = love;
			ViewBag.Friend = friend;
			return View();
		}
		public ActionResult Money()
		{
			//未登入的用戶導回首頁
			if (Session["fToken"] == null || Session["fid"] == null)
				return RedirectToAction("Index", "Home");
			string life = "";
			string move = "";
			string company = "";
			string money = "";
			string love = "";
			string friend = "";
			//初始化WebClient並呼叫WebAPI
			client.Encoding = Encoding.UTF8;
			client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
			var body = client.DownloadString("http://localhost:7717/api/lobby/getLife?fid=" + Session["fid"].ToString());
			//TODO:反序列化
			var list = serializer.Deserialize<List<LifePage>>(body);
			foreach (var member in list)
			{
				life = member.life;
				move = member.move;
				company = member.company;
				money = member.money;
				love = member.love;
				friend = member.friend;
			}
			ViewBag.Life = life;
			ViewBag.Move = move;
			ViewBag.Company = company;
			ViewBag.Money = money;
			ViewBag.Love = love;
			ViewBag.Friend = friend;
			return View();
		}
		public ActionResult Love()
		{
			//未登入的用戶導回首頁
			if (Session["fToken"] == null || Session["fid"] == null)
				return RedirectToAction("Index", "Home");
			string life = "";
			string move = "";
			string company = "";
			string money = "";
			string love = "";
			string friend = "";
			//初始化WebClient並呼叫WebAPI
			client.Encoding = Encoding.UTF8;
			client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
			var body = client.DownloadString("http://localhost:7717/api/lobby/getLife?fid=" + Session["fid"].ToString());
			//TODO:反序列化
			var list = serializer.Deserialize<List<LifePage>>(body);
			foreach (var member in list)
			{
				life = member.life;
				move = member.move;
				company = member.company;
				money = member.money;
				love = member.love;
				friend = member.friend;
			}
			ViewBag.Life = life;
			ViewBag.Move = move;
			ViewBag.Company = company;
			ViewBag.Money = money;
			ViewBag.Love = love;
			ViewBag.Friend = friend;
			return View();
		}
		//取得本機IPV6位置 提供日後部屬到IIS的路徑使用
		private static IEnumerable<String> GetLocalIPV6IP()
		{
			return (from adapter in NetworkInterface.GetAllNetworkInterfaces()
					where adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet
					from AddressInfo in adapter.GetIPProperties().UnicastAddresses.OfType<UnicastIPAddressInformation>()
					where AddressInfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6
					let ipAddress = AddressInfo.Address.ToString()
					select ipAddress);
		}
		public ActionResult uploadPhoto()
		{
			return View();
		}
		[HttpPost]
		public ActionResult uploadPhoto(MemberLogin member, HttpPostedFileBase image)
		{
			var path = String.Empty;
			var uploadDir = Server.MapPath("~/images");
			try{
				if (image != null && image.ContentLength > 0)
				{
					if (!Directory.Exists(uploadDir))
						Directory.CreateDirectory(uploadDir);
					path = Path.Combine(uploadDir, (Session["fid"].ToString() + ".jpg"));
					image.SaveAs(path);
				}
			}
			catch (IOException ex)
			{
				ViewBag.Message = "上傳檔案失敗";
				Console.WriteLine(ex.ToString());
			}
			ViewBag.Message = "上傳檔案成功";
			return View();
		}
		public ActionResult Logout()
		{
			//登出帳號 清除所有Session
			Session.RemoveAll();
			return RedirectToAction("Index", "Home");
		}
		private List<LobbyTemple> setMatchLevel(string fid, List<LobbyTemple> list)
		{
			LobbyTemple lobbyTmp = new LobbyTemple();
			//讀取會員本人的資料提供比對
			foreach (var item in list)
			{
				if (item.id == fid)
					lobbyTmp = item;
			}
			//把會員本人資料去除
			list.Remove(lobbyTmp);
			//比對會員與其他異性會員的速配等級
			foreach (var item in list)
			{
				if (item.id != fid)
					item.matchLevel = getMatchLevel(item, lobbyTmp);
			}
			return list;
		}

		private int getMatchLevel(LobbyTemple item, LobbyTemple lobbyTmp)
		{
			int i = 0;
			//把會員的命盤資料與異性會員的資料轉換成陣列提供比對
			string[] pair1OppositeList = item.pair1.Split('.');
			string[] pair1MemberList = lobbyTmp.pair1.Split('.');
			string[] pair2OppositeList = item.pair2.Split('.');
			string[] pair2MemberList = lobbyTmp.pair2.Split('.');
			string[] pair3OppositeList = item.pair3.Split('.');
			string[] pair3MemberList = lobbyTmp.pair3.Split('.');
			string[] pair4OppositeList = item.pair4.Split('.');
			string[] pair4MemberList = lobbyTmp.pair4.Split('.');
			for (int j = 0; j< pair1OppositeList.Length; j++)
			{
				//比對命盤 並產生速配指數
				for(int k = 0; k < pair1OppositeList.Length; k++)
				{
					if (pair1OppositeList[j] == pair1MemberList[k])
						i++;
					if (pair2OppositeList[j] == pair2MemberList[k])
						i++;
					if (pair3OppositeList[j] == pair3MemberList[k])
						i++;
					if (pair4OppositeList[j] == pair4MemberList[k])
						i++;
				}
			}
			return i;
		}
	}
}