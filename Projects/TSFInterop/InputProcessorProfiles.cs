using System.Collections.Generic;
using System.Runtime.InteropServices;
using Interop;

using LANGID = System.UInt16;

// https://msdn.microsoft.com/zh-tw/library/windows/desktop/ms538984(v=vs.85).aspx

namespace System.Windows.TextServices {
    /// <summary>
    /// ITfInputProcessorProfiles介面的實作類別
    /// </summary>
    public class InputProcessorProfiles : IDisposable {

        // TSF 的 COM
        private ITfInputProcessorProfiles inputProcessorProfiles;

        /// <summary>
        /// 初始化ITfInputProcessorProfiles介面
        /// </summary>
        public InputProcessorProfiles() {
            Result result = NativeAPI.CreateInputProcessorProfiles(out var profiles);
            if (result.OK) {
                inputProcessorProfiles = profiles;
            } else {
                inputProcessorProfiles = null;
            }
            result.CheckError();
        }

        /// <summary>
        /// 列舉所有安裝的語言
        /// </summary>
        public LANGID[] LanguageIDs {
            get {
                List<LANGID> list = new List<LANGID>();

                Result result = inputProcessorProfiles.GetLanguageList(out IntPtr p, out uint count);

                if (!result.OK) return list.ToArray();

                for (int i = 0; i < count; i++) {
                    var lang = Marshal.ReadInt16(p, i * Marshal.SizeOf<LANGID>());
                    list.Add((LANGID)lang);
                }
                Marshal.FreeCoTaskMem(p);
                return list.ToArray();
            }
        }

        /// <summary>
        /// 獲取可讀的語言名稱
        /// </summary>
        public string GetLanguageName(LANGID langid) {
            Result result = NativeAPI.GetLocaleInfo(langid, LOCALE_NAME.DisplayName, out string name);
            return name;
        }

        /// <summary>
        /// 列舉所有語言名稱
        /// </summary>
        public string[] Languages {
            get {
                List<string> list = new List<string>();
                var ids = LanguageIDs;
                foreach (var id in ids) {
                    list.Add(GetLanguageName(id));
                }
                return list.ToArray();
            }
        }

        /// <summary>
        /// 預設語言
        /// </summary>
        public LANGID DefaultLanguageID {
            get {
                var ids = LanguageIDs;
                if (ids.Length > 0) return ids[0];
                else return 0;
            }
        }

        /// <summary>
        /// 當前語言ID
        /// </summary>
        public LANGID CurrentLanguageID {
            get {
                Result result = inputProcessorProfiles.GetCurrentLanguage(out var current);
                result.CheckError();
                return current;
            }
            
        }

        /// <summary>
        /// 預設語言名稱
        /// </summary>
        public string DefaultLanguage {
            get {
                Result result = NativeAPI.GetLocaleInfo(DefaultLanguageID, LOCALE_NAME.DisplayName, out string name);
                result.CheckError();
                return name;
            }
        }

        /// <summary>
        /// 獲取該語言的輸入法
        /// </summary>
        /// <param name="langid">語言</param>
        public LanguageProfile[] GetLanguageProfiles(LANGID langid) {

            List<LanguageProfile> list = new List<LanguageProfile>();

            Result result = inputProcessorProfiles.EnumLanguageProfiles(langid, out IEnumTfLanguageProfiles enumerator);

            if (!result.OK) return list.ToArray();

            IntPtr p = Marshal.AllocCoTaskMem(Marshal.SizeOf<LanguageProfile>());
            while ((result = enumerator.Next(1, p, out var fetched)).HResult == 0) {
                LanguageProfile profile = new LanguageProfile();
                Marshal.PtrToStructure(p, profile);
                list.Add(profile);
            }
            Marshal.FreeCoTaskMem(p);

            return list.ToArray();
        }

        public Guid[] GetInputProcessorInfo() {

            List<Guid> list = new List<Guid>();

            Result result = inputProcessorProfiles.EnumInputProcessorInfo(out IEnumGUID enumerator);

            if (!result.OK) return list.ToArray();

            IntPtr p = Marshal.AllocCoTaskMem(Marshal.SizeOf<Guid>());
            while ((result = enumerator.Next(1, p, out var fetched)).HResult == 0) {
                Guid guid = (Guid)Marshal.PtrToStructure(p, typeof(Guid));
                list.Add(guid);
            }
            Marshal.FreeCoTaskMem(p);

            return list.ToArray();
        }

        /// <summary>
        /// 獲取輸入法名稱
        /// </summary>
        /// <param name="profile">輸入法</param>
        public string GetLanguageProfileDescription(LanguageProfile profile) {
            Result result = inputProcessorProfiles.GetLanguageProfileDescription(profile.clsid, profile.langid, profile.guidProfile, out string desc);
            result.CheckError();
            return desc;
        }

        /// <summary>
        /// 詢問該輸入法是否啟用
        /// </summary>
        /// <param name="profile">輸入法</param>
        public bool IsEnabledLanguageProfile(LanguageProfile profile) {
            inputProcessorProfiles.IsEnabledLanguageProfile(profile.clsid, profile.langid, profile.guidProfile, out bool enable);
            return enable;
        }

        /// <summary>
        /// 切換至輸入法 (需啟用)
        /// </summary>
        /// <param name="profile">輸入法</param>
        public void ActivateLanguageProfile(LanguageProfile profile) {

            inputProcessorProfiles.GetCurrentLanguage(out var current_langid);
            Result result = 0;
            if (current_langid != profile.langid) {
                // 先切換語言才能切換輸入法
                result = inputProcessorProfiles.ChangeCurrentLanguage(profile.langid);
                result.CheckError();
            }
            result = inputProcessorProfiles.ActivateLanguageProfile(profile.clsid, profile.langid, profile.guidProfile);
            result.CheckError();
        }

        /// <summary>
        /// 獲得當前輸入法名稱
        /// </summary>
        /// <returns></returns>
        public string GetCurrentLanguageProfileName() {
            LANGID current = CurrentLanguageID;
            var profiles = GetLanguageProfiles(current);
            if (profiles.Length > 0) {
                foreach (var profile in profiles) {
                    if (profile.fActive) {
                        return $"{GetLanguageName(current)} - {GetLanguageProfileDescription(profile)}";
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// 設定預設輸入法
        /// </summary>
        public void SetDefaultLanguageProfile(LanguageProfile profile) {
            Result result = inputProcessorProfiles.SetDefaultLanguageProfile(profile.langid, profile.clsid, profile.guidProfile);
            result.CheckError();
        }

        /// <summary>
        /// finalizer
        /// </summary>
        ~InputProcessorProfiles() {
            Dispose(false);
        }

        /// <summary>
        /// 釋放COM
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 釋放COM介面
        /// </summary>
        /// <param name="disposing">是否主動釋放</param>
        private void Dispose(bool disposing) {

            if (disposing) {
                // 如果有其他unmanaged東西則在此釋放
            }

            // 如果有其他Textbox之類的, 那麼referenceCount可能會大於0
            int referenceCount = Marshal.ReleaseComObject(inputProcessorProfiles);
            inputProcessorProfiles = null;
        }
    }

}
