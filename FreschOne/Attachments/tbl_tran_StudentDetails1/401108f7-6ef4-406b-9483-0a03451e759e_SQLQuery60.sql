                
ALTER PROCEDURE [dbo].[usp_MonthEndRun_Processor]                 
(  @vOperatorGroup NVARCHAR(20), @vTableName NVARCHAR(128), @dPostingDate DATE , @dPostingDateEnd DATE )                 
AS                
                
 DECLARE @vSQL VARCHAR(MAX)                 

 SET @vTableName = @vTableName         
      
 WHILE @dPostingDate < @dPostingDateEnd                
 BEGIN                
                
  EXEC [Jumpman_Digifeed_Generic_Export] @vOperatorGroup  = @vOperatorGroup,                
        @vTableName  = @vTableName,                
        @dPostingDate      = @dPostingDate,                
        @dPostingDateOverride    = NULL,                
        @fMaxDeleteThreshold    = '1',                
        @iMinDeleteThresholdRecords   = 100000,                
        @bTestMode       = NULL,                
        @vMandatoryFields     = N'[Account Type],[Description],[Currency Code],[Bal_ Account Type],[Region D2],[Document No],[Account No]'                  
    PRINT ( cast ( @dPostingDate as varchar ) + ' - '+ @vOperatorGroup + ' - ' + @vTableName )            
   SET @dPostingDate = DATEADD(DAY, 1, @dPostingDate)                
            
                
 END            

  GO

  EXEC [usp_MonthEndRun_Processor] 'SIXGAMING','Jumpman_DigifeedPurchases','2025-01-01','2025-02-01'
  EXEC [usp_MonthEndRun_Processor] 'SIXGAMING','Jumpman_Settlements','2025-01-01','2025-02-01'
  EXEC [usp_MonthEndRun_Processor] 'SIXGAMING','Jumpman_COF','2025-01-01','2025-02-01' 
  EXEC [usp_MonthEndRun_Processor] 'SIXGAMING','Jumpman_Ledgers','2025-01-01','2025-02-01'
