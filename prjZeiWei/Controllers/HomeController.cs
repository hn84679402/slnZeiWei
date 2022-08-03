using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
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
		string token = "";
		public ActionResult Index(string errorMessages)
		{
			//回傳API的錯誤訊息給前端顯示
			ViewBag.errorMessages = errorMessages;
			return View();
		}
		[HttpPost]
		public ActionResult checkValidationNumber(string fAccount, string fPassword, string requestPath,  string validationNumber, string CheckPassword= "")
		{
			ViewBag.ID = fAccount;
			//檢查驗證碼是否有誤
			if(TempData["Number"].ToString() != validationNumber)
			{
				if (requestPath == "/Home/Index" || requestPath == "/")
					return RedirectToAction("Index", "Home", new { errorMessages = "驗證碼錯誤"});
				else
					return RedirectToAction("Register", "Home", new { errorMessages = "驗證碼錯誤" });
			}
			//分流登入及註冊導向的頁面
			if(requestPath == "/Home/Index" || requestPath == "/")
				return RedirectToAction("Login", "Home", new { fAccount = fAccount , fPassword = fPassword});
			else
				return RedirectToAction("RegisterGet", "Home", new { fAccount = fAccount, fPassword = fPassword , CheckPassword  = CheckPassword});
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
				//Get backend WebAPI
				var body = client.DownloadString("http://localhost:7717/api/Member/MemberLogin?fAccount=" + fAccount + "&fPassword=" + fPassword);
				//反序列化
				var list = new JavaScriptSerializer().Deserialize<List<MemberLogin>>(body);
				//把資料倒出來
				foreach (var member in list)
				{
					fid = member.fId;
					fToken = member.fToken;
					errorMessages = member.errorMessages;
				}
				
				Console.WriteLine(fid + fToken + errorMessages);
				//依照errorMessages決定頁面導向
				if (errorMessages == "登入成功")
				{
					return RedirectToAction("Index", "Lobby", new { fToken = fToken, fid = fid });
				}
				else
				{
					return RedirectToAction("Index", "Home", new { errorMessages = errorMessages });
				}
			}
			//API伺服器未開啟或其他HTTP問題 導回index並回傳錯誤訊息
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return RedirectToAction("Index", "Home", new { errorMessages = "伺服器維護中，請稍後再試"});
			}			
		}
		public ActionResult Register(string errorMessages)
		{
			//回傳API的錯誤訊息給前端顯示
			ViewBag.errorMessages = errorMessages;
			return View();
		}
		public ActionResult RegisterGet(string fAccount, string fPassword, string CheckPassword)
		{
			string fid = "";
			string fToken = "";
			string errorMessages = "登入成功";
			//先驗證密碼是否有不一致
			if (fPassword != CheckPassword)
				return RedirectToAction("Register", "Home", new { errorMessages = "請重復確認密碼是否一致" });

			client.Encoding = Encoding.UTF8;
			client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
			//打後端API確認註冊狀態
			var body = client.DownloadString("http://localhost:7717/api/Member/registerZeiWeiMember?fAccount=" + fAccount + "&fPassword=" + fPassword);
			//TODO:反序列化
			var list = serializer.Deserialize<List<MemberLogin>>(body);
			foreach (var member in list)
			{
				fid = member.fId;
				fToken = member.fToken;
				errorMessages = member.errorMessages;
			}
			fToken = token;
			Console.WriteLine(errorMessages);
			//依照API回傳的錯誤訊息 決定是否要讓使用者繼續下一步
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
			//產生性別的SelectListItem
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
			//產生西元年的SelectListItem
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
			//產生月份的SelectListItem
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
			//產生日期的SelectListItem
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
			string[] textArray = { "子", "丑", "寅" , "卯", "辰", "巳", "午", "未", "申", "酉", "戌" , "亥" };
			//產生時辰的SelectListItem
			List<SelectListItem> TimeSelectItemList = new List<SelectListItem>();
			for(int a = 0; a < textArray.Length; a++)
			{
				TimeSelectItemList.Add(new SelectListItem()
				{
					Text = textArray[a],
					Value = textArray[a],
					Selected = false
				});
			}

			//利用ViewBag把所有的SelectListItem帶到前端顯示
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
			//從已登入的會員帶過來的ID會是空 從Session拿出來
			if (Mid == "")
				Mid = Session["fid"].ToString();
			string status = "";			
			client.Encoding = Encoding.UTF8;
			client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
			var body = client.DownloadString("http://localhost:7717/api/Member/GettCase?Mid=" + Mid + "&MLName=" + MLName + "&MLSex=" + MLSex + "&MLBirYear=" + MLBirYear + "&MLBirMonth=" + MLBirMonth+ "&MLBirDate=" + MLBirDate+ "&MLBirTime=" + MLBirTime);
			//TODO:反序列化			
			var list = serializer.Deserialize<List<Life>>(body);
			foreach (var member in list)
			{
				status = member.Status;
			}
			if(status == "新增資料成功" )
			{
				return RedirectToAction("Index", "Home", new { mid = Mid});
			}
			else if (status == "更新資料成功")
			{
				return RedirectToAction("ShowMemberInfo", "MemberInfo");
			}
			else
			{
				return RedirectToAction("MemberData", "Home", new { mid = Mid});
			}
		}
		public FileContentResult getVaildationImg()
		{
			//取得五碼的亂數驗證碼
			string validationString = setAStringNumber(5);
			//儲存驗證碼以供確認
			TempData["Number"] = validationString;
			//設定驗證圖尺寸
			Bitmap img = new Bitmap(validationString.Length * 20, 40);
			Graphics g = Graphics.FromImage(img);
			//Random RGB Number for BackGround
			Random rd = new Random();
			int intRed = rd.Next(180, 256);
			int intGreen = rd.Next(180, 256);
			int intBlue = rd.Next(180, 256);
			//Clear BackGroud,then draw the color by random
			g.Clear(Color.FromArgb(intRed, intGreen, intBlue));
			//draw the Noise line
			int x1 = 0, x2 = 0, y1 = 0, y2 = 0;
			for (int i = 0; i < 50; i++)
			{
				x1 = rd.Next(0, img.Width);
				y2 = rd.Next(0, img.Height);

				x2 = rd.Next(0, img.Width);
				y2 = rd.Next(0, img.Height);

				g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
			}
			//Draw the Noise point
			for (int i = 0; i < 300; i++)
			{
				x1 = rd.Next(0, img.Width);
				y2 = rd.Next(0, img.Height);

				img.SetPixel(x1, y1, Color.FromArgb(rd.Next(256), rd.Next(256), rd.Next(256)));
			}
			//Draw font,use gradient color
			Rectangle myRect = new Rectangle(0, 0, img.Width, img.Height);
			LinearGradientBrush brush = new LinearGradientBrush(myRect, Color.FromArgb(rd.Next(256), rd.Next(256), rd.Next(256)), Color.FromArgb(rd.Next(256), rd.Next(256), rd.Next(256)), 1.2f);
			//Set font-family
			Font font = new Font("Calibri", 20, FontStyle.Bold);
			//Draw validation number on Graphic
			g.DrawString(validationString, font, brush, 5, 5);
			//Draw Graphic's border
			g.DrawRectangle(new Pen(Color.Silver), 0, 0, img.Width, img.Height);

			Image image = img;

			MemoryStream ms = new MemoryStream();
			image.Save(ms, ImageFormat.Jpeg);
			return File(ms.ToArray(), "image/Jpeg");
		}
		public string setAStringNumber(int validationCount)
		{
			string strNumber = "";
			//因為數字比較方便使用者輸入 多加兩組提高數字出現在驗證碼的機率
			//另外拿掉易產生誤會的0 O l 等驗證碼 方便使用者輸入
			string str = @"123456789123456789123456789abcdefghijkmnpqrstuvwxyzABCDEFGHIJKLMNPQRSTUVWXYZ";
			Random rd = new Random();
			for (int i = 0; i < validationCount; i++)
			{
				strNumber += str.Substring(rd.Next(74), 1);
			}
			return strNumber;
		}
	}
}