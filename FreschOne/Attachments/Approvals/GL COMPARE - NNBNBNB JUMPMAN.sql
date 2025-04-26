

drop table Purchase2025GLMissings_SIX

;WITH March2025 AS (
    SELECT 
        CONVERT(DATETIME, [Posting Date], 103) AS [PostingDate],
		[Source Code] AS ImportType,
        [Bank Account No_] AS [AccountNo],
		[Document No_] AS [DocumentNo],
        Description,
        ABS ( SUM ( Amount )) AS Amount,
        [Bal_ Account No_] AS [BalAccountNo],
        [Shortcut Dimension 1 Code] AS [Product D1],
        [Shortcut Dimension 4 Code] AS D4 --SELECT *  
    FROM select_player_balance_six_march2025 WITH (NOLOCK)
    WHERE [Source Code] IN (  'PTSDEP','CTACASHIN' )  -- AND [Posting Date] = '26/03/2025'
	GROUP BY 
	 CONVERT(DATETIME, [Posting Date], 103) ,
	 [Source Code],
        [Bank Account No_],
        [Document No_] ,
        Description,
        [Bal_ Account No_],
        [Shortcut Dimension 1 Code] ,
        [Shortcut Dimension 4 Code] 
),
History AS (
    SELECT [Posting Date] AS [PostingDate],
		ImportType,
        [Account No] AS [AccountNo],
        [Document No] AS [DocumentNo],
        Description,
        ABS ( SUM ( Amount )) AS Amount,
        [Bal_ Account No_] AS [BalAccountNo],
        [Product D1],
        D4 
    FROM dbGroupTwoB..[The Six Gaming Limited$BINAVImport History] WITH (NOLOCK)
    WHERE 
        [Posting Date] >= '2025-03-01'  AND [Posting Date] < '2025-04-01'
        AND OperatorGroup = 'SIXGAMING'
        AND Exceptions = ''
        AND ImportType IN  ( 'PTSDEPOSITS', 'CTACASHIN' ) 
		GROUP BY [Posting Date] ,
		ImportType,
        [Account No] ,
        [Document No] ,
        Description,
        [Bal_ Account No_] ,
        [Product D1],
        D4 
),
  FullAmounts AS ( SELECT 
        CONVERT(DATETIME, [Posting Date], 103) AS [PostingDate],
		[Source Code] AS ImportType,
        [Bank Account No_] AS [AccountNo],
        [Document No_] AS [DocumentNo],
        Description,
        ABS ( SUM ( Amount )) AS Amount,
        [Bal_ Account No_] AS [BalAccountNo],
        [Shortcut Dimension 1 Code] AS [Product D1],
        [Shortcut Dimension 4 Code] AS D4
    FROM select_player_balance_six_march2025 WITH (NOLOCK)
    WHERE [Source Code] IN (  'PTSDEP','CTACASHIN' )  -- AND [Posting Date] = '26/03/2025'
	GROUP BY 
	 CONVERT(DATETIME, [Posting Date], 103) ,
	 [Source Code] ,
        [Bank Account No_],
        [Document No_] ,
        Description,
        [Bal_ Account No_],
        [Shortcut Dimension 1 Code] ,
        [Shortcut Dimension 4 Code] 
	) 

SELECT distinct
    m.PostingDate,
	m.ImportType,
	m.DocumentNo,
    m.Description,
    m.Amount,
    m.BalAccountNo,
    m.[Product D1],
    m.D4,
	--h.[Line No],
	h.PostingDate as PostingDate_1,
	h.ImportType as ImportType_1,
	h.DocumentNo as DocumentNo_1 ,
    h.Description as Description_1,
    h.Amount as Amount_1,
    h.BalAccountNo as BalAccountNo_1,
    h.[Product D1] as [Product D1_1],
    h.D4 as D4_1 ,
	--
	f.PostingDate as PostingDate_f,
	f.ImportType as ImportType_f,
	f.DocumentNo as DocumentNo_f ,
    f.Description as Description_f,
    f.Amount as Amount_f,
    f.BalAccountNo as BalAccountNo_f,
    f.[Product D1] as [Product D1_f],
    f.D4 as D4_f  
	INTO dbo.Purchase2025GLMissings_SIX    
FROM March2025  m
LEFT JOIN  History h
    ON  m.DocumentNo collate SQL_Latin1_General_CP1_CI_AS = h.DocumentNo collate SQL_Latin1_General_CP1_CI_AS 
	and left ( m.ImportType, 3 ) collate SQL_Latin1_General_CP1_CI_AS = left ( h.ImportType, 3 ) collate SQL_Latin1_General_CP1_CI_AS
   -- AND h.AccountNo collate SQL_Latin1_General_CP1_CI_AS = m.AccountNo collate SQL_Latin1_General_CP1_CI_AS
    --AND h.PostingDate = m.PostingDate
LEFT JOIN FullAmounts f ON m.DocumentNo  = f.DocumentNo AND LEFT ( m.ImportType, 3 ) collate SQL_Latin1_General_CP1_CI_AS = left ( f.ImportType, 3 ) collate SQL_Latin1_General_CP1_CI_AS
WHERE CAST ( ABS ( m.Amount ) AS DECIMAL (18, 2 )) != CAST ( ABS ( h.Amount ) AS DECIMAL(18,2 ))

SELECT PostingDate, ImportType, SUM ( Amount ) , sum ( amount_1 ), SUM ( Amount_f ) FROM Purchase2025GLMissings_SIX --where DocumentNo_f = '106828760' --where PostingDate_1 = '2025-03-31' 
Group by  PostingDate,ImportType

SELECT *FROM Purchase2025GLMissings_SIX


--AND m.DocumentNo = '106820113'

--AND m.DocumentNo = '106823848'
--117454791




--SELECT *FROM Purchase2025GLMissings_extra  WHERE  DocumentNo_1 = '59-105494489'



/*
SELECT [Posting Date],
       [Document No],
       [Account Type],
       [Account No],
       Description,
       [Currency Code],
       Amount as Amount,
       [Bal_ Account Type],
       [Bal_ Account No_],
       [Product D1],
       [Region D2],
       D3 ,
       D4,
       D5,
       D6,
       D7,
       D8,
       [External Doc No_],
       GETDATE() AS ImportDate,
       OperatorGroup,
       ImportType,
       Processed = 0,
	   ReasonCode,
	   D9,
	   D10,
	   D11,
	   D12,
	   D13,
	   D14,
	   D15,
	   D16,
	   'DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD' AS BatchGuid,
	   'DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD' AS RowGuid,
	   [Document Date] 
	   FROM dbGroupTwoB..[Jumpman Limited$BINAVImport History] WITH (NOLOCK)
	   WHERE [Posting Date] >= '2025-03-01' AND [Posting Date] < '2025-04-01'
	   AND ImportType = 'PTSDEPOSITS'
	   AND Exceptions = ''
	  -- AND [Import Date] >= '2025-04-01'
	   AND [Document No] IN ( SELECT [DocumentNo] FROM dbFinanceReporting.dbo.Purchase2025GLMissings with (nolock) ) 



	   --select *from Purchase2025GLMissings ORDER BY 1
	   */