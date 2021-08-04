using prjZeiWei.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace prjZeiWei.Controllers
{
    public class MemberInfoController : Controller
    {
		WebClient client = new WebClient();
		JavaScriptSerializer serializer = new JavaScriptSerializer();
		// GET: MemberInfo
		public ActionResult ShowMemberInfo()
        {
			//初始化WebClient 並傳送請求到WebAPI
			client.Encoding = Encoding.UTF8;
			client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
			var body = client.DownloadString("http://localhost:7717/api/memberinfo/getMemberInfo?fid=" + Session["fid"].ToString());
			//反序列化
			var list = new JavaScriptSerializer().Deserialize<List<MemberInfo>>(body);
			MemberInfo memberinfo = new MemberInfo();
			//倒出資料
			foreach (var item in list)
			{
				memberinfo = item;
			}
			//回傳會員資料
			return View(memberinfo);
        }
	}
}