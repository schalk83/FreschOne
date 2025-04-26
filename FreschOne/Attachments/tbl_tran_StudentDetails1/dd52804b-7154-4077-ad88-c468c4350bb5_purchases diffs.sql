--exec sp_helptext vw_PurchaseDetail

select *from vw_PurchaseDetail where DateKey = 20240908          AND DateKey < 20240930  

select DateKey, SUM ( transaction_value )	from vw_PurchaseDetail		where DateKey >= 20240901          AND DateKey < 20240930      group by DateKey
select DateKey, sum ( TRANSACTION_VALUE )  from psp_transaction_1_0 pt	where  oPERATION <> 'D' and DateKey >= 20240901   AND DateKey < 20240930 and category = 'Deposit' and description = 'Deposit'      AND [pt].[description] <> 'WITHDRAWAL_FINALISED'     
   group by DateKey
--exec sp_helptext vw_cashindetails

SELECT  SUM ( transaction_value )  from vw_CashinDetails where DateKey >= 20240901             AND DateKey < 20241001  
select  SUM ( transaction_value ) , SUM ( CASE                         
          -- NGN fees are 250 for debit and credit                        
          WHEN pt.transaction_value > 0 AND p.Currency = 'NGN'  THEN pt.transaction_value + 250                        
          WHEN pt.transaction_value < 0 AND P.Currency = 'NGN'  THEN pt.transaction_value - 250                        
          -- Other currency fees are 2.5 for debit and credit                        
          WHEN pt.transaction_value > 0 AND P.Currency <> 'NGN' THEN pt.transaction_value + 2.5                        
         ELSE                         
          pt.transaction_value - 2.5 -- Only remaining condition should be when the values are negative for all currencies except NGN. These should get a - 2.5                        
         END                          )
		 from psp_transaction_1_0 pt 
		 
		     JOIN dbo.NAVEntryTypeMapping netm                        
     ON netm.account_type = ''                        
     AND pt.[description] = netm.[description]                        
     AND pt.[category] = netm.[category]                        
   AND netm.ImportType IN ('CTACASHIN' )      
   LEFT JOIN dbo.Player p                        
     ON pt.user_account = p.id                        
     AND pt.DateKey >= p.FromDate                        
     AND pt.DateKey < p.ToDate    

		 where DateKey >= 20240901   AND DateKey < 20241001
--and category IN ( 'CASH_PENDING_WITHDRAWAL',
--'CASH_PENDING_WITHDRAWAL_EXCEPTION',
--'WITHDRAWAL_CANCELLATION',
--'WITHDRAWAL_CANCELLATION_EXCEPTION',
--'WITHDRAWAL_EXPIRY',
--'WITHDRAWAL_FAILED',
--'WITHDRAWAL_REVERSAL' ) 
  --AND pt.Operation <> 'D'               
 -- AND NOT                         
   --    (                        
   --    ISNULL(p.address_country, '') = 'Canada'                         
   --    AND CASE WHEN p.address_region = 'CA-Ontario' then 'ON' ELSE ISNULL(p.RegionCode, '') END IS NULL                           
   --    )                        
   --   AND ISNULL(p.address_country, '') <> ''                        
   --   AND ISNULL(p.[brand_name], '') <> ''                        
   --   AND ISNULL(p.brand_type, '') <> ''                        
   --   AND pt.Operation <> 'D'          

  --select sum ( Amount ) From Cashin where [Posting Date] >= '2024-09-01'   AND [Posting Date] < '2024-10-01' 


  SELECT	 SUM ( WithdrawalValue ) Amount_Source
	--INTO #source	
	FROM vw_CashinDetails
		 where DateKey >= 20240901   AND DateKey < 20241001
	--GROUP BY LEFT ( DateKey, 6 ), DateKey,OperatorGroup, CurrencyCode, REPLACE ( REPLACE ( BrandName,'Super Mega Fluffy Rainbow Vegas Jackpot Casino','SUPER MEGA FLUFFY RAINBOW VEGA' ),'SUPER MEGA FLUFFY RAINBOW VEGAS JACKPOT','SUPER MEGA FLUFFY RAINBOW VEGA' ),brand_status
	--HAVING SUM ( WithdrawalValue ) <> 0 

