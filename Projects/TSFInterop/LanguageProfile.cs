using System.Runtime.InteropServices;

namespace System.Windows.TextServices {
    [StructLayout(LayoutKind.Sequential)]
    public class LanguageProfile {
        public Guid clsid;
        public UInt16 langid;
        public Guid catid;
        [MarshalAs(UnmanagedType.Bool)]
        public bool fActive;
        public Guid guidProfile;
    }
}
