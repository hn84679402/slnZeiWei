using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace prjZeiWei.Controllers
{
    public class ChatController : Controller
    {
		private static Socket _clientSocket= new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		
		// GET: Chat
		public void sendMessage(string message, string userName)
		{
			//使用迴圈去打聊天系統 打到聊天系統開啟
			LoopConnect();
			//確認Socket連線建立 發送訊息by發送人
			SendLoop(userName, message);
		}

		private ActionResult SendLoop(string userNickName, string message)
		{
			byte[] buffer;
			//刷新聊天室聊天內容使用
			if (message == "get chat")
			{
				buffer = Encoding.UTF8.GetBytes("get chat");
				_clientSocket.Send(buffer);
			}
			//首次加入聊天室
			else if (message == "")
			{
				buffer = Encoding.UTF8.GetBytes(userNickName + "進入聊天室");
				_clientSocket.Send(buffer);
			}
			//離開聊天室頁面或登出
			else if (message == "logout")
			{
				buffer = Encoding.UTF8.GetBytes(userNickName + "離開聊天室");
				_clientSocket.Send(buffer);
			}
			//傳送訊息
			else
			{
				buffer = Encoding.UTF8.GetBytes(userNickName + ":" + message);
				_clientSocket.Send(buffer);
			}

			byte[] receivedBuf = new byte[1024];
			//取得伺服器那邊的聊天訊息
			int rec = _clientSocket.Receive(receivedBuf);
			byte[] data = new byte[rec];
			Array.Copy(receivedBuf, data, rec);
			Console.WriteLine("Received:" + Encoding.UTF8.GetString(data));
			string request = Encoding.UTF8.GetString(data);
			Response.Write(request);
			return View();
		}

		private void LoopConnect()
		{
			int attempts = 0;
			//打聊天伺服器 直到Socket打通為止
			while (!_clientSocket.Connected)
			{
				try
				{
					attempts++;
					_clientSocket.Connect(IPAddress.Loopback, 100);
				}
				catch (SocketException)
				{
					Console.Clear();
					Console.WriteLine("Connection attampts" + attempts.ToString());
				}
			}
		}

		public ActionResult chatRoom(string mid)
		{
			//如果會員未登入 導回登入頁面
			if (Session["fToken"] == null || Session["fid"] == null)
				return RedirectToAction("Index", "Home");
			//如果有登入 用ViewBag帶回會員的暱稱供前端顯示
			ViewBag.userName = Session["userName"].ToString();
			return View();
		}
	}
}