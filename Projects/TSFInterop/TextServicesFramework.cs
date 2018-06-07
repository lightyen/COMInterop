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
        int Register([In] ref Guid clsid);
        [PreserveSig]
        int Unregister([In] ref Guid clsid);
        [PreserveSig]
        int AddLanguageProfile([In] ref Guid clsid, [In] LANGID langid, [In] ref Guid guidProfile, [In] [MarshalAs(UnmanagedType.LPWStr)] string pchDesc, UInt32 cchDesc, [In] [MarshalAs(UnmanagedType.LPWStr)] string pchIconFile, [In] UInt32 cchFile, [In] UInt32 uIconIndex);
        [PreserveSig]
        int RemoveLanguageProfile([In] ref Guid rclsid, [In] LANGID langid, [In] ref Guid guidProfile);
        [PreserveSig]
        int EnumInputProcessorInfo([Out] out IEnumGUID ppEnum);
        [PreserveSig]
        int GetDefaultLanguageProfile([In] LANGID langid, [In] ref Guid catid, [Out] out Guid clsid, [Out] out Guid profile);
        [PreserveSig]
        int SetDefaultLanguageProfile([In] LANGID langid, [In] ref Guid catid, [In] ref Guid pguidProfile);
        [PreserveSig]
        int ActivateLanguageProfile([In] ref Guid rclsid, [In] LANGID langid, [In] ref Guid guidProfiles);
        [PreserveSig]
        int GetActiveLanguageProfile([In] ref Guid rclsid, [Out] out LANGID plangid, [Out] out Guid pguidProfile);
        [PreserveSig]
        int GetLanguageProfileDescription([In] ref Guid clsid, [In] LANGID langid, [In] ref Guid guidProfile, [Out, MarshalAs(UnmanagedType.BStr)] out string pbstrProfile);
        [PreserveSig]
        int GetCurrentLanguage([Out] out LANGID langid);
        /// <summary>
        /// 切換語言
        /// </summary>
        [PreserveSig]
        int ChangeCurrentLanguage([In] LANGID langid);
        /// <summary>
        /// 獲取所有可取得的語言
        /// </summary>
        [PreserveSig]
        int GetLanguageList([Out] out IntPtr ppLangId, [Out] out UInt32 count);
        /// <summary>
        /// 列舉在這個LANGID底下的輸入法
        /// </summary>
        [PreserveSig]
        int EnumLanguageProfiles([In] LANGID langid, [Out] out IEnumTfLanguageProfiles ppEnum);
        /// <summary>
        /// 啟用輸入法
        /// </summary>
        [PreserveSig]
        int EnableLanguageProfile([In] ref Guid rclsid, [In] LANGID langid, [In] ref Guid guidProfile, [In, MarshalAs(UnmanagedType.Bool)] bool fEnable);
        /// <summary>
        /// 查詢這個輸入法是否啟用
        /// </summary>
        [PreserveSig]
        int IsEnabledLanguageProfile([In] ref Guid rclsid, [In] LANGID langid, [In] ref Guid guidProfile, [Out, MarshalAs(UnmanagedType.Bool)] out bool pfEnable);
        /// <summary>
        /// 啟用或停用輸入法
        /// </summary>
        [PreserveSig]
        int EnableLanguageProfileByDefault([In] ref Guid rclsid, [In] LANGID langid, [In] ref Guid guidProfile, [In, MarshalAs(UnmanagedType.Bool)] bool fEnable);
        [PreserveSig]
        int SubstituteKeyboardLayout([In] ref Guid rclsid, [In] UInt16 langid, [In] ref Guid guidProfile, [In] IntPtr hKL);
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

        // 不知道幹嘛用的,先放著,
        // 可能GetDefaultLanguageProfile()會用到
        public static readonly Guid GUID_TFCAT_CATEGORY_OF_TIP = new Guid("534c48c1-0607-4098-a521-4fc899c73e90");
        public static readonly Guid GUID_TFCAT_TIP_KEYBOARD = new Guid("34745c63-b2f0-4784-8b67-5e12c8701a31");
        public static readonly Guid GUID_TFCAT_TIP_SPEECH = new Guid("b5a73cd1-8355-426b-a161-259808f26b14");
        public static readonly Guid GUID_TFCAT_TIP_HANDWRITING = new Guid("246ecb87-c2f2-4abe-905b-c8b38add2c43");
        public static readonly Guid GUID_TFCAT_TIPCAP_SECUREMODE = new Guid("49d2f9ce-1f5e-11d7-a6d3-00065b84435c");
        public static readonly Guid GUID_TFCAT_TIPCAP_UIELEMENTENABLED = new Guid("49d2f9cf-1f5e-11d7-a6d3-00065b84435c");
        public static readonly Guid GUID_TFCAT_TIPCAP_INPUTMODECOMPARTMENT = new Guid("ccf05dd7-4a87-11d7-a6e2-00065b84435c");
        public static readonly Guid GUID_TFCAT_TIPCAP_COMLESS = new Guid("364215d9-75bc-11d7-a6ef-00065b84435c");
        public static readonly Guid GUID_TFCAT_TIPCAP_WOW16 = new Guid("364215da-75bc-11d7-a6ef-00065b84435c");
        public static readonly Guid GUID_TFCAT_TIPCAP_IMMERSIVESUPPORT = new Guid("13A016DF-560B-46CD-947A-4C3AF1E0E35D");
        public static readonly Guid GUID_TFCAT_TIPCAP_SYSTRAYSUPPORT = new Guid("13A016DF-560B-46CD-947A-4C3AF1E0E35D");
        public static readonly Guid GUID_TFCAT_PROP_AUDIODATA = new Guid("9b7be3a9-e8ab-4d47-a8fe-254fa423436d");
        public static readonly Guid GUID_TFCAT_PROP_INKDATA = new Guid("7c6a82ae-b0d7-4f14-a745-14f28b009d61");
        public static readonly Guid GUID_TFCAT_PROPSTYLE_CUSTOM = new Guid("25504FB4-7BAB-4BC1-9C69-CF81890F0EF5");
        public static readonly Guid GUID_TFCAT_PROPSTYLE_STATIC = new Guid("565fb8d8-6bd4-4ca1-b223-0f2ccb8f4f96");
        public static readonly Guid GUID_TFCAT_PROPSTYLE_STATICCOMPACT = new Guid("85f9794b-4d19-40d8-8864-4e747371a66d");
        public static readonly Guid GUID_TFCAT_DISPLAYATTRIBUTEPROVIDER = new Guid("046b8c80-1647-40f7-9b21-b93b81aabc1b");
        public static readonly Guid GUID_TFCAT_DISPLAYATTRIBUTEPROPERTY = new Guid("b95f181b-ea4c-4af1-8056-7c321abbb091");
    }
}
