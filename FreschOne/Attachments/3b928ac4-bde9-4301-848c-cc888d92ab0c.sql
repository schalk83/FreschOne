
USE dbJumpman
GO
ALTER PROCEDURE [dbo].[usp_FULLMonthEndRun]                
(                                                            
  @dPostingDate                  DATE              = NULL                                                            
 ,@dPostingDateEnd               DATE                    = NULL                                                    
 )           
AS                                                           
                                                           
 DECLARE @ImportType    VARCHAR(MAX)                                                          
 ,@SQL      VARCHAR(MAX)                                                          
 ,@OperatorGroup    VARCHAR(MAX)             
         
            
 IF @dPostingDate IS NULL           
 BEGIN           
                                        
  IF DAY ( GETDATE()) < 7                                                          
  BEGIN                                                           
                                                          
     SET @dPostingDate =  DATEADD ( DAY, - DAY ( GETDATE() - 1 ) , GETDATE() )  - 7                       
     SET @dPostingDateEnd = GETDATE()                                 
                             
  END                                                           
  ELSE                                                           
  BEGIN                                                          
   SET @dPostingDate = DATEADD ( DAY, - DAY ( GETDATE() - 1 ) , GETDATE() )                                                           
   SET @dPostingDateEnd = GETDATE()            
  END                                   
                  
END             
                 
CREATE TABLE #OperatorGroups                                                          
( OperatorGroup VARCHAR(MAX))                                                           
                                                          
INSERT INTO #OperatorGroups                                                           
VALUES --( 'JUMPMAN' )              
   ( 'SIXGAMING' )                                
                                                          
CREATE TABLE #ImportTypes                                                           
( ImportType VARCHAR(MAX))                                                           
                                                                                 
                                                              
WHILE ( SELECT COUNT(*) from #OperatorGroups ) > 0                                                           
BEGIN                                                           
                                                          
 SELECT @OperatorGroup = OperatorGroup FROM #OperatorGroups                                                          
 PRINT (@OperatorGroup)                                                          
 DELETE FROM #OperatorGroups where OperatorGroup = @OperatorGroup                                                          
                                                          
 INSERT INTO #ImportTypes                                                           
 VALUES ( 'REVENUE' )                                                         
    --( 'PURCHASE' ),                                                    
    --( 'CASHIN' ),                                                       
    --( 'CASHINTXFEE' ),                                                      
    --( 'ADJUSTMENT' ),                                                         
    --( 'ALLOCATION' )                                                        
                                                         
 WHILE ( SELECT COUNT(*) from #ImportTypes ) > 0                                                           
 BEGIN                                                           
                                                          
  SELECT @ImportType = ImportType FROM #ImportTypes       
  PRINT (@ImportType)                                                          
  DELETE FROM #ImportTypes where ImportType = @ImportType                                 
                                                            
  EXEC dbo.usp_MonthEndRun @OperatorGroup, @ImportType,@dPostingDate,@dPostingDateEnd                                  
  PRINT ( @OperatorGroup + ' - ' +  @ImportType  + @OperatorGroup + ' - ' + @ImportType+ ' - ' + CAST ( @dPostingDate AS VARCHAR )+ ' - ' + CAST ( @dPostingDateEnd AS VARCHAR) )                                                         
                                                     
                                            
 END                                                                               
                                                          
END                           
      
GO
USE DBMANTA_SERVING_UAT 
GO

delete fROM [Jumpman_Revenue_Log] WHERE [Posting Date] >= '2025-01-01' AND  [Posting Date] < '2025-02-01' AND OperatorGroup = 'SixGaming'
delete fROM [Jumpman_Revenue_batch_tracking] WHERE [PostingDate] >= '2025-01-01' AND [PostingDate] < '2025-02-01' AND OperatorGroup = 'SixGaming'
delete From dbJumpman..Revenue_Snapshot WHERE [Posting Date] >= '2025-01-01'  AND [Posting Date] < '2025-02-01' AND OperatorGroup = 'SixGaming'

--delete fROM [Jumpman_Cashin_Log] WHERE [Posting Date] >= '2025-01-01' AND  [Posting Date] < '2025-02-01' AND OperatorGroup = 'SixGaming'
--delete fROM [Jumpman_Cashin_batch_tracking] WHERE [PostingDate] >= '2025-01-01' AND [PostingDate] < '2025-02-01' AND OperatorGroup = 'SixGaming'
--delete From dbJumpman..Cashin_Snapshot WHERE [Posting Date] >= '2025-01-01'  AND [Posting Date] < '2025-02-01' AND OperatorGroup = 'SixGaming'

--delete fROM [Jumpman_CashinTxFee_Log] WHERE [Posting Date] >= '2025-01-01' AND  [Posting Date] < '2025-02-01' AND OperatorGroup = 'SixGaming'
--delete fROM [Jumpman_CashinTxFee_batch_tracking] WHERE [PostingDate] >= '2025-01-01' AND [PostingDate] < '2025-02-01' AND OperatorGroup = 'SixGaming'
--delete From dbJumpman..CashinTxFee_Snapshot WHERE [Posting Date] >= '2025-01-01'  AND [Posting Date] < '2025-02-01' AND OperatorGroup = 'SixGaming'

--delete fROM [Jumpman_Adjustment_Log] WHERE [Posting Date] >= '2025-01-01' AND  [Posting Date] < '2025-02-01' AND OperatorGroup = 'SixGaming'
--delete fROM [Jumpman_Adjustment_batch_tracking] WHERE [PostingDate] >= '2025-01-01' AND [PostingDate] < '2025-02-01' AND OperatorGroup = 'SixGaming'
--delete From dbJumpman..Adjustment_Snapshot WHERE [Posting Date] >= '2025-01-01'  AND [Posting Date] < '2025-02-01' AND OperatorGroup = 'SixGaming'


--delete fROM [Jumpman_Purchase_Log] WHERE [Posting Date] >= '2025-01-01' AND  [Posting Date] < '2025-02-01' AND OperatorGroup = 'SixGaming'
--delete fROM [Jumpman_Purchase_batch_tracking] WHERE [PostingDate] >= '2025-01-01' AND [PostingDate] < '2025-02-01' AND OperatorGroup = 'SixGaming'
--delete From dbJumpman..Purchase_Snapshot WHERE [Posting Date] >= '2025-01-01'  AND [Posting Date] < '2025-02-01' AND OperatorGroup = 'SixGaming'

EXEC dbJumpman..[usp_FULLMonthEndRun] '2025-01-01','2025-02-01'      

select *from [Jumpman_Revenue_Log] WHERE [Posting Date] >= '2025-01-01' AND  [Posting Date] < '2025-02-01' AND OperatorGroup = 'SixGaming'
select *from [Jumpman_Revenue_batch_tracking] WHERE [PostingDate] >= '2025-01-01' AND [PostingDate] < '2025-02-01' AND OperatorGroup = 'SixGaming'
select *from dbJumpman..Revenue_Snapshot WHERE [Posting Date] >= '2025-01-01'  AND [Posting Date] < '2025-02-01' AND OperatorGroup = 'SixGaming'

--select *from [Jumpman_Cashin_Log] WHERE [Posting Date] >= '2025-01-01' AND  [Posting Date] < '2025-02-01' AND OperatorGroup = 'SixGaming'
--select *from [Jumpman_Cashin_batch_tracking] WHERE [PostingDate] >= '2025-01-01' AND [PostingDate] < '2025-02-01' AND OperatorGroup = 'SixGaming'
--select *from dbJumpman..Cashin_Snapshot WHERE [Posting Date] >= '2025-01-01'  AND [Posting Date] < '2025-02-01' AND OperatorGroup = 'SixGaming'

--select *from [Jumpman_CashinTxFee_Log] WHERE [Posting Date] >= '2025-01-01' AND  [Posting Date] < '2025-02-01' AND OperatorGroup = 'SixGaming'
--select *from [Jumpman_CashinTxFee_batch_tracking] WHERE [PostingDate] >= '2025-01-01' AND [PostingDate] < '2025-02-01' AND OperatorGroup = 'SixGaming'
--select *from dbJumpman..CashinTxFee_Snapshot WHERE [Posting Date] >= '2025-01-01'  AND [Posting Date] < '2025-02-01' AND OperatorGroup = 'SixGaming'

--select *from [Jumpman_Adjustment_Log] WHERE [Posting Date] >= '2025-01-01' AND  [Posting Date] < '2025-02-01' AND OperatorGroup = 'SixGaming'
--select *from [Jumpman_Adjustment_batch_tracking] WHERE [PostingDate] >= '2025-01-01' AND [PostingDate] < '2025-02-01' AND OperatorGroup = 'SixGaming'
--select *from dbJumpman..Adjustment_Snapshot WHERE [Posting Date] >= '2025-01-01'  AND [Posting Date] < '2025-02-01' AND OperatorGroup = 'SixGaming'

--select *from [Jumpman_Purchase_Log] WHERE [Posting Date] >= '2025-01-01' AND  [Posting Date] < '2025-02-01' AND OperatorGroup = 'SixGaming'
--select *from [Jumpman_Purchase_batch_tracking] WHERE [PostingDate] >= '2025-01-01' AND [PostingDate] < '2025-02-01' AND OperatorGroup = 'SixGaming'
--select *from dbJumpman..Purchase_Snapshot WHERE [Posting Date] >= '2025-01-01'  AND [Posting Date] < '2025-02-01' AND OperatorGroup = 'SixGaming'


/*

select *from [Jumpman_Adjustment_batch_tracking] WHERE [PostingDate] >= '2025-01-01' AND [PostingDate] < '2025-02-01' AND OperatorGroup = 'SixGaming' ORDER BY PostingDate
select *from [Jumpman_Allocation_batch_tracking] WHERE [PostingDate] >= '2025-01-01' AND [PostingDate] < '2025-02-01' AND OperatorGroup = 'SixGaming' ORDER BY PostingDate
select *from [Jumpman_CashinTxFee_batch_tracking] WHERE [PostingDate] >= '2025-01-01' AND [PostingDate] < '2025-02-01' AND OperatorGroup = 'SixGaming' ORDER BY PostingDate
select *from [Jumpman_Cashin_batch_tracking] WHERE [PostingDate] >= '2025-01-01' AND [PostingDate] < '2025-02-01' AND OperatorGroup = 'SixGaming' ORDER BY PostingDate
select *from [Jumpman_Revenue_batch_tracking] WHERE [PostingDate] >= '2025-01-01' AND [PostingDate] < '2025-02-01' AND OperatorGroup = 'SixGaming' ORDER BY PostingDate
select *from [Jumpman_Purchase_batch_tracking] WHERE [PostingDate] >= '2025-01-01' AND [PostingDate] < '2025-02-01' AND OperatorGroup = 'SixGaming' ORDER BY PostingDate


SELECT *FROM BCErrorLog WHERE OperatorGroup = 'sixgaming' and errormessage not like '%THE SIX GAMING%'
ORDER BY ImportDate Desc

--update OperatorGroupsToProcess set TargetCompany = 'The Six Gaming Ltd' where OperatorGroup = 'SIXGAMING'
*/

--select distinct D16 from Jumpman_REVENUE_LOG