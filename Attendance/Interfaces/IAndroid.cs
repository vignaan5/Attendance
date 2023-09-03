using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Attendance.Interfaces
{
	public interface IAndroid
	{
		public void StartMyService();

		public void StopMyService();

		public bool IsForeGroundServiceRunning();

		public void update_duration(int seconds);

	}
}
