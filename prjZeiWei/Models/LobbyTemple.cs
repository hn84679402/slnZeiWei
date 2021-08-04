using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjZeiWei.Models
{
	public class LobbyTemple
	{
		public string id { get; set; }
		public string name { get; set; }
		public string sex { get; set; }
		public string age { get; set; }
		public string pair1 { get; set; }
		public string pair2 { get; set; }
		public string pair3 { get; set; }
		public string pair4 { get; set; }
		public int matchLevel { get; set; }
		public string errorMessage { get; set; }
	}
}