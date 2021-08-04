using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjZeiWei.Models
{
	public class MemberLogin
	{
		public string fId { get; set; }
		public string fToken { get; set; }
		public string errorMessages { get; set; }
		public byte[] photoFile{ get; set; }
		public string imageMimeType { get; set; }
	}
}