using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance.Models
{
	public class CollectionItem
	{
		public string ReceiverID { get; set; }
		public string SenderID { get; set; }
		public string Message { get; set; }

		public string Time { get; set; }
		public int  Seen { get; set;}


	}
}
