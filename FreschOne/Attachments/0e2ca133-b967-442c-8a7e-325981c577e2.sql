
SELECT  [Document No], sum ( Amount ), OperatorGroup FROM Revenue_Log WHERE OperatorGroup = 'SIXGAMING' and [Posting Date] >= '2025-02-01' and [Posting Date] < '2025-03-01'
GROUP BY [Document No], OperatorGroup


SELECT [Posting Date], SUM ( Amount ) FROM Cashin_Log WHERE OperatorGroup = 'SIXGAMING'  and [Posting Date] >= '2025-02-01' and [Posting Date] < '2025-03-01' --and left ( [Document No], 3 )  = 'PAY'
GROUP BY  [Posting Date]


SELECT   sum ( Amount ), OperatorGroup FROM adjustment_Log WHERE OperatorGroup = 'SIXGAMING'  and [Posting Date] = '2025-02-01' and [Posting Date] < '2025-03-01' --and left ( [Document No], 3 )  = 'PAY'
GROUP BY  OperatorGroup


SELECT   sum ( Amount ), OperatorGroup FROM ALLOCATION_Log WHERE OperatorGroup = 'SIXGAMING'  and [Posting Date] >= '2025-02-01' and [Posting Date] < '2025-03-01' --and left ( [Document No], 3 )  = 'PAY'
GROUP BY  OperatorGroup


SELECT   sum ( Amount ), OperatorGroup FROM CASHINTXFEE_Log WHERE OperatorGroup = 'SIXGAMING'  and [Posting Date] >= '2025-02-01' and [Posting Date] < '2025-03-01' --and left ( [Document No], 3 )  = 'PAY'
GROUP BY  OperatorGroup


SELECT   sum ( Amount ), OperatorGroup FROM PURCHASE_Log WHERE OperatorGroup = 'SIXGAMING'  and [Posting Date] >= '2025-02-01' and [Posting Date] < '2025-03-01' --and left ( [Document No], 3 )  = 'PAY'
GROUP BY  OperatorGroup





select *from vw_CashinDetails where datekey = 20250226