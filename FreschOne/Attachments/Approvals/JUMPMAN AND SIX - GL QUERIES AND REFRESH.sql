truncate table dbFinanceReporting..select_player_balance_SIX_march2025
--use 
--insert into select_player_balance_march2025
--exec dbICControl..[usp_select_player_balance_report] '2025-03-01' , '2025-03-31', 'Jumpman Limited', 'PLAYER ACC|PLAYERACC'
--truncate table dbFinanceReporting..select_player_balance_SIX_march2025

--select top 5 *From select_player_balance_march2025

truncate table select_player_balance_march2025

insert into select_player_balance_march2025
exec dbICControl..[usp_select_player_balance_report] '2025-03-01' , '2025-03-31', 'Jumpman Limited', 'PLAYER ACC|PLAYERACC'

INSERT INTO dbFinanceReporting..select_player_balance_SIX_march2025
SELECT 'dbGroupTwoB' [Database Name]      
     ,'The Six Gaming Limited' [Company Name]      
	 ,ble.[Entry No_]      
     ,ble.[Bank Account No_]      
     ,CONVERT(VARCHAR(10),ble.[Posting Date],103) [Posting Date]      
     ,DATENAME(MONTH,ble.[Posting Date]) [Posting Month]      
     ,DATENAME(YEAR,ble.[Posting Date]) [Posting Year]       
     ,ble.[Document Date]      
     ,replace(REPLACE(REPLACE(ble.[Document No_], CHAR(13), ''), CHAR(10), ''),CHAR(44),'') [Document No_]          
     ,replace(REPLACE(REPLACE(ble.[Description], CHAR(13), ''), CHAR(10), ''),CHAR(44),'') [Description]      
     ,ble.[Currency Code]      
     ,ble.[Amount]      
     ,ble.[Remaining Amount]      
     ,ble.[Amount (LCY)]      
     ,ble.[Bank Acc_ Posting Group]      
     ,ble.[User ID]      
     ,ble.[Source Code]      
     ,ble.[Open]      
     ,ble.[Positive]      
     ,ble.[Journal Batch Name]      
     ,ble.[Reason Code]      
     ,BAccType.OptionName [Bal_ Account Type]      
     ,ble.[Bal_ Account No_]      
     ,ble.[Transaction No_]      
     ,ble.[Statement Status]      
     ,ble.[Statement No_]      
     ,ble.[Statement Line No_]      
     ,replace(REPLACE(REPLACE(ble.[External Document No_], CHAR(13), ''''), CHAR(10), ''''),CHAR(44),'''') [External Document No_]      
     ,ble.[Dimension Set ID]      
       ,ISNULL(dse.[Shortcut Dimension 1 Code],'') [Shortcut Dimension 1 Code]      
     ,ISNULL(dse.[Shortcut Dimension 2 Code],'') [Shortcut Dimension 2 Code]      
    ,ISNULL(dse.[Shortcut Dimension 3 Code],'') [Shortcut Dimension 3 Code]      
    ,ISNULL(dse.[Shortcut Dimension 4 Code],'') [Shortcut Dimension 4 Code]      
     ,ISNULL(dse.[Shortcut Dimension 5 Code],'') [Shortcut Dimension 5 Code]      
     ,ISNULL(dse.[Shortcut Dimension 6 Code],'') [Shortcut Dimension 6 Code]      
     ,ISNULL(dse.[Shortcut Dimension 7 Code],'') [Shortcut Dimension 7 Code]      
     ,ISNULL(dse.[Shortcut Dimension 8 Code],'') [Shortcut Dimension 8 Code]     
	 --, glook.[Lookup Value] as 'GameName GenLookup BI'
     ,cod.Name [Bal. Account Type Coding]       
	 ,ISNULL(dse.[Shortcut Dimension 16 Code],'') [Shortcut Dimension 16 Code]
    FROM dbGroupTwoB.dbo.[The Six Gaming Limited$Bank Account Ledger Entry] ble with(nolock)      
   left join [NAV$Dimension Setup] d_setup with(nolock) on      
    d_setup.[Database Name] = 'dbGroupTwoB' 
   LEFT JOIN dbGroupTwoB.dbo.[The Six Gaming Limited$Default Dimension] co with(nolock) ON      
     ((co.[Table ID] = 15 and ble.[Bal_ Account Type] = 0)       
      or (co.[Table ID] = 18 and ble.[Bal_ Account Type] = 1)      
      or (co.[Table ID] = 23 and ble.[Bal_ Account Type] = 2)      
      or (co.[Table ID] = 27 and ble.[Bal_ Account Type] = 3))      
        AND co.No_ = ble.[Bal_ Account No_]      
     and co.[Dimension Code] = d_setup.[Shortcut Dimension 13 Code]        
    left join dbGroupTwoB.dbo.[The Six Gaming Limited$Dimension Value] cod with(nolock) on      
	cod.[Dimension Code] = co.[Dimension Code]      
	  and cod.Code = co.[Dimension Value Code]    
	     left join dbGroupTwoB.dbo.[The Six Gaming Limited$Dimension Value] dv with (nolock) on
     dv.[Code] = [Shortcut Dimension 16 Code]
	 and dv.[Dimension Code] = 'GAMENAME'
   left join NAVOptionName BAccType with(nolock) on      
    BAccType.TableName = 'Bank Account Ledger Entry'      
    and BAccType.FieldName = 'Bal_ Account Type'        
    and BAccType.OptionNo = ble.[Bal_ Account Type]      
LEFT JOIN [NAV$Dimension Set Entry] dse with(nolock) ON      
    dse.[Dimension Set ID] = ble.[Dimension Set ID]      
    and [Company Name (SQL)] = 'The Six Gaming Limited' 
left join dbGroupTwoB.dbo.[The Six Gaming Limited$Generic Lookups] glook with (nolock) on      
	glook.[Lookup Type] = 'GAMENAME' and dse.[Shortcut Dimension 16 Code] = glook.[Replacement Value]	   
INNER JOIN dbGroupTwoB.dbo.[The Six Gaming Limited$Bank Account] BankAcc with(nolock) on      
    BankAcc.No_ = ble.[Bank Account No_]      
    WHERE ble.[Posting Date] between '2025-03-01' and '2025-04-01'
	  --and [Source Code] = 'REVENUE'


 SELECT * FROM dbFinanceReporting..select_player_balance_march2025
 
/*select ble.*, dse.[Dimension Value Code]  as 'GameName',dv.[Name]  as 'GameName Name', glook.[Lookup Value] as 'GameName GenLookup BI', dv.[Map-to IC Dimension Value Code] as 'GameName DimValueLookup BI' from dbGroupTwoB.dbo.[The Six Gaming Limited$Bank Account Ledger Entry] ble with (nolock)
left join dbGroupTwoB.dbo.[The Six Gaming Limited$Dimension Set Entry] dse with (nolock) on      
	dse.[Dimension Set ID] = ble.[Dimension Set ID] and dse.[Dimension Code] = 'GAMENAME'
left join dbGroupTwoB.dbo.[The Six Gaming Limited$Generic Lookups] glook with (nolock) on      
	glook.[Lookup Type] = 'GAMENAME' and dse.[Dimension Value Code] = glook.[Replacement Value]
left join dbGroupTwoB.dbo.[The Six Gaming Limited$Dimension Value] dv with (nolock) on      
	dv.[Dimension Code] = 'GAMENAME' and dv.[Code] = dse.[Dimension Value Code]
where [Posting Date] between '2025-03-01' and '2025-03-10'
  and [Source Code] = 'REVENUE'
  
 */