using System;

using System.Text;
using System.Text.RegularExpressions;

namespace BrowserEmulator {
	public class Common {
		static public bool ValidateRx(string input, string pattern) {
			Match m = Regex.Match(input, pattern);
			if (m.Success == false) return false;
			if (m.Length != input.Length) return false;
			return true;
		}

		static public string PrintMMYYYY(DateTime val) {
			return val.Month + "/" + val.Year;
		}

		static public string Print(DateTime val) {
			return val.ToShortDateString();
		}

		static public string Print(int val) {
			string ret;
			if (val < 0) ret = "";
			else ret = val.ToString();
			return ret;
		}

		static public string PrintF0(float val) {
			string ret;
			if (val < 0) ret = "";
			else ret = Math.Round(val, 0).ToString("F0");
			return ret;
		}

		static public string PrintF1(float val) {
			string ret;
			if (val < 0) ret = "";
			else ret = Math.Round(val, 1).ToString("F1");
			return ret;
		}

		static public string PrintF2Negative(float val) {
			return Math.Round(val, 2).ToString("F2");
		}

		static public string PrintF2(float val) {
			string ret;
			if (val < 0) ret = "";
			else ret = Math.Round(val, 2).ToString("F2");
			return ret;
		}

		static public string PrintD2(int val) {
			return val.ToString("D2");
		}

		static public string PrintD4(int val) {
			return val.ToString("D4");
		}
	
		static public string PrintD5(int val) {
			return val.ToString("D5");
		}

		static public string PrintDollarAmountF2(float val) {
			string strVal = PrintF2(val);

			string Afterdot = strVal.Substring(strVal.IndexOf("."), 3);
			strVal = strVal.Substring(0, strVal.IndexOf("."));
			int CommaPos;

			if (strVal.Length > 3) {
				int MaxCommas = (strVal.Length / 3) - 1;

				if ((strVal.Length % 3) == 0) MaxCommas = MaxCommas - 1;

				for (int i = 0; i <= MaxCommas; i++) {
					CommaPos = ((i + 1) * 3) + i;
					strVal = strVal.Substring(0, strVal.Length - CommaPos) + "," + strVal.Substring(strVal.Length - CommaPos, CommaPos);
				}
			}
			return (strVal + Afterdot);
		}

		static public string PrintF3(float val) {
			string ret;
			if (val < 0) ret = "";
			else ret = Math.Round(val, 3).ToString("F3");
			return ret;
		}
	}
}
