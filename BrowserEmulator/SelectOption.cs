using System;

namespace BrowserEmulator {
	public class SelectOption {
		public SelectOption(string p_httpValue, string p_screenValue) {
			HttpValue = p_httpValue;
			ScreenValue = p_screenValue;
		}
		public string HttpValue = "";
		public string ScreenValue = "";
	}
}
