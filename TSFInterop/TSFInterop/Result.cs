﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Interop {
    internal class Result {
        public uint HResult { get; private set; }

        public Result(int hResult) {
            HResult = (uint)(hResult);
        }

        public bool OK {
            get {
                return HResult == 0;
            }
        }

        public void CheckError() {
            if (HResult != 0) {
                COMException ex = null;
                switch (HResult) {
                    case 0x80004004:
                        ex = new COMException("E_ABORT: Operation aborted");
                        break;
                    case 0x80070005:
                        ex = new COMException("E_ACCESSDENIED: General access denied error");
                        break;
                    case 0x80004005:
                        ex = new COMException("E_FAIL: 	Unspecified failure");
                        break;
                    case 0x80070006:
                        ex = new COMException("E_HANDLE: Handle that is not valid");
                        break;
                    case 0x80070057:
                        ex = new COMException("E_INVALIDARG: One or more arguments are not valid");
                        break;
                    case 0x80004002:
                        ex = new COMException("E_NOINTERFACE: No such interface supported");
                        break;
                    case 0x80004001:
                        ex = new COMException("E_NOTIMPL: Not implemented");
                        break;
                    case 0x8007000E:
                        ex = new COMException("E_OUTOFMEMORY: Failed to allocate necessary memory");
                        break;
                    case 0x80004003:
                        ex = new COMException("E_POINTER: 	Pointer that is not valid");
                        break;
                    case 0x8000FFFF:
                        ex = new COMException("E_UNEXPECTED: Unexpected failure");
                        break;
                }
                if (ex != null) throw ex;
            }
        }

        public static implicit operator int (Result r) {
            return ((int)r.HResult);
        }

        public static implicit operator Result(int hr) {
            return new Result(hr);
        }
    }
}