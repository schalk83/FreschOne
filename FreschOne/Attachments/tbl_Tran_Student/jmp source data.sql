
--SELECT count(*) FROM account_balance_snapshot_1_2 with (nolock) WHERE Datekey	 >= 20250101

select count(*)  from slots_latest_1_0 with (nolock)   WHERE  Datekey	 >= 20250201 and DateKey < 20250301

select count(*) from player_event_dims_1_0 WHERE  Datekey	 >= 20250201 and DateKey < 20250301

select count(*)
from [dbo].[account_1_2] WHERE Datekey	 >= 20250201 and DateKey < 20250301

select 
count(*) from psp_transaction_1_0 WHERE  Datekey	 >= 20250201 and DateKey < 20250301

select 
count(*)
from psp_brand_transaction_1_0 WHERE  Datekey	 >= 20250201 and DateKey < 20250301



select 
count(*)from revenue_summary_hourly_1_0 WHERE  Datekey	 >= 20250201 and DateKey < 20250301

select count(*) from [dbo].[adjustments_summary_hourly_1_0] with (nolock) WHERE  Datekey	 >= 20250201 and DateKey < 20250301


SELECT 
count(*) FROM  Gamesmapping 

SELECT 
count(*) FROM  Player  with (nolock) WHERE id in ( select user_account from revenue_summary_hourly_1_0  with (nolock) WHERE  Datekey	 >= 20250201 and DateKey < 20250301 ) 


select count(*) from currencyconversion

select  distinct left (  gamecode, charindex('_',gamecode) - 1 ), sub_network from GamesMapping_sub


