using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace prjZeiWei.Models
{
    public class HomeController : Controller
    {
		JavaScriptSerializer serializer = new JavaScriptSerializer();
		WebClient client = new WebClient();
		// GET: Home
		string token = "";
		public ActionResult Index(string errorMessages)
		{
			ViewBag.errorMessages = errorMessages;
			return View();
		}
		public ActionResult Login(string fAccount, string fPassword)
		{
			string fid = "";
			string fToken = "";
			string errorMessages = "登入成功";
			WebClient client = new WebClient();
			client.Encoding = Encoding.UTF8;
			client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
			try
			{
				var body = client.DownloadString("http://localhost:4099/api/loginzeiwei?fAccount=" + fAccount + "&fPassword=" + fPassword);
				//反序列化
				var list = new JavaScriptSerializer().Deserialize<List<Member>>(body);
				foreach (var member in list)
				{
					fid = member.fId;
					fToken = member.fToken;
					errorMessages = member.errorMessages;
				}
				Console.WriteLine(fid + fToken + errorMessages);
				fToken = token;
				if (errorMessages == "登入失敗")
				{
					return RedirectToAction("Index", "Home", new { errorMessages = errorMessages });
				}
				else
				{
					return RedirectToAction("Index", "Lobby", new { fToken = fToken, fid = fid });
				}
			}
			catch (Exception ex)
			{
				return RedirectToAction("Index", "Home", new { errorMessages = "伺服器維護中，請稍後再試"});
			}			
		}
		public ActionResult Register(string errorMessages)
		{
			ViewBag.errorMessages = errorMessages;
			return View();
		}
		public ActionResult RegisterGet(string fAccount, string fPassword, string CheckPassword)
		{
			string fid = "";
			string fToken = "";
			string errorMessages = "登入成功";
			if (fPassword != CheckPassword)
				return RedirectToAction("Register", "Home", new { errorMessages = "請重復確認密碼" });			
			client.Encoding = Encoding.UTF8;
			client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
			var body = client.DownloadString("http://localhost:4099/api/registerzeiwei?fAccount=" + fAccount + "&fPassword=" + fPassword);
			//TODO:反序列化
			var list = serializer.Deserialize<List<Member>>(body);
			foreach (var member in list)
			{
				fid = member.fId;
				fToken = member.fToken;
				errorMessages = member.errorMessages;
			}
			fToken = token;
			Console.WriteLine(errorMessages);
			if (errorMessages == "帳號重複")
			{
				return RedirectToAction("Register", "Home", new { errorMessages = errorMessages });
			}
			else
			{
				return RedirectToAction("MemberData", "Home", new { mid = fid});
			}
		}
		public ActionResult MemberData(string mid)
		{

			List<SelectListItem> mySelectItemList = new List<SelectListItem>();
			mySelectItemList.Add(new SelectListItem()
			{
				Text = "男",
				Value ="M",
				Selected = true
			});
			mySelectItemList.Add(new SelectListItem()
			{
				Text = "女",
				Value = "F",
				Selected = false
			});
			List<SelectListItem> YearSelectItemList = new List<SelectListItem>();
			for(int a = 2019; a > 1900; a--)
			{
				YearSelectItemList.Add(new SelectListItem()
				{
					Text = a.ToString(),
					Value = a.ToString(),
					Selected = false
				});
			}
			List<SelectListItem> MonthSelectItemList = new List<SelectListItem>();
			for (int a = 1; a < 13; a++)
			{
				MonthSelectItemList.Add(new SelectListItem()
				{
					Text = a.ToString(),
					Value = a.ToString(),
					Selected = false
				});
			}
			List<SelectListItem> DateSelectItemList = new List<SelectListItem>();
			for (int a = 1; a < 32; a++)
			{
				DateSelectItemList.Add(new SelectListItem()
				{
					Text = a.ToString(),
					Value = a.ToString(),
					Selected = false
				});
			}
			List<SelectListItem> TimeSelectItemList = new List<SelectListItem>();
			TimeSelectItemList.Add(new SelectListItem()
			{
				Text = "子",
				Value = "子",
				Selected = true
			});
			TimeSelectItemList.Add(new SelectListItem()
			{
				Text = "丑",
				Value = "丑",
				Selected = false
			});
			TimeSelectItemList.Add(new SelectListItem()
			{
				Text = "寅",
				Value = "寅",
				Selected = false
			});
			TimeSelectItemList.Add(new SelectListItem()
			{
				Text = "卯",
				Value = "卯",
				Selected = false
			});
			TimeSelectItemList.Add(new SelectListItem()
			{
				Text = "辰",
				Value = "辰",
				Selected = false
			});
			TimeSelectItemList.Add(new SelectListItem()
			{
				Text = "巳",
				Value = "巳",
				Selected = false
			});
			TimeSelectItemList.Add(new SelectListItem()
			{
				Text = "午",
				Value = "午",
				Selected = false
			}); TimeSelectItemList.Add(new SelectListItem()
			{
				Text = "未",
				Value = "未",
				Selected = false
			});
			TimeSelectItemList.Add(new SelectListItem()
			{
				Text = "申",
				Value = "申",
				Selected = false
			});
			TimeSelectItemList.Add(new SelectListItem()
			{
				Text = "酉",
				Value = "酉",
				Selected = false
			});
			TimeSelectItemList.Add(new SelectListItem()
			{
				Text = "戌",
				Value = "戌",
				Selected = false
			});
			TimeSelectItemList.Add(new SelectListItem()
			{
				Text = "亥",
				Value = "亥",
				Selected = false
			});
			ViewBag.Mid = mid;
			ViewBag.MLSex = mySelectItemList;
			ViewBag.MLBirYear = YearSelectItemList;
			ViewBag.MLBirMonth = MonthSelectItemList;
			ViewBag.MLBirDate = DateSelectItemList;
			ViewBag.MLBirTime = TimeSelectItemList;
			return View();
		}
		public ActionResult MemberLife(string Mid, string MLName,string MLSex,string MLBirYear, string MLBirMonth, string MLBirDate, string MLBirTime)
		{
			string status = "";			
			client.Encoding = Encoding.UTF8;
			client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
			var body = client.DownloadString("http://localhost:4099/api/MemberLife?Mid=" + Mid + "&MLName=" + MLName + "&MLSex=" + MLSex + "&MLBirYear=" + MLBirYear + "&MLBirMonth=" + MLBirMonth+ "&MLBirDate=" + MLBirDate+ "&MLBirTime=" + MLBirTime);
			//TODO:反序列化			
			var list = serializer.Deserialize<List<Life>>(body);
			foreach (var member in list)
			{
				status = member.Status;
			}
			if(status == "新增資料成功")
			{
				return RedirectToAction("Index", "Home", new { mid = Mid});
			}
			else
			{
				return RedirectToAction("MemberData", "Home", new { mid = Mid});
			}
			return View();
		}
    }
}