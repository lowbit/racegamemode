using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace RaceClient
{
	public static class CommonClass
	{
		public static void SendChatMessage(string message, int r, int g, int b, string from = "")
		{
			var msg = new Dictionary<string, object>
			{
				["color"] = new[] { r, g, b },
				["args"] = new[] { from == "" ? "[SERVER]" : from, message }
			};

			BaseScript.TriggerEvent("chat:addMessage", msg);
		}
	}
}
