# COMInterop
嘗試使用COM Interop的技術讓.net也能使用只有釋出C++介面的COM

------

第一次用PINVOKE包裝COM，好不容易總算有個模樣，所以趁這個機會紀錄一下。

如有不好的地方請大力鞭。



## 程式碼目錄

- ./Projects/TSFInterop: 用COMInterop把TSF包裝成.net可用的程式庫。

- ./Projects/WpfApp1: 使用TSFInterop.dll的範例。



## 延伸閱讀

[Text Services Framework](https://msdn.microsoft.com/zh-tw/library/windows/desktop/ms629032(v=vs.85).aspx)

[TSF API](https://msdn.microsoft.com/zh-tw/library/windows/desktop/ms538984(v=vs.85).aspx)

[NyaRuRu Blog](http://nyaruru.hatenablog.com/entry/20070309/p1)

https://github.com/NyaRuRu/TSF-TypeLib