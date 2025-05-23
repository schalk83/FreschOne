USE [dbJumpman]
GO

/****** Object:  StoredProcedure [dbo].[Partition_Maintain]    Script Date: 2024/12/12 08:50:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Partition_Maintain]  
AS  
    BEGIN  
  
    /* *****************************************************************************  
    ** -----------------------------------------------------------------------------  
    ** AUTHOR: Jean Van Iddekinge  
    ** DATE: 2020-06-20  
    ** PURPOSE: This procedure adds and deletes new partitions to the partition schemes  
 **          PartitionFunction_Date and PartitionFunction_DateKey  
    ** ==================================================================================  
    ** When           | Jira Tkt / Ammendment    | Who            |  
    ** ---------------|-------------------------------------+---------------------------+  
    ** ---------------|-------------------------------------+---------------------------+  
 **  
    ** =============================================================================  
    ** PROCESS ORDER (TABLE OF CONTENTS):  
    ** =================================  
    ** 1) DECLARE VARIABLES, CREATE TEMP TABLES  
    ** 2) POPULATE VARIABLES TEMP TABLES   
 ** 3) ADD FUTURE PARTITIONS NEEDED  
 ** 4) ADD HISTORY PARTITIONS NEEDED  
 ** 5) DELETE FUTURE PARTITIONS NO LONGER NEEDED  
 ** 6) DELETE HISTORY PARTITIONS NO LONGER NEEDED  
    **  
    ** ADDITIONAL NOTES:  
    ** ================  
    ** The @dStartDate and @dEndDate can cover any range of time from a single day to 2 weeks etc.  
 **  
    ** PARAMETERS:  
    ** ================  
 ** @dStartDate - This determines the beginning date of the period to process  
 ** @dEndDate - This determines the end date of the period to process  
 **  
    ** =============================================================================  
    *******************************************************************************/  
  
    /* -----------------------------------------------------------------------------  
    ** 1) DECLARE VARIABLES, CREATE TEMP TABLES  
    ** -------------------------------------------------------------------------- */  
  
        SET NOCOUNT ON;  
  
        DECLARE   
        --  debugging and logging parameters  
        @bDebug             bit           = 1,   
        @vObjectName        nvarchar(256) = N'Partition_Maintain',   
        @vLogText           varchar(1000),   
        @vErrorMessage      nvarchar(max),   
        @dLogDateTime       datetime      = GETDATE(),   
        -- logic parameters  
        @iFutureWindowDays  int           = 14, -- default 2 weeks  
        @iHistoryWindowDays int           = 600, -- default 600 days   
        @sCurrentMaxDate    sql_variant,   
        @sCurrentMinDate    sql_variant,   
        @sCurrentMaxDateKey sql_variant,   
        @sCurrentMinDateKey sql_variant,   
        @dCurrentMaxDate    date,   
        @dCurrentMinDate    date,   
        @iCurrentMaxDateKey int,   
        @iCurrentMinDateKey int,   
        @dCurrentMaxDateKey date,   
        @dCurrentMinDateKey date,   
        @dNextDate          date,   
        @dPreviousDate      date,   
        @dFutureDate        date,   
        @dHistoryDate       date,   
        @dNextDateKey       date,   
        @iNextDateKey       int,   
        @dPreviousDateKey   date,   
        @iPreviousDateKey   int,   
        @dFutureDateKey     date,   
        @dHistoryDateKey    date;  
  
        BEGIN TRY  
  
            IF(@bDebug = 1)  
                BEGIN  
  
                    SELECT @vLogText = @vObjectName + ' ** 1) DECLARE VARIABLES, CREATE TEMP TABLES ';  
                    EXEC dbo.LogTransaction   
                         @vObjectName = @vObjectName,   
                         @vStepName = @vLogText,   
                         @dLogDateTime = @dLogDateTime OUTPUT;  
  
            END;  
  
  /* -----------------------------------------------------------------------------  
        ** 2) POPULATE VARIABLES TEMP TABLES   
        ** -------------------------------------------------------------------------- */  
  
            SELECT @sCurrentMaxDate = MAX(value),   
                   @sCurrentMinDate = MIN(Value)  
            FROM sys.partition_functions f WITH(NOLOCK)  
                 LEFT JOIN sys.partition_range_values g WITH(NOLOCK) ON g.function_id = f.function_id  
                   WHERE f.name = 'PartitionFunction_Date';  
            SELECT @dCurrentMaxDate = ISNULL(CAST(@sCurrentMaxDate AS date), GETDATE()),   
                   @dCurrentMinDate = ISNULL(CAST(@sCurrentMinDate AS date), GETDATE());  
  
            SELECT @sCurrentMaxDateKey = MAX(value),   
                   @sCurrentMinDateKey = MIN(Value)  
            FROM sys.partition_functions f WITH(NOLOCK)  
                 LEFT JOIN sys.partition_range_values g WITH(NOLOCK) ON g.function_id = f.function_id  
                   WHERE f.name = 'PartitionFunction_DateKey';  
            SELECT @iCurrentMaxDateKey = ISNULL(CAST(@sCurrentMaxDateKey AS int), -1),   
                   @iCurrentMinDateKey = ISNULL(CAST(@sCurrentMinDateKey AS int), -1);  
  
            SELECT @dCurrentMaxDateKey = LEFT(@iCurrentMaxDateKey, 4) + '-' + SUBSTRING(CAST(@iCurrentMaxDateKey AS nvarchar(8)), 5, 2) + '-' + RIGHT(@iCurrentMaxDateKey, 2);  
            SELECT @dCurrentMinDateKey = LEFT(@iCurrentMinDateKey, 4) + '-' + SUBSTRING(CAST(@iCurrentMinDateKey AS nvarchar(8)), 5, 2) + '-' + RIGHT(@iCurrentMinDateKey, 2);  
  
   SELECT @dFutureDate = DATEADD(day, @iFutureWindowDays, GETDATE());  
            SELECT @dHistoryDate = DATEADD(day, -@iHistoryWindowDays, GETDATE());  
            SELECT @dFutureDateKey = DATEADD(day, @iFutureWindowDays, GETDATE());  
            SELECT @dHistoryDateKey = DATEADD(day, -@iHistoryWindowDays, GETDATE());  
  
            IF(@bDebug = 1)  
                BEGIN  
  
                    SELECT @vLogText = @vObjectName + ' ** 2) POPULATE VARIABLES TEMP TABLES ';  
                    EXEC dbo.LogTransaction   
                         @vObjectName = @vObjectName,   
                         @vStepName = @vLogText,   
                         @dLogDateTime = @dLogDateTime OUTPUT;  
  
            END;  
  
    /* -----------------------------------------------------------------------------  
    ** 3) ADD FUTURE PARTITIONS NEEDED  
    ** -------------------------------------------------------------------------- */  
           
            -- date  
            WHILE(@dCurrentMaxDate < @dFutureDate)  
                BEGIN  
  
                    SELECT @dNextDate = DATEADD(day, 1, @dCurrentMaxDate);  
  
                    PRINT 'ADDING FUTURE PARTITION :' + CAST(@dNextDate AS nvarchar(400));  
  
                    ALTER PARTITION SCHEME PartitionScheme_Date NEXT USED [PRIMARY];  
                    ALTER PARTITION FUNCTION PartitionFunction_Date() SPLIT RANGE(@dNextDate);  
  
                    SELECT @sCurrentMaxDate = MAX(value)  
                    FROM sys.partition_functions f  
                         LEFT JOIN sys.partition_range_values g ON g.function_id = f.function_id  
                           WHERE f.name = 'PartitionFunction_Date';  
                    SELECT @dCurrentMaxDate = CAST(@sCurrentMaxDate AS date);  
  
                END;  
  
            -- integer  
            WHILE(@dCurrentMaxDateKey < @dFutureDateKey)  
                BEGIN  
  
                    SELECT @dNextDateKey = DATEADD(day, 1, @dCurrentMaxDateKey);  
                    SELECT @iNextDateKey = FORMAT(@dNextDateKey, 'yyyyMMdd');  
  
                    PRINT 'ADDING FUTURE PARTITION :' + CAST(@dNextDateKey AS nvarchar(400));  
  
                    ALTER PARTITION SCHEME PartitionScheme_DateKey NEXT USED [PRIMARY];  
                    ALTER PARTITION FUNCTION PartitionFunction_DateKey() SPLIT RANGE(@iNextDateKey);  
  
                    SELECT @sCurrentMaxDateKey = MAX(value)  
                    FROM sys.partition_functions f  
                         LEFT JOIN sys.partition_range_values g ON g.function_id = f.function_id  
                           WHERE f.name = 'PartitionFunction_DateKey';  
                    SELECT @iCurrentMaxDateKey = ISNULL(CAST(@sCurrentMaxDateKey AS int), -1);  
  
             SELECT @dCurrentMaxDateKey = LEFT(@iCurrentMaxDateKey, 4) + '-' + SUBSTRING(CAST(@iCurrentMaxDateKey AS nvarchar(8)), 5, 2) + '-' + RIGHT(@iCurrentMaxDateKey, 2);  
  
                END;  
  
  
            IF(@bDebug = 1)  
                BEGIN  
  
                    SELECT @vLogText = @vObjectName + ' ** 3) ADD FUTURE PARTITIONS NEEDED ';  
                    EXEC dbo.LogTransaction   
                         @vObjectName = @vObjectName,   
                         @vStepName = @vLogText,   
                         @dLogDateTime = @dLogDateTime OUTPUT;  
  
            END;  
  
    /* -----------------------------------------------------------------------------  
    ** 4) ADD HISTORY PARTITIONS NEEDED  
    ** -------------------------------------------------------------------------- */  
  
            -- date  
            WHILE(@dCurrentMinDate > @dHistoryDate)  
                BEGIN  
  
                    SELECT @dPreviousDate = DATEADD(day, -1, @dCurrentMinDate);  
  
                    PRINT 'ADDING HISTORY PARTITION :' + CAST(@dPreviousDate AS nvarchar(400));  
  
                    ALTER PARTITION SCHEME PartitionScheme_Date NEXT USED [PRIMARY];  
                    ALTER PARTITION FUNCTION PartitionFunction_Date() SPLIT RANGE(@dPreviousDate);  
  
                    SELECT @sCurrentMinDate = MIN(value)  
                    FROM sys.partition_functions f  
                         LEFT JOIN sys.partition_range_values g ON g.function_id = f.function_id  
                           WHERE f.name = 'PartitionFunction_Date';  
                    SELECT @dCurrentMinDate = CAST(@sCurrentMinDate AS date);  
  
                END;  
  
            -- integer  
            WHILE(@dCurrentMinDateKey > @dHistoryDateKey)  
                BEGIN  
  
                    SELECT @dPreviousDateKey = DATEADD(day, -1, @dCurrentMinDateKey);  
                    SELECT @iPreviousDateKey = FORMAT(@dPreviousDateKey, 'yyyyMMdd');  
  
                    PRINT 'ADDING HISTORY PARTITION :' + CAST(@dPreviousDateKey AS nvarchar(400));  
  
                    ALTER PARTITION SCHEME PartitionScheme_DateKey NEXT USED [PRIMARY];  
                    ALTER PARTITION FUNCTION PartitionFunction_DateKey() SPLIT RANGE(@iPreviousDateKey);  
  
                    SELECT @sCurrentMinDateKey = MIN(value)  
                    FROM sys.partition_functions f  
                         LEFT JOIN sys.partition_range_values g ON g.function_id = f.function_id  
                           WHERE f.name = 'PartitionFunction_DateKey';  
                    SELECT @iCurrentMinDateKey = ISNULL(CAST(@sCurrentMinDateKey AS int), -1);  
  
                    SELECT @dCurrentMinDateKey = LEFT(@iCurrentMinDateKey, 4) + '-' + SUBSTRING(CAST(@iCurrentMinDateKey AS nvarchar(8)), 5, 2) + '-' + RIGHT(@iCurrentMinDateKey, 2);  
  
                END;  
  
  
            IF(@bDebug = 1)  
                BEGIN  
  
                    SELECT @vLogText = @vObjectName + ' ** 4) ADD HISTORY PARTITIONS NEEDED ';  
                    EXEC dbo.LogTransaction   
                         @vObjectName = @vObjectName,   
                         @vStepName = @vLogText,   
                         @dLogDateTime = @dLogDateTime OUTPUT;  
  
            END;  
  
    /* -----------------------------------------------------------------------------  
    ** 5) DELETE FUTURE PARTITIONS NO LONGER NEEDED  
    ** -------------------------------------------------------------------------- */  
           
            -- date  
            WHILE(@dCurrentMaxDate > @dFutureDate)  
                BEGIN  
  
                    PRINT 'DELETING FUTURE PARTITION: ' + CAST(@dCurrentMaxDate AS nvarchar(400));  
                    ALTER PARTITION SCHEME PartitionScheme_Date NEXT USED [PRIMARY];  
                    ALTER PARTITION FUNCTION PartitionFunction_Date() MERGE RANGE(@dCurrentMaxDate);  
  
                    SELECT @sCurrentMaxDate = MAX(value)  
                    FROM sys.partition_functions f  
                         LEFT JOIN sys.partition_range_values g ON g.function_id = f.function_id  
                           WHERE f.name = 'PartitionFunction_Date';  
  
                    SELECT @dCurrentMaxDate = CAST(@sCurrentMaxDate AS date);  
  
                END;  
  
            -- integer  
            WHILE(@dCurrentMaxDateKey > @dFutureDateKey)  
                BEGIN  
  
                    PRINT 'DELETING FUTURE PARTITION: ' + CAST(@dCurrentMaxDateKey AS nvarchar(400));  
                    SELECT @iCurrentMaxDateKey = FORMAT(@dCurrentMaxDateKey, 'yyyyMMdd');  
  
                    ALTER PARTITION SCHEME PartitionScheme_DateKey NEXT USED [PRIMARY];  
                    ALTER PARTITION FUNCTION PartitionFunction_DateKey() MERGE RANGE(@iCurrentMaxDateKey);  
  
                    SELECT @sCurrentMaxDateKey = MAX(value)  
                    FROM sys.partition_functions f  
                         LEFT JOIN sys.partition_range_values g ON g.function_id = f.function_id  
                           WHERE f.name = 'PartitionFunction_DateKey';  
                    SELECT @iCurrentMaxDateKey = ISNULL(CAST(@sCurrentMaxDateKey AS int), -1);  
                    SELECT @dCurrentMaxDateKey = LEFT(@iCurrentMaxDateKey, 4) + '-' + SUBSTRING(CAST(@iCurrentMaxDateKey AS nvarchar(8)), 5, 2) + '-' + RIGHT(@iCurrentMaxDateKey, 2);  
  
                END;  
            IF(@bDebug = 1)  
                BEGIN  
  
                    SELECT @vLogText = @vObjectName + ' ** 5) DELETE FUTURE PARTITIONS NO LONGER NEEDED ';  
                    EXEC dbo.LogTransaction   
                         @vObjectName = @vObjectName,   
                         @vStepName = @vLogText,   
                         @dLogDateTime = @dLogDateTime OUTPUT;  
  
            END;  
  
    /* -----------------------------------------------------------------------------  
    ** 6) DELETE HISTORY PARTITIONS NO LONGER NEEDED  
    ** -------------------------------------------------------------------------- */  
  
            -- date  
            WHILE(@dCurrentMinDate < @dHistoryDate)  
                BEGIN  
  
                    PRINT 'DELETING HISTORY PARTITION: ' + CAST(@dCurrentMinDate AS nvarchar(400));  
                    ALTER PARTITION SCHEME PartitionScheme_Date NEXT USED [PRIMARY];  
                    ALTER PARTITION FUNCTION PartitionFunction_Date() MERGE RANGE(@dCurrentMinDate);  
  
                    SELECT @sCurrentMinDate = MIN(value)  
                    FROM sys.partition_functions f  
                         LEFT JOIN sys.partition_range_values g ON g.function_id = f.function_id  
                           WHERE f.name = 'PartitionFunction_Date';  
  
                    SELECT @dCurrentMinDate = CAST(@sCurrentMinDate AS date);  
  
                END;  
  
            -- integer  
            WHILE(@dCurrentMinDateKey < @dHistoryDateKey)  
                BEGIN  
  
                    PRINT 'DELETING HISTORY PARTITION: ' + CAST(@dCurrentMinDateKey AS nvarchar(400));  
                    SELECT @iCurrentMinDateKey = FORMAT(@dCurrentMinDateKey, 'yyyyMMdd');  
  
                    ALTER PARTITION SCHEME PartitionScheme_DateKey NEXT USED [PRIMARY];  
                    ALTER PARTITION FUNCTION PartitionFunction_DateKey() MERGE RANGE(@iCurrentMinDateKey);  
  
                    SELECT @sCurrentMinDateKey = MIN(value)  
                    FROM sys.partition_functions f  
                         LEFT JOIN sys.partition_range_values g ON g.function_id = f.function_id  
                           WHERE f.name = 'PartitionFunction_DateKey';  
                    SELECT @iCurrentMinDateKey = ISNULL(CAST(@sCurrentMinDateKey AS int), -1);  
                    SELECT @dCurrentMinDateKey = LEFT(@iCurrentMinDateKey, 4) + '-' + SUBSTRING(CAST(@iCurrentMinDateKey AS nvarchar(8)), 5, 2) + '-' + RIGHT(@iCurrentMinDateKey, 2);  
  
                END;  
  
            IF(@bDebug = 1)  
                BEGIN  
  
                    SELECT @vLogText = @vObjectName + ' ** 6) DELETE HISTORY PARTITIONS NO LONGER NEEDED ';  
EXEC dbo.LogTransaction   
                         @vObjectName = @vObjectName,   
                         @vStepName = @vLogText,   
                         @dLogDateTime = @dLogDateTime OUTPUT;  
  
            END;  
        END TRY  
        BEGIN CATCH  
  
            SET @vErrorMessage = ERROR_MESSAGE();  
  
            SELECT @vLogText = 'An Error was encountered while building up the data for the dbo.Revenue. Error Message: ' + @vErrorMessage + '. Error Line: ' + CAST(ERROR_LINE() AS nvarchar(10));  
            PRINT @vLogText;  
            RAISERROR('%s', 16, 1, @vErrorMessage);  
        END CATCH;  
  
    END;  
  
/* SANDBOX  
  
GO  
  
EXEC [dbo].[Partition_Maintain];  
  
*/  
  
GO


