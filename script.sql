
DECLARE @ProcessInstanceID BIGINT = 50
declare @pkid int = 161
 

 
--select *from foStepHist where ProcessInstanceID = @ProcessInstanceID

--select *from foDecisions
select *from tbl_tran_Student where id = @pkid order by 1 desc
select *from tbl_tran_StudentDetails1 where studentid = @pkid --order by 1 desc
--select *from tbl_tran_StudentDetails2 where studentid = @pkid order by 1 desc
--select *from tbl_tran_StudentDetails3 where studentid = @pkid order by 1 desc

				-- SELECT *FROM foApprovalHist WHERE ProcessInstanceID = @ProcessInstanceIDv
				 --SELECT *FROM foApproval	 WHERE ProcessInstanceID = @ProcessInstanceID 
				-- SELECT *fROM foApprovalSteps