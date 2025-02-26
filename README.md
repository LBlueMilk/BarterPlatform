# 112-2 行動商務系統設計與開發 - 易物平台

## 問題處理指南

### 1. 資料表匯出遇到逾時問題
在匯出資料表時，可能會遇到 SQL Server 執行逾時的情況，可透過增加等待時間來解決。

#### 解決方法：
使用以下 SQL 指令提高 SQL Server 等待時間（單位：秒）：
```sql
EXEC sp_configure 'remote query timeout', 600;
RECONFIGURE;
```
此指令將等待時間設定為 **10 分鐘 (600 秒)**，可根據需求自行調整。

#### 確認設定是否生效：
```sql
EXEC sp_configure 'remote query timeout';
```
預期輸出結果：
```
name                   minimum    maximum    config_value    run_value
remote query timeout      0        2147483647      600          600
```
- **config_value = 600**：表示設定成功
- **run_value = 600**：表示設定已生效

#### 恢復預設值：
若使用結束後想恢復為預設 30 秒，請執行：
```sql
EXEC sp_configure 'remote query timeout', 30;
RECONFIGURE;
```

---

### 2. 資料表匯出遇到 VARBINARY 資料類型錯誤（圖片儲存）
當資料表中包含 `VARBINARY(MAX)` 類型的欄位（如圖片），直接匯出可能會發生錯誤。

#### 解決方法：
將 `VARBINARY(MAX)` 轉換為 **Base64 字串** 進行匯出，並在目標資料庫將其轉回 `VARBINARY(MAX)`。

#### 完整流程：
1. **在 A 資料庫**，將 `VARBINARY(MAX)` 轉換為 Base64 字串
2. **匯出 Base64 資料至 B 資料庫**
3. **在 B 資料庫**，將 Base64 字串轉回 `VARBINARY(MAX)`

#### SQL 範例：
**步驟 1：在 A 資料庫，轉換 VARBINARY(MAX) 為 Base64**
```sql
SELECT
    MemID, PersonalName, Nickname, Gender, BirthDate,
    PostalCode, AdminRegion_AdmID, District_DisID, OtherAddress,
    Username, Password, Email, Phone, NationalID,
    CAST(N'' AS XML).value('xs:base64Binary(xs:hexBinary(sql:column("IDImage")))', 'NVARCHAR(MAX)') AS IDImage_Base64,
    CreationTime, DeletionTime, LastLogin, Status
FROM A.dbo.Member;
```
> **注意**：這裡只有 `IDImage` 欄位需要轉為 Base64，其餘欄位保持原狀。

**步驟 2：將轉換後的資料匯出至 B 資料庫**

**步驟 3：在 B 資料庫，將 Base64 轉回 VARBINARY(MAX)**
```sql
UPDATE B.dbo.Member
SET IDImage = CAST(N'' AS XML).value('xs:hexBinary(xs:base64Binary(sql:column("IDImage_Base64")))', 'VARBINARY(MAX)');
```
> **這樣 `IDImage` 欄位就會成功轉回 `VARBINARY(MAX)`，恢復為原始的二進位圖片格式。**

