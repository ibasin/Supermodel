using System;

namespace BrowserEmulator {
	public class HttpField {
		public enum Type : byte { TextBox = 1, TextArea, CheckBox, Radio, Select, EmptySelect, RuntimeSelect, Hidden, HiddenConst, Submit, Image, Reset, Button, Label };

		public HttpField(HttpField original) {
			HttpName = original.HttpName;
			HttpValue = original.HttpValue;
			ScreenName = original.ScreenName;
			ScreenValue = original.ScreenValue;
			FieldType = original.FieldType;
			Submit = original.Submit;
			Dynamic = original.Dynamic;
			Visible = original.Visible;
		}
		public HttpField(string p_httpName, string p_httpValue, string p_screenName, string p_screenValue, Type p_type) {
			HttpName = p_httpName;
			HttpValue = p_httpValue;
			ScreenName = p_screenName;
			ScreenValue = p_screenValue;
			FieldType = p_type;
		}

		public string ScreenName = "";
		public string ScreenValue = "";
		public string HttpName = "";
		public string HttpValue = "";
		public Type FieldType = HttpField.Type.TextBox;

		public bool Submit = true;
		public bool Dynamic = false;
		public bool Visible {
			get {
				return mVisible;
			}
			set {
				mVisible = value;
				if (value == false) ScreenName = "Invisible";
			}
		}
		private bool mVisible = true;
	}
}
