

EXEC [usp_MonthEndRun] 'SIXGAMING','REVENUE','2025-01-01','2025-03-01'
EXEC [usp_MonthEndRun] 'SIXGAMING','ADJUSTMENT','2025-01-01','2025-03-01'
EXEC [usp_MonthEndRun] 'SIXGAMING','ALLOCATION','2025-01-01','2025-03-01'
EXEC [usp_MonthEndRun] 'SIXGAMING','CASHIN','2025-01-01','2025-03-01'
EXEC [usp_MonthEndRun] 'SIXGAMING','CASHINTXFEE','2025-01-01','2025-03-01'
EXEC [usp_MonthEndRun] 'SIXGAMING','PURCHASE','2025-01-01','2025-03-01'

EXEC dbManta_Serving_UAT..[usp_MonthEndRun_Processor] 'SIXGAMING','JUMPMAN_DIGIFEEDPURCHASES','2025-01-01','2025-03-01'
EXEC dbManta_Serving_UAT..[usp_MonthEndRun_Processor] 'SIXGAMING','Jumpman_Ledgersales','2025-01-01','2025-03-01'
EXEC dbManta_Serving_UAT..[usp_MonthEndRun_Processor] 'SIXGAMING','Jumpman_LedgerFEE','2025-01-01','2025-03-01'
EXEC dbManta_Serving_UAT..[usp_MonthEndRun_Processor] 'SIXGAMING','Jumpman_LedgerREFUNDS','2025-01-01','2025-03-01'

/*

select * from dbManta_Serving_UAT..Jumpman_Adjustment_Batch_Tracking WHERE OperatorGroup = 'SIXGAMING' and [PostingDate] >= '2025-02-01' and [PostingDate] < '2025-03-01'
select * from dbManta_Serving_UAT..Jumpman_Allocation_Batch_Tracking WHERE OperatorGroup = 'SIXGAMING' and [PostingDate] >= '2025-02-01' and [PostingDate] < '2025-03-01'
select * from dbManta_Serving_UAT..Jumpman_Cashin_Batch_Tracking WHERE OperatorGroup = 'SIXGAMING' and [PostingDate] >= '2025-02-01' and [PostingDate] < '2025-03-01'
select * from dbManta_Serving_UAT..Jumpman_CashinTxFee_Batch_Tracking WHERE OperatorGroup = 'SIXGAMING' and [PostingDate] >= '2025-02-01' and [PostingDate] < '2025-03-01'
select * from dbManta_Serving_UAT..Jumpman_Purchase_Batch_Tracking WHERE OperatorGroup = 'SIXGAMING' and [PostingDate] >= '2025-02-01' and [PostingDate] < '2025-03-01'
select * from dbManta_Serving_UAT..Jumpman_Revenue_Batch_Tracking WHERE OperatorGroup = 'SIXGAMING' and [PostingDate] >= '2025-02-01' and [PostingDate] < '2025-03-01'
select * FROM dbManta_Serving_UAT..Jumpman_DigifeedPurchases_Batch_Tracking WHERE OperatorGroup = 'SIXGAMING' and [PostingDate] >= '2025-02-01' and [PostingDate] < '2025-03-01'
select * FROM dbManta_Serving_UAT..Jumpman_LedgerFEE_Batch_Tracking WHERE OperatorGroup = 'SIXGAMING' and [PostingDate] >= '2025-02-01' and [PostingDate] < '2025-03-01'
select * FROM dbManta_Serving_UAT..Jumpman_LedgerSALES_Batch_Tracking WHERE OperatorGroup = 'SIXGAMING' and [PostingDate] >= '2025-02-01' and [PostingDate] < '2025-03-01'
select * FROM dbManta_Serving_UAT..Jumpman_LedgerREFUNDS_Batch_Tracking WHERE OperatorGroup = 'SIXGAMING' and [PostingDate] >= '2025-02-01' and [PostingDate] < '2025-03-01'

SELECT  [Document No], sum ( Amount ), OperatorGroup FROM dbManta_Serving_UAT..jumpman_revenue_log WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-02-01' and [Posting Date] < '2025-03-01' GROUP BY [Document No], OperatorGroup ORDER BY 1
SELECT SUM ( Amount ), OperatorGroup FROM dbManta_Serving_UAT..jumpman_CASHINTXFEE_log WHERE OperatorGroup = 'SIXGAMING'  and [Posting Date] >= '2025-02-01' and [Posting Date] < '2025-03-01' GROUP BY  OperatorGroup
SELECT SUM ( Amount ), OperatorGroup FROM dbManta_Serving_UAT..jumpman_CASHIN_log WHERE OperatorGroup = 'SIXGAMING'  and [Posting Date] >= '2025-02-01' and [Posting Date] < '2025-03-01' GROUP BY  OperatorGroup
SELECT SUM ( Amount ), OperatorGroup FROM dbManta_Serving_UAT..jumpman_Adjustment_log WHERE OperatorGroup = 'SIXGAMING'  and [Posting Date] >= '2025-02-01' and [Posting Date] < '2025-03-01' GROUP BY  OperatorGroup
SELECT [posting Date],LEFT ( AccNumberDesc, 2 ), SUM ( Amount ), OperatorGroup FROM dbManta_Serving_UAT..jumpman_ALLOCATION_log WHERE OperatorGroup = 'SIXGAMING'  and [Posting Date] >= '2025-02-01' and [Posting Date] < '2025-03-01' GROUP BY [posting Date], LEFT ( AccNumberDesc, 2 ),OperatorGroup
SELECT [posting Date], SUM ( Amount ), OperatorGroup FROM dbManta_Serving_UAT..jumpman_PURCHASE_log WHERE OperatorGroup = 'SIXGAMING'  and [Posting Date] >= '2025-02-01' and [Posting Date] < '2025-03-01' GROUP BY [posting Date], OperatorGroup
SELECT ImporttYPE, SUM ( Amount )  FROM dbManta_Serving_UAT..Jumpman_DigifeedPurchases_log WHERE OperatorGroup = 'SIXGAMING'  and [Posting Date] >= '2025-02-01' and [Posting Date] < '2025-03-01' GROUP BY ImporttYPE
SELECT ImporttYPE, SUM ( Amount ) from dbManta_Serving_UAT..Jumpman_Ledgersales_log WHERE OperatorGroup = 'SIXGAMING'  and [Posting Date] >= '2025-02-01' and [Posting Date] < '2025-03-01' GROUP BY ImporttYPE
SELECT ImporttYPE, SUM ( Amount ) from dbManta_Serving_UAT..Jumpman_Ledgerfee_log WHERE OperatorGroup = 'SIXGAMING'  and [Posting Date] >= '2025-02-01' and [Posting Date] < '2025-03-01' GROUP BY ImporttYPE
SELECT ImporttYPE, SUM ( Amount ) from dbManta_Serving_UAT..Jumpman_Ledgerrefunds_log WHERE OperatorGroup = 'SIXGAMING'  and [Posting Date] >= '2025-02-01' and [Posting Date] < '2025-03-01' GROUP BY ImporttYPE


delete from ADJUSTMENT_Snapshot  WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
delete from ALLOCATION_Snapshot  WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
delete from Cashin_Snapshot  WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
delete from CashinTxFee_Snapshot  WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
delete from REVENUE_Snapshot  WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
delete from PURCHASE_Snapshot  WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'

delete from dbManta_Serving_UAT..Jumpman_Adjustment_Batch_Tracking  WHERE OperatorGroup = 'SIXGAMING' and [PostingDate] >= '2025-01-01' and [PostingDate] < '2025-03-01'
delete from dbManta_Serving_UAT..Jumpman_Adjustment_ERRORLOG  WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
delete from dbManta_Serving_UAT..Jumpman_Adjustment_LOG  WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'

delete from dbManta_Serving_UAT..Jumpman_Allocation_Batch_Tracking  WHERE OperatorGroup = 'SIXGAMING' and [PostingDate] >= '2025-01-01' and [PostingDate] < '2025-03-01'
delete from dbManta_Serving_UAT..Jumpman_Allocation_ERRORLOG  WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
delete from dbManta_Serving_UAT..Jumpman_Allocation_Log  WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'

delete from dbManta_Serving_UAT..Jumpman_Cashin_Batch_Tracking  WHERE OperatorGroup = 'SIXGAMING' and [PostingDate] >= '2025-01-01' and [PostingDate] < '2025-03-01'
delete from dbManta_Serving_UAT..Jumpman_CASHIN_ERRORLOG  WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
delete from dbManta_Serving_UAT..Jumpman_CASHIN_LOG  WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'

delete from dbManta_Serving_UAT..Jumpman_CashinTxFee_Batch_Tracking  WHERE OperatorGroup = 'SIXGAMING' and [PostingDate] >= '2025-01-01' and [PostingDate] < '2025-03-01'
delete from dbManta_Serving_UAT..Jumpman_CASHINTXFEE_ERRORLOG  WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
delete from dbManta_Serving_UAT..Jumpman_CASHINTXFEE_LOG  WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'

delete from dbManta_Serving_UAT..Jumpman_Purchase_Batch_Tracking  WHERE OperatorGroup = 'SIXGAMING' and [PostingDate] >= '2025-01-01' and [PostingDate] < '2025-03-01'
delete from dbManta_Serving_UAT..Jumpman_PURCHASE_ERRORLOG  WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
delete from dbManta_Serving_UAT..Jumpman_PURCHASE_LOG  WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'

delete from dbManta_Serving_UAT..Jumpman_Revenue_Batch_Tracking  WHERE OperatorGroup = 'SIXGAMING' and [PostingDate] >= '2025-01-01' and [PostingDate] < '2025-03-01'
delete from dbManta_Serving_UAT..Jumpman_REVENUE_ERRORLOG  WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
delete from dbManta_Serving_UAT..Jumpman_REVENUE_LOG  WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'

DELETE FROM dbManta_Serving_UAT..Jumpman_DigifeedPurchases_Batch_Tracking  WHERE OperatorGroup = 'SIXGAMING' and [PostingDate] >= '2025-01-01' and [PostingDate] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_DigifeedPurchases_ERRORLOG  WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_DigifeedPurchases_Errorlog_Stage  WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_DigifeedPurchases_LOG  WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_DigifeedPurchases_LOG_Stage  WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_DigifeedPurchases_Snapshot  WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_DigifeedPurchases_Snapshot_Stage  WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_DigifeedPurchases_Stage  WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'

DELETE FROM dbManta_Serving_UAT..Jumpman_LEDGERFEE_Batch_Tracking  WHERE OperatorGroup = 'SIXGAMING' and [PostingDate] >= '2025-01-01' and [PostingDate] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_LEDGERFEE_ERRORLOG WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_LEDGERFEE_Errorlog_Stage WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_LEDGERFEE_LOG WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_LEDGERFEE_LOG_Stage WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_LEDGERFEE_Snapshot WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_LEDGERFEE_Snapshot_Stage WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_LEDGERFEE_Stage WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'

DELETE FROM dbManta_Serving_UAT..Jumpman_LEDGERSALES_Batch_Tracking  WHERE OperatorGroup = 'SIXGAMING' and [PostingDate] >= '2025-01-01' and [PostingDate] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_LEDGERSALES_ERRORLOG WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_LEDGERSALES_Errorlog_Stage WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_LEDGERSALES_LOG WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_LEDGERSALES_LOG_Stage WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_LEDGERSALES_Snapshot WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_LEDGERSALES_Snapshot_Stage WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_LEDGERSALES_Stage WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'

DELETE FROM dbManta_Serving_UAT..Jumpman_LEDGERREFUNDS_Batch_Tracking  WHERE OperatorGroup = 'SIXGAMING' and [PostingDate] >= '2025-01-01' and [PostingDate] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_LEDGERREFUNDS_ERRORLOG WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_LEDGERREFUNDS_Errorlog_Stage WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_LEDGERREFUNDS_LOG WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_LEDGERREFUNDS_LOG_Stage WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_LEDGERREFUNDS_Snapshot WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_LEDGERREFUNDS_Snapshot_Stage WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'
DELETE FROM dbManta_Serving_UAT..Jumpman_LEDGERREFUNDS_Stage WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-01-01' and [Posting Date] < '2025-03-01'


*/

 


 select *from Revenue_Summary_hourly_1_0 where Datekey = 20250101

 select *from brand_1_1

 DELETE FROM PSP_OrderID_Lookup where DateKey >= 20250101 AND DateKey < 20250501