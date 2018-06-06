using System;
using System.Text;
using System.Runtime.InteropServices;

using LANGID = System.UInt16;
using LCID = System.UInt32;

using System.Windows.TextServices;

namespace Interop {

    /// <summary>
    /// C++ 的 ITfInputProcessorProfiles 介面
    /// </summary>
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("1F02B6C5-7842-4EE6-8A0B-9A24183A95CA")]
    internal interface ITfInputProcessorProfiles {
        // 方法名稱無所謂,按照順序排就行,但為了方便使用還是參考"msctf.h"的方法以及參數的名稱.
        [PreserveSig]
        int Register(ref Guid clsid);
        [PreserveSig]
        int Unregister(ref Guid clsid);
        [PreserveSig]
        int AddLanguageProfile(ref Guid clsid, LANGID langid, ref Guid guidProfile, [MarshalAs(UnmanagedType.LPWStr)] string pchDesc, UInt32 cchDesc, [MarshalAs(UnmanagedType.LPWStr)] string pchIconFile, UInt32 cchFile, UInt32 uIconIndex);
        [PreserveSig]
        int RemoveLanguageProfile(ref Guid rclsid, LANGID langid, ref Guid guidProfile);
        [PreserveSig]
        int EnumInputProcessorInfo(out IEnumGUID ppEnum);
        [PreserveSig]
        int GetDefaultLanguageProfile(LANGID langid, ref Guid catid, out Guid clsid, out Guid profile);
        [PreserveSig]
        int SetDefaultLanguageProfile(LANGID langid, ref Guid catid, ref Guid pguidProfile);
        [PreserveSig]
        int ActivateLanguageProfile(ref Guid rclsid, LANGID langid, ref Guid guidProfiles);
        [PreserveSig]
        int GetActiveLanguageProfile(ref Guid rclsid, out LANGID plangid, out Guid pguidProfile);
        [PreserveSig]
        int GetLanguageProfileDescription(ref Guid clsid, LANGID langid, ref Guid guidProfile, out IntPtr pbstrProfile);
        [PreserveSig]
        int GetCurrentLanguage(out LANGID langid);
        [PreserveSig]
        int ChangeCurrentLanguage(LANGID langid);
        [PreserveSig]
        int GetLanguageList(out IntPtr ppLangId, out UInt32 count);
        /// <summary>
        /// 列舉在這個LANGID底下的輸入法
        /// </summary>
        [PreserveSig]
        int EnumLanguageProfiles(LANGID langid, out IEnumTfLanguageProfiles ppEnum);
        [PreserveSig]
        int EnableLanguageProfile(ref Guid rclsid, LANGID langid, ref Guid guidProfile, [MarshalAs(UnmanagedType.Bool)] bool fEnable);
        /// <summary>
        /// 查詢這個輸入法是否啟用
        /// </summary>
        [PreserveSig]
        int IsEnabledLanguageProfile(ref Guid rclsid, LANGID langid, ref Guid guidProfile, [Out, MarshalAs(UnmanagedType.Bool)] out bool pfEnable);
        [PreserveSig]
        int EnableLanguageProfileByDefault(ref Guid rclsid, LANGID langid, ref Guid guidProfile, [MarshalAs(UnmanagedType.Bool)] bool fEnable);
        [PreserveSig]
        int SubstituteKeyboardLayout(ref Guid rclsid, UInt16 langid, ref Guid guidProfile, IntPtr hKL);
    }

    /// <summary>
    /// C++ 的 IEnumTfLanguageProfiles 介面
    /// </summary>
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("3D61BF11-AC5F-42C8-A4CB-931BCC28C744")]
    internal interface IEnumTfLanguageProfiles {
        [PreserveSig]
        int Clone(out IEnumTfLanguageProfiles ppEnum);
        /// <summary>
        /// 下一個
        /// </summary>
        [PreserveSig]
        int Next(UInt32 count, IntPtr profiles, out UInt32 pcFetch);
        [PreserveSig]
        int Reset();
        [PreserveSig]
        int Skip(UInt32 count);
    }

    /// <summary>
    /// C++ 的 IEnumGUID 介面
    /// </summary>
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("0002E000-0000-0000-C000-000000000046")]
    internal interface IEnumGUID {
        /// <summary>
        /// 下一個
        /// </summary>
        [PreserveSig]
        int Next(UInt32 count, IntPtr rgelt, out UInt32 pceltFetched);
        [PreserveSig]
        int Skip(UInt32 count);
        [PreserveSig]
        int Reset();
        [PreserveSig]
        int Clone(out IEnumGUID ppEnum);
    }

    /// <summary>
    /// 一些將會引用到的 Win32 API
    /// </summary>
    internal static class NativeAPI {

        [DllImport("msctf.dll", EntryPoint = "TF_CreateInputProcessorProfiles")]
        public static extern int CreateInputProcessorProfiles(out ITfInputProcessorProfiles profiles);

        [DllImport("kernel32.dll", EntryPoint = "GetLocaleInfoEx", CharSet = CharSet.Unicode)]
        public static extern int GetLocaleInfoEx([MarshalAs(UnmanagedType.LPWStr)] string lpLocaleName, LOCALE_NAME localeType, StringBuilder lcData, int cchData);

        public static int GetLocaleInfoEx(string lpLocaleName, LOCALE_NAME localeType, out string name) {
            const int maxLength = 260;
            StringBuilder sb = new StringBuilder(maxLength);
            int result = GetLocaleInfoEx(lpLocaleName, localeType, sb, maxLength);
            name = sb.ToString();
            return result;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetLocaleInfo(LCID lcid, LOCALE_NAME localeType, StringBuilder lcData, int cchData);

        public static int GetLocaleInfo(LCID lcid, LOCALE_NAME localeType, out string name) {
            const int maxLength = 260;
            StringBuilder sb = new StringBuilder(maxLength);
            int result = GetLocaleInfo(lcid, localeType, sb, maxLength);
            name = sb.ToString();
            return result;
        }

        private static LCID MakeLocaleID(LANGID langid, SortID srtid) {
            return ((uint)srtid << 16) | langid;
        }

        public static int GetLocaleInfo(LANGID langid, LOCALE_NAME localeType, out string name) {
            const int maxLength = 260;
            StringBuilder sb = new StringBuilder(maxLength);
            int result = GetLocaleInfo(MakeLocaleID(langid, SortID.Default), localeType, out name);
            return result;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetKeyboardLayout(int threadid);

        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();

        public static readonly Guid GUID_TFCAT_TIP_KEYBOARD;

        static NativeAPI() {
            // 不知道幹嘛用的,先放著
            GUID_TFCAT_TIP_KEYBOARD = new Guid("34745C63-B2F0-4784-8B67-5E12C8701A31");
        }
    }
}
