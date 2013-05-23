using System;

namespace BrowserEmulator {
	public class BrowserEmulatorException : ApplicationException {
		public BrowserEmulatorException(String message) : base(message) { }
	}
	//--------------------------------------------------------------------
	public class EOFException : BrowserEmulatorException {
		public EOFException(String message) : base(message) { }
	}
	//--------------------------------------------------------------------
	public class FieldDoesNotExistException : BrowserEmulatorException {
		public FieldDoesNotExistException(String message) : base(message) { }
	}
}