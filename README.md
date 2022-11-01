# ASP_NET_MVC_5x

網際網路購物網站
ASP.NET MVC version 4.7 shopping cart 
說明請看(作品TheOcean.pdf)

*網站開發使用技術與工具*
架構：C#、ASP.NET MVC
前端：HTML、CSS、JavaScript、jQuery
資料庫運用：SQL Server
開發工具：Visual Studio 版本控制：Azure DevOps


*功能*
首頁 / 會員登入與註冊 / 產品細節 / 產品詳細頁 / 購物車
管理員登入 / 後台管理CRUD

*說明*
1. 登入運用Authorize Attribute 指定控制器所有的動作方法必須
通過授權才能執行，區分會員與管理員權限。
2. 使用SHA256 雜湊碼用於登入密碼，提高安全性。
