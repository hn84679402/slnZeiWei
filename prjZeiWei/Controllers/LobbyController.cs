using prjZeiWei.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
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
		public ActionResult Index(string fToken, string fid)
        {
			ViewBag.fToken = fToken;
			ViewBag.fid = fid;
			string[] id = new string[] { "01", "02", "05", "04" };
			string[] name = new string[] { "命盤個性", "命盤財運", "命盤姻緣", "聊天交友"};
			string[] address = new string[] { "個性可以影響你的一生", "你是不是土豪看這就知道", "你是異性絕緣體?或是桃花王", "交友聊天一點通"};
			string[] controller = new string[] { "Life", "Money", "Love", "Friend"};
			List<LobbyTemple> list = new List<LobbyTemple>();
			for (var i = 0; i < id.Length; i++)
			{
				LobbyTemple lobbyTemple = new LobbyTemple();
				lobbyTemple.Id = id[i];
				lobbyTemple.Name = name[i];
				lobbyTemple.Address = address[i];
				lobbyTemple.Controller = controller[i];
				list.Add(lobbyTemple);
			}
			foreach(var i in GetLocalIPV6IP())
			{
				ViewBag.IP = i;
			}
			return View(list);
        }
		public ActionResult Life(string mid)
		{
			if (mid == null)
				return RedirectToAction("Index", "Home");
			string mllife="";
			string mlmove="";			
			client.Encoding = Encoding.UTF8; client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
			var body = client.DownloadString("http://localhost:4099/api/lifezeiwei?mid=" + mid);
			//TODO:反序列化     			
			var list = serializer.Deserialize<List<LifePage>>(body);
			foreach (var member in list)
			{
				mllife = member.MLLife;
				mlmove = member.MLMove;
			}
			ViewBag.Life = mllife;
			ViewBag.Move = mlmove;
			string[] arrlife = mllife.Split('.');
			ViewBag.Life1 = arrlife[0];
			return View();
		}
		public ActionResult Money(string mid)
		{
			if (mid == null)
				return RedirectToAction("Index", "Home");
			string mlmoney = "";
			string mllucky = "";			
			client.Encoding = Encoding.UTF8; client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
			var body = client.DownloadString("http://localhost:4099/api/moneyzeiwei?mid=" + mid);
			//TODO:反序列化     
			var list = serializer.Deserialize<List<MoneyPage>>(body);
			foreach (var member in list)
			{
				mlmoney = member.MLMoney;
				mllucky = member.MLLucky;
			}
			ViewBag.Money = mlmoney;
			ViewBag.Lucky = mllucky;
			return View();
		}
		public ActionResult Love(string mid)
		{
			if (mid == null)
				return RedirectToAction("Index", "Home");
			string mlcompany = "";
			string mlcouple = "";			
			client.Encoding = Encoding.UTF8; client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
			var body = client.DownloadString("http://localhost:4099/api/lovezeiwei?mid=" + mid);
			//TODO:反序列化
			var list = serializer.Deserialize<List<LovePage>>(body);
			foreach (var member in list)
			{
				mlcompany = member.MLCompany;
				mlcouple = member.MLCouple;
			}
			ViewBag.Company = mlcompany;
			ViewBag.Couple = mlcouple;
			return View();
		}
		public ActionResult Friend(string mid)
		{
			if (mid == null)
				return RedirectToAction("Index", "Home");
			return View();
		}
		private static IEnumerable<String> GetLocalIPV6IP()
		{
			return (from adapter in NetworkInterface.GetAllNetworkInterfaces()
					where adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet
					from AddressInfo in adapter.GetIPProperties().UnicastAddresses.OfType<UnicastIPAddressInformation>()
					where AddressInfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6
					let ipAddress = AddressInfo.Address.ToString()
					select ipAddress);
		}
	}
}