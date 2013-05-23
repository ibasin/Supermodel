using System;
using System.Text;

//using LAShared;


using BrowserEmulator;

namespace BrowserEmulator {
	public class BrowserEmulatorParser : ParseHTML {
		public BrowserEmulatorParser(string page)
			: base() {
			this.Source = page;
		}

		public AttributeList LookForForm(string formName, string formActionStartsWith) {
			//find correct <form>
			AttributeList tag;
			while (true) {
				tag = LookForTag("form");

				string name;
				if (tag["name"] == null) name = "";
				else name = tag["name"].Value;

				string action;
				if (tag["action"] == null) action = "";
				else action = tag["action"].Value;

				if ((name.ToLower() == formName.ToLower()) && ((action.ToLower().StartsWith(formActionStartsWith.ToLower()) == true) || (formActionStartsWith.ToLower().StartsWith(action.ToLower()) == true))) break;
			}
			return tag;
		}
		public AttributeList LookForTag(string tagName) {
			try {
				while (true) {
					char ch = this.Parse();
					if (ch == 0) {
						AttributeList tag = this.GetTag();
						if (tag.Name.ToLower() == tagName.ToLower()) return tag;
					}
				}
			}
			catch (IndexOutOfRangeException) {
				throw new EOFException("Unexpected EOF where <" + tagName + "> is expected");
			}
		}
		public AttributeList LookForTagXML(string tagName, string parentEndTag) {
			try {
				while (true) {
					char ch = this.Parse();
					if (ch == 0) {
						AttributeList tag = this.GetTag();
						if (tag.Name.ToLower() == tagName.ToLower()) return tag;
						if (tag.Name.ToLower() == parentEndTag.ToLower()) throw new EOFException("Unexpected <" + parentEndTag + "> where <" + tagName + "> is expected");
					}
				}
			}
			catch (IndexOutOfRangeException) {
				throw new EOFException("Unexpected EOF where <" + tagName + "> is expected");
			}
		}

		public AttributeList LookFor2Tags(string tag1Name, string tag2Name) {
			try {
				while (true) {
					char ch = this.Parse();
					if (ch == 0) {
						AttributeList tag = this.GetTag();
						if ((tag.Name.ToLower() == tag1Name.ToLower()) || (tag.Name.ToLower() == tag2Name.ToLower())) return tag;
					}
				}
			}
			catch (IndexOutOfRangeException) {
				throw new EOFException("Unexpected EOF where <" + tag1Name + "> or <" + tag2Name + "> is expected");
			}
		}
		public AttributeList LookFor3Tags(string tag1Name, string tag2Name, string tag3Name) {
			try {
				while (true) {
					char ch = this.Parse();
					if (ch == 0) {
						AttributeList tag = this.GetTag();
						if ((tag.Name.ToLower() == tag1Name.ToLower()) ||
							(tag.Name.ToLower() == tag2Name.ToLower()) ||
							(tag.Name.ToLower() == tag3Name.ToLower()))
							return tag;
					}
				}
			}
			catch (IndexOutOfRangeException) {
				throw new EOFException("Unexpected EOF where <" + tag1Name + "> or <" + tag2Name + "> or <" + tag3Name + "> is expected");
			}
		}
		public AttributeList LookFor4Tags(string tag1Name, string tag2Name, string tag3Name, string tag4Name) {
			try {
				while (true) {
					char ch = this.Parse();
					if (ch == 0) {
						AttributeList tag = this.GetTag();
						if ((tag.Name.ToLower() == tag1Name.ToLower()) ||
							(tag.Name.ToLower() == tag2Name.ToLower()) ||
							(tag.Name.ToLower() == tag3Name.ToLower()) ||
							(tag.Name.ToLower() == tag4Name.ToLower()))
							return tag;
					}
				}
			}
			catch (IndexOutOfRangeException) {
				throw new EOFException("Unexpected EOF where <" + tag1Name + "> or <" + tag2Name + "> or <" + tag3Name + "> or <" + tag4Name + "> is expected");
			}
		}
		public AttributeList LookFor5Tags(string tag1Name, string tag2Name, string tag3Name, string tag4Name, string tag5Name) {
			try {
				while (true) {
					char ch = this.Parse();
					if (ch == 0) {
						AttributeList tag = this.GetTag();
						if ((tag.Name.ToLower() == tag1Name.ToLower()) ||
							(tag.Name.ToLower() == tag2Name.ToLower()) ||
							(tag.Name.ToLower() == tag3Name.ToLower()) ||
							(tag.Name.ToLower() == tag4Name.ToLower()) ||
							(tag.Name.ToLower() == tag5Name.ToLower()))
							return tag;
					}
				}
			}
			catch (IndexOutOfRangeException) {
				throw new EOFException("Unexpected EOF where <" + tag1Name + "> or <" + tag2Name + "> or <" + tag3Name + "> or <" + tag4Name + "> or <" + tag5Name + "> is expected");
			}
		}
		public void ProcessTextToTag(string expectedText, string tagName) {
			string text = GetTextToTag(tagName);
			if (text != expectedText) throw new BrowserEmulatorException("'" + text + "' while expecting '" + expectedText + "' before <" + tagName + ">");
		}
		public AttributeList ProcessTag(string tagName) {
			try {
				while (true) {
					char ch = this.Parse();
					if (ch == 0) {
						AttributeList tag = this.GetTag();
						if (tag.Name.ToLower() == tagName.ToLower()) return tag;
						else throw new BrowserEmulatorException("<" + tag.Name + "> where <" + tagName + "> is expected");
					}
				}
			}
			catch (IndexOutOfRangeException) {
				throw new EOFException("Unexpected EOF where <" + tagName + "> is expected");
			}
		}
		public AttributeList PeekNextTag() {
			int tmpIdx = m_idx;
			try {
				while (true) {
					char ch = this.Parse();
					if (ch == 0) return this.GetTag();
				}
			}
			catch (IndexOutOfRangeException) {
				throw new EOFException("Unexpected EOF where an HTML tag is expected");
			}
			finally {
				m_idx = tmpIdx;
			}
		}
		public AttributeList GetNextTag() {
			try {
				while (true) {
					char ch = this.Parse();
					if (ch == 0) return this.GetTag();
				}
			}
			catch (IndexOutOfRangeException) {
				throw new EOFException("Unexpected EOF where an HTML tag is expected");
			}
		}
		public string GetTextToNextTag(out AttributeList tag) {
			StringBuilder text = new StringBuilder();
			try {
				while (true) {
					char ch = this.Parse();
					if (ch == 0) {
						tag = this.GetTag();
						break;
					} else {
						text.Append(ch);
					}
				}
			}
			catch (IndexOutOfRangeException) {
				throw new EOFException("Unexpected EOF where an HTML tag is expected");
			}

			return text.ToString().Trim();
		}

		public string GetTextAndTagsToTag(string tagName) {
			int initPosition = GetParserPoint();
			LookForTag(tagName);
			int tagPosition = GetParserPoint();
			string textAndTags = m_source.Substring(initPosition, tagPosition - initPosition);
			string lowTextAndTags = textAndTags.ToLower();
			int closingTagIdx = lowTextAndTags.IndexOf("<" + tagName);
			string textAndTagsMinusEndTag = textAndTags.Substring(0, closingTagIdx);
			return textAndTagsMinusEndTag;
		}

		public string GetTextToTag(string tagName) {
			StringBuilder text = new StringBuilder();
			try {
				while (true) {
					char ch = this.Parse();
					if (ch == 0) {
						AttributeList tag = this.GetTag();
						if (tag.Name == tagName) break;
						else throw new BrowserEmulatorException("<" + tag.Name + "> where <" + tagName + "> is expected");
					} else {
						text.Append(ch);
					}
				}
			}
			catch (IndexOutOfRangeException) {
				throw new EOFException("Unexpected EOF where " + tagName + " is expected");
			}

			return text.ToString().Trim();
		}

		public int FindText(string text) {
			return FindText(text, false);
		}

		public int FindText(string text, bool ignoreCase) {
			if (ignoreCase) {
				return m_source.ToLower().IndexOf(text.ToLower(), m_idx);
			}

			return m_source.IndexOf(text, m_idx);
		}

		public void FastForwardToText(string text) {
			int idx = FindText(text);
			if (idx == -1) throw new EOFException("Unexpected EOF while fast forwarding to '" + text + "' text is expected");
			SetParserPoint(idx);
		}
		public int _2TextsExistsAhead(string text1, string text2) {
			int idx1 = m_source.IndexOf(text1, m_idx);
			int idx2 = m_source.IndexOf(text2, m_idx);
			if ((idx1 == -1) && (idx2 == -1)) return 0;
			if (idx1 == -1) return 2;
			if (idx2 == -1) return 1;
			if (idx1 <= idx2) return 1;
			if (idx2 <= idx1) return 2;
			throw new BrowserEmulatorException("Error 3732185: execution should never get here");
		}
		public bool TextExistsAhead(string text) {
			int idx = m_source.IndexOf(text, m_idx);
			if (idx != -1) return true;
			return false;
		}

		public AttributeList LookForXMLClosingTag(AttributeList tag) {
			if (tag["/"] != null) return tag;
			else return LookForTag("/" + tag.Name);
		}
		public int GetParserPoint() {
			return m_idx;
		}

		public void SetParserPoint(int parserPoint) {
			if (parserPoint < 0) throw new BrowserEmulatorException("Parser point must be geater than 0, while it is " + parserPoint);
			m_idx = parserPoint;
		}
	}
}
