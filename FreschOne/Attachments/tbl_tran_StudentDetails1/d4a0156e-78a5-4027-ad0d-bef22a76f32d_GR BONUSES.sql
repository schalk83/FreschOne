    declare @StartDate int = 20240901,                                
    @EndDate int  = 20241001;
  
 
 IF OBJECT_ID('Tempdb.dbo.#DimPlayer') IS NOT NULL                                
        DROP TABLE #DimPlayer;                             
    CREATE TABLE #DimPlayer                                
    (                                
        PlayerKey INT NOT NULL,                                
        Hist_PTSGamingserverID INT NOT NULL                                
            DEFAULT 0,                                
        Hist_PTSCasinoID INT NOT NULL                                
            DEFAULT 0,                                
        Hist_PTSUserID INT NOT NULL                                
            DEFAULT 0,                                
        Country NVARCHAR(100) NOT NULL                                
            DEFAULT 'Unknown',                                
  StateOrProvince varchar(128) NOT NULL                 
   DEFAULT 'Unknown',                  
        PlayerType VARCHAR(150) NOT NULL                                
            DEFAULT 'Unknown'                                
    );   

	IF OBJECT_ID('Tempdb.dbo.#PlayerKeys') IS NOT NULL                                
        DROP TABLE #PlayerKeys;                                
    CREATE TABLE #PlayerKeys                                
    (                                
        DateKey INT NOT NULL                                
            DEFAULT 0,                                
        PlayerKey INT NOT NULL                                
            DEFAULT 0,                                
        IncomeCurrencyValue FLOAT NOT NULL                                
            DEFAULT 0                                
    );   

	 INSERT INTO #PlayerKeys                                
            (                        
                DateKey,                                
                PlayerKey,                                
                IncomeCurrencyValue                                
            )                                
            SELECT DISTINCT                                
                   fr.dateKey,                                
                   fr.playerKey,              
    SUM(fr.IncomeCurrencyValue)                                
            FROM dbDWAlignment.dbo.FactRevenue fr                 
            WHERE fr.dateKey >= @StartDate --This date is necessary for the MTD User Counts                                                            
                  AND fr.dateKey < @EndDate
            GROUP BY fr.dateKey,                                
                     fr.playerKey;   

			INSERT INTO #PlayerKeys                       
            (                                
                DateKey,                                
                PlayerKey,                                
                IncomeCurrencyValue                                
            )                                
            SELECT DISTINCT                                
          fpe.dateKey,                                
                   fpe.playerKey,                                
                   0                                
            FROM dbDWAlignment.dbo.factPlayerEvent fpe                                
                LEFT JOIN #PlayerKeys pk                                
                    ON pk.PlayerKey = fpe.playerKey              
            WHERE fpe.dateKey >= @StartDate --Some of the Opens playerkeys don't appear in factrevenue                                                        
                  AND fpe.dateKey < @EndDate                                
                  AND pk.PlayerKey IS NULL;                                
                                
            CREATE INDEX PlayerKey_01 ON #PlayerKeys (PlayerKey);                                
            CREATE INDEX PlayerKey_02                                
            ON #PlayerKeys (                                
       DateKey,                                
                               PlayerKey,                                
    IncomeCurrencyValue                                
                           );        

	 INSERT INTO #DimPlayer                                
            (                                
                PlayerKey,                                
                Hist_PTSGamingserverID,                                
                Hist_PTSCasinoID,                                
                Hist_PTSUserID,                                
                Country,                                
    StateOrProvince,                  
                PlayerType                                
            )                                
            SELECT DISTINCT                                
                   pk.PlayerKey,                                
                   ISNULL(dp.Hist_PTSGamingserverID, 0),                                
                   ISNULL(dp.Hist_PTSCasinoID, 0),                                
                   ISNULL(dp.Hist_PTSUserID, 0),                                
                   ISNULL(dp.Country, 'Unknown'),                              
       ISNULL (dp.[State],'Unknown'),                  
                   ISNULL(dp.PlayerType, 'Unknown')                                
            FROM dbDWAlignment.dbo.dimPlayer dp                                
                JOIN #PlayerKeys pk                                
                    ON dp.playerKey = pk.PlayerKey                                
              LEFT JOIN [dbFinanceReporting].[dbo].[PlayerTypeExclusions] pte --this table includes a list of playertypeIDs that should be excluded from reporting                                                            
                    ON dp.PlayerType = pte.PlayerTypeDescription        
    LEFT JOIN dbBISApplication.dbo.tbl_TestAccounts AS TTA WITH (NOLOCK)     
    ON TTA.GamingserverID = DP.Hist_PTSGamingserverID     
    AND TTA.UserID = DP.Hist_PTSUserID    
            WHERE pte.PlayerTypeID IS NULL                  
       AND TTA.UserID IS NULL    

	     CREATE INDEX DimPlayer_01                                
            ON #DimPlayer (                                
                              PlayerKey,                                
            Hist_PTSGamingserverID,                                
                              Hist_PTSCasinoID                                
                          )        
            INCLUDE (                                
                        Country,                                
      StateOrProvince,                  
                        PlayerType                                
                    );                                
            CREATE INDEX DimPlayer_02                                
            ON #DimPlayer (                                
                              Hist_PTSGamingserverID,                                
                              Hist_PTSUserID                                
                          )                                
         INCLUDE (Country);     
  IF OBJECT_ID('Tempdb.dbo.#factPlayerAdjustment') IS NOT NULL                                
        DROP TABLE #factPlayerAdjustment;                                
   
  SELECT * INTO     #factPlayerAdjustment  
   FROM  dbDWAlignment.dbo.factPlayerAdjustment a                     
   WHERE a.dateKey >=   @StartDate                 
 AND  a.dateKey < @EndDate                
 AND     a.PTSEventID NOT IN (  12239, 12240,68353,66623,38792,38795,72821,72822,72906,72908,73302 ) --   BGN ADMINEVENT EXCLUSIONS    
 AND	a.PTSEventID NOT IN (  60900,52554,23969,53241,68063,52534,52555,53239,72907,73302,38795 )   --   BGN ADMINEVENT EXCLUSIONS 2.0
      
       

SELECT DISTINCT                                
                       ISNULL(dcd.SortOrder, -1),                                
                       ISNULL(dcd.FinGroup, 'Unknown'),                                
                       dcd.Hist_PTSGamingserverID,                                
                       dcd.Hist_PTSCasinoID,                                
                       ISNULL(dcd.BetwayGroup, 'Unknown'),                                
                       ISNULL(dcd.BetwayBrand, 'Unknown'),                        
                       ISNULL(dcd.BetwayOwner, 'Unknown'),                                
                       ISNULL(dcd.Product, 'Unknown'),                                
                       ISNULL(dp.Country, 'Unknown'),                            
              ISNULL (dp.StateOrProvince,'Unknown'),                  
                  
                       SUM(   CASE                                
                                  WHEN b.AdminCategory = 'Bonus-Cash Items' THEN          
                    ISNULL(AdjustmentCurrencyValue, 0) * fcc.SourceDestinationExchRate                                
                                  --    END                                  
                                  ELSE                                
                                      0                                
                              END                                
                          ) BCI,                                
                       SUM(   CASE                                
      WHEN b.AdminCategory = 'Non-Cash Item' THEN                                
                                      ISNULL(AdjustmentCurrencyValue, 0) * fcc.SourceDestinationExchRate                             
                                  --END                                  
                                  ELSE                                
                                      0                                
                              END                                
                          ) NCI,                                
                       SUM(   CASE                                
                                  WHEN b.AdminCategory = 'Cash Item' THEN             
                                      ISNULL(AdjustmentCurrencyValue, 0) * fcc.SourceDestinationExchRate             
                                  ELSE                                
                          0                                
                              END                                
                          ) CI,                                
                       SUM(   CASE                                
                                  WHEN b.AdminCategory = 'Other Gaming Income' THEN                                
                                                                         ISNULL(AdjustmentCurrencyValue, 0) * fcc.SourceDestinationExchRate                                
                                  ELSE                                
                  0                                
                              END                                
                          ) OGI,                                
                       SUM(   CASE                                
							WHEN b.AdminCategory = 'Best Odds Guarentee' THEN                                
                                      --CASE                                  
                                      ISNULL(AdjustmentCurrencyValue, 0) * fcc.SourceDestinationExchRate                                
                                  --END                     
                                  ELSE                                
                         0                                
                              END                                
                          ) as BOG,                                
                       SUM(   CASE                                
                                  WHEN b.AdminType = 'Resettlement' THEN                                
                                      ISNULL(AdjustmentCurrencyValue, 0) * fcc.SourceDestinationExchRate                                
                                  ELSE                                
                                      0                                
                              END                                
                          )      AS Resettlement                         
                --select top 10 *                                  
                FROM dbDWAlignment.dbo.factPlayerAdjustment a                                
                    JOIN #DimPlayer dp                                      
                    --JOIN dbDWAlignment.dbo.dimPlayer dp                                
                        ON dp.playerKey = a.playerKey --dbdwalignment.dbo.dimPlayer                                                            
                    JOIN dbDWAlignment.dbo.dimPlayerAdjustment b                                
                        ON a.playerAdjustmentKey = b.playerAdjustmentKey                                
                    JOIN dbDWAlignment.dbo.dimCasinoDetail_New dcd                         
                        ON dp.Hist_PTSGamingserverID = dcd.Hist_PTSGamingserverID                                
                           AND dp.Hist_PTSCasinoID = dcd.Hist_PTSCasinoID                                
                    INNER JOIN dbdwalignment.dbo.factCurrencyConversion fcc                                                           
                        ON a.dateKey = fcc.ConversionDateKey                                
                           AND a.currencyKey = fcc.SourceCurrencyKey                                
                           AND fcc.DestinationCurrencyKey = 27                                
                WHERE a.dateKey >= @StartDate AND a.dateKey < @EndDate                                
                      AND b.AdminCategory IN ( 'Bonus-Cash Items', 'Non-Cash Item', 'Cash Item', 'Other Gaming Income',                                
                                               'Best Odds Guarentee' ) --BCI, NCI, CI, OGI, BOG                                                              
                      AND dcd.Product <> 'Pay2Play'                                
                      AND dcd.BetwayGroup <> 'BW Africa'                                
					  AND dcd.BetwayBrand <> '1X1'                                
                      AND a.PTSEventID <> 18                  
					  AND dcd.Hist_PTSGamingserverID = 312      
					AND dcd.Hist_PTSCasinoID in ( 42499, 42498 )   
                GROUP BY ISNULL(dcd.SortOrder, -1),                          
                         ISNULL(dcd.FinGroup, 'Unknown'),                                
                         ISNULL(dcd.BetwayBrand, 'Unknown'),                                
                         ISNULL(dcd.BetwayGroup, 'Unknown'),                                
                         ISNULL(BetwayOwner, 'Unknown'),                                
                         ISNULL(dcd.Product, 'Unknown'),                                
                         Country,                                
				ISNULL (dp.StateOrProvince,'Unknown'),    
                         dcd.Hist_PTSGamingserverID,                                
                         dcd.Hist_PTSCasinoID;               