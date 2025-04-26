CREATE VIEW [dbo].[uv_GroupReports_Entity]                
AS                 
                
WITH RunningTotals AS                
    (                
    SELECT                
             a.YYYYMM                
   ,CASE WHEN a.[Group] = 'Jumpman'        THEN 'Jumpman'                
     WHEN a.[Group] IN ('BW Africa', 'DGI', 'BWP', 'BWF') THEN 'BWL'                
    ELSE                 
     c.FinGroup                
    END AS FinGroup                
            ,a.[Group]          --SELECT *FROM uv_GroupReports_RunningTotal_ALL WHERE [Group] =  'BW Africa' and brand != 'Jackpot City South Africa'      
   ,CASE WHEN [A].[Group] = 'BW Africa' THEN a.[Brand]                  
     WHEN [A].[Group] = 'DGI'  THEN 'Betway88'                
     WHEN [A].[Group] = 'BWP'  THEN 'BetWay Portugal Sports'                 
     WHEN [A].[Group] = 'Jumpman' THEN 'Unspecified MLG JUMP Brand'                
     WHEN [A].[Group] = 'BWF'  THEN 'Betway France'                
    ELSE                 
     C.BrandName                 
    END AS Brand                      
   ,CASE WHEN a.[Group] = 'Jumpman'      THEN 'Casino'                 
     WHEN a.[Group] IN ('BW Africa', 'BWP', 'BWF') THEN 'Sportsbook'                 
     WHEN a.[Group] = 'DGI'       THEN 'Unspecified ASI Product'                
    ELSE                 
     a.Product                 
    END AS Product                
 ,a.Country  --,CASE WHEN [A].[Group] = 'Jumpman' THEN 'United Kingdom of Great Britain and Northern Ireland' ELSE a.Country END AS Country             
 ,CASE WHEN a.Country in ( 'Canada','Argentina' ) then a.StateOrProvince ELSE 'Other' END AS StateOrProvince           
 ,SUM(a.Users_MTD) AS Users                 
    FROM dbo.uv_GroupReports_RunningTotal_ALL a        --select *From uv_GroupReports_RunningTotal_ALL where YYYYMM >= 202301        
    JOIN dbDWAlignment.dbo.dimDate_New b                
            ON a.YYYYMMDD = b.dateKey                
            AND b.LastDayOfMonthFlag = 'Y'                
            AND b.dateKey >= 20190101                
 LEFT JOIN dbDWAlignment.dbo.dimCasinoDetail_New c                
   ON a.GamingServerID = c.Hist_PTSGamingserverID                
   AND a.CasinoID = c.Hist_PTSCasinoID                
 WHERE                
  a.[Group] NOT IN ('CSAG')                
    GROUP BY                
    a.YYYYMM                
   ,CASE WHEN a.[Group] = 'Jumpman'        THEN 'Jumpman'                
     WHEN a.[Group] IN ('BW Africa', 'DGI', 'BWP', 'BWF') THEN 'BWL'                
    ELSE                 
     c.FinGroup                
    END                
    ,a.Country  --,CASE WHEN [A].[Group] = 'Jumpman' THEN 'United Kingdom of Great Britain and Northern Ireland' ELSE a.Country END AS Country             
 ,CASE WHEN a.Country in ( 'Canada','Argentina' ) then a.StateOrProvince ELSE 'Other' END           
    ,a.[Group]                  
   ,CASE WHEN a.[Group] = 'Jumpman'      THEN 'Casino'                 
     WHEN a.[Group] IN ('BW Africa', 'BWP', 'BWF') THEN 'Sportsbook'                 
     WHEN a.[Group] = 'DGI'       THEN 'Unspecified ASI Product'                
    ELSE                 
     a.Product                 
    END                  
   ,CASE WHEN [A].[Group] = 'BW Africa' THEN a.[Brand]                   
     WHEN [A].[Group] = 'DGI'  THEN 'Betway88'                
     WHEN [A].[Group] = 'BWP'  THEN 'BetWay Portugal Sports'                 
     WHEN [A].[Group] = 'Jumpman' THEN 'Unspecified MLG JUMP Brand'                 
     WHEN [A].[Group] = 'BWF'  THEN 'Betway France'                
    ELSE                 
     C.BrandName                 
    END                
 HAVING SUM(a.Users_MTD) <> 0                
    )                
        
 ----START of First Purchase Count (Active P)        
SELECT                  
        LEFT(a.YYYYMM,4) AS FinancialYear,                
  RIGHT(a.YYYYMM,2) AS FinancialMonth,                
  'Actual' AS [Version],                
  'Local' AS Currency,                
  CASE WHEN b.Entity = '02_Betway Ltd' AND A.country = 'Spain'                
    THEN '09_Betway_Spain'                
    WHEN b.Entity = 'GRP2.19_Baytree2' AND A.country NOT IN (                
                   'Canada'   ,'Andorra'      ,'Honduras'                  
                  ,'Argentina'  ,'Cost Rica'     ,'Nicaragua'                
                  ,'Bolivia'   ,'Dominican Republic'   ,'Panama'                
                  ,'Brazil'   ,'Ecuador'      ,'Paraguay'                
                  ,'Chile'   ,'El Salvador'     ,'Peru'                
                  ,'Columbia'  ,'Falkland Islands (Malvinas)' ,'Suriname'                
                  ,'Mexico'   ,'Guatemala'     ,'Uruguay'                
                  ,'Spain'   ,'Guyana'      ,'Venezuela'                
                  )                
    THEN '05_Bayton Ltd'                
    WHEN a.FinGroup = 'TBFVS' AND a.[Group] IN (                
                'FLG Bingo'  ,'BRE Bingo' ,'TPG Casino'                 
               ,'FLG Casino'  ,'BRE Casino' ,'TPG Poker'                 
               ,'FLG Poker'  ,'BRE Poker' ,'TPG Sports'                
              )                
          AND a.Country = 'Australia'                
    THEN '0'                
    WHEN a.[Group] = 'BW Africa' THEN CASE                 
            WHEN A.Country = 'Uganda'  THEN 'GRP1A_1_TheRangers'                
            WHEN A.Country = 'Ghana'  THEN 'GRP1A_2_SportsBettingGroup'                
            WHEN A.Country = 'Zambia'  THEN 'Emerald Bay Limited'                            
   WHEN A.Country = 'Kenya'  THEN 'GRP1A_4_BluJay'                
            WHEN A.Country = 'Nigeria'  THEN 'GRP1A_6_DigiBayLimited'                
            WHEN A.Country = 'South Africa' AND a.Brand != 'Jackpot City South Africa'THEN 'New_RagingRiverTrading'          
   WHEN A.Country = 'South Africa' AND a.Brand = 'Jackpot City South Africa' THEN 'Eastern Dawn'            
            WHEN A.Country = 'Mozambique' THEN 'JSE - Jogos Sociais E Enteretenimento SA'                
            WHEN A.Country = 'Tanzania'  THEN 'GRP1A_7_MediaBayLimited'                
   WHEN A.Country = 'Malawi' THEN 'Golden Bay'            
             WHEN A.Country = 'Botswana' THEN 'Delta Bay'  
  
  
           ELSE                
            b.Entity                
           END                 
   ELSE                 
    b.Entity                
  END AS Entity,                
  CASE WHEN  a.[Group] = 'BW Africa' THEN 'Unspecified AFR Platform' ELSE b.[Platform] END AS [Platform],                    
        UPPER(CASE WHEN a.[Group] = 'Jumpman'      THEN 'Casino'                 
     WHEN a.[Group] IN ('BW Africa', 'BWP', 'BWF') THEN 'Sportsbook'                 
     WHEN a.[Group] = 'DGI'       THEN 'Unspecified ASI Product'                
     ELSE                 
     a.Product                 
     END) AS Product,                
        UPPER(a.Country) AS Region,             
  CASE WHEN a.Country in ( 'Canada','Argentina' ) then a.StateOrProvince ELSE 'Other' END AS StateOrProvince,          
  CASE WHEN [A].[Group] = 'BW Africa'  THEN a.Brand                
    WHEN [A].[Group] = 'DGI'   THEN 'Betway88'                
    WHEN [A].[Group] = 'BWP'   THEN 'BetWay Portugal Sports'                 
    WHEN [A].[Group] = 'Jumpman'  THEN 'Unspecified MLG JUMP Brand'                 
    WHEN [A].[Group] = 'BWF'   THEN 'Betway France'                
  ELSE                 
    C.BrandName                 
  END AS Brand,                
  CASE WHEN  a.[Group] = 'BW Africa' THEN 'Unspecified AFR Department' ELSE b.[Department] END AS [Department],                
  CA.GLAccount,                
  'Amount' AS Finance_m,                
        SUM(CASE                 
    WHEN CA.GLAccount = 'First Purchase Count (Active P)' THEN ISNULL([A].[ActiveP],0)                
 WHEN CA.GLAccount = 'New Accounts Count (Opens)' THEN ISNULL ( a.Opens,0 )         
 WHEN CA.GLAccount = 'First Played Count (Actives)' THEN ISNULL ( a.Actives, 0 )         
   ELSE                 
    CASE WHEN A.[Group] = 'DGI' THEN ISNULL([A].[Cashins],0)  --We only want DGI Cashins                
    ELSE                 
     0                
    END                 
   END ) AS [Values]                 
  ,a.[Group]         
FROM dbFinanceReporting.dbo.uv_GroupReports_ALL A    --SELECT TOP 50 * FROM uv_GroupReports_ALL WHERE [Group] = 'BW Africa' AND Country = 'Malawi' ORDER BY YYYYMM DESC            
LEFT JOIN dbFinanceReporting.dbo.tbl_GroupReports_Entity b    --SELECT *FROM dbFinanceReporting.dbo.tbl_GroupReports_Entity WHERE [Group] = 'BW Africa'            
 ON A.[Group] = B.[Group]                
 AND a.FinGroup = b.Fingroup                
LEFT JOIN dbDWAlignment.dbo.dimCasinoDetail_New AS C                 
 ON A.CasinoID = C.Hist_PTSCasinoID                 
 AND A.GamingServerID = C.Hist_PTSGamingserverID                
CROSS APPLY                
            (                
                SELECT GLAccount                
                FROM(VALUES                
                            (                
                            'First Purchase Count (Active P)'                
   ),                
       (        
       'New Accounts Count (Opens)'        
       ),        
       (        
       'First Played Count (Actives)'        
       )        
       ,        
       (                
                            'Net Cashins'                
                            )) AS NJ(GLAccount)                
            ) CA                
WHERE                
    a.YYYYMM IN (SELECT DISTINCT YYYYMM FROM RunningTotals)                
 AND a.[Group] NOT IN ('CSAG')                
GROUP BY                
        LEFT(a.YYYYMM,4),                
  RIGHT(a.YYYYMM,2),                
   CASE WHEN b.Entity = '02_Betway Ltd' AND A.country = 'Spain'                
    THEN '09_Betway_Spain'                
    WHEN b.Entity = 'GRP2.19_Baytree2' AND A.country NOT IN (                
                   'Canada'   ,'Andorra'      ,'Honduras'                  
                  ,'Argentina'  ,'Cost Rica'     ,'Nicaragua'                
         ,'Bolivia'   ,'Dominican Republic'   ,'Panama'                
                  ,'Brazil'   ,'Ecuador'      ,'Paraguay'                
                  ,'Chile'   ,'El Salvador'     ,'Peru'                
                  ,'Columbia'  ,'Falkland Islands (Malvinas)' ,'Suriname'                
                  ,'Mexico'   ,'Guatemala'     ,'Uruguay'                
                  ,'Spain'   ,'Guyana'      ,'Venezuela'                
                  )                
    THEN '05_Bayton Ltd'                
   WHEN a.FinGroup = 'TBFVS' AND a.[Group] IN (                
                'FLG Bingo'  ,'BRE Bingo' ,'TPG Casino'                 
               ,'FLG Casino'  ,'BRE Casino' ,'TPG Poker'                 
               ,'FLG Poker'  ,'BRE Poker' ,'TPG Sports'                
              )                
          AND a.Country = 'Australia'                
    THEN '0'                
    WHEN a.[Group] = 'BW Africa' THEN CASE                 
            WHEN A.Country = 'Uganda'  THEN 'GRP1A_1_TheRangers'                
            WHEN A.Country = 'Ghana'  THEN 'GRP1A_2_SportsBettingGroup'                
            WHEN A.Country = 'Zambia'  THEN 'Emerald Bay Limited'                
            WHEN A.Country = 'Kenya'  THEN 'GRP1A_4_BluJay'                
            WHEN A.Country = 'Nigeria'  THEN 'GRP1A_6_DigiBayLimited'                
            WHEN A.Country = 'South Africa' AND a.Brand != 'Jackpot City South Africa'THEN 'New_RagingRiverTrading'          
   WHEN A.Country = 'South Africa' AND a.Brand = 'Jackpot City South Africa' THEN 'Eastern Dawn'            
            WHEN A.Country = 'Mozambique' THEN 'JSE - Jogos Sociais E Enteretenimento SA'                
            WHEN A.Country = 'Tanzania'  THEN 'GRP1A_7_MediaBayLimited'                
      WHEN A.Country = 'Malawi' THEN 'Golden Bay'      
   WHEN A.Country = 'Botswana' THEN 'Delta Bay'  
      
           ELSE                
            b.Entity                
           END                 
   ELSE                 
    b.Entity                
  END,                
  CASE WHEN  a.[Group] = 'BW Africa' THEN 'Unspecified AFR Platform' ELSE b.[Platform] END,                 
  CASE WHEN  a.[Group] = 'BW Africa' THEN 'Unspecified AFR Department' ELSE b.[Department] END,                
        UPPER(CASE WHEN a.[Group] = 'Jumpman'      THEN 'Casino'                 
     WHEN a.[Group] IN ('BW Africa', 'BWP', 'BWF') THEN 'Sportsbook'                 
     WHEN a.[Group] = 'DGI'       THEN 'Unspecified ASI Product'                
     ELSE                 
     a.Product                 
     END),                
   UPPER(a.Country) ,             
  CASE WHEN a.Country in ( 'Canada','Argentina' ) then a.StateOrProvince ELSE 'Other' END ,--AS StateOrProvince,          
  CASE WHEN [A].[Group] = 'BW Africa'  THEN  a.Brand                
    WHEN [A].[Group] = 'DGI'   THEN 'Betway88'                
    WHEN [A].[Group] = 'BWP'   THEN 'BetWay Portugal Sports'                 
    WHEN [A].[Group] = 'Jumpman'  THEN 'Unspecified MLG JUMP Brand'                 
    WHEN [A].[Group] = 'BWF'   THEN 'Betway France'                
   ELSE                 
    C.BrandName                 
   END,                
  CA.GLAccount,                
  a.[Group]                
HAVING SUM(CASE                 
        
    WHEN CA.GLAccount = 'First Purchase Count (Active P)' THEN ISNULL([A].[ActiveP],0)                
 WHEN CA.GLAccount = 'New Accounts Count (Opens)' THEN ISNULL ( a.Opens,0 )         
 WHEN CA.GLAccount = 'First Played Count (Actives)' THEN ISNULL ( a.Actives, 0 )         
        
   ELSE                 
    CASE WHEN A.[Group] = 'DGI' THEN ISNULL([A].[Cashins],0)  --We only want DGI Cashins                
    ELSE                 
     0                
    END                 
   END) <> 0                
    ---- of First Purchase Count (Active P)        
        
UNION                 
                
SELECT                
                  
        LEFT(a.YYYYMM,4) AS FinancialYear,                
  RIGHT(a.YYYYMM,2) AS FinancialMonth,                
  'Actual' AS [Version],                
  'Local' AS Currency,                
   CASE WHEN b.Entity = '02_Betway Ltd' AND A.country = 'Spain'                
    THEN '09_Betway_Spain'                
    WHEN b.Entity = 'GRP2.19_Baytree2' AND A.country NOT IN (                
                   'Canada'   ,'Andorra'      ,'Honduras'                  
                  ,'Argentina'  ,'Cost Rica'     ,'Nicaragua'                
                  ,'Bolivia'   ,'Dominican Republic'   ,'Panama'                
                  ,'Brazil'   ,'Ecuador'      ,'Paraguay'                
                  ,'Chile'   ,'El Salvador'     ,'Peru'                
                  ,'Columbia'  ,'Falkland Islands (Malvinas)' ,'Suriname'                
                  ,'Mexico'   ,'Guatemala'     ,'Uruguay'                
                  ,'Spain'   ,'Guyana'      ,'Venezuela'                
                  )                
    THEN '05_Bayton Ltd'                
    WHEN a.FinGroup = 'TBFVS' AND a.[Group] IN (                
                'FLG Bingo'  ,'BRE Bingo' ,'TPG Casino'                 
               ,'FLG Casino'  ,'BRE Casino' ,'TPG Poker'                 
               ,'FLG Poker'  ,'BRE Poker' ,'TPG Sports'                 
              )                
          AND a.Country = 'Australia'                
    THEN '0'                
    WHEN a.[Group] = 'BW Africa' THEN CASE                 
            WHEN A.Country = 'Uganda'  THEN 'GRP1A_1_TheRangers'                
            WHEN A.Country = 'Ghana'  THEN 'GRP1A_2_SportsBettingGroup'                
            WHEN A.Country = 'Zambia'  THEN 'Emerald Bay Limited'                
            WHEN A.Country = 'Kenya'  THEN 'GRP1A_4_BluJay'                
   WHEN A.Country = 'Nigeria'  THEN 'GRP1A_6_DigiBayLimited'                
            WHEN A.Country = 'South Africa' AND a.Brand != 'Jackpot City South Africa'THEN 'New_RagingRiverTrading'          
   WHEN A.Country = 'South Africa' AND a.Brand = 'Jackpot City South Africa' THEN 'Eastern Dawn'            
            WHEN A.Country = 'Mozambique' THEN 'JSE - Jogos Sociais E Enteretenimento SA'                
            WHEN A.Country = 'Tanzania'  THEN 'GRP1A_7_MediaBayLimited'                
   WHEN A.Country = 'Malawi' THEN 'Golden Bay'          
      WHEN A.Country = 'Botswana' THEN 'Delta Bay'      
           ELSE                
            b.Entity                
           END                 
   ELSE                 
    b.Entity                
  END AS Entity,                
  CASE WHEN  a.[Group] = 'BW Africa' THEN 'Unspecified AFR Platform' ELSE b.[Platform] END AS [Platform],                 
        UPPER(CASE WHEN a.[Group] = 'Jumpman'      THEN 'Casino'                 
     WHEN a.[Group] IN ('BW Africa', 'BWP', 'BWF') THEN 'Sportsbook'                 
     WHEN a.[Group] = 'DGI'       THEN 'Unspecified ASI Product'                
     ELSE                 
     a.Product                 
     END) AS Product,                
        UPPER(a.Country) AS Region,                
  CASE WHEN a.Country in ( 'Canada','Argentina' ) then a.StateOrProvince ELSE 'Other' END AS StateOrProvince,          
  a.Brand,            
  CASE WHEN  a.[Group] = 'BW Africa' THEN 'Unspecified AFR Department' ELSE b.[Department] END AS [Department],                
  'Distinct Player Count(Users)' AS GLAccount,                
        
        
  'Amount' AS Finance_m,                
        ISNULL(a.Users,0)                            AS [Values]                 
  ,a.[Group]                                        
FROM RunningTotals A                
LEFT JOIN dbFinanceReporting.dbo.tbl_GroupReports_Entity b                
 ON A.[Group] = B.[Group]                
 AND a.FinGroup = b.fingroup                
        