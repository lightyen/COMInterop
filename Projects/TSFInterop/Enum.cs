namespace System.Windows.TextServices {

    [Flags]
    public enum LOCALE_NAME {
        DisplayName = 0x00000002,
        EnglishDisplayName = 0x00000072,
        NativeDisplayName = 0x00000073,

        LanguageName = 0x0000006f,
        EnglishLanguageName = 0x00001001,
        NativeLanguageName = 0x00000004,

        CountryName = 0x00000006,
        EnglishCountryName = 0x00001002,
        NativeCountryName = 0x00000008,
    }

    [Flags]
    public enum SortID {
        Default = 0,

        Invariant = 1,

        JapaneseXJIS = 0,
        JapaneseUnicode = 1,
        JapaneseRadicalStroke = 4,

        ChineseBIG5 = 0,

        ChinesePRCP = 0,
        ChineseUnicode = 1,
        ChinesePRC = 2,
        ChineseBopomofo = 3,
        ChineseRadicalStroke = 4,

        KoreanKSC = 0,
        KoreanUnicode = 1,

        GermanPhoneBook = 1,

        HungarianDefault = 0,
        HungarianTechnical = 1,

        GeorgianTraditional = 0,
        GeorgianModern = 1,
    }

}
