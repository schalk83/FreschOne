--SELECT * FROM [Jumpman Limited$BINAVImport] WITH (NOLOCK ) WHERE ImportType = 'REVENUE' 
----AND LEFT ( D16 , CHARINDEX ( '_',D16) - 1 ) NOT IN ( SELECT uniqueid from dbFinanceReporting..[Jumpman_GamesMapping] ) 
--AND D16 = '6767_JMP'


--SELECT * from dbFinanceReporting..[Jumpman_GamesMapping] WHERE UniqueID = 6767


--SELECT COUNT(*) FROM [Jumpman Limited$BINAVImport] WITH (NOLOCK ) WHERE ImportType = 'REVENUE' 
--AND D16 NOT IN ( SELECT uniqueid from dbFinanceReporting..[Jumpman_GamesMapping] ) 

--SELECT  *
--FROM dbo.[Jumpman Limited$Dimension Value]
--WHERE [Dimension Code] = 'GAMENAME'
--AND [Map-to IC Dimension Value Code]  = ''


--SELECT  LEFT ( D16 , CHARINDEX ( '_',D16) - 1 ),* FROM [Jumpman Limited$BINAVImport History] WITH (NOLOCK ) WHERE [Posting Date] >='2025-03-01' AND  ImportType = 'REVENUE'  AND D16 != ''
--AND Exceptions = ''
--AND D16 NOT IN  ( 
--SELECT
--	[Lookup Value]
--FROM [dbGroupTwoB].dbo.[Jumpman Limited$Generic Lookups] GL WITH (NOLOCK)
--Where GL.[Lookup Type] = 'GAMENAME' AND RIGHT ( [Lookup Value], 3 ) in ( 'TSG','JMP')) --AND [Lookup Value] = '6767_JMP') 

--SELECT
--	[Lookup Value]
--FROM [dbGroupTwoB].dbo.[Jumpman Limited$Generic Lookups] GL WITH (NOLOCK)
--Where GL.[Lookup Type] = 'GAMENAME' AND RIGHT ( [Lookup Value], 3 ) in ( 'TSG','JMP') AND [Lookup Value] = '11787_JMP' 
--order by 1


--SELECT SUM ( Amount ) FROM [Jumpman Limited$BINAVImport History] WITH (NOLOCK ) WHERE [Posting Date] ='2025-03-01' AND  ImportType = 'REVENUE'  --AND D16 != ''
--AND [Document No] = 'INC_MAR2025'  AND D4 = 'THE SUN PLAY' AND D8 = 'NOLIMITCITY'
--AND D16 IN  ( 
--SELECT
--	[Lookup Value]
--FROM [dbGroupTwoB].dbo.[Jumpman Limited$Generic Lookups] GL WITH (NOLOCK)
--Where GL.[Lookup Type] = 'GAMENAME' AND RIGHT ( [Lookup Value], 3 ) in ( 'TSG','JMP')) --AND [Lookup Value] = '6767_JMP') 


--SELECT SUM ( Amount ) FROM [Jumpman Limited$BINAVImport History] WITH (NOLOCK ) WHERE [Posting Date] ='2025-03-01' AND  ImportType = 'REVENUE'  --AND D16 != ''
--AND [Document No] = 'INC_MAR2025'  AND D4 = 'THE SUN PLAY' AND D8 = 'NOLIMITCITY'
--AND D16 NOT IN  ( 
--SELECT
--	[Lookup Value]
--FROM [dbGroupTwoB].dbo.[Jumpman Limited$Generic Lookups] GL WITH (NOLOCK)
--Where GL.[Lookup Type] = 'GAMENAME' AND RIGHT ( [Lookup Value], 3 ) in ( 'TSG','JMP')) --AND [Lookup Value] = '6767_JMP') 



SELECT  SUM ( Amount ) FROM [Jumpman Limited$BINAVImport] WITH (NOLOCK ) WHERE [Posting Date] ='2025-03-01' AND  ImportType = 'REVENUE'  --AND D16 != ''
AND [Document No] = 'INC_MAR2025'  AND D4 = 'THE SUN PLAY' 
AND Exceptions = ''


select [Posting Date], [Document No], count(*), SUM ( Amount ) from [Jumpman Limited$BINAVImport History] WITH (NOLOCK )
WHERE [Posting Date] >= '2025-03-01' AND [Posting Date] < '2025-04-01' AND ImportType = 'REVENUE' AND BatchGUID = 'DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD'
AND [Import Date] >= '2025-03-26'
AND Exceptions = ''
GROUP BY [Posting Date], [Document No]

SELECT *FROM [Jumpman Limited$BINAVImport History] WITH (NOLOCK ) 
WHERE [Posting Date] >= '2025-03-01' AND  ImportType = 'REVENUE' 
AND BatchGUID = '8D85037F-84F5-4E5A-8BB8-6701D5C202F9'


SELECT [Document No], COUNT(*), SUM ( Amount ) from [Jumpman Limited$BINAVImport History] WITH (NOLOCK ) 
WHERE [Posting Date] >= '2025-03-01' AND  ImportType = 'REVENUE' 
AND Exceptions = 'FIN-33409'
GROUP BY [Document No]


select *from [Jumpman Limited$BINAVImport] WITH (NOLOCK ) WHERE [Posting Date] >= '2025-03-01' AND [Posting Date] < '2025-04-01'
AND LEFT ( D16 , CHARINDEX ( '_',D16) - 1 ) NOT IN ( SELECT uniqueid from dbFinanceReporting..[Jumpman_GamesMapping] ) 

select [Document No], Sum ( Amount ), cOUNT(*) from [Jumpman Limited$BINAVImport] where OperatorGroup ='JUMPMAN' AND [Posting Date] >= '2025-03-01' and [Posting Date] < '2025-04-01' AND ImportType = 'REVENUE'   --AND BatchGUID = '717B2787-4B80-4CD9-9682-55A63AB5EB0C'
--and BatchGUID IN (  LEFT ( D16, CHARINDEX ( '_',D16) - 1 ) NOT IN ( SELECT UniqueID FROM GamesMapping ) 
--AND Exceptions != ''
GROUP BY [Document No]
ORDER BY 1 

SELECT DISTINCT BatchGUID from [Jumpman Limited$BINAVImport]
WHERE BatchGUID != 'DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD'

select ImportType, Count(*) from [Jumpman Limited$BINAVImport] WITH (NOLOCK ) 
WHERE [Posting Date] >= '2025-03-01' and [Posting Date] < '2025-04-01' 
Group by ImportType

select ImportType, Count(*) from [Jumpman Limited$BINAVImport] WITH (NOLOCK ) 
WHERE [Posting Date] >= '2025-03-01' and [Posting Date] < '2025-04-01' 
and Exceptions = '' 
Group by ImportType

select [Document No], Count(*), SUM ( Amount )  from [Jumpman Limited$BINAVImport] WITH (NOLOCK ) 
WHERE [Posting Date] >= '2025-03-01' and [Posting Date] < '2025-04-01' 
and Exceptions = '' 
Group by [Document No]

