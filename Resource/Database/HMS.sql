USE [HMS]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetTotalWorkingDaysAccordingToShift]    Script Date: 9/6/2024 5:45:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- select * from fn_GetTotalWorkingDaysAccordingToShift (2190,'68235,68236','2021-03-01','2021-03-31')
create FUNCTION [dbo].[fn_GetTotalWorkingDaysAccordingToShift]
(@CompanyID Numeric(18),
@EmployeeIds   Varchar(Max),
@MonthStartDate date,
@MonthEndDate date)
RETURNS  @Result TABLE 
		(
			Employeeid  NUMERIC(18, 0),
			ComanyID NUMERIC(18, 0),			
			TotWorkingDays int
		)

BEGIN
declare @tblEmployeeIds Table
		(
		EmployeeIDS  NUMERIC(18, 0)
		)
		declare @tempShift Table
		(
		EmployeeID Numeric(18,0),
		shiftID Numeric(18,0),
	    WDMonday bit,
		WDTuesday bit,
		WDWednesday bit,
		WDThursday bit,
		WDFriday bit,
		WDSatuday bit,
		WDSunday bit
		)

insert into @tblEmployeeIds(EmployeeIDS)
Select		ID 
From		FNC_Split(@EmployeeIds,',')


Insert into @tempShift
  SELECT distinct mf.ID 'EmployeeID', sh.ID 'shiftID',sh.WDMonday,sh.WDTuesday,sh.WDWednesday,sh.WDThursday,sh.WDFriday,sh.WDSatuday,sh.WDSunday
  from pr_employee_mf mf inner join pr_employee_shift  sh on mf.ShiftID=sh.ID    
  where mf.CompanyID=@CompanyID and Exists (Select 1 from @tblEmployeeIds d where d.EmployeeIDS = mf.ID)


DECLARE @DateFrom DATETIME
DECLARE @DateTo DATETIME
DECLARE @cnt INT = 0;
Declare @tillCount as int;
Declare @ID as int;
 set @tillCount=(select Distinct Count(EmployeeIDS) from @tblEmployeeIds)
 WHILE @cnt < @tillCount
 BEGIN
SET @DateFrom = @MonthStartDate
--'2020/09/01'
SET @DateTo =@MonthEndDate
-- '2020/09/30'

declare @Monday as varchar(10)
declare @Tuesday as varchar(10)
declare @Wednesday as varchar(10)
declare @Thursday as varchar(10)
declare @Friday as varchar(10)
declare @Saturday as varchar(10)
declare @Sunday as varchar(10)
set @Monday= (select case when WDMonday=1 then 'Monday' else 'NoMonday' end  from @tempShift where EmployeeID In (select Top 1 EmployeeIDS from @tblEmployeeIds) ) 
set @Tuesday=(select case when WDTuesday=1 then 'Tuesday' else 'NoTuesday' end  from @TempShift where EmployeeID In (select Top 1 EmployeeIDS from @tblEmployeeIds)  ) 
set @Wednesday=(select case when WDWednesday=1 then 'Wednesday' else 'NoWednesday' end  from @TempShift where EmployeeID In (select Top 1 EmployeeIDS from @tblEmployeeIds) ) 
set @Thursday=(select case when WDThursday=1 then 'Thursday' else 'NoThursday' end  from @TempShift where EmployeeID In (select Top 1 EmployeeIDS from @tblEmployeeIds)  ) 
set @Friday=(select case when WDFriday=1 then 'Friday' else 'NoFriday' end  from @TempShift where EmployeeID In (select Top 1 EmployeeIDS from @tblEmployeeIds) ) 
set @Saturday=(select case when WDSatuday=1 then 'Saturday' else 'NoSaturday' end  from @TempShift where EmployeeID In (select Top 1 EmployeeIDS from @tblEmployeeIds) ) 
set @Sunday=(select case when WDSunday=1 then 'Sunday' else 'NoSunday' end  from @TempShift where EmployeeID In (select Top 1 EmployeeIDS from @tblEmployeeIds)  ) 

    DECLARE @TotWorkingDays INT= 0;
         WHILE @DateFrom <= @DateTo
		             BEGIN
					
                 IF DATENAME(WEEKDAY, @DateFrom) IN( @Monday, @Tuesday, @Wednesday, @Thursday, @Friday,@Saturday,@Sunday)
                     BEGIN
                         SET @TotWorkingDays = @TotWorkingDays + 1;
                 END;
                 SET @DateFrom = DATEADD(DAY, 1, @DateFrom);
             END;
       
	Insert Into @Result (Employeeid,ComanyID,TotWorkingDays)
select (select Top 1 EmployeeIDS from @tblEmployeeIds),@CompanyID,@TotWorkingDays
		 
   SET @cnt = @cnt + 1;
   Delete Top(1) from @tblEmployeeIds
END;


RETURN
END





GO
/****** Object:  UserDefinedFunction [dbo].[FNC_Split]    Script Date: 9/6/2024 5:45:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create FUNCTION [dbo].[FNC_Split](
    @sInputList VARCHAR(8000) -- List of delimited items
  , @sDelimiter VARCHAR(8000) = ',' -- delimiter that separates items
) RETURNS @List TABLE (ID VARCHAR(8000))

BEGIN
DECLARE @sItem VARCHAR(8000)
WHILE CHARINDEX(@sDelimiter,@sInputList,0) <> 0
 BEGIN
 SELECT
  @sItem=RTRIM(LTRIM(SUBSTRING(@sInputList,1,CHARINDEX(@sDelimiter,@sInputList,0)-1))),
  @sInputList=RTRIM(LTRIM(SUBSTRING(@sInputList,CHARINDEX(@sDelimiter,@sInputList,0)+LEN(@sDelimiter),LEN(@sInputList))))
 
 IF LEN(@sItem) > 0
  INSERT INTO @List SELECT @sItem
 END

IF LEN(@sInputList) > 0
 INSERT INTO @List SELECT @sInputList -- Put the last item in
RETURN
END






GO
/****** Object:  UserDefinedFunction [dbo].[ParseCommaStringToInt]    Script Date: 9/6/2024 5:45:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[ParseCommaStringToInt]
( @CommaSeparatedStr nvarchar(1000) =NULL)
RETURNS @myTable TABLE ([Id] [int] NOT NULL)
AS
BEGIN
declare @pos int
declare @piece varchar(500)
if right(rtrim(@CommaSeparatedStr ),1) <> ','
set @CommaSeparatedStr = @CommaSeparatedStr + ','
set @pos = patindex('%,%' , @CommaSeparatedStr )
while @pos <> 0
begin
set @piece = left(@CommaSeparatedStr , @pos-1)
insert @myTable
select @piece

set @CommaSeparatedStr = stuff(@CommaSeparatedStr , 1, @pos, '')
set @pos = patindex('%,%' , @CommaSeparatedStr )
end
RETURN
END

GO
/****** Object:  UserDefinedFunction [dbo].[ufn_PR_CalculateLateArrivalDeduction]    Script Date: 9/6/2024 5:45:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  FUNCTION [dbo].[ufn_PR_CalculateLateArrivalDeduction] (@CompanyID Numeric(18),@EmployeeIds   Varchar(Max),
@MonthStartDate date,
@MonthEndDate date)
RETURNS @Result TABLE 
		(
			Employeeid  NUMERIC(18, 0),
			ComanyID NUMERIC(18, 0),
			DeductLateAmount NUMERIC(18, 0)
		)
AS
begin
	declare @tempShift Table
		(
		EmployeeID Numeric(18,0),
		shiftID Numeric(18,0),
	    WDMonday bit,
		WDTuesday bit,
		WDWednesday bit,
		WDThursday bit,
		WDFriday bit,
		WDSatuday bit,
		WDSunday bit
		)

		declare @tbEmployeeAllDates Table
		(
		Employeeid  NUMERIC(18, 0),
		ComanyID NUMERIC(18, 0),
		DeductDays NUMERIC(18, 0)
		)
		declare @tbEmployeeSalery Table
		(
		Employeeid  NUMERIC(18, 0),
		ComanyID NUMERIC(18, 0),
		BasicSalary NUMERIC(18, 0),
		GrossSalary NUMERIC(18, 0),
		PenaltyRange NUMERIC(18, 0)
		)


		declare @tblEmployeeIds Table
		(
		EmployeeIDS  NUMERIC(18, 0)
		)
		declare @tblEmployeeIds1 Table
		(
		EmployeeIDS  NUMERIC(18, 0)
		)
		declare @mintimein Table
		(
		TimeIn  datetime,
		EmployeeID numeric(18,0),
		AttendanceDate date
		)

	declare @SalaryMethodID as int
	set @SalaryMethodID=(select SalaryMethodID from adm_company where ID=@CompanyID)
		
insert into @tblEmployeeIds(EmployeeIDS)
Select		ID 
From		FNC_Split(@EmployeeIds,',')

insert into @tblEmployeeIds1(EmployeeIDS)
Select		ID 
From		FNC_Split(@EmployeeIds,',') 


declare @OfficeTime time
   set @OfficeTime=(select top 1 convert(varchar(8),dateadd(minute,IsNull(rulemf.LA_GracePeriod,0),_shift.StartTime),108)
				from pr_employee_shift _shift
				inner join pr_time_rule_mf rulemf on rulemf.CompanyID=_shift.CompanyID
				where _shift.CompanyID=@CompanyID)
Insert Into @mintimein(TimeIn,EmployeeID,AttendanceDate)
select min(TimeIn) TimeIn,EmployeeID,AttendanceDate from pr_time_entry 
where CompanyID =@CompanyID and AttendanceDate between @MonthStartDate and @MonthEndDate
group by AttendanceDate, EmployeeID


 Insert Into @tbEmployeeAllDates(employeeid,ComanyID,DeductDays)
	select EmployeeID,CompanyID,
	floor(CAST((CAST(case when isnull(LA_MaximumDayLate,0)<=lateCount then lateCount else 0 end AS float)/CAST(LA_MaximumDayLate as float))as float)*LA_IncidentsLeaveDays) as DeductDays
		from(
					select lo.CompanyID,lo.EmployeeID,count(lo.EmployeeID) as lateCount ,mf.LA_MaximumDayLate,LA_IncidentsLeaveDays
					from pr_time_summary lo
					inner join @tblEmployeeIds empids on empids.EmployeeIDS=lo.EmployeeID
					inner join pr_employee_mf employeemf on employeemf.ID=lo.EmployeeID and employeemf.CompanyID=lo.CompanyID
					inner join pr_time_rule_mf mf on mf.CompanyID=lo.CompanyID
					inner join pr_employee_shift sh on sh.ID=employeemf.ShiftID
					where CONVERT(VARCHAR(8), convert(varchar(8),dateadd(minute,IsNull(mf.LA_GracePeriod,0),sh.StartTime),108), 108) < CONVERT(VARCHAR(8), lo.TimeIn, 108) 
					--where CONVERT(VARCHAR(8), @OfficeTime, 108) < CONVERT(VARCHAR(8), lo.TimeIn, 108) 
					and lo.CompanyID=@CompanyID
					and lo.AttendanceDate between @MonthStartDate and @MonthEndDate
					and employeemf.TimeRule=1 and employeemf.LastArrival=1
					group by lo.EmployeeID,lo.CompanyID,mf.LA_MaximumDayLate,LA_IncidentsLeaveDays
					)tab


					
Declare @GrasePenalrty NUMERIC(18, 2),
@StandardPenaltyCharges NUMERIC(18, 2),
@StandardPenaltyMinute NUMERIC(18, 2),
@SpecialPenaltyMinute NUMERIC(18, 2),
@SpecialPenaltyCharges NUMERIC(18, 2),
@SpecialRestPenaltyMinute NUMERIC(18, 2),
@SpecialRestPenaltyCharges NUMERIC(18, 2),
@PenaltyType bit,
@LateDeductionType bit,
@PenaltySalaryRange numeric(18,0),
@TotalPenaltyCharges NUMERIC(18, 2)

select @GrasePenalrty = isnull(LA_GracePeriod,0)  from pr_time_rule_mf where CompanyID = @CompanyID
 select @PenaltyType = PenaltyType,@LateDeductionType = LateDeductionHourType,@PenaltySalaryRange=IsNull(PenaltySalaryRange,0) from pr_time_rule_mf where CompanyID=@CompanyID

if(@SalaryMethodID=1 or @SalaryMethodID is null)
Begin	
-- this one calculate deduction only on the basic salay
Insert Into @tbEmployeeSalery(employeeid,ComanyID,BasicSalary,GrossSalary,PenaltyRange)
select ID,CompanyID,BasicSalary,(GrossSalary/DATEPART(day,EOMONTH(@MonthStartDate)))as GrossSalary,(GrossSalary/DATEPART(day,EOMONTH(@MonthStartDate))) *(@PenaltySalaryRange/100)
		from
		(
			select (empMf.BasicSalary)as GrossSalary,empMf.BasicSalary,empMf.ID,empMf.CompanyID
			from pr_employee_mf empMf
			left join pr_employee_allowance allowance on allowance.CompanyID=empMf.CompanyID and allowance.EmployeeID=empMf.ID 
			where empMf.CompanyID =@CompanyID 
			and empMf.StatusDropDownID = 7 and empMf.StatusID = 1 and Not empMf.JoiningDate is Null
			and empMf.TimeRule=1
			group by empMf.BasicSalary,empMf.ID,empMf.CompanyID
		)tab
		
		-- Commented code calculate deduction in Gross Salary= Basic Salary+Allowances

--		Insert Into @tbEmployeeSalery(employeeid,ComanyID,BasicSalary,GrossSalary,PenaltyRange)
--select ID,CompanyID,BasicSalary,allowance,(GrossSalary/DATEPART(day,EOMONTH(GETDATE())))as GrossSalary,(GrossSalary/DATEPART(day,EOMONTH(GETDATE()))) *(0/100)
--		from
--		(
--			select (empMf.BasicSalary + Isnull(sum(allowance.Amount),0))as GrossSalary,Isnull(sum(allowance.Amount),0) 'allowance',empMf.BasicSalary,empMf.ID,empMf.CompanyID
--			from pr_employee_mf empMf
--			left join pr_employee_allowance allowance on allowance.CompanyID=empMf.CompanyID and allowance.EmployeeID=empMf.ID 
--			where empMf.CompanyID =1230 
--			and empMf.StatusDropDownID = 7 and empMf.StatusID = 1 and Not empMf.JoiningDate is Null
--			and empMf.TimeRule=1
--			group by empMf.BasicSalary,empMf.ID,empMf.CompanyID
--		)tab

--drop table #temp
End

if(@SalaryMethodID=2)
Begin	

Insert into @tempShift
  SELECT distinct mf.ID 'EmployeeID', sh.ID 'shiftID',sh.WDMonday,sh.WDTuesday,sh.WDWednesday,sh.WDThursday,sh.WDFriday,sh.WDSatuday,sh.WDSunday
  from pr_employee_mf mf inner join pr_employee_shift  sh on mf.ShiftID=sh.ID    
  where mf.CompanyID=@CompanyID and Exists (Select 1 from @tblEmployeeIds1 d where d.EmployeeIDS = mf.ID)


DECLARE @DateFrom DATETIME
DECLARE @DateTo DATETIME
DECLARE @cnt INT = 0;
Declare @tillCount as int;
Declare @ID as int;
 set @tillCount=(select Distinct Count(EmployeeIDS) from @tblEmployeeIds)
 WHILE @cnt < @tillCount
 BEGIN
SET @DateFrom = @MonthStartDate
--'2020/09/01'
SET @DateTo =@MonthEndDate
-- '2020/09/30'

declare @Monday as varchar(10)
declare @Tuesday as varchar(10)
declare @Wednesday as varchar(10)
declare @Thursday as varchar(10)
declare @Friday as varchar(10)
declare @Saturday as varchar(10)
declare @Sunday as varchar(10)
set @Monday= (select case when WDMonday=1 then 'Monday' else 'NoMonday' end  from @tempShift where EmployeeID In (select Top 1 EmployeeIDS from @tblEmployeeIds1) ) 
set @Tuesday=(select case when WDTuesday=1 then 'Tuesday' else 'NoTuesday' end  from @TempShift where EmployeeID In (select Top 1 EmployeeIDS from @tblEmployeeIds1)  ) 
set @Wednesday=(select case when WDWednesday=1 then 'Wednesday' else 'NoWednesday' end  from @TempShift where EmployeeID In (select Top 1 EmployeeIDS from @tblEmployeeIds1) ) 
set @Thursday=(select case when WDThursday=1 then 'Thursday' else 'NoThursday' end  from @TempShift where EmployeeID In (select Top 1 EmployeeIDS from @tblEmployeeIds1)  ) 
set @Friday=(select case when WDFriday=1 then 'Friday' else 'NoFriday' end  from @TempShift where EmployeeID In (select Top 1 EmployeeIDS from @tblEmployeeIds1) ) 
set @Saturday=(select case when WDSatuday=1 then 'Saturday' else 'NoSaturday' end  from @TempShift where EmployeeID In (select Top 1 EmployeeIDS from @tblEmployeeIds1) ) 
set @Sunday=(select case when WDSunday=1 then 'Sunday' else 'NoSunday' end  from @TempShift where EmployeeID In (select Top 1 EmployeeIDS from @tblEmployeeIds1)  ) 

    DECLARE @TotWorkingDays INT= 0;
         WHILE @DateFrom <= @DateTo
		             BEGIN
					
                 IF DATENAME(WEEKDAY, @DateFrom) IN( @Monday, @Tuesday, @Wednesday, @Thursday, @Friday,@Saturday,@Sunday)
                     BEGIN
                         SET @TotWorkingDays = @TotWorkingDays + 1;
                 END;
                 SET @DateFrom = DATEADD(DAY, 1, @DateFrom);
             END;
       
		Insert Into @tbEmployeeSalery(employeeid,ComanyID,BasicSalary,GrossSalary,PenaltyRange)
        select ID,CompanyID,BasicSalary,case when @TotWorkingDays>0 then (GrossSalary/@TotWorkingDays) end as GrossSalary, 
		case when @TotWorkingDays>0 then (GrossSalary/ @TotWorkingDays) *(@PenaltySalaryRange/100) end 
		from
		(
			select empMf.BasicSalary as GrossSalary,empMf.BasicSalary,empMf.ID,empMf.CompanyID
			from pr_employee_mf empMf
			left join pr_employee_allowance allowance on allowance.CompanyID=empMf.CompanyID and allowance.EmployeeID=empMf.ID 
			where empMf.CompanyID =@CompanyID 
			and empMf.StatusDropDownID = 7 and empMf.StatusID = 1 and Not empMf.JoiningDate is Null
			and empMf.TimeRule=1 and empMf.ID in (select Top 1 EmployeeIDS from @tblEmployeeIds1) 
			group by empMf.BasicSalary,empMf.ID,empMf.CompanyID
		)tab
		    
   SET @cnt = @cnt + 1;
   Delete Top(1) from @tblEmployeeIds1
END;
END


declare @tbAllEmployeePenalties Table
		(
		total NUMERIC(18, 2),
		StartTime time,
		TimeIn time,
		realdiff NUMERIC(18, 2),
		TimeDiff  NUMERIC(18, 2),
		EmployeeID int,
		temp NUMERIC(18, 2)
		)

insert into @tbAllEmployeePenalties(total,StartTime,TimeIn,realdiff,TimeDiff,EmployeeID,temp)
 select 0,cast(min(s.StartTime) as time), CAST(min(PTE.TimeIn) as time),
DateDiff(MINUTE,cast(min(s.StartTime) as time),CAST(min(PTE.TimeIn) as time)),
case when DateDiff(MINUTE,cast(min(s.StartTime) as time),CAST(min(PTE.TimeIn) as time)) <= 0 then 0 else DateDiff(MINUTE,cast(min(s.StartTime) as time),CAST(min(PTE.TimeIn) as time)) - @GrasePenalrty end,
PTE.EmployeeID,0
from pr_employee_shift s 
inner join pr_employee_mf m
on m.ShiftID = s.ID
inner join pr_time_entry PTE
on m.ID = PTE.EmployeeID
inner join @tblEmployeeIds empids 
on empids.EmployeeIDS=m.ID
inner join @mintimein t
on t.EmployeeID = PTE.EmployeeID and t.AttendanceDate = PTE.AttendanceDate and t.TimeIn = PTE.TimeIn
where m.CompanyID=@CompanyID and (PTE.AttendanceDate between @MonthStartDate and @MonthEndDate)
group by PTE.AttendanceDate,PTE.EmployeeID

update @tbAllEmployeePenalties
set TimeDiff = 0
where TimeDiff <0


if(@PenaltyType = 1)
Begin
	select @StandardPenaltyCharges = isnull(StandardPenaltyCharges,0),@StandardPenaltyMinute = isnull(StandardPenaltyMinute,0) 
	from pr_time_rule_mf where CompanyID=@CompanyID
	update @tbAllEmployeePenalties
	set total = case When @StandardPenaltyMinute>0 Then (TimeDiff/@StandardPenaltyMinute) * @StandardPenaltyCharges END

	update @tbAllEmployeePenalties
	set total = PenaltyRange
	from @tbAllEmployeePenalties t
	inner join @tbEmployeeSalery s
	on s.employeeid = t.EmployeeID
	where total > PenaltyRange
end
if(@PenaltyType = 0)
Begin

	select @SpecialPenaltyCharges = isnull(SpecialPenaltyCharges,0),@SpecialPenaltyMinute=isnull(SpecialPenaltyMinute,0),
	@SpecialRestPenaltyCharges = isnull(SpecialRestPenaltyCharges,0),@SpecialRestPenaltyMinute=isnull(SpecialRestPenaltyMinute,0)  
	from pr_time_rule_mf where	CompanyID=@CompanyID
	update @tbAllEmployeePenalties
	set temp =
	(
		case when TimeDiff < @SpecialPenaltyMinute then TimeDiff else @SpecialPenaltyMinute end
	)
	where TimeDiff >0
	
	
	update @tbAllEmployeePenalties
	set total = total + (@SpecialPenaltyCharges * temp)
	where TimeDiff >0
	
	update @tbAllEmployeePenalties
	set total = 
	(
		case when TimeDiff < @SpecialPenaltyMinute then  total else total + ((TimeDiff - @SpecialPenaltyMinute)* @SpecialRestPenaltyCharges) end
	)
	where TimeDiff >0 --and  TimeDiff > @SpecialPenaltyCharges
	
	update @tbAllEmployeePenalties
	set total = PenaltyRange
	from @tbAllEmployeePenalties t
	inner join @tbEmployeeSalery s
	on s.employeeid = t.EmployeeID
	where total > PenaltyRange
end

Insert Into @Result (Employeeid,ComanyID,DeductLateAmount)
select sal.Employeeid,sal.ComanyID, case when @LateDeductionType= 1 then (sal.GrossSalary*emp.DeductDays) else  isnull(t.total,0) end   as DeductLateAmount --(sal.GrossSalary*emp.DeductDays) -
 from @tbEmployeeAllDates emp
inner join @tbEmployeeSalery sal on sal.employeeid=emp.employeeid and sal.ComanyID=emp.ComanyID
left Join	(
				Select	sum(total) total,EmployeeID EmployeeID
				From @tbAllEmployeePenalties 
				Group By EmployeeID
			) t On t.EmployeeID = sal.employeeid

Return

end



GO
/****** Object:  UserDefinedFunction [dbo].[ufn_PR_CalculateLeaveRestrictionDeduction]    Script Date: 9/6/2024 5:45:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- select * from ufn_PR_CalculateLeaveRestrictionDeduction(13,'71','20201001','20201031')
create  FUNCTION [dbo].[ufn_PR_CalculateLeaveRestrictionDeduction] (@CompanyID Numeric(18),@EmployeeIds   Varchar(Max),
@MonthStartDate date,
@MonthEndDate date)
RETURNS @Result TABLE 
		(
			Employeeid  NUMERIC(18, 0),
			ComanyID NUMERIC(18, 0),
			DeductRestrictionAmount NUMERIC(18, 0)
		)
AS
begin
	declare @tempShift Table
		(
		EmployeeID Numeric(18,0),
		shiftID Numeric(18,0),
	    WDMonday bit,
		WDTuesday bit,
		WDWednesday bit,
		WDThursday bit,
		WDFriday bit,
		WDSatuday bit,
		WDSunday bit
		)
declare @tbAculalEmployeeWorking Table
		(
		CompanyID  NUMERIC(18, 0),
		EmployeeID NUMERIC(18, 0),
		WorkPercentage NUMERIC(18, 0),
		GrossSalary NUMERIC(18, 0),
		daysDeduct NUMERIC(18, 0),
		LR_IgonreRule bit
		)
		declare @tbEmployeeWorkingDays Table
		(
		EmpID NUMERIC(18, 0),
		CID  NUMERIC(18, 0),
		LR_MissingHourPercentage NUMERIC(18, 0),
		DeductDays NUMERIC(18, 0)
		)
		declare @tblEmployeeIds Table
		(
		EmployeeIDS  NUMERIC(18, 0)
		)
		declare @tblEmployeeIds1 Table
		(
		EmployeeIDS  NUMERIC(18, 0)
		)

insert into @tblEmployeeIds(EmployeeIDS)
Select		ID 
From		FNC_Split(@EmployeeIds,',')

insert into @tblEmployeeIds1(EmployeeIDS)
Select		ID 
From		FNC_Split(@EmployeeIds,',')

declare @SalaryMethodID as int
	set @SalaryMethodID=(select SalaryMethodID from adm_company where ID=@CompanyID)

		insert into @tbAculalEmployeeWorking(CompanyID,EmployeeID,WorkPercentage,GrossSalary,daysDeduct,LR_IgonreRule)
select distinct CompanyID,EmployeeID,Round(cast(TotalEmpWorkingMints as float)/cast(ActualTotalWorkMint as float)*100,0) as WorkPercentage,GrossSalary,0 daysDeduct,LR_IgonreRule
from
(
Select CompanyID,EmployeeID
	,Sum(PerDayWorking) Over(PARTITION BY EmployeeID) TotalEmpWorkingMints,(WorkingHour*DATEPART(day,EOMONTH(@MonthStartDate))*60+mint)ActualTotalWorkMint,GrossSalary,LR_IgonreRule
	From
	(
		select pr_time.EmployeeID,pr_time.CompanyID,
		DATEDIFF(minute, [TimeIn],[TimeOut]) AS PerDayWorking,(SUBSTRING(_shift.WorkingHour,1,2))WorkingHour,SUBSTRING(_shift.WorkingHour,4,5)mint,
		--(empMf.BasicSalary + Isnull(sum(allowance.Amount),0))as GrossSalary	
		empMf.BasicSalary as GrossSalary,
		rulemf.LR_IgonreRule
		from pr_employee_mf empMf 
		inner join pr_time_Summary pr_time on pr_time.EmployeeID=empMf.ID and pr_time.CompanyID=empMf.CompanyID
		inner join @tblEmployeeIds empids on empids.EmployeeIDS=pr_time.EmployeeID
		inner join pr_time_rule_mf rulemf on rulemf.CompanyID=empMf.CompanyID
		 left join pr_employee_allowance allowance on allowance.CompanyID=empMf.CompanyID and allowance.EmployeeID=empMf.ID 
		inner join pr_employee_shift _shift on _shift.CompanyID=empMf.CompanyID and _shift.ID=empMf.ShiftID
		where AttendanceDate between @MonthStartDate and @MonthEndDate  and pr_time.CompanyID=@CompanyID and pr_time.StatusID in(3,4)
		and empMf.TimeRule=1 and empMf.LeaveRestrictions=1
		group by pr_time.EmployeeID, pr_time.CompanyID,_shift.WorkingHour,pr_time.TimeIn,pr_time.TimeOut,empMf.BasicSalary,rulemf.LR_IgonreRule
	) dd
)ff



insert into @tbEmployeeWorkingDays(EmpID,CID,LR_MissingHourPercentage,DeductDays)
select EmpID,CID,LR_MissingHourPercentage,
	floor(CAST((CAST(lateCount AS float)/CAST(LR_MissingExceedDay as float))as float)*LR_MissingSwipeDays) as DeductDays
		from(
	select lo.CompanyID CID,lo.EmployeeID EmpID,count(lo.EmployeeID) as lateCount ,rulemf.LR_MissingSwipeDays,LR_MissingHourPercentage,LR_MissingExceedDay
					from pr_time_summary lo
					inner join pr_employee_mf empMf on empMf.id=lo.EmployeeID and empMf.CompanyID=lo.CompanyID
					inner join pr_time_rule_mf rulemf on rulemf.CompanyID=lo.CompanyID
					where lo.StatusID in(3,4)
					and lo.CompanyID=@CompanyID
					and lo.AttendanceDate between @MonthStartDate and @MonthEndDate
					and empMf.TimeRule=1 and empMf.LeaveRestrictions=1
					group by lo.EmployeeID,lo.CompanyID,rulemf.LR_MissingSwipeDays,rulemf.LR_MissingHourPercentage,rulemf.LR_MissingExceedDay
					)tab


update @tbAculalEmployeeWorking
set daysDeduct = ISNULL((select top 1 DeductDays from @tbEmployeeWorkingDays where CID = CompanyID
and EmpID=EmployeeID  and LR_MissingHourPercentage > WorkPercentage and LR_IgonreRule=1),0)
 if(@SalaryMethodID=1 or @SalaryMethodID is null)
Begin
Insert Into @Result (Employeeid,ComanyID,DeductRestrictionAmount)
select EmployeeID,CompanyID,GrossSalary/DATEPART(day,EOMONTH(@MonthStartDate))* daysDeduct as DeductAmount from @tbAculalEmployeeWorking
END



if(@SalaryMethodID=2)
Begin	

Insert into @tempShift
  SELECT distinct mf.ID 'EmployeeID', sh.ID 'shiftID',sh.WDMonday,sh.WDTuesday,sh.WDWednesday,sh.WDThursday,sh.WDFriday,sh.WDSatuday,sh.WDSunday
  from pr_employee_mf mf inner join pr_employee_shift  sh on mf.ShiftID=sh.ID    
  where mf.CompanyID=@CompanyID and Exists (Select 1 from @tblEmployeeIds1 d where d.EmployeeIDS = mf.ID)


DECLARE @DateFrom DATETIME
DECLARE @DateTo DATETIME
DECLARE @cnt INT = 0;
Declare @tillCount as int;
Declare @ID as int;
 set @tillCount=(select Distinct Count(EmployeeIDS) from @tblEmployeeIds)
 WHILE @cnt < @tillCount
 BEGIN
SET @DateFrom = @MonthStartDate
--'2020/09/01'
SET @DateTo =@MonthEndDate
-- '2020/09/30'

declare @Monday as varchar(10)
declare @Tuesday as varchar(10)
declare @Wednesday as varchar(10)
declare @Thursday as varchar(10)
declare @Friday as varchar(10)
declare @Saturday as varchar(10)
declare @Sunday as varchar(10)
set @Monday= (select case when WDMonday=1 then 'Monday' else 'NoMonday' end  from @tempShift where EmployeeID In (select Top 1 EmployeeIDS from @tblEmployeeIds1) ) 
set @Tuesday=(select case when WDTuesday=1 then 'Tuesday' else 'NoTuesday' end  from @TempShift where EmployeeID In (select Top 1 EmployeeIDS from @tblEmployeeIds1)  ) 
set @Wednesday=(select case when WDWednesday=1 then 'Wednesday' else 'NoWednesday' end  from @TempShift where EmployeeID In (select Top 1 EmployeeIDS from @tblEmployeeIds1) ) 
set @Thursday=(select case when WDThursday=1 then 'Thursday' else 'NoThursday' end  from @TempShift where EmployeeID In (select Top 1 EmployeeIDS from @tblEmployeeIds1)  ) 
set @Friday=(select case when WDFriday=1 then 'Friday' else 'NoFriday' end  from @TempShift where EmployeeID In (select Top 1 EmployeeIDS from @tblEmployeeIds1) ) 
set @Saturday=(select case when WDSatuday=1 then 'Saturday' else 'NoSaturday' end  from @TempShift where EmployeeID In (select Top 1 EmployeeIDS from @tblEmployeeIds1) ) 
set @Sunday=(select case when WDSunday=1 then 'Sunday' else 'NoSunday' end  from @TempShift where EmployeeID In (select Top 1 EmployeeIDS from @tblEmployeeIds1)  ) 

    DECLARE @TotWorkingDays INT= 0;
         WHILE @DateFrom <= @DateTo
		             BEGIN
					
                 IF DATENAME(WEEKDAY, @DateFrom) IN( @Monday, @Tuesday, @Wednesday, @Thursday, @Friday,@Saturday,@Sunday)
                     BEGIN
                         SET @TotWorkingDays = @TotWorkingDays + 1;
                 END;
                 SET @DateFrom = DATEADD(DAY, 1, @DateFrom);
             END;
       
Insert Into @Result (Employeeid,ComanyID,DeductRestrictionAmount)
select EmployeeID,CompanyID,case when @TotWorkingDays>0 then (GrossSalary/@TotWorkingDays)* daysDeduct end as DeductAmount from @tbAculalEmployeeWorking
where Employeeid in (select Top 1 EmployeeIDS from @tblEmployeeIds1) 
		    
   SET @cnt = @cnt + 1;
   Delete Top(1) from @tblEmployeeIds1
END;
END










Return

end




GO
/****** Object:  UserDefinedFunction [dbo].[ufn_PR_CalculateLoanDeduction]    Script Date: 9/6/2024 5:45:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create  FUNCTION [dbo].[ufn_PR_CalculateLoanDeduction](
@CompanyID Numeric(18),
@EmployeeID Numeric(18),
@LoanID as Numeric(18),
@SalaryDate as Date,
@BasicSalary as Numeric(24,2),
@SalaryAmount as Numeric(24,2))

RETURNS Numeric(24,2)
AS   

BEGIN  

DECLARE 
@LoanAmount Float,
@DeductionValue as Float,
@LoanDedAmount Numeric(24,2),
@PaidLoanAmount Float,
@DeductionType Char(1),
@Amount Numeric(24,2)

Select		@DeductionType = p.DeductionType, @DeductionValue = p.DeductionValue,
			@LoanAmount = p.LoanAmount + IsNull(Case	when IsNull(p.AdjustmentType,'') = '' then 0 
														when IsNull(p.AdjustmentType,'') = 'C' then p.AdjustmentAmount 
														else p.AdjustmentAmount * -1 end
												 ,0)
From		pr_loan p
inner join	sys_drop_down_value d on d.ID = p.PaymentMethodID and d.DropDownID = 12 and d.Value='Salary'  
Where		p.CompanyID = @CompanyID And p.EmployeeID = @EmployeeID and p.ID = @LoanID
			and Convert(char(6),p.PaymentStartDate,112) <= Convert(char(6),@SalaryDate,112) and p.ApprovalStatusID='1'
			
If			IsNull(@LoanAmount,0) <= 0
			Return 0;
			
Select		@PaidLoanAmount = Sum(Amount + IsNull(Case	when IsNull(AdjustmentType,'') = '' then 0 
														when IsNull(AdjustmentType,'') = 'C' then AdjustmentAmount 
														else AdjustmentAmount * -1 end
												 ,0)) 
From		pr_loan_payment_dt WHere CompanyID = @CompanyID and LoanID = @LoanID


set			@PaidLoanAmount= isnull(@PaidLoanAmount,0)

Select		@PaidLoanAmount += IsNull(Sum(d.Amount + IsNull(Case	when IsNull(d.AdjustmentType,'') = '' then 0 
														when IsNull(D.AdjustmentType,'') = 'C' then d.AdjustmentAmount 
														else D.AdjustmentAmount * -1 end
												 ,0)),0)
From		pr_employee_payroll_mf m 
Inner Join	pr_employee_payroll_dt d on d.PayrollID = m.ID and m.Status = 'P'
			and d.CompanyID = m.CompanyID and D.EmployeeID = m.EmployeeID
Where		M.CompanyID = @CompanyID and M.EmployeeID = @EmployeeID and d.RefID = @LoanID
			and Convert(char(6),m.PayScheduleFromDate,112) <= Convert(char(6),@SalaryDate,112)

Set			@Amount = @LoanAmount - IsNull(@PaidLoanAmount,0)

If			IsNull(@Amount,0) <= 0
			Return 0;

If	@DeductionType = 'P'
	Set	@DeductionValue = @BasicSalary * @DeductionValue / 100.0;

If @SalaryAmount < @DeductionValue
	Set @LoanDedAmount = @SalaryAmount
Else	
	Set @LoanDedAmount = @DeductionValue

If  @Amount < @LoanDedAmount
	Set @LoanDedAmount = @Amount 

    RETURN @LoanDedAmount;  
END;



GO
/****** Object:  UserDefinedFunction [dbo].[ufn_PR_CalculateMissingDeduction]    Script Date: 9/6/2024 5:45:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- select ID,FirstName,LastName from pr_employee_mf where CompanyID=1230 and Status=1
-- select * from ufn_PR_CalculateMissingDeduction(1323,'5768','20210501','20210531')
-- select * from ufn_PR_CalculateMissingDeduction(1230,'5755,5631,67925,68791','2021-04-01','2021-04-30')
create  FUNCTION [dbo].[ufn_PR_CalculateMissingDeduction]
(@CompanyID Numeric(18),
@EmployeeIds   Varchar(Max),
@MonthStartDate date,
@MonthEndDate date)

RETURNS @Result TABLE 
		(
			Employeeid  NUMERIC(18, 0),
			ComanyID NUMERIC(18, 0),
			DeductMissingAmount NUMERIC(18, 0),
			TotWorkingDays int,
			DeductDays int
		)
AS   
BEGIN  
	
declare @tbEmployeeAllDates Table
		(
		EmpID  NUMERIC(18, 0),
		CID NUMERIC(18, 0),
		DeductDays NUMERIC(18, 0)
		)
		declare @tbEmployeeSalery Table
		(
		CompanyID NUMERIC(18, 0),
		Employeeid  NUMERIC(18, 0),
		GrossSalary NUMERIC(18, 0),
		daysDeduct NUMERIC(18, 0),		
		MA_MissingDayAfter  float,
		MA_AutomaticallyDeductLeave float
		)
declare @tblEmployeeIds Table
		(
		EmployeeIDS  NUMERIC(18, 0)
		)
		declare @tblEmployeeIds1 Table
		(
		EmployeeIDS  NUMERIC(18, 0)
		)
		DECLARE @TempMonthlyRecord TABLE(
		MonthDate date
		)
		DECLARE @Summary TABLE(
		StatusID int,EmployeeID numeric(18,0)
		)

insert into @tblEmployeeIds(EmployeeIDS)
Select		ID 
From		FNC_Split(@EmployeeIds,',')

insert into @tblEmployeeIds1(EmployeeIDS)
Select		ID 
From		FNC_Split(@EmployeeIds,',')

INSERT INTO  @TempMonthlyRecord(MonthDate)
SELECT  TOP (DATEDIFF(DAY, @MonthStartDate, @MonthEndDate) + 1)
        MonthDate = DATEADD(DAY, ROW_NUMBER() OVER(ORDER BY a.object_id) - 1, @MonthStartDate)
FROM    sys.all_objects a       
	     CROSS JOIN sys.all_objects b;

declare @SalaryMethodID as int
	set @SalaryMethodID=(select SalaryMethodID from adm_company where ID=@CompanyID)

DECLARE @AutomaticallyDeductLeave Float
set @AutomaticallyDeductLeave=(select MA_AutomaticallyDeductLeave from pr_time_rule_mf where CompanyID=@CompanyID)

DECLARE @ISDeductSalary Float
set @ISDeductSalary=(select MA_IsPaidLeave from pr_time_rule_mf where CompanyID=@CompanyID)

if(@AutomaticallyDeductLeave = 1)
begin



insert into @tbEmployeeSalery(CompanyID,Employeeid,GrossSalary,daysDeduct,MA_AutomaticallyDeductLeave,MA_MissingDayAfter)
Select CompanyID,EmployeeID,GrossSalary ,0 daysDeduct,MA_AutomaticallyDeductLeave,MA_MissingDayAfter 
	From
	(
	--(empMf.BasicSalary + Isnull(sum(allowance.Amount),0))
		select pr_time.EmployeeID,pr_time.CompanyID,empMf.BasicSalary as GrossSalary,rulemf.MA_AutomaticallyDeductLeave,rulemf.MA_MissingDayAfter
		from pr_employee_mf empMf 
		inner join pr_time_Summary pr_time on pr_time.EmployeeID=empMf.ID and pr_time.CompanyID=empMf.CompanyID
		inner join @tblEmployeeIds empids on empids.EmployeeIDS=pr_time.EmployeeID
		inner join pr_time_rule_mf rulemf on rulemf.CompanyID=pr_time.CompanyID
		left join pr_employee_allowance allowance on allowance.CompanyID=empMf.CompanyID and allowance.EmployeeID=empMf.ID 
		where AttendanceDate between @MonthStartDate and @MonthEndDate  and pr_time.CompanyID=@CompanyID --and pr_time.StatusID in(4,5) and pr_time.EffectiveMinute=0
		and empMf.TimeRule=1 and empMf.MissingAttendance=1 
		group by pr_time.EmployeeID, pr_time.CompanyID,empMf.BasicSalary,rulemf.MA_MissingDayAfter,rulemf.MA_AutomaticallyDeductLeave
	) dd
	

	insert into @tbEmployeeAllDates(EmpID,CID,DeductDays)

--select EmpID,CID,
--	floor(CAST((CAST(lateCount AS float)/CAST(MA_MissingDayAfter as float))as float)*MA_AutomaticallyDeductLeave)
--	  as DeductDays
--		from(
--	select lo.CompanyID CID,lo.EmployeeID EmpID,count(lo.EmployeeID) as lateCount,MA_AutomaticallyDeductLeave ,rulemf.MA_MissingDayAfter
--					from pr_time_summary lo
--					inner join @tblEmployeeIds empids on empids.EmployeeIDS=lo.EmployeeID
--					inner join pr_Employee_Mf  empMf on empMf.ID= lo.EmployeeID and empMf.CompanyID=lo.CompanyID
--					inner join pr_time_rule_mf rulemf on rulemf.CompanyID=lo.CompanyID
--					where lo.StatusID in(4,5) and lo.EffectiveMinute=0
--					and lo.CompanyID=@CompanyID
--					and AttendanceDate between @MonthStartDate and @MonthEndDate
--					and empMf.TimeRule=1 and empMf.MissingAttendance=1 
--					group by lo.EmployeeID,lo.CompanyID,rulemf.MA_MissingDayAfter,rulemf.MA_AutomaticallyDeductLeave
--					)tab

select EmpID,CID,
	lateCount as daysDeduct
		from(
	select lo.CompanyID CID,lo.EmployeeID EmpID,count(lo.EmployeeID) as lateCount
					from pr_time_summary lo
					inner join @tblEmployeeIds empids on empids.EmployeeIDS=lo.EmployeeID
					inner join pr_Employee_Mf  empMf on empMf.ID= lo.EmployeeID and empMf.CompanyID=lo.CompanyID
					inner join pr_time_rule_mf rulemf on rulemf.CompanyID=lo.CompanyID
					where --lo.StatusID in(4,5) and lo.EffectiveMinute=0 and
					 lo.CompanyID=@CompanyID
					and AttendanceDate between @MonthStartDate and @MonthEndDate
					and empMf.TimeRule=1 and empMf.MissingAttendance=1 
					group by lo.EmployeeID,lo.CompanyID,rulemf.MA_MissingDayAfter,rulemf.MA_AutomaticallyDeductLeave
					)tab

-- Get All Days having 0 minutes works e.g clockin clockout at the same time and Absent Days			
update @tbEmployeeSalery set daysDeduct = ISNULL((select top 1 DeductDays from @tbEmployeeAllDates where CID = CompanyID and EmpID=EmployeeID),0)

 -- select * from ufn_PR_CalculateMissingDeduction(1230,'68769,67799,67925,68763','20210401','20210430')
--  select * from ufn_PR_CalculateMissingDeduction(1230,'5755,68763,68769,67799,67925','20210401','20210430')
 UPDATE t1
SET T1.daysDeduct=isnull(t2.TotAbsentDays,0)-(isnull(t2.TotSystemHolidays,0)+isnull(t2.TotPaidLeaves,0))                                          
  FROM @tbEmployeeSalery AS t1 INNER JOIN (select * from fn_GetTotalOffDaysAccordingToAttendance (@CompanyID,@EmployeeIds,@MonthStartDate,@MonthEndDate)) AS t2
  ON t1.Employeeid = t2.Employeeid

  -- select * from ufn_PR_CalculateMissingDeduction (1230,'5755,5631,67925,68791','2021-04-01','2021-04-30')
  -- select * from ufn_PR_CalculateMissingDeduction(1323,'5768','20210501','20210531')

UPDATE t1
SET T1.daysDeduct=
  CASE WHEN T1.MA_MissingDayAfter>0 and T1.MA_AutomaticallyDeductLeave>0 and t1.daysDeduct>0THEN 
  floor(CAST((CAST( isnull(t1.daysDeduct,0) AS float)
                                           /CAST(T1.MA_MissingDayAfter as float))as float)*T1.MA_AutomaticallyDeductLeave) ELSE 0 END
  FROM @tbEmployeeSalery AS t1 INNER JOIN (select * from fn_GetTotalOffDaysAccordingToAttendance (@CompanyID,@EmployeeIds,@MonthStartDate,@MonthEndDate)) AS t2
  ON t1.Employeeid = t2.Employeeid 

if(@SalaryMethodID=1 or @SalaryMethodID is null)
Begin
Insert Into @Result (Employeeid,ComanyID,DeductMissingAmount,TotWorkingDays,DeductDays)
select EmployeeID,CompanyID,GrossSalary/DATEPART(day,EOMONTH(@MonthStartDate))* daysDeduct as DeductAmount,DATEPART(day,EOMONTH(@MonthStartDate)),daysDeduct
from @tbEmployeeSalery
end

if(@SalaryMethodID=2)
Begin	
Insert Into @Result (Employeeid,ComanyID,DeductMissingAmount,TotWorkingDays,DeductDays)
select TWD.Employeeid,TWD.ComanyID, case when TWD.TotWorkingDays>0 then (TES.GrossSalary/TWD.TotWorkingDays )* daysDeduct end as DeductAmount,TWD.TotWorkingDays,TES.daysDeduct
from fn_GetTotalWorkingDaysAccordingToShift(@CompanyID,@EmployeeIds,@MonthStartDate,@MonthEndDate) TWD inner join 
@tbEmployeeSalery TES on TWD.Employeeid=TES.Employeeid
END

END

    RETURN   
END;





GO
/****** Object:  UserDefinedFunction [dbo].[ufn_PR_EarlyOutWageBasedInHoursAndMinutesDeduction]    Script Date: 9/6/2024 5:45:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- 71107,71108,71109,71110,71111,71113,71168,71169,71170,71171,71172,71173,72170

-- select * from ufn_PR_EarlyOutWageBasedInHoursAndMinutesDeduction(2973,'71107,71108,71109,71110,71111,71113,71168,71169,71170,71171,71172,71173,72170','20210726','20210825')
create FUNCTION [dbo].[ufn_PR_EarlyOutWageBasedInHoursAndMinutesDeduction] 
(
	@CompanyID Numeric(18),
	@EmployeeIds   Varchar(Max),
	@MonthStartDate date,
	@MonthEndDate date
)
RETURNS 

@Result TABLE 
(
	CompanyID Numeric(18),
	EmployeeID NUMERIC(18, 0),
	Amount float
)
AS
begin
		declare @tblEmployeeIds Table (
		EmployeeIDS  NUMERIC(18, 0)
		)

		DECLARE  @WeekDaysName TABLE
		(
		EmployeeID numeric(18,0),
		DayName  nvarchar(50)		
		)

		DECLARE  @TempMonthlyRecord TABLE
		(
		EmployeeID Numeric(18,0),
		MonthDate datetime,
		WeekDays  nvarchar(50)		
		)

		DECLARE  @Date TABLE
		(
		MonthDate datetime,
		WeekDays  nvarchar(50)		
		)

		DECLARE @tempShift TABLE
		(
		EmployeeID Numeric(18,0),
		shiftID Numeric(18,0),
	    WDMonday bit,
		WDTuesday bit,
		WDWednesday bit,
		WDThursday bit,
		WDFriday bit,
		WDSatuday bit,
		WDSunday bit
		)

		DECLARE @tblTempAllRecord TABLE (
		MonthDate DateTime,
		AttendanceDate DateTime,
		CompanyID  NUMERIC(18, 0),
		EmployeeID NUMERIC(18, 0),
		BasicSalary NUMERIC(18,0),
		LAWageRateType  bit,
		PerDayWorking INT,
		EarlyoutMinutes INT,
		Hr INT,
		DeductionAmount NUMERIC(18,2),
		ShiftWorkingMints INT
		)

		DECLARE @tblAllRecord TABLE (
		CompanyID  NUMERIC(18, 0),
		EmployeeID NUMERIC(18, 0),
		BasicSalary NUMERIC(18,0),
		LAWageRateType  bit,
		EarlyoutMinutes INT,
		Hr INT,
		DeductionAmount NUMERIC(18,2),
		ShiftWorkingMints INT
		)

		
	declare @SalaryMethodID as int
	set @SalaryMethodID=(select SalaryMethodID from adm_company where ID=@CompanyID)
	 
	 declare @EOWageRateType as bit
	 set @EOWageRateType=(select IsNULL(EOWageRateType,1) from pr_time_rule_mf where CompanyID=@CompanyID)

insert into @tblEmployeeIds(EmployeeIDS)
Select		ID 
From		FNC_Split(@EmployeeIds,',')

INSERT INTO @Date(MonthDate,WeekDays)
SELECT  TOP (DATEDIFF(DAY, @MonthStartDate, @MonthEndDate) + 1)
        MonthDate = DATEADD(DAY, ROW_NUMBER() OVER(ORDER BY a.object_id) - 1, @MonthStartDate) ,
			WeekDays=DateName(DW,DATEADD(DAY, ROW_NUMBER() OVER(ORDER BY a.object_id) - 1, @MonthStartDate) )
FROM    sys.all_objects a 

Insert INTO @TempMonthlyRecord(MonthDate,WeekDays,EmployeeID)
Select		MonthDate,WeekDays,EmployeeID
	From       (
				Select	D.MonthDate,D.WeekDays,E.ID as EmployeeID
				From		pr_employee_mf E
				Cross Join	@Date D
				Where	    Cast(E.JoiningDate as Date) <= D.MonthDate and E.CompanyID=@CompanyID and e.ID in(Select ID from @tblEmployeeIds)
				) A

		INSERT INTO @tempShift(EmployeeID,shiftID,WDMonday,WDTuesday,WDWednesday,WDThursday,WDFriday,WDSatuday,WDSunday) 		 
  SELECT distinct mf.ID 'EmployeeID', sh.ID 'shiftID',sh.WDMonday,sh.WDTuesday,sh.WDWednesday,sh.WDThursday,sh.WDFriday,sh.WDSatuday,sh.WDSunday
  from pr_employee_mf mf inner join pr_employee_shift  sh on mf.ShiftID=sh.ID    
  where mf.CompanyID=@CompanyID and mf.ID in  (SELECT EmployeeIDS from @tblEmployeeIds)

  
  INSERT INTO @WeekDaysName(EmployeeID,[DayName])
		SELECT EmployeeID,CASE WHEN WDSunday=0    THEN 'Sunday'      END FROM @tempShift 
INSERT INTO @WeekDaysName(EmployeeID,[DayName])	  
		SELECT EmployeeID,CASE WHEN WDSatuday=0   THEN 'Saturday'    END FROM @tempShift 
INSERT INTO @WeekDaysName(EmployeeID,[DayName])	  
		SELECT EmployeeID,CASE WHEN WDMonday=0    THEN 'Monday'      END FROM @tempShift 
INSERT INTO @WeekDaysName(EmployeeID,[DayName])	  
		SELECT EmployeeID,CASE WHEN WDTuesday=0   THEN 'Tuesday'     END FROM @tempShift 
INSERT INTO @WeekDaysName(EmployeeID,[DayName])
		SELECT EmployeeID,CASE WHEN WDWednesday=0 THEN 'Wednesday'   END FROM @tempShift 
INSERT INTO @WeekDaysName(EmployeeID,[DayName])
		SELECT EmployeeID,CASE WHEN WDThursday=0  THEN 'Thursday'    END FROM @tempShift 
INSERT INTO @WeekDaysName(EmployeeID,[DayName])						 
		SELECT EmployeeID,CASE WHEN WDFriday=0    THEN 'Friday'      END FROM @tempShift 

		DELETE from @TempMonthlyRecord where WeekDays  in  (Select [DayName] from @WeekDaysName) and EmployeeID in (Select EmployeeID from @WeekDaysName)

INSERT INTO @tblTempAllRecord(MonthDate,AttendanceDate,EmployeeID,CompanyID,BasicSalary,LAWageRateType,PerDayWorking,EarlyoutMinutes,Hr,DeductionAmount,ShiftWorkingMints)

Select AttendanceDate,AttendanceDate,pr_time_entry.EmployeeID,pr_time_entry.CompanyID,Max(isNull(e.BasicSalary,0)) as BasicSalary,
rulemf.LAWageRateType as LAWageRateType,
            Sum(Case When [TimeIn] Is Null Or [TimeOut] Is Null Then 0 else DATEDIFF(minute, [TimeIn],[TimeOut]) End) AS PerDayWorking,
			Sum(Cast(case when convert(char(5),  _shift.EndTime, 108) >convert(char(5), [TimeOut], 108) then 
			DATEDIFF(minute, convert(varchar(5), RIGHT('0'+CAST(DATEPART(hour, [TimeOut]) as varchar(2)),2) + ':' + RIGHT('0'+CAST(DATEPART(minute, [TimeOut])as varchar(2)),2), 108),
			IsNull(CONVERT(varchar(5),_shift.EndTime,108),0)
			) else 0  end as int)) EarlyoutMinutes,
			0 as Hr,0 as DeductionAmount,
			 Max(cast(Convert(varchar(4), (SUBSTRING(_shift.WorkingHour,1,2)*60))as int)  +  cast(Convert(varchar(4), (SUBSTRING(_shift.WorkingHour,4,5)%60),2)as int)) as ShiftWorkingMints
			from 
			pr_time_entry pr_time_entry 
			inner join @tblEmployeeIds empids on empids.EmployeeIDS=pr_time_entry.EmployeeID
			inner join pr_employee_mf e on e.ID=pr_time_entry.EmployeeID 			
			inner join pr_employee_shift _shift on _shift.CompanyID=pr_time_entry.CompanyID
			left join pr_time_rule_mf rulemf on rulemf.CompanyID=_shift.CompanyID
where		pr_time_entry.AttendanceDate between cast(@MonthStartDate as date) and cast(@MonthEndDate as date)
					and e.ShiftID=_shift.ID  and pr_time_entry.CompanyID=@CompanyID 
						and e.TimeRule=1 and e.EarlyOut=1 and rulemf.EOSystemDeductLeave=1
				and  convert(char(5),  _shift.EndTime, 108) >convert(char(5), [TimeOut], 108)  
			group by pr_time_entry.CompanyID,pr_time_entry.EmployeeID,AttendanceDate,LAWageRateType





--select finalTab.MonthDate,finalTab.AttendanceDate,finalTab.EmployeeID,finalTab.CompanyID,finalTab.BasicSalary,finalTab.LAWageRateType,
--finalTab.PerDayWorking,finalTab.EarlyoutMinutes,finalTab.Hr,finalTab.DeductionAmount,finalTab.ShiftWorkingMints
--from (
--select MonthDate ,tab.* ,0 as Hr,0 as DeductionAmount from  @TempMonthlyRecord MR left join (
--Select AttendanceDate,pr_time_entry.EmployeeID,pr_time_entry.CompanyID,Max(isNull(e.BasicSalary,0)) as BasicSalary,rulemf.LAWageRateType as LAWageRateType,
--			Sum(Case When [TimeIn] Is Null Or [TimeOut] Is Null Then 0 else DATEDIFF(minute, [TimeIn],[TimeOut]) End) AS PerDayWorking,
--			Min([TimeIn])[TimeIn],Max([TimeOut])[TimeOut],
--			Sum(Cast(case when convert(char(5),  _shift.EndTime, 108) >convert(char(5), [TimeOut], 108) then 
--			DATEDIFF(minute, convert(varchar(5), RIGHT('0'+CAST(DATEPART(hour, [TimeOut]) as varchar(2)),2) + ':' + RIGHT('0'+CAST(DATEPART(minute, [TimeOut])as varchar(2)),2), 108),
--			IsNull(CONVERT(varchar(5),_shift.EndTime,108),0)
--			) else 0  end as int)) EarlyoutMinutes,
--			 Max(cast(Convert(varchar(4), (SUBSTRING(_shift.WorkingHour,1,2)*60))as int)  +  cast(Convert(varchar(4), (SUBSTRING(_shift.WorkingHour,4,5)%60),2)as int)) as ShiftWorkingMints
--			from 
--			pr_time_entry pr_time_entry 
--			inner join @tblEmployeeIds empids on empids.EmployeeIDS=pr_time_entry.EmployeeID
--			inner join pr_employee_mf e on e.ID=pr_time_entry.EmployeeID 			
--			inner join pr_employee_shift _shift on _shift.CompanyID=pr_time_entry.CompanyID
--			left join pr_time_rule_mf rulemf on rulemf.CompanyID=_shift.CompanyID
--where		pr_time_entry.AttendanceDate between cast(@MonthStartDate as date) and cast(@MonthEndDate as date)
--					and e.ShiftID=_shift.ID  and pr_time_entry.CompanyID=@CompanyID 
--					and e.TimeRule=1 and e.EarlyOut=1 and rulemf.EOSystemDeductLeave=1
--					and  convert(char(5),  _shift.EndTime, 108) >convert(char(5), [TimeOut], 108)  
--			group by pr_time_entry.CompanyID,pr_time_entry.EmployeeID,AttendanceDate,LAWageRateType
--			)  as tab on MR.MonthDate=AttendanceDate and tab.EmployeeID=MR.EmployeeID
--			) as  finalTab where AttendanceDate is not null


			delete from @tblTempAllRecord where AttendanceDate in (
			select AttendanceDate
			FROM @tblTempAllRecord w
            INNER JOIN sys_holidays _sys
            ON w.CompanyID=_sys.CompanyID
            WHERE w.CompanyID=@CompanyID and AttendanceDate between _sys.FromDate and _sys.ToDate
            )
			delete from @tblTempAllRecord where AttendanceDate in (
			select AttendanceDate
			FROM @tblTempAllRecord w
            INNER JOIN pr_leave_application e
            ON w.CompanyID=e.CompanyID and w.EmployeeID=e.EmployeeID
            WHERE w.CompanyID=@CompanyID and AttendanceDate between e.FromDate and e.ToDate
			)
			

           update  @tblTempAllRecord set HR=
           CASE WHEN LAWageRateType=1 THEN 
           CAST(isnull(Convert(varchar(4), (EarlyoutMinutes/60)),'0')as int)  else 0 
           END

		 INSERT INTO @tblAllRecord(EmployeeID,CompanyID,BasicSalary,LAWageRateType,EarlyoutMinutes,Hr,DeductionAmount,ShiftWorkingMints)
		 SELECT EmployeeID,Max(CompanyID) as CompanyID,Max(BasicSalary) as BasicSalary,LAWageRateType,Sum(EarlyoutMinutes),SUM(Hr),0 as DeductionAmount,Max(ShiftWorkingMints)
         FROM @tblTempAllRecord group by EmployeeID,LAWageRateType


	if(@SalaryMethodID=1 or @SalaryMethodID is null)
	BEGIN
	 UPDATE @tblAllRecord
SET DeductionAmount=Case WHEN @EoWageRateType=1 
then
(Cast(BasicSalary/DAY(EOMONTH(@MonthStartDate)) as numeric(18,3))/Cast(Cast(ShiftWorkingMints as numeric(18,2))/60 as numeric(18,2)))*ISNULL(Hr,0)
WHEN @EoWageRateType=0
THEN Round(Cast(Cast(BasicSalary/Day(EOMONTH(@MonthStartDate)) as numeric(18,3))/ShiftWorkingMints  as numeric(18,2))*ISNULL(EarlyoutMinutes,0),2)
else 0 end 
  END
  
  
	if(@SalaryMethodID=2)
	BEGIN
	 UPDATE t1
SET T1.DeductionAmount=Case WHEN @EoWageRateType=1
then
(Cast(T1.BasicSalary/T2.TotWorkingDays as numeric(18,3))/Cast(Cast(T1.ShiftWorkingMints as numeric(18,2))/60 as numeric(18,2)))*ISNULL(T1.Hr,0)
WHEN @EoWageRateType=0 
THEN 
Cast(Cast(T1.BasicSalary/T2.TotWorkingDays as numeric(18,3))/T1.ShiftWorkingMints as numeric(18,2))*ISNULL(T1.EarlyoutMinutes,0)
else 0 end
  FROM @tblAllRecord AS t1 INNER JOIN (select * from fn_GetTotalWorkingDaysAccordingToShift (@CompanyID,@EmployeeIds,@MonthStartDate,@MonthEndDate)) AS t2
  ON t1.Employeeid = t2.Employeeid 
  END

Insert Into @Result (CompanyID,EmployeeID,Amount)
select Max(CompanyID),EmployeeID,SUM(Cast(DeductionAmount as float))
from @tblAllRecord emp 
group by EmployeeID

Return

end



GO
/****** Object:  UserDefinedFunction [dbo].[ufn_PR_HoursInaDayDeduction]    Script Date: 9/6/2024 5:45:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- select * from ufn_PR_HoursInaDayDeduction(2973,'71168,71113,71111','20210726','20210825')
create FUNCTION [dbo].[ufn_PR_HoursInaDayDeduction] 
(
	@CompanyID Numeric(18),
	@EmployeeIds   Varchar(Max),
	@MonthStartDate date,
	@MonthEndDate date
)

RETURNS @Result TABLE 
(
	CompanyID Numeric(18),
	EmployeeID NUMERIC(18, 0),
	Amount float
)
AS
begin
		declare @tblEmployeeIds Table (
		EmployeeIDS  NUMERIC(18, 0)
		)

		DECLARE  @WeekDaysName TABLE
		(
		EmployeeID numeric(18,0),
		DayName  nvarchar(50)		
		)

		DECLARE  @TempMonthlyRecord TABLE
		(
		EmployeeID Numeric(18,0),
		MonthDate datetime,
		WeekDays  nvarchar(50)		
		)

		DECLARE  @Date TABLE
		(
		MonthDate datetime,
		WeekDays  nvarchar(50)		
		)
		
		DECLARE @tempShift TABLE
		(
		EmployeeID Numeric(18,0),
		shiftID Numeric(18,0),
	    WDMonday bit,
		WDTuesday bit,
		WDWednesday bit,
		WDThursday bit,
		WDFriday bit,
		WDSatuday bit,
		WDSunday bit
		)


		declare @tblAllRecord Table (
			CompanyID  NUMERIC(18, 0),
			EmployeeID NUMERIC(18, 0),
			PresentWorkingMints INT,
			SumOfShiftWorkingMints INT,
			ShiftWorkingMints INT,
			BasicSalary NUMERIC(18, 0),
			DeductionAmount NUMERIC(18, 0),
			DeductionMinutes INT,
			TotalHrs INT,
			TotalMins INT,
			RoundWorkedHours INT
		)

	declare @SalaryMethodID as int
	set @SalaryMethodID=(select SalaryMethodID from adm_company where ID=@CompanyID)
	 
	 declare @WageDeductionTypeID as bit
	 set @WageDeductionTypeID=(select IsNULL(WageDeductionTypeID,1) from pr_time_rule_mf where CompanyID=@CompanyID)

insert into @tblEmployeeIds(EmployeeIDS)
Select		ID 
From		FNC_Split(@EmployeeIds,',')

INSERT INTO @Date(MonthDate,WeekDays)
SELECT  TOP (DATEDIFF(DAY, @MonthStartDate, @MonthEndDate) + 1)
        MonthDate = DATEADD(DAY, ROW_NUMBER() OVER(ORDER BY a.object_id) - 1, @MonthStartDate) ,
			WeekDays=DateName(DW,DATEADD(DAY, ROW_NUMBER() OVER(ORDER BY a.object_id) - 1, @MonthStartDate) )
FROM    sys.all_objects a 

Insert INTO @TempMonthlyRecord(MonthDate,WeekDays,EmployeeID)
Select		MonthDate,WeekDays,EmployeeID
	From       (
				Select	D.MonthDate,D.WeekDays,E.ID as EmployeeID
				From		pr_employee_mf E
				Cross Join	@Date D
				Where	    Cast(E.JoiningDate as Date) <= D.MonthDate and E.CompanyID=@CompanyID and e.ID in(Select ID from @tblEmployeeIds)
				) A

		INSERT INTO @tempShift(EmployeeID,shiftID,WDMonday,WDTuesday,WDWednesday,WDThursday,WDFriday,WDSatuday,WDSunday) 		 
  SELECT distinct mf.ID 'EmployeeID', sh.ID 'shiftID',sh.WDMonday,sh.WDTuesday,sh.WDWednesday,sh.WDThursday,sh.WDFriday,sh.WDSatuday,sh.WDSunday
  from pr_employee_mf mf inner join pr_employee_shift  sh on mf.ShiftID=sh.ID    
  where mf.CompanyID=@CompanyID and mf.ID in  (SELECT EmployeeIDS from @tblEmployeeIds)

  
  INSERT INTO @WeekDaysName(EmployeeID,[DayName])
		SELECT EmployeeID,CASE WHEN WDSunday=0    THEN 'Sunday'      END FROM @tempShift 
INSERT INTO @WeekDaysName(EmployeeID,[DayName])	  
		SELECT EmployeeID,CASE WHEN WDSatuday=0   THEN 'Saturday'    END FROM @tempShift 
INSERT INTO @WeekDaysName(EmployeeID,[DayName])	  
		SELECT EmployeeID,CASE WHEN WDMonday=0    THEN 'Monday'      END FROM @tempShift 
INSERT INTO @WeekDaysName(EmployeeID,[DayName])	  
		SELECT EmployeeID,CASE WHEN WDTuesday=0   THEN 'Tuesday'     END FROM @tempShift 
INSERT INTO @WeekDaysName(EmployeeID,[DayName])
		SELECT EmployeeID,CASE WHEN WDWednesday=0 THEN 'Wednesday'   END FROM @tempShift 
INSERT INTO @WeekDaysName(EmployeeID,[DayName])
		SELECT EmployeeID,CASE WHEN WDThursday=0  THEN 'Thursday'    END FROM @tempShift 
INSERT INTO @WeekDaysName(EmployeeID,[DayName])						 
		SELECT EmployeeID,CASE WHEN WDFriday=0    THEN 'Friday'      END FROM @tempShift 

		DELETE from @TempMonthlyRecord where WeekDays  in  (Select [DayName] from @WeekDaysName) and EmployeeID in (Select EmployeeID from @WeekDaysName)

INSERT INTO @tblAllRecord(EmployeeID,CompanyID,PresentWorkingMints,SumOfShiftWorkingMints,ShiftWorkingMints,BasicSalary,
DeductionAmount,DeductionMinutes,TotalHrs,TotalMins,RoundWorkedHours)


	 Select	 EmployeeID,Max(tab.CompanyID) as CompanyID,Sum(tab.PerDayWorking) as PresentWorkingMints,SUM(ShiftWorkingMints) as SumOfShiftWorkingMints ,
	 Max(ShiftWorkingMints),Max(BasicSalary),0 as DeductionAmount,
	 SUM(DeductionMinutes) as DeductionMinutes,SUM(TotalHrs) as TotalHrs,SUM(TotalMins) as TotalMins,SUM(RoundWorkedHours) as RoundWorkedHours
From
			(
			Select pr_time_entry.EmployeeID,pr_time_entry.CompanyID,Max(e.BasicSalary) as BasicSalary, 
			CAST(isnull(Convert(varchar(4), (Sum(Case When [TimeIn] Is Null Or [TimeOut] Is Null Then 0 else DATEDIFF(minute, [TimeIn],[TimeOut]) End)/60)),'0')as int) TotalHrs,
			CAST(isnull(Convert(varchar(4), (Sum(Case When [TimeIn] Is Null Or [TimeOut] Is Null Then 0 else DATEDIFF(minute, [TimeIn],[TimeOut]) End)%60),2),'00')as int) as TotalMins,
			-- When present working minutes less then shift working minutes then subtract total present workingminutes from total shift minutes as per day
			Case WHEN Sum(Case When [TimeIn] Is Null Or [TimeOut] Is Null Then 0 else DATEDIFF(minute, [TimeIn],[TimeOut]) End)< 
			Max((Cast(SUBSTRING(isnull(_shift.WorkingHour,'00:00'),1,2)as int)*60)+Cast(SUBSTRING(isnull(_shift.WorkingHour,'00:00'),4,5)as int)) 
			then Max((Cast(SUBSTRING(isnull(_shift.WorkingHour,'00:00'),1,2)as int)*60)+Cast(SUBSTRING(isnull(_shift.WorkingHour,'00:00'),4,5)as int))
			-Sum(Case When [TimeIn] Is Null Or [TimeOut] Is Null Then 0 else DATEDIFF(minute, [TimeIn],[TimeOut]) End)
			ELSE 0 END as DeductionMinutes,

		     case when Sum(Case When [TimeIn] Is Null Or [TimeOut] Is Null Then 0 else DATEDIFF(minute, [TimeIn],[TimeOut]) End) <= 
			 Max((Cast(SUBSTRING(isnull(_shift.WorkingHour,'00:00'),1,2)as int)*60)+Cast(SUBSTRING(isnull(_shift.WorkingHour,'00:00'),4,5)as int)) then (
                   Round(cast (convert(varchar(10),Round(Sum(Case When [TimeIn] Is Null Or [TimeOut] Is Null Then 0 else DATEDIFF(minute, [TimeIn],[TimeOut]) End)/60,0)) + '.' +
                   CONVERT(varchar(10),CEILING(Sum(Case When [TimeIn] Is Null Or [TimeOut] Is Null Then 0 else DATEDIFF(minute, [TimeIn],[TimeOut]) End)%60)) as numeric(18,0)),0))
		 else (
                   Round(cast (convert(varchar(10),Round(Sum(Case When [TimeIn] Is Null Or [TimeOut] Is Null Then 0 else DATEDIFF(minute, [TimeIn],[TimeOut]) End)/60,0)) + '.' +
                   CONVERT(varchar(10),CEILING(Sum(Case When [TimeIn] Is Null Or [TimeOut] Is Null Then 0 else DATEDIFF(minute, [TimeIn],[TimeOut]) End)%60)) as numeric(18,0)),0))  
				   end as RoundWorkedHours,

			Sum(Case When [TimeIn] Is Null Or [TimeOut] Is Null Then 0 else DATEDIFF(minute, [TimeIn],[TimeOut]) End) AS PerDayWorking,
			Max((Cast(SUBSTRING(isnull(_shift.WorkingHour,'00:00'),1,2)as int)*60)+Cast(SUBSTRING(isnull(_shift.WorkingHour,'00:00'),4,5)as int)) as ShiftWorkingMints
			from pr_time_entry pr_time_entry  with (nolock)
			inner join @tblEmployeeIds EMPIds on EMPIds.EmployeeIDS=pr_time_entry.EmployeeID
			inner join pr_employee_mf e on e.ID=pr_time_entry.EmployeeID 			
			inner join pr_employee_shift _shift on _shift.CompanyID=pr_time_entry.CompanyID
			left join pr_time_rule_mf rulemf on rulemf.CompanyID=_shift.CompanyID
			left join sys_holidays _holidays on _holidays.CompanyID=pr_time_entry.CompanyID and pr_time_entry.AttendanceDate between Cast(_holidays.FromDate as Date) and Cast(_holidays.ToDate as Date)
where		 
			Cast(AttendanceDate as date) between cast(@MonthStartDate as Date) and @MonthEndDate
					and e.ShiftID=_shift.ID and pr_time_entry.CompanyID=@CompanyID 
			group by pr_time_entry.CompanyID,pr_time_entry.EmployeeID,AttendanceDate
			)tab Group by EmployeeID



	if(@SalaryMethodID=1 or @SalaryMethodID is null)
	BEGIN
	 UPDATE @tblAllRecord
SET DeductionAmount=(Case WHEN @WageDeductionTypeID=0 
then
Cast(Cast(Cast((CAST(BasicSalary/Day(EOMONTH(@MonthStartDate)) as numeric(18,3)))/Cast(Cast(ShiftWorkingMints as numeric(18,1))/60 as numeric(18,1)) /60 as numeric(18,2))*DeductionMinutes as numeric(18,2)) as numeric(18,2)) 
WHEN @WageDeductionTypeID=1 
THEN 
Round(Cast( Cast(Cast(BasicSalary/Day(EOMONTH(@MonthStartDate)) as numeric(18,2))/Cast(Cast(ShiftWorkingMints as numeric(18,2))/60 as numeric(18,2)) as numeric(18,2)) * (RoundWorkedHours-TotalHrs) as numeric(18,2)),0)
else 0 end ) from @tblEmployeeIds e where e.EmployeeIDS=EmployeeID
  END
  
  
	if(@SalaryMethodID=2)
	BEGIN
	 UPDATE t1
SET T1.DeductionAmount=Case WHEN @WageDeductionTypeID=0 
then
Cast(Cast(Cast((CAST(T1.BasicSalary/T2.TotWorkingDays as numeric(18,3)))/Cast(Cast(T1.ShiftWorkingMints as numeric(18,1))/60 as numeric(18,1)) /60 as numeric(18,2))*T1.DeductionMinutes as numeric(18,2)) as numeric(18,2)) 
WHEN @WageDeductionTypeID=1 
THEN 
Round(Cast( Cast(Cast(T1.BasicSalary/T2.TotWorkingDays as numeric(18,2))/Cast(Cast(T1.ShiftWorkingMints as numeric(18,2))/60 as numeric(18,2)) as numeric(18,2)) * (T1.RoundWorkedHours-T1.TotalHrs) as numeric(18,2)),0)
else 0 end
  FROM @tblAllRecord AS t1 INNER JOIN (select * from fn_GetTotalWorkingDaysAccordingToShift (@CompanyID,@EmployeeIds,@MonthStartDate,@MonthEndDate)) AS t2
  ON t1.Employeeid = t2.Employeeid 
  END



Insert Into @Result (CompanyID,EmployeeID,Amount)
select Max(CompanyID),EmployeeID,SUM(Cast(DeductionAmount as float))
from @tblAllRecord emp 
group by EmployeeID

Return

end



GO
/****** Object:  UserDefinedFunction [dbo].[ufn_PR_LateArivalWageBasedInHoursAndMinutesDeduction]    Script Date: 9/6/2024 5:45:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--71107,71108,71109,71110,71111,71113,71168,71169,71170,71171,71172,71173,72170
--select * from ufn_PR_LateArivalWageBasedInHoursAndMinutesDeduction(2973,'71107,71108,71109,71110,71111,71113,71168,71169,71170,71171,71172,71173,72170','20210726','20210825')
create FUNCTION [dbo].[ufn_PR_LateArivalWageBasedInHoursAndMinutesDeduction] 
(
	@CompanyID Numeric(18),
	@EmployeeIds   Varchar(Max),
	@MonthStartDate date,
	@MonthEndDate date
)
RETURNS 
@Result TABLE 
(
	CompanyID Numeric(18),
	EmployeeID NUMERIC(18, 0),
	Amount float
)
AS
begin
		declare @tblEmployeeIds Table (
		EmployeeIDS  NUMERIC(18, 0)
		)

		DECLARE  @WeekDaysName TABLE
		(
		EmployeeID numeric(18,0),
		DayName  nvarchar(50)		
		)

		DECLARE  @TempMonthlyRecord TABLE
		(
		EmployeeID Numeric(18,0),
		MonthDate datetime,
		WeekDays  nvarchar(50)		
		)

		DECLARE  @Date TABLE
		(
		MonthDate datetime,
		WeekDays  nvarchar(50)		
		)

		DECLARE @tempShift TABLE
		(
		EmployeeID Numeric(18,0),
		shiftID Numeric(18,0),
	    WDMonday bit,
		WDTuesday bit,
		WDWednesday bit,
		WDThursday bit,
		WDFriday bit,
		WDSatuday bit,
		WDSunday bit
		)

		DECLARE @tblTempAllRecord TABLE (
		MonthDate DateTime,
		AttendanceDate DateTime,
		CompanyID  NUMERIC(18, 0),
		EmployeeID NUMERIC(18, 0),
		BasicSalary NUMERIC(18,0),
		LAWageRateType  bit,
		PerDayWorking INT,
		LateNessMinutes INT,
		Hr INT,
		DeductionAmount NUMERIC(18,2),
		ShiftWorkingMints INT
		)

		DECLARE @tblAllRecord TABLE (
		CompanyID  NUMERIC(18, 0),
		EmployeeID NUMERIC(18, 0),
		BasicSalary NUMERIC(18,0),
		LAWageRateType  bit,
		LateNessMinutes INT,
		Hr INT,
		DeductionAmount NUMERIC(18,2),
		ShiftWorkingMints INT
		)

		
	declare @SalaryMethodID as int
	set @SalaryMethodID=(select SalaryMethodID from adm_company where ID=@CompanyID)
	 
	 declare @LAWageRateType as bit
	 set @LAWageRateType=(select IsNULL(LAWageRateType,1) from pr_time_rule_mf where CompanyID=@CompanyID)

insert into @tblEmployeeIds(EmployeeIDS)
Select		ID 
From		FNC_Split(@EmployeeIds,',')


INSERT INTO @Date(MonthDate,WeekDays)
SELECT  TOP (DATEDIFF(DAY, @MonthStartDate, @MonthEndDate) + 1)
        MonthDate = DATEADD(DAY, ROW_NUMBER() OVER(ORDER BY a.object_id) - 1, @MonthStartDate) ,
			WeekDays=DateName(DW,DATEADD(DAY, ROW_NUMBER() OVER(ORDER BY a.object_id) - 1, @MonthStartDate) )
FROM    sys.all_objects a 

Insert INTO @TempMonthlyRecord(MonthDate,WeekDays,EmployeeID)
Select		MonthDate,WeekDays,EmployeeID
	From       (
				Select	D.MonthDate,D.WeekDays,E.ID as EmployeeID
				From		pr_employee_mf E
				Cross Join	@Date D
				Where	    Cast(E.JoiningDate as Date) <= D.MonthDate and E.CompanyID=@CompanyID and e.ID in(Select EmployeeIDS from @tblEmployeeIds)
				) A

		INSERT INTO @tempShift(EmployeeID,shiftID,WDMonday,WDTuesday,WDWednesday,WDThursday,WDFriday,WDSatuday,WDSunday) 		 
  SELECT distinct mf.ID 'EmployeeID', sh.ID 'shiftID',sh.WDMonday,sh.WDTuesday,sh.WDWednesday,sh.WDThursday,sh.WDFriday,sh.WDSatuday,sh.WDSunday
  from pr_employee_mf mf inner join pr_employee_shift  sh on mf.ShiftID=sh.ID    
  where mf.CompanyID=@CompanyID and mf.ID in  (SELECT EmployeeIDS from @tblEmployeeIds)

  
  INSERT INTO @WeekDaysName(EmployeeID,[DayName])
		SELECT EmployeeID,CASE WHEN WDSunday=0    THEN 'Sunday'      END FROM @tempShift 
INSERT INTO @WeekDaysName(EmployeeID,[DayName])	  
		SELECT EmployeeID,CASE WHEN WDSatuday=0   THEN 'Saturday'    END FROM @tempShift 
INSERT INTO @WeekDaysName(EmployeeID,[DayName])	  
		SELECT EmployeeID,CASE WHEN WDMonday=0    THEN 'Monday'      END FROM @tempShift 
INSERT INTO @WeekDaysName(EmployeeID,[DayName])	  
		SELECT EmployeeID,CASE WHEN WDTuesday=0   THEN 'Tuesday'     END FROM @tempShift 
INSERT INTO @WeekDaysName(EmployeeID,[DayName])
		SELECT EmployeeID,CASE WHEN WDWednesday=0 THEN 'Wednesday'   END FROM @tempShift 
INSERT INTO @WeekDaysName(EmployeeID,[DayName])
		SELECT EmployeeID,CASE WHEN WDThursday=0  THEN 'Thursday'    END FROM @tempShift 
INSERT INTO @WeekDaysName(EmployeeID,[DayName])						 
		SELECT EmployeeID,CASE WHEN WDFriday=0    THEN 'Friday'      END FROM @tempShift 

		DELETE from @TempMonthlyRecord where WeekDays  in  (Select [DayName] from @WeekDaysName) and EmployeeID in (Select EmployeeID from @WeekDaysName)

INSERT INTO @tblTempAllRecord(MonthDate,AttendanceDate,EmployeeID,CompanyID,BasicSalary,LAWageRateType,PerDayWorking,LateNessMinutes,Hr,DeductionAmount,ShiftWorkingMints)


Select AttendanceDate,AttendanceDate,pr_time_entry.EmployeeID,pr_time_entry.CompanyID,Max(isNull(e.BasicSalary,0)) as BasicSalary,
rulemf.LAWageRateType as LAWageRateType,
            Sum(Case When [TimeIn] Is Null Or [TimeOut] Is Null Then 0 else DATEDIFF(minute, [TimeIn],[TimeOut]) End) AS PerDayWorking,
			Sum(Cast(case when convert(char(5),dateadd(minute,IsNull(rulemf.LA_GracePeriod,0),_shift.StartTime),108)< convert(char(5), [TimeIn], 108)  
			then DATEDIFF(minute, IsNull(CONVERT(varchar(5),_shift.StartTime,108),0),
			convert(varchar(5), RIGHT('0'+CAST(DATEPART(hour, [TimeIn]) as varchar(2)),2) + ':' + RIGHT('0'+CAST(DATEPART(minute, [TimeIn])as varchar(2)),2), 108)
			)-ISNULL(rulemf.LA_GracePeriod,0) else 0  end as int)) LateNessMinutes,0 as Hr,0 as DeductionAmount,
			 Max(cast(Convert(varchar(4), (SUBSTRING(_shift.WorkingHour,1,2)*60))as int)  +  cast(Convert(varchar(4), (SUBSTRING(_shift.WorkingHour,4,5)%60),2)as int)) as ShiftWorkingMints
			from 
			pr_time_entry pr_time_entry 
			inner join @tblEmployeeIds empids on empids.EmployeeIDS=pr_time_entry.EmployeeID
			inner join pr_employee_mf e on e.ID=pr_time_entry.EmployeeID 			
			inner join pr_employee_shift _shift on _shift.CompanyID=pr_time_entry.CompanyID
			left join pr_time_rule_mf rulemf on rulemf.CompanyID=_shift.CompanyID
where		pr_time_entry.AttendanceDate between cast(@MonthStartDate as date) and cast(@MonthEndDate as date)
					and e.ShiftID=_shift.ID  and pr_time_entry.CompanyID=@CompanyID 
					and e.TimeRule=1 and e.LastArrival=1 and rulemf.LateDeductionWageRateType=1
					and  convert(char(5),dateadd(minute,IsNull(rulemf.LA_GracePeriod,0),_shift.StartTime),108)< convert(char(5), [TimeIn], 108) 
			group by pr_time_entry.CompanyID,pr_time_entry.EmployeeID,AttendanceDate,LAWageRateType
		






			delete from @tblTempAllRecord where AttendanceDate in (
			select AttendanceDate
			FROM @tblTempAllRecord w
            INNER JOIN sys_holidays _sys
            ON w.CompanyID=_sys.CompanyID
            WHERE w.CompanyID=@CompanyID and AttendanceDate between _sys.FromDate and _sys.ToDate
            )
			delete from @tblTempAllRecord where AttendanceDate in (
			select AttendanceDate
			FROM @tblTempAllRecord w
            INNER JOIN pr_leave_application e
            ON w.CompanyID=e.CompanyID and w.EmployeeID=e.EmployeeID
            WHERE w.CompanyID=@CompanyID and AttendanceDate between e.FromDate and e.ToDate
			)
			

           update  @tblTempAllRecord set HR=
           CASE WHEN LAWageRateType=1 THEN 
           CAST(isnull(Convert(varchar(4), (LateNessMinutes/60)),'0')as int)  else 0 
           END

		 INSERT INTO @tblAllRecord(EmployeeID,CompanyID,BasicSalary,LAWageRateType,LateNessMinutes,Hr,DeductionAmount,ShiftWorkingMints)
		 SELECT EmployeeID,Max(CompanyID) as CompanyID,Max(BasicSalary) as BasicSalary,LAWageRateType,Sum(LateNessMinutes),SUM(Hr),0 as DeductionAmount,Max(ShiftWorkingMints)
         FROM @tblTempAllRecord group by EmployeeID,LAWageRateType


	if(@SalaryMethodID=1 or @SalaryMethodID is null)
	BEGIN
	 UPDATE @tblAllRecord
SET DeductionAmount=Case WHEN @LAWageRateType=1 
then
(Cast(BasicSalary/DAY(EOMONTH(@MonthStartDate)) as numeric(18,3))/Cast(Cast(ShiftWorkingMints as numeric(18,2))/60 as numeric(18,2)))*ISNULL(Hr,0)
WHEN @LAWageRateType=0
THEN Round(Cast(Cast(BasicSalary/Day(EOMONTH(@MonthStartDate)) as numeric(18,3))/ShiftWorkingMints  as numeric(18,2))*ISNULL(LateNessMinutes,0),2)
else 0 end 
  END
  
  
	if(@SalaryMethodID=2)
	BEGIN
	 UPDATE t1
SET T1.DeductionAmount=Case WHEN @LAWageRateType=1
then
(Cast(T1.BasicSalary/T2.TotWorkingDays as numeric(18,3))/Cast(Cast(T1.ShiftWorkingMints as numeric(18,2))/60 as numeric(18,2)))*ISNULL(T1.Hr,0)
WHEN @LAWageRateType=0 
THEN 
Cast(Cast(T1.BasicSalary/T2.TotWorkingDays as numeric(18,3))/T1.ShiftWorkingMints as numeric(18,2))*ISNULL(T1.LateNessMinutes,0)
else 0 end
  FROM @tblAllRecord AS t1 INNER JOIN (select * from fn_GetTotalWorkingDaysAccordingToShift (@CompanyID,@EmployeeIds,@MonthStartDate,@MonthEndDate)) AS t2
  ON t1.Employeeid = t2.Employeeid 
  END

Insert Into @Result (CompanyID,EmployeeID,Amount)
select Max(CompanyID),EmployeeID,SUM(Cast(DeductionAmount as float))
from @tblAllRecord emp 
group by EmployeeID

Return

end

GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 9/6/2024 5:45:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[adm_company]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[adm_company](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyName] [nvarchar](max) NULL,
	[CompanyTypeDropDownID] [int] NOT NULL,
	[CompanyTypeID] [int] NULL,
	[ReceiptFooter] [nvarchar](max) NULL,
	[GenderID] [int] NULL,
	[ContactPersonFirstName] [nvarchar](max) NULL,
	[IsCNICMandatory] [bit] NOT NULL,
	[ContactPersonLastName] [nvarchar](max) NULL,
	[IsShowBillReceptionist] [bit] NOT NULL,
	[CompanyAddress1] [nvarchar](max) NULL,
	[CompanyAddress2] [nvarchar](max) NULL,
	[LanguageID] [int] NULL,
	[CityDropDownId] [int] NULL,
	[CompanyLogo] [nvarchar](max) NULL,
	[CountryDropdownId] [int] NULL,
	[Phone] [nvarchar](max) NULL,
	[Fax] [nvarchar](max) NULL,
	[PostalCode] [nvarchar](max) NULL,
	[Province] [nvarchar](max) NULL,
	[Website] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[IsTrialVersion] [bit] NOT NULL,
	[IsBackDatedAppointment] [bit] NOT NULL,
	[IsUpdateBillDate] [bit] NULL,
	[DateFormatDropDownID] [int] NOT NULL,
	[DateFormatId] [int] NULL,
	[SalaryMethodID] [int] NULL,
	[WDMonday] [bit] NOT NULL,
	[WDTuesday] [bit] NOT NULL,
	[WDWednesday] [bit] NOT NULL,
	[WDThursday] [bit] NOT NULL,
	[WDFriday] [bit] NOT NULL,
	[WDSatuday] [bit] NOT NULL,
	[WDSunday] [bit] NOT NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_adm_company] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[adm_company_location]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[adm_company_location](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[LocationName] [nvarchar](max) NULL,
	[Address] [nvarchar](max) NULL,
	[CountryDropDownID] [int] NOT NULL,
	[CountryID] [int] NULL,
	[CityDropDownID] [int] NOT NULL,
	[CityID] [int] NULL,
	[ZipCode] [nvarchar](max) NULL,
 CONSTRAINT [PK_adm_company_location] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[adm_item]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[adm_item](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[SKU] [nvarchar](max) NULL,
	[Image] [nvarchar](max) NULL,
	[UnitDropDownID] [int] NOT NULL,
	[UnitID] [int] NULL,
	[ItemTypeDropDownID] [int] NOT NULL,
	[ItemTypeId] [int] NULL,
	[CategoryDropDownID] [int] NOT NULL,
	[CategoryID] [int] NULL,
	[TrackInventory] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CostPrice] [decimal](18, 2) NOT NULL,
	[SalePrice] [decimal](18, 2) NOT NULL,
	[InventoryOpeningStock] [decimal](18, 2) NULL,
	[InventoryStockPerUnit] [decimal](18, 2) NULL,
	[InventoryStockQuantity] [decimal](18, 2) NULL,
	[SaveStatus] [int] NOT NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_adm_item] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[adm_multilingual_dt]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[adm_multilingual_dt](
	[ID] [numeric](18, 0) NOT NULL,
	[MultilingualId] [numeric](18, 0) NOT NULL,
	[Module] [nvarchar](max) NULL,
	[Form] [nvarchar](max) NULL,
	[Keyword] [nvarchar](max) NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_adm_multilingual_dt] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[adm_multilingual_mf]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[adm_multilingual_mf](
	[MultilingualId] [numeric](18, 0) IDENTITY(1,1) NOT NULL,
	[MultilingualName] [nvarchar](max) NULL,
 CONSTRAINT [PK_adm_multilingual_mf] PRIMARY KEY CLUSTERED 
(
	[MultilingualId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[adm_role_dt]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[adm_role_dt](
	[ID] [numeric](18, 0) NOT NULL,
	[RoleID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[DropDownScreenID] [int] NOT NULL,
	[ScreenID] [int] NOT NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
	[ViewRights] [bit] NOT NULL,
	[CreateRights] [bit] NOT NULL,
	[DeleteRights] [bit] NOT NULL,
	[EditRights] [bit] NOT NULL,
 CONSTRAINT [PK_adm_role_dt] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[RoleID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[adm_role_mf]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[adm_role_mf](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[RoleName] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[SystemGenerated] [bit] NOT NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[IsUpdateText] [bit] NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_adm_role_mf] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[adm_user_company]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[adm_user_company](
	[ID] [numeric](18, 0) NOT NULL,
	[UserID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[EmployeeID] [numeric](18, 0) NOT NULL,
	[RoleID] [numeric](18, 0) NOT NULL,
	[AdminID] [numeric](18, 0) NOT NULL,
	[IsDefault] [bit] NOT NULL,
 CONSTRAINT [PK_adm_user_company] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[adm_user_mf]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[adm_user_mf](
	[ID] [numeric](18, 0) NOT NULL,
	[Email] [nvarchar](max) NULL,
	[Pwd] [nvarchar](max) NULL,
	[Name] [nvarchar](max) NULL,
	[PhoneNo] [nvarchar](max) NULL,
	[AccountType] [nvarchar](max) NULL,
	[CultureID] [int] NOT NULL,
	[AccountStatus] [int] NULL,
	[IsGenderDropdown] [nvarchar](max) NULL,
	[AppointmentStatusId] [int] NULL,
	[EmployeeID] [numeric](18, 0) NOT NULL,
	[LoginFailureNo] [int] NOT NULL,
	[UserLock] [bit] NOT NULL,
	[IsActivated] [bit] NOT NULL,
	[UserImage] [nvarchar](max) NULL,
	[RepotFooter] [nvarchar](max) NULL,
	[ActivationToken] [nvarchar](max) NULL,
	[Qualification] [nvarchar](max) NULL,
	[Designation] [nvarchar](max) NULL,
	[ActivationTokenDate] [datetime2](7) NULL,
	[ActivatedDate] [datetime2](7) NULL,
	[LastSignIn] [datetime2](7) NULL,
	[ForgotToken] [nvarchar](max) NULL,
	[ForgotTokenDate] [datetime2](7) NULL,
	[PhoneNotification] [bit] NOT NULL,
	[EmailNotification] [bit] NOT NULL,
	[SlotTime] [time](7) NULL,
	[StartTime] [time](7) NULL,
	[EndTime] [time](7) NULL,
	[IsOverLap] [bit] NOT NULL,
	[ExpiryDate] [datetime2](7) NULL,
	[SpecialtyId] [int] NULL,
	[SpecialtyDropdownId] [int] NULL,
	[Type] [nvarchar](max) NULL,
	[OffDay] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsShowDoctor] [nvarchar](max) NULL,
	[MultilingualId] [numeric](18, 0) NULL,
 CONSTRAINT [PK_adm_user_mf] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[adm_user_token]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[adm_user_token](
	[ID] [numeric](18, 0) NOT NULL,
	[UserID] [numeric](18, 0) NOT NULL,
	[TokenKey] [nvarchar](max) NULL,
	[ExpiryDate] [datetime2](7) NOT NULL,
	[IsExpired] [bit] NOT NULL,
	[DeviceType] [nvarchar](max) NULL,
	[DeviceID] [nvarchar](max) NULL,
 CONSTRAINT [PK_adm_user_token] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[contact]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[contact](
	[ID] [numeric](18, 0) IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[Phone] [nvarchar](max) NULL,
	[Speciality] [nvarchar](max) NULL,
	[Message] [nvarchar](max) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_contact] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[emr_appointment_mf]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[emr_appointment_mf](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[PatientId] [numeric](18, 0) NOT NULL,
	[PatientProblem] [nvarchar](max) NULL,
	[DoctorId] [numeric](18, 0) NOT NULL,
	[AppointmentDate] [date] NOT NULL,
	[AppointmentTime] [time](7) NOT NULL,
	[TokenNo] [int] NOT NULL,
	[Notes] [nvarchar](max) NULL,
	[IsAdmission] [bit] NOT NULL,
	[AdmissionId] [numeric](18, 0) NULL,
	[StatusId] [int] NULL,
	[IsAdmit] [bit] NOT NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_emr_appointment_mf] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[emr_bill_type]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[emr_bill_type](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[ServiceName] [nvarchar](max) NULL,
	[IsItem] [bit] NOT NULL,
	[Price] [decimal](18, 2) NULL,
	[IsSystemGenerated] [bit] NOT NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_emr_bill_type] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[emr_complaint]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[emr_complaint](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[Complaint] [nvarchar](max) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_emr_complaint] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[emr_diagnos]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[emr_diagnos](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[Diagnos] [nvarchar](max) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_emr_diagnos] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[emr_document]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[emr_document](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[Date] [date] NULL,
	[DocumentUpload] [nvarchar](max) NULL,
	[DocumentTypeId] [int] NOT NULL,
	[DocumentTypeDropdownId] [int] NOT NULL,
	[Remarks] [nvarchar](max) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_emr_document] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[emr_expense]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[emr_expense](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[CategoryId] [int] NOT NULL,
	[CategoryDropdownId] [int] NOT NULL,
	[Date] [datetime2](7) NOT NULL,
	[Remark] [nvarchar](max) NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[ClinicId] [decimal](18, 2) NOT NULL,
	[InvoiceDate] [datetime2](7) NULL,
	[InvoiceNumber] [nvarchar](max) NULL,
	[Vendor] [nvarchar](max) NULL,
	[PaymentStatusId] [int] NULL,
	[PaymentStatusDropdownId] [int] NULL,
	[PaymentRemrks] [nvarchar](max) NULL,
	[Attachment] [nvarchar](max) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_emr_expense] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[emr_income]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[emr_income](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[CategoryId] [int] NOT NULL,
	[CategoryDropdownId] [int] NOT NULL,
	[Date] [datetime2](7) NOT NULL,
	[Remark] [nvarchar](max) NULL,
	[DueAmount] [decimal](18, 2) NOT NULL,
	[ReceivedAmount] [decimal](18, 2) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[Image] [nvarchar](max) NULL,
	[ModifiedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_emr_income] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[emr_instruction]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[emr_instruction](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[Instructions] [nvarchar](max) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_emr_instruction] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[emr_investigation]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[emr_investigation](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[Investigation] [nvarchar](max) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_emr_investigation] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[emr_medicine]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[emr_medicine](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[Medicine] [nvarchar](max) NULL,
	[Price] [decimal](18, 2) NULL,
	[UnitId] [int] NULL,
	[UnitDropdownId] [int] NULL,
	[TypeId] [int] NULL,
	[TypeDropdownId] [int] NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_emr_medicine] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[emr_notes_favorite]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[emr_notes_favorite](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[DoctorId] [bigint] NOT NULL,
	[ReferenceId] [bigint] NOT NULL,
	[ReferenceType] [nvarchar](max) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_emr_notes_favorite] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[emr_observation]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[emr_observation](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[Observation] [nvarchar](max) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_emr_observation] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[emr_patient_bill]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[emr_patient_bill](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[AdmissionId] [numeric](18, 0) NULL,
	[AppointmentId] [numeric](18, 0) NULL,
	[DoctorId] [numeric](18, 0) NOT NULL,
	[PatientId] [numeric](18, 0) NOT NULL,
	[ServiceId] [numeric](18, 0) NOT NULL,
	[OutstandingBalance] [decimal](18, 2) NOT NULL,
	[Remarks] [nvarchar](max) NULL,
	[BillDate] [datetime2](7) NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[Discount] [decimal](18, 2) NOT NULL,
	[PaidAmount] [decimal](18, 2) NOT NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[RefundAmount] [decimal](18, 2) NULL,
	[RefundDate] [datetime2](7) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_emr_patient_bill] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[emr_patient_bill_payment]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[emr_patient_bill_payment](
	[ID] [numeric](18, 0) NOT NULL,
	[BillId] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[Amount] [decimal](18, 2) NULL,
	[Remarks] [nvarchar](max) NULL,
	[CreatedBy] [numeric](18, 0) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[PaymentDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_emr_patient_bill_payment_1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[emr_patient_mf]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[emr_patient_mf](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[PatientName] [nvarchar](max) NULL,
	[Gender] [int] NOT NULL,
	[DOB] [datetime2](7) NULL,
	[Email] [nvarchar](max) NULL,
	[Mobile] [nvarchar](max) NULL,
	[CNIC] [nvarchar](max) NULL,
	[Image] [nvarchar](max) NULL,
	[Notes] [nvarchar](max) NULL,
	[MRNO] [nvarchar](max) NULL,
	[BillTypeId] [int] NOT NULL,
	[BillTypeDropdownId] [int] NOT NULL,
	[ContactNo] [nvarchar](max) NULL,
	[PrefixTittleId] [int] NOT NULL,
	[PrefixDropdownId] [int] NOT NULL,
	[Father_Husband] [nvarchar](max) NULL,
	[BloodGroupId] [int] NULL,
	[BloodGroupDropDownId] [int] NULL,
	[EmergencyNo] [nvarchar](max) NULL,
	[Address] [nvarchar](max) NULL,
	[ReferredBy] [nvarchar](max) NULL,
	[AnniversaryDate] [datetime2](7) NULL,
	[Illness_Diabetes] [bit] NOT NULL,
	[Illness_Tuberculosis] [bit] NOT NULL,
	[Illness_HeartPatient] [bit] NOT NULL,
	[Illness_LungsRelated] [bit] NOT NULL,
	[Illness_BloodPressure] [bit] NOT NULL,
	[Illness_Migraine] [bit] NOT NULL,
	[Illness_Other] [nvarchar](max) NULL,
	[Allergies_Food] [nvarchar](max) NULL,
	[Allergies_Drug] [nvarchar](max) NULL,
	[Allergies_Other] [nvarchar](max) NULL,
	[Habits_Smoking] [nvarchar](max) NULL,
	[Habits_Drinking] [nvarchar](max) NULL,
	[Habits_Tobacco] [nvarchar](max) NULL,
	[Habits_Other] [nvarchar](max) NULL,
	[MedicalHistory] [nvarchar](max) NULL,
	[CurrentMedication] [nvarchar](max) NULL,
	[HabitsHistory] [nvarchar](max) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[Age] [decimal](18, 2) NULL,
 CONSTRAINT [PK_emr_patient_mf] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[emr_prescription_complaint]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[emr_prescription_complaint](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[PrescriptionId] [numeric](18, 0) NOT NULL,
	[ComplaintId] [numeric](18, 0) NULL,
	[Complaint] [nvarchar](max) NULL,
	[PatientId] [numeric](18, 0) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_emr_prescription_complaint] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[emr_prescription_diagnos]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[emr_prescription_diagnos](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[PrescriptionId] [numeric](18, 0) NOT NULL,
	[DiagnosId] [numeric](18, 0) NULL,
	[Diagnos] [nvarchar](max) NULL,
	[PatientId] [numeric](18, 0) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_emr_prescription_diagnos] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[emr_prescription_investigation]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[emr_prescription_investigation](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[PrescriptionId] [numeric](18, 0) NOT NULL,
	[InvestigationId] [numeric](18, 0) NULL,
	[Investigation] [nvarchar](max) NULL,
	[PatientId] [numeric](18, 0) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_emr_prescription_investigation] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[emr_prescription_mf]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[emr_prescription_mf](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[IsTemplate] [bit] NOT NULL,
	[AppointmentDate] [datetime2](7) NOT NULL,
	[PatientId] [numeric](18, 0) NOT NULL,
	[ClinicId] [numeric](18, 0) NOT NULL,
	[DoctorId] [numeric](18, 0) NOT NULL,
	[FollowUpDate] [datetime2](7) NULL,
	[FollowUpTime] [time](7) NULL,
	[IsCreateAppointment] [bit] NOT NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[Day] [int] NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_emr_prescription_mf] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[emr_prescription_observation]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[emr_prescription_observation](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[PrescriptionId] [numeric](18, 0) NOT NULL,
	[ObservationId] [numeric](18, 0) NULL,
	[Observation] [nvarchar](max) NULL,
	[PatientId] [numeric](18, 0) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_emr_prescription_observation] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[emr_prescription_treatment]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[emr_prescription_treatment](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[PrescriptionId] [numeric](18, 0) NOT NULL,
	[MedicineName] [nvarchar](max) NULL,
	[MedicineId] [numeric](18, 0) NULL,
	[Duration] [int] NULL,
	[PatientId] [numeric](18, 0) NULL,
	[Measure] [nvarchar](max) NULL,
	[IsMorning] [bit] NOT NULL,
	[IsEvening] [bit] NOT NULL,
	[IsSOS] [bit] NOT NULL,
	[IsNoon] [bit] NOT NULL,
	[Instructions] [nvarchar](max) NULL,
	[InstructionId] [numeric](18, 0) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_emr_prescription_treatment] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[emr_prescription_treatment_template]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[emr_prescription_treatment_template](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[TemplateName] [nvarchar](max) NULL,
	[PrescriptionId] [numeric](18, 0) NOT NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_emr_prescription_treatment_template] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[emr_vital]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[emr_vital](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[Measure] [nvarchar](max) NULL,
	[Date] [datetime2](7) NULL,
	[VitalId] [int] NOT NULL,
	[VitalDropdownId] [int] NOT NULL,
	[PatientId] [numeric](18, 0) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_emr_vital] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[inv_stock]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[inv_stock](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[ItemID] [numeric](18, 0) NOT NULL,
	[Quantity] [decimal](18, 2) NOT NULL,
	[BatchSarialNumber] [int] NOT NULL,
	[ExpiredWarrantyDate] [datetime2](7) NOT NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_inv_stock_1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ipd_admission]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ipd_admission](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[AdmissionNo] [nvarchar](max) NULL,
	[PatientId] [numeric](18, 0) NOT NULL,
	[AdmissionTypeId] [int] NOT NULL,
	[AdmissionTypeDropdownId] [int] NULL,
	[TypeId] [int] NULL,
	[WardTypeId] [int] NULL,
	[WardTypeDropdownId] [int] NULL,
	[BedId] [int] NULL,
	[BedDropdownId] [int] NULL,
	[RoomId] [int] NULL,
	[RoomDropdownId] [int] NULL,
	[DoctorId] [int] NOT NULL,
	[AdmissionDate] [datetime2](7) NOT NULL,
	[AdmissionTime] [time](7) NOT NULL,
	[DischargeDate] [datetime2](7) NULL,
	[DischargeTime] [time](7) NULL,
	[Location] [nvarchar](max) NULL,
	[ReasonForVisit] [nvarchar](max) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_ipd_admission] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ipd_admission_charges]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ipd_admission_charges](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[AdmissionId] [numeric](18, 0) NOT NULL,
	[PatientId] [numeric](18, 0) NOT NULL,
	[AppointmentId] [numeric](18, 0) NOT NULL,
	[AnnualPE] [decimal](18, 2) NULL,
	[General] [decimal](18, 2) NULL,
	[Medical] [decimal](18, 2) NULL,
	[ICUCharges] [decimal](18, 2) NULL,
	[ExamRoom] [decimal](18, 2) NULL,
	[PrivateWard] [decimal](18, 2) NULL,
	[RIP] [decimal](18, 2) NULL,
	[OtherAllCharges] [decimal](18, 2) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_ipd_admission_charges] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ipd_admission_discharge]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ipd_admission_discharge](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[AdmissionId] [numeric](18, 0) NOT NULL,
	[PatientId] [numeric](18, 0) NOT NULL,
	[Weight] [nvarchar](max) NULL,
	[Temperature] [nvarchar](max) NULL,
	[DiagnosisAdmission] [nvarchar](max) NULL,
	[ComplaintSummary] [nvarchar](max) NULL,
	[ConditionAdmission] [nvarchar](max) NULL,
	[GeneralCondition] [nvarchar](max) NULL,
	[RespiratoryRate] [nvarchar](max) NULL,
	[OtherRemarks] [nvarchar](max) NULL,
	[CheckedBy] [nvarchar](max) NULL,
	[BP] [nvarchar](max) NULL,
	[Other] [nvarchar](max) NULL,
	[Systemic] [nvarchar](max) NULL,
	[PA] [nvarchar](max) NULL,
	[PV] [nvarchar](max) NULL,
	[UrineProteins] [nvarchar](max) NULL,
	[Sugar] [nvarchar](max) NULL,
	[Microscopy] [nvarchar](max) NULL,
	[BloodHB] [nvarchar](max) NULL,
	[TLC] [nvarchar](max) NULL,
	[P] [nvarchar](max) NULL,
	[L] [nvarchar](max) NULL,
	[E] [nvarchar](max) NULL,
	[ESR] [nvarchar](max) NULL,
	[BloodSugar] [nvarchar](max) NULL,
	[BloodGroup] [nvarchar](max) NULL,
	[Ultrasound] [nvarchar](max) NULL,
	[UltrasoundRemark] [nvarchar](max) NULL,
	[XRay] [nvarchar](max) NULL,
	[XRayRemark] [nvarchar](max) NULL,
	[Conservative] [nvarchar](max) NULL,
	[Operation] [nvarchar](max) NULL,
	[Date] [nvarchar](max) NULL,
	[Surgeon] [nvarchar](max) NULL,
	[Assistant] [nvarchar](max) NULL,
	[Anaesthesia] [nvarchar](max) NULL,
	[Incision] [nvarchar](max) NULL,
	[OperativeFinding] [nvarchar](max) NULL,
	[OprationNotes] [nvarchar](max) NULL,
	[OPMedicines] [nvarchar](max) NULL,
	[OPProgress] [nvarchar](max) NULL,
	[ConditionDischarge] [nvarchar](max) NULL,
	[RemovalDate] [nvarchar](max) NULL,
	[ConditionWound] [nvarchar](max) NULL,
	[AdviseMedicine] [nvarchar](max) NULL,
	[FollowUpDate] [datetime2](7) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_ipd_admission_discharge] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ipd_admission_imaging]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ipd_admission_imaging](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[AdmissionId] [numeric](18, 0) NOT NULL,
	[AppointmentId] [numeric](18, 0) NOT NULL,
	[PatientId] [numeric](18, 0) NOT NULL,
	[ImagingTypeId] [int] NOT NULL,
	[ImagingTypeDropdownId] [int] NOT NULL,
	[Notes] [nvarchar](max) NULL,
	[StatusId] [int] NOT NULL,
	[StatusDropdownId] [int] NOT NULL,
	[ResultId] [int] NOT NULL,
	[ResultDropdownId] [int] NOT NULL,
	[Image] [nvarchar](max) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_ipd_admission_imaging] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ipd_admission_lab]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ipd_admission_lab](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[AdmissionId] [numeric](18, 0) NOT NULL,
	[AppointmentId] [numeric](18, 0) NOT NULL,
	[PatientId] [numeric](18, 0) NOT NULL,
	[LabTypeId] [int] NOT NULL,
	[LabTypeDropdownId] [int] NULL,
	[Notes] [nvarchar](max) NULL,
	[CollectDate] [datetime2](7) NULL,
	[TestDate] [datetime2](7) NULL,
	[ReportDate] [datetime2](7) NULL,
	[OrderingPhysician] [nvarchar](max) NULL,
	[Parameter] [nvarchar](max) NULL,
	[ResultValues] [nvarchar](max) NULL,
	[ABN] [nvarchar](max) NULL,
	[Flags] [nvarchar](max) NULL,
	[Comment] [nvarchar](max) NULL,
	[TestPerformedAt] [nvarchar](max) NULL,
	[TestDescription] [nvarchar](max) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[StatusId] [int] NOT NULL,
	[StatusDropdownId] [int] NOT NULL,
	[ResultId] [int] NOT NULL,
	[ResultDropdownId] [int] NOT NULL,
 CONSTRAINT [PK_ipd_admission_lab] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ipd_admission_medication]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ipd_admission_medication](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[AdmissionId] [numeric](18, 0) NOT NULL,
	[AppointmentId] [numeric](18, 0) NOT NULL,
	[PatientId] [numeric](18, 0) NOT NULL,
	[MedicineId] [numeric](18, 0) NOT NULL,
	[Prescription] [nvarchar](max) NULL,
	[PrescriptionDate] [datetime2](7) NULL,
	[QuantityRequested] [decimal](18, 2) NOT NULL,
	[Refills] [nvarchar](max) NULL,
	[IsRequestNow] [bit] NULL,
	[BillTo] [nvarchar](max) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_ipd_admission_medication] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ipd_admission_notes]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ipd_admission_notes](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[AdmissionId] [numeric](18, 0) NOT NULL,
	[AppointmentId] [numeric](18, 0) NOT NULL,
	[PatientId] [numeric](18, 0) NOT NULL,
	[OnBehalfOf] [nvarchar](max) NULL,
	[Note] [nvarchar](max) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_ipd_admission_notes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ipd_admission_vital]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ipd_admission_vital](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[AdmissionId] [numeric](18, 0) NOT NULL,
	[AppointmentId] [numeric](18, 0) NOT NULL,
	[PatientId] [numeric](18, 0) NOT NULL,
	[DateRecorded] [datetime2](7) NOT NULL,
	[Temperature] [bigint] NOT NULL,
	[Weight] [bigint] NULL,
	[Height] [bigint] NULL,
	[SBP] [nvarchar](max) NULL,
	[DBP] [nvarchar](max) NULL,
	[HeartRate] [nvarchar](max) NULL,
	[RespiratoryRate] [nvarchar](max) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_ipd_admission_vital] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ipd_diagnosis]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ipd_diagnosis](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[AdmissionId] [numeric](18, 0) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Date] [datetime2](7) NULL,
	[IsType] [nvarchar](max) NULL,
	[IsVisitType] [bit] NOT NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_ipd_diagnosis] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ipd_procedure_charged]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ipd_procedure_charged](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[ProcedureId] [numeric](18, 0) NOT NULL,
	[AppointmentId] [numeric](18, 0) NOT NULL,
	[PatientId] [numeric](18, 0) NOT NULL,
	[Date] [datetime2](7) NULL,
	[Item] [nvarchar](max) NULL,
	[Quantity] [decimal](18, 2) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_ipd_procedure_charged] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ipd_procedure_medication]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ipd_procedure_medication](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[ProcedureId] [numeric](18, 0) NOT NULL,
	[AppointmentId] [numeric](18, 0) NOT NULL,
	[PatientId] [numeric](18, 0) NOT NULL,
	[MedicineId] [numeric](18, 0) NOT NULL,
	[Quantity] [decimal](18, 2) NOT NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_ipd_procedure_medication] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ipd_procedure_mf]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ipd_procedure_mf](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[AdmissionId] [numeric](18, 0) NOT NULL,
	[AppointmentId] [numeric](18, 0) NOT NULL,
	[PatientId] [numeric](18, 0) NOT NULL,
	[PatientProcedure] [nvarchar](max) NULL,
	[Date] [datetime2](7) NOT NULL,
	[Time] [time](7) NULL,
	[CPTCodeId] [int] NULL,
	[CPTCodeDropdownId] [int] NULL,
	[Location] [nvarchar](max) NULL,
	[Physician] [nvarchar](max) NULL,
	[Assistant] [nvarchar](max) NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_ipd_procedure_mf] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pr_allowance]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pr_allowance](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[CategoryDropDownID] [int] NOT NULL,
	[CategoryID] [int] NOT NULL,
	[AllowanceName] [nvarchar](max) NULL,
	[AllowanceType] [nvarchar](max) NULL,
	[AllowanceValue] [float] NOT NULL,
	[Taxable] [bit] NOT NULL,
	[Default] [bit] NOT NULL,
	[Formula] [bit] NOT NULL,
	[SystemGenerated] [bit] NOT NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_pr_allowance] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pr_deduction_contribution]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pr_deduction_contribution](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[Category] [nvarchar](max) NULL,
	[DeductionContributionName] [nvarchar](max) NULL,
	[DeductionContributionType] [nvarchar](max) NULL,
	[DeductionContributionValue] [float] NOT NULL,
	[Default] [bit] NOT NULL,
	[Taxable] [bit] NOT NULL,
	[StartingBalance] [bit] NOT NULL,
	[SystemGenerated] [bit] NOT NULL,
	[Formula] [bit] NOT NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_pr_deduction_contribution] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pr_department]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pr_department](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[DepartmentName] [nvarchar](max) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_pr_department] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pr_designation]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pr_designation](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[DesignationName] [nvarchar](max) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_pr_designation] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pr_employee_Action_dt]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pr_employee_Action_dt](
	[EmployeeActionDtID] [numeric](18, 0) NOT NULL,
	[EmployeeActionMfID] [numeric](18, 0) NOT NULL,
	[CompanyID] [numeric](18, 0) NOT NULL,
	[EmployeeID] [numeric](18, 0) NOT NULL,
	[AttachmentPath] [nvarchar](200) NULL,
	[FileName] [nvarchar](200) NULL,
	[Description] [nvarchar](max) NULL,
	[AttchmentRemarks] [nvarchar](max) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pr_employee_Action_mf]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pr_employee_Action_mf](
	[EmployeeActionMfID] [numeric](18, 0) NOT NULL,
	[EmployeeID] [numeric](18, 0) NOT NULL,
	[CompanyID] [numeric](18, 0) NOT NULL,
	[HireDate] [datetime] NULL,
	[JoiningDate] [datetime] NULL,
	[InfoTypeDropDownID] [int] NULL,
	[InfoTypeID] [int] NULL,
	[EmployeeTypeDropDownID] [int] NULL,
	[EmployeeTypeID] [int] NULL,
	[TypeStartDate] [datetime] NULL,
	[TypeEndDate] [datetime] NULL,
	[LocationID] [numeric](18, 0) NULL,
	[DesignationID] [numeric](18, 0) NULL,
	[DepartmentID] [numeric](18, 0) NULL,
	[PayScheduleID] [numeric](18, 0) NULL,
	[LineManagerID] [numeric](18, 0) NULL,
	[PayrollEffectiveDate] [datetime] NULL,
	[ShiftID] [numeric](18, 0) NULL,
	[PayTypeDropDownID] [int] NULL,
	[PayTypeID] [int] NULL,
	[BasicSalary] [float] NULL,
	[PaymentMethodDropDownID] [int] NULL,
	[PaymentMethodID] [int] NULL,
	[Remarks] [nvarchar](max) NULL,
	[Post] [bit] NOT NULL,
	[ID] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime] NULL,
	[ContractTypeDropDownID] [int] NULL,
	[ContractTypeID] [int] NULL,
	[StatusID] [int] NULL,
	[TerminatedDate] [datetime] NULL,
	[FinalSettlementDate] [datetime] NULL,
	[OldHireDate] [datetime] NULL,
	[OldJoiningDate] [datetime] NULL,
	[OldInfoTypeID] [int] NULL,
	[OldEmployeeTypeID] [int] NULL,
	[OldTypeStartDate] [datetime] NULL,
	[OldTypeEndDate] [datetime] NULL,
	[OldLocationID] [numeric](18, 0) NULL,
	[OldDesignationID] [numeric](18, 0) NULL,
	[OldDepartmentID] [numeric](18, 0) NULL,
	[OldPayScheduleID] [numeric](18, 0) NULL,
	[OldLineManagerID] [numeric](18, 0) NULL,
	[OldEffectiveDate] [datetime] NULL,
	[OldPayrollEffectiveDate] [datetime] NULL,
	[OldShiftID] [numeric](18, 0) NULL,
	[OldPayTypeID] [int] NULL,
	[OldBasicSalary] [float] NULL,
	[OldStructureID] [numeric](18, 0) NULL,
	[OldPaymentMethodID] [int] NULL,
	[OldSubContractCompanyName] [nvarchar](max) NULL,
	[OldContractTypeID] [int] NULL,
	[OldStatusID] [int] NULL,
	[OldTerminatedDate] [datetime] NULL,
	[OldFinalSettlementDate] [datetime] NULL,
	[OldIsActive] [bit] NULL,
	[OldNetSalary] [numeric](18, 0) NULL,
	[OldTGrossSalary] [numeric](18, 0) NULL,
	[OldTaxPerPeriod] [numeric](18, 0) NULL,
	[IsActive] [bit] NULL,
	[ExculdeFromPayroll] [bit] NULL,
	[OldExculdeFromPayroll] [bit] NULL,
	[ActionEffectiveFrom] [datetime] NULL,
	[PaidEffectiveDate] [datetime] NULL,
	[UseAsDefault] [bit] NULL,
	[ApprovalStatusID] [nvarchar](10) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pr_employee_allowance]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pr_employee_allowance](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[EmployeeID] [numeric](18, 0) NOT NULL,
	[EffectiveFrom] [datetime2](7) NOT NULL,
	[EffectiveTo] [datetime2](7) NULL,
	[PayScheduleID] [numeric](18, 0) NOT NULL,
	[AllowanceID] [numeric](18, 0) NOT NULL,
	[Percentage] [float] NOT NULL,
	[Amount] [float] NOT NULL,
	[Taxable] [bit] NOT NULL,
	[IsHouseOrTransAllow] [bit] NULL,
 CONSTRAINT [PK_pr_employee_allowance] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pr_employee_arrears_mf]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pr_employee_arrears_mf](
	[ID] [numeric](18, 0) IDENTITY(1,1) NOT NULL,
	[CompanyID] [numeric](18, 0) NOT NULL,
	[EmployeeID] [numeric](18, 0) NOT NULL,
	[OldBasicSalary] [numeric](18, 0) NOT NULL,
	[NewBasicSalary] [numeric](18, 0) NOT NULL,
	[ArrearsAmount] [numeric](18, 0) NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[ActionEffectiveDate] [datetime] NOT NULL,
	[PaidEffectiveDate] [datetime] NOT NULL,
	[IsPaid] [bit] NOT NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ArrearReleatedMonth] [datetime] NOT NULL,
	[PayrollID] [numeric](18, 0) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pr_employee_ded_contribution]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pr_employee_ded_contribution](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[EmployeeID] [numeric](18, 0) NOT NULL,
	[EffectiveFrom] [datetime2](7) NOT NULL,
	[EffectiveTo] [datetime2](7) NULL,
	[PayScheduleID] [numeric](18, 0) NOT NULL,
	[Category] [nvarchar](max) NULL,
	[DeductionContributionID] [numeric](18, 0) NOT NULL,
	[Percentage] [float] NOT NULL,
	[Amount] [float] NOT NULL,
	[StartingBalance] [float] NOT NULL,
	[Taxable] [bit] NULL,
 CONSTRAINT [PK_pr_employee_ded_contribution] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pr_employee_Dependent]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pr_employee_Dependent](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[EmployeeID] [numeric](18, 0) NOT NULL,
	[RelationshipDropdownID] [int] NULL,
	[RelationshipTypeID] [int] NULL,
	[IsEmergencyContact] [bit] NULL,
	[IsTicketEligible] [bit] NULL,
	[FirstName] [nvarchar](max) NULL,
	[LastName] [nvarchar](max) NULL,
	[Gender] [nvarchar](max) NULL,
	[NationalityTypeDropdownID] [int] NULL,
	[NationalityTypeID] [int] NULL,
	[IdentificationNumber] [nvarchar](max) NULL,
	[PassportNumber] [nvarchar](max) NULL,
	[MaritalStatusTypeDropdownID] [int] NULL,
	[MaritalStatusTypeID] [int] NULL,
	[DOB] [datetime2](7) NULL,
	[Remarks] [nvarchar](max) NULL,
 CONSTRAINT [PK_pr_employee_Dependent] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pr_employee_document]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pr_employee_document](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[EmployeeID] [numeric](18, 0) NOT NULL,
	[DocumentTypeID] [int] NOT NULL,
	[DocumentTypeDropdownID] [int] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[AttachmentPath] [nvarchar](max) NULL,
	[UploadDate] [datetime2](7) NOT NULL,
	[ExpireDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_pr_employee_document] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pr_employee_leave]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pr_employee_leave](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[EmployeeID] [numeric](18, 0) NOT NULL,
	[LeaveTypeID] [numeric](18, 0) NOT NULL,
	[Hours] [float] NOT NULL,
 CONSTRAINT [PK_pr_employee_leave] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pr_employee_mf]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pr_employee_mf](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[FirstName] [nvarchar](max) NULL,
	[LastName] [nvarchar](max) NULL,
	[Gender] [nvarchar](max) NULL,
	[DOB] [datetime2](7) NULL,
	[StreetAddress] [nvarchar](max) NULL,
	[CityDropDownID] [int] NULL,
	[CityID] [int] NULL,
	[ZipCode] [nvarchar](max) NULL,
	[CountryDropDownID] [int] NULL,
	[CountryID] [int] NULL,
	[Email] [nvarchar](max) NULL,
	[HomePhone] [nvarchar](max) NULL,
	[Mobile] [nvarchar](max) NULL,
	[EmergencyContactPerson] [nvarchar](max) NULL,
	[EmergencyContactNo] [nvarchar](max) NULL,
	[HireDate] [datetime2](7) NULL,
	[JoiningDate] [datetime2](7) NULL,
	[PayTypeDropDownID] [int] NOT NULL,
	[PayTypeID] [int] NOT NULL,
	[BasicSalary] [float] NULL,
	[StatusDropDownID] [int] NOT NULL,
	[StatusID] [int] NOT NULL,
	[TerminatedDate] [datetime2](7) NULL,
	[FinalSettlementDate] [datetime2](7) NULL,
	[PaymentMethodDropDownID] [int] NOT NULL,
	[PaymentMethodID] [int] NOT NULL,
	[SpecialtyTypeDropdownID] [int] NULL,
	[SpecialtyTypeID] [int] NULL,
	[ClassificationTypeDropdownID] [int] NULL,
	[ClassificationTypeID] [int] NULL,
	[BankName] [nvarchar](max) NULL,
	[BranchName] [nvarchar](max) NULL,
	[BranchCode] [nvarchar](max) NULL,
	[AccountNo] [nvarchar](max) NULL,
	[SwiftCode] [nvarchar](max) NULL,
	[EmployeeTypeDropDownID] [int] NOT NULL,
	[EmployeeTypeID] [int] NOT NULL,
	[TypeStartDate] [datetime2](7) NULL,
	[TypeEndDate] [datetime2](7) NULL,
	[DesignationID] [numeric](18, 0) NULL,
	[DepartmentID] [numeric](18, 0) NULL,
	[NICNoExpiryDate] [datetime2](7) NULL,
	[NICNo] [nvarchar](max) NULL,
	[NationalSecurityNo] [nvarchar](max) NULL,
	[NationalityDropDownID] [int] NULL,
	[NationalityID] [int] NULL,
	[EmployeeCode] [nvarchar](max) NULL,
	[PayScheduleID] [numeric](18, 0) NULL,
	[EmployeePic] [nvarchar](max) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[SubContractCompanyName] [nvarchar](max) NULL,
	[PassportNumber] [nvarchar](max) NULL,
	[PassportExpiryDate] [datetime2](7) NULL,
	[SCHSNO] [nvarchar](max) NULL,
	[SCHSNOExpiryDate] [datetime2](7) NULL,
	[MedicalInsuranceProvided] [nvarchar](max) NULL,
	[InsuranceCardNo] [nvarchar](max) NULL,
	[InsuranceCardNoExpiryDate] [datetime2](7) NULL,
	[InsuranceClassTypeDropdownID] [int] NULL,
	[InsuranceClassTypeID] [int] NULL,
	[AirTicketProvided] [nvarchar](max) NULL,
	[NoOfAirTicket] [int] NULL,
	[AirTicketClassTypeDropdownID] [int] NULL,
	[AirTicketClassTypeID] [int] NULL,
	[AirTicketFrequencyTypeDropdownID] [int] NULL,
	[AirTicketFrequencyTypeID] [int] NULL,
	[OriginCountryDropdownTypeID] [int] NULL,
	[OriginCountryTypeID] [int] NULL,
	[DestinationCountryDropdownTypeID] [int] NULL,
	[DestinationCountryTypeID] [int] NULL,
	[OriginCityDropdownTypeID] [int] NULL,
	[OriginCityTypeID] [int] NULL,
	[DestinationCityDropdownTypeID] [int] NULL,
	[DestinationCityTypeID] [int] NULL,
	[AirTicketRemarks] [nvarchar](max) NULL,
	[MaritalStatusTypeDropdownID] [int] NULL,
	[MaritalStatusTypeID] [int] NULL,
	[ContractTypeDropDownID] [int] NULL,
	[ContractTypeID] [int] NULL,
	[TotalPolicyAmountMonthly] [int] NULL,
	[ExculdeFromPayroll] [bit] NULL,
	[EffectiveDate] [datetime] NULL,
	[IsActive] [bit] NULL,
	[TimeRule] [bit] NULL,
	[LastArrival] [bit] NULL,
	[ShiftID] [numeric](18, 0) NULL,
	[EarlyOut] [bit] NULL,
	[LeaveRestrictions] [bit] NULL,
	[MissingAttendance] [bit] NULL,
 CONSTRAINT [PK_pr_employee_mf] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pr_employee_payroll_dt]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pr_employee_payroll_dt](
	[ID] [numeric](18, 0) NOT NULL,
	[PayrollID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[PayScheduleID] [numeric](18, 0) NOT NULL,
	[PayDate] [date] NOT NULL,
	[EmployeeID] [numeric](18, 0) NOT NULL,
	[Type] [nvarchar](max) NULL,
	[AllowDedID] [int] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[Taxable] [bit] NOT NULL,
	[AdjustmentDate] [datetime2](7) NULL,
	[AdjustmentType] [nvarchar](max) NULL,
	[AdjustmentAmount] [decimal](18, 2) NULL,
	[AdjustmentComments] [nvarchar](max) NULL,
	[AdjustmentBy] [decimal](18, 2) NULL,
	[RefID] [decimal](18, 2) NOT NULL,
	[Remarks] [nvarchar](50) NULL,
	[ArrearReleatedMonth] [datetime] NULL,
 CONSTRAINT [PK_pr_employee_payroll_dt] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pr_employee_payroll_mf]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pr_employee_payroll_mf](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[PayScheduleID] [numeric](18, 0) NOT NULL,
	[PayDate] [date] NOT NULL,
	[EmployeeID] [numeric](18, 0) NOT NULL,
	[PayScheduleFromDate] [datetime2](7) NOT NULL,
	[PayScheduleToDate] [datetime2](7) NOT NULL,
	[FromDate] [datetime2](7) NOT NULL,
	[ToDate] [datetime2](7) NOT NULL,
	[BasicSalary] [decimal](18, 2) NULL,
	[Status] [nvarchar](max) NULL,
	[AdjustmentDate] [datetime2](7) NULL,
	[AdjustmentType] [nvarchar](max) NULL,
	[AdjustmentAmount] [decimal](18, 2) NULL,
	[AdjustmentComments] [nvarchar](max) NULL,
	[AdjustmentBy] [decimal](18, 2) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_pr_employee_payroll_mf] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pr_employee_shift]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pr_employee_shift](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyID] [numeric](18, 0) NOT NULL,
	[ShiftName] [nvarchar](500) NOT NULL,
	[GPSLocationEnable] [bit] NULL,
	[GPSDistance] [float] NULL,
	[GPSCoordinate] [nvarchar](200) NULL,
	[StartTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
	[WDMonday] [bit] NOT NULL,
	[WDTuesday] [bit] NOT NULL,
	[WDWednesday] [bit] NOT NULL,
	[WDThursday] [bit] NOT NULL,
	[WDFriday] [bit] NOT NULL,
	[WDSatuday] [bit] NOT NULL,
	[WDSunday] [bit] NOT NULL,
	[WorkingHour] [varchar](50) NULL,
	[MissingCheckOutHours] [varchar](50) NULL,
	[OvertimeStartLimit] [varchar](50) NULL,
	[MaximumOvertimeLimit] [varchar](50) NULL,
 CONSTRAINT [PK_pr_employee_shift] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pr_expense_application]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pr_expense_application](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyID] [numeric](18, 0) NOT NULL,
	[EmployeeID] [numeric](18, 0) NOT NULL,
	[Date] [datetime] NOT NULL,
	[TotalAmount] [numeric](18, 0) NOT NULL,
	[Document] [nvarchar](max) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime] NULL,
	[Reason] [nvarchar](max) NULL,
	[ExpenseTypeID] [numeric](18, 0) NULL,
	[ApprovalStatusID] [nvarchar](50) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pr_leave_application]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pr_leave_application](
	[ID] [numeric](18, 0) NOT NULL,
	[FromDate] [datetime2](7) NOT NULL,
	[ToDate] [datetime2](7) NOT NULL,
	[Hours] [float] NOT NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[EmployeeID] [numeric](18, 0) NOT NULL,
	[LeaveTypeID] [numeric](18, 0) NOT NULL,
 CONSTRAINT [PK_pr_leave_application] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pr_leave_type]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pr_leave_type](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[Category] [nvarchar](max) NULL,
	[TypeName] [nvarchar](max) NULL,
	[AccuralDropDownID] [int] NOT NULL,
	[AccrualFrequencyID] [int] NOT NULL,
	[EarnedValue] [float] NOT NULL,
	[SystemGenerated] [bit] NOT NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_pr_leave_type] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pr_loan]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pr_loan](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[EmployeeID] [numeric](18, 0) NOT NULL,
	[PaymentMethodDropdownID] [int] NULL,
	[PaymentMethodID] [int] NULL,
	[LoanTypeDropdownID] [int] NULL,
	[LoanTypeID] [int] NOT NULL,
	[LoanCode] [int] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[PaymentStartDate] [datetime2](7) NOT NULL,
	[LoanDate] [datetime2](7) NOT NULL,
	[LoanAmount] [float] NOT NULL,
	[DeductionType] [nvarchar](max) NULL,
	[DeductionValue] [float] NOT NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
	[AdjustmentDate] [datetime2](7) NULL,
	[AdjustmentType] [nvarchar](max) NULL,
	[AdjustmentAmount] [decimal](18, 2) NULL,
	[AdjustmentComments] [nvarchar](max) NULL,
	[AdjustmentBy] [decimal](18, 2) NULL,
	[InstallmentByBaseSalary] [float] NULL,
	[ApprovalStatusID] [nvarchar](50) NULL,
 CONSTRAINT [PK_pr_loan] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pr_loan_payment_dt]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pr_loan_payment_dt](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[LoanID] [numeric](18, 0) NOT NULL,
	[PaymentDate] [datetime2](7) NOT NULL,
	[Comment] [nvarchar](max) NULL,
	[Amount] [float] NOT NULL,
	[AdjustmentDate] [datetime2](7) NULL,
	[AdjustmentType] [nvarchar](max) NULL,
	[AdjustmentAmount] [decimal](18, 2) NULL,
	[AdjustmentComments] [nvarchar](max) NULL,
	[AdjustmentBy] [decimal](18, 2) NULL,
 CONSTRAINT [PK_pr_loan_payment_dt] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pr_pay_schedule]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pr_pay_schedule](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[PayTypeDropDownID] [int] NOT NULL,
	[PayTypeID] [int] NOT NULL,
	[ScheduleName] [nvarchar](max) NULL,
	[PeriodStartDate] [date] NOT NULL,
	[PeriodEndDate] [date] NOT NULL,
	[FallInHolidayDropDownID] [int] NOT NULL,
	[FallInHolidayID] [int] NOT NULL,
	[PayDate] [date] NOT NULL,
	[Lock] [bit] NOT NULL,
	[Active] [bit] NOT NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_pr_pay_schedule] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pr_time_entry]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pr_time_entry](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[EmployeeID] [numeric](18, 0) NOT NULL,
	[AttendanceDate] [datetime2](7) NOT NULL,
	[LocationID] [numeric](18, 0) NULL,
	[TimeIn] [datetime2](7) NOT NULL,
	[TimeOut] [datetime2](7) NOT NULL,
	[StatusDropDownID] [int] NOT NULL,
	[StatusID] [int] NOT NULL,
	[Remarks] [nvarchar](max) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_pr_time_entry] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pr_time_rule_dt]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pr_time_rule_dt](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyID] [numeric](18, 0) NOT NULL,
	[TimeRuleMfId] [numeric](18, 0) NOT NULL,
	[DeductDays] [numeric](18, 0) NULL,
	[Percentage] [numeric](18, 0) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_pr_time_rule_dt] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pr_time_rule_mf]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pr_time_rule_mf](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyID] [numeric](18, 0) NOT NULL,
	[MA_AutomaticallyDeductLeave] [bit] NULL,
	[MA_DeductDayLeave] [numeric](18, 0) NULL,
	[MA_MissingDayAfter] [numeric](18, 0) NULL,
	[MA_IsPaidLeave] [bit] NULL,
	[LA_SystemDeductLeave] [bit] NULL,
	[LA_GracePeriod] [numeric](18, 0) NULL,
	[LA_MaximumDayLate] [numeric](18, 0) NULL,
	[LA_IncidentsType] [nvarchar](50) NULL,
	[LA_EveryIncidents] [numeric](18, 0) NULL,
	[LA_IncidentsLeaveDays] [numeric](18, 0) NULL,
	[LA_IsPaidLeave] [bit] NULL,
	[HD_DeductLeaveShortage] [nvarchar](50) NULL,
	[HD_DeductBasedCriteria] [bit] NULL,
	[HD_IsPaidLeave] [bit] NULL,
	[LR_Deduct] [bit] NULL,
	[LR_MissingSwipeDays] [numeric](18, 0) NULL,
	[LR_MissingSwipeType] [nvarchar](50) NULL,
	[LR_MissingExceedDay] [numeric](18, 0) NULL,
	[LR_IgonreRule] [bit] NULL,
	[LR_MissingHourPercentage] [numeric](18, 0) NULL,
	[LR_IsPaidLeave] [bit] NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime] NULL,
	[StandardPenaltyMinute] [numeric](18, 0) NULL,
	[StandardPenaltyCharges] [numeric](18, 0) NULL,
	[SpecialPenaltyMinute] [numeric](18, 0) NULL,
	[SpecialPenaltyCharges] [numeric](18, 0) NULL,
	[SpecialRestPenaltyMinute] [numeric](18, 0) NULL,
	[SpecialRestPenaltyCharges] [numeric](18, 0) NULL,
	[PenaltyType] [bit] NULL,
	[PenaltySalaryRange] [numeric](18, 0) NULL,
	[LateDeductionPenaltyType] [bit] NULL,
	[LateDeductionHourType] [bit] NULL,
	[OvertimeEffect] [varchar](100) NULL,
	[IsOverTimeSelect] [varchar](2) NULL,
	[IsHoulyWageBased] [bit] NULL,
	[IsHoulyPercentageBased] [bit] NULL,
	[ActiveHoursInaDayPolicyID] [bit] NULL,
	[WageDeductionTypeID] [bit] NULL,
	[LateDeductionWageRateType] [bit] NULL,
	[LAWageRateType] [bit] NULL,
	[EOSystemDeductLeave] [bit] NULL,
	[EOWageRateType] [bit] NULL,
 CONSTRAINT [PK_pr_time_rule_mf_1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pr_time_Summary]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pr_time_Summary](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyID] [numeric](18, 0) NOT NULL,
	[EmployeeID] [numeric](18, 0) NOT NULL,
	[AttendanceDate] [date] NOT NULL,
	[TimeIn] [datetime] NULL,
	[TimeOut] [datetime] NULL,
	[StatusDropDownID] [int] NOT NULL,
	[StatusID] [int] NOT NULL,
	[EffectiveMinute] [int] NOT NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime] NULL,
	[IsApprove] [bit] NULL,
 CONSTRAINT [PK_pr_time_Summary] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pur_invoice_dt]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pur_invoice_dt](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[InvoiceID] [numeric](18, 0) NOT NULL,
	[ItemID] [numeric](18, 0) NOT NULL,
	[ItemDescription] [nvarchar](max) NULL,
	[Quantity] [decimal](18, 2) NULL,
	[Rate] [decimal](18, 2) NULL,
	[Amount] [decimal](18, 2) NULL,
	[Discount] [decimal](18, 2) NULL,
	[DiscountAmount] [decimal](18, 2) NULL,
	[CreatedBy] [numeric](18, 0) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[BatchSarialNumber] [int] NULL,
	[ExpiredWarrantyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_pur_invoice_dt] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pur_invoice_mf]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pur_invoice_mf](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[VendorID] [numeric](18, 0) NOT NULL,
	[BillNo] [decimal](18, 2) NOT NULL,
	[OrderNo] [nvarchar](max) NULL,
	[BillDate] [datetime2](7) NOT NULL,
	[DueDate] [datetime2](7) NOT NULL,
	[Total] [decimal](18, 2) NULL,
	[DiscountAmount] [decimal](18, 2) NULL,
	[Discount] [decimal](18, 2) NULL,
	[IsItemLevelDiscount] [bit] NOT NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[SaveStatus] [int] NOT NULL,
 CONSTRAINT [PK_pur_invoice_mf] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pur_payment]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pur_payment](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[InvoiveId] [numeric](18, 0) NULL,
	[PaymentMethodDropdownID] [int] NULL,
	[PaymentMethodID] [int] NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[PaymentDate] [datetime2](7) NOT NULL,
	[Notes] [nvarchar](max) NOT NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_pur_payment] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pur_sale_dt]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pur_sale_dt](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[SaleID] [numeric](18, 0) NOT NULL,
	[ItemID] [numeric](18, 0) NOT NULL,
	[ItemDescription] [nvarchar](max) NULL,
	[Quantity] [decimal](18, 2) NULL,
	[Rate] [decimal](18, 2) NULL,
	[DiscountType] [int] NOT NULL,
	[Discount] [decimal](18, 2) NULL,
	[DiscountAmount] [decimal](18, 2) NULL,
	[TotalAmount] [decimal](18, 2) NULL,
	[CreatedBy] [numeric](18, 0) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[BatchSarialNumber] [int] NULL,
	[ExpiredWarrantyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_pur_sale_dt] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pur_sale_mf]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pur_sale_mf](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[CustomerId] [numeric](18, 0) NOT NULL,
	[ReturnInvoiceId] [numeric](18, 0) NULL,
	[Date] [datetime2](7) NOT NULL,
	[SubTotal] [decimal](18, 2) NULL,
	[Discount] [decimal](18, 2) NULL,
	[TaxAmount] [decimal](18, 2) NULL,
	[Total] [decimal](18, 2) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[SaleTypeID] [int] NOT NULL,
	[SaleTypeDropDownId] [int] NOT NULL,
 CONSTRAINT [PK_pur_sale_mf] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pur_vendor]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pur_vendor](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[FirstName] [nvarchar](max) NULL,
	[LastName] [nvarchar](max) NULL,
	[CompanyName] [nvarchar](max) NULL,
	[VendorPhone] [nvarchar](max) NULL,
	[VendorEmail] [nvarchar](max) NULL,
	[Address] [nvarchar](max) NULL,
	[Address2] [nvarchar](max) NULL,
	[City] [nvarchar](max) NULL,
	[State] [nvarchar](max) NULL,
	[ZipCode] [nvarchar](max) NULL,
	[Phone] [nvarchar](max) NULL,
	[Fax] [nvarchar](max) NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_pur_vendor] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sys_drop_down_mf]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_drop_down_mf](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](max) NULL,
 CONSTRAINT [PK_sys_drop_down_mf] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sys_drop_down_value]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_drop_down_value](
	[ID] [int] NOT NULL,
	[DropDownID] [int] NOT NULL,
	[Value] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[DependedDropDownID] [int] NULL,
	[DependedDropDownValueID] [int] NULL,
	[SystemGenerated] [bit] NULL,
	[CompanyId] [bigint] NULL,
	[Unit] [nvarchar](max) NULL,
 CONSTRAINT [PK_sys_drop_down_value] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[DropDownID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sys_holidays]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_holidays](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[HolidayName] [nvarchar](max) NULL,
	[FromDate] [datetime2](7) NOT NULL,
	[ToDate] [datetime2](7) NOT NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_sys_holidays] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sys_notification_alert]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_notification_alert](
	[ID] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[TypeID] [int] NOT NULL,
	[EmailFrom] [nvarchar](max) NULL,
	[EmailTo] [nvarchar](max) NULL,
	[Subject] [nvarchar](max) NULL,
	[Body] [nvarchar](max) NULL,
	[SentTime] [datetime2](7) NULL,
	[FailureCount] [int] NOT NULL,
	[IsRead] [bit] NOT NULL,
	[AttachmentPath] [nvarchar](max) NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_sys_notification_alert] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sys_setting]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_setting](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SettingIdOrName] [varchar](500) NOT NULL,
	[SettingIdOrNameValue] [nvarchar](max) NOT NULL,
	[SettingIdOrNameDepAllowValue] [varchar](500) NULL,
	[SettingIdOrNameDepDedValue] [varchar](500) NULL,
 CONSTRAINT [PK_sys_setting] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[user_payment]    Script Date: 9/6/2024 5:45:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_payment](
	[ID] [numeric](18, 0) NOT NULL,
	[UserId] [numeric](18, 0) NOT NULL,
	[CompanyId] [numeric](18, 0) NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[Remarks] [nvarchar](max) NULL,
	[Date] [datetime2](7) NOT NULL,
	[CreatedBy] [numeric](18, 0) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedBy] [numeric](18, 0) NULL,
	[ModifiedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_user_payment] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240623134517__init', N'5.0.10')
GO
INSERT [dbo].[adm_company] ([ID], [CompanyName], [CompanyTypeDropDownID], [CompanyTypeID], [ReceiptFooter], [GenderID], [ContactPersonFirstName], [IsCNICMandatory], [ContactPersonLastName], [IsShowBillReceptionist], [CompanyAddress1], [CompanyAddress2], [LanguageID], [CityDropDownId], [CompanyLogo], [CountryDropdownId], [Phone], [Fax], [PostalCode], [Province], [Website], [Email], [IsTrialVersion], [IsBackDatedAppointment], [IsUpdateBillDate], [DateFormatDropDownID], [DateFormatId], [SalaryMethodID], [WDMonday], [WDTuesday], [WDWednesday], [WDThursday], [WDFriday], [WDSatuday], [WDSunday], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), N'Ahsan Clinic', 9, 14, NULL, 1, N'Saleem', 0, N'Saleem', 0, NULL, NULL, NULL, 6, N'2022052517005767720358.jpg', 1, N'9230004854', NULL, NULL, NULL, NULL, N'mohsinshahbaz@gmail.com', 1, 1, NULL, 36, 1, NULL, 0, 0, 0, 0, 0, 0, 0, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.3500000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-12-17T02:13:07.7533333' AS DateTime2))
INSERT [dbo].[adm_company] ([ID], [CompanyName], [CompanyTypeDropDownID], [CompanyTypeID], [ReceiptFooter], [GenderID], [ContactPersonFirstName], [IsCNICMandatory], [ContactPersonLastName], [IsShowBillReceptionist], [CompanyAddress1], [CompanyAddress2], [LanguageID], [CityDropDownId], [CompanyLogo], [CountryDropdownId], [Phone], [Fax], [PostalCode], [Province], [Website], [Email], [IsTrialVersion], [IsBackDatedAppointment], [IsUpdateBillDate], [DateFormatDropDownID], [DateFormatId], [SalaryMethodID], [WDMonday], [WDTuesday], [WDWednesday], [WDThursday], [WDFriday], [WDSatuday], [WDSunday], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(2 AS Numeric(18, 0)), N'Medicare Clinic', 9, NULL, NULL, 1, N'Mohsin Shahbaz', 0, N'Mohsin Shahbaz', 0, NULL, NULL, NULL, 56, NULL, 1, N'923333', NULL, NULL, NULL, NULL, N'mohsin@gmail.com', 1, 0, NULL, 36, 1, NULL, 0, 0, 0, 0, 0, 0, 0, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.6800000' AS DateTime2), CAST(1 AS Numeric(18, 0)), NULL)
INSERT [dbo].[adm_company] ([ID], [CompanyName], [CompanyTypeDropDownID], [CompanyTypeID], [ReceiptFooter], [GenderID], [ContactPersonFirstName], [IsCNICMandatory], [ContactPersonLastName], [IsShowBillReceptionist], [CompanyAddress1], [CompanyAddress2], [LanguageID], [CityDropDownId], [CompanyLogo], [CountryDropdownId], [Phone], [Fax], [PostalCode], [Province], [Website], [Email], [IsTrialVersion], [IsBackDatedAppointment], [IsUpdateBillDate], [DateFormatDropDownID], [DateFormatId], [SalaryMethodID], [WDMonday], [WDTuesday], [WDWednesday], [WDThursday], [WDFriday], [WDSatuday], [WDSunday], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(3 AS Numeric(18, 0)), N'test', 9, NULL, NULL, 1, N'test', 0, N'test', 0, NULL, NULL, NULL, 56, NULL, 1, N'656565656', NULL, NULL, NULL, NULL, N'salee@gmail.com', 1, 0, NULL, 36, 1, NULL, 0, 0, 0, 0, 0, 0, 0, CAST(1 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.3000000' AS DateTime2), CAST(1 AS Numeric(18, 0)), NULL)
INSERT [dbo].[adm_company] ([ID], [CompanyName], [CompanyTypeDropDownID], [CompanyTypeID], [ReceiptFooter], [GenderID], [ContactPersonFirstName], [IsCNICMandatory], [ContactPersonLastName], [IsShowBillReceptionist], [CompanyAddress1], [CompanyAddress2], [LanguageID], [CityDropDownId], [CompanyLogo], [CountryDropdownId], [Phone], [Fax], [PostalCode], [Province], [Website], [Email], [IsTrialVersion], [IsBackDatedAppointment], [IsUpdateBillDate], [DateFormatDropDownID], [DateFormatId], [SalaryMethodID], [WDMonday], [WDTuesday], [WDWednesday], [WDThursday], [WDFriday], [WDSatuday], [WDSunday], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(4 AS Numeric(18, 0)), N'Ahsan Clinic', 9, 14, NULL, 1, N'mohsin shahbaz', 0, N'mohsin shahbaz', 0, N'Nizam Pura (West)', N'Street # 4', NULL, 6, N'', 1, N'3001820818', NULL, N'62300', N'Punjab', NULL, N'mohsinshahbaz@gmail.com', 1, 1, NULL, 36, 1, NULL, 0, 0, 0, 0, 0, 0, 0, CAST(1 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8066667' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2023-05-21T10:28:02.8766667' AS DateTime2))
INSERT [dbo].[adm_company] ([ID], [CompanyName], [CompanyTypeDropDownID], [CompanyTypeID], [ReceiptFooter], [GenderID], [ContactPersonFirstName], [IsCNICMandatory], [ContactPersonLastName], [IsShowBillReceptionist], [CompanyAddress1], [CompanyAddress2], [LanguageID], [CityDropDownId], [CompanyLogo], [CountryDropdownId], [Phone], [Fax], [PostalCode], [Province], [Website], [Email], [IsTrialVersion], [IsBackDatedAppointment], [IsUpdateBillDate], [DateFormatDropDownID], [DateFormatId], [SalaryMethodID], [WDMonday], [WDTuesday], [WDWednesday], [WDThursday], [WDFriday], [WDSatuday], [WDSunday], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(5 AS Numeric(18, 0)), N'Testing', 9, NULL, NULL, 1, N'saleem', 0, N'saleem', 0, NULL, NULL, NULL, 56, NULL, 1, N'2121212', NULL, NULL, NULL, NULL, N'saleem86481@gmail.com', 1, 0, NULL, 36, 1, NULL, 0, 0, 0, 0, 0, 0, 0, CAST(1 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:54.9533333' AS DateTime2), CAST(1 AS Numeric(18, 0)), NULL)
GO
INSERT [dbo].[adm_item] ([ID], [CompanyId], [Name], [SKU], [Image], [UnitDropDownID], [UnitID], [ItemTypeDropDownID], [ItemTypeId], [CategoryDropDownID], [CategoryID], [TrackInventory], [IsActive], [CostPrice], [SalePrice], [InventoryOpeningStock], [InventoryStockPerUnit], [InventoryStockQuantity], [SaveStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'Test Service', NULL, N'', 57, 1, 59, 2, 58, 2, 0, 1, CAST(100.00 AS Decimal(18, 2)), CAST(200.00 AS Decimal(18, 2)), NULL, NULL, NULL, 1, CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-01T12:32:36.8904964' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-01T12:32:36.8904964' AS DateTime2))
INSERT [dbo].[adm_item] ([ID], [CompanyId], [Name], [SKU], [Image], [UnitDropDownID], [UnitID], [ItemTypeDropDownID], [ItemTypeId], [CategoryDropDownID], [CategoryID], [TrackInventory], [IsActive], [CostPrice], [SalePrice], [InventoryOpeningStock], [InventoryStockPerUnit], [InventoryStockQuantity], [SaveStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'test Batch', NULL, N'', 57, 2, 59, 3, 58, 1, 1, 1, CAST(10.00 AS Decimal(18, 2)), CAST(20.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), CAST(200.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 2, CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-01T12:49:08.5641818' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-02T17:11:51.4768427' AS DateTime2))
INSERT [dbo].[adm_item] ([ID], [CompanyId], [Name], [SKU], [Image], [UnitDropDownID], [UnitID], [ItemTypeDropDownID], [ItemTypeId], [CategoryDropDownID], [CategoryID], [TrackInventory], [IsActive], [CostPrice], [SalePrice], [InventoryOpeningStock], [InventoryStockPerUnit], [InventoryStockQuantity], [SaveStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(4 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'Test Sarial', NULL, N'', 57, 2, 59, 4, 58, 2, 1, 1, CAST(100.00 AS Decimal(18, 2)), CAST(200.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), CAST(2000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 2, CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-01T13:45:48.1741912' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-01T13:45:52.8674945' AS DateTime2))
INSERT [dbo].[adm_item] ([ID], [CompanyId], [Name], [SKU], [Image], [UnitDropDownID], [UnitID], [ItemTypeDropDownID], [ItemTypeId], [CategoryDropDownID], [CategoryID], [TrackInventory], [IsActive], [CostPrice], [SalePrice], [InventoryOpeningStock], [InventoryStockPerUnit], [InventoryStockQuantity], [SaveStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(5 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'sas', NULL, N'', 57, 1, 59, 1, 58, 2, 1, 1, CAST(50.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), CAST(1.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 2, CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-02T17:17:13.6431248' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-02T17:17:22.4389845' AS DateTime2))
INSERT [dbo].[adm_item] ([ID], [CompanyId], [Name], [SKU], [Image], [UnitDropDownID], [UnitID], [ItemTypeDropDownID], [ItemTypeId], [CategoryDropDownID], [CategoryID], [TrackInventory], [IsActive], [CostPrice], [SalePrice], [InventoryOpeningStock], [InventoryStockPerUnit], [InventoryStockQuantity], [SaveStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(6 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'test batch 2', NULL, N'', 57, 1, 59, 3, 58, 1, 1, 1, CAST(20.00 AS Decimal(18, 2)), CAST(50.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), CAST(500.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 2, CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-05T13:10:04.6347337' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-05T13:10:21.7193329' AS DateTime2))
GO
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 1, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.4366667' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.4366667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 2, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.4533333' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.4533333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 3, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.4566667' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.4566667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(4 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 4, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.4733333' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.4733333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(5 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 5, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.4766667' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.4766667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(6 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 6, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.4866667' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.4866667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(7 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 7, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.4933333' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.4933333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(8 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 8, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.4966667' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.4966667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(9 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 9, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5033333' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5033333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(10 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 10, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5066667' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5066667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(11 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 11, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5133333' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5133333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(12 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 12, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5166667' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5166667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(13 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 13, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5233333' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5233333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(14 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 14, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5233333' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5233333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(15 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 15, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5300000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5300000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(16 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 16, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5333333' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5333333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(17 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 17, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5366667' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5366667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(18 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 18, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5433333' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5433333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(19 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 19, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5466667' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5466667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(20 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 20, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5500000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5500000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(21 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 21, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5533333' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5533333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(22 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 22, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5533333' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5533333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(23 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 23, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5566667' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5566667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(24 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 24, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5600000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5600000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(25 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 25, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5633333' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5633333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(26 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 27, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5633333' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5633333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(27 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 28, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5666667' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5666667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(28 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 29, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5700000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5700000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(29 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 30, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5700000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5700000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(30 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 31, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5733333' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5733333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(31 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 32, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5733333' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5733333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(32 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 33, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5766667' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5766667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(33 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 34, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5800000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5800000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(34 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 35, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5800000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5800000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(35 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 36, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5833333' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5833333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(36 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 37, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5833333' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5833333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(37 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 38, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5866667' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5866667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(38 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 39, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5900000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5900000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(39 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 40, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5900000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5900000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(40 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 41, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5933333' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5933333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(41 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 42, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5966667' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.5966667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(42 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 43, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6000000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6000000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(43 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 44, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6000000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6000000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(44 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 45, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6033333' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6033333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(45 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 46, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6066667' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6066667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(46 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 47, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6066667' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6066667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(47 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 48, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6100000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(48 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 1, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6133333' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(49 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 2, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6200000' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(50 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 3, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6233333' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(51 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 4, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6300000' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(52 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 5, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6333333' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(53 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 6, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6366667' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(54 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 7, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6400000' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(55 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 8, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6433333' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(56 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 9, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6466667' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(57 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 10, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6500000' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(58 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 11, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6533333' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9466667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(59 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 12, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6566667' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(60 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 13, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6600000' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9466667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(61 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 14, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6633333' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(62 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 15, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6700000' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(63 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 16, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6733333' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(64 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 17, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6766667' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(65 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 18, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6800000' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(66 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 19, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6833333' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(67 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 20, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6866667' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9633333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(68 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 21, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6900000' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9633333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(69 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 22, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6933333' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9633333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(70 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 23, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6933333' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9633333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(71 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 24, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6966667' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9633333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(72 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 25, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7000000' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9633333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(73 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 27, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7033333' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(74 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 28, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7066667' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(75 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 29, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7100000' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(76 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 30, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7133333' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9633333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(77 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 31, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7133333' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9633333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(78 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 32, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7166667' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(79 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 33, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7200000' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9633333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(80 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 34, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7233333' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(81 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 35, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7266667' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(82 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 36, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7333333' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9633333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(83 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 37, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7366667' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(84 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 38, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7400000' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(85 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 39, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7433333' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(86 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 40, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7466667' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(87 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 41, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7500000' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(88 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 42, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7533333' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(89 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 43, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7566667' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(90 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 44, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7600000' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9800000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(91 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 45, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7633333' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9800000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(92 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 46, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7666667' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9800000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(93 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 47, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7700000' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9800000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(94 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 48, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7733333' AS DateTime2), CAST(12 AS Numeric(18, 0)), CAST(N'2022-12-17T02:21:15.9800000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(95 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 1, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7833333' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(96 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 2, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7866667' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(97 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 3, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7933333' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(98 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 4, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7966667' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(99 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 5, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7966667' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(100 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 6, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.8033333' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9466667' AS DateTime2), 0, 0, 0, 0)
GO
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(101 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 7, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.8066667' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(102 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 8, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.8100000' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(103 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 9, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.8133333' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(104 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 10, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.8166667' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(105 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 11, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.8200000' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(106 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 12, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.8233333' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(107 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 13, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.8266667' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9466667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(108 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 14, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.8300000' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9466667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(109 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 15, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.8333333' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(110 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 16, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.8400000' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(111 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 17, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.8433333' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(112 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 18, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.8466667' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(113 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 19, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.8500000' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(114 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 20, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.8566667' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(115 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 21, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.8600000' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(116 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 22, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.8633333' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(117 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 23, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.8733333' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(118 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 24, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.8800000' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(119 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 25, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.8866667' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(120 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 27, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.8900000' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(121 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 28, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.8933333' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(122 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 29, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.9000000' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(123 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 30, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.9033333' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(124 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 31, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.9066667' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(125 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 32, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.9133333' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(126 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 33, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.9166667' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(127 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 34, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.9200000' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(128 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 35, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.9233333' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9766667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(129 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 36, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.9266667' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9766667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(130 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 37, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.9300000' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9766667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(131 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 38, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.9366667' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9766667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(132 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 39, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.9400000' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9766667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(133 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 40, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.9433333' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9766667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(134 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 41, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.9500000' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9766667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(135 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 42, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.9533333' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9766667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(136 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 43, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.9566667' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9766667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(137 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 44, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.9600000' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9766667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(138 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 45, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.9633333' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9933333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(139 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 46, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.9700000' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9933333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(140 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 47, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.9733333' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9933333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(141 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 7, 48, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.9766667' AS DateTime2), CAST(11 AS Numeric(18, 0)), CAST(N'2022-12-17T02:16:31.9933333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(189 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 1, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.7700000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.7700000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(190 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 2, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.7866667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.7866667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(191 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 3, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8066667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8066667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(192 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 4, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8066667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8066667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(193 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 5, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8100000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(194 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 6, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8100000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(195 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 7, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8133333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8133333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(196 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 8, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8166667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8166667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(197 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 9, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8166667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8166667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(198 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 10, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8200000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8200000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(199 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 11, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8233333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8233333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(200 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 12, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8266667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8266667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(201 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 13, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8266667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8266667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(202 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 14, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8333333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8333333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(203 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 15, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8366667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8366667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(204 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 16, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8366667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8366667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(205 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 17, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8433333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8433333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(206 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 18, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8433333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8433333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(207 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 19, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8466667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8466667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(208 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 20, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8533333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8533333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(209 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 21, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8566667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8566667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(210 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 22, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8600000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8600000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(211 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 23, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8666667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8666667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(212 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 24, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8700000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8700000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(213 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 25, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8733333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8733333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(214 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 27, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8766667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8766667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(215 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 28, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8800000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8800000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(216 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 29, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8833333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8833333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(217 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 30, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8866667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8866667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(218 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 31, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8900000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8900000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(219 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 32, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8966667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8966667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(220 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 33, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8966667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.8966667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(221 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 34, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.9000000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.9000000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(222 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 35, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.9066667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.9066667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(223 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 36, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.9066667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.9066667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(224 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 37, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.9100000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.9100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(225 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 38, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.9133333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.9133333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(226 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 39, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.9933333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.9933333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(227 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 40, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.0466667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.0466667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(228 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 41, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.0900000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.0900000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(229 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 42, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.0933333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.0933333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(230 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 43, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.0966667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.0966667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(231 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 44, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1000000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1000000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(232 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 45, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1000000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1000000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(233 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 46, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1033333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1033333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(234 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 47, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1066667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1066667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(235 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 48, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1066667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1066667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(236 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 1, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1133333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1133333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(237 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 2, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1166667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1166667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(238 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 3, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1166667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1166667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(239 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 4, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1200000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1200000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(240 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 5, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1233333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1233333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(241 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 6, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1266667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1266667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(242 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 7, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1300000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1300000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(243 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 8, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1333333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1333333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(244 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 9, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1366667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1366667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(245 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 10, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1400000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1400000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(246 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 11, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1433333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1433333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(247 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 12, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1466667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1466667' AS DateTime2), 0, 0, 0, 0)
GO
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(248 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 13, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1466667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(249 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 14, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1500000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1500000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(250 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 15, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1533333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(251 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 16, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1566667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1566667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(252 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 17, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1600000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1600000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(253 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 18, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1633333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(254 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 19, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1633333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(255 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 20, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1666667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1666667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(256 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 21, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1700000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1700000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(257 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 22, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1733333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1733333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(258 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 23, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1766667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1766667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(259 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 24, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1800000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1800000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(260 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 25, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1800000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1800000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(261 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 27, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1833333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1833333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(262 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 28, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1866667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(263 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 29, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1900000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1900000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(264 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 30, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1933333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1933333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(265 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 31, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1966667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1966667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(266 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 32, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2000000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2000000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(267 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 33, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2033333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2033333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(268 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 34, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2066667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(269 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 35, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2100000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2100000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(270 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 36, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2133333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2133333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(271 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 37, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2166667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2166667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(272 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 38, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2166667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2166667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(273 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 39, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2200000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2200000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(274 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 40, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2233333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2233333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(275 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 41, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2266667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2266667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(276 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 42, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2300000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2300000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(277 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 43, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2333333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2333333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(278 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 44, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2366667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2366667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(279 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 45, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2400000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2400000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(280 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 46, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2433333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2433333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(281 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 47, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2466667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(282 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 48, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2500000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2500000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(283 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 1, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2566667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2566667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(284 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 2, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2600000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2600000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(285 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 3, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2733333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2733333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(286 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 4, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2966667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2966667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(287 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 5, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3033333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3033333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(288 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 6, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3066667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(289 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 7, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3100000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3100000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(290 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 8, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3133333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3133333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(291 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 9, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3166667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3166667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(292 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 10, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3200000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3200000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(293 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 11, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3333333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3333333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(294 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 12, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3366667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3366667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(295 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 13, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3433333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3433333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(296 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 14, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3466667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(297 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 15, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3466667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(298 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 16, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3533333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(299 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 17, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3566667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3566667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(300 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 18, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3633333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(301 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 19, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3666667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3666667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(302 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 20, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3700000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3700000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(303 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 21, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3733333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3733333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(304 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 22, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3766667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3766667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(305 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 23, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3800000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3800000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(306 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 24, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3833333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3833333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(307 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 25, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3866667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(308 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 27, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3933333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3933333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(309 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 28, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3966667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.3966667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(310 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 29, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4000000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4000000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(311 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 30, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4033333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4033333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(312 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 31, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4066667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(313 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 32, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4100000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4100000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(314 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 33, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4166667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4166667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(315 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 34, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4200000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4200000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(316 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 35, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4266667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4266667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(317 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 36, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4266667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4266667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(318 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 37, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4366667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4366667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(319 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 38, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4400000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4400000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(320 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 39, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4466667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(321 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 40, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4533333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(322 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 41, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4566667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4566667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(323 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 42, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4600000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4600000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(324 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 43, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4666667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4666667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(325 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 44, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4700000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4700000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(326 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 45, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4766667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4766667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(327 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 46, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4800000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4800000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(328 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 47, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4866667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(329 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 48, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4900000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4900000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(330 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 1, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.5000000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.5000000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(331 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 2, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.5066667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.5066667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(332 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 3, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.5100000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.5100000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(333 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 4, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.5266667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.5266667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(334 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 5, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.5733333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.5733333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(335 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 6, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.5800000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.5800000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(336 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 7, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.5900000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.5900000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(337 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 8, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.5933333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.5933333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(338 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 9, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.6000000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.6000000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(339 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 10, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.6533333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.6533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(340 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 11, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.7066667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.7066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(341 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 12, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.7166667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.7166667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(342 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 13, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.7233333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.7233333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(343 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 14, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.7300000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.7300000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(344 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 15, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.7366667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.7366667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(345 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 16, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.7433333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.7433333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(346 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 17, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.7466667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.7466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(347 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 18, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.7533333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.7533333' AS DateTime2), 0, 0, 0, 0)
GO
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(348 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 19, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.7566667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.7566667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(349 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 20, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.7633333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.7633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(350 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 21, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.8133333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.8133333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(351 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 22, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.8233333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.8233333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(352 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 23, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.8366667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.8366667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(353 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 24, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.8433333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.8433333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(354 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 25, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.8500000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.8500000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(355 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 27, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.8533333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.8533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(356 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 28, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.8600000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.8600000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(357 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 29, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.8633333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.8633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(358 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 30, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.8700000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.8700000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(359 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 31, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.8733333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.8733333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(360 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 32, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.8800000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.8800000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(361 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 33, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.8866667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.8866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(362 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 34, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.8933333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.8933333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(363 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 35, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.9000000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.9000000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(364 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 36, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.9033333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.9033333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(365 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 37, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.9100000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.9100000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(366 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 38, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.9166667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.9166667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(367 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 39, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.9233333' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.9233333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(368 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 40, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.9300000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.9300000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(369 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 41, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.9400000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.9400000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(370 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 42, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.9500000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.9500000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(371 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 43, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.9666667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.9666667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(372 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 44, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.9800000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.9800000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(373 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 45, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.9900000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.9900000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(374 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 46, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:14.0000000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:14.0000000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(375 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 47, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:14.0100000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:14.0100000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(376 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 7, 48, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:14.0666667' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:14.0666667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(377 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 1, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.3933333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.3933333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(378 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 2, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.3933333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.3933333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(379 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 3, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.3933333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.3933333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(380 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 4, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.3933333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.3933333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(381 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 5, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.3933333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.3933333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(382 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 6, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4100000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(383 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 7, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4100000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(384 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 8, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4100000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(385 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 9, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4100000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(386 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 10, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4100000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(387 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 11, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4100000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(388 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 12, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4100000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(389 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 13, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4100000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(390 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 14, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4100000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(391 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 15, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4100000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(392 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 16, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4100000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(393 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 17, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4100000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(394 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 18, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4266667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4266667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(395 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 19, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4266667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4266667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(396 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 20, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4266667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4266667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(397 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 21, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4266667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4266667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(398 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 22, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4266667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4266667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(399 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 23, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4266667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4266667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(400 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 24, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4266667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4266667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(401 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 25, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4266667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4266667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(402 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 27, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4266667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4266667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(403 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 28, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4266667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4266667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(404 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 29, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4266667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4266667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(405 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 30, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4400000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4400000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(406 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 31, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4400000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4400000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(407 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 32, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4400000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4400000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(408 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 33, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4400000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4400000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(409 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 34, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4400000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4400000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(410 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 35, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4400000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4400000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(411 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 36, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4400000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4400000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(412 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 37, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4400000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4400000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(413 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 38, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4400000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4400000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(414 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 39, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4566667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4566667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(415 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 40, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4566667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4566667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(416 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 41, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4566667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4566667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(417 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 42, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4566667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4566667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(418 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 43, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4566667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4566667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(419 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 44, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4566667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4566667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(420 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 45, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4566667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4566667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(421 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 46, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4566667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4566667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(422 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 47, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4566667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4566667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(423 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 48, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4733333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4733333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(424 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 49, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4733333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4733333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(425 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 1, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4733333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4733333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(426 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 2, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4733333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4733333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(427 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 3, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4733333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4733333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(428 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 4, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4733333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4733333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(429 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 5, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4866667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(430 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 6, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4866667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(431 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 7, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4866667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(432 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 8, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4866667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(433 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 9, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4866667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(434 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 10, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4866667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(435 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 11, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4866667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(436 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 12, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5033333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5033333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(437 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 13, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5033333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5033333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(438 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 14, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5033333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5033333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(439 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 15, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5033333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5033333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(440 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 16, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5033333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5033333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(441 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 17, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5033333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5033333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(442 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 18, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5033333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5033333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(443 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 19, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5200000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5200000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(444 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 20, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5200000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5200000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(445 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 21, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5200000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5200000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(446 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 22, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5200000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5200000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(447 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 23, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5200000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5200000' AS DateTime2), 0, 0, 0, 0)
GO
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(448 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 24, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5200000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5200000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(449 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 25, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5333333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5333333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(450 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 27, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5333333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5333333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(451 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 28, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5333333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5333333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(452 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 29, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5333333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5333333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(453 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 30, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5333333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5333333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(454 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 31, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5333333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5333333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(455 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 32, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5533333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(456 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 33, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5533333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(457 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 34, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5533333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(458 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 35, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5533333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(459 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 36, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5533333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(460 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 37, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5533333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(461 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 38, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5533333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(462 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 39, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5533333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(463 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 40, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5533333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(464 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 41, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5533333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(465 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 42, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5666667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5666667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(466 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 43, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5666667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5666667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(467 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 44, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5666667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5666667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(468 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 45, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5666667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5666667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(469 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 46, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5666667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5666667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(470 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 47, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5666667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5666667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(471 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 48, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5666667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5666667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(472 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 49, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5666667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5666667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(473 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 1, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5666667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5666667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(474 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 2, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5833333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5833333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(475 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 3, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5833333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5833333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(476 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 4, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5833333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5833333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(477 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 5, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5833333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5833333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(478 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 6, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5833333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5833333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(479 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 7, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5833333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5833333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(480 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 8, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5833333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5833333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(481 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 9, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5833333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5833333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(482 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 10, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5833333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5833333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(483 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 11, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5966667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5966667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(484 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 12, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5966667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5966667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(485 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 13, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5966667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5966667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(486 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 14, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5966667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5966667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(487 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 15, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5966667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5966667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(488 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 16, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5966667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5966667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(489 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 17, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6133333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6133333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(490 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 18, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6133333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6133333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(491 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 19, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6133333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6133333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(492 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 20, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6133333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6133333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(493 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 21, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6133333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6133333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(494 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 22, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6133333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6133333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(495 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 23, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6133333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6133333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(496 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 24, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6133333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6133333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(497 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 25, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6300000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6300000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(498 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 27, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6300000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6300000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(499 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 28, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6300000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6300000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(500 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 29, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6300000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6300000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(501 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 30, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6300000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6300000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(502 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 31, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6300000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6300000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(503 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 32, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6300000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6300000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(504 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 33, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6300000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6300000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(505 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 34, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6466667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(506 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 35, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6466667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(507 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 36, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6466667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(508 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 37, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6466667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(509 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 38, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6466667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(510 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 39, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6600000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6600000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(511 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 40, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6600000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6600000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(512 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 41, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6600000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6600000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(513 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 42, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6766667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6766667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(514 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 43, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6766667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6766667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(515 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 44, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6766667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6766667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(516 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 45, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6766667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6766667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(517 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 46, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6933333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6933333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(518 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 47, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6933333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6933333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(519 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 48, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6933333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6933333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(520 AS Numeric(18, 0)), CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 49, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6933333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.6933333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(521 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 1, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7066667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(522 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 2, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7066667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(523 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 3, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7233333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7233333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(524 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 4, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7233333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7233333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(525 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 5, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7233333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7233333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(526 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 6, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7233333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7233333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(527 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 7, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7400000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7400000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(528 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 8, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7400000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7400000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(529 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 9, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7400000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7400000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(530 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 10, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7400000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7400000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(531 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 11, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7533333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(532 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 12, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7533333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(533 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 13, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7533333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(534 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 14, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7700000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7700000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(535 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 15, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7700000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7700000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(536 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 16, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7700000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7700000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(537 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 17, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7866667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(538 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 18, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7866667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(539 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 19, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7866667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(540 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 20, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7866667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(541 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 21, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.8000000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.8000000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(542 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 22, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.8000000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.8000000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(543 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 23, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.8000000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.8000000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(544 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 24, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.8166667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.8166667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(545 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 25, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.8166667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.8166667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(546 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 27, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.8166667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.8166667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(547 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 28, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.8166667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.8166667' AS DateTime2), 0, 0, 0, 0)
GO
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(548 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 29, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.8166667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.8166667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(549 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 30, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.8166667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.8166667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(550 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 31, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.8333333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.8333333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(551 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 32, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.8333333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.8333333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(552 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 33, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.8333333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.8333333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(553 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 34, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9100000' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9100000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(554 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 35, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9266667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9266667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(555 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 36, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9266667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9266667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(556 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 37, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9266667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9266667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(557 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 38, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9266667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9266667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(558 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 39, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9266667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9266667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(559 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 40, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9433333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9433333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(560 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 41, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9433333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9433333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(561 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 42, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9433333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9433333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(562 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 43, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9433333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9433333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(563 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 44, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9433333' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9433333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(564 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 45, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9566667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9566667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(565 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 46, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9566667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9566667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(566 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 47, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9566667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9566667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(567 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 48, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9566667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9566667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(568 AS Numeric(18, 0)), CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), 7, 49, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9566667' AS DateTime2), CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.9566667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(569 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 1, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8200000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8200000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(570 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 2, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8200000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8200000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(571 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 3, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8366667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8366667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(572 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 4, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8366667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8366667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(573 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 5, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8366667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8366667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(574 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 6, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8366667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8366667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(575 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 7, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8366667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8366667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(576 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 8, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8366667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8366667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(577 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 9, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8366667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8366667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(578 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 10, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8366667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8366667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(579 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 11, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8366667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8366667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(580 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 12, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8366667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8366667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(581 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 13, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8366667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8366667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(582 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 14, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8366667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8366667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(583 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 15, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8366667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8366667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(584 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 16, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8533333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8533333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(585 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 17, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8533333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8533333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(586 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 18, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8533333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8533333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(587 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 19, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8533333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8533333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(588 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 20, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8533333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8533333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(589 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 21, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8533333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8533333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(590 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 22, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8533333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8533333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(591 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 23, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8533333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8533333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(592 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 24, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8533333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8533333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(593 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 25, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8533333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8533333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(594 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 27, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8666667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8666667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(595 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 28, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8666667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8666667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(596 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 29, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8666667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8666667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(597 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 30, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8666667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8666667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(598 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 31, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8666667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8666667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(599 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 32, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8666667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8666667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(600 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 33, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8666667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8666667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(601 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 34, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8666667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8666667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(602 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 35, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8666667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8666667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(603 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 36, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8833333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8833333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(604 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 37, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8833333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8833333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(605 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 38, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8833333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8833333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(606 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 39, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8833333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8833333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(607 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 40, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8833333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8833333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(608 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 41, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8833333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8833333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(609 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 42, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8833333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8833333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(610 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 43, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8833333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8833333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(611 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 44, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8833333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8833333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(612 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 45, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9000000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9000000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(613 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 46, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9000000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9000000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(614 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 47, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9000000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9000000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(615 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 48, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9000000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9000000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(616 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 49, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9000000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9000000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(617 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 1, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9000000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9000000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(618 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 2, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9133333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9133333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(619 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 3, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9133333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9133333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(620 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 4, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9133333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9133333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(621 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 5, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9133333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9133333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(622 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 6, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9133333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9133333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(623 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 7, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9133333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9133333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(624 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 8, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9133333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9133333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(625 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 9, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9300000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9300000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(626 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 10, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9300000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9300000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(627 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 11, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9300000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9300000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(628 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 12, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9300000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9300000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(629 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 13, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9466667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(630 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 14, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9466667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(631 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 15, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9466667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(632 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 16, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9466667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(633 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 17, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9600000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9600000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(634 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 18, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9600000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9600000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(635 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 19, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9600000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9600000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(636 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 20, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9600000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9600000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(637 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 21, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9600000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9600000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(638 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 22, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9600000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9600000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(639 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 23, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9766667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9766667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(640 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 24, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9766667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9766667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(641 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 25, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9766667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9766667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(642 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 27, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9766667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9766667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(643 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 28, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9766667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9766667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(644 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 29, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9933333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9933333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(645 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 30, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9933333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9933333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(646 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 31, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9933333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9933333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(647 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 32, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9933333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9933333' AS DateTime2), 0, 0, 0, 0)
GO
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(648 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 33, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9933333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9933333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(649 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 34, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9933333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9933333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(650 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 35, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0066667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(651 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 36, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0066667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(652 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 37, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0066667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(653 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 38, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0066667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(654 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 39, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0066667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(655 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 40, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0066667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(656 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 41, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0066667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(657 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 42, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0066667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(658 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 43, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0066667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(659 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 44, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0233333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0233333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(660 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 45, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0233333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0233333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(661 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 46, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0233333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0233333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(662 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 47, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0233333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0233333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(663 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 48, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0233333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0233333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(664 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 49, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0233333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0233333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(665 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 1, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0400000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0400000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(666 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 2, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0400000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0400000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(667 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 3, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0400000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0400000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(668 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 4, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0400000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0400000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(669 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 5, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0400000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0400000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(670 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 6, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0400000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0400000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(671 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 7, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0400000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0400000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(672 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 8, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0400000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0400000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(673 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 9, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0400000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0400000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(674 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 10, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0566667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0566667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(675 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 11, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0566667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0566667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(676 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 12, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0566667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0566667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(677 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 13, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0566667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0566667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(678 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 14, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0566667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0566667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(679 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 15, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0566667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0566667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(680 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 16, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0566667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0566667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(681 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 17, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0566667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0566667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(682 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 18, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0566667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0566667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(683 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 19, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0700000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0700000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(684 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 20, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0700000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0700000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(685 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 21, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0700000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0700000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(686 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 22, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0700000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0700000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(687 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 23, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0700000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0700000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(688 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 24, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0700000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0700000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(689 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 25, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0700000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0700000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(690 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 27, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0866667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(691 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 28, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0866667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(692 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 29, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0866667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(693 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 30, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0866667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(694 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 31, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0866667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(695 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 32, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0866667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(696 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 33, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1033333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1033333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(697 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 34, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1033333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1033333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(698 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 35, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1033333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1033333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(699 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 36, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1033333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1033333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(700 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 37, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1033333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1033333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(701 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 38, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1333333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1333333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(702 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 39, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1333333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1333333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(703 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 40, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1333333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1333333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(704 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 41, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1333333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1333333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(705 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 42, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1500000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1500000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(706 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 43, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1500000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1500000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(707 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 44, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1500000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1500000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(708 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 45, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1633333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(709 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 46, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1633333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(710 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 47, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1633333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(711 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 48, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1633333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(712 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 49, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1800000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1800000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(713 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 1, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1800000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1800000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(714 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 2, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1966667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1966667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(715 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 3, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1966667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1966667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(716 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 4, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1966667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1966667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(717 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 5, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1966667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1966667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(718 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 6, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2100000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2100000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(719 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 7, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2100000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2100000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(720 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 8, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2100000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2100000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(721 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 9, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2266667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2266667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(722 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 10, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2266667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2266667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(723 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 11, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2266667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2266667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(724 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 12, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2266667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2266667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(725 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 13, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2433333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2433333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(726 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 14, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2433333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2433333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(727 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 15, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2433333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2433333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(728 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 16, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2566667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2566667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(729 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 17, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2566667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2566667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(730 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 18, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2566667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2566667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(731 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 19, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2733333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2733333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(732 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 20, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2733333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2733333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(733 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 21, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2900000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2900000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(734 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 22, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2900000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2900000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(735 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 23, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2900000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2900000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(736 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 24, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2900000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2900000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(737 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 25, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2900000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2900000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(738 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 27, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2900000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.2900000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(739 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 28, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3066667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(740 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 29, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3066667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(741 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 30, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3066667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(742 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 31, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3200000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3200000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(743 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 32, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3200000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3200000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(744 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 33, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3200000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3200000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(745 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 34, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3200000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3200000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(746 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 35, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3200000' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3200000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(747 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 36, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3366667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3366667' AS DateTime2), 0, 0, 0, 0)
GO
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(748 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 37, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3366667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3366667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(749 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 38, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3366667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3366667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(750 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 39, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3366667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3366667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(751 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 40, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3366667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3366667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(752 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 41, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3533333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(753 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 42, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3533333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(754 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 43, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3533333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(755 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 44, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3533333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(756 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 45, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3533333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(757 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 46, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3533333' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(758 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 47, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3666667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3666667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(759 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 48, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3666667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3666667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(760 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), 7, 49, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3666667' AS DateTime2), CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.3666667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(761 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 1, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(762 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 2, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(763 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 3, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(764 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 4, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(765 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 5, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(766 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 6, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(767 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 7, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(768 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 8, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(769 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 9, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(770 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 10, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(771 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 11, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(772 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 12, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(773 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 13, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(774 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 14, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(775 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 15, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1100000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(776 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 16, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1266667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1266667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(777 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 17, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1266667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1266667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(778 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 18, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1266667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1266667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(779 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 19, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1266667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1266667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(780 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 20, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1266667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1266667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(781 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 21, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1266667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1266667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(782 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 22, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1266667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1266667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(783 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 23, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1266667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1266667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(784 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 24, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(785 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 25, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(786 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 27, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(787 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 28, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(788 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 29, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(789 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 30, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(790 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 31, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(791 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 32, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(792 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 33, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(793 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 34, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(794 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 35, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(795 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 36, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(796 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 37, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(797 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 38, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1433333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(798 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 39, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1633333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1633333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(799 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 40, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1633333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1633333' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(800 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 41, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1766667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1766667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(801 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 42, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1766667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1766667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(802 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 43, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1766667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1766667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(803 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 44, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1766667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1766667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(804 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 45, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1766667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1766667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(805 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 46, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1766667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1766667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(806 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 47, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1766667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1766667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(807 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 48, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1766667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1766667' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(808 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 49, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1900000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1900000' AS DateTime2), 1, 1, 1, 1)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(809 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 1, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1900000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1900000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(810 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 2, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1900000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1900000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(811 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 3, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1900000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1900000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(812 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 4, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1900000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1900000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(813 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 5, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1900000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1900000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(814 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 6, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1900000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1900000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(815 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 7, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1900000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1900000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(816 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 8, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1900000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1900000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(817 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 9, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1900000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1900000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(818 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 10, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1900000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1900000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(819 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 11, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2066667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(820 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 12, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2066667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(821 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 13, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2066667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(822 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 14, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2066667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(823 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 15, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2066667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(824 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 16, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2066667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(825 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 17, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2066667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(826 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 18, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2066667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(827 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 19, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2066667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(828 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 20, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2200000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2200000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(829 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 21, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2200000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2200000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(830 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 22, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2200000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2200000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(831 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 23, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2200000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2200000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(832 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 24, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2200000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2200000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(833 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 25, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2200000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2200000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(834 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 27, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2200000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2200000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(835 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 28, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2366667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2366667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(836 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 29, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2366667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2366667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(837 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 30, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2366667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2366667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(838 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 31, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2366667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2366667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(839 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 32, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2366667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2366667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(840 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 33, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2366667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2366667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(841 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 34, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2366667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2366667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(842 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 35, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2366667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2366667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(843 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 36, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2366667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2366667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(844 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 37, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2366667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2366667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(845 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 38, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2366667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2366667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(846 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 39, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2533333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(847 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 40, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2533333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2533333' AS DateTime2), 0, 0, 0, 0)
GO
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(848 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 41, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2533333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(849 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 42, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2533333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(850 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 43, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2533333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(851 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 44, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2533333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(852 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 45, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2533333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(853 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 46, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2533333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(854 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 47, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2533333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(855 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 48, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2533333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2533333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(856 AS Numeric(18, 0)), CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 49, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2666667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2666667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(857 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 1, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2666667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2666667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(858 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 2, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2666667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2666667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(859 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 3, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2666667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2666667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(860 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 4, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2666667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2666667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(861 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 5, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2666667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2666667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(862 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 6, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2666667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2666667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(863 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 7, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2666667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2666667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(864 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 8, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2833333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2833333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(865 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 9, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2833333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2833333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(866 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 10, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2833333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2833333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(867 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 11, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2833333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2833333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(868 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 12, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2833333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2833333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(869 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 13, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2833333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2833333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(870 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 14, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2833333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2833333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(871 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 15, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2833333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2833333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(872 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 16, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3000000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3000000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(873 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 17, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3000000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3000000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(874 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 18, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3000000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3000000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(875 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 19, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3000000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3000000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(876 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 20, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3000000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3000000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(877 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 21, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3000000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3000000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(878 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 22, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3133333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3133333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(879 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 23, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3300000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3300000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(880 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 24, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3300000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3300000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(881 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 25, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3300000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3300000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(882 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 27, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3466667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(883 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 28, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3466667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(884 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 29, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3466667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(885 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 30, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3466667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3466667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(886 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 31, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3600000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3600000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(887 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 32, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3600000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3600000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(888 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 33, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3600000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3600000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(889 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 34, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3600000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3600000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(890 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 35, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3766667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3766667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(891 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 36, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3766667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3766667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(892 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 37, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3766667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3766667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(893 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 38, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3766667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3766667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(894 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 39, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3933333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3933333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(895 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 40, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3933333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3933333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(896 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 41, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3933333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3933333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(897 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 42, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3933333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.3933333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(898 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 43, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4066667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(899 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 44, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4066667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(900 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 45, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4066667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4066667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(901 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 46, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4233333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4233333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(902 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 47, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4233333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4233333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(903 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 48, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4233333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4233333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(904 AS Numeric(18, 0)), CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 49, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4233333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4233333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(905 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 1, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4400000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4400000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(906 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 2, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4400000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4400000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(907 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 3, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4566667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4566667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(908 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 4, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4566667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4566667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(909 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 5, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4566667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4566667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(910 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 6, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4566667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4566667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(911 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 7, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4700000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4700000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(912 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 8, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4700000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4700000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(913 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 9, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4700000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4700000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(914 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 10, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4866667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(915 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 11, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4866667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(916 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 12, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4866667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(917 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 13, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4866667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(918 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 14, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4866667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(919 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 15, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4866667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4866667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(920 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 16, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5033333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5033333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(921 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 17, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5033333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5033333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(922 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 18, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5033333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5033333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(923 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 19, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5033333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5033333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(924 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 20, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5033333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5033333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(925 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 21, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5033333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5033333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(926 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 22, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5166667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5166667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(927 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 23, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5166667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5166667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(928 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 24, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5166667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5166667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(929 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 25, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5166667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5166667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(930 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 27, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5166667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5166667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(931 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 28, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5166667' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5166667' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(932 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 29, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5333333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5333333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(933 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 30, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5333333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5333333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(934 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 31, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5333333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5333333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(935 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 32, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5333333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5333333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(936 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 33, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5333333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5333333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(937 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 34, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5333333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5333333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(938 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 35, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5333333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5333333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(939 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 36, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5500000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5500000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(940 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 37, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5500000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5500000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(941 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 38, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5500000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5500000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(942 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 39, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5500000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5500000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(943 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 40, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5500000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5500000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(944 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 41, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5500000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5500000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(945 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 42, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5633333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(946 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 43, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5633333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(947 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 44, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5633333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5633333' AS DateTime2), 0, 0, 0, 0)
GO
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(948 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 45, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5633333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(949 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 46, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5633333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(950 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 47, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5633333' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5633333' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(951 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 48, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5800000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5800000' AS DateTime2), 0, 0, 0, 0)
INSERT [dbo].[adm_role_dt] ([ID], [RoleID], [CompanyId], [DropDownScreenID], [ScreenID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ViewRights], [CreateRights], [DeleteRights], [EditRights]) VALUES (CAST(952 AS Numeric(18, 0)), CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 7, 49, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5800000' AS DateTime2), CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.5800000' AS DateTime2), 0, 0, 0, 0)
GO
INSERT [dbo].[adm_role_mf] ([ID], [CompanyId], [RoleName], [Description], [SystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [IsUpdateText], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'Administrator', NULL, 1, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.4266667' AS DateTime2), CAST(1 AS Numeric(18, 0)), 0, CAST(N'2022-05-24T22:15:41.4266667' AS DateTime2))
INSERT [dbo].[adm_role_mf] ([ID], [CompanyId], [RoleName], [Description], [SystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [IsUpdateText], [ModifiedDate]) VALUES (CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'Doctor', NULL, 1, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.6133333' AS DateTime2), CAST(12 AS Numeric(18, 0)), 1, CAST(N'2022-12-17T02:21:15.9333333' AS DateTime2))
INSERT [dbo].[adm_role_mf] ([ID], [CompanyId], [RoleName], [Description], [SystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [IsUpdateText], [ModifiedDate]) VALUES (CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'Receptionist', NULL, 1, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:41.7800000' AS DateTime2), CAST(11 AS Numeric(18, 0)), 1, CAST(N'2022-12-17T02:16:31.9000000' AS DateTime2))
INSERT [dbo].[adm_role_mf] ([ID], [CompanyId], [RoleName], [Description], [SystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [IsUpdateText], [ModifiedDate]) VALUES (CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), N'Administrator', NULL, 1, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:12.7566667' AS DateTime2), CAST(2 AS Numeric(18, 0)), 0, CAST(N'2022-05-24T22:20:12.7566667' AS DateTime2))
INSERT [dbo].[adm_role_mf] ([ID], [CompanyId], [RoleName], [Description], [SystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [IsUpdateText], [ModifiedDate]) VALUES (CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), N'Doctor', NULL, 1, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.1100000' AS DateTime2), CAST(2 AS Numeric(18, 0)), 1, CAST(N'2022-05-24T22:20:13.1100000' AS DateTime2))
INSERT [dbo].[adm_role_mf] ([ID], [CompanyId], [RoleName], [Description], [SystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [IsUpdateText], [ModifiedDate]) VALUES (CAST(7 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), N'Receptionist', NULL, 1, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.2533333' AS DateTime2), CAST(2 AS Numeric(18, 0)), 1, CAST(N'2022-05-24T22:20:13.2533333' AS DateTime2))
INSERT [dbo].[adm_role_mf] ([ID], [CompanyId], [RoleName], [Description], [SystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [IsUpdateText], [ModifiedDate]) VALUES (CAST(8 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), N'Nurse', NULL, 1, CAST(2 AS Numeric(18, 0)), CAST(N'2022-05-24T22:20:13.4933333' AS DateTime2), CAST(2 AS Numeric(18, 0)), 1, CAST(N'2022-05-24T22:20:13.4933333' AS DateTime2))
INSERT [dbo].[adm_role_mf] ([ID], [CompanyId], [RoleName], [Description], [SystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [IsUpdateText], [ModifiedDate]) VALUES (CAST(9 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), N'Administrator', NULL, 1, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.3800000' AS DateTime2), CAST(13 AS Numeric(18, 0)), 0, CAST(N'2022-12-17T11:52:44.3800000' AS DateTime2))
INSERT [dbo].[adm_role_mf] ([ID], [CompanyId], [RoleName], [Description], [SystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [IsUpdateText], [ModifiedDate]) VALUES (CAST(10 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), N'Doctor', NULL, 1, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.4733333' AS DateTime2), CAST(13 AS Numeric(18, 0)), 1, CAST(N'2022-12-17T11:52:44.4733333' AS DateTime2))
INSERT [dbo].[adm_role_mf] ([ID], [CompanyId], [RoleName], [Description], [SystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [IsUpdateText], [ModifiedDate]) VALUES (CAST(11 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), N'Receptionist', NULL, 1, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.5666667' AS DateTime2), CAST(13 AS Numeric(18, 0)), 1, CAST(N'2022-12-17T11:52:44.5666667' AS DateTime2))
INSERT [dbo].[adm_role_mf] ([ID], [CompanyId], [RoleName], [Description], [SystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [IsUpdateText], [ModifiedDate]) VALUES (CAST(12 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), N'Nurse', NULL, 1, CAST(13 AS Numeric(18, 0)), CAST(N'2022-12-17T11:52:44.7066667' AS DateTime2), CAST(13 AS Numeric(18, 0)), 1, CAST(N'2022-12-17T11:52:44.7066667' AS DateTime2))
INSERT [dbo].[adm_role_mf] ([ID], [CompanyId], [RoleName], [Description], [SystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [IsUpdateText], [ModifiedDate]) VALUES (CAST(13 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), N'Administrator', NULL, 1, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.8200000' AS DateTime2), CAST(14 AS Numeric(18, 0)), 0, CAST(N'2022-12-17T12:49:04.8200000' AS DateTime2))
INSERT [dbo].[adm_role_mf] ([ID], [CompanyId], [RoleName], [Description], [SystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [IsUpdateText], [ModifiedDate]) VALUES (CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), N'Doctor', NULL, 1, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:04.9000000' AS DateTime2), CAST(14 AS Numeric(18, 0)), 1, CAST(N'2022-12-17T12:49:04.9000000' AS DateTime2))
INSERT [dbo].[adm_role_mf] ([ID], [CompanyId], [RoleName], [Description], [SystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [IsUpdateText], [ModifiedDate]) VALUES (CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), N'Receptionist', NULL, 1, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.0233333' AS DateTime2), CAST(14 AS Numeric(18, 0)), 1, CAST(N'2022-12-17T12:49:05.0233333' AS DateTime2))
INSERT [dbo].[adm_role_mf] ([ID], [CompanyId], [RoleName], [Description], [SystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [IsUpdateText], [ModifiedDate]) VALUES (CAST(16 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), N'Nurse', NULL, 1, CAST(14 AS Numeric(18, 0)), CAST(N'2022-12-17T12:49:05.1800000' AS DateTime2), CAST(14 AS Numeric(18, 0)), 1, CAST(N'2022-12-17T12:49:05.1800000' AS DateTime2))
INSERT [dbo].[adm_role_mf] ([ID], [CompanyId], [RoleName], [Description], [SystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [IsUpdateText], [ModifiedDate]) VALUES (CAST(17 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), N'Administrator', NULL, 1, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.0966667' AS DateTime2), CAST(16 AS Numeric(18, 0)), 0, CAST(N'2023-02-08T15:14:55.0966667' AS DateTime2))
INSERT [dbo].[adm_role_mf] ([ID], [CompanyId], [RoleName], [Description], [SystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [IsUpdateText], [ModifiedDate]) VALUES (CAST(18 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), N'Doctor', NULL, 1, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.1900000' AS DateTime2), CAST(16 AS Numeric(18, 0)), 1, CAST(N'2023-02-08T15:14:55.1900000' AS DateTime2))
INSERT [dbo].[adm_role_mf] ([ID], [CompanyId], [RoleName], [Description], [SystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [IsUpdateText], [ModifiedDate]) VALUES (CAST(19 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), N'Receptionist', NULL, 1, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.2666667' AS DateTime2), CAST(16 AS Numeric(18, 0)), 1, CAST(N'2023-02-08T15:14:55.2666667' AS DateTime2))
INSERT [dbo].[adm_role_mf] ([ID], [CompanyId], [RoleName], [Description], [SystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [IsUpdateText], [ModifiedDate]) VALUES (CAST(20 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), N'Nurse', NULL, 1, CAST(16 AS Numeric(18, 0)), CAST(N'2023-02-08T15:14:55.4400000' AS DateTime2), CAST(16 AS Numeric(18, 0)), 1, CAST(N'2023-02-08T15:14:55.4400000' AS DateTime2))
GO
INSERT [dbo].[adm_user_company] ([ID], [UserID], [CompanyId], [EmployeeID], [RoleID], [AdminID], [IsDefault]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(0 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 0)
INSERT [dbo].[adm_user_company] ([ID], [UserID], [CompanyId], [EmployeeID], [RoleID], [AdminID], [IsDefault]) VALUES (CAST(2 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(0 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), 0)
INSERT [dbo].[adm_user_company] ([ID], [UserID], [CompanyId], [EmployeeID], [RoleID], [AdminID], [IsDefault]) VALUES (CAST(3 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(0 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 0)
INSERT [dbo].[adm_user_company] ([ID], [UserID], [CompanyId], [EmployeeID], [RoleID], [AdminID], [IsDefault]) VALUES (CAST(4 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(0 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 0)
INSERT [dbo].[adm_user_company] ([ID], [UserID], [CompanyId], [EmployeeID], [RoleID], [AdminID], [IsDefault]) VALUES (CAST(5 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(0 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 0)
INSERT [dbo].[adm_user_company] ([ID], [UserID], [CompanyId], [EmployeeID], [RoleID], [AdminID], [IsDefault]) VALUES (CAST(6 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(0 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 0)
INSERT [dbo].[adm_user_company] ([ID], [UserID], [CompanyId], [EmployeeID], [RoleID], [AdminID], [IsDefault]) VALUES (CAST(7 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(0 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 0)
INSERT [dbo].[adm_user_company] ([ID], [UserID], [CompanyId], [EmployeeID], [RoleID], [AdminID], [IsDefault]) VALUES (CAST(8 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(0 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 0)
INSERT [dbo].[adm_user_company] ([ID], [UserID], [CompanyId], [EmployeeID], [RoleID], [AdminID], [IsDefault]) VALUES (CAST(9 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(0 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 0)
INSERT [dbo].[adm_user_company] ([ID], [UserID], [CompanyId], [EmployeeID], [RoleID], [AdminID], [IsDefault]) VALUES (CAST(10 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(0 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 0)
INSERT [dbo].[adm_user_company] ([ID], [UserID], [CompanyId], [EmployeeID], [RoleID], [AdminID], [IsDefault]) VALUES (CAST(13 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(0 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), 0)
INSERT [dbo].[adm_user_company] ([ID], [UserID], [CompanyId], [EmployeeID], [RoleID], [AdminID], [IsDefault]) VALUES (CAST(14 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), CAST(0 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), 0)
INSERT [dbo].[adm_user_company] ([ID], [UserID], [CompanyId], [EmployeeID], [RoleID], [AdminID], [IsDefault]) VALUES (CAST(15 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), CAST(0 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), 0)
INSERT [dbo].[adm_user_company] ([ID], [UserID], [CompanyId], [EmployeeID], [RoleID], [AdminID], [IsDefault]) VALUES (CAST(16 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(0 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), 0)
INSERT [dbo].[adm_user_company] ([ID], [UserID], [CompanyId], [EmployeeID], [RoleID], [AdminID], [IsDefault]) VALUES (CAST(17 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(0 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 0)
GO
INSERT [dbo].[adm_user_mf] ([ID], [Email], [Pwd], [Name], [PhoneNo], [AccountType], [CultureID], [AccountStatus], [IsGenderDropdown], [AppointmentStatusId], [EmployeeID], [LoginFailureNo], [UserLock], [IsActivated], [UserImage], [RepotFooter], [ActivationToken], [Qualification], [Designation], [ActivationTokenDate], [ActivatedDate], [LastSignIn], [ForgotToken], [ForgotTokenDate], [PhoneNotification], [EmailNotification], [SlotTime], [StartTime], [EndTime], [IsOverLap], [ExpiryDate], [SpecialtyId], [SpecialtyDropdownId], [Type], [OffDay], [IsDeleted], [IsShowDoctor], [MultilingualId]) VALUES (CAST(1 AS Numeric(18, 0)), N'saleem@gmail.com', N'123456', N'saleem', N'987645679', N'A', 0, NULL, N'1', 0, CAST(1 AS Numeric(18, 0)), 0, 0, 1, N'', NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2024-09-06T15:34:00.6260198' AS DateTime2), NULL, NULL, 0, 0, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, 0, N'7,17,10,9,8', NULL)
INSERT [dbo].[adm_user_mf] ([ID], [Email], [Pwd], [Name], [PhoneNo], [AccountType], [CultureID], [AccountStatus], [IsGenderDropdown], [AppointmentStatusId], [EmployeeID], [LoginFailureNo], [UserLock], [IsActivated], [UserImage], [RepotFooter], [ActivationToken], [Qualification], [Designation], [ActivationTokenDate], [ActivatedDate], [LastSignIn], [ForgotToken], [ForgotTokenDate], [PhoneNotification], [EmailNotification], [SlotTime], [StartTime], [EndTime], [IsOverLap], [ExpiryDate], [SpecialtyId], [SpecialtyDropdownId], [Type], [OffDay], [IsDeleted], [IsShowDoctor], [MultilingualId]) VALUES (CAST(2 AS Numeric(18, 0)), N'mohsin@gmail.com', N'123456', N'Mohsin Shahbaz', N'923333', N'A', 0, NULL, NULL, 0, CAST(1 AS Numeric(18, 0)), 0, 0, 1, NULL, NULL, N'cbd4fe9e-0ead-4d80-8ec1-7e2e2d792e76', NULL, NULL, CAST(N'2022-05-24T22:20:12.6433333' AS DateTime2), NULL, CAST(N'2023-04-17T11:47:53.1700000' AS DateTime2), NULL, NULL, 0, 0, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, 0, NULL, CAST(1 AS Numeric(18, 0)))
INSERT [dbo].[adm_user_mf] ([ID], [Email], [Pwd], [Name], [PhoneNo], [AccountType], [CultureID], [AccountStatus], [IsGenderDropdown], [AppointmentStatusId], [EmployeeID], [LoginFailureNo], [UserLock], [IsActivated], [UserImage], [RepotFooter], [ActivationToken], [Qualification], [Designation], [ActivationTokenDate], [ActivatedDate], [LastSignIn], [ForgotToken], [ForgotTokenDate], [PhoneNotification], [EmailNotification], [SlotTime], [StartTime], [EndTime], [IsOverLap], [ExpiryDate], [SpecialtyId], [SpecialtyDropdownId], [Type], [OffDay], [IsDeleted], [IsShowDoctor], [MultilingualId]) VALUES (CAST(3 AS Numeric(18, 0)), N'ali@gmail.com', N'123456', N'Dr.Ali', N'93002029383', N'A', 0, NULL, N'2', NULL, CAST(1 AS Numeric(18, 0)), 0, 0, 1, N'', NULL, NULL, N'MBBS', N'Medical', NULL, NULL, CAST(N'2022-07-02T16:08:39.1800000' AS DateTime2), N'55aa5cea-a14c-4eac-b069-bbf9697dda3b', CAST(N'2022-05-24T22:56:52.0566667' AS DateTime2), 0, 0, CAST(N'00:20:00' AS Time), CAST(N'09:00:00' AS Time), CAST(N'13:00:00' AS Time), 0, NULL, 14, 24, NULL, N'', 0, NULL, NULL)
INSERT [dbo].[adm_user_mf] ([ID], [Email], [Pwd], [Name], [PhoneNo], [AccountType], [CultureID], [AccountStatus], [IsGenderDropdown], [AppointmentStatusId], [EmployeeID], [LoginFailureNo], [UserLock], [IsActivated], [UserImage], [RepotFooter], [ActivationToken], [Qualification], [Designation], [ActivationTokenDate], [ActivatedDate], [LastSignIn], [ForgotToken], [ForgotTokenDate], [PhoneNotification], [EmailNotification], [SlotTime], [StartTime], [EndTime], [IsOverLap], [ExpiryDate], [SpecialtyId], [SpecialtyDropdownId], [Type], [OffDay], [IsDeleted], [IsShowDoctor], [MultilingualId]) VALUES (CAST(4 AS Numeric(18, 0)), N'dr@gmail.com', N'123456', N'Dr.Mohsin', N'920000', N'A', 0, NULL, N'2', NULL, CAST(1 AS Numeric(18, 0)), 0, 0, 0, N'', NULL, NULL, N'MBBS', N'Doctor', NULL, NULL, NULL, N'3393efc7-9fbc-4a8f-ba13-18b02ba1b169', CAST(N'2022-06-03T23:39:57.1066667' AS DateTime2), 0, 0, CAST(N'00:20:00' AS Time), CAST(N'09:00:00' AS Time), CAST(N'14:00:00' AS Time), 0, NULL, 17, NULL, NULL, N'', 0, NULL, NULL)
INSERT [dbo].[adm_user_mf] ([ID], [Email], [Pwd], [Name], [PhoneNo], [AccountType], [CultureID], [AccountStatus], [IsGenderDropdown], [AppointmentStatusId], [EmployeeID], [LoginFailureNo], [UserLock], [IsActivated], [UserImage], [RepotFooter], [ActivationToken], [Qualification], [Designation], [ActivationTokenDate], [ActivatedDate], [LastSignIn], [ForgotToken], [ForgotTokenDate], [PhoneNotification], [EmailNotification], [SlotTime], [StartTime], [EndTime], [IsOverLap], [ExpiryDate], [SpecialtyId], [SpecialtyDropdownId], [Type], [OffDay], [IsDeleted], [IsShowDoctor], [MultilingualId]) VALUES (CAST(5 AS Numeric(18, 0)), N'Farooq@gmail.com', N'123456', N'Dr.Farooq', N'920000', N'A', 0, NULL, NULL, NULL, CAST(1 AS Numeric(18, 0)), 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'570c0001-043e-4e11-8d3c-f37ce5767e92', CAST(N'2022-06-03T23:41:51.5433333' AS DateTime2), 0, 0, CAST(N'00:15:00' AS Time), CAST(N'08:00:00' AS Time), CAST(N'15:00:00' AS Time), 0, NULL, NULL, NULL, NULL, N'', 0, NULL, NULL)
INSERT [dbo].[adm_user_mf] ([ID], [Email], [Pwd], [Name], [PhoneNo], [AccountType], [CultureID], [AccountStatus], [IsGenderDropdown], [AppointmentStatusId], [EmployeeID], [LoginFailureNo], [UserLock], [IsActivated], [UserImage], [RepotFooter], [ActivationToken], [Qualification], [Designation], [ActivationTokenDate], [ActivatedDate], [LastSignIn], [ForgotToken], [ForgotTokenDate], [PhoneNotification], [EmailNotification], [SlotTime], [StartTime], [EndTime], [IsOverLap], [ExpiryDate], [SpecialtyId], [SpecialtyDropdownId], [Type], [OffDay], [IsDeleted], [IsShowDoctor], [MultilingualId]) VALUES (CAST(6 AS Numeric(18, 0)), N'mohhassanafzal@gmail.com', N'123456', N'Muhammad Hassan', N'03004038008', N'A', 0, NULL, NULL, NULL, CAST(1 AS Numeric(18, 0)), 0, 0, 1, N'', NULL, NULL, N'Master', N'Doctor', NULL, NULL, CAST(N'2022-11-20T07:56:55.2666667' AS DateTime2), N'0502de9a-fd0f-401e-a05a-d4c508262d14', CAST(N'2022-10-08T09:39:45.6433333' AS DateTime2), 0, 0, CAST(N'00:15:00' AS Time), CAST(N'01:00:00' AS Time), CAST(N'20:00:00' AS Time), 0, NULL, NULL, NULL, N'R', N'', 0, NULL, NULL)
INSERT [dbo].[adm_user_mf] ([ID], [Email], [Pwd], [Name], [PhoneNo], [AccountType], [CultureID], [AccountStatus], [IsGenderDropdown], [AppointmentStatusId], [EmployeeID], [LoginFailureNo], [UserLock], [IsActivated], [UserImage], [RepotFooter], [ActivationToken], [Qualification], [Designation], [ActivationTokenDate], [ActivatedDate], [LastSignIn], [ForgotToken], [ForgotTokenDate], [PhoneNotification], [EmailNotification], [SlotTime], [StartTime], [EndTime], [IsOverLap], [ExpiryDate], [SpecialtyId], [SpecialtyDropdownId], [Type], [OffDay], [IsDeleted], [IsShowDoctor], [MultilingualId]) VALUES (CAST(7 AS Numeric(18, 0)), N'saleemsab@gmail.com', N'123456', N'Saleem Sab', N'(444) 444-44446', N'A', 0, NULL, NULL, NULL, CAST(1 AS Numeric(18, 0)), 0, 0, 1, N'', NULL, NULL, N'MCS', N'Doctor Asp.net', NULL, NULL, CAST(N'2023-09-17T17:49:25.7333333' AS DateTime2), N'a042d3f9-d68e-4e58-86d1-4f6f556fbf44', CAST(N'2022-11-15T14:23:41.5466667' AS DateTime2), 0, 0, CAST(N'00:10:00' AS Time), CAST(N'08:23:00' AS Time), CAST(N'07:00:00' AS Time), 0, NULL, 34, NULL, N'R', N'', 0, NULL, NULL)
INSERT [dbo].[adm_user_mf] ([ID], [Email], [Pwd], [Name], [PhoneNo], [AccountType], [CultureID], [AccountStatus], [IsGenderDropdown], [AppointmentStatusId], [EmployeeID], [LoginFailureNo], [UserLock], [IsActivated], [UserImage], [RepotFooter], [ActivationToken], [Qualification], [Designation], [ActivationTokenDate], [ActivatedDate], [LastSignIn], [ForgotToken], [ForgotTokenDate], [PhoneNotification], [EmailNotification], [SlotTime], [StartTime], [EndTime], [IsOverLap], [ExpiryDate], [SpecialtyId], [SpecialtyDropdownId], [Type], [OffDay], [IsDeleted], [IsShowDoctor], [MultilingualId]) VALUES (CAST(8 AS Numeric(18, 0)), N'saeed@gmail.com', N'123456', N'saeed', N'(323) 232-32323', N'A', 0, NULL, N'1,2,3', NULL, CAST(1 AS Numeric(18, 0)), 0, 0, 1, N'', NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2022-11-21T17:26:06.3400000' AS DateTime2), N'd2f7dcda-fae6-4d05-b5a1-2c8a954c85a6', CAST(N'2022-11-16T06:41:43.0633333' AS DateTime2), 0, 0, CAST(N'00:20:00' AS Time), CAST(N'09:00:00' AS Time), CAST(N'15:00:00' AS Time), 0, NULL, 14, 24, NULL, N'0', 0, NULL, NULL)
INSERT [dbo].[adm_user_mf] ([ID], [Email], [Pwd], [Name], [PhoneNo], [AccountType], [CultureID], [AccountStatus], [IsGenderDropdown], [AppointmentStatusId], [EmployeeID], [LoginFailureNo], [UserLock], [IsActivated], [UserImage], [RepotFooter], [ActivationToken], [Qualification], [Designation], [ActivationTokenDate], [ActivatedDate], [LastSignIn], [ForgotToken], [ForgotTokenDate], [PhoneNotification], [EmailNotification], [SlotTime], [StartTime], [EndTime], [IsOverLap], [ExpiryDate], [SpecialtyId], [SpecialtyDropdownId], [Type], [OffDay], [IsDeleted], [IsShowDoctor], [MultilingualId]) VALUES (CAST(9 AS Numeric(18, 0)), N'Mohsin1@gmail.com', N'123456', N'Dr.Mohsin', N'(123) 456-7890', N'A', 0, NULL, NULL, NULL, CAST(1 AS Numeric(18, 0)), 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'94edf8da-bf6c-42bf-9d40-3a3949ebb2fb', CAST(N'2022-11-19T12:56:02.4000000' AS DateTime2), 0, 0, NULL, CAST(N'00:00:00' AS Time), CAST(N'20:00:00' AS Time), 0, NULL, 7, NULL, NULL, N'', 0, NULL, NULL)
INSERT [dbo].[adm_user_mf] ([ID], [Email], [Pwd], [Name], [PhoneNo], [AccountType], [CultureID], [AccountStatus], [IsGenderDropdown], [AppointmentStatusId], [EmployeeID], [LoginFailureNo], [UserLock], [IsActivated], [UserImage], [RepotFooter], [ActivationToken], [Qualification], [Designation], [ActivationTokenDate], [ActivatedDate], [LastSignIn], [ForgotToken], [ForgotTokenDate], [PhoneNotification], [EmailNotification], [SlotTime], [StartTime], [EndTime], [IsOverLap], [ExpiryDate], [SpecialtyId], [SpecialtyDropdownId], [Type], [OffDay], [IsDeleted], [IsShowDoctor], [MultilingualId]) VALUES (CAST(10 AS Numeric(18, 0)), N'mohsinshah@gmail.com', N'123456', N'Mohsin Shahbaz', N'(300) 847-46233', N'A', 0, NULL, N'1', NULL, CAST(1 AS Numeric(18, 0)), 0, 0, 1, N'', NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2023-02-06T17:04:52.5266667' AS DateTime2), N'405e22ae-43fd-4807-b674-af47fbf7c14d', CAST(N'2022-12-17T02:07:57.9400000' AS DateTime2), 0, 0, CAST(N'00:20:00' AS Time), CAST(N'09:09:00' AS Time), CAST(N'22:00:00' AS Time), 0, NULL, NULL, 24, NULL, N'', 0, NULL, NULL)
INSERT [dbo].[adm_user_mf] ([ID], [Email], [Pwd], [Name], [PhoneNo], [AccountType], [CultureID], [AccountStatus], [IsGenderDropdown], [AppointmentStatusId], [EmployeeID], [LoginFailureNo], [UserLock], [IsActivated], [UserImage], [RepotFooter], [ActivationToken], [Qualification], [Designation], [ActivationTokenDate], [ActivatedDate], [LastSignIn], [ForgotToken], [ForgotTokenDate], [PhoneNotification], [EmailNotification], [SlotTime], [StartTime], [EndTime], [IsOverLap], [ExpiryDate], [SpecialtyId], [SpecialtyDropdownId], [Type], [OffDay], [IsDeleted], [IsShowDoctor], [MultilingualId]) VALUES (CAST(13 AS Numeric(18, 0)), N'salee@gmail.com', N'123456', N'test', N'656565656', N'A', 0, NULL, NULL, NULL, CAST(1 AS Numeric(18, 0)), 0, 0, 0, NULL, NULL, N'afba586c-5206-49e1-bd6f-d9eb97fc311e', NULL, NULL, CAST(N'2022-12-17T11:52:44.2533333' AS DateTime2), NULL, CAST(N'2022-12-17T11:52:44.2533333' AS DateTime2), NULL, NULL, 0, 0, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, 0, NULL, CAST(1 AS Numeric(18, 0)))
INSERT [dbo].[adm_user_mf] ([ID], [Email], [Pwd], [Name], [PhoneNo], [AccountType], [CultureID], [AccountStatus], [IsGenderDropdown], [AppointmentStatusId], [EmployeeID], [LoginFailureNo], [UserLock], [IsActivated], [UserImage], [RepotFooter], [ActivationToken], [Qualification], [Designation], [ActivationTokenDate], [ActivatedDate], [LastSignIn], [ForgotToken], [ForgotTokenDate], [PhoneNotification], [EmailNotification], [SlotTime], [StartTime], [EndTime], [IsOverLap], [ExpiryDate], [SpecialtyId], [SpecialtyDropdownId], [Type], [OffDay], [IsDeleted], [IsShowDoctor], [MultilingualId]) VALUES (CAST(14 AS Numeric(18, 0)), N'mohsinshahbaz@gmail.com', N'123456', N'Mohsin Shahbaz', N'(300) 792-0818', N'A', 0, NULL, N'1', 0, CAST(1 AS Numeric(18, 0)), 0, 0, 1, N'', NULL, N'aadd3e46-8e53-4ed1-80ca-b7b5f54739e7', NULL, NULL, CAST(N'2022-12-17T12:49:04.8066667' AS DateTime2), NULL, CAST(N'2024-06-23T08:32:49.9500000' AS DateTime2), NULL, NULL, 0, 0, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[adm_user_mf] ([ID], [Email], [Pwd], [Name], [PhoneNo], [AccountType], [CultureID], [AccountStatus], [IsGenderDropdown], [AppointmentStatusId], [EmployeeID], [LoginFailureNo], [UserLock], [IsActivated], [UserImage], [RepotFooter], [ActivationToken], [Qualification], [Designation], [ActivationTokenDate], [ActivatedDate], [LastSignIn], [ForgotToken], [ForgotTokenDate], [PhoneNotification], [EmailNotification], [SlotTime], [StartTime], [EndTime], [IsOverLap], [ExpiryDate], [SpecialtyId], [SpecialtyDropdownId], [Type], [OffDay], [IsDeleted], [IsShowDoctor], [MultilingualId]) VALUES (CAST(15 AS Numeric(18, 0)), N'driqrarashid@gmail.com', N'123456', N'Dr. Iqra Rashid', N'(300) 182-0818', N'A', 0, NULL, N'2', 25, CAST(1 AS Numeric(18, 0)), 0, 0, 1, N'', NULL, NULL, N'MBBS, DGO,', N'Consultant Gynaecologist', NULL, NULL, CAST(N'2023-01-02T15:03:46.1733333' AS DateTime2), N'488a57ec-0cbb-407f-852b-e14a5cbc0280', CAST(N'2023-01-01T12:11:52.2533333' AS DateTime2), 0, 0, CAST(N'00:15:00' AS Time), CAST(N'15:00:00' AS Time), CAST(N'21:00:00' AS Time), 0, NULL, 14, 24, N'R', N'0', 0, NULL, NULL)
INSERT [dbo].[adm_user_mf] ([ID], [Email], [Pwd], [Name], [PhoneNo], [AccountType], [CultureID], [AccountStatus], [IsGenderDropdown], [AppointmentStatusId], [EmployeeID], [LoginFailureNo], [UserLock], [IsActivated], [UserImage], [RepotFooter], [ActivationToken], [Qualification], [Designation], [ActivationTokenDate], [ActivatedDate], [LastSignIn], [ForgotToken], [ForgotTokenDate], [PhoneNotification], [EmailNotification], [SlotTime], [StartTime], [EndTime], [IsOverLap], [ExpiryDate], [SpecialtyId], [SpecialtyDropdownId], [Type], [OffDay], [IsDeleted], [IsShowDoctor], [MultilingualId]) VALUES (CAST(16 AS Numeric(18, 0)), N'saleem86481@gmail.com', N'123456', N'saleem', N'2121212', N'A', 0, NULL, NULL, NULL, CAST(1 AS Numeric(18, 0)), 0, 0, 1, NULL, NULL, N'879ee1cd-a053-44c3-8349-21ef119ef8fa', NULL, NULL, CAST(N'2023-02-08T15:14:54.9066667' AS DateTime2), NULL, CAST(N'2024-05-13T15:32:19.1500000' AS DateTime2), NULL, NULL, 0, 0, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, 0, NULL, CAST(1 AS Numeric(18, 0)))
INSERT [dbo].[adm_user_mf] ([ID], [Email], [Pwd], [Name], [PhoneNo], [AccountType], [CultureID], [AccountStatus], [IsGenderDropdown], [AppointmentStatusId], [EmployeeID], [LoginFailureNo], [UserLock], [IsActivated], [UserImage], [RepotFooter], [ActivationToken], [Qualification], [Designation], [ActivationTokenDate], [ActivatedDate], [LastSignIn], [ForgotToken], [ForgotTokenDate], [PhoneNotification], [EmailNotification], [SlotTime], [StartTime], [EndTime], [IsOverLap], [ExpiryDate], [SpecialtyId], [SpecialtyDropdownId], [Type], [OffDay], [IsDeleted], [IsShowDoctor], [MultilingualId]) VALUES (CAST(17 AS Numeric(18, 0)), N'saleem343@gmail.com', N'123456', N'test', N'030040987632', N'A', 0, NULL, NULL, NULL, CAST(0 AS Numeric(18, 0)), 0, 0, 0, NULL, NULL, NULL, N'as', N'QW', NULL, NULL, NULL, N'24c81fab-45e9-4863-add2-80add07636c4', CAST(N'2024-07-10T15:32:37.0508214' AS DateTime2), 0, 0, CAST(N'00:15:00' AS Time), CAST(N'00:00:00' AS Time), CAST(N'21:00:00' AS Time), 0, NULL, NULL, NULL, N'R', N'', 0, NULL, NULL)
GO
INSERT [dbo].[adm_user_token] ([ID], [UserID], [TokenKey], [ExpiryDate], [IsExpired], [DeviceType], [DeviceID]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjEiLCJuYmYiOjE3MjU2MTg4NDAsImV4cCI6MTcyNTg3ODA0MCwiaWF0IjoxNzI1NjE4ODQwfQ.Iz3ODH4pdTOr0s8tHDYz0dRyHDgIf7oRYpLz4kui0bY', CAST(N'2024-09-09T10:34:00.0000000' AS DateTime2), 0, N'web', N'-1')
INSERT [dbo].[adm_user_token] ([ID], [UserID], [TokenKey], [ExpiryDate], [IsExpired], [DeviceType], [DeviceID]) VALUES (CAST(2 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjIiLCJuYmYiOjE2ODE3MzIwNzMsImV4cCI6MTY4MTk5MTI3MywiaWF0IjoxNjgxNzMyMDczfQ.foFe9-lckCFn-qihQURsspowpblN8PJq4yPxJgKt6QM', CAST(N'2023-04-20T11:47:53.0000000' AS DateTime2), 0, N'web', N'-1')
INSERT [dbo].[adm_user_token] ([ID], [UserID], [TokenKey], [ExpiryDate], [IsExpired], [DeviceType], [DeviceID]) VALUES (CAST(3 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjMiLCJuYmYiOjE2NTY3NjAxMTksImV4cCI6MTY1NzAxOTMxOSwiaWF0IjoxNjU2NzYwMTE5fQ.rw-PrJMK4qPY9pznKJIene4YSIWyDsMLEswoWXCL4hk', CAST(N'2022-07-05T11:08:39.0000000' AS DateTime2), 0, N'web', N'-1')
INSERT [dbo].[adm_user_token] ([ID], [UserID], [TokenKey], [ExpiryDate], [IsExpired], [DeviceType], [DeviceID]) VALUES (CAST(4 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), N'3393efc7-9fbc-4a8f-ba13-18b02ba1b169', CAST(N'2022-06-06T23:39:59.4866667' AS DateTime2), 0, N'web', N'-1')
INSERT [dbo].[adm_user_token] ([ID], [UserID], [TokenKey], [ExpiryDate], [IsExpired], [DeviceType], [DeviceID]) VALUES (CAST(5 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), N'570c0001-043e-4e11-8d3c-f37ce5767e92', CAST(N'2022-06-06T23:41:51.5500000' AS DateTime2), 0, N'web', N'-1')
INSERT [dbo].[adm_user_token] ([ID], [UserID], [TokenKey], [ExpiryDate], [IsExpired], [DeviceType], [DeviceID]) VALUES (CAST(6 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjYiLCJuYmYiOjE2Njg5MzEwMTUsImV4cCI6MTY2OTE5MDIxNSwiaWF0IjoxNjY4OTMxMDE1fQ.f2JwY5T0JLYl_RXXcEwq_Q0gKktm8T2VH9SztMK2b_I', CAST(N'2022-11-23T07:56:55.0000000' AS DateTime2), 0, N'web', N'-1')
INSERT [dbo].[adm_user_token] ([ID], [UserID], [TokenKey], [ExpiryDate], [IsExpired], [DeviceType], [DeviceID]) VALUES (CAST(7 AS Numeric(18, 0)), CAST(7 AS Numeric(18, 0)), N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjciLCJuYmYiOjE2OTQ5NzI5NjUsImV4cCI6MTY5NTIzMjE2NSwiaWF0IjoxNjk0OTcyOTY1fQ.M_3mFoTHJUwQQ-yOk22z_6AC4GcGOcNNpH5yzMFaDwY', CAST(N'2023-09-20T17:49:25.0000000' AS DateTime2), 0, N'web', N'-1')
INSERT [dbo].[adm_user_token] ([ID], [UserID], [TokenKey], [ExpiryDate], [IsExpired], [DeviceType], [DeviceID]) VALUES (CAST(8 AS Numeric(18, 0)), CAST(8 AS Numeric(18, 0)), N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjgiLCJuYmYiOjE2NjkwNTE1NjYsImV4cCI6MTY2OTMxMDc2NiwiaWF0IjoxNjY5MDUxNTY2fQ.bmVhp-KZa8-BDwLdGZGyzyOsTxnSfX7wYhcJZZn1CM4', CAST(N'2022-11-24T17:26:06.0000000' AS DateTime2), 0, N'web', N'-1')
INSERT [dbo].[adm_user_token] ([ID], [UserID], [TokenKey], [ExpiryDate], [IsExpired], [DeviceType], [DeviceID]) VALUES (CAST(9 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), N'94edf8da-bf6c-42bf-9d40-3a3949ebb2fb', CAST(N'2022-11-22T12:56:02.4766667' AS DateTime2), 0, N'web', N'-1')
INSERT [dbo].[adm_user_token] ([ID], [UserID], [TokenKey], [ExpiryDate], [IsExpired], [DeviceType], [DeviceID]) VALUES (CAST(10 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjEwIiwibmJmIjoxNjc1NzAzMDkyLCJleHAiOjE2NzU5NjIyOTIsImlhdCI6MTY3NTcwMzA5Mn0.817GC_JSL7sqUh8L1pva8IIXrKuiEqGVdqHFSzf_i58', CAST(N'2023-02-09T17:04:52.0000000' AS DateTime2), 0, N'web', N'-1')
INSERT [dbo].[adm_user_token] ([ID], [UserID], [TokenKey], [ExpiryDate], [IsExpired], [DeviceType], [DeviceID]) VALUES (CAST(13 AS Numeric(18, 0)), CAST(13 AS Numeric(18, 0)), N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjEzIiwibmJmIjoxNjcxMjc3OTY0LCJleHAiOjE2NzE1MzcxNjQsImlhdCI6MTY3MTI3Nzk2NH0.zL3oBKueFosI1faGaLgNO_jSMMT7X3PNNHdIG7sIcRY', CAST(N'2022-12-20T11:52:44.0000000' AS DateTime2), 0, N'web', N'-1')
INSERT [dbo].[adm_user_token] ([ID], [UserID], [TokenKey], [ExpiryDate], [IsExpired], [DeviceType], [DeviceID]) VALUES (CAST(14 AS Numeric(18, 0)), CAST(14 AS Numeric(18, 0)), N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjE0IiwibmJmIjoxNzE5MTMxNTcwLCJleHAiOjE3MTkzOTA3NzAsImlhdCI6MTcxOTEzMTU3MH0.3ms1cBQl2KGPhTZnMtfHXJ3HvWWUUTE24lZaJERYtFU', CAST(N'2024-06-26T08:32:50.0000000' AS DateTime2), 0, N'web', N'-1')
INSERT [dbo].[adm_user_token] ([ID], [UserID], [TokenKey], [ExpiryDate], [IsExpired], [DeviceType], [DeviceID]) VALUES (CAST(15 AS Numeric(18, 0)), CAST(15 AS Numeric(18, 0)), N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjE1IiwibmJmIjoxNjcyNjcxODI2LCJleHAiOjE2NzI5MzEwMjYsImlhdCI6MTY3MjY3MTgyNn0.lqnuZ3oiGcipsvmSa2SHS6XREyuk6jlq4yR0-K8VkcM', CAST(N'2023-01-05T15:03:46.0000000' AS DateTime2), 0, N'web', N'-1')
INSERT [dbo].[adm_user_token] ([ID], [UserID], [TokenKey], [ExpiryDate], [IsExpired], [DeviceType], [DeviceID]) VALUES (CAST(16 AS Numeric(18, 0)), CAST(16 AS Numeric(18, 0)), N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjE2IiwibmJmIjoxNzE1NjE0MzM5LCJleHAiOjE3MTU4NzM1MzksImlhdCI6MTcxNTYxNDMzOX0.iFsQBGvMoIlpeSpnhTsDnPvo7i-CsYM9_SjZ-NieL_0', CAST(N'2024-05-16T15:32:19.0000000' AS DateTime2), 0, N'web', N'-1')
INSERT [dbo].[adm_user_token] ([ID], [UserID], [TokenKey], [ExpiryDate], [IsExpired], [DeviceType], [DeviceID]) VALUES (CAST(17 AS Numeric(18, 0)), CAST(17 AS Numeric(18, 0)), N'24c81fab-45e9-4863-add2-80add07636c4', CAST(N'2024-07-13T15:32:37.0857002' AS DateTime2), 0, N'web', N'-1')
GO
INSERT [dbo].[emr_appointment_mf] ([ID], [CompanyId], [PatientId], [PatientProblem], [DoctorId], [AppointmentDate], [AppointmentTime], [TokenNo], [Notes], [IsAdmission], [AdmissionId], [StatusId], [IsAdmit], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(4 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), N'', CAST(6 AS Numeric(18, 0)), CAST(N'2024-07-10' AS Date), CAST(N'18:15:00' AS Time), 3, NULL, 0, NULL, 5, 0, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T16:32:04.1886993' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T17:08:23.9884377' AS DateTime2))
INSERT [dbo].[emr_appointment_mf] ([ID], [CompanyId], [PatientId], [PatientProblem], [DoctorId], [AppointmentDate], [AppointmentTime], [TokenNo], [Notes], [IsAdmission], [AdmissionId], [StatusId], [IsAdmit], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(5 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), N'', CAST(6 AS Numeric(18, 0)), CAST(N'2024-07-10' AS Date), CAST(N'19:30:00' AS Time), 4, NULL, 0, NULL, 25, 0, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T17:39:22.1074651' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T17:39:45.0930673' AS DateTime2))
INSERT [dbo].[emr_appointment_mf] ([ID], [CompanyId], [PatientId], [PatientProblem], [DoctorId], [AppointmentDate], [AppointmentTime], [TokenNo], [Notes], [IsAdmission], [AdmissionId], [StatusId], [IsAdmit], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(6 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), NULL, CAST(4 AS Numeric(18, 0)), CAST(N'2024-07-11' AS Date), CAST(N'09:00:00' AS Time), 1, NULL, 1, CAST(1 AS Numeric(18, 0)), 25, 1, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T17:54:22.8992462' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T17:54:22.8992462' AS DateTime2))
INSERT [dbo].[emr_appointment_mf] ([ID], [CompanyId], [PatientId], [PatientProblem], [DoctorId], [AppointmentDate], [AppointmentTime], [TokenNo], [Notes], [IsAdmission], [AdmissionId], [StatusId], [IsAdmit], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(7 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), NULL, CAST(3 AS Numeric(18, 0)), CAST(N'2024-07-11' AS Date), CAST(N'02:00:00' AS Time), 0, NULL, 0, NULL, 25, 0, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:42:13.0000000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:42:13.0000000' AS DateTime2))
INSERT [dbo].[emr_appointment_mf] ([ID], [CompanyId], [PatientId], [PatientProblem], [DoctorId], [AppointmentDate], [AppointmentTime], [TokenNo], [Notes], [IsAdmission], [AdmissionId], [StatusId], [IsAdmit], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(8 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'', CAST(6 AS Numeric(18, 0)), CAST(N'2024-08-31' AS Date), CAST(N'13:15:00' AS Time), 1, NULL, 0, NULL, 25, 0, CAST(1 AS Numeric(18, 0)), CAST(N'2024-08-31T12:24:37.3464412' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-08-31T12:24:37.3464412' AS DateTime2))
INSERT [dbo].[emr_appointment_mf] ([ID], [CompanyId], [PatientId], [PatientProblem], [DoctorId], [AppointmentDate], [AppointmentTime], [TokenNo], [Notes], [IsAdmission], [AdmissionId], [StatusId], [IsAdmit], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(9 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), N'', CAST(9 AS Numeric(18, 0)), CAST(N'2024-08-31' AS Date), CAST(N'12:45:00' AS Time), 2, NULL, 0, NULL, 25, 0, CAST(1 AS Numeric(18, 0)), CAST(N'2024-08-31T12:24:53.1826871' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-08-31T12:24:53.1826871' AS DateTime2))
INSERT [dbo].[emr_appointment_mf] ([ID], [CompanyId], [PatientId], [PatientProblem], [DoctorId], [AppointmentDate], [AppointmentTime], [TokenNo], [Notes], [IsAdmission], [AdmissionId], [StatusId], [IsAdmit], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(10 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), N'', CAST(10 AS Numeric(18, 0)), CAST(N'2024-08-31' AS Date), CAST(N'16:00:00' AS Time), 3, NULL, 0, NULL, 25, 0, CAST(1 AS Numeric(18, 0)), CAST(N'2024-08-31T12:25:21.0273666' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-08-31T12:25:21.0273666' AS DateTime2))
INSERT [dbo].[emr_appointment_mf] ([ID], [CompanyId], [PatientId], [PatientProblem], [DoctorId], [AppointmentDate], [AppointmentTime], [TokenNo], [Notes], [IsAdmission], [AdmissionId], [StatusId], [IsAdmit], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(11 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), N'', CAST(6 AS Numeric(18, 0)), CAST(N'2024-09-01' AS Date), CAST(N'17:00:00' AS Time), 1, NULL, 0, NULL, 25, 0, CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-01T16:38:33.9446052' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-01T16:38:33.9446052' AS DateTime2))
GO
INSERT [dbo].[emr_bill_type] ([ID], [CompanyId], [ServiceName], [IsItem], [Price], [IsSystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'Consultation', 0, CAST(600.00 AS Decimal(18, 2)), 1, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:42.2630000' AS DateTime2), NULL, NULL)
INSERT [dbo].[emr_bill_type] ([ID], [CompanyId], [ServiceName], [IsItem], [Price], [IsSystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'Item', 1, CAST(1000.00 AS Decimal(18, 2)), 1, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:42.2630000' AS DateTime2), NULL, NULL)
INSERT [dbo].[emr_bill_type] ([ID], [CompanyId], [ServiceName], [IsItem], [Price], [IsSystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'InPatient', 0, CAST(0.00 AS Decimal(18, 2)), 1, CAST(1 AS Numeric(18, 0)), CAST(N'2022-05-24T22:15:42.2630000' AS DateTime2), NULL, NULL)
GO
INSERT [dbo].[emr_complaint] ([ID], [CompanyId], [Complaint], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'sa', CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:50:25.5128708' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:50:25.5128708' AS DateTime2))
GO
INSERT [dbo].[emr_diagnos] ([ID], [CompanyId], [Diagnos], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'aa', CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:50:50.4184198' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:50:50.4184198' AS DateTime2))
GO
INSERT [dbo].[emr_document] ([ID], [CompanyId], [Date], [DocumentUpload], [DocumentTypeId], [DocumentTypeDropdownId], [Remarks], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10' AS Date), N'', 1, 13, N'kj', CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T18:32:14.240' AS DateTime), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T18:32:14.243' AS DateTime))
INSERT [dbo].[emr_document] ([ID], [CompanyId], [Date], [DocumentUpload], [DocumentTypeId], [DocumentTypeDropdownId], [Remarks], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11' AS Date), N'20240711090326deshboard.png', 2, 13, NULL, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:03:27.837' AS DateTime), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:03:27.837' AS DateTime))
GO
INSERT [dbo].[emr_expense] ([ID], [CompanyId], [CategoryId], [CategoryDropdownId], [Date], [Remark], [Amount], [ClinicId], [InvoiceDate], [InvoiceNumber], [Vendor], [PaymentStatusId], [PaymentStatusDropdownId], [PaymentRemrks], [Attachment], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 50, 18, CAST(N'2024-07-11T09:45:42.5180000' AS DateTime2), N'JHJHHJ', CAST(900.00 AS Decimal(18, 2)), CAST(1.00 AS Decimal(18, 2)), CAST(N'2024-07-11T09:45:42.5180000' AS DateTime2), NULL, NULL, NULL, 19, NULL, N'', CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:45:53.7504360' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:45:53.7504360' AS DateTime2))
GO
INSERT [dbo].[emr_income] ([ID], [CompanyId], [CategoryId], [CategoryDropdownId], [Date], [Remark], [DueAmount], [ReceivedAmount], [CreatedBy], [CreatedDate], [ModifiedBy], [Image], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 63, 20, CAST(N'2024-07-11T09:45:16.4710000' AS DateTime2), N'WQ', CAST(200.00 AS Decimal(18, 2)), CAST(200.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:45:33.6173191' AS DateTime2), CAST(1 AS Numeric(18, 0)), N'', CAST(N'2024-07-11T14:45:33.6183171' AS DateTime2))
INSERT [dbo].[emr_income] ([ID], [CompanyId], [CategoryId], [CategoryDropdownId], [Date], [Remark], [DueAmount], [ReceivedAmount], [CreatedBy], [CreatedDate], [ModifiedBy], [Image], [ModifiedDate]) VALUES (CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 66, 20, CAST(N'2024-08-31T07:11:03.5630000' AS DateTime2), N'test', CAST(500.00 AS Decimal(18, 2)), CAST(1500.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-08-31T12:11:27.8722735' AS DateTime2), CAST(1 AS Numeric(18, 0)), N'', CAST(N'2024-08-31T12:11:27.8732773' AS DateTime2))
INSERT [dbo].[emr_income] ([ID], [CompanyId], [CategoryId], [CategoryDropdownId], [Date], [Remark], [DueAmount], [ReceivedAmount], [CreatedBy], [CreatedDate], [ModifiedBy], [Image], [ModifiedDate]) VALUES (CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 64, 20, CAST(N'2024-08-31T07:14:07.7200000' AS DateTime2), N'test', CAST(0.00 AS Decimal(18, 2)), CAST(1000.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-08-31T12:14:21.8086248' AS DateTime2), CAST(1 AS Numeric(18, 0)), N'', CAST(N'2024-08-31T12:14:21.8086248' AS DateTime2))
INSERT [dbo].[emr_income] ([ID], [CompanyId], [CategoryId], [CategoryDropdownId], [Date], [Remark], [DueAmount], [ReceivedAmount], [CreatedBy], [CreatedDate], [ModifiedBy], [Image], [ModifiedDate]) VALUES (CAST(4 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 64, 20, CAST(N'2024-08-30T00:00:00.0000000' AS DateTime2), N'df', CAST(0.00 AS Decimal(18, 2)), CAST(300.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-08-31T12:17:35.5029399' AS DateTime2), CAST(1 AS Numeric(18, 0)), N'', CAST(N'2024-08-31T12:17:35.5029399' AS DateTime2))
INSERT [dbo].[emr_income] ([ID], [CompanyId], [CategoryId], [CategoryDropdownId], [Date], [Remark], [DueAmount], [ReceivedAmount], [CreatedBy], [CreatedDate], [ModifiedBy], [Image], [ModifiedDate]) VALUES (CAST(5 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 67, 20, CAST(N'2024-08-31T08:38:03.1500000' AS DateTime2), N'i', CAST(0.00 AS Decimal(18, 2)), CAST(900.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-08-31T13:40:07.3293287' AS DateTime2), CAST(1 AS Numeric(18, 0)), N'', CAST(N'2024-08-31T13:40:07.3293287' AS DateTime2))
GO
INSERT [dbo].[emr_instruction] ([ID], [CompanyId], [Instructions], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'asss', CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:51:39.6996587' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:51:39.7006559' AS DateTime2))
INSERT [dbo].[emr_instruction] ([ID], [CompanyId], [Instructions], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'qwqw', CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-02T14:14:22.7270629' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-02T14:14:22.7270629' AS DateTime2))
GO
INSERT [dbo].[emr_investigation] ([ID], [CompanyId], [Investigation], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'as', CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:51:03.2219996' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:51:03.2219996' AS DateTime2))
GO
INSERT [dbo].[emr_medicine] ([ID], [CompanyId], [Medicine], [Price], [UnitId], [UnitDropdownId], [TypeId], [TypeDropdownId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'Panadol', CAST(7.00 AS Decimal(18, 2)), 1, 14, 39, 15, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T18:28:12.0428060' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T18:28:12.0428060' AS DateTime2))
GO
INSERT [dbo].[emr_observation] ([ID], [CompanyId], [Observation], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'sas', CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:50:39.4745546' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:50:39.4745546' AS DateTime2))
GO
INSERT [dbo].[emr_patient_bill] ([ID], [CompanyId], [AdmissionId], [AppointmentId], [DoctorId], [PatientId], [ServiceId], [OutstandingBalance], [Remarks], [BillDate], [Price], [Discount], [PaidAmount], [CreatedBy], [RefundAmount], [RefundDate], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), NULL, CAST(1 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(0.00 AS Decimal(18, 2)), NULL, CAST(N'2024-07-10T10:44:14.2740000' AS DateTime2), CAST(600.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(600.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), NULL, NULL, CAST(N'2024-07-10T15:45:52.1495355' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T15:45:52.1505336' AS DateTime2))
INSERT [dbo].[emr_patient_bill] ([ID], [CompanyId], [AdmissionId], [AppointmentId], [DoctorId], [PatientId], [ServiceId], [OutstandingBalance], [Remarks], [BillDate], [Price], [Discount], [PaidAmount], [CreatedBy], [RefundAmount], [RefundDate], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), NULL, CAST(2 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(0.00 AS Decimal(18, 2)), N'', CAST(N'2024-07-10T00:00:00.0000000' AS DateTime2), CAST(600.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(600.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), NULL, NULL, CAST(N'2024-07-10T15:46:26.1325258' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T15:46:26.1325258' AS DateTime2))
INSERT [dbo].[emr_patient_bill] ([ID], [CompanyId], [AdmissionId], [AppointmentId], [DoctorId], [PatientId], [ServiceId], [OutstandingBalance], [Remarks], [BillDate], [Price], [Discount], [PaidAmount], [CreatedBy], [RefundAmount], [RefundDate], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), NULL, CAST(3 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(0.00 AS Decimal(18, 2)), N'', CAST(N'2024-07-10T00:00:00.0000000' AS DateTime2), CAST(600.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(600.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), NULL, NULL, CAST(N'2024-07-10T15:47:35.3212555' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T15:47:35.3212555' AS DateTime2))
INSERT [dbo].[emr_patient_bill] ([ID], [CompanyId], [AdmissionId], [AppointmentId], [DoctorId], [PatientId], [ServiceId], [OutstandingBalance], [Remarks], [BillDate], [Price], [Discount], [PaidAmount], [CreatedBy], [RefundAmount], [RefundDate], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(4 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), NULL, CAST(4 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(0.00 AS Decimal(18, 2)), N'', CAST(N'2024-07-10T00:00:00.0000000' AS DateTime2), CAST(600.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(600.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), NULL, NULL, CAST(N'2024-07-10T16:32:04.2174681' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T16:32:04.2184735' AS DateTime2))
INSERT [dbo].[emr_patient_bill] ([ID], [CompanyId], [AdmissionId], [AppointmentId], [DoctorId], [PatientId], [ServiceId], [OutstandingBalance], [Remarks], [BillDate], [Price], [Discount], [PaidAmount], [CreatedBy], [RefundAmount], [RefundDate], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(5 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), NULL, CAST(5 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(0.00 AS Decimal(18, 2)), N'', CAST(N'2024-07-10T00:00:00.0000000' AS DateTime2), CAST(600.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(600.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), NULL, NULL, CAST(N'2024-07-10T17:39:22.1624587' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T17:39:22.1624587' AS DateTime2))
INSERT [dbo].[emr_patient_bill] ([ID], [CompanyId], [AdmissionId], [AppointmentId], [DoctorId], [PatientId], [ServiceId], [OutstandingBalance], [Remarks], [BillDate], [Price], [Discount], [PaidAmount], [CreatedBy], [RefundAmount], [RefundDate], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(6 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(0.00 AS Decimal(18, 2)), NULL, CAST(N'2024-07-10T17:54:22.9842337' AS DateTime2), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), NULL, NULL, CAST(N'2024-07-10T17:54:22.9842337' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T17:54:22.9842337' AS DateTime2))
INSERT [dbo].[emr_patient_bill] ([ID], [CompanyId], [AdmissionId], [AppointmentId], [DoctorId], [PatientId], [ServiceId], [OutstandingBalance], [Remarks], [BillDate], [Price], [Discount], [PaidAmount], [CreatedBy], [RefundAmount], [RefundDate], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(7 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), NULL, NULL, CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(0.00 AS Decimal(18, 2)), NULL, CAST(N'2024-07-11T09:43:12.5860000' AS DateTime2), CAST(600.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(600.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), NULL, NULL, CAST(N'2024-07-11T14:43:35.6945552' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:43:35.6945552' AS DateTime2))
INSERT [dbo].[emr_patient_bill] ([ID], [CompanyId], [AdmissionId], [AppointmentId], [DoctorId], [PatientId], [ServiceId], [OutstandingBalance], [Remarks], [BillDate], [Price], [Discount], [PaidAmount], [CreatedBy], [RefundAmount], [RefundDate], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(8 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), NULL, CAST(8 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(0.00 AS Decimal(18, 2)), N'', CAST(N'2024-08-31T00:00:00.0000000' AS DateTime2), CAST(600.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(600.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), NULL, NULL, CAST(N'2024-08-31T12:24:37.3734451' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-08-31T12:24:37.3744712' AS DateTime2))
INSERT [dbo].[emr_patient_bill] ([ID], [CompanyId], [AdmissionId], [AppointmentId], [DoctorId], [PatientId], [ServiceId], [OutstandingBalance], [Remarks], [BillDate], [Price], [Discount], [PaidAmount], [CreatedBy], [RefundAmount], [RefundDate], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(9 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), NULL, CAST(9 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(0.00 AS Decimal(18, 2)), N'', CAST(N'2024-08-31T00:00:00.0000000' AS DateTime2), CAST(600.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(600.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), NULL, NULL, CAST(N'2024-08-31T12:24:53.1876865' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-08-31T12:24:53.1876865' AS DateTime2))
INSERT [dbo].[emr_patient_bill] ([ID], [CompanyId], [AdmissionId], [AppointmentId], [DoctorId], [PatientId], [ServiceId], [OutstandingBalance], [Remarks], [BillDate], [Price], [Discount], [PaidAmount], [CreatedBy], [RefundAmount], [RefundDate], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(10 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), NULL, CAST(10 AS Numeric(18, 0)), CAST(10 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(0.00 AS Decimal(18, 2)), N'', CAST(N'2024-08-31T00:00:00.0000000' AS DateTime2), CAST(600.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(600.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), NULL, NULL, CAST(N'2024-08-31T12:25:21.0553641' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-08-31T12:25:21.0553641' AS DateTime2))
INSERT [dbo].[emr_patient_bill] ([ID], [CompanyId], [AdmissionId], [AppointmentId], [DoctorId], [PatientId], [ServiceId], [OutstandingBalance], [Remarks], [BillDate], [Price], [Discount], [PaidAmount], [CreatedBy], [RefundAmount], [RefundDate], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(11 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), NULL, CAST(11 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(0.00 AS Decimal(18, 2)), N'', CAST(N'2024-09-01T00:00:00.0000000' AS DateTime2), CAST(600.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(600.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), NULL, NULL, CAST(N'2024-09-01T16:38:33.9796045' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-01T16:38:33.9796045' AS DateTime2))
GO
INSERT [dbo].[emr_patient_mf] ([ID], [CompanyId], [PatientName], [Gender], [DOB], [Email], [Mobile], [CNIC], [Image], [Notes], [MRNO], [BillTypeId], [BillTypeDropdownId], [ContactNo], [PrefixTittleId], [PrefixDropdownId], [Father_Husband], [BloodGroupId], [BloodGroupDropDownId], [EmergencyNo], [Address], [ReferredBy], [AnniversaryDate], [Illness_Diabetes], [Illness_Tuberculosis], [Illness_HeartPatient], [Illness_LungsRelated], [Illness_BloodPressure], [Illness_Migraine], [Illness_Other], [Allergies_Food], [Allergies_Drug], [Allergies_Other], [Habits_Smoking], [Habits_Drinking], [Habits_Tobacco], [Habits_Other], [MedicalHistory], [CurrentMedication], [HabitsHistory], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [Age]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'ali aslam', 1, NULL, NULL, N'030040987653', NULL, N'', NULL, N'0000000001', 1, 22, NULL, 1, 23, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T15:45:28.8178724' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T15:45:28.8178724' AS DateTime2), CAST(10.00 AS Decimal(18, 2)))
INSERT [dbo].[emr_patient_mf] ([ID], [CompanyId], [PatientName], [Gender], [DOB], [Email], [Mobile], [CNIC], [Image], [Notes], [MRNO], [BillTypeId], [BillTypeDropdownId], [ContactNo], [PrefixTittleId], [PrefixDropdownId], [Father_Husband], [BloodGroupId], [BloodGroupDropDownId], [EmergencyNo], [Address], [ReferredBy], [AnniversaryDate], [Illness_Diabetes], [Illness_Tuberculosis], [Illness_HeartPatient], [Illness_LungsRelated], [Illness_BloodPressure], [Illness_Migraine], [Illness_Other], [Allergies_Food], [Allergies_Drug], [Allergies_Other], [Habits_Smoking], [Habits_Drinking], [Habits_Tobacco], [Habits_Other], [MedicalHistory], [CurrentMedication], [HabitsHistory], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [Age]) VALUES (CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'saleem', 1, CAST(N'1901-07-04T00:00:00.0000000' AS DateTime2), NULL, N'03009876543', NULL, N'', NULL, N'0000000002', 1, 22, NULL, 1, 23, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T15:47:30.8240702' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T15:47:30.8240702' AS DateTime2), CAST(123.00 AS Decimal(18, 2)))
INSERT [dbo].[emr_patient_mf] ([ID], [CompanyId], [PatientName], [Gender], [DOB], [Email], [Mobile], [CNIC], [Image], [Notes], [MRNO], [BillTypeId], [BillTypeDropdownId], [ContactNo], [PrefixTittleId], [PrefixDropdownId], [Father_Husband], [BloodGroupId], [BloodGroupDropDownId], [EmergencyNo], [Address], [ReferredBy], [AnniversaryDate], [Illness_Diabetes], [Illness_Tuberculosis], [Illness_HeartPatient], [Illness_LungsRelated], [Illness_BloodPressure], [Illness_Migraine], [Illness_Other], [Allergies_Food], [Allergies_Drug], [Allergies_Other], [Habits_Smoking], [Habits_Drinking], [Habits_Tobacco], [Habits_Other], [MedicalHistory], [CurrentMedication], [HabitsHistory], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [Age]) VALUES (CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'saeed', 1, NULL, NULL, N'03009876541', NULL, N'20240710113124appointment.png', NULL, N'0000000003', 1, 22, NULL, 1, 23, N'', NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T16:31:48.9445283' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T16:31:48.9445283' AS DateTime2), CAST(12.00 AS Decimal(18, 2)))
INSERT [dbo].[emr_patient_mf] ([ID], [CompanyId], [PatientName], [Gender], [DOB], [Email], [Mobile], [CNIC], [Image], [Notes], [MRNO], [BillTypeId], [BillTypeDropdownId], [ContactNo], [PrefixTittleId], [PrefixDropdownId], [Father_Husband], [BloodGroupId], [BloodGroupDropDownId], [EmergencyNo], [Address], [ReferredBy], [AnniversaryDate], [Illness_Diabetes], [Illness_Tuberculosis], [Illness_HeartPatient], [Illness_LungsRelated], [Illness_BloodPressure], [Illness_Migraine], [Illness_Other], [Allergies_Food], [Allergies_Drug], [Allergies_Other], [Habits_Smoking], [Habits_Drinking], [Habits_Tobacco], [Habits_Other], [MedicalHistory], [CurrentMedication], [HabitsHistory], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [Age]) VALUES (CAST(4 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'asif', 1, CAST(N'1998-07-17T00:00:00.0000000' AS DateTime2), NULL, N'03009876543', NULL, N'20240710124100deshboard.png', NULL, N'0000000004', 1, 22, NULL, 1, 23, NULL, 45, NULL, NULL, NULL, NULL, CAST(N'2024-07-10T00:00:00.0000000' AS DateTime2), 1, 0, 0, 0, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T17:40:09.7647798' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T17:41:02.7404269' AS DateTime2), CAST(26.00 AS Decimal(18, 2)))
INSERT [dbo].[emr_patient_mf] ([ID], [CompanyId], [PatientName], [Gender], [DOB], [Email], [Mobile], [CNIC], [Image], [Notes], [MRNO], [BillTypeId], [BillTypeDropdownId], [ContactNo], [PrefixTittleId], [PrefixDropdownId], [Father_Husband], [BloodGroupId], [BloodGroupDropDownId], [EmergencyNo], [Address], [ReferredBy], [AnniversaryDate], [Illness_Diabetes], [Illness_Tuberculosis], [Illness_HeartPatient], [Illness_LungsRelated], [Illness_BloodPressure], [Illness_Migraine], [Illness_Other], [Allergies_Food], [Allergies_Drug], [Allergies_Other], [Habits_Smoking], [Habits_Drinking], [Habits_Tobacco], [Habits_Other], [MedicalHistory], [CurrentMedication], [HabitsHistory], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [Age]) VALUES (CAST(5 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'sadia', 1, CAST(N'2024-07-10T00:00:00.0000000' AS DateTime2), NULL, N'03009876543', N'3009876543197', N'', NULL, N'0000000005', 1, 22, NULL, 1, 23, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T17:52:31.1598196' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T17:52:31.1598196' AS DateTime2), CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[emr_patient_mf] ([ID], [CompanyId], [PatientName], [Gender], [DOB], [Email], [Mobile], [CNIC], [Image], [Notes], [MRNO], [BillTypeId], [BillTypeDropdownId], [ContactNo], [PrefixTittleId], [PrefixDropdownId], [Father_Husband], [BloodGroupId], [BloodGroupDropDownId], [EmergencyNo], [Address], [ReferredBy], [AnniversaryDate], [Illness_Diabetes], [Illness_Tuberculosis], [Illness_HeartPatient], [Illness_LungsRelated], [Illness_BloodPressure], [Illness_Migraine], [Illness_Other], [Allergies_Food], [Allergies_Drug], [Allergies_Other], [Habits_Smoking], [Habits_Drinking], [Habits_Tobacco], [Habits_Other], [MedicalHistory], [CurrentMedication], [HabitsHistory], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [Age]) VALUES (CAST(6 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'sadia', 1, NULL, NULL, N'0300987654', N'2009876543192', N'', NULL, N'0000000006', 1, 22, NULL, 1, 23, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T17:53:31.2433369' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T17:53:32.5173242' AS DateTime2), CAST(90.00 AS Decimal(18, 2)))
INSERT [dbo].[emr_patient_mf] ([ID], [CompanyId], [PatientName], [Gender], [DOB], [Email], [Mobile], [CNIC], [Image], [Notes], [MRNO], [BillTypeId], [BillTypeDropdownId], [ContactNo], [PrefixTittleId], [PrefixDropdownId], [Father_Husband], [BloodGroupId], [BloodGroupDropDownId], [EmergencyNo], [Address], [ReferredBy], [AnniversaryDate], [Illness_Diabetes], [Illness_Tuberculosis], [Illness_HeartPatient], [Illness_LungsRelated], [Illness_BloodPressure], [Illness_Migraine], [Illness_Other], [Allergies_Food], [Allergies_Drug], [Allergies_Other], [Habits_Smoking], [Habits_Drinking], [Habits_Tobacco], [Habits_Other], [MedicalHistory], [CurrentMedication], [HabitsHistory], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [Age]) VALUES (CAST(7 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'Walk In', 1, NULL, NULL, N'03009876545', NULL, N'', NULL, N'0000000007', 1, 22, NULL, 1, 23, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-03T19:18:02.7613266' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-04T20:14:47.5542771' AS DateTime2), CAST(6.00 AS Decimal(18, 2)))
INSERT [dbo].[emr_patient_mf] ([ID], [CompanyId], [PatientName], [Gender], [DOB], [Email], [Mobile], [CNIC], [Image], [Notes], [MRNO], [BillTypeId], [BillTypeDropdownId], [ContactNo], [PrefixTittleId], [PrefixDropdownId], [Father_Husband], [BloodGroupId], [BloodGroupDropDownId], [EmergencyNo], [Address], [ReferredBy], [AnniversaryDate], [Illness_Diabetes], [Illness_Tuberculosis], [Illness_HeartPatient], [Illness_LungsRelated], [Illness_BloodPressure], [Illness_Migraine], [Illness_Other], [Allergies_Food], [Allergies_Drug], [Allergies_Other], [Habits_Smoking], [Habits_Drinking], [Habits_Tobacco], [Habits_Other], [MedicalHistory], [CurrentMedication], [HabitsHistory], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [Age]) VALUES (CAST(8 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'rizwan', 1, NULL, NULL, N'999999999999', NULL, N'20240904134752hms.jpg', N'as', N'0000000008', 1, 22, NULL, 1, 23, N'ali', NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-04T18:47:56.8842766' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-04T18:47:56.8842766' AS DateTime2), CAST(10.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[emr_prescription_complaint] ([ID], [CompanyId], [PrescriptionId], [ComplaintId], [Complaint], [PatientId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), NULL, N'test', CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:42:13.0000000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:42:13.0000000' AS DateTime2))
INSERT [dbo].[emr_prescription_complaint] ([ID], [CompanyId], [PrescriptionId], [ComplaintId], [Complaint], [PatientId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(4 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), NULL, N'as', CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-02T17:10:36.0969555' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-02T17:10:36.0969555' AS DateTime2))
GO
INSERT [dbo].[emr_prescription_diagnos] ([ID], [CompanyId], [PrescriptionId], [DiagnosId], [Diagnos], [PatientId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), NULL, N'Diagnosis', CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:42:13.0000000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:42:13.0000000' AS DateTime2))
INSERT [dbo].[emr_prescription_diagnos] ([ID], [CompanyId], [PrescriptionId], [DiagnosId], [Diagnos], [PatientId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(4 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), NULL, N'as', CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-02T17:10:36.1029526' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-02T17:10:36.1029526' AS DateTime2))
GO
INSERT [dbo].[emr_prescription_investigation] ([ID], [CompanyId], [PrescriptionId], [InvestigationId], [Investigation], [PatientId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), NULL, N'Investigations', CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:42:13.0000000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:42:13.0000000' AS DateTime2))
INSERT [dbo].[emr_prescription_investigation] ([ID], [CompanyId], [PrescriptionId], [InvestigationId], [Investigation], [PatientId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(4 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), NULL, N'as', CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-02T17:10:36.1069479' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-02T17:10:36.1069479' AS DateTime2))
GO
INSERT [dbo].[emr_prescription_mf] ([ID], [CompanyId], [IsTemplate], [AppointmentDate], [PatientId], [ClinicId], [DoctorId], [FollowUpDate], [FollowUpTime], [IsCreateAppointment], [Notes], [CreatedBy], [Day], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 1, CAST(N'2024-07-11T00:00:00.0000000' AS DateTime2), CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(N'2024-07-13T00:00:00.0000000' AS DateTime2), NULL, 1, NULL, CAST(1 AS Numeric(18, 0)), 2, CAST(N'2024-07-11T14:42:13.0000000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:42:13.0000000' AS DateTime2))
INSERT [dbo].[emr_prescription_mf] ([ID], [CompanyId], [IsTemplate], [AppointmentDate], [PatientId], [ClinicId], [DoctorId], [FollowUpDate], [FollowUpTime], [IsCreateAppointment], [Notes], [CreatedBy], [Day], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 0, CAST(N'2024-07-11T00:00:00.0000000' AS DateTime2), CAST(5 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), CAST(N'2024-09-01T00:00:00.0000000' AS DateTime2), NULL, 0, NULL, CAST(1 AS Numeric(18, 0)), 1, CAST(N'2024-08-31T12:26:48.0000000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-08-31T12:26:48.0000000' AS DateTime2))
INSERT [dbo].[emr_prescription_mf] ([ID], [CompanyId], [IsTemplate], [AppointmentDate], [PatientId], [ClinicId], [DoctorId], [FollowUpDate], [FollowUpTime], [IsCreateAppointment], [Notes], [CreatedBy], [Day], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 0, CAST(N'2024-08-31T00:00:00.0000000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(9 AS Numeric(18, 0)), CAST(N'2024-08-31T00:00:00.0000000' AS DateTime2), NULL, 0, NULL, CAST(1 AS Numeric(18, 0)), 0, CAST(N'2024-08-31T12:28:04.0000000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-08-31T12:28:04.0000000' AS DateTime2))
INSERT [dbo].[emr_prescription_mf] ([ID], [CompanyId], [IsTemplate], [AppointmentDate], [PatientId], [ClinicId], [DoctorId], [FollowUpDate], [FollowUpTime], [IsCreateAppointment], [Notes], [CreatedBy], [Day], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(4 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 0, CAST(N'2024-08-31T00:00:00.0000000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(N'2024-09-01T00:00:00.0000000' AS DateTime2), CAST(N'12:00:00' AS Time), 0, NULL, CAST(1 AS Numeric(18, 0)), 0, CAST(N'2024-09-01T11:45:38.0000000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-01T11:46:25.6852879' AS DateTime2))
INSERT [dbo].[emr_prescription_mf] ([ID], [CompanyId], [IsTemplate], [AppointmentDate], [PatientId], [ClinicId], [DoctorId], [FollowUpDate], [FollowUpTime], [IsCreateAppointment], [Notes], [CreatedBy], [Day], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(5 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 0, CAST(N'2024-09-01T00:00:00.0000000' AS DateTime2), CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(N'2024-09-02T12:10:10.8970000' AS DateTime2), NULL, 0, N'asas', CAST(1 AS Numeric(18, 0)), 0, CAST(N'2024-09-01T16:38:42.0000000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-02T17:10:36.0629584' AS DateTime2))
GO
INSERT [dbo].[emr_prescription_observation] ([ID], [CompanyId], [PrescriptionId], [ObservationId], [Observation], [PatientId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), NULL, N'Observation', CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:42:13.0000000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:42:13.0000000' AS DateTime2))
INSERT [dbo].[emr_prescription_observation] ([ID], [CompanyId], [PrescriptionId], [ObservationId], [Observation], [PatientId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(4 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), NULL, N'sa', CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-02T17:10:36.1119787' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-02T17:10:36.1129555' AS DateTime2))
GO
INSERT [dbo].[emr_prescription_treatment] ([ID], [CompanyId], [PrescriptionId], [MedicineName], [MedicineId], [Duration], [PatientId], [Measure], [IsMorning], [IsEvening], [IsSOS], [IsNoon], [Instructions], [InstructionId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'Panadol', CAST(1 AS Numeric(18, 0)), 2, CAST(3 AS Numeric(18, 0)), NULL, 0, 0, 1, 0, N'qwq', NULL, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:42:13.0000000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:42:13.0000000' AS DateTime2))
INSERT [dbo].[emr_prescription_treatment] ([ID], [CompanyId], [PrescriptionId], [MedicineName], [MedicineId], [Duration], [PatientId], [Measure], [IsMorning], [IsEvening], [IsSOS], [IsNoon], [Instructions], [InstructionId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(4 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), N'Panadol', CAST(1 AS Numeric(18, 0)), 1, CAST(2 AS Numeric(18, 0)), N'as', 1, 1, 0, 1, N'qwqw', CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-02T17:10:36.1169520' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-02T17:10:36.1169520' AS DateTime2))
GO
INSERT [dbo].[emr_prescription_treatment_template] ([ID], [CompanyId], [TemplateName], [PrescriptionId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'new template', CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:42:13.0000000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:42:13.0000000' AS DateTime2))
GO
INSERT [dbo].[inv_stock] ([ID], [CompanyId], [ItemID], [Quantity], [BatchSarialNumber], [ExpiredWarrantyDate], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), CAST(1.00 AS Decimal(18, 2)), 123, CAST(N'2024-09-04T00:00:00.0000000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-04T12:52:00.7953040' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-04T12:52:00.7953040' AS DateTime2))
INSERT [dbo].[inv_stock] ([ID], [CompanyId], [ItemID], [Quantity], [BatchSarialNumber], [ExpiredWarrantyDate], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(20.00 AS Decimal(18, 2)), 12, CAST(N'2024-09-03T00:00:00.0000000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-04T12:52:50.8596460' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-04T12:52:50.8596460' AS DateTime2))
INSERT [dbo].[inv_stock] ([ID], [CompanyId], [ItemID], [Quantity], [BatchSarialNumber], [ExpiredWarrantyDate], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1.00 AS Decimal(18, 2)), 1112, CAST(N'2024-09-05T00:00:00.0000000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-05T12:33:23.7217284' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-05T12:33:23.7227325' AS DateTime2))
INSERT [dbo].[inv_stock] ([ID], [CompanyId], [ItemID], [Quantity], [BatchSarialNumber], [ExpiredWarrantyDate], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(4 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(1.00 AS Decimal(18, 2)), 56756, CAST(N'2024-09-23T00:00:00.0000000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-05T12:33:23.7797271' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-05T12:33:23.7797271' AS DateTime2))
INSERT [dbo].[inv_stock] ([ID], [CompanyId], [ItemID], [Quantity], [BatchSarialNumber], [ExpiredWarrantyDate], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(5 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(10.00 AS Decimal(18, 2)), 0, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-05T13:10:21.7323374' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-05T13:10:21.7323374' AS DateTime2))
INSERT [dbo].[inv_stock] ([ID], [CompanyId], [ItemID], [Quantity], [BatchSarialNumber], [ExpiredWarrantyDate], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(6 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(0.00 AS Decimal(18, 2)), 11323, CAST(N'2024-09-10T00:00:00.0000000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-05T13:11:22.0090795' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-05T13:11:22.0090795' AS DateTime2))
INSERT [dbo].[inv_stock] ([ID], [CompanyId], [ItemID], [Quantity], [BatchSarialNumber], [ExpiredWarrantyDate], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(7 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(1.00 AS Decimal(18, 2)), 2, CAST(N'2024-09-10T00:00:00.0000000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-05T13:11:22.0420787' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-05T13:11:22.0420787' AS DateTime2))
GO
INSERT [dbo].[ipd_admission] ([ID], [CompanyId], [AdmissionNo], [PatientId], [AdmissionTypeId], [AdmissionTypeDropdownId], [TypeId], [WardTypeId], [WardTypeDropdownId], [BedId], [BedDropdownId], [RoomId], [RoomDropdownId], [DoctorId], [AdmissionDate], [AdmissionTime], [DischargeDate], [DischargeTime], [Location], [ReasonForVisit], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'IP-07102024-1', CAST(5 AS Numeric(18, 0)), 1, 25, 1, 1, 33, 2, 35, NULL, NULL, 4, CAST(N'2024-07-11T00:00:00.0000000' AS DateTime2), CAST(N'09:00:00' AS Time), NULL, NULL, NULL, NULL, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T17:54:22.8432354' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T17:54:38.5784845' AS DateTime2))
GO
INSERT [dbo].[ipd_admission_charges] ([ID], [CompanyId], [AdmissionId], [PatientId], [AppointmentId], [AnnualPE], [General], [Medical], [ICUCharges], [ExamRoom], [PrivateWard], [RIP], [OtherAllCharges], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(90.00 AS Decimal(18, 2)), CAST(90.00 AS Decimal(18, 2)), CAST(90.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T18:24:24.2200154' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T18:24:24.2200154' AS DateTime2))
GO
INSERT [dbo].[ipd_admission_imaging] ([ID], [CompanyId], [AdmissionId], [AppointmentId], [PatientId], [ImagingTypeId], [ImagingTypeDropdownId], [Notes], [StatusId], [StatusDropdownId], [ResultId], [ResultDropdownId], [Image], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 1, 27, N'nj', 1, 31, 1, 32, N'', CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T18:31:46.3141096' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T18:31:46.3141096' AS DateTime2))
GO
INSERT [dbo].[ipd_admission_lab] ([ID], [CompanyId], [AdmissionId], [AppointmentId], [PatientId], [LabTypeId], [LabTypeDropdownId], [Notes], [CollectDate], [TestDate], [ReportDate], [OrderingPhysician], [Parameter], [ResultValues], [ABN], [Flags], [Comment], [TestPerformedAt], [TestDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [StatusId], [StatusDropdownId], [ResultId], [ResultDropdownId]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), 1, 28, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T18:32:00.6043451' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T18:32:00.6043451' AS DateTime2), 1, 31, 2, 32)
GO
INSERT [dbo].[ipd_admission_medication] ([ID], [CompanyId], [AdmissionId], [AppointmentId], [PatientId], [MedicineId], [Prescription], [PrescriptionDate], [QuantityRequested], [Refills], [IsRequestNow], [BillTo], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'yeas', CAST(N'2024-07-10T13:29:57.1360000' AS DateTime2), CAST(0.00 AS Decimal(18, 2)), NULL, 0, NULL, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T18:31:29.5626520' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T18:31:29.5626520' AS DateTime2))
GO
INSERT [dbo].[ipd_admission_notes] ([ID], [CompanyId], [AdmissionId], [AppointmentId], [PatientId], [OnBehalfOf], [Note], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), N'jkj', N'JK', CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T18:22:40.0158755' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T18:22:40.0158755' AS DateTime2))
GO
INSERT [dbo].[ipd_admission_vital] ([ID], [CompanyId], [AdmissionId], [AppointmentId], [PatientId], [DateRecorded], [Temperature], [Weight], [Height], [SBP], [DBP], [HeartRate], [RespiratoryRate], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(N'2024-07-10T00:00:00.0000000' AS DateTime2), 90, 99, 98, NULL, NULL, NULL, NULL, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T18:22:31.3799974' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T18:22:31.3799974' AS DateTime2))
GO
INSERT [dbo].[ipd_diagnosis] ([ID], [CompanyId], [AdmissionId], [Description], [Date], [IsType], [IsVisitType], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'hgtt', CAST(N'2024-07-10T13:07:21.3180000' AS DateTime2), N'P', 0, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T18:07:29.9976274' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T18:07:29.9976274' AS DateTime2))
INSERT [dbo].[ipd_diagnosis] ([ID], [CompanyId], [AdmissionId], [Description], [Date], [IsType], [IsVisitType], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'jhh', CAST(N'2024-07-10T13:07:35.1360000' AS DateTime2), N'S', 0, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T18:07:40.8600866' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T18:07:40.8600866' AS DateTime2))
INSERT [dbo].[ipd_diagnosis] ([ID], [CompanyId], [AdmissionId], [Description], [Date], [IsType], [IsVisitType], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'mm', CAST(N'2024-07-10T13:07:45.0600000' AS DateTime2), N'A', 0, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T18:07:48.7353883' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-10T18:07:48.7353883' AS DateTime2))
GO
INSERT [dbo].[ipd_procedure_charged] ([ID], [CompanyId], [ProcedureId], [AppointmentId], [PatientId], [Date], [Item], [Quantity], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(N'2024-07-11T00:00:00.0000000' AS DateTime2), N'test', CAST(1.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:05:42.1396083' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:05:42.1396083' AS DateTime2))
GO
INSERT [dbo].[ipd_procedure_medication] ([ID], [CompanyId], [ProcedureId], [AppointmentId], [PatientId], [MedicineId], [Quantity], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:05:42.1546096' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:05:42.1546096' AS DateTime2))
GO
INSERT [dbo].[ipd_procedure_mf] ([ID], [CompanyId], [AdmissionId], [AppointmentId], [PatientId], [PatientProcedure], [Date], [Time], [CPTCodeId], [CPTCodeDropdownId], [Location], [Physician], [Assistant], [Notes], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), CAST(5 AS Numeric(18, 0)), N'test', CAST(N'2024-07-12T00:00:00.0000000' AS DateTime2), CAST(N'15:06:00' AS Time), 1, 29, N'lah', N'asif', N'ali', N'hi', CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:05:42.1099085' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:05:42.1099085' AS DateTime2))
GO
INSERT [dbo].[pr_allowance] ([ID], [CompanyId], [CategoryDropDownID], [CategoryID], [AllowanceName], [AllowanceType], [AllowanceValue], [Taxable], [Default], [Formula], [SystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 52, 2, N'Overtime Allowance', N'F', 500, 1, 0, 0, 0, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:55:27.2383835' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:55:27.2383835' AS DateTime2))
INSERT [dbo].[pr_allowance] ([ID], [CompanyId], [CategoryDropDownID], [CategoryID], [AllowanceName], [AllowanceType], [AllowanceValue], [Taxable], [Default], [Formula], [SystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 52, 1, N'Unused Leave Encashment', N'F', 900, 1, 0, 0, 0, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T15:41:59.1479741' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T15:41:59.1479741' AS DateTime2))
GO
INSERT [dbo].[pr_deduction_contribution] ([ID], [CompanyId], [Category], [DeductionContributionName], [DeductionContributionType], [DeductionContributionValue], [Default], [Taxable], [StartingBalance], [SystemGenerated], [Formula], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'D', N'Deduction ', N'F', 100, 0, 0, 0, 0, 0, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T15:42:18.9339217' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T15:42:18.9349251' AS DateTime2))
GO
INSERT [dbo].[pr_department] ([ID], [CompanyId], [DepartmentName], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'IT', CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:53:36.5877312' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:53:36.5887304' AS DateTime2))
INSERT [dbo].[pr_department] ([ID], [CompanyId], [DepartmentName], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'HR', CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T15:42:30.6775487' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T15:42:30.6785518' AS DateTime2))
GO
INSERT [dbo].[pr_designation] ([ID], [CompanyId], [DesignationName], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'Software engineer', CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:54:19.2877125' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:54:19.2887134' AS DateTime2))
INSERT [dbo].[pr_designation] ([ID], [CompanyId], [DesignationName], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'Data Entry', CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T15:42:52.1325053' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T15:42:52.1325053' AS DateTime2))
GO
INSERT [dbo].[pr_employee_allowance] ([ID], [CompanyId], [EmployeeID], [EffectiveFrom], [EffectiveTo], [PayScheduleID], [AllowanceID], [Percentage], [Amount], [Taxable], [IsHouseOrTransAllow]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T00:00:00.0000000' AS DateTime2), NULL, CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 0.17, 500, 1, 1)
GO
INSERT [dbo].[pr_employee_mf] ([ID], [CompanyId], [FirstName], [LastName], [Gender], [DOB], [StreetAddress], [CityDropDownID], [CityID], [ZipCode], [CountryDropDownID], [CountryID], [Email], [HomePhone], [Mobile], [EmergencyContactPerson], [EmergencyContactNo], [HireDate], [JoiningDate], [PayTypeDropDownID], [PayTypeID], [BasicSalary], [StatusDropDownID], [StatusID], [TerminatedDate], [FinalSettlementDate], [PaymentMethodDropDownID], [PaymentMethodID], [SpecialtyTypeDropdownID], [SpecialtyTypeID], [ClassificationTypeDropdownID], [ClassificationTypeID], [BankName], [BranchName], [BranchCode], [AccountNo], [SwiftCode], [EmployeeTypeDropDownID], [EmployeeTypeID], [TypeStartDate], [TypeEndDate], [DesignationID], [DepartmentID], [NICNoExpiryDate], [NICNo], [NationalSecurityNo], [NationalityDropDownID], [NationalityID], [EmployeeCode], [PayScheduleID], [EmployeePic], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [SubContractCompanyName], [PassportNumber], [PassportExpiryDate], [SCHSNO], [SCHSNOExpiryDate], [MedicalInsuranceProvided], [InsuranceCardNo], [InsuranceCardNoExpiryDate], [InsuranceClassTypeDropdownID], [InsuranceClassTypeID], [AirTicketProvided], [NoOfAirTicket], [AirTicketClassTypeDropdownID], [AirTicketClassTypeID], [AirTicketFrequencyTypeDropdownID], [AirTicketFrequencyTypeID], [OriginCountryDropdownTypeID], [OriginCountryTypeID], [DestinationCountryDropdownTypeID], [DestinationCountryTypeID], [OriginCityDropdownTypeID], [OriginCityTypeID], [DestinationCityDropdownTypeID], [DestinationCityTypeID], [AirTicketRemarks], [MaritalStatusTypeDropdownID], [MaritalStatusTypeID], [ContractTypeDropDownID], [ContractTypeID], [TotalPolicyAmountMonthly], [ExculdeFromPayroll], [EffectiveDate], [IsActive], [TimeRule], [LastArrival], [ShiftID], [EarlyOut], [LeaveRestrictions], [MissingAttendance]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'usman', N'ali', N'M', CAST(N'2006-06-01T00:00:00.0000000' AS DateTime2), N'lahore', 4, 1, NULL, 3, 1, NULL, NULL, NULL, NULL, NULL, CAST(N'2024-07-11T00:00:00.0000000' AS DateTime2), CAST(N'2024-07-11T00:00:00.0000000' AS DateTime2), 38, 1, 300000, 40, 1, NULL, NULL, 41, 1, 47, 1, 48, 1, N'', N'', N'', N'', N'', 42, 1, NULL, NULL, CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-31T00:00:00.0000000' AS DateTime2), NULL, NULL, 43, 1, NULL, CAST(1 AS Numeric(18, 0)), N'', CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:56:15.9974472' AS DateTime2), NULL, NULL, N'', NULL, NULL, NULL, NULL, N'N', NULL, NULL, 44, 1, N'N', NULL, 45, 1, 46, 1, 3, 2, 3, 2, 4, NULL, 4, NULL, NULL, 49, 1, 50, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[pr_employee_mf] ([ID], [CompanyId], [FirstName], [LastName], [Gender], [DOB], [StreetAddress], [CityDropDownID], [CityID], [ZipCode], [CountryDropDownID], [CountryID], [Email], [HomePhone], [Mobile], [EmergencyContactPerson], [EmergencyContactNo], [HireDate], [JoiningDate], [PayTypeDropDownID], [PayTypeID], [BasicSalary], [StatusDropDownID], [StatusID], [TerminatedDate], [FinalSettlementDate], [PaymentMethodDropDownID], [PaymentMethodID], [SpecialtyTypeDropdownID], [SpecialtyTypeID], [ClassificationTypeDropdownID], [ClassificationTypeID], [BankName], [BranchName], [BranchCode], [AccountNo], [SwiftCode], [EmployeeTypeDropDownID], [EmployeeTypeID], [TypeStartDate], [TypeEndDate], [DesignationID], [DepartmentID], [NICNoExpiryDate], [NICNo], [NationalSecurityNo], [NationalityDropDownID], [NationalityID], [EmployeeCode], [PayScheduleID], [EmployeePic], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [SubContractCompanyName], [PassportNumber], [PassportExpiryDate], [SCHSNO], [SCHSNOExpiryDate], [MedicalInsuranceProvided], [InsuranceCardNo], [InsuranceCardNoExpiryDate], [InsuranceClassTypeDropdownID], [InsuranceClassTypeID], [AirTicketProvided], [NoOfAirTicket], [AirTicketClassTypeDropdownID], [AirTicketClassTypeID], [AirTicketFrequencyTypeDropdownID], [AirTicketFrequencyTypeID], [OriginCountryDropdownTypeID], [OriginCountryTypeID], [DestinationCountryDropdownTypeID], [DestinationCountryTypeID], [OriginCityDropdownTypeID], [OriginCityTypeID], [DestinationCityDropdownTypeID], [DestinationCityTypeID], [AirTicketRemarks], [MaritalStatusTypeDropdownID], [MaritalStatusTypeID], [ContractTypeDropDownID], [ContractTypeID], [TotalPolicyAmountMonthly], [ExculdeFromPayroll], [EffectiveDate], [IsActive], [TimeRule], [LastArrival], [ShiftID], [EarlyOut], [LeaveRestrictions], [MissingAttendance]) VALUES (CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'saleem', N'aslam', N'M', NULL, N'lahore', 4, 1, NULL, 3, 1, NULL, NULL, NULL, NULL, NULL, CAST(N'2024-05-01T00:00:00.0000000' AS DateTime2), CAST(N'2024-05-01T00:00:00.0000000' AS DateTime2), 38, 1, 20000, 40, 1, NULL, NULL, 41, 1, 47, NULL, 48, NULL, NULL, NULL, NULL, NULL, NULL, 42, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 43, NULL, NULL, CAST(1 AS Numeric(18, 0)), N'', CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-02T15:21:01.2474682' AS DateTime2), NULL, NULL, N'', NULL, NULL, NULL, NULL, N'N', NULL, NULL, 44, 1, N'N', NULL, 45, 1, 46, 1, 3, 2, 3, 2, 4, NULL, 4, NULL, NULL, 49, 1, 50, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[pr_employee_payroll_dt] ([ID], [PayrollID], [CompanyId], [PayScheduleID], [PayDate], [EmployeeID], [Type], [AllowDedID], [Amount], [Taxable], [AdjustmentDate], [AdjustmentType], [AdjustmentAmount], [AdjustmentComments], [AdjustmentBy], [RefID], [Remarks], [ArrearReleatedMonth]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-04' AS Date), CAST(1 AS Numeric(18, 0)), N'A', 1, CAST(500.00 AS Decimal(18, 2)), 1, NULL, NULL, NULL, NULL, NULL, CAST(0.00 AS Decimal(18, 2)), N'', NULL)
GO
INSERT [dbo].[pr_employee_payroll_mf] ([ID], [CompanyId], [PayScheduleID], [PayDate], [EmployeeID], [PayScheduleFromDate], [PayScheduleToDate], [FromDate], [ToDate], [BasicSalary], [Status], [AdjustmentDate], [AdjustmentType], [AdjustmentAmount], [AdjustmentComments], [AdjustmentBy], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-04' AS Date), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-01T00:00:00.0000000' AS DateTime2), CAST(N'2024-07-31T00:00:00.0000000' AS DateTime2), CAST(N'2024-07-01T00:00:00.0000000' AS DateTime2), CAST(N'2024-07-31T00:00:00.0000000' AS DateTime2), CAST(300000.00 AS Decimal(18, 2)), N'P', NULL, NULL, NULL, NULL, NULL, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T18:12:07.7400000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T18:14:40.1445664' AS DateTime2))
GO
INSERT [dbo].[pr_leave_application] ([ID], [FromDate], [ToDate], [Hours], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [CompanyId], [EmployeeID], [LeaveTypeID]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T00:00:00.0000000' AS DateTime2), CAST(N'2024-07-11T00:00:00.0000000' AS DateTime2), 1, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T15:57:01.1306117' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T15:57:01.1306117' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)))
GO
INSERT [dbo].[pr_leave_type] ([ID], [CompanyId], [Category], [TypeName], [AccuralDropDownID], [AccrualFrequencyID], [EarnedValue], [SystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'V', N'Vacation', 37, 2, 80, 0, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:55:57.9494513' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:55:57.9494513' AS DateTime2))
INSERT [dbo].[pr_leave_type] ([ID], [CompanyId], [Category], [TypeName], [AccuralDropDownID], [AccrualFrequencyID], [EarnedValue], [SystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'S', N' Sick Leave', 37, 2, 80, 0, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T15:55:11.3020420' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T15:55:11.3020420' AS DateTime2))
INSERT [dbo].[pr_leave_type] ([ID], [CompanyId], [Category], [TypeName], [AccuralDropDownID], [AccrualFrequencyID], [EarnedValue], [SystemGenerated], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'L', N'LWP', 37, 2, 80, 0, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T15:55:35.4334252' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T15:55:35.4334252' AS DateTime2))
GO
INSERT [dbo].[pr_loan] ([ID], [CompanyId], [EmployeeID], [PaymentMethodDropdownID], [PaymentMethodID], [LoanTypeDropdownID], [LoanTypeID], [LoanCode], [Description], [PaymentStartDate], [LoanDate], [LoanAmount], [DeductionType], [DeductionValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [AdjustmentDate], [AdjustmentType], [AdjustmentAmount], [AdjustmentComments], [AdjustmentBy], [InstallmentByBaseSalary], [ApprovalStatusID]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 51, 1, 56, 1, 91, N'test', CAST(N'2024-07-03T00:00:00.0000000' AS DateTime2), CAST(N'2024-07-01T00:00:00.0000000' AS DateTime2), 50000, N'F', 10000, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T18:17:39.3629201' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T18:17:39.3629201' AS DateTime2), NULL, NULL, NULL, NULL, NULL, 0, NULL)
GO
INSERT [dbo].[pr_loan_payment_dt] ([ID], [CompanyId], [LoanID], [PaymentDate], [Comment], [Amount], [AdjustmentDate], [AdjustmentType], [AdjustmentAmount], [AdjustmentComments], [AdjustmentBy]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T13:22:19.3060000' AS DateTime2), N'first payment', 10000, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[pr_loan_payment_dt] ([ID], [CompanyId], [LoanID], [PaymentDate], [Comment], [Amount], [AdjustmentDate], [AdjustmentType], [AdjustmentAmount], [AdjustmentComments], [AdjustmentBy]) VALUES (CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T00:00:00.0000000' AS DateTime2), N'first payment', 5000, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[pr_pay_schedule] ([ID], [CompanyId], [PayTypeDropDownID], [PayTypeID], [ScheduleName], [PeriodStartDate], [PeriodEndDate], [FallInHolidayDropDownID], [FallInHolidayID], [PayDate], [Lock], [Active], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 38, 1, N'Month', CAST(N'2024-08-01' AS Date), CAST(N'2024-08-31' AS Date), 39, 1, CAST(N'2024-08-04' AS Date), 1, 0, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:55:02.887' AS DateTime), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T18:14:40.143' AS DateTime))
GO
INSERT [dbo].[pr_time_entry] ([ID], [CompanyId], [EmployeeID], [AttendanceDate], [LocationID], [TimeIn], [TimeOut], [StatusDropDownID], [StatusID], [Remarks], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T10:57:20.0690000' AS DateTime2), NULL, CAST(N'2024-07-11T09:00:00.0000000' AS DateTime2), CAST(N'2024-07-11T18:00:00.0000000' AS DateTime2), 54, 1, NULL, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T15:57:49.8375856' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T15:57:49.8375856' AS DateTime2))
GO
INSERT [dbo].[pur_invoice_dt] ([ID], [CompanyId], [InvoiceID], [ItemID], [ItemDescription], [Quantity], [Rate], [Amount], [Discount], [DiscountAmount], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [BatchSarialNumber], [ExpiredWarrantyDate]) VALUES (CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), NULL, CAST(1.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), NULL, CAST(0.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-04T12:52:00.7403019' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-04T12:52:00.7403019' AS DateTime2), 123, CAST(N'2024-09-04T00:00:00.0000000' AS DateTime2))
INSERT [dbo].[pur_invoice_dt] ([ID], [CompanyId], [InvoiceID], [ItemID], [ItemDescription], [Quantity], [Rate], [Amount], [Discount], [DiscountAmount], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [BatchSarialNumber], [ExpiredWarrantyDate]) VALUES (CAST(4 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), NULL, CAST(1.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), NULL, CAST(0.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-04T12:52:00.7473036' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-04T12:52:00.7473036' AS DateTime2), 123, CAST(N'2024-09-04T00:00:00.0000000' AS DateTime2))
INSERT [dbo].[pur_invoice_dt] ([ID], [CompanyId], [InvoiceID], [ItemID], [ItemDescription], [Quantity], [Rate], [Amount], [Discount], [DiscountAmount], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [BatchSarialNumber], [ExpiredWarrantyDate]) VALUES (CAST(8 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), NULL, CAST(10.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), NULL, CAST(0.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-04T12:52:50.8466593' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-04T12:52:50.8466593' AS DateTime2), 12, CAST(N'2024-09-03T00:00:00.0000000' AS DateTime2))
INSERT [dbo].[pur_invoice_dt] ([ID], [CompanyId], [InvoiceID], [ItemID], [ItemDescription], [Quantity], [Rate], [Amount], [Discount], [DiscountAmount], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [BatchSarialNumber], [ExpiredWarrantyDate]) VALUES (CAST(9 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), NULL, CAST(10.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), NULL, CAST(0.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-04T12:52:50.8476563' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-04T12:52:50.8476563' AS DateTime2), 12, CAST(N'2024-09-04T00:00:00.0000000' AS DateTime2))
INSERT [dbo].[pur_invoice_dt] ([ID], [CompanyId], [InvoiceID], [ItemID], [ItemDescription], [Quantity], [Rate], [Amount], [Discount], [DiscountAmount], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [BatchSarialNumber], [ExpiredWarrantyDate]) VALUES (CAST(10 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), CAST(2 AS Numeric(18, 0)), NULL, CAST(10.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), CAST(1000.00 AS Decimal(18, 2)), NULL, CAST(0.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-04T12:52:50.8486427' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-04T12:52:50.8486427' AS DateTime2), NULL, NULL)
INSERT [dbo].[pur_invoice_dt] ([ID], [CompanyId], [InvoiceID], [ItemID], [ItemDescription], [Quantity], [Rate], [Amount], [Discount], [DiscountAmount], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [BatchSarialNumber], [ExpiredWarrantyDate]) VALUES (CAST(13 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), NULL, CAST(1.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), NULL, CAST(0.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-05T12:33:23.6407250' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-05T12:33:23.6407250' AS DateTime2), 1112, CAST(N'2024-09-05T00:00:00.0000000' AS DateTime2))
INSERT [dbo].[pur_invoice_dt] ([ID], [CompanyId], [InvoiceID], [ItemID], [ItemDescription], [Quantity], [Rate], [Amount], [Discount], [DiscountAmount], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [BatchSarialNumber], [ExpiredWarrantyDate]) VALUES (CAST(14 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), CAST(3 AS Numeric(18, 0)), NULL, CAST(1.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), NULL, CAST(0.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-05T12:33:23.6527270' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-05T12:33:23.6527270' AS DateTime2), 56756, CAST(N'2024-09-23T00:00:00.0000000' AS DateTime2))
INSERT [dbo].[pur_invoice_dt] ([ID], [CompanyId], [InvoiceID], [ItemID], [ItemDescription], [Quantity], [Rate], [Amount], [Discount], [DiscountAmount], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [BatchSarialNumber], [ExpiredWarrantyDate]) VALUES (CAST(17 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), NULL, CAST(1.00 AS Decimal(18, 2)), CAST(20.00 AS Decimal(18, 2)), CAST(20.00 AS Decimal(18, 2)), NULL, CAST(0.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-05T13:11:21.9610798' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-05T13:11:21.9610798' AS DateTime2), 11323, CAST(N'2024-09-10T00:00:00.0000000' AS DateTime2))
INSERT [dbo].[pur_invoice_dt] ([ID], [CompanyId], [InvoiceID], [ItemID], [ItemDescription], [Quantity], [Rate], [Amount], [Discount], [DiscountAmount], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [BatchSarialNumber], [ExpiredWarrantyDate]) VALUES (CAST(18 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(4 AS Numeric(18, 0)), CAST(6 AS Numeric(18, 0)), NULL, CAST(1.00 AS Decimal(18, 2)), CAST(20.00 AS Decimal(18, 2)), CAST(20.00 AS Decimal(18, 2)), NULL, CAST(0.00 AS Decimal(18, 2)), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-05T13:11:21.9620792' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-05T13:11:21.9620792' AS DateTime2), 2, CAST(N'2024-09-10T00:00:00.0000000' AS DateTime2))
GO
INSERT [dbo].[pur_invoice_mf] ([ID], [CompanyId], [VendorID], [BillNo], [OrderNo], [BillDate], [DueDate], [Total], [DiscountAmount], [Discount], [IsItemLevelDiscount], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [SaveStatus]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(12.00 AS Decimal(18, 2)), N'12', CAST(N'2024-09-03T19:00:00.0000000' AS DateTime2), CAST(N'2024-09-04T07:51:31.5910000' AS DateTime2), CAST(200.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, 0, CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-04T12:51:57.6394297' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-04T12:52:00.6882952' AS DateTime2), 2)
INSERT [dbo].[pur_invoice_mf] ([ID], [CompanyId], [VendorID], [BillNo], [OrderNo], [BillDate], [DueDate], [Total], [DiscountAmount], [Discount], [IsItemLevelDiscount], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [SaveStatus]) VALUES (CAST(2 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(121.00 AS Decimal(18, 2)), N'12', CAST(N'2024-09-03T19:00:00.0000000' AS DateTime2), CAST(N'2024-09-04T07:52:05.0620000' AS DateTime2), CAST(1200.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, 0, CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-04T12:52:37.4373427' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-04T12:52:50.8346460' AS DateTime2), 2)
INSERT [dbo].[pur_invoice_mf] ([ID], [CompanyId], [VendorID], [BillNo], [OrderNo], [BillDate], [DueDate], [Total], [DiscountAmount], [Discount], [IsItemLevelDiscount], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [SaveStatus]) VALUES (CAST(3 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1111.00 AS Decimal(18, 2)), N'23', CAST(N'2024-09-22T19:00:00.0000000' AS DateTime2), CAST(N'2024-09-05T07:32:53.1500000' AS DateTime2), CAST(20.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, 0, CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-05T12:33:19.8514709' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-05T12:33:23.5337282' AS DateTime2), 2)
INSERT [dbo].[pur_invoice_mf] ([ID], [CompanyId], [VendorID], [BillNo], [OrderNo], [BillDate], [DueDate], [Total], [DiscountAmount], [Discount], [IsItemLevelDiscount], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [SaveStatus]) VALUES (CAST(4 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(212.00 AS Decimal(18, 2)), N'21', CAST(N'2024-09-09T19:00:00.0000000' AS DateTime2), CAST(N'2024-09-05T08:10:50.3620000' AS DateTime2), CAST(40.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, 0, CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-05T13:11:16.7768650' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-05T13:11:21.8230823' AS DateTime2), 2)
GO
INSERT [dbo].[pur_payment] ([ID], [CompanyId], [InvoiveId], [PaymentMethodDropdownID], [PaymentMethodID], [Amount], [PaymentDate], [Notes], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), 41, 2, CAST(200.00 AS Decimal(18, 2)), CAST(N'2024-09-04T09:02:04.9520000' AS DateTime2), N'asa', CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-04T14:02:12.0896560' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-09-04T14:02:12.0906557' AS DateTime2))
GO
INSERT [dbo].[pur_vendor] ([ID], [CompanyId], [FirstName], [LastName], [CompanyName], [VendorPhone], [VendorEmail], [Address], [Address2], [City], [State], [ZipCode], [Phone], [Fax], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'ALI', N'Usman', N'test', N'90000000', N'sa@gmail.com', N'lhr', N'lhr', NULL, NULL, NULL, NULL, NULL, CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:46:44.7417187' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T14:46:44.7427208' AS DateTime2))
GO
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (1, N'Appointment Status')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (2, N'Gender Type')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (3, N'Country')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (4, N'City')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (5, N'Culture')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (6, N'Module')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (7, N'Screen')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (8, N'Report')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (9, N'Company Type')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (11, N'Country')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (12, N'Working Day')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (13, N'DocumentType')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (14, N'Unit')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (15, N'MedicineType')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (16, N'Dose')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (17, N'Blood Group')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (18, N'Category')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (19, N'Payment Status')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (20, N'Income Category')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (21, N'Vital')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (22, N'Bill Type')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (23, N'Prefix Tittle')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (24, N'Specialty')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (25, N'Admission Type')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (26, N'Admit Location')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (27, N'Imaging Type')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (28, N'Lab Type')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (29, N'CPT Code')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (30, N'Visit Type')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (31, N'Status Type')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (32, N'Result Type')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (33, N'Ward')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (34, N'Room')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (35, N'Bed')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (36, N'Date Format')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (37, N'Accrual Frequency')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (38, N'Pay Type')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (39, N'PayDayFallOnHoliday')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (40, N'EmployeeStatus')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (41, N'SalaryPaymentMethod')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (42, N'EmployeeType')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (43, N'Nationality')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (44, N'InsuranceClassType')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (45, N'AirTicketClassType')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (46, N'AirTicketFrequency')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (47, N'SpecialtyType')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (48, N'ClassificationType')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (49, N'MaritalType')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (50, N'ContractType')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (51, N'LoanPaymentMethod')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (52, N'AllowanceCategory')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (53, N'RelationshipType')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (54, N'TimeStatus')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (55, N'EmpDocumentType')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (56, N'LoanType')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (57, N'ItemUnitDropDownID')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (58, N'CategoryDropDownID')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (59, N'ItemTypeDropDownID')
INSERT [dbo].[sys_drop_down_mf] ([ID], [Name]) VALUES (60, N'Sale Type')
GO
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (0, 12, N'Sunday', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 1, N'All', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 2, N'Male', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 3, N'Pakistan', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 4, N'Abbotabad	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 5, N'English', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 6, N'Security', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 7, N'Payment Summary Report', 0, 6, 5, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 12, N'Monday', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 13, N'X-Ray', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 14, N'kg', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 22, N'Cash', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 23, N'Mr', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 24, N'Acupuncturist', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 25, N'By Pass', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 26, N'Ward1', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 27, N'CT scan', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 28, N'CT scan', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 29, N'8905', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 30, N'Admission', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 31, N'Pending', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 32, N'Positive ', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 33, N'Ward1', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 34, N'Room1', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 35, N'Bed1', 0, 33, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 36, N'DD/MM/YYYY', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 37, N'Beginning of Fiscal Year', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 38, N'Per Month', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 39, N'Run Payroll Earlier', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 40, N'Active', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 41, N'Bank Transfer', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 42, N'Permanent', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 43, N'Pakistani', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 44, N'VIP', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 45, N'First Class', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 46, N'6 Months', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 47, N'Type1', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 48, N'Classification Type', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 49, N'Single', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 50, N'Company', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 51, N'Salary', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 52, N'Unused Leave Encashment', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 53, N'Relationship Type', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 54, N'Missing In', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 55, N'National ID', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 56, N'Loan', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 57, N'box', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 58, N'Sachet', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 59, N'Goods', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (1, 60, N'Sell', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 1, N'Missed', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 2, N'Female', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 4, N'Ahmedpur East', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 5, N'Urdu', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 6, N'Appointment', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 7, N'Roles', 0, 6, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 12, N'Tuesday', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 13, N'Medical test', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 22, N'Insurance', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 23, N'Mrs', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 24, N'Alternate Medicine', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 25, N'Gynaec', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 26, N'Ward2', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 27, N'X-Ray', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 28, N'X-Ray', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 29, N'8907', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 30, N'Consultation', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 31, N'Completed', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 32, N'Negative ', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 33, N'Ward2', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 34, N'Room2', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 35, N'Bed2', 0, 33, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 36, N'MM/DD/YYYY', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 37, N'Beginning of Calendar Year', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 38, N'Fortnightly', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 39, N'Run Payroll Later', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 40, N'Deactivated', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 41, N'Cash', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 42, N'Contract', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 43, N'USA', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 44, N'A+', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 45, N'Business Class', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 46, N'Annual', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 47, N'Type2', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 48, N'Classification Type 2', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 49, N'Married', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 50, N'Sub-contract', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 51, N'Cash', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 52, N'Overtime Allowance', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 53, N'Relationship Type1', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 54, N'Missing Out', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 55, N'Passport', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 56, N'Advance', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 57, N'sqm', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 58, N'Tablet', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 59, N'Service', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (2, 60, N'Refund', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 2, N'Other', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 4, N'Arif Wala	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 6, N'General Setting', 0, NULL, NULL, 1, NULL, NULL)
GO
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 7, N'Users', 0, 6, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 12, N'Wednesday', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 13, N'Operative Report', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 14, N'mol', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 23, N'Miss', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 24, N'Anaesthetist', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 27, N'LUG', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 29, N'8908', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 30, N'Consultation Gyneco', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 32, N'Good', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 33, N'Ward3', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 34, N'Room3', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 35, N'Bed1', 0, 33, 2, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 41, N'Cheque', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 42, N'Director/Owner', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 44, N'A', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 45, N'Economy Class', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 49, N'Divorced or Widow', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 52, N'Mobile Allowance', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 54, N'Lateness', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 55, N'Insurance Card', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 57, N'dz', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 58, N'Capsules', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (3, 59, N'Batch', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (4, 4, N'Attock		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (4, 6, N'Master Setting', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (4, 7, N'Profile', 0, 6, 3, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (4, 12, N'Thursday', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (4, 23, N'Sir', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (4, 24, N'Audiologist', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (4, 30, N'Consultation MG', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (4, 34, N'Room4', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (4, 42, N'Probation', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (4, 44, N'B', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (4, 52, N'Transport Allowance', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (4, 54, N'Early Out', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (4, 55, N'Driving License', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (4, 57, N'box', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (4, 58, N'Drops', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (4, 59, N'Sarial', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (5, 1, N'Arrival', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (5, 4, N'Badin		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (5, 6, N'Reports', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (5, 7, N'Complaints', 0, 6, 4, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (5, 12, N'Friday', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (5, 23, N'Dr', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (5, 24, N'Ayurveda', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (5, 30, N'Consultation Rhumato', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (5, 34, N'Room5', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (5, 42, N'Resigned', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (5, 52, N'Holiday Overtime', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (5, 54, N'Absent', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (5, 55, N'Other', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (5, 57, N'ft', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (5, 58, N'Injections', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (6, 1, N'Cancelled', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (6, 4, N'Bahawalnagar', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (6, 6, N'Patient', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (6, 7, N'Observations / Notes', 0, 6, 4, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (6, 12, N'Saturday', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (6, 24, N'Cardiologist', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (6, 30, N'Imaging', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (6, 34, N'Room6', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (6, 42, N'Terminated', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (6, 54, N'On Time', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (6, 57, N'g', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (6, 58, N'Syrup', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (7, 1, N'Delay', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (7, 4, N'Bahawalpur	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (7, 6, N'Billing', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (7, 7, N'Diagnosis', 0, 6, 4, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (7, 24, N'Dentist', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (7, 30, N'Lab', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (7, 54, N'Weekend', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (7, 57, N'kg', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (8, 1, N'Waiting', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (8, 4, N'Bhakkar		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (8, 6, N'Income', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (8, 7, N'Investigations', 0, 6, 4, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (8, 24, N'Dermatologist', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (8, 30, N'Pharmacy', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (8, 54, N'Public Holiday', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (8, 57, N'km', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (9, 1, N'Checkout', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (9, 4, N'Bhalwal		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (9, 6, N'Expenses', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (9, 7, N'Medicine', 0, 6, 4, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (9, 24, N'Diabetologist', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (9, 30, N'Testing', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (9, 57, N'mg', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (10, 1, N'Engaged', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (10, 4, N'Burewala	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (10, 6, N'Admission', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (10, 7, N'Medicine Instructions', 0, 6, 4, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (10, 24, N'Dietitian', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (10, 57, N'mt', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (11, 4, N'Chakwal		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (11, 7, N'Patient', 0, 6, 6, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (11, 24, N'ENT Specialist', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (11, 57, N'pcs', 0, NULL, NULL, 1, NULL, NULL)
GO
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (12, 4, N'Chaman		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (12, 7, N'Billing', 0, 6, 7, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (12, 24, N'General Practice', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (13, 4, N'Charsadda	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (13, 7, N'Appointment', 0, 6, 2, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (13, 24, N'General Surgeon', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (14, 4, N'Chiniot		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (14, 7, N'Expenses', 0, 6, 9, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (14, 9, N'Partnership Firm', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (14, 24, N'Gynecologist', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (15, 4, N'Chishtian	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (15, 7, N'Appointment Report', 0, 6, 5, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (15, 9, N'Private Limited Company', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (15, 24, N'Homeopathy', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (16, 4, N'Dadu		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (16, 7, N'Fee Report', 0, 6, 5, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (16, 24, N'Infertility Specialist', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (17, 4, N'Daharki		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (17, 7, N'Patient Details Report', 0, 6, 5, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (17, 24, N'Naturopathy', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (18, 4, N'Daska		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (18, 7, N'Clinics', 0, 6, 3, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (18, 24, N'Nephrologist', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (19, 4, N'Dera Ghazi Khan', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (19, 7, N'Change Password', 0, 6, 3, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (19, 24, N'Neurologist', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (20, 4, N'Dera Ismail Khan', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (20, 7, N'Income', 0, 6, 8, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (20, 24, N'Nutritionist', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (21, 4, N'Faisalabad	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (21, 7, N'Prescription', 0, 6, 2, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (21, 11, N'Pakistan', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (21, 24, N'Oncologist', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (22, 4, N'Ferozwala	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (22, 7, N'Schedule', 0, 6, 2, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (22, 11, N'Saudi Arabia', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (22, 24, N'Ophthalmologist', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (23, 4, N'Ghotki		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (23, 7, N'Health', 0, 6, 2, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (23, 11, N'UAE', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (23, 24, N'Orthopedist', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (24, 4, N'Gojra		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (24, 7, N'Vital', 0, 6, 2, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (24, 11, N'USA', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (24, 24, N'Paramedic', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (25, 1, N'Scheduled', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (25, 4, N'Gujranwala	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (25, 7, N'Document', 0, 6, 2, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (25, 24, N'Pathologist', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (26, 1, N'teste', 0, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (26, 4, N'Gujranwala Cantonment', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (26, 24, N'Pediatrician', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (27, 1, N'asas', 0, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (27, 4, N'Gujrat		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (27, 7, N'Cash Flow Report', 0, 6, 5, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (27, 24, N'Physiotherapist', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (28, 4, N'Gwadar		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (28, 7, N'Outstanding Income Report', 0, 6, 5, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (28, 24, N'Podiatrist', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (29, 4, N'Hafizabad	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (29, 7, N'Treatment wise Report', 0, 6, 5, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (29, 24, N'Psychiatrist', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (30, 4, N'Haroonabad	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (30, 7, N'Outstanding Report', 0, 6, 5, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (30, 24, N'Psychologist', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (31, 4, N'Hasilpur	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (31, 7, N'Payment Report', 0, 6, 5, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (31, 24, N'Radiologist', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (32, 4, N'Hub			', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (32, 7, N'Birthday Report', 0, 6, 5, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (32, 24, N'Sexologist', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (33, 4, N'Hyderabad	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (33, 7, N'Follow-up Report', 0, 6, 5, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (33, 24, N'Speech Therapist', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (34, 4, N'Islamabad	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (34, 7, N'Detailed Patient wise Fees Report', 0, 6, 5, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (34, 24, N'Therapist', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (35, 4, N'Jacobabad	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (35, 7, N'Doctors wise Fees Report', 0, 6, 5, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (35, 24, N'Urologist', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (36, 4, N'Jaranwala	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (36, 7, N' Doctors wise Payment Report', 0, 6, 5, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (36, 24, N'Veterinarian', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (37, 4, N'Jatoi		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (37, 7, N'Doctors wise Summary Payment Report', 0, 6, 5, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (37, 14, N'mg', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (38, 4, N'Jhang		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (38, 7, N'Clinic wise Payment Report', 0, 6, 5, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (39, 4, N'Jhelum		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (39, 7, N'Visit', 0, 6, 10, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (39, 15, N'Tablet', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (40, 4, N'Kabal		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (40, 7, N'
Medication', 0, 6, 10, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (40, 15, N'Capsul', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (41, 4, N'Kamalia		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (41, 7, N'Imaging', 0, 6, 10, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (41, 15, N'Syrup', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (42, 4, N'Kamber Ali Khan', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (42, 7, N'
Labs', 0, 6, 10, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (42, 16, N'Morning', 0, NULL, NULL, 1, NULL, NULL)
GO
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (43, 4, N'Kamoke		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (43, 7, N'Vitals', 0, 6, 10, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (43, 16, N'Evening', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (44, 4, N'Kandhkot	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (44, 7, N'
Notes', 0, 6, 10, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (44, 16, N'Mor+Eve', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (45, 4, N'Karachi		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (45, 7, N'Orders', 0, 6, 10, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (45, 17, N'A+', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (46, 4, N'Kasur		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (46, 7, N'
Procedure', 0, 6, 10, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (46, 17, N'B+', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (47, 4, N'Khairpur	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (47, 7, N'Charges', 0, 6, 10, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (47, 17, N'O-', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (48, 4, N'Khanewal	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (48, 7, N'History', 0, 6, 10, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (48, 17, N'O+', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (49, 4, N'Khanpur		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (49, 7, N'Master Setting', 0, 6, 4, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (49, 17, N'AB+', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (50, 4, N'Khushab		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (50, 18, N'Staff Salary', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (51, 4, N'Khuzdar		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (51, 18, N'Clinic Rent', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (52, 4, N'Kohat		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (52, 18, N'Maintenance Charges', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (53, 4, N'Kot Abdul Malik', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (53, 18, N'House Keeping', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (54, 4, N'Kot Addu	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (54, 18, N'Transportation', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (55, 4, N'Kotri		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (55, 18, N'Lab Work', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (56, 4, N'Lahore		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (56, 18, N'Automobile', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (57, 4, N'Larkana		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (57, 18, N'Utility Bills', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (58, 4, N'Layyah		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (58, 18, N'Personal', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (59, 4, N'Lodhran		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (59, 18, N'Doctor/Consultant Fees', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (60, 4, N'Mandi Bahauddin', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (60, 18, N'Others', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (61, 4, N'Mansehra	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (61, 19, N'Paid', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (62, 4, N'Mardan		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (62, 19, N'Unpaid', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (63, 4, N'Mianwali	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (63, 20, N'Maternity Services', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (64, 4, N'Mingora		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (64, 20, N'Other Clinic Visits', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (65, 4, N'Mirpur Khas	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (65, 20, N'Pathology', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (66, 4, N'Mirpur Mathelo', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (66, 20, N'General Income', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (67, 4, N'Multan		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (67, 20, N'Pharmacy', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (67, 21, N'Blood Sugar', 0, NULL, NULL, 1, NULL, N'mg/dl')
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (68, 4, N'Muridke		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (68, 20, N'Lab', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (68, 21, N'Blood Pressure', 0, NULL, NULL, 1, NULL, N'sys/dia ')
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (69, 4, N'Muzaffargarh', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (69, 20, N'IPD', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (69, 21, N'Pulse Rate', 0, NULL, NULL, 1, NULL, N'pulse/min')
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (70, 4, N'Narowal		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (70, 20, N'External Lab', 0, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (70, 21, N'Respiratory Rate', 0, NULL, NULL, 1, NULL, N'/min')
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (71, 4, N'Nawabshah	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (71, 21, N'Body Temperature', 0, NULL, NULL, 1, NULL, N'°F')
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (72, 4, N'Nowshera	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (72, 21, N'Weight', 0, NULL, NULL, 1, NULL, N'kg')
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (73, 4, N'Okara		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (73, 21, N'Height', 0, NULL, NULL, 1, NULL, N'cm')
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (74, 4, N'Pakpattan	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (74, 21, N'SPO2', 0, NULL, NULL, 1, NULL, N'%')
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (75, 4, N'Peshawar	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (76, 4, N'Quetta		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (77, 4, N'Rahim Yar Khan', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (78, 4, N'Rawalpindi	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (79, 4, N'Sadiqabad	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (80, 4, N'Sahiwal		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (81, 4, N'Sambrial	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (82, 4, N'Samundri	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (83, 4, N'Sargodha	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (84, 4, N'Shahdadkot	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (85, 4, N'Sheikhupura	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (86, 4, N'Shikarpur	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (87, 4, N'Sialkot		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (88, 4, N'Sukkur		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (89, 4, N'Swabi		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (90, 4, N'Tando Adam	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (91, 4, N'Tando Allahyar', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (92, 4, N'Tando Muhammad Khan', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (93, 4, N'Taxila		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (94, 4, N'Turbat		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (95, 4, N'Umerkot		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (96, 4, N'Vehari		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (97, 4, N'Wah Cantonmeant', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (98, 4, N'Wazirabad	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (99, 4, N'Islamamad	', 0, 3, 1, 1, NULL, NULL)
GO
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (100, 4, N'Quota		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (101, 4, N'Fasilabad	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (102, 4, N'NanKanaSab	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (103, 4, N'Mianchannu	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (104, 4, N'Rajanpur	', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (105, 4, N'Jampur		', 0, 3, 1, 1, NULL, NULL)
INSERT [dbo].[sys_drop_down_value] ([ID], [DropDownID], [Value], [IsDeleted], [DependedDropDownID], [DependedDropDownValueID], [SystemGenerated], [CompanyId], [Unit]) VALUES (106, 4, N'Mailsi		', 0, 3, 1, 1, NULL, NULL)
GO
INSERT [dbo].[sys_holidays] ([ID], [CompanyId], [HolidayName], [FromDate], [ToDate], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (CAST(1 AS Numeric(18, 0)), CAST(1 AS Numeric(18, 0)), N'14 Agust', CAST(N'2024-08-14T00:00:00.0000000' AS DateTime2), CAST(N'2024-08-14T00:00:00.0000000' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T15:59:19.8373794' AS DateTime2), CAST(1 AS Numeric(18, 0)), CAST(N'2024-07-11T15:59:19.8373794' AS DateTime2))
GO
/****** Object:  Index [AK_adm_item_ID_CompanyId]    Script Date: 9/6/2024 5:45:26 PM ******/
ALTER TABLE [dbo].[adm_item] ADD  CONSTRAINT [AK_adm_item_ID_CompanyId] UNIQUE NONCLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [AK_adm_role_mf_ID_CompanyId]    Script Date: 9/6/2024 5:45:26 PM ******/
ALTER TABLE [dbo].[adm_role_mf] ADD  CONSTRAINT [AK_adm_role_mf_ID_CompanyId] UNIQUE NONCLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [AK_emr_appointment_mf_ID_CompanyId]    Script Date: 9/6/2024 5:45:26 PM ******/
ALTER TABLE [dbo].[emr_appointment_mf] ADD  CONSTRAINT [AK_emr_appointment_mf_ID_CompanyId] UNIQUE NONCLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [AK_emr_bill_type_ID_CompanyId]    Script Date: 9/6/2024 5:45:26 PM ******/
ALTER TABLE [dbo].[emr_bill_type] ADD  CONSTRAINT [AK_emr_bill_type_ID_CompanyId] UNIQUE NONCLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [AK_emr_medicine_ID_CompanyId]    Script Date: 9/6/2024 5:45:26 PM ******/
ALTER TABLE [dbo].[emr_medicine] ADD  CONSTRAINT [AK_emr_medicine_ID_CompanyId] UNIQUE NONCLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [AK_emr_patient_bill_ID_CompanyId]    Script Date: 9/6/2024 5:45:26 PM ******/
ALTER TABLE [dbo].[emr_patient_bill] ADD  CONSTRAINT [AK_emr_patient_bill_ID_CompanyId] UNIQUE NONCLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [AK_emr_patient_mf_ID_CompanyId]    Script Date: 9/6/2024 5:45:26 PM ******/
ALTER TABLE [dbo].[emr_patient_mf] ADD  CONSTRAINT [AK_emr_patient_mf_ID_CompanyId] UNIQUE NONCLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [AK_emr_prescription_mf_ID_CompanyId]    Script Date: 9/6/2024 5:45:26 PM ******/
ALTER TABLE [dbo].[emr_prescription_mf] ADD  CONSTRAINT [AK_emr_prescription_mf_ID_CompanyId] UNIQUE NONCLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [AK_ipd_admission_ID_CompanyId]    Script Date: 9/6/2024 5:45:26 PM ******/
ALTER TABLE [dbo].[ipd_admission] ADD  CONSTRAINT [AK_ipd_admission_ID_CompanyId] UNIQUE NONCLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [AK_ipd_procedure_mf_ID_CompanyId]    Script Date: 9/6/2024 5:45:26 PM ******/
ALTER TABLE [dbo].[ipd_procedure_mf] ADD  CONSTRAINT [AK_ipd_procedure_mf_ID_CompanyId] UNIQUE NONCLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [AK_pr_allowance_ID_CompanyId]    Script Date: 9/6/2024 5:45:26 PM ******/
ALTER TABLE [dbo].[pr_allowance] ADD  CONSTRAINT [AK_pr_allowance_ID_CompanyId] UNIQUE NONCLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [AK_pr_deduction_contribution_ID_CompanyId]    Script Date: 9/6/2024 5:45:26 PM ******/
ALTER TABLE [dbo].[pr_deduction_contribution] ADD  CONSTRAINT [AK_pr_deduction_contribution_ID_CompanyId] UNIQUE NONCLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [AK_pr_department_ID_CompanyId]    Script Date: 9/6/2024 5:45:26 PM ******/
ALTER TABLE [dbo].[pr_department] ADD  CONSTRAINT [AK_pr_department_ID_CompanyId] UNIQUE NONCLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [AK_pr_designation_ID_CompanyId]    Script Date: 9/6/2024 5:45:27 PM ******/
ALTER TABLE [dbo].[pr_designation] ADD  CONSTRAINT [AK_pr_designation_ID_CompanyId] UNIQUE NONCLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [AK_pr_employee_mf_ID_CompanyId]    Script Date: 9/6/2024 5:45:27 PM ******/
ALTER TABLE [dbo].[pr_employee_mf] ADD  CONSTRAINT [AK_pr_employee_mf_ID_CompanyId] UNIQUE NONCLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [AK_pr_employee_payroll_mf_ID_CompanyId_PayScheduleID_PayDate]    Script Date: 9/6/2024 5:45:27 PM ******/
ALTER TABLE [dbo].[pr_employee_payroll_mf] ADD  CONSTRAINT [AK_pr_employee_payroll_mf_ID_CompanyId_PayScheduleID_PayDate] UNIQUE NONCLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC,
	[PayScheduleID] ASC,
	[PayDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [AK_pr_leave_type_ID_CompanyId]    Script Date: 9/6/2024 5:45:27 PM ******/
ALTER TABLE [dbo].[pr_leave_type] ADD  CONSTRAINT [AK_pr_leave_type_ID_CompanyId] UNIQUE NONCLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [AK_pr_loan_ID_CompanyId]    Script Date: 9/6/2024 5:45:27 PM ******/
ALTER TABLE [dbo].[pr_loan] ADD  CONSTRAINT [AK_pr_loan_ID_CompanyId] UNIQUE NONCLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [AK_pr_pay_schedule_ID_CompanyId]    Script Date: 9/6/2024 5:45:27 PM ******/
ALTER TABLE [dbo].[pr_pay_schedule] ADD  CONSTRAINT [AK_pr_pay_schedule_ID_CompanyId] UNIQUE NONCLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [AK_pur_invoice_mf_ID_CompanyId]    Script Date: 9/6/2024 5:45:27 PM ******/
ALTER TABLE [dbo].[pur_invoice_mf] ADD  CONSTRAINT [AK_pur_invoice_mf_ID_CompanyId] UNIQUE NONCLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [AK_pur_sale_mf_ID_CompanyId]    Script Date: 9/6/2024 5:45:27 PM ******/
ALTER TABLE [dbo].[pur_sale_mf] ADD  CONSTRAINT [AK_pur_sale_mf_ID_CompanyId] UNIQUE NONCLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [AK_pur_vendor_ID_CompanyId]    Script Date: 9/6/2024 5:45:27 PM ******/
ALTER TABLE [dbo].[pur_vendor] ADD  CONSTRAINT [AK_pur_vendor_ID_CompanyId] UNIQUE NONCLUSTERED 
(
	[ID] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [AK_sys_drop_down_value_ID_DropDownID]    Script Date: 9/6/2024 5:45:27 PM ******/
ALTER TABLE [dbo].[sys_drop_down_value] ADD  CONSTRAINT [AK_sys_drop_down_value_ID_DropDownID] UNIQUE NONCLUSTERED 
(
	[ID] ASC,
	[DropDownID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_sys_setting]    Script Date: 9/6/2024 5:45:27 PM ******/
ALTER TABLE [dbo].[sys_setting] ADD  CONSTRAINT [IX_sys_setting] UNIQUE NONCLUSTERED 
(
	[SettingIdOrName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[emr_appointment_mf] ADD  CONSTRAINT [DF_emr_appointment_mf_IsAdmission]  DEFAULT ((0)) FOR [IsAdmission]
GO
ALTER TABLE [dbo].[emr_appointment_mf] ADD  CONSTRAINT [DF_emr_appointment_mf_IsAdmit]  DEFAULT ((0)) FOR [IsAdmit]
GO
ALTER TABLE [dbo].[pr_employee_shift] ADD  DEFAULT ((0)) FOR [GPSLocationEnable]
GO
ALTER TABLE [dbo].[pr_employee_shift] ADD  CONSTRAINT [DF_pr_employee_shift_WDMonday_1]  DEFAULT ((1)) FOR [WDMonday]
GO
ALTER TABLE [dbo].[pr_employee_shift] ADD  CONSTRAINT [DF_pr_employee_shift_WDTuesday_1]  DEFAULT ((1)) FOR [WDTuesday]
GO
ALTER TABLE [dbo].[pr_employee_shift] ADD  CONSTRAINT [DF_pr_employee_shift_WDWednesday_1]  DEFAULT ((1)) FOR [WDWednesday]
GO
ALTER TABLE [dbo].[pr_employee_shift] ADD  CONSTRAINT [DF_pr_employee_shift_WDThursday_1]  DEFAULT ((1)) FOR [WDThursday]
GO
ALTER TABLE [dbo].[pr_employee_shift] ADD  CONSTRAINT [DF_pr_employee_shift_WDFriday_1]  DEFAULT ((1)) FOR [WDFriday]
GO
ALTER TABLE [dbo].[pr_employee_shift] ADD  CONSTRAINT [DF_pr_employee_shift_WDSatuday_1]  DEFAULT ((0)) FOR [WDSatuday]
GO
ALTER TABLE [dbo].[pr_employee_shift] ADD  CONSTRAINT [DF_pr_employee_shift_WDSunday_1]  DEFAULT ((0)) FOR [WDSunday]
GO
ALTER TABLE [dbo].[pr_time_Summary] ADD  CONSTRAINT [DF_pr_time_Summary_StatusDropDownID]  DEFAULT ((27)) FOR [StatusDropDownID]
GO
ALTER TABLE [dbo].[pr_time_Summary] ADD  CONSTRAINT [DF_pr_time_Summary_StatusID]  DEFAULT ((6)) FOR [StatusID]
GO
ALTER TABLE [dbo].[adm_company]  WITH CHECK ADD  CONSTRAINT [FK_adm_company_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[adm_company] CHECK CONSTRAINT [FK_adm_company_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[adm_company]  WITH CHECK ADD  CONSTRAINT [FK_adm_company_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[adm_company] CHECK CONSTRAINT [FK_adm_company_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[adm_company]  WITH CHECK ADD  CONSTRAINT [FK_adm_company_sys_drop_down_value_CompanyTypeID_CompanyTypeDropDownID] FOREIGN KEY([CompanyTypeID], [CompanyTypeDropDownID])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[adm_company] CHECK CONSTRAINT [FK_adm_company_sys_drop_down_value_CompanyTypeID_CompanyTypeDropDownID]
GO
ALTER TABLE [dbo].[adm_company_location]  WITH CHECK ADD  CONSTRAINT [FK_adm_company_location_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[adm_company_location] CHECK CONSTRAINT [FK_adm_company_location_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[adm_company_location]  WITH CHECK ADD  CONSTRAINT [FK_adm_company_location_sys_drop_down_value_CityID_CityDropDownID] FOREIGN KEY([CityID], [CityDropDownID])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[adm_company_location] CHECK CONSTRAINT [FK_adm_company_location_sys_drop_down_value_CityID_CityDropDownID]
GO
ALTER TABLE [dbo].[adm_company_location]  WITH CHECK ADD  CONSTRAINT [FK_adm_company_location_sys_drop_down_value_CountryID_CountryDropDownID] FOREIGN KEY([CountryID], [CountryDropDownID])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[adm_company_location] CHECK CONSTRAINT [FK_adm_company_location_sys_drop_down_value_CountryID_CountryDropDownID]
GO
ALTER TABLE [dbo].[adm_item]  WITH CHECK ADD  CONSTRAINT [FK_adm_item_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[adm_item] CHECK CONSTRAINT [FK_adm_item_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[adm_item]  WITH CHECK ADD  CONSTRAINT [FK_adm_item_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[adm_item] CHECK CONSTRAINT [FK_adm_item_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[adm_item]  WITH CHECK ADD  CONSTRAINT [FK_adm_item_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[adm_item] CHECK CONSTRAINT [FK_adm_item_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[adm_item]  WITH CHECK ADD  CONSTRAINT [FK_adm_item_sys_drop_down_value_CategoryID_CategoryDropDownID] FOREIGN KEY([CategoryID], [CategoryDropDownID])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[adm_item] CHECK CONSTRAINT [FK_adm_item_sys_drop_down_value_CategoryID_CategoryDropDownID]
GO
ALTER TABLE [dbo].[adm_item]  WITH CHECK ADD  CONSTRAINT [FK_adm_item_sys_drop_down_value_ItemTypeId_ItemTypeDropDownID] FOREIGN KEY([ItemTypeId], [ItemTypeDropDownID])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[adm_item] CHECK CONSTRAINT [FK_adm_item_sys_drop_down_value_ItemTypeId_ItemTypeDropDownID]
GO
ALTER TABLE [dbo].[adm_item]  WITH CHECK ADD  CONSTRAINT [FK_adm_item_sys_drop_down_value_UnitID_UnitDropDownID] FOREIGN KEY([UnitID], [UnitDropDownID])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[adm_item] CHECK CONSTRAINT [FK_adm_item_sys_drop_down_value_UnitID_UnitDropDownID]
GO
ALTER TABLE [dbo].[adm_multilingual_dt]  WITH CHECK ADD  CONSTRAINT [FK_adm_multilingual_dt_adm_multilingual_mf_MultilingualId] FOREIGN KEY([MultilingualId])
REFERENCES [dbo].[adm_multilingual_mf] ([MultilingualId])
GO
ALTER TABLE [dbo].[adm_multilingual_dt] CHECK CONSTRAINT [FK_adm_multilingual_dt_adm_multilingual_mf_MultilingualId]
GO
ALTER TABLE [dbo].[adm_role_dt]  WITH CHECK ADD  CONSTRAINT [FK_adm_role_dt_adm_role_mf_RoleID_CompanyId] FOREIGN KEY([RoleID], [CompanyId])
REFERENCES [dbo].[adm_role_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[adm_role_dt] CHECK CONSTRAINT [FK_adm_role_dt_adm_role_mf_RoleID_CompanyId]
GO
ALTER TABLE [dbo].[adm_role_dt]  WITH CHECK ADD  CONSTRAINT [FK_adm_role_dt_sys_drop_down_value_ScreenID_DropDownScreenID] FOREIGN KEY([ScreenID], [DropDownScreenID])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[adm_role_dt] CHECK CONSTRAINT [FK_adm_role_dt_sys_drop_down_value_ScreenID_DropDownScreenID]
GO
ALTER TABLE [dbo].[adm_role_mf]  WITH CHECK ADD  CONSTRAINT [FK_adm_role_mf_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[adm_role_mf] CHECK CONSTRAINT [FK_adm_role_mf_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[adm_user_company]  WITH CHECK ADD  CONSTRAINT [FK_adm_user_company_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[adm_user_company] CHECK CONSTRAINT [FK_adm_user_company_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[adm_user_company]  WITH CHECK ADD  CONSTRAINT [FK_adm_user_company_adm_role_mf_RoleID_CompanyId] FOREIGN KEY([RoleID], [CompanyId])
REFERENCES [dbo].[adm_role_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[adm_user_company] CHECK CONSTRAINT [FK_adm_user_company_adm_role_mf_RoleID_CompanyId]
GO
ALTER TABLE [dbo].[adm_user_company]  WITH CHECK ADD  CONSTRAINT [FK_adm_user_company_adm_user_mf_AdminID] FOREIGN KEY([AdminID])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[adm_user_company] CHECK CONSTRAINT [FK_adm_user_company_adm_user_mf_AdminID]
GO
ALTER TABLE [dbo].[adm_user_company]  WITH CHECK ADD  CONSTRAINT [FK_adm_user_company_adm_user_mf_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[adm_user_company] CHECK CONSTRAINT [FK_adm_user_company_adm_user_mf_UserID]
GO
ALTER TABLE [dbo].[adm_user_mf]  WITH CHECK ADD  CONSTRAINT [FK_adm_user_mf_sys_drop_down_value_SpecialtyId_SpecialtyDropdownId] FOREIGN KEY([SpecialtyId], [SpecialtyDropdownId])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[adm_user_mf] CHECK CONSTRAINT [FK_adm_user_mf_sys_drop_down_value_SpecialtyId_SpecialtyDropdownId]
GO
ALTER TABLE [dbo].[adm_user_token]  WITH CHECK ADD  CONSTRAINT [FK_adm_user_token_adm_user_mf_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[adm_user_token] CHECK CONSTRAINT [FK_adm_user_token_adm_user_mf_UserID]
GO
ALTER TABLE [dbo].[emr_appointment_mf]  WITH CHECK ADD  CONSTRAINT [FK_emr_appointment_mf_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[emr_appointment_mf] CHECK CONSTRAINT [FK_emr_appointment_mf_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[emr_appointment_mf]  WITH CHECK ADD  CONSTRAINT [FK_emr_appointment_mf_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_appointment_mf] CHECK CONSTRAINT [FK_emr_appointment_mf_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[emr_appointment_mf]  WITH CHECK ADD  CONSTRAINT [FK_emr_appointment_mf_adm_user_mf_DoctorId] FOREIGN KEY([DoctorId])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_appointment_mf] CHECK CONSTRAINT [FK_emr_appointment_mf_adm_user_mf_DoctorId]
GO
ALTER TABLE [dbo].[emr_appointment_mf]  WITH CHECK ADD  CONSTRAINT [FK_emr_appointment_mf_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_appointment_mf] CHECK CONSTRAINT [FK_emr_appointment_mf_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[emr_appointment_mf]  WITH CHECK ADD  CONSTRAINT [FK_emr_appointment_mf_emr_patient_mf_PatientId_CompanyId] FOREIGN KEY([PatientId], [CompanyId])
REFERENCES [dbo].[emr_patient_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[emr_appointment_mf] CHECK CONSTRAINT [FK_emr_appointment_mf_emr_patient_mf_PatientId_CompanyId]
GO
ALTER TABLE [dbo].[emr_bill_type]  WITH CHECK ADD  CONSTRAINT [FK_emr_bill_type_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[emr_bill_type] CHECK CONSTRAINT [FK_emr_bill_type_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[emr_bill_type]  WITH CHECK ADD  CONSTRAINT [FK_emr_bill_type_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_bill_type] CHECK CONSTRAINT [FK_emr_bill_type_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[emr_bill_type]  WITH CHECK ADD  CONSTRAINT [FK_emr_bill_type_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_bill_type] CHECK CONSTRAINT [FK_emr_bill_type_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[emr_complaint]  WITH CHECK ADD  CONSTRAINT [FK_emr_complaint_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_complaint] CHECK CONSTRAINT [FK_emr_complaint_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[emr_complaint]  WITH CHECK ADD  CONSTRAINT [FK_emr_complaint_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_complaint] CHECK CONSTRAINT [FK_emr_complaint_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[emr_diagnos]  WITH CHECK ADD  CONSTRAINT [FK_emr_diagnos_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_diagnos] CHECK CONSTRAINT [FK_emr_diagnos_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[emr_diagnos]  WITH CHECK ADD  CONSTRAINT [FK_emr_diagnos_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_diagnos] CHECK CONSTRAINT [FK_emr_diagnos_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[emr_document]  WITH CHECK ADD  CONSTRAINT [FK_emr_document_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[emr_document] CHECK CONSTRAINT [FK_emr_document_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[emr_document]  WITH CHECK ADD  CONSTRAINT [FK_emr_document_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_document] CHECK CONSTRAINT [FK_emr_document_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[emr_document]  WITH CHECK ADD  CONSTRAINT [FK_emr_document_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_document] CHECK CONSTRAINT [FK_emr_document_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[emr_document]  WITH CHECK ADD  CONSTRAINT [FK_emr_document_sys_drop_down_value_DocumentTypeId_DocumentTypeDropdownId] FOREIGN KEY([DocumentTypeId], [DocumentTypeDropdownId])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[emr_document] CHECK CONSTRAINT [FK_emr_document_sys_drop_down_value_DocumentTypeId_DocumentTypeDropdownId]
GO
ALTER TABLE [dbo].[emr_expense]  WITH CHECK ADD  CONSTRAINT [FK_emr_expense_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[emr_expense] CHECK CONSTRAINT [FK_emr_expense_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[emr_expense]  WITH CHECK ADD  CONSTRAINT [FK_emr_expense_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_expense] CHECK CONSTRAINT [FK_emr_expense_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[emr_expense]  WITH CHECK ADD  CONSTRAINT [FK_emr_expense_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_expense] CHECK CONSTRAINT [FK_emr_expense_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[emr_expense]  WITH CHECK ADD  CONSTRAINT [FK_emr_expense_sys_drop_down_value_CategoryId_CategoryDropdownId] FOREIGN KEY([CategoryId], [CategoryDropdownId])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[emr_expense] CHECK CONSTRAINT [FK_emr_expense_sys_drop_down_value_CategoryId_CategoryDropdownId]
GO
ALTER TABLE [dbo].[emr_income]  WITH CHECK ADD  CONSTRAINT [FK_emr_income_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[emr_income] CHECK CONSTRAINT [FK_emr_income_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[emr_income]  WITH CHECK ADD  CONSTRAINT [FK_emr_income_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_income] CHECK CONSTRAINT [FK_emr_income_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[emr_income]  WITH CHECK ADD  CONSTRAINT [FK_emr_income_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_income] CHECK CONSTRAINT [FK_emr_income_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[emr_income]  WITH CHECK ADD  CONSTRAINT [FK_emr_income_sys_drop_down_value_CategoryId_CategoryDropdownId] FOREIGN KEY([CategoryId], [CategoryDropdownId])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[emr_income] CHECK CONSTRAINT [FK_emr_income_sys_drop_down_value_CategoryId_CategoryDropdownId]
GO
ALTER TABLE [dbo].[emr_instruction]  WITH CHECK ADD  CONSTRAINT [FK_emr_instruction_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_instruction] CHECK CONSTRAINT [FK_emr_instruction_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[emr_instruction]  WITH CHECK ADD  CONSTRAINT [FK_emr_instruction_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_instruction] CHECK CONSTRAINT [FK_emr_instruction_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[emr_investigation]  WITH CHECK ADD  CONSTRAINT [FK_emr_investigation_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_investigation] CHECK CONSTRAINT [FK_emr_investigation_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[emr_investigation]  WITH CHECK ADD  CONSTRAINT [FK_emr_investigation_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_investigation] CHECK CONSTRAINT [FK_emr_investigation_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[emr_medicine]  WITH CHECK ADD  CONSTRAINT [FK_emr_medicine_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[emr_medicine] CHECK CONSTRAINT [FK_emr_medicine_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[emr_medicine]  WITH CHECK ADD  CONSTRAINT [FK_emr_medicine_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_medicine] CHECK CONSTRAINT [FK_emr_medicine_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[emr_medicine]  WITH CHECK ADD  CONSTRAINT [FK_emr_medicine_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_medicine] CHECK CONSTRAINT [FK_emr_medicine_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[emr_medicine]  WITH CHECK ADD  CONSTRAINT [FK_emr_medicine_sys_drop_down_value_TypeId_TypeDropdownId] FOREIGN KEY([TypeId], [TypeDropdownId])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[emr_medicine] CHECK CONSTRAINT [FK_emr_medicine_sys_drop_down_value_TypeId_TypeDropdownId]
GO
ALTER TABLE [dbo].[emr_medicine]  WITH CHECK ADD  CONSTRAINT [FK_emr_medicine_sys_drop_down_value_UnitId_UnitDropdownId] FOREIGN KEY([UnitId], [UnitDropdownId])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[emr_medicine] CHECK CONSTRAINT [FK_emr_medicine_sys_drop_down_value_UnitId_UnitDropdownId]
GO
ALTER TABLE [dbo].[emr_notes_favorite]  WITH CHECK ADD  CONSTRAINT [FK_emr_notes_favorite_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[emr_notes_favorite] CHECK CONSTRAINT [FK_emr_notes_favorite_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[emr_notes_favorite]  WITH CHECK ADD  CONSTRAINT [FK_emr_notes_favorite_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_notes_favorite] CHECK CONSTRAINT [FK_emr_notes_favorite_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[emr_notes_favorite]  WITH CHECK ADD  CONSTRAINT [FK_emr_notes_favorite_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_notes_favorite] CHECK CONSTRAINT [FK_emr_notes_favorite_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[emr_observation]  WITH CHECK ADD  CONSTRAINT [FK_emr_observation_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_observation] CHECK CONSTRAINT [FK_emr_observation_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[emr_observation]  WITH CHECK ADD  CONSTRAINT [FK_emr_observation_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_observation] CHECK CONSTRAINT [FK_emr_observation_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[emr_patient_bill]  WITH CHECK ADD  CONSTRAINT [FK_emr_patient_bill_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[emr_patient_bill] CHECK CONSTRAINT [FK_emr_patient_bill_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[emr_patient_bill]  WITH CHECK ADD  CONSTRAINT [FK_emr_patient_bill_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_patient_bill] CHECK CONSTRAINT [FK_emr_patient_bill_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[emr_patient_bill]  WITH CHECK ADD  CONSTRAINT [FK_emr_patient_bill_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_patient_bill] CHECK CONSTRAINT [FK_emr_patient_bill_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[emr_patient_bill]  WITH CHECK ADD  CONSTRAINT [FK_emr_patient_bill_emr_bill_type_ServiceId_CompanyId] FOREIGN KEY([ServiceId], [CompanyId])
REFERENCES [dbo].[emr_bill_type] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[emr_patient_bill] CHECK CONSTRAINT [FK_emr_patient_bill_emr_bill_type_ServiceId_CompanyId]
GO
ALTER TABLE [dbo].[emr_patient_bill]  WITH CHECK ADD  CONSTRAINT [FK_emr_patient_bill_emr_patient_mf_PatientId_CompanyId] FOREIGN KEY([PatientId], [CompanyId])
REFERENCES [dbo].[emr_patient_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[emr_patient_bill] CHECK CONSTRAINT [FK_emr_patient_bill_emr_patient_mf_PatientId_CompanyId]
GO
ALTER TABLE [dbo].[emr_patient_bill]  WITH CHECK ADD  CONSTRAINT [FK_emr_patient_bill_ipd_admission_AdmissionId_CompanyId] FOREIGN KEY([AdmissionId], [CompanyId])
REFERENCES [dbo].[ipd_admission] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[emr_patient_bill] CHECK CONSTRAINT [FK_emr_patient_bill_ipd_admission_AdmissionId_CompanyId]
GO
ALTER TABLE [dbo].[emr_patient_bill_payment]  WITH CHECK ADD  CONSTRAINT [FK_emr_patient_bill_payment_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[emr_patient_bill_payment] CHECK CONSTRAINT [FK_emr_patient_bill_payment_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[emr_patient_bill_payment]  WITH CHECK ADD  CONSTRAINT [FK_emr_patient_bill_payment_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_patient_bill_payment] CHECK CONSTRAINT [FK_emr_patient_bill_payment_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[emr_patient_bill_payment]  WITH CHECK ADD  CONSTRAINT [FK_emr_patient_bill_payment_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_patient_bill_payment] CHECK CONSTRAINT [FK_emr_patient_bill_payment_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[emr_patient_bill_payment]  WITH CHECK ADD  CONSTRAINT [FK_emr_patient_bill_payment_emr_patient_bill_BillId_CompanyId] FOREIGN KEY([BillId], [CompanyId])
REFERENCES [dbo].[emr_patient_bill] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[emr_patient_bill_payment] CHECK CONSTRAINT [FK_emr_patient_bill_payment_emr_patient_bill_BillId_CompanyId]
GO
ALTER TABLE [dbo].[emr_patient_mf]  WITH CHECK ADD  CONSTRAINT [FK_emr_patient_mf_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[emr_patient_mf] CHECK CONSTRAINT [FK_emr_patient_mf_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[emr_patient_mf]  WITH CHECK ADD  CONSTRAINT [FK_emr_patient_mf_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_patient_mf] CHECK CONSTRAINT [FK_emr_patient_mf_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[emr_patient_mf]  WITH CHECK ADD  CONSTRAINT [FK_emr_patient_mf_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_patient_mf] CHECK CONSTRAINT [FK_emr_patient_mf_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[emr_patient_mf]  WITH CHECK ADD  CONSTRAINT [FK_emr_patient_mf_sys_drop_down_value_BillTypeId_BillTypeDropdownId] FOREIGN KEY([BillTypeId], [BillTypeDropdownId])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[emr_patient_mf] CHECK CONSTRAINT [FK_emr_patient_mf_sys_drop_down_value_BillTypeId_BillTypeDropdownId]
GO
ALTER TABLE [dbo].[emr_patient_mf]  WITH CHECK ADD  CONSTRAINT [FK_emr_patient_mf_sys_drop_down_value_BloodGroupId_BloodGroupDropDownId] FOREIGN KEY([BloodGroupId], [BloodGroupDropDownId])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[emr_patient_mf] CHECK CONSTRAINT [FK_emr_patient_mf_sys_drop_down_value_BloodGroupId_BloodGroupDropDownId]
GO
ALTER TABLE [dbo].[emr_patient_mf]  WITH CHECK ADD  CONSTRAINT [FK_emr_patient_mf_sys_drop_down_value_PrefixTittleId_PrefixDropdownId] FOREIGN KEY([PrefixTittleId], [PrefixDropdownId])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[emr_patient_mf] CHECK CONSTRAINT [FK_emr_patient_mf_sys_drop_down_value_PrefixTittleId_PrefixDropdownId]
GO
ALTER TABLE [dbo].[emr_prescription_complaint]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_complaint_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[emr_prescription_complaint] CHECK CONSTRAINT [FK_emr_prescription_complaint_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[emr_prescription_complaint]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_complaint_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_prescription_complaint] CHECK CONSTRAINT [FK_emr_prescription_complaint_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[emr_prescription_complaint]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_complaint_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_prescription_complaint] CHECK CONSTRAINT [FK_emr_prescription_complaint_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[emr_prescription_complaint]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_complaint_emr_prescription_mf_PrescriptionId_CompanyId] FOREIGN KEY([PrescriptionId], [CompanyId])
REFERENCES [dbo].[emr_prescription_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[emr_prescription_complaint] CHECK CONSTRAINT [FK_emr_prescription_complaint_emr_prescription_mf_PrescriptionId_CompanyId]
GO
ALTER TABLE [dbo].[emr_prescription_diagnos]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_diagnos_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[emr_prescription_diagnos] CHECK CONSTRAINT [FK_emr_prescription_diagnos_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[emr_prescription_diagnos]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_diagnos_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_prescription_diagnos] CHECK CONSTRAINT [FK_emr_prescription_diagnos_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[emr_prescription_diagnos]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_diagnos_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_prescription_diagnos] CHECK CONSTRAINT [FK_emr_prescription_diagnos_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[emr_prescription_diagnos]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_diagnos_emr_prescription_mf_PrescriptionId_CompanyId] FOREIGN KEY([PrescriptionId], [CompanyId])
REFERENCES [dbo].[emr_prescription_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[emr_prescription_diagnos] CHECK CONSTRAINT [FK_emr_prescription_diagnos_emr_prescription_mf_PrescriptionId_CompanyId]
GO
ALTER TABLE [dbo].[emr_prescription_investigation]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_investigation_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[emr_prescription_investigation] CHECK CONSTRAINT [FK_emr_prescription_investigation_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[emr_prescription_investigation]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_investigation_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_prescription_investigation] CHECK CONSTRAINT [FK_emr_prescription_investigation_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[emr_prescription_investigation]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_investigation_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_prescription_investigation] CHECK CONSTRAINT [FK_emr_prescription_investigation_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[emr_prescription_investigation]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_investigation_emr_prescription_mf_PrescriptionId_CompanyId] FOREIGN KEY([PrescriptionId], [CompanyId])
REFERENCES [dbo].[emr_prescription_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[emr_prescription_investigation] CHECK CONSTRAINT [FK_emr_prescription_investigation_emr_prescription_mf_PrescriptionId_CompanyId]
GO
ALTER TABLE [dbo].[emr_prescription_mf]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_mf_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[emr_prescription_mf] CHECK CONSTRAINT [FK_emr_prescription_mf_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[emr_prescription_mf]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_mf_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_prescription_mf] CHECK CONSTRAINT [FK_emr_prescription_mf_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[emr_prescription_mf]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_mf_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_prescription_mf] CHECK CONSTRAINT [FK_emr_prescription_mf_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[emr_prescription_mf]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_mf_emr_patient_mf_PatientId_CompanyId] FOREIGN KEY([PatientId], [CompanyId])
REFERENCES [dbo].[emr_patient_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[emr_prescription_mf] CHECK CONSTRAINT [FK_emr_prescription_mf_emr_patient_mf_PatientId_CompanyId]
GO
ALTER TABLE [dbo].[emr_prescription_observation]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_observation_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[emr_prescription_observation] CHECK CONSTRAINT [FK_emr_prescription_observation_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[emr_prescription_observation]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_observation_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_prescription_observation] CHECK CONSTRAINT [FK_emr_prescription_observation_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[emr_prescription_observation]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_observation_emr_prescription_mf_PrescriptionId_CompanyId] FOREIGN KEY([PrescriptionId], [CompanyId])
REFERENCES [dbo].[emr_prescription_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[emr_prescription_observation] CHECK CONSTRAINT [FK_emr_prescription_observation_emr_prescription_mf_PrescriptionId_CompanyId]
GO
ALTER TABLE [dbo].[emr_prescription_treatment]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_treatment_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[emr_prescription_treatment] CHECK CONSTRAINT [FK_emr_prescription_treatment_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[emr_prescription_treatment]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_treatment_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_prescription_treatment] CHECK CONSTRAINT [FK_emr_prescription_treatment_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[emr_prescription_treatment]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_treatment_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_prescription_treatment] CHECK CONSTRAINT [FK_emr_prescription_treatment_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[emr_prescription_treatment]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_treatment_emr_prescription_mf_PrescriptionId_CompanyId] FOREIGN KEY([PrescriptionId], [CompanyId])
REFERENCES [dbo].[emr_prescription_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[emr_prescription_treatment] CHECK CONSTRAINT [FK_emr_prescription_treatment_emr_prescription_mf_PrescriptionId_CompanyId]
GO
ALTER TABLE [dbo].[emr_prescription_treatment_template]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_treatment_template_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[emr_prescription_treatment_template] CHECK CONSTRAINT [FK_emr_prescription_treatment_template_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[emr_prescription_treatment_template]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_treatment_template_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_prescription_treatment_template] CHECK CONSTRAINT [FK_emr_prescription_treatment_template_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[emr_prescription_treatment_template]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_treatment_template_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_prescription_treatment_template] CHECK CONSTRAINT [FK_emr_prescription_treatment_template_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[emr_prescription_treatment_template]  WITH CHECK ADD  CONSTRAINT [FK_emr_prescription_treatment_template_emr_prescription_mf_PrescriptionId_CompanyId] FOREIGN KEY([PrescriptionId], [CompanyId])
REFERENCES [dbo].[emr_prescription_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[emr_prescription_treatment_template] CHECK CONSTRAINT [FK_emr_prescription_treatment_template_emr_prescription_mf_PrescriptionId_CompanyId]
GO
ALTER TABLE [dbo].[emr_vital]  WITH CHECK ADD  CONSTRAINT [FK_emr_vital_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[emr_vital] CHECK CONSTRAINT [FK_emr_vital_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[emr_vital]  WITH CHECK ADD  CONSTRAINT [FK_emr_vital_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_vital] CHECK CONSTRAINT [FK_emr_vital_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[emr_vital]  WITH CHECK ADD  CONSTRAINT [FK_emr_vital_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[emr_vital] CHECK CONSTRAINT [FK_emr_vital_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[emr_vital]  WITH CHECK ADD  CONSTRAINT [FK_emr_vital_sys_drop_down_value_VitalId_VitalDropdownId] FOREIGN KEY([VitalId], [VitalDropdownId])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[emr_vital] CHECK CONSTRAINT [FK_emr_vital_sys_drop_down_value_VitalId_VitalDropdownId]
GO
ALTER TABLE [dbo].[inv_stock]  WITH CHECK ADD  CONSTRAINT [FK_inv_stock_adm_company] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[inv_stock] CHECK CONSTRAINT [FK_inv_stock_adm_company]
GO
ALTER TABLE [dbo].[inv_stock]  WITH CHECK ADD  CONSTRAINT [FK_inv_stock_adm_item] FOREIGN KEY([ItemID], [CompanyId])
REFERENCES [dbo].[adm_item] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[inv_stock] CHECK CONSTRAINT [FK_inv_stock_adm_item]
GO
ALTER TABLE [dbo].[inv_stock]  WITH CHECK ADD  CONSTRAINT [FK_inv_stock_adm_user_mf] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[inv_stock] CHECK CONSTRAINT [FK_inv_stock_adm_user_mf]
GO
ALTER TABLE [dbo].[inv_stock]  WITH CHECK ADD  CONSTRAINT [FK_inv_stock_adm_user_mf1] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[inv_stock] CHECK CONSTRAINT [FK_inv_stock_adm_user_mf1]
GO
ALTER TABLE [dbo].[ipd_admission]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[ipd_admission] CHECK CONSTRAINT [FK_ipd_admission_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[ipd_admission]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[ipd_admission] CHECK CONSTRAINT [FK_ipd_admission_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[ipd_admission]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[ipd_admission] CHECK CONSTRAINT [FK_ipd_admission_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[ipd_admission]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_emr_patient_mf_PatientId_CompanyId] FOREIGN KEY([PatientId], [CompanyId])
REFERENCES [dbo].[emr_patient_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[ipd_admission] CHECK CONSTRAINT [FK_ipd_admission_emr_patient_mf_PatientId_CompanyId]
GO
ALTER TABLE [dbo].[ipd_admission]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_sys_drop_down_value_AdmissionTypeId_AdmissionTypeDropdownId] FOREIGN KEY([AdmissionTypeId], [AdmissionTypeDropdownId])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[ipd_admission] CHECK CONSTRAINT [FK_ipd_admission_sys_drop_down_value_AdmissionTypeId_AdmissionTypeDropdownId]
GO
ALTER TABLE [dbo].[ipd_admission]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_sys_drop_down_value_BedId_BedDropdownId] FOREIGN KEY([BedId], [BedDropdownId])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[ipd_admission] CHECK CONSTRAINT [FK_ipd_admission_sys_drop_down_value_BedId_BedDropdownId]
GO
ALTER TABLE [dbo].[ipd_admission]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_sys_drop_down_value_RoomId_RoomDropdownId] FOREIGN KEY([RoomId], [RoomDropdownId])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[ipd_admission] CHECK CONSTRAINT [FK_ipd_admission_sys_drop_down_value_RoomId_RoomDropdownId]
GO
ALTER TABLE [dbo].[ipd_admission]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_sys_drop_down_value_WardTypeId_WardTypeDropdownId] FOREIGN KEY([WardTypeId], [WardTypeDropdownId])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[ipd_admission] CHECK CONSTRAINT [FK_ipd_admission_sys_drop_down_value_WardTypeId_WardTypeDropdownId]
GO
ALTER TABLE [dbo].[ipd_admission_charges]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_charges_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[ipd_admission_charges] CHECK CONSTRAINT [FK_ipd_admission_charges_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[ipd_admission_charges]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_charges_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[ipd_admission_charges] CHECK CONSTRAINT [FK_ipd_admission_charges_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[ipd_admission_charges]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_charges_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[ipd_admission_charges] CHECK CONSTRAINT [FK_ipd_admission_charges_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[ipd_admission_charges]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_charges_emr_appointment_mf_AppointmentId_CompanyId] FOREIGN KEY([AppointmentId], [CompanyId])
REFERENCES [dbo].[emr_appointment_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[ipd_admission_charges] CHECK CONSTRAINT [FK_ipd_admission_charges_emr_appointment_mf_AppointmentId_CompanyId]
GO
ALTER TABLE [dbo].[ipd_admission_charges]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_charges_ipd_admission_AdmissionId_CompanyId] FOREIGN KEY([AdmissionId], [CompanyId])
REFERENCES [dbo].[ipd_admission] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[ipd_admission_charges] CHECK CONSTRAINT [FK_ipd_admission_charges_ipd_admission_AdmissionId_CompanyId]
GO
ALTER TABLE [dbo].[ipd_admission_discharge]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_discharge_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[ipd_admission_discharge] CHECK CONSTRAINT [FK_ipd_admission_discharge_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[ipd_admission_discharge]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_discharge_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[ipd_admission_discharge] CHECK CONSTRAINT [FK_ipd_admission_discharge_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[ipd_admission_discharge]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_discharge_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[ipd_admission_discharge] CHECK CONSTRAINT [FK_ipd_admission_discharge_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[ipd_admission_discharge]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_discharge_ipd_admission_AdmissionId_CompanyId] FOREIGN KEY([AdmissionId], [CompanyId])
REFERENCES [dbo].[ipd_admission] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[ipd_admission_discharge] CHECK CONSTRAINT [FK_ipd_admission_discharge_ipd_admission_AdmissionId_CompanyId]
GO
ALTER TABLE [dbo].[ipd_admission_imaging]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_imaging_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[ipd_admission_imaging] CHECK CONSTRAINT [FK_ipd_admission_imaging_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[ipd_admission_imaging]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_imaging_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[ipd_admission_imaging] CHECK CONSTRAINT [FK_ipd_admission_imaging_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[ipd_admission_imaging]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_imaging_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[ipd_admission_imaging] CHECK CONSTRAINT [FK_ipd_admission_imaging_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[ipd_admission_imaging]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_imaging_emr_appointment_mf_AppointmentId_CompanyId] FOREIGN KEY([AppointmentId], [CompanyId])
REFERENCES [dbo].[emr_appointment_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[ipd_admission_imaging] CHECK CONSTRAINT [FK_ipd_admission_imaging_emr_appointment_mf_AppointmentId_CompanyId]
GO
ALTER TABLE [dbo].[ipd_admission_imaging]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_imaging_ipd_admission_AdmissionId_CompanyId] FOREIGN KEY([AdmissionId], [CompanyId])
REFERENCES [dbo].[ipd_admission] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[ipd_admission_imaging] CHECK CONSTRAINT [FK_ipd_admission_imaging_ipd_admission_AdmissionId_CompanyId]
GO
ALTER TABLE [dbo].[ipd_admission_imaging]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_imaging_sys_drop_down_value_ImagingTypeId_ImagingTypeDropdownId] FOREIGN KEY([ImagingTypeId], [ImagingTypeDropdownId])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[ipd_admission_imaging] CHECK CONSTRAINT [FK_ipd_admission_imaging_sys_drop_down_value_ImagingTypeId_ImagingTypeDropdownId]
GO
ALTER TABLE [dbo].[ipd_admission_imaging]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_imaging_sys_drop_down_value_ResultId_ResultDropdownId] FOREIGN KEY([ResultId], [ResultDropdownId])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[ipd_admission_imaging] CHECK CONSTRAINT [FK_ipd_admission_imaging_sys_drop_down_value_ResultId_ResultDropdownId]
GO
ALTER TABLE [dbo].[ipd_admission_imaging]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_imaging_sys_drop_down_value_StatusId_StatusDropdownId] FOREIGN KEY([StatusId], [StatusDropdownId])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[ipd_admission_imaging] CHECK CONSTRAINT [FK_ipd_admission_imaging_sys_drop_down_value_StatusId_StatusDropdownId]
GO
ALTER TABLE [dbo].[ipd_admission_lab]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_lab_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[ipd_admission_lab] CHECK CONSTRAINT [FK_ipd_admission_lab_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[ipd_admission_lab]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_lab_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[ipd_admission_lab] CHECK CONSTRAINT [FK_ipd_admission_lab_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[ipd_admission_lab]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_lab_emr_appointment_mf_AppointmentId_CompanyId] FOREIGN KEY([AppointmentId], [CompanyId])
REFERENCES [dbo].[emr_appointment_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[ipd_admission_lab] CHECK CONSTRAINT [FK_ipd_admission_lab_emr_appointment_mf_AppointmentId_CompanyId]
GO
ALTER TABLE [dbo].[ipd_admission_lab]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_lab_ipd_admission_AdmissionId_CompanyId] FOREIGN KEY([AdmissionId], [CompanyId])
REFERENCES [dbo].[ipd_admission] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[ipd_admission_lab] CHECK CONSTRAINT [FK_ipd_admission_lab_ipd_admission_AdmissionId_CompanyId]
GO
ALTER TABLE [dbo].[ipd_admission_lab]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_lab_sys_drop_down_value_LabTypeId_LabTypeDropdownId] FOREIGN KEY([LabTypeId], [LabTypeDropdownId])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[ipd_admission_lab] CHECK CONSTRAINT [FK_ipd_admission_lab_sys_drop_down_value_LabTypeId_LabTypeDropdownId]
GO
ALTER TABLE [dbo].[ipd_admission_lab]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_lab_sys_drop_down_value_ResultId_ResultDropdownId] FOREIGN KEY([ResultId], [ResultDropdownId])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[ipd_admission_lab] CHECK CONSTRAINT [FK_ipd_admission_lab_sys_drop_down_value_ResultId_ResultDropdownId]
GO
ALTER TABLE [dbo].[ipd_admission_lab]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_lab_sys_drop_down_value_StatusId_StatusDropdownId] FOREIGN KEY([StatusId], [StatusDropdownId])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[ipd_admission_lab] CHECK CONSTRAINT [FK_ipd_admission_lab_sys_drop_down_value_StatusId_StatusDropdownId]
GO
ALTER TABLE [dbo].[ipd_admission_medication]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_medication_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[ipd_admission_medication] CHECK CONSTRAINT [FK_ipd_admission_medication_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[ipd_admission_medication]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_medication_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[ipd_admission_medication] CHECK CONSTRAINT [FK_ipd_admission_medication_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[ipd_admission_medication]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_medication_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[ipd_admission_medication] CHECK CONSTRAINT [FK_ipd_admission_medication_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[ipd_admission_medication]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_medication_emr_appointment_mf_AppointmentId_CompanyId] FOREIGN KEY([AppointmentId], [CompanyId])
REFERENCES [dbo].[emr_appointment_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[ipd_admission_medication] CHECK CONSTRAINT [FK_ipd_admission_medication_emr_appointment_mf_AppointmentId_CompanyId]
GO
ALTER TABLE [dbo].[ipd_admission_medication]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_medication_emr_medicine_MedicineId_CompanyId] FOREIGN KEY([MedicineId], [CompanyId])
REFERENCES [dbo].[emr_medicine] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[ipd_admission_medication] CHECK CONSTRAINT [FK_ipd_admission_medication_emr_medicine_MedicineId_CompanyId]
GO
ALTER TABLE [dbo].[ipd_admission_medication]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_medication_ipd_admission_AdmissionId_CompanyId] FOREIGN KEY([AdmissionId], [CompanyId])
REFERENCES [dbo].[ipd_admission] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[ipd_admission_medication] CHECK CONSTRAINT [FK_ipd_admission_medication_ipd_admission_AdmissionId_CompanyId]
GO
ALTER TABLE [dbo].[ipd_admission_notes]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_notes_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[ipd_admission_notes] CHECK CONSTRAINT [FK_ipd_admission_notes_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[ipd_admission_notes]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_notes_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[ipd_admission_notes] CHECK CONSTRAINT [FK_ipd_admission_notes_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[ipd_admission_notes]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_notes_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[ipd_admission_notes] CHECK CONSTRAINT [FK_ipd_admission_notes_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[ipd_admission_notes]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_notes_emr_appointment_mf_AppointmentId_CompanyId] FOREIGN KEY([AppointmentId], [CompanyId])
REFERENCES [dbo].[emr_appointment_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[ipd_admission_notes] CHECK CONSTRAINT [FK_ipd_admission_notes_emr_appointment_mf_AppointmentId_CompanyId]
GO
ALTER TABLE [dbo].[ipd_admission_notes]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_notes_ipd_admission_AdmissionId_CompanyId] FOREIGN KEY([AdmissionId], [CompanyId])
REFERENCES [dbo].[ipd_admission] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[ipd_admission_notes] CHECK CONSTRAINT [FK_ipd_admission_notes_ipd_admission_AdmissionId_CompanyId]
GO
ALTER TABLE [dbo].[ipd_admission_vital]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_vital_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[ipd_admission_vital] CHECK CONSTRAINT [FK_ipd_admission_vital_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[ipd_admission_vital]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_vital_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[ipd_admission_vital] CHECK CONSTRAINT [FK_ipd_admission_vital_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[ipd_admission_vital]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_vital_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[ipd_admission_vital] CHECK CONSTRAINT [FK_ipd_admission_vital_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[ipd_admission_vital]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_vital_emr_appointment_mf_AppointmentId_CompanyId] FOREIGN KEY([AppointmentId], [CompanyId])
REFERENCES [dbo].[emr_appointment_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[ipd_admission_vital] CHECK CONSTRAINT [FK_ipd_admission_vital_emr_appointment_mf_AppointmentId_CompanyId]
GO
ALTER TABLE [dbo].[ipd_admission_vital]  WITH CHECK ADD  CONSTRAINT [FK_ipd_admission_vital_ipd_admission_AdmissionId_CompanyId] FOREIGN KEY([AdmissionId], [CompanyId])
REFERENCES [dbo].[ipd_admission] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[ipd_admission_vital] CHECK CONSTRAINT [FK_ipd_admission_vital_ipd_admission_AdmissionId_CompanyId]
GO
ALTER TABLE [dbo].[ipd_diagnosis]  WITH CHECK ADD  CONSTRAINT [FK_ipd_diagnosis_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[ipd_diagnosis] CHECK CONSTRAINT [FK_ipd_diagnosis_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[ipd_diagnosis]  WITH CHECK ADD  CONSTRAINT [FK_ipd_diagnosis_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[ipd_diagnosis] CHECK CONSTRAINT [FK_ipd_diagnosis_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[ipd_diagnosis]  WITH CHECK ADD  CONSTRAINT [FK_ipd_diagnosis_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[ipd_diagnosis] CHECK CONSTRAINT [FK_ipd_diagnosis_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[ipd_diagnosis]  WITH CHECK ADD  CONSTRAINT [FK_ipd_diagnosis_ipd_admission_AdmissionId_CompanyId] FOREIGN KEY([AdmissionId], [CompanyId])
REFERENCES [dbo].[ipd_admission] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[ipd_diagnosis] CHECK CONSTRAINT [FK_ipd_diagnosis_ipd_admission_AdmissionId_CompanyId]
GO
ALTER TABLE [dbo].[ipd_procedure_charged]  WITH CHECK ADD  CONSTRAINT [FK_ipd_procedure_charged_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[ipd_procedure_charged] CHECK CONSTRAINT [FK_ipd_procedure_charged_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[ipd_procedure_charged]  WITH CHECK ADD  CONSTRAINT [FK_ipd_procedure_charged_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[ipd_procedure_charged] CHECK CONSTRAINT [FK_ipd_procedure_charged_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[ipd_procedure_charged]  WITH CHECK ADD  CONSTRAINT [FK_ipd_procedure_charged_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[ipd_procedure_charged] CHECK CONSTRAINT [FK_ipd_procedure_charged_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[ipd_procedure_charged]  WITH CHECK ADD  CONSTRAINT [FK_ipd_procedure_charged_emr_appointment_mf_AppointmentId_CompanyId] FOREIGN KEY([AppointmentId], [CompanyId])
REFERENCES [dbo].[emr_appointment_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[ipd_procedure_charged] CHECK CONSTRAINT [FK_ipd_procedure_charged_emr_appointment_mf_AppointmentId_CompanyId]
GO
ALTER TABLE [dbo].[ipd_procedure_charged]  WITH CHECK ADD  CONSTRAINT [FK_ipd_procedure_charged_ipd_procedure_mf_ProcedureId_CompanyId] FOREIGN KEY([ProcedureId], [CompanyId])
REFERENCES [dbo].[ipd_procedure_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[ipd_procedure_charged] CHECK CONSTRAINT [FK_ipd_procedure_charged_ipd_procedure_mf_ProcedureId_CompanyId]
GO
ALTER TABLE [dbo].[ipd_procedure_medication]  WITH CHECK ADD  CONSTRAINT [FK_ipd_procedure_medication_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[ipd_procedure_medication] CHECK CONSTRAINT [FK_ipd_procedure_medication_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[ipd_procedure_medication]  WITH CHECK ADD  CONSTRAINT [FK_ipd_procedure_medication_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[ipd_procedure_medication] CHECK CONSTRAINT [FK_ipd_procedure_medication_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[ipd_procedure_medication]  WITH CHECK ADD  CONSTRAINT [FK_ipd_procedure_medication_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[ipd_procedure_medication] CHECK CONSTRAINT [FK_ipd_procedure_medication_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[ipd_procedure_medication]  WITH CHECK ADD  CONSTRAINT [FK_ipd_procedure_medication_emr_appointment_mf_AppointmentId_CompanyId] FOREIGN KEY([AppointmentId], [CompanyId])
REFERENCES [dbo].[emr_appointment_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[ipd_procedure_medication] CHECK CONSTRAINT [FK_ipd_procedure_medication_emr_appointment_mf_AppointmentId_CompanyId]
GO
ALTER TABLE [dbo].[ipd_procedure_medication]  WITH CHECK ADD  CONSTRAINT [FK_ipd_procedure_medication_emr_medicine_MedicineId_CompanyId] FOREIGN KEY([MedicineId], [CompanyId])
REFERENCES [dbo].[emr_medicine] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[ipd_procedure_medication] CHECK CONSTRAINT [FK_ipd_procedure_medication_emr_medicine_MedicineId_CompanyId]
GO
ALTER TABLE [dbo].[ipd_procedure_medication]  WITH CHECK ADD  CONSTRAINT [FK_ipd_procedure_medication_ipd_procedure_mf_ProcedureId_CompanyId] FOREIGN KEY([ProcedureId], [CompanyId])
REFERENCES [dbo].[ipd_procedure_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[ipd_procedure_medication] CHECK CONSTRAINT [FK_ipd_procedure_medication_ipd_procedure_mf_ProcedureId_CompanyId]
GO
ALTER TABLE [dbo].[ipd_procedure_mf]  WITH CHECK ADD  CONSTRAINT [FK_ipd_procedure_mf_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[ipd_procedure_mf] CHECK CONSTRAINT [FK_ipd_procedure_mf_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[ipd_procedure_mf]  WITH CHECK ADD  CONSTRAINT [FK_ipd_procedure_mf_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[ipd_procedure_mf] CHECK CONSTRAINT [FK_ipd_procedure_mf_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[ipd_procedure_mf]  WITH CHECK ADD  CONSTRAINT [FK_ipd_procedure_mf_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[ipd_procedure_mf] CHECK CONSTRAINT [FK_ipd_procedure_mf_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[ipd_procedure_mf]  WITH CHECK ADD  CONSTRAINT [FK_ipd_procedure_mf_emr_appointment_mf_AppointmentId_CompanyId] FOREIGN KEY([AppointmentId], [CompanyId])
REFERENCES [dbo].[emr_appointment_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[ipd_procedure_mf] CHECK CONSTRAINT [FK_ipd_procedure_mf_emr_appointment_mf_AppointmentId_CompanyId]
GO
ALTER TABLE [dbo].[ipd_procedure_mf]  WITH CHECK ADD  CONSTRAINT [FK_ipd_procedure_mf_ipd_admission_AdmissionId_CompanyId] FOREIGN KEY([AdmissionId], [CompanyId])
REFERENCES [dbo].[ipd_admission] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[ipd_procedure_mf] CHECK CONSTRAINT [FK_ipd_procedure_mf_ipd_admission_AdmissionId_CompanyId]
GO
ALTER TABLE [dbo].[ipd_procedure_mf]  WITH CHECK ADD  CONSTRAINT [FK_ipd_procedure_mf_sys_drop_down_value_CPTCodeId_CPTCodeDropdownId] FOREIGN KEY([CPTCodeId], [CPTCodeDropdownId])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[ipd_procedure_mf] CHECK CONSTRAINT [FK_ipd_procedure_mf_sys_drop_down_value_CPTCodeId_CPTCodeDropdownId]
GO
ALTER TABLE [dbo].[pr_allowance]  WITH CHECK ADD  CONSTRAINT [FK_pr_allowance_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[pr_allowance] CHECK CONSTRAINT [FK_pr_allowance_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[pr_allowance]  WITH CHECK ADD  CONSTRAINT [FK_pr_allowance_sys_drop_down_value_CategoryID_CategoryDropDownID] FOREIGN KEY([CategoryID], [CategoryDropDownID])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[pr_allowance] CHECK CONSTRAINT [FK_pr_allowance_sys_drop_down_value_CategoryID_CategoryDropDownID]
GO
ALTER TABLE [dbo].[pr_deduction_contribution]  WITH CHECK ADD  CONSTRAINT [FK_pr_deduction_contribution_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[pr_deduction_contribution] CHECK CONSTRAINT [FK_pr_deduction_contribution_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[pr_department]  WITH CHECK ADD  CONSTRAINT [FK_pr_department_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[pr_department] CHECK CONSTRAINT [FK_pr_department_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[pr_designation]  WITH CHECK ADD  CONSTRAINT [FK_pr_designation_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[pr_designation] CHECK CONSTRAINT [FK_pr_designation_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[pr_employee_allowance]  WITH CHECK ADD  CONSTRAINT [FK_pr_employee_allowance_pr_allowance_AllowanceID_CompanyId] FOREIGN KEY([AllowanceID], [CompanyId])
REFERENCES [dbo].[pr_allowance] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pr_employee_allowance] CHECK CONSTRAINT [FK_pr_employee_allowance_pr_allowance_AllowanceID_CompanyId]
GO
ALTER TABLE [dbo].[pr_employee_allowance]  WITH CHECK ADD  CONSTRAINT [FK_pr_employee_allowance_pr_employee_mf_EmployeeID_CompanyId] FOREIGN KEY([EmployeeID], [CompanyId])
REFERENCES [dbo].[pr_employee_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pr_employee_allowance] CHECK CONSTRAINT [FK_pr_employee_allowance_pr_employee_mf_EmployeeID_CompanyId]
GO
ALTER TABLE [dbo].[pr_employee_allowance]  WITH CHECK ADD  CONSTRAINT [FK_pr_employee_allowance_pr_pay_schedule_PayScheduleID_CompanyId] FOREIGN KEY([PayScheduleID], [CompanyId])
REFERENCES [dbo].[pr_pay_schedule] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pr_employee_allowance] CHECK CONSTRAINT [FK_pr_employee_allowance_pr_pay_schedule_PayScheduleID_CompanyId]
GO
ALTER TABLE [dbo].[pr_employee_ded_contribution]  WITH CHECK ADD  CONSTRAINT [FK_pr_employee_ded_contribution_pr_deduction_contribution_DeductionContributionID_CompanyId] FOREIGN KEY([DeductionContributionID], [CompanyId])
REFERENCES [dbo].[pr_deduction_contribution] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pr_employee_ded_contribution] CHECK CONSTRAINT [FK_pr_employee_ded_contribution_pr_deduction_contribution_DeductionContributionID_CompanyId]
GO
ALTER TABLE [dbo].[pr_employee_ded_contribution]  WITH CHECK ADD  CONSTRAINT [FK_pr_employee_ded_contribution_pr_employee_mf_EmployeeID_CompanyId] FOREIGN KEY([EmployeeID], [CompanyId])
REFERENCES [dbo].[pr_employee_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pr_employee_ded_contribution] CHECK CONSTRAINT [FK_pr_employee_ded_contribution_pr_employee_mf_EmployeeID_CompanyId]
GO
ALTER TABLE [dbo].[pr_employee_ded_contribution]  WITH CHECK ADD  CONSTRAINT [FK_pr_employee_ded_contribution_pr_pay_schedule_PayScheduleID_CompanyId] FOREIGN KEY([PayScheduleID], [CompanyId])
REFERENCES [dbo].[pr_pay_schedule] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pr_employee_ded_contribution] CHECK CONSTRAINT [FK_pr_employee_ded_contribution_pr_pay_schedule_PayScheduleID_CompanyId]
GO
ALTER TABLE [dbo].[pr_employee_Dependent]  WITH CHECK ADD  CONSTRAINT [FK_pr_employee_Dependent_pr_employee_mf_EmployeeID_CompanyId] FOREIGN KEY([EmployeeID], [CompanyId])
REFERENCES [dbo].[pr_employee_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pr_employee_Dependent] CHECK CONSTRAINT [FK_pr_employee_Dependent_pr_employee_mf_EmployeeID_CompanyId]
GO
ALTER TABLE [dbo].[pr_employee_Dependent]  WITH CHECK ADD  CONSTRAINT [FK_pr_employee_Dependent_sys_drop_down_value_MaritalStatusTypeID_MaritalStatusTypeDropdownID] FOREIGN KEY([MaritalStatusTypeID], [MaritalStatusTypeDropdownID])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[pr_employee_Dependent] CHECK CONSTRAINT [FK_pr_employee_Dependent_sys_drop_down_value_MaritalStatusTypeID_MaritalStatusTypeDropdownID]
GO
ALTER TABLE [dbo].[pr_employee_Dependent]  WITH CHECK ADD  CONSTRAINT [FK_pr_employee_Dependent_sys_drop_down_value_NationalityTypeID_NationalityTypeDropdownID] FOREIGN KEY([NationalityTypeID], [NationalityTypeDropdownID])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[pr_employee_Dependent] CHECK CONSTRAINT [FK_pr_employee_Dependent_sys_drop_down_value_NationalityTypeID_NationalityTypeDropdownID]
GO
ALTER TABLE [dbo].[pr_employee_Dependent]  WITH CHECK ADD  CONSTRAINT [FK_pr_employee_Dependent_sys_drop_down_value_RelationshipTypeID_RelationshipDropdownID] FOREIGN KEY([RelationshipTypeID], [RelationshipDropdownID])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[pr_employee_Dependent] CHECK CONSTRAINT [FK_pr_employee_Dependent_sys_drop_down_value_RelationshipTypeID_RelationshipDropdownID]
GO
ALTER TABLE [dbo].[pr_employee_document]  WITH CHECK ADD  CONSTRAINT [FK_pr_employee_document_pr_employee_mf_EmployeeID_CompanyId] FOREIGN KEY([EmployeeID], [CompanyId])
REFERENCES [dbo].[pr_employee_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pr_employee_document] CHECK CONSTRAINT [FK_pr_employee_document_pr_employee_mf_EmployeeID_CompanyId]
GO
ALTER TABLE [dbo].[pr_employee_document]  WITH CHECK ADD  CONSTRAINT [FK_pr_employee_document_sys_drop_down_value_DocumentTypeID_DocumentTypeDropdownID] FOREIGN KEY([DocumentTypeID], [DocumentTypeDropdownID])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[pr_employee_document] CHECK CONSTRAINT [FK_pr_employee_document_sys_drop_down_value_DocumentTypeID_DocumentTypeDropdownID]
GO
ALTER TABLE [dbo].[pr_employee_leave]  WITH CHECK ADD  CONSTRAINT [FK_pr_employee_leave_pr_employee_mf_EmployeeID_CompanyId] FOREIGN KEY([EmployeeID], [CompanyId])
REFERENCES [dbo].[pr_employee_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pr_employee_leave] CHECK CONSTRAINT [FK_pr_employee_leave_pr_employee_mf_EmployeeID_CompanyId]
GO
ALTER TABLE [dbo].[pr_employee_leave]  WITH CHECK ADD  CONSTRAINT [FK_pr_employee_leave_pr_leave_type_LeaveTypeID_CompanyId] FOREIGN KEY([LeaveTypeID], [CompanyId])
REFERENCES [dbo].[pr_leave_type] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pr_employee_leave] CHECK CONSTRAINT [FK_pr_employee_leave_pr_leave_type_LeaveTypeID_CompanyId]
GO
ALTER TABLE [dbo].[pr_employee_mf]  WITH CHECK ADD  CONSTRAINT [FK_pr_employee_mf_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[pr_employee_mf] CHECK CONSTRAINT [FK_pr_employee_mf_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[pr_employee_mf]  WITH CHECK ADD  CONSTRAINT [FK_pr_employee_mf_pr_department_DepartmentID_CompanyId] FOREIGN KEY([DepartmentID], [CompanyId])
REFERENCES [dbo].[pr_department] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pr_employee_mf] CHECK CONSTRAINT [FK_pr_employee_mf_pr_department_DepartmentID_CompanyId]
GO
ALTER TABLE [dbo].[pr_employee_mf]  WITH CHECK ADD  CONSTRAINT [FK_pr_employee_mf_pr_designation_DesignationID_CompanyId] FOREIGN KEY([DesignationID], [CompanyId])
REFERENCES [dbo].[pr_designation] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pr_employee_mf] CHECK CONSTRAINT [FK_pr_employee_mf_pr_designation_DesignationID_CompanyId]
GO
ALTER TABLE [dbo].[pr_employee_mf]  WITH CHECK ADD  CONSTRAINT [FK_pr_employee_mf_pr_pay_schedule_PayScheduleID_CompanyId] FOREIGN KEY([PayScheduleID], [CompanyId])
REFERENCES [dbo].[pr_pay_schedule] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pr_employee_mf] CHECK CONSTRAINT [FK_pr_employee_mf_pr_pay_schedule_PayScheduleID_CompanyId]
GO
ALTER TABLE [dbo].[pr_employee_mf]  WITH CHECK ADD  CONSTRAINT [FK_pr_employee_mf_sys_drop_down_value_StatusID_StatusDropDownID] FOREIGN KEY([StatusID], [StatusDropDownID])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[pr_employee_mf] CHECK CONSTRAINT [FK_pr_employee_mf_sys_drop_down_value_StatusID_StatusDropDownID]
GO
ALTER TABLE [dbo].[pr_employee_payroll_dt]  WITH CHECK ADD  CONSTRAINT [FK_pr_employee_payroll_dt_pr_employee_mf_EmployeeID_CompanyId] FOREIGN KEY([EmployeeID], [CompanyId])
REFERENCES [dbo].[pr_employee_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pr_employee_payroll_dt] CHECK CONSTRAINT [FK_pr_employee_payroll_dt_pr_employee_mf_EmployeeID_CompanyId]
GO
ALTER TABLE [dbo].[pr_employee_payroll_dt]  WITH CHECK ADD  CONSTRAINT [FK_pr_employee_payroll_dt_pr_employee_payroll_mf_PayrollID_CompanyId_PayScheduleID_PayDate] FOREIGN KEY([PayrollID], [CompanyId], [PayScheduleID], [PayDate])
REFERENCES [dbo].[pr_employee_payroll_mf] ([ID], [CompanyId], [PayScheduleID], [PayDate])
GO
ALTER TABLE [dbo].[pr_employee_payroll_dt] CHECK CONSTRAINT [FK_pr_employee_payroll_dt_pr_employee_payroll_mf_PayrollID_CompanyId_PayScheduleID_PayDate]
GO
ALTER TABLE [dbo].[pr_employee_payroll_mf]  WITH CHECK ADD  CONSTRAINT [FK_pr_employee_payroll_mf_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[pr_employee_payroll_mf] CHECK CONSTRAINT [FK_pr_employee_payroll_mf_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[pr_employee_payroll_mf]  WITH CHECK ADD  CONSTRAINT [FK_pr_employee_payroll_mf_pr_employee_mf_EmployeeID_CompanyId] FOREIGN KEY([EmployeeID], [CompanyId])
REFERENCES [dbo].[pr_employee_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pr_employee_payroll_mf] CHECK CONSTRAINT [FK_pr_employee_payroll_mf_pr_employee_mf_EmployeeID_CompanyId]
GO
ALTER TABLE [dbo].[pr_employee_payroll_mf]  WITH CHECK ADD  CONSTRAINT [FK_pr_employee_payroll_mf_pr_pay_schedule_PayScheduleID_CompanyId] FOREIGN KEY([PayScheduleID], [CompanyId])
REFERENCES [dbo].[pr_pay_schedule] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pr_employee_payroll_mf] CHECK CONSTRAINT [FK_pr_employee_payroll_mf_pr_pay_schedule_PayScheduleID_CompanyId]
GO
ALTER TABLE [dbo].[pr_employee_shift]  WITH CHECK ADD  CONSTRAINT [FK_pr_employee_shift_adm_company] FOREIGN KEY([CompanyID])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[pr_employee_shift] CHECK CONSTRAINT [FK_pr_employee_shift_adm_company]
GO
ALTER TABLE [dbo].[pr_leave_application]  WITH CHECK ADD  CONSTRAINT [FK_pr_leave_application_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[pr_leave_application] CHECK CONSTRAINT [FK_pr_leave_application_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[pr_leave_application]  WITH CHECK ADD  CONSTRAINT [FK_pr_leave_application_pr_employee_mf_EmployeeID_CompanyId] FOREIGN KEY([EmployeeID], [CompanyId])
REFERENCES [dbo].[pr_employee_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pr_leave_application] CHECK CONSTRAINT [FK_pr_leave_application_pr_employee_mf_EmployeeID_CompanyId]
GO
ALTER TABLE [dbo].[pr_leave_application]  WITH CHECK ADD  CONSTRAINT [FK_pr_leave_application_pr_leave_type_LeaveTypeID_CompanyId] FOREIGN KEY([LeaveTypeID], [CompanyId])
REFERENCES [dbo].[pr_leave_type] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pr_leave_application] CHECK CONSTRAINT [FK_pr_leave_application_pr_leave_type_LeaveTypeID_CompanyId]
GO
ALTER TABLE [dbo].[pr_leave_type]  WITH CHECK ADD  CONSTRAINT [FK_pr_leave_type_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[pr_leave_type] CHECK CONSTRAINT [FK_pr_leave_type_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[pr_leave_type]  WITH CHECK ADD  CONSTRAINT [FK_pr_leave_type_sys_drop_down_value_AccrualFrequencyID_AccuralDropDownID] FOREIGN KEY([AccrualFrequencyID], [AccuralDropDownID])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[pr_leave_type] CHECK CONSTRAINT [FK_pr_leave_type_sys_drop_down_value_AccrualFrequencyID_AccuralDropDownID]
GO
ALTER TABLE [dbo].[pr_loan]  WITH CHECK ADD  CONSTRAINT [FK_pr_loan_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[pr_loan] CHECK CONSTRAINT [FK_pr_loan_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[pr_loan]  WITH CHECK ADD  CONSTRAINT [FK_pr_loan_pr_employee_mf_EmployeeID_CompanyId] FOREIGN KEY([EmployeeID], [CompanyId])
REFERENCES [dbo].[pr_employee_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pr_loan] CHECK CONSTRAINT [FK_pr_loan_pr_employee_mf_EmployeeID_CompanyId]
GO
ALTER TABLE [dbo].[pr_loan]  WITH CHECK ADD  CONSTRAINT [FK_pr_loan_sys_drop_down_value_PaymentMethodID_PaymentMethodDropdownID] FOREIGN KEY([PaymentMethodID], [PaymentMethodDropdownID])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[pr_loan] CHECK CONSTRAINT [FK_pr_loan_sys_drop_down_value_PaymentMethodID_PaymentMethodDropdownID]
GO
ALTER TABLE [dbo].[pr_loan_payment_dt]  WITH CHECK ADD  CONSTRAINT [FK_pr_loan_payment_dt_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[pr_loan_payment_dt] CHECK CONSTRAINT [FK_pr_loan_payment_dt_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[pr_loan_payment_dt]  WITH CHECK ADD  CONSTRAINT [FK_pr_loan_payment_dt_pr_loan_LoanID_CompanyId] FOREIGN KEY([LoanID], [CompanyId])
REFERENCES [dbo].[pr_loan] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pr_loan_payment_dt] CHECK CONSTRAINT [FK_pr_loan_payment_dt_pr_loan_LoanID_CompanyId]
GO
ALTER TABLE [dbo].[pr_pay_schedule]  WITH CHECK ADD  CONSTRAINT [FK_pr_pay_schedule_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[pr_pay_schedule] CHECK CONSTRAINT [FK_pr_pay_schedule_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[pr_pay_schedule]  WITH CHECK ADD  CONSTRAINT [FK_pr_pay_schedule_sys_drop_down_value_FallInHolidayID_FallInHolidayDropDownID] FOREIGN KEY([FallInHolidayID], [FallInHolidayDropDownID])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[pr_pay_schedule] CHECK CONSTRAINT [FK_pr_pay_schedule_sys_drop_down_value_FallInHolidayID_FallInHolidayDropDownID]
GO
ALTER TABLE [dbo].[pr_pay_schedule]  WITH CHECK ADD  CONSTRAINT [FK_pr_pay_schedule_sys_drop_down_value_PayTypeID_PayTypeDropDownID] FOREIGN KEY([PayTypeID], [PayTypeDropDownID])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[pr_pay_schedule] CHECK CONSTRAINT [FK_pr_pay_schedule_sys_drop_down_value_PayTypeID_PayTypeDropDownID]
GO
ALTER TABLE [dbo].[pr_time_entry]  WITH CHECK ADD  CONSTRAINT [FK_pr_time_entry_pr_employee_mf_EmployeeID_CompanyId] FOREIGN KEY([EmployeeID], [CompanyId])
REFERENCES [dbo].[pr_employee_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pr_time_entry] CHECK CONSTRAINT [FK_pr_time_entry_pr_employee_mf_EmployeeID_CompanyId]
GO
ALTER TABLE [dbo].[pr_time_entry]  WITH CHECK ADD  CONSTRAINT [FK_pr_time_entry_sys_drop_down_value_StatusID_StatusDropDownID] FOREIGN KEY([StatusID], [StatusDropDownID])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[pr_time_entry] CHECK CONSTRAINT [FK_pr_time_entry_sys_drop_down_value_StatusID_StatusDropDownID]
GO
ALTER TABLE [dbo].[pr_time_rule_dt]  WITH CHECK ADD  CONSTRAINT [FK_pr_time_rule_dt_pr_time_rule_mf] FOREIGN KEY([TimeRuleMfId], [CompanyID])
REFERENCES [dbo].[pr_time_rule_mf] ([ID], [CompanyID])
GO
ALTER TABLE [dbo].[pr_time_rule_dt] CHECK CONSTRAINT [FK_pr_time_rule_dt_pr_time_rule_mf]
GO
ALTER TABLE [dbo].[pr_time_rule_mf]  WITH CHECK ADD  CONSTRAINT [FKpr_time_rule_mf_adm_company] FOREIGN KEY([CompanyID])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[pr_time_rule_mf] CHECK CONSTRAINT [FKpr_time_rule_mf_adm_company]
GO
ALTER TABLE [dbo].[pr_time_Summary]  WITH CHECK ADD  CONSTRAINT [FK_pr_time_Summary_adm_company] FOREIGN KEY([CompanyID])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[pr_time_Summary] CHECK CONSTRAINT [FK_pr_time_Summary_adm_company]
GO
ALTER TABLE [dbo].[pr_time_Summary]  WITH CHECK ADD  CONSTRAINT [FK_pr_time_Summary_pr_employee_mf] FOREIGN KEY([EmployeeID], [CompanyID])
REFERENCES [dbo].[pr_employee_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pr_time_Summary] CHECK CONSTRAINT [FK_pr_time_Summary_pr_employee_mf]
GO
ALTER TABLE [dbo].[pr_time_Summary]  WITH CHECK ADD  CONSTRAINT [FK_pr_time_Summary_sys_drop_down_value] FOREIGN KEY([StatusID], [StatusDropDownID])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[pr_time_Summary] CHECK CONSTRAINT [FK_pr_time_Summary_sys_drop_down_value]
GO
ALTER TABLE [dbo].[pur_invoice_dt]  WITH CHECK ADD  CONSTRAINT [FK_pur_invoice_dt_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[pur_invoice_dt] CHECK CONSTRAINT [FK_pur_invoice_dt_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[pur_invoice_dt]  WITH CHECK ADD  CONSTRAINT [FK_pur_invoice_dt_adm_item_ItemID_CompanyId] FOREIGN KEY([ItemID], [CompanyId])
REFERENCES [dbo].[adm_item] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pur_invoice_dt] CHECK CONSTRAINT [FK_pur_invoice_dt_adm_item_ItemID_CompanyId]
GO
ALTER TABLE [dbo].[pur_invoice_dt]  WITH CHECK ADD  CONSTRAINT [FK_pur_invoice_dt_pur_invoice_mf_InvoiceID_CompanyId] FOREIGN KEY([InvoiceID], [CompanyId])
REFERENCES [dbo].[pur_invoice_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pur_invoice_dt] CHECK CONSTRAINT [FK_pur_invoice_dt_pur_invoice_mf_InvoiceID_CompanyId]
GO
ALTER TABLE [dbo].[pur_invoice_mf]  WITH CHECK ADD  CONSTRAINT [FK_pur_invoice_mf_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[pur_invoice_mf] CHECK CONSTRAINT [FK_pur_invoice_mf_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[pur_invoice_mf]  WITH CHECK ADD  CONSTRAINT [FK_pur_invoice_mf_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[pur_invoice_mf] CHECK CONSTRAINT [FK_pur_invoice_mf_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[pur_invoice_mf]  WITH CHECK ADD  CONSTRAINT [FK_pur_invoice_mf_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[pur_invoice_mf] CHECK CONSTRAINT [FK_pur_invoice_mf_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[pur_invoice_mf]  WITH CHECK ADD  CONSTRAINT [FK_pur_invoice_mf_pur_vendor_VendorID_CompanyId] FOREIGN KEY([VendorID], [CompanyId])
REFERENCES [dbo].[pur_vendor] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pur_invoice_mf] CHECK CONSTRAINT [FK_pur_invoice_mf_pur_vendor_VendorID_CompanyId]
GO
ALTER TABLE [dbo].[pur_payment]  WITH CHECK ADD  CONSTRAINT [FK_pur_payment_adm_company] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[pur_payment] CHECK CONSTRAINT [FK_pur_payment_adm_company]
GO
ALTER TABLE [dbo].[pur_payment]  WITH CHECK ADD  CONSTRAINT [FK_pur_payment_adm_user_mf] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[pur_payment] CHECK CONSTRAINT [FK_pur_payment_adm_user_mf]
GO
ALTER TABLE [dbo].[pur_payment]  WITH CHECK ADD  CONSTRAINT [FK_pur_payment_adm_user_mf1] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[pur_payment] CHECK CONSTRAINT [FK_pur_payment_adm_user_mf1]
GO
ALTER TABLE [dbo].[pur_payment]  WITH CHECK ADD  CONSTRAINT [FK_pur_payment_pur_invoice_mf] FOREIGN KEY([InvoiveId], [CompanyId])
REFERENCES [dbo].[pur_invoice_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pur_payment] CHECK CONSTRAINT [FK_pur_payment_pur_invoice_mf]
GO
ALTER TABLE [dbo].[pur_payment]  WITH CHECK ADD  CONSTRAINT [FK_pur_payment_sys_drop_down_value] FOREIGN KEY([PaymentMethodID], [PaymentMethodDropdownID])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[pur_payment] CHECK CONSTRAINT [FK_pur_payment_sys_drop_down_value]
GO
ALTER TABLE [dbo].[pur_sale_dt]  WITH CHECK ADD  CONSTRAINT [FK_pur_sale_dt_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[pur_sale_dt] CHECK CONSTRAINT [FK_pur_sale_dt_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[pur_sale_dt]  WITH CHECK ADD  CONSTRAINT [FK_pur_sale_dt_adm_item_ItemID_CompanyId] FOREIGN KEY([ItemID], [CompanyId])
REFERENCES [dbo].[adm_item] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pur_sale_dt] CHECK CONSTRAINT [FK_pur_sale_dt_adm_item_ItemID_CompanyId]
GO
ALTER TABLE [dbo].[pur_sale_dt]  WITH CHECK ADD  CONSTRAINT [FK_pur_sale_dt_pur_sale_mf_SaleID_CompanyId] FOREIGN KEY([SaleID], [CompanyId])
REFERENCES [dbo].[pur_sale_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pur_sale_dt] CHECK CONSTRAINT [FK_pur_sale_dt_pur_sale_mf_SaleID_CompanyId]
GO
ALTER TABLE [dbo].[pur_sale_mf]  WITH CHECK ADD  CONSTRAINT [FK_pur_sale_mf_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[pur_sale_mf] CHECK CONSTRAINT [FK_pur_sale_mf_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[pur_sale_mf]  WITH CHECK ADD  CONSTRAINT [FK_pur_sale_mf_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[pur_sale_mf] CHECK CONSTRAINT [FK_pur_sale_mf_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[pur_sale_mf]  WITH CHECK ADD  CONSTRAINT [FK_pur_sale_mf_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[pur_sale_mf] CHECK CONSTRAINT [FK_pur_sale_mf_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[pur_sale_mf]  WITH CHECK ADD  CONSTRAINT [FK_pur_sale_mf_emr_patient_mf] FOREIGN KEY([CustomerId], [CompanyId])
REFERENCES [dbo].[emr_patient_mf] ([ID], [CompanyId])
GO
ALTER TABLE [dbo].[pur_sale_mf] CHECK CONSTRAINT [FK_pur_sale_mf_emr_patient_mf]
GO
ALTER TABLE [dbo].[pur_sale_mf]  WITH CHECK ADD  CONSTRAINT [FK_pur_sale_mf_sys_drop_down_value] FOREIGN KEY([SaleTypeID], [SaleTypeDropDownId])
REFERENCES [dbo].[sys_drop_down_value] ([ID], [DropDownID])
GO
ALTER TABLE [dbo].[pur_sale_mf] CHECK CONSTRAINT [FK_pur_sale_mf_sys_drop_down_value]
GO
ALTER TABLE [dbo].[pur_vendor]  WITH CHECK ADD  CONSTRAINT [FK_pur_vendor_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[pur_vendor] CHECK CONSTRAINT [FK_pur_vendor_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[pur_vendor]  WITH CHECK ADD  CONSTRAINT [FK_pur_vendor_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[pur_vendor] CHECK CONSTRAINT [FK_pur_vendor_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[pur_vendor]  WITH CHECK ADD  CONSTRAINT [FK_pur_vendor_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[pur_vendor] CHECK CONSTRAINT [FK_pur_vendor_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[sys_drop_down_value]  WITH CHECK ADD  CONSTRAINT [FK_sys_drop_down_value_sys_drop_down_mf_DropDownID] FOREIGN KEY([DropDownID])
REFERENCES [dbo].[sys_drop_down_mf] ([ID])
GO
ALTER TABLE [dbo].[sys_drop_down_value] CHECK CONSTRAINT [FK_sys_drop_down_value_sys_drop_down_mf_DropDownID]
GO
ALTER TABLE [dbo].[sys_holidays]  WITH CHECK ADD  CONSTRAINT [FK_sys_holidays_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[sys_holidays] CHECK CONSTRAINT [FK_sys_holidays_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[user_payment]  WITH CHECK ADD  CONSTRAINT [FK_user_payment_adm_company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[adm_company] ([ID])
GO
ALTER TABLE [dbo].[user_payment] CHECK CONSTRAINT [FK_user_payment_adm_company_CompanyId]
GO
ALTER TABLE [dbo].[user_payment]  WITH CHECK ADD  CONSTRAINT [FK_user_payment_adm_user_mf_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[user_payment] CHECK CONSTRAINT [FK_user_payment_adm_user_mf_CreatedBy]
GO
ALTER TABLE [dbo].[user_payment]  WITH CHECK ADD  CONSTRAINT [FK_user_payment_adm_user_mf_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[user_payment] CHECK CONSTRAINT [FK_user_payment_adm_user_mf_ModifiedBy]
GO
ALTER TABLE [dbo].[user_payment]  WITH CHECK ADD  CONSTRAINT [FK_user_payment_adm_user_mf_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[adm_user_mf] ([ID])
GO
ALTER TABLE [dbo].[user_payment] CHECK CONSTRAINT [FK_user_payment_adm_user_mf_UserId]
GO
/****** Object:  StoredProcedure [dbo].[LoadTemplate]    Script Date: 9/6/2024 5:45:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--exec LoadTemplate 1
CREATE procedure [dbo].[LoadTemplate]
@CompanyId numeric(18,0)
as
begin
select mf.Id as MfId,temp.TemplateName,
Medicine = isnull(STUFF((SELECT ', ' + Medicine
                      FROM emr_medicine b 
					  inner join emr_prescription_treatment dt on dt.PrescriptionId=mf.Id
                      WHERE b.Id = dt.MedicineId 
                      FOR XML PATH('')), 1, 2, ''),'')
from emr_prescription_mf mf
inner join emr_prescription_treatment_template temp on temp.PrescriptionId=mf.Id
where mf.CompanyID=@CompanyId and mf.IsTemplate=1
end
GO
/****** Object:  StoredProcedure [dbo].[SP_Adm_GetAllScreen]    Script Date: 9/6/2024 5:45:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[SP_Adm_GetAllScreen]
As

select s.*,(select m.Value from sys_drop_down_value m where DropDownID = s.DependedDropDownID and m.ID = s.DependedDropDownValueID ) ModuleName from sys_drop_down_value s where DropDownID=7







GO
/****** Object:  StoredProcedure [dbo].[SP_CalendarData]    Script Date: 9/6/2024 5:45:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--exec SP_CalendarData 1,1,0,'2022-11-19',1,10,''
CREATE procedure [dbo].[SP_CalendarData]
@CompanyID Numeric(18),
@UserId numeric(18,0),
@StatusId int,
@CurrentDate date,
@CurrentPageNo as Int ,
@RecordPerPage  as Int ,
@SearchText as Varchar(200)
As

declare
@TotalRecord as int,
@IsShowDoctorId as varchar(200),
@StId int,
@AppointmentStatusId int,
@RoleId int,@RoleName nvarchar(50)
set @StId=@StatusId
if(@StatusId=0)
set @StId=1;

set @AppointmentStatusId=(select AppointmentStatusId from adm_user_mf where ID=@UserId)
if(@AppointmentStatusId=0 or @AppointmentStatusId is null)
set @AppointmentStatusId=1;

set @RoleId= (select RoleID from adm_user_company where UserID=@UserId)
set @RoleName=(select RoleName from adm_role_mf where id=@RoleId)


select  mf.ID,u.Name as DoctorName,p.PatientName,mf.AppointmentDate,mf.PatientProblem,bill.ServiceId,bill.PaidAmount,bill.Discount,bill.BillDate,bill.Price,bType.ServiceName,mf.TokenNo,
CONVERT(varchar,mf.AppointmentTime ,108)AppointmentTime,
CONVERT(varchar(100),mf.AppointmentDate)+'T'+CONVERT(varchar,mf.AppointmentTime ,108) as StartDate,
CONVERT(varchar(100),mf.AppointmentDate)+'T'+CONVERT(varchar,dateadd(minute,DATEPART(MINUTE, u.SlotTime),cast(mf.AppointmentTime  as datetime)),108)EndDate,
mf.DoctorId,
mf.StatusId,p.CNIC,mf.PatientId,u.Name as CreatedBy,p.Gender, p.Image,mf.Notes
,case when mf.StatusId=1 then '#dddddd'
when mf.StatusId=2 then '#fd8d64'
when mf.StatusId=5 then '#fdca66'
when mf.StatusId=6 then '#dd2626'
when mf.StatusId=7 then '#65d3fd'
when mf.StatusId=8 then '#6597ff'
when mf.StatusId=9 then '#0bb8ab'
when mf.StatusId=10 then '#fb64a7'
when mf.StatusId=25 then '#6265fd' else '' end as Color
Into #Result
from emr_appointment_mf mf
inner join adm_user_mf u on u.ID=mf.DoctorId
inner join emr_patient_mf p on p.ID=mf.PatientId
left join emr_patient_bill bill on bill.AppointmentId=mf.ID and bill.PatientId=mf.PatientId and bill.DoctorId=mf.DoctorId
left join emr_bill_type bType on bType.ID=bill.ServiceId and bType.CompanyId=bill.CompanyId
where mf.CompanyId=@CompanyID and mf.AppointmentDate=cast(@CurrentDate as  date) and mf.StatusId =((CASE WHEN @AppointmentStatusId =1 THEN mf.StatusId ELSE @AppointmentStatusId END))
and  mf.DoctorId=(case when @RoleName='Doctor' then  @UserId else mf.DoctorId end)
Select * Into #Patient From #Result   


If			IsNull(@SearchText,'') != '' and @SearchText!=null
			Delete From #Patient Where PatientName like '%' + @SearchText + '%'

Select		@TotalRecord = Count(1) From #Patient

Select		* Into #PatientList
From		(
				SELECT *,ROW_NUMBER()OVER (Order by ID) RNumber FROM #Patient 
			) E Where E.RNumber Between ((@CurrentPageNo - 1) * @RecordPerPage) + 1 and (@CurrentPageNo * @RecordPerPage)

Select		*,@TotalRecord TotalRecord from #Result Order By ID


-----gender List
select * from sys_drop_down_value where DropDownID=2 and (CompanyID=@CompanyID or CompanyID is null)

--status list

select ID,Value from sys_drop_down_value where DropDownID=1 and (CompanyID=@CompanyID or CompanyID is null)

--All status list



select tab.ID,tab.Value,cast(tab.IsActive as bit)IsActive,
(
case when tab.ID=1 then (select isnull(count(ID),0) from emr_appointment_mf where CompanyId=@CompanyID and cast(AppointmentDate as date)= cast(@CurrentDate as  date) and DoctorId=(case when @RoleName='Doctor' then  @UserId else DoctorId end)) else
(select isnull(count(ID),0) from emr_appointment_mf where CompanyId=@CompanyID and cast(AppointmentDate as date)= cast(@CurrentDate as  date) and StatusId=tab.ID 
and DoctorId=(case when @RoleName='Doctor' then  @UserId else DoctorId end)
) end
) count
from(
select val.ID,val.Value,(case when (val.ID=@AppointmentStatusId) then 1 else 0 end)IsActive
from sys_drop_down_value val where DropDownID=1 and (CompanyID=@CompanyID or CompanyID is null))tab



----doctor list
exec SP_GetDoctorList @CompanyId,@UserId

-----DoctorIds
set @IsShowDoctorId=(select IsShowDoctor from adm_user_mf where id=@UserId);
select @IsShowDoctorId IsShowDoctorIds

-----DoctorCalander
exec SP_GetDoctorList @CompanyId,@UserId

--select mf.Name,mf.ID,mf.StartTime,mf.EndTime,mf.SlotTime,mf.OffDay,mf.IsShowDoctor,0 IsDoctor,mf.Qualification,mf.Designation,mf.PhoneNo
--from adm_user_company ucomp
--inner join adm_user_mf mf on mf.ID=ucomp.UserID
--inner join adm_role_mf rolemf on rolemf.ID=ucomp.RoleID 
--where ucomp.CompanyID=@CompanyId and rolemf.RoleName='Doctor'and mf.ID not in (select * from ParseCommaStringToInt(@IsShowDoctorId))
--order by mf.ID
------patientList
select * from emr_patient_mf where CompanyId=@CompanyID
-----medicine list
select Id,Medicine from emr_medicine where CompanyID=@CompanyID
--------------------company info
select comp.CompanyID,cp.CompanyName from adm_user_company comp
inner join adm_company cp on cp.ID=comp.CompanyID
where comp.UserID=@UserId
-----------blood list
select * from sys_drop_down_value where DropDownID=17 and (CompanyID=@CompanyID or CompanyID is null)
-------service type
select ID,ServiceName,Price  from emr_bill_type where CompanyId=@CompanyID and ServiceName='Consultation'
---------token no
select isnull(MAX(TokenNo),0)+1 as TokenNo from emr_appointment_mf where CompanyId=@CompanyID and AppointmentDate=cast(GETDATE() as  date)
--BackDated entry
select IsBackDatedAppointment from adm_company where ID=@CompanyID



GO
/****** Object:  StoredProcedure [dbo].[SP_Dashboard]    Script Date: 9/6/2024 5:45:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--exec SP_Dashboard 1,'2024-08-31','2024-08-31',1		
CREATE procedure [dbo].[SP_Dashboard]
@CompanyId numeric(18,0),
@FromeDate date,
@ToDate date,
@UserId numeric(18,0)
as begin
declare @RoleId int,@RoleName nvarchar(50);
set @RoleId= (select RoleID from adm_user_company where UserID=@UserId)
set @RoleName=(select RoleName from adm_role_mf where id=@RoleId)


select isnull(sum(tab.Child),0) Child,
isnull(sum(tab.PreviousApp),0) NewPatients, 
isnull(sum(tab.Male),0) as Male,
isnull(sum(tab.Female),0) as Female,
isnull(sum(tab.Other),0) as Other,
isnull(sum(tab.PatientVisits),0) PatientVisits,
isnull(sum(tab.cnt),0) TotalAppointments,
isnull(sum(tab.MissedAppointment),0) MissedAppointments,
(select isnull(sum(PaidAmount),0) PaymentCollection from emr_patient_bill where CompanyId=@CompanyId and cast(BillDate as date)>= cast(@FromeDate as  date) and cast(BillDate as date)<=cast(@ToDate as date) and DoctorId=(case when @RoleName='Doctor' then  @UserId else DoctorId end)) PaymentCollection,
(select isnull(sum(OutstandingBalance),0)OutstandingAmount from emr_patient_bill where CompanyId=@CompanyId and cast(BillDate as date)>= cast(@FromeDate as  date) and cast(BillDate as date)<=cast(@ToDate as date)and DoctorId=(case when @RoleName='Doctor' then  @UserId else DoctorId end)) OutstandingAmount,
(select isnull(sum(Price),0) ProfessionalFees from emr_patient_bill where CompanyId=@CompanyId and cast(BillDate as date)>= cast(@FromeDate as  date) and cast(BillDate as date)<=cast(@ToDate as date)and DoctorId=(case when @RoleName='Doctor' then  @UserId else DoctorId end)) ProfessionalFees,
(select isnull(sum(ex.Amount),0) from emr_expense ex where ex.CompanyId=@CompanyId and cast(ex.Date as date)>=cast(@FromeDate as  date) and cast(ex.Date as date)<=cast(@ToDate as date)) Expenses, 
(select isnull(sum(ex.DueAmount),0) from emr_income ex where ex.CompanyId=@CompanyId and cast(ex.Date as date)>=cast(@FromeDate as  date) and cast(ex.Date as date)<=cast(@ToDate as date)) OutStandingIncome,
0 Income, 
(select isnull(sum(ex.ReceivedAmount),0) from emr_income ex where ex.CompanyId=@CompanyId and CategoryId=67 and cast(ex.Date as date)>=cast(@FromeDate as  date) and cast(ex.Date as date)<=cast(@ToDate as date)) PharmacyIncome ,
(select isnull(sum(ex.ReceivedAmount),0) from emr_income ex where ex.CompanyId=@CompanyId and CategoryId=69 and cast(ex.Date as date)>=cast(@FromeDate as  date) and cast(ex.Date as date)<=cast(@ToDate as date)) IPDIncome ,
(select isnull(sum(ex.ReceivedAmount),0) from emr_income ex where ex.CompanyId=@CompanyId and CategoryId=65 and cast(ex.Date as date)>=cast(@FromeDate as  date) and cast(ex.Date as date)<=cast(@ToDate as date)) PathologyIncome,
(select isnull(sum(ex.ReceivedAmount),0) from emr_income ex where ex.CompanyId=@CompanyId and CategoryId=66 and cast(ex.Date as date)>=cast(@FromeDate as  date) and cast(ex.Date as date)<=cast(@ToDate as date)) GeneralIncome
FROM
(
select
patient.id,
case when patient.Age<=18 then 1 else 0 end as Child,
case when (select count(mf.Patientid) from emr_appointment_mf mf where mf.PatientId = patient.ID and mf.CompanyId = patient.CompanyId and cast(mf.AppointmentDate as date) < @FromeDate) = 0 then 1 else 0 end  as PreviousApp,
case when patient.Gender=1 then 1 else 0 end as Male,
case when patient.Gender=2 then 1 else 0 end as Female,
case when patient.Gender=3 then 1 else 0 end as Other,
case when app.StatusId not in (1,2,6,25) then 1 else 0 end  PatientVisits,
1 as cnt,
app.ID AppointmentId,
case when app.StatusId in (1,2,6) then 1 else 0 end  MissedAppointment
from emr_patient_mf patient 
inner join emr_appointment_mf app on app.PatientId=patient.ID and app.CompanyId =patient.CompanyId and app.CompanyId=@CompanyId  
where cast(app.AppointmentDate as date)>= cast(@FromeDate as  date) and cast(app.AppointmentDate as date)<=cast(@ToDate as date)
)tab

--Appointment query
select
CONVERT(varchar(15),CAST(mf.AppointmentTime AS TIME),100)StartTime,CONVERT(varchar(15),
CAST(dateadd(minute,DATEPART(MINUTE, admuser.SlotTime),cast(mf.AppointmentTime  as datetime)) AS TIME),100)EndTime,
pat.PatientName,
(case when mf.StatusId=2 then 'Missed' 
 when mf.StatusId=5 then 'Arrival' 
when mf.StatusId=6 then 'Cancelled' 
when mf.StatusId=7 then 'Delay' 
when mf.StatusId=8 then 'Waiting'
when mf.StatusId=9 then 'Checkout'
when mf.StatusId=10 then 'Engaged' else 'Scheduled'end) as ScheduledStatus,admuser.Name
from emr_appointment_mf mf 
inner join emr_patient_mf pat on pat.ID=mf.PatientId
inner join adm_user_mf admuser on admuser.ID=mf.DoctorId
where mf.CompanyId=@CompanyId
and cast(mf.AppointmentDate as date)>=cast(GETDATE() as  date) and cast(mf.AppointmentDate as date)<=cast(GETDATE() as date) 

--Birthday query
select PatientName,DOB,mf.Image,mf.Gender from emr_patient_mf mf where mf.CompanyId=@CompanyId
and cast(mf.DOB as date)>=cast(GETDATE() as  date) and cast(mf.DOB as date)<=cast(GETDATE() as date) 

--FollowUp query
select distinct
CONVERT(varchar(15),CAST(mf.FollowUpTime AS TIME),100)AppointmentTime,pat.PatientName,admuser.Name,pat.ID,mf.IsCreateAppointment,
CONVERT(varchar(15),CAST(dateadd(minute,DATEPART(MINUTE, admuser.SlotTime),cast(mf.FollowUpTime  as datetime)) AS TIME),100)EndTime
from emr_prescription_mf mf 
inner join emr_patient_mf pat on pat.ID=mf.PatientId
inner join adm_user_mf admuser on admuser.ID=mf.DoctorId
where mf.CompanyId=@CompanyId
and cast(mf.FollowUpDate as date)>=cast(GETDATE() as  date) and cast(mf.FollowUpDate as date)<=cast(GETDATE() as date)  


--Expense and income graph
exec SP_IncomeAndExpense @FromeDate,@CompanyId

end


GO
/****** Object:  StoredProcedure [dbo].[SP_DischargeRpt]    Script Date: 9/6/2024 5:45:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--exec SP_DischargeRpt 1,1,1

CREATE procedure [dbo].[SP_DischargeRpt]
@CompanyId numeric(18,0),
@AdmissionId numeric(18,0),
@PatientId numeric(18,0)
as
begin

select mf.MRNO,mf.PatientName,Specval.Value as Dept,mf.Father_Husband,mf.Age,mf.DOB,admis.AdmissionDate,

case when mf.Gender=1 then 'Male' when mf.Gender=2 then 'Female' else 'Other'end as Sex
,comp.CompanyName,
comp.CompanyAddress1,comp.Phone,comp.Email,comp.CompanyLogo,u.Name as DrName,drval.Value,Specval.Value as Specialty
,val.Value as Room,val1.Value as Ward,
vital.Temperature,vital.Weight,vital.SBP,vital.RespiratoryRate,vital.HeartRate,vital.DBP
from ipd_admission_discharge discharge
inner join adm_company comp on comp.ID=discharge.CompanyId
inner join emr_patient_mf mf on mf.ID=discharge.PatientId and mf.CompanyId=@CompanyId
inner join ipd_admission admis on admis.ID=discharge.AdmissionId and admis.CompanyId=@CompanyId
inner join adm_user_mf u on u.ID=admis.DoctorId
left join sys_drop_down_value drval on drval.ID=admis.AdmissionTypeId and drval.DropDownID=admis.AdmissionTypeDropdownId
left join sys_drop_down_value Specval on Specval.ID=u.SpecialtyId and Specval.DropDownID=u.SpecialtyDropdownId
left join sys_drop_down_value val on val.ID=admis.RoomId and val.DropDownID=admis.RoomDropdownId
left join sys_drop_down_value val1 on val.ID=admis.WardTypeId and val.DropDownID=admis.WardTypeDropdownId
left join(
select Temperature,Weight,SBP,RespiratoryRate,HeartRate,DBP,AdmissionId,PatientId,CompanyId from ipd_admission_vital where AdmissionId=@AdmissionId and PatientId=@PatientId and CompanyId=@CompanyId
and ID=(select Max(ID) from ipd_admission_vital where AdmissionId=@AdmissionId and PatientId=@PatientId and CompanyId=@CompanyId)
) vital on vital.AdmissionId=discharge.AdmissionId and vital.PatientId=discharge.PatientId and vital.CompanyId=discharge.CompanyID
where discharge.AdmissionId=@AdmissionId and discharge.PatientId=@PatientId



---ipd_admission_discharge
select * from ipd_admission_discharge where AdmissionId=@AdmissionId and PatientId=@PatientId and CompanyID=@CompanyId
end




GO
/****** Object:  StoredProcedure [dbo].[SP_Emr_CashFlowRpt]    Script Date: 9/6/2024 5:45:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[SP_Emr_CashFlowRpt]
@CompanyId numeric(18,0),
@FromeDate date,
@ToDate date,
@Type int
as
begin

if(@Type=0)
begin
select BillDate as Date,'Professional Fees'as Description ,Sum(Price)as Amount , 'Cr' as Credit from emr_patient_bill bill
where CompanyId=@CompanyId and cast(BillDate as date) > cast(@FromeDate as date) and cast(BillDate as date) < cast(@ToDate as date)
group by BillDate
union all

select Date,'Income'as Description,sum(DueAmount)Amount ,'Cr' as Credit from emr_income
where CompanyId=@CompanyId and cast(Date as date) > cast(@FromeDate as date) and cast(Date as date) < cast(@ToDate as date)
group by Date
union all
select Date,'Expenses'as Description,sum(Amount)Amount ,'Dr' as Credit from emr_expense
where CompanyId=@CompanyId and cast(Date as date) > cast(@FromeDate as date) and cast(Date as date) < cast(@ToDate as date)
group by Date
end
if(@Type=1)
begin
select Date,'Income'as Description,sum(DueAmount)Amount ,'Cr' as Credit from emr_income
where CompanyId=@CompanyId and cast(Date as date) > cast(@FromeDate as date) and cast(Date as date) < cast(@ToDate as date)
group by Date
end
if(@Type=2)
begin
select Date,'Expenses'as Description,sum(Amount)Amount ,'Dr' as Credit from emr_expense
where CompanyId=@CompanyId and cast(Date as date) > cast(@FromeDate as date) and cast(Date as date) < cast(@ToDate as date)
group by Date
end
if(@Type=3)
begin
select BillDate as Date,'Professional Fees'as Description ,Sum(Price)as Amount , 'Cr' as Credit from emr_patient_bill bill
where CompanyId=@CompanyId and cast(BillDate as date) > cast(@FromeDate as date) and cast(BillDate as date) < cast(@ToDate as date)
group by BillDate
end
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetAdmitPatientSummaryByPatientId]    Script Date: 9/6/2024 5:45:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--exec GetAdmitPatientSummaryByPatientId 1,4
CREATE procedure [dbo].[SP_GetAdmitPatientSummaryByPatientId]
@CompanyID Numeric(18),
@PatientId numeric(18,0),
@AdmissionId numeric(18,0)
as


select mf.* from emr_patient_mf mf where CompanyId=@CompanyID and ID=@PatientId

----Gender List
select * from sys_drop_down_value where DropDownID=2 and (CompanyID=@CompanyID or CompanyID is null)
-----medicine List
select Id,Medicine from emr_medicine where CompanyID=@CompanyID
-----------blood list
select * from sys_drop_down_value where DropDownID=17 and (CompanyID=@CompanyID or CompanyID is null)
---bill type
select ID,Value from sys_drop_down_value where DropDownID=22 and (CompanyID=@CompanyID or CompanyID is null)

---Tittle list
select ID,Value from sys_drop_down_value where DropDownID=23 and (CompanyID=@CompanyID or CompanyID is null)

---------PaidAndOutamount
select isnull(sum(b.OutstandingBalance),0)PaidAndOutamount
from emr_prescription_mf p
inner join emr_patient_bill b on b.PatientId=p.PatientId and b.DoctorId=p.DoctorId
where p.PatientId=@PatientId and p.CompanyID=@CompanyID


---admissionNo
select AdmissionNo from ipd_admission where PatientId=@PatientId and CompanyId=@CompanyID
---ipd_diagnosis list

select * from ipd_diagnosis where CompanyID=@CompanyID and AdmissionId=@AdmissionId



GO
/****** Object:  StoredProcedure [dbo].[SP_GetDoctorList]    Script Date: 9/6/2024 5:45:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[SP_GetDoctorList]
@CompanyId numeric(18,0),
@UserId numeric(18,0)
as
begin
declare @RoleId int,@RoleName nvarchar(50),@IsShowDoctorId as varchar(200);
set @IsShowDoctorId=(select IsShowDoctor from adm_user_mf where id=@UserId);

set @RoleId= (select RoleID from adm_user_company where UserID=@UserId)

set @RoleName=(select RoleName from adm_role_mf where id=@RoleId)
if(@RoleName='Administrator')
begin
select mf.Name,mf.ID,mf.StartTime,mf.EndTime,mf.SlotTime,mf.OffDay,mf.IsShowDoctor,cast(0 as bit)IsDoctor,mf.Qualification,mf.Designation,mf.PhoneNo from adm_user_company ucomp
inner join adm_user_mf mf on mf.ID=ucomp.UserID
inner join adm_role_mf rolemf on rolemf.ID=ucomp.RoleID 
where ucomp.CompanyID=@CompanyId and rolemf.RoleName='Doctor'
order by mf.ID
end

else if(@RoleName='Doctor')
begin
select mf.Name,mf.ID,mf.StartTime,mf.EndTime,mf.SlotTime,mf.OffDay,mf.IsShowDoctor,0 IsDoctor,mf.Qualification,mf.Designation,mf.PhoneNo
from adm_user_company ucomp
inner join adm_user_mf mf on mf.ID=ucomp.UserID
inner join adm_role_mf rolemf on rolemf.ID=ucomp.RoleID 
where ucomp.CompanyID=@CompanyId and rolemf.RoleName='Doctor'and mf.ID=@UserId
order by mf.ID
end
else
begin
select mf.Name,mf.ID,mf.StartTime,mf.EndTime,mf.SlotTime,mf.OffDay,mf.IsShowDoctor,cast(0 as bit)IsDoctor,mf.Qualification,mf.Designation,mf.PhoneNo from adm_user_company ucomp
inner join adm_user_mf mf on mf.ID=ucomp.UserID
inner join adm_role_mf rolemf on rolemf.ID=ucomp.RoleID 
where ucomp.CompanyID=@CompanyId and rolemf.RoleName='Doctor' and mf.ID=@UserId
order by mf.ID
end



end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetOpenPayrollPayScheduleIds]    Script Date: 9/6/2024 5:45:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[SP_GetOpenPayrollPayScheduleIds]
 @CompanyID     NUMERIC(18, 0)
As
begin
select distinct PayScheduleID from pr_employee_payroll_mf where CompanyID=@CompanyID and Status='O'
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetPatientSummaryByPatientId]    Script Date: 9/6/2024 5:45:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--exec SP_GetPatientSummaryByPatientId 1,27
CREATE procedure [dbo].[SP_GetPatientSummaryByPatientId]
@CompanyID Numeric(18),
@PatientId numeric(18,0)
as


select mf.* from emr_patient_mf mf where CompanyId=@CompanyID and ID=@PatientId

----Gender List
select * from sys_drop_down_value where DropDownID=2 and (CompanyID=@CompanyID or CompanyID is null)
-----medicine List
select Id,Medicine from emr_medicine where CompanyID=@CompanyID
-----------blood list
select * from sys_drop_down_value where DropDownID=17 and (CompanyID=@CompanyID or CompanyID is null)
---bill type
select ID,Value from sys_drop_down_value where DropDownID=22 and (CompanyID=@CompanyID or CompanyID is null)

---Tittle list
select ID,Value from sys_drop_down_value where DropDownID=23 and (CompanyID=@CompanyID or CompanyID is null)

---------PaidAndOutamount
select isnull(sum(b.OutstandingBalance),0)PaidAndOutamount
from emr_prescription_mf p
inner join emr_patient_bill b on b.PatientId=p.PatientId and b.DoctorId=p.DoctorId
where p.PatientId=@PatientId and p.CompanyID=@CompanyID


---admissionNo
select isnull(AdmissionNo,0)AdmissionNo from ipd_admission where PatientId=@PatientId and CompanyId=@CompanyID
---Prescription list
--select p.Id,p.AppointmentDate,t.*,c.*,inv.*,ob.*,d.* from emr_prescription_mf p
--inner join emr_prescription_treatment t on t.PrescriptionId=p.Id
--left outer join emr_prescription_complaint c on c.PrescriptionId=p.Id
--left outer join emr_prescription_investigation inv on inv.PrescriptionId=p.Id
--left outer join emr_prescription_observation ob on ob.PrescriptionId=p.Id
--left outer join emr_prescription_diagnos d on d.PrescriptionId=p.Id
--where p.PatientId=1 and p.CompanyID=@CompanyID
---------PaidAndOutamount
--select isnull(sum(b.OutstandingBalance),0)PaidAndOutamount from emr_prescription_mf p
--inner join emr_patient_bill b on b.ID=p.PatientId and b.DoctorId=p.DoctorId
--where p.PatientId=@PatientId and p.CompanyID=@CompanyID



GO
/****** Object:  StoredProcedure [dbo].[SP_GetPrescriptionAfterSaveRecord]    Script Date: 9/6/2024 5:45:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--SP_GetPrescriptionAfterSaveRecord 1 ,4,1

CREATE procedure [dbo].[SP_GetPrescriptionAfterSaveRecord]
@CompanyID Numeric(18),
@PatientId numeric(18,0),
@UserId numeric(18,0)

as
CREATE TABLE #Temp(
	[Name] [nvarchar](650) NULL,
	[ID] Numeric(18),	
	[StartTime] Time(7),
	EndTime Time(7),
	SlotTime Time(7),
	OffDay [nvarchar](650) NULL,
	IsShowDoctor[nvarchar](650) NULL,
	IsDoctor bit,
	[Qualification] [nvarchar](255) NULL,	
	[Designation] [nvarchar](255) NULL,
	[PhoneNo] [float] NULL	
);

select p.PatientName,p.Mobile,p.Age,c.CompanyLogo as ClinicIogo from emr_patient_mf p 
inner join adm_company c on c.ID=p.CompanyId
where p.id=@PatientId and p.CompanyId=@CompanyID
--
INSERT INTO #Temp
EXEC SP_GetDoctorList @CompanyID,@UserId

select Name,Qualification,Designation,PhoneNo from #Temp where ID=@PatientId

---vitallist
select v.Measure+' '+d.Unit as Measure,d.Value as Name from emr_vital v
inner join sys_drop_down_value d on d.ID=v.VitalId
where v.CompanyID=@CompanyID and v.PatientId=@PatientId


GO
/****** Object:  StoredProcedure [dbo].[SP_GetPrescriptionByPatientId]    Script Date: 9/6/2024 5:45:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--exec SP_GetPrescriptionByPatientId 1,2,43,'2022-11-19'
CREATE procedure [dbo].[SP_GetPrescriptionByPatientId]
@CompanyID Numeric(18),
@PatientId numeric(18,0),
@AppontId numeric(18,0),
@Date date
as

declare @DoctorId int;

set @DoctorId=(select DoctorId from emr_appointment_mf where CompanyId=@CompanyID and ID=@AppontId)
select @DoctorId as Doctorid;
select * from emr_patient_mf p where p.id=@PatientId and p.CompanyId=@CompanyID
-----CurntPrevDate
select ID, PatientId,convert(varchar, AppointmentDate, 23)AppointmentDate from emr_appointment_mf
where PatientId=@PatientId and CompanyId=@CompanyID
order by AppointmentDate desc
---ComplaintList
SELECT  distinct top 10  cmp.Id as id,
case when dt.Complaint is null then cmp.Complaint else dt.Complaint end as Complaint,ft.id as favoriteId,
(case when ft.ReferenceId is null then 0 else 1 end) as Isfavorite FROM emr_complaint cmp 
inner join emr_prescription_complaint dt on dt.ComplaintId = cmp.Id 
and dt.CompanyID=@CompanyID 
left outer join emr_notes_favorite ft on ft.ReferenceId = cmp.Id and ft.ReferenceType = 'C'
and ft.DoctorId =@DoctorId where ft.CompanyID=@CompanyID
---ObservationList
SELECT  distinct top 10  ob.Id as id,
case when dt.Observation is null then ob.Observation else dt.Observation end as Observation ,
ft.id as favoriteId,(case when ft.ReferenceId is null then 0 else 1 end) as Isfavorite 
FROM emr_observation ob 
inner join emr_prescription_observation dt on dt.ObservationId = ob.Id 
and dt.CompanyID=@CompanyID
left outer join emr_notes_favorite ft on ft.ReferenceId = ob.Id and ft.ReferenceType = 'O' 
and ft.DoctorId = @DoctorId where ft.CompanyID=@CompanyID
---InvestigationsList
SELECT  distinct top 10  inv.Id as id,
case when dt.Investigation is null then inv.Investigation else dt.Investigation end as Investigation ,
ft.id as favoriteId,(case when ft.ReferenceId is null then 0 else 1 end) as Isfavorite
FROM emr_investigation inv 
inner join emr_prescription_investigation dt on dt.InvestigationId = inv.Id 
and dt.CompanyID=@CompanyID
left outer join emr_notes_favorite ft on ft.ReferenceId = inv.Id 
and ft.ReferenceType = 'I' and ft.DoctorId = @DoctorId
where ft.CompanyID=@CompanyID
----DiagnosisList
SELECT  distinct top 10  dia.Id as id,
case when dt.Diagnos is null then dia.Diagnos else dt.Diagnos end as Diagnos ,
ft.id as favoriteId,(case when ft.ReferenceId is null then 0 else 1 end) as Isfavorite
FROM emr_diagnos dia inner join emr_prescription_diagnos dt on dt.DiagnosId = dia.Id
and dt.CompanyID=@CompanyID
left outer join emr_notes_favorite ft on ft.ReferenceId = dia.Id and ft.ReferenceType = 'D' 
and ft.DoctorId =@DoctorId
where ft.CompanyID=@CompanyID

---bill type list
select ID,Value from sys_drop_down_value where DropDownID=22 and (CompanyID=@CompanyID or CompanyID is null)
---Tittle list
select ID,Value from sys_drop_down_value where DropDownID=23 and (CompanyID=@CompanyID or CompanyID is null)
---AppointmentList


select mf.ID,u.Name as DoctorName,p.PatientName,mf.AppointmentDate,CONVERT(varchar(15),mf.AppointmentTime,22)AppointmentTime,mf.DoctorId,mf.StatusId,mf.PatientId,
p.CNIC,mf.Notes as Note,usr.Name as CreatedBy
,case when mf.StatusId=1 then '#dddddd'
when mf.StatusId=2 then '#fd8d64'
when mf.StatusId=5 then '#fdca66'
when mf.StatusId=6 then '#dd2626'
when mf.StatusId=7 then '#65d3fd'
when mf.StatusId=8 then '#6597ff'
when mf.StatusId=9 then '#0bb8ab'
when mf.StatusId=10 then '#fb64a7'
when mf.StatusId=25 then '#6265fd' else '' end as Color,
CONVERT(varchar(100),mf.AppointmentDate)+'T'+CONVERT(varchar,mf.AppointmentTime ,108) as StartDate,
CONVERT(varchar(100),mf.AppointmentDate)+'T'+CONVERT(varchar,dateadd(minute,DATEPART(MINUTE, u.SlotTime),cast(mf.AppointmentTime  as datetime)),108)EndDate
from emr_appointment_mf mf
inner join adm_user_mf u on u.ID=mf.DoctorId
inner join adm_user_mf usr on usr.ID=mf.CreatedBy
inner join emr_patient_mf p on p.ID=mf.PatientId and p.CompanyId=@CompanyID 
where mf.CompanyId=@CompanyID and AppointmentDate=cast(@Date as  date)  and p.ID=@PatientId


 
--prescription_mf
select  pmf.*,t.TemplateName from emr_prescription_mf pmf 
left join emr_prescription_treatment_template t on t.PrescriptionId=pmf.Id and t.CompanyID=@CompanyID
where pmf.PatientId=@PatientId and pmf.CompanyID=@CompanyID and pmf.AppointmentDate=CAST(@Date as date) and DoctorId=@DoctorId
order by AppointmentDate desc
---prescription_treatment
select t.*  from emr_prescription_mf pmf 
inner join emr_prescription_treatment t on t.PrescriptionId=pmf.Id and t.CompanyID=@CompanyID
where pmf.PatientId=@PatientId and pmf.CompanyID=@CompanyID and pmf.AppointmentDate=CAST(@Date as date)and pmf.DoctorId=@DoctorId

---prescription_complaint
select c.*  from emr_prescription_mf pmf 
inner join emr_prescription_complaint c on c.PrescriptionId=pmf.Id and c.CompanyID=@CompanyID
where pmf.PatientId=@PatientId and pmf.CompanyID=@CompanyID and pmf.AppointmentDate=CAST(@Date as date) and pmf.DoctorId=@DoctorId

---prescription_diagnos
select d.*  from emr_prescription_mf pmf 
inner join emr_prescription_diagnos d on d.PrescriptionId=pmf.Id and d.CompanyID=@CompanyID
where pmf.PatientId=@PatientId and pmf.CompanyID=@CompanyID and pmf.AppointmentDate=CAST(@Date as date)and pmf.DoctorId=@DoctorId
---prescription_investigation
select inv.*  from emr_prescription_mf pmf 
inner join emr_prescription_investigation inv on inv.PrescriptionId=pmf.Id and inv.CompanyID=@CompanyID
where pmf.PatientId=@PatientId and pmf.CompanyID=@CompanyID and pmf.AppointmentDate=CAST(@Date as date)and pmf.DoctorId=@DoctorId
---prescription_observation
select ob.*  from emr_prescription_mf pmf 
inner join emr_prescription_observation ob on ob.PrescriptionId=pmf.Id and ob.CompanyID=@CompanyID
where pmf.PatientId=@PatientId and pmf.CompanyID=@CompanyID and pmf.AppointmentDate=CAST(@Date as date)and pmf.DoctorId=@DoctorId





GO
/****** Object:  StoredProcedure [dbo].[SP_GetPrescriptionByPatientIdAndDoctorId]    Script Date: 9/6/2024 5:45:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--exec SP_GetPrescriptionByPatientIdAndDoctorId 1,13,6,'2022-03-18',1'
CREATE procedure [dbo].[SP_GetPrescriptionByPatientIdAndDoctorId]
@CompanyID Numeric(18),
@PatientId numeric(18,0),
@Doctorid numeric(18,0),
@Date date,
@UserId numeric(18,0)
as
declare @IsShowDoctorId as varchar(200)
---patientInfo 0
select * from emr_patient_mf p where p.id=@PatientId AND P.CompanyId=@CompanyID
-----CurntPrevDate 1
select PatientId,convert(varchar, AppointmentDate, 23)AppointmentDate from emr_appointment_mf
where PatientId=@PatientId and CompanyId=@CompanyID and DoctorId=@Doctorid
order by AppointmentDate desc

---ComplaintList 2
SELECT  distinct top 10  cmp.Id as id,
case when dt.Complaint is null then cmp.Complaint else dt.Complaint end as Complaint,ft.id as favoriteId,
(case when ft.ReferenceId is null then 0 else 1 end) as Isfavorite FROM emr_complaint cmp 
inner join emr_prescription_complaint dt on dt.ComplaintId = cmp.Id 
and dt.CompanyID=@CompanyID 
left outer join emr_notes_favorite ft on ft.ReferenceId = cmp.Id and ft.ReferenceType = 'C'
and ft.DoctorId =@DoctorId where ft.CompanyID=@CompanyID
---ObservationList 3
SELECT  distinct top 10  ob.Id as id,
case when dt.Observation is null then ob.Observation else dt.Observation end as Observation ,
ft.id as favoriteId,(case when ft.ReferenceId is null then 0 else 1 end) as Isfavorite 
FROM emr_observation ob 
inner join emr_prescription_observation dt on dt.ObservationId = ob.Id 
and dt.CompanyID=@CompanyID
left outer join emr_notes_favorite ft on ft.ReferenceId = ob.Id and ft.ReferenceType = 'O' 
and ft.DoctorId = @DoctorId where ft.CompanyID=@CompanyID
---InvestigationsList 4
SELECT  distinct top 10  inv.Id as id,
case when dt.Investigation is null then inv.Investigation else dt.Investigation end as Investigation ,
ft.id as favoriteId,(case when ft.ReferenceId is null then 0 else 1 end) as Isfavorite
FROM emr_investigation inv 
inner join emr_prescription_investigation dt on dt.InvestigationId = inv.Id 
and dt.CompanyID=@CompanyID
left outer join emr_notes_favorite ft on ft.ReferenceId = inv.Id 
and ft.ReferenceType = 'I' and ft.DoctorId = @DoctorId
where ft.CompanyID=@CompanyID
----DiagnosisList 5
SELECT  distinct top 10  dia.Id as id,
case when dt.Diagnos is null then dia.Diagnos else dt.Diagnos end as Diagnos ,
ft.id as favoriteId,(case when ft.ReferenceId is null then 0 else 1 end) as Isfavorite
FROM emr_diagnos dia inner join emr_prescription_diagnos dt on dt.DiagnosId = dia.Id
and dt.CompanyID=@CompanyID
left outer join emr_notes_favorite ft on ft.ReferenceId = dia.Id and ft.ReferenceType = 'D' 
and ft.DoctorId =@DoctorId
where ft.CompanyID=@CompanyID

---BackDated entry 6
select IsBackDatedAppointment from adm_company where id=@CompanyID
---AppointmentList 7
select mf.ID,u.Name as DoctorName,p.PatientName,mf.AppointmentDate,mf.AppointmentTime,mf.DoctorId,mf.StatusId,mf.PatientId,
p.CNIC,mf.Notes as Note,usr.Name as CreatedBy
,case when mf.StatusId=1 then '#dddddd'
when mf.StatusId=2 then '#fd8d64'
when mf.StatusId=5 then '#fdca66'
when mf.StatusId=6 then '#dd2626'
when mf.StatusId=7 then '#65d3fd'
when mf.StatusId=8 then '#6597ff'
when mf.StatusId=9 then '#0bb8ab'
when mf.StatusId=10 then '#fb64a7'
when mf.StatusId=25 then '#6265fd' else '' end as Color,
CONVERT(varchar(100),mf.AppointmentDate)+'T'+CONVERT(varchar,mf.AppointmentTime ,108) as StartDate,
CONVERT(varchar(100),mf.AppointmentDate)+'T'+CONVERT(varchar,dateadd(minute,DATEPART(MINUTE, u.SlotTime),cast(mf.AppointmentTime  as datetime)),108)EndDate
from emr_appointment_mf mf
inner join adm_user_mf u on u.ID=mf.DoctorId
inner join adm_user_mf usr on usr.ID=mf.CreatedBy
inner join emr_patient_mf p on p.ID=mf.PatientId and p.CompanyId=@CompanyID
where mf.CompanyId=@CompanyID and AppointmentDate=cast(GETDATE() as  date) and mf.DoctorId=@Doctorid


--prescription_mf 8
select  top 1 *  from emr_prescription_mf pmf 
where pmf.PatientId=@PatientId and pmf.CompanyID=@CompanyID and pmf.AppointmentDate=CAST(@Date as date)
order by AppointmentDate desc
---prescription_treatment 9
select t.*  from emr_prescription_mf pmf 
inner join emr_prescription_treatment t on t.PrescriptionId=pmf.Id and t.CompanyID=@CompanyID
where pmf.PatientId=@PatientId and pmf.CompanyID=@CompanyID and pmf.AppointmentDate=CAST(@Date as date)

---prescription_complaint 10
select c.*  from emr_prescription_mf pmf 
inner join emr_prescription_complaint c on c.PrescriptionId=pmf.Id and c.CompanyID=@CompanyID
where pmf.PatientId=@PatientId and pmf.CompanyID=@CompanyID and pmf.AppointmentDate=CAST(@Date as date)

---prescription_diagnos 11
select d.*  from emr_prescription_mf pmf 
inner join emr_prescription_diagnos d on d.PrescriptionId=pmf.Id and d.CompanyID=@CompanyID
where pmf.PatientId=@PatientId and pmf.CompanyID=@CompanyID and pmf.AppointmentDate=CAST(@Date as date)
---prescription_investigation 12
select inv.*  from emr_prescription_mf pmf 
inner join emr_prescription_investigation inv on inv.PrescriptionId=pmf.Id and inv.CompanyID=@CompanyID
where pmf.PatientId=@PatientId and pmf.CompanyID=@CompanyID and pmf.AppointmentDate=CAST(@Date as date)
---prescription_observation 13
select ob.*  from emr_prescription_mf pmf 
inner join emr_prescription_observation ob on ob.PrescriptionId=pmf.Id and ob.CompanyID=@CompanyID
where pmf.PatientId=@PatientId and pmf.CompanyID=@CompanyID and pmf.AppointmentDate=CAST(@Date as date)

-----medicine List 14
select Id,Medicine from emr_medicine where CompanyID=@CompanyID
---UnitList 15
select * from sys_drop_down_value where DropDownID=14 and (CompanyID=@CompanyID or CompanyID is null)

---MadicineTypeList 16
select * from sys_drop_down_value where DropDownID=15 and (CompanyID=@CompanyID or CompanyID is null)

---DoesList 17
select * from sys_drop_down_value where DropDownID=16 and (CompanyID=@CompanyID or CompanyID is null)
----Gender List 18
select * from sys_drop_down_value where DropDownID=2 and (CompanyID=@CompanyID or CompanyID is null)

----blood List 19
select * from sys_drop_down_value where DropDownID=17 and (CompanyID=@CompanyID or CompanyID is null)

----doctor list 20
exec SP_GetDoctorList @CompanyId,@UserId
---IsShowDoctorIds 21
set @IsShowDoctorId=(select IsShowDoctor from adm_user_mf where id=@UserId);
select @IsShowDoctorId IsShowDoctorIds

--DoctorCalander 22
select mf.Name,mf.ID,mf.StartTime,mf.EndTime,mf.SlotTime,mf.OffDay,mf.IsShowDoctor,0 IsDoctor,mf.Qualification,mf.Designation,mf.PhoneNo 
from adm_user_company ucomp
inner join adm_user_mf mf on mf.ID=ucomp.UserID
inner join adm_role_mf rolemf on rolemf.ID=ucomp.RoleID 
where ucomp.CompanyID=@CompanyId and rolemf.RoleName='Doctor'and mf.ID not in (select * from ParseCommaStringToInt(@IsShowDoctorId))
order by mf.ID
GO
/****** Object:  StoredProcedure [dbo].[SP_GetPrescriptionMaxIds]    Script Date: 9/6/2024 5:45:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--exec SP_GetPrescriptionMaxIds 1,2
CREATE procedure [dbo].[SP_GetPrescriptionMaxIds]
@CompanyID Numeric(18),
@PatientId numeric(18,0)
as
---emr_appointment_mf top 1
select top 1 * from emr_appointment_mf where CompanyId=@CompanyID and PatientId=@PatientId

---emr_prescription_mf id
select isnull(MAX(Id),0)+1 as ID  from emr_prescription_mf
---emr_prescription_complaint id
select isnull(MAX(Id),0)+1 as complaintID  from emr_prescription_complaint
---emr_prescription_diagnos id
select isnull(MAX(Id),0)+1 as diagnosID  from emr_prescription_diagnos
---emr_prescription_investigation id
select isnull(MAX(Id),0)+1 as investigationID  from emr_prescription_investigation
---emr_prescription_observation id
select isnull(MAX(Id),0)+1 as observationID  from emr_prescription_observation

---emr_prescription_treatment id
select isnull(MAX(Id),0)+1 as treatmentID  from emr_prescription_treatment

---emr_prescription_treatment_template id
select isnull(MAX(Id),0)+1 as TemplateID  from emr_prescription_treatment_template

---emr_appointment_mf id
select isnull(MAX(Id),0)+1 as AppID  from emr_appointment_mf
GO
/****** Object:  StoredProcedure [dbo].[SP_IncomeAndExpense]    Script Date: 9/6/2024 5:45:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--exec SP_IncomeAndExpense'2024-08-31',1
CREATE PROCEDURE [dbo].[SP_IncomeAndExpense]
@FromeDate DATE,
@CompanyId NUMERIC(18,0)
AS 
BEGIN
    DECLARE @CurrentMonth INT;
    SET @CurrentMonth = MONTH(GETDATE());
    SET @FromeDate = DATEADD(YEAR, DATEDIFF(YEAR, 0, @FromeDate), 0);
    ;WITH mcte AS (
        SELECT @FromeDate AS MONTH_NAME
        UNION ALL
        SELECT DATEADD(MONTH, 1, MONTH_NAME)
        FROM mcte
        WHERE DATEPART(MONTH, MONTH_NAME) < @CurrentMonth
    )
    SELECT 
        DATENAME(MONTH, d.MONTH_NAME) AS MONTH_NAME,
        ISNULL(SUM(ex.Amount), 0) AS ExAmount,
		 ISNULL(SUM(incom.ReceivedAmount + ISNULL(bill.PaidAmount, 0)), 0) AS Income
		
    FROM mcte d  
    LEFT OUTER JOIN (
        SELECT 
            YEAR(Date) AS Year, 
            MONTH(Date) AS Month, 
            SUM(Amount) AS Amount
        FROM emr_expense
        WHERE CompanyId = @CompanyId
        GROUP BY YEAR(Date), MONTH(Date)
    ) ex 
        ON YEAR(d.MONTH_NAME) = ex.Year 
        AND MONTH(d.MONTH_NAME) = ex.Month 
    LEFT OUTER JOIN (
        SELECT 
            YEAR(Date) AS Year, 
            MONTH(Date) AS Month, 
            SUM(ReceivedAmount) AS ReceivedAmount
        FROM emr_income
        WHERE CompanyId = @CompanyId
        GROUP BY YEAR(Date), MONTH(Date)
    ) incom 
        ON YEAR(d.MONTH_NAME) = incom.Year 
        AND MONTH(d.MONTH_NAME) = incom.Month 
    LEFT OUTER JOIN (
        SELECT 
            YEAR(BillDate) AS Year, 
            MONTH(BillDate) AS Month, 
            SUM(PaidAmount) AS PaidAmount
        FROM emr_patient_bill
        WHERE CompanyId = @CompanyId
        GROUP BY YEAR(BillDate), MONTH(BillDate)
    ) bill 
        ON YEAR(d.MONTH_NAME) = bill.Year 
        AND MONTH(d.MONTH_NAME) = bill.Month 
    GROUP BY d.MONTH_NAME
    ORDER BY d.MONTH_NAME
END



GO
/****** Object:  StoredProcedure [dbo].[SP_IncomeAndExpense1]    Script Date: 9/6/2024 5:45:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_IncomeAndExpense1]
@FromeDate DATE,
@CompanyId NUMERIC(18,0)
AS 
BEGIN
    DECLARE @CurrentMonth INT;
    SET @CurrentMonth = MONTH(GETDATE());

    -- Setting FromDate to the start of the year of the provided date
    SET @FromeDate = DATEADD(YEAR, DATEDIFF(YEAR, 0, @FromeDate), 0);

    -- CTE to generate all months from FromDate to the current month
    ;WITH mcte AS (
        SELECT @FromeDate AS MONTH_NAME
        UNION ALL
        SELECT DATEADD(MONTH, 1, MONTH_NAME)
        FROM mcte
        WHERE DATEPART(MONTH, MONTH_NAME) < @CurrentMonth
    )
    SELECT 
        DATENAME(MONTH, d.MONTH_NAME) AS MONTH_NAME,
        ISNULL(SUM(ex.Amount), 0) AS ExAmount,
        ISNULL(SUM(incom.ReceivedAmount), 0) AS Income,
        ISNULL(SUM(bill.PaidAmount), 0) AS PaidAmount,
		 ISNULL(SUM(incom.ReceivedAmount + ISNULL(bill.PaidAmount, 0)), 0) AS Income
    FROM mcte d  
    LEFT OUTER JOIN (
        SELECT 
            YEAR(Date) AS Year, 
            MONTH(Date) AS Month, 
            SUM(Amount) AS Amount
        FROM emr_expense
        WHERE CompanyId = @CompanyId
        GROUP BY YEAR(Date), MONTH(Date)
    ) ex 
        ON YEAR(d.MONTH_NAME) = ex.Year 
        AND MONTH(d.MONTH_NAME) = ex.Month 
    LEFT OUTER JOIN (
        SELECT 
            YEAR(Date) AS Year, 
            MONTH(Date) AS Month, 
            SUM(ReceivedAmount) AS ReceivedAmount
        FROM emr_income
        WHERE CompanyId = @CompanyId
        GROUP BY YEAR(Date), MONTH(Date)
    ) incom 
        ON YEAR(d.MONTH_NAME) = incom.Year 
        AND MONTH(d.MONTH_NAME) = incom.Month 
    LEFT OUTER JOIN (
        SELECT 
            YEAR(BillDate) AS Year, 
            MONTH(BillDate) AS Month, 
            SUM(PaidAmount) AS PaidAmount
        FROM emr_patient_bill
        WHERE CompanyId = @CompanyId
        GROUP BY YEAR(BillDate), MONTH(BillDate)
    ) bill 
        ON YEAR(d.MONTH_NAME) = bill.Year 
        AND MONTH(d.MONTH_NAME) = bill.Month 
    GROUP BY d.MONTH_NAME
    ORDER BY d.MONTH_NAME
END

--exec SP_IncomeAndExpense1'2024-08-31',1

--select * from emr_expense where MONTH(Date)=8,900
--select * from emr_income where MONTH(Date)=8,200,3700
--select * from emr_patient_bill where MONTH(BillDate)=8 3600,1800
GO
/****** Object:  StoredProcedure [dbo].[SP_InsertContact]    Script Date: 9/6/2024 5:45:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[SP_InsertContact]
	@Name nvarchar(max),
	@Phone nvarchar(max),
	@Email nvarchar(max),
	@Speciality nvarchar(max),
	@Message nvarchar(max)
AS
BEGIN
declare @Maxid as int;
set @Maxid=(select isnull(MAX(id),0)+1  from contact)

insert into dbo.contact(
Id,
	Name,
	Email,
	Phone,
	Speciality,
	Message,
	CreatedDate
	)
	values (
	@Maxid,
	@Name,
	@Email,
	@Phone,
	@Speciality,
	@Message,
	GETDATE()
	)

END
GO
/****** Object:  StoredProcedure [dbo].[SP_InsertPrescription]    Script Date: 9/6/2024 5:45:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[SP_InsertPrescription]
	@json nvarchar(max),
	@AppmfData nvarchar(max)
AS
BEGIN
declare @IsTemplate [bit];
declare @IsCreateAppointment [bit];
set @IsTemplate=(SELECT * from
OPENJSON ( @json)  
WITH (   
	[IsTemplate] [bit] '$.IsTemplate'
	)
	)
set @IsCreateAppointment=(SELECT * from
OPENJSON ( @json)  
WITH (   
	[IsCreateAppointment] [bit]'$.IsCreateAppointment'
	)
	)
	
Begin Transaction


insert into emr_prescription_mf(ID,CompanyID,IsTemplate,AppointmentDate,PatientId,DoctorId,ClinicId,FollowUpDate,IsCreateAppointment,Notes
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Day)
select ID,CompanyID,IsTemplate,AppointmentDate,PatientId,DoctorId,ClinicId,FollowUpDate,IsCreateAppointment,Notes
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Day
FROM  
 OPENJSON ( @json)  
WITH (   
	[ID] [numeric](18, 0)'$.ID',
	[CompanyID] [numeric](18, 0)'$.CompanyID',
	[IsTemplate] [bit] '$.IsTemplate',
	[AppointmentDate] [date] '$.AppointmentDate',
	[PatientId] [numeric](18, 0)'$.PatientId',
	[DoctorId] [numeric](18, 0) '$.DoctorId',
	[ClinicId] [decimal](18, 0) '$.ClinicId',
	[FollowUpDate] [date] '$.FollowUpDate',
	[FollowUpTime] [time](7) '$.FollowUpTime',
	[IsCreateAppointment] [bit]'$.IsCreateAppointment',
	[Notes] [nvarchar](500) '$.Notes',
	[CreatedBy] [numeric](18, 0)'$.CreatedBy',
	[CreatedDate] [datetime] '$.CreatedDate',
	[ModifiedBy] [numeric](18, 0) '$.ModifiedBy',
	[ModifiedDate] [datetime]'$.ModifiedDate',
		[Day] [int]'$.Day'
)
If		@@ERROR != 0
Begin
			Rollback
			Return
End

insert into emr_prescription_complaint(ID,CompanyID,PrescriptionId,ComplaintId,Complaint,PatientId
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select ID,CompanyID,PrescriptionId,ComplaintId,Complaint,PatientId
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
FROM  
 OPENJSON ( @json, '$.emr_prescription_complaint')  
WITH (   
	[ID] [numeric](18, 0) '$.ID',
	[CompanyID] [numeric](18, 0)'$.CompanyID',
	[PrescriptionId] [numeric](18, 0)'$.PrescriptionId',
	[ComplaintId] [numeric](18, 0)'$.ComplaintId',
	[Complaint] [nvarchar](250) '$.Complaint',
	[PatientId] [numeric](18, 0)'$.PatientId',
	[CreatedBy] [numeric](18, 0)'$.CreatedBy',
	[CreatedDate] [datetime]'$.CreatedDate',
	[ModifiedBy] [numeric](18, 0)'$.ModifiedBy',
	[ModifiedDate] [datetime] '$.ModifiedDate'
 )
 If			@@ERROR != 0
Begin
			Rollback
			Return
End
insert into emr_prescription_diagnos(ID,CompanyID,PrescriptionId,DiagnosId,Diagnos,PatientId
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select ID,CompanyID,PrescriptionId,DiagnosId,Diagnos,PatientId
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
 FROM  
 OPENJSON ( @json, '$.emr_prescription_diagnos')  
WITH (   
	[ID] [numeric](18, 0) '$.ID',
	[CompanyID] [numeric](18, 0)'$.CompanyID',
	[PrescriptionId] [numeric](18, 0)'$.PrescriptionId',
	[DiagnosId] [numeric](18, 0)'$.DiagnosId',
	[Diagnos] [nvarchar](250) '$.Diagnos',
	[PatientId] [numeric](18, 0)'$.PatientId',
	[CreatedBy] [numeric](18, 0)'$.CreatedBy',
	[CreatedDate] [datetime]'$.CreatedDate',
	[ModifiedBy] [numeric](18, 0)'$.ModifiedBy',
	[ModifiedDate] [datetime] '$.ModifiedDate'	
 )
 If			@@ERROR != 0
Begin
			Rollback
			Return
End
insert into emr_prescription_investigation(ID,CompanyID,PrescriptionId,InvestigationId,Investigation,PatientId
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select ID,CompanyID,PrescriptionId,InvestigationId,Investigation,PatientId
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
 FROM   
 OPENJSON ( @json, '$.emr_prescription_investigation')  
WITH (   
	[ID] [numeric](18, 0) '$.ID',
	[CompanyID] [numeric](18, 0)'$.CompanyID',
	[PrescriptionId] [numeric](18, 0)'$.PrescriptionId',
	[InvestigationId] [numeric](18, 0)'$.InvestigationId',
	[Investigation] [nvarchar](250) '$.Investigation',
	[PatientId] [numeric](18, 0)'$.PatientId',
	[CreatedBy] [numeric](18, 0)'$.CreatedBy',
	[CreatedDate] [datetime]'$.CreatedDate',
	[ModifiedBy] [numeric](18, 0)'$.ModifiedBy',
	[ModifiedDate] [datetime] '$.ModifiedDate'	
 )
 If			@@ERROR != 0
Begin
			Rollback
			Return
End
insert into emr_prescription_observation(ID,CompanyID,PrescriptionId,ObservationId,Observation,PatientId
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select ID,CompanyID,PrescriptionId,ObservationId,Observation,PatientId
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
FROM  
 OPENJSON ( @json, '$.emr_prescription_observation')  
WITH (   
	[ID] [numeric](18, 0) '$.ID',
	[CompanyID] [numeric](18, 0)'$.CompanyID',
	[PrescriptionId] [numeric](18, 0)'$.PrescriptionId',
	[ObservationId] [numeric](18, 0)'$.ObservationId',
	[Observation] [nvarchar](250) '$.Observation',
	[PatientId] [numeric](18, 0)'$.PatientId',
	[CreatedBy] [numeric](18, 0)'$.CreatedBy',
	[CreatedDate] [datetime]'$.CreatedDate',
	[ModifiedBy] [numeric](18, 0)'$.ModifiedBy',
	[ModifiedDate] [datetime] '$.ModifiedDate'	
 )
 If			@@ERROR != 0
Begin
			Rollback
			Return
End
insert into emr_prescription_treatment(ID,CompanyID,PrescriptionId,MedicineName,MedicineId,Duration,PatientId,Measure,IsMorning,
IsEvening,IsSOS,IsNoon,Instructions,InstructionId
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select ID,CompanyID,PrescriptionId,MedicineName,MedicineId,Duration,PatientId,Measure,IsMorning,
IsEvening,IsSOS,IsNoon,Instructions,InstructionId
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
FROM  
 OPENJSON ( @json, '$.emr_prescription_treatment')  
WITH (   
	[ID] [numeric](18, 0) '$.ID',
	[CompanyID] [numeric](18, 0) '$.CompanyID',
	[PrescriptionId] [numeric](18, 0)'$.PrescriptionId',
	[MedicineName] [nvarchar](250) '$.MedicineName',
	[MedicineId] [numeric](18, 0) '$.MedicineId',
	[Duration] [int] '$.Duration',
	[PatientId] [numeric](18, 0)'$.PatientId',
	[Measure] [nvarchar](50)'$.Measure',
	[IsMorning] [bit] '$.IsMorning',
	[IsEvening] [bit] '$.IsEvening',
	[IsSOS] [bit]'$.IsSOS',
	[IsNoon] [bit]'$.IsNoon',
	[Instructions] [nvarchar](250) '$.Instructions',
	[InstructionId] [numeric](18, 0) '$.InstructionId',
	[CreatedBy] [numeric](18, 0) '$.CreatedBy',
	[CreatedDate] [datetime]'$.CreatedDate',
	[ModifiedBy] [numeric](18, 0) '$.ModifiedBy',
	[ModifiedDate] [datetime] '$.ModifiedDate'
 )
 If			@@ERROR != 0
Begin
			Rollback
			Return
End
if(@IsTemplate=1)
begin
	insert into emr_prescription_treatment_template(ID,CompanyID,TemplateName,PrescriptionId
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select ID,CompanyID,TemplateName,PrescriptionId
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
FROM  
 OPENJSON ( @json, '$.emr_prescription_treatment_template')  
WITH (   
	[ID] [numeric](18, 0) '$.ID',
	[CompanyID] [numeric](18, 0) '$.CompanyID',
	[TemplateName] [nvarchar](100) '$.TemplateName',
	[PrescriptionId] [numeric](18, 0)'$.PrescriptionId',
	[CreatedBy] [numeric](18, 0)'$.CreatedBy',
	[CreatedDate] [datetime] '$.CreatedDate',
	[ModifiedBy] [numeric](18, 0) '$.ModifiedBy',
	[ModifiedDate] [datetime] '$.ModifiedDate'
 )
end
If			@@ERROR != 0
Begin
			Rollback
			Return
End
if(@IsCreateAppointment=1)
begin
insert into emr_appointment_mf(ID,CompanyID,PatientId,PatientProblem,DoctorId,AppointmentDate,AppointmentTime,Notes,StatusId
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,TokenNo)
select ID,CompanyID,PatientId,PatientProblem,DoctorId,AppointmentDate,AppointmentTime,Notes,StatusId
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,TokenNo
FROM  
 OPENJSON (@AppmfData)  
WITH (   
	[ID] [numeric](18, 0)'$.ID',
	[CompanyId] [numeric](18, 0)'$.CompanyId',
	[PatientId] [numeric](18, 0) '$.PatientId',
	[PatientProblem] [nvarchar](500)'$.PatientProblem',
	[DoctorId] [numeric](18, 0) '$.DoctorId',
	[AppointmentDate] [date]'$.AppointmentDate',
	[AppointmentTime] [time](7)'$.AppointmentTime',
	[Notes] [nvarchar](500) '$.Notes',
	[StatusId] [int]'$.StatusId',
	[CreatedBy] [numeric](18, 0)'$.CreatedBy',
	[CreatedDate] [datetime] '$.CreatedDate',
	[ModifiedBy] [numeric](18, 0) '$.ModifiedBy',
	[ModifiedDate] [datetime] '$.ModifiedDate',
	[TokenNo] [int] '$.TokenNo'

	
 )
end


If			@@ERROR != 0
Begin
			Rollback
			Return
End

Commit

END
GO
/****** Object:  StoredProcedure [dbo].[SP_IPDBillSummary]    Script Date: 9/6/2024 5:45:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--exec SP_IPDBillSummary 1 ,3
CREATE procedure [dbo].[SP_IPDBillSummary]
@CompanyId numeric(18,0),
@AdmissionId numeric(18,0)

as
begin
select bill.ID,u.Name as DoctorName,bill.BillDate as Date,isnull(chr.AnnualPE,0)AnnualPE,isnull(chr.General,0)General,isnull(chr.Medical,0)Medical,
isnull(chr.ICUCharges,0)ICUCharges,
isnull(chr.ExamRoom,0)ExamRoom,isnull(chr.PrivateWard,0)PrivateWard,isnull(chr.RIP,0)RIP,isnull(chr.OtherAllCharges,0)OtherAllCharges,isnull((chr.AnnualPE+chr.General+chr.Medical+chr.ICUCharges+
chr.ExamRoom+chr.PrivateWard+chr.RIP+chr.OtherAllCharges),0)as total
from emr_patient_bill bill
left join ipd_admission_charges chr on chr.AdmissionId=bill.AdmissionId
inner join emr_appointment_mf app on app.ID=chr.AppointmentId
inner join adm_user_mf u on u.ID=app.DoctorId

where bill.AdmissionId=@AdmissionId and bill.CompanyId=@CompanyId
end


GO
/****** Object:  StoredProcedure [dbo].[SP_LoadDropdown]    Script Date: 9/6/2024 5:45:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--exec SP_LoadDropdown 1,1
CREATE procedure [dbo].[SP_LoadDropdown]
@CompanyID Numeric(18),
@UserId numeric(18,0)
as
declare @IsShowDoctorId as varchar(200);

----Gender List 0
select * from sys_drop_down_value where DropDownID=2 and (CompanyID=@CompanyID or CompanyID is null)
----doctor list 1
exec SP_GetDoctorList @CompanyId,@UserId
-----DoctorIDS 2
set @IsShowDoctorId=(select IsShowDoctor from adm_user_mf where id=@UserId);
select @IsShowDoctorId IsShowDoctorIds
-----DoctorCalander 3
select mf.Name,mf.ID,mf.StartTime,mf.EndTime,mf.SlotTime,mf.OffDay,mf.IsShowDoctor,0 IsDoctor,mf.Qualification,mf.Designation,mf.PhoneNo
from adm_user_company ucomp
inner join adm_user_mf mf on mf.ID=ucomp.UserID
inner join adm_role_mf rolemf on rolemf.ID=ucomp.RoleID 
where ucomp.CompanyID=@CompanyId and rolemf.RoleName='Doctor'and mf.ID not in (select * from ParseCommaStringToInt(@IsShowDoctorId))
order by mf.ID


-----medicine List 4
select Id,Medicine from emr_medicine where CompanyID=@CompanyID
--------------------company info 5
select comp.CompanyID,cp.CompanyName from adm_user_company comp
inner join adm_company cp on cp.ID=comp.CompanyID
where comp.UserID=@UserId

-----------blood list 6
select * from sys_drop_down_value where DropDownID=17 and (CompanyID=@CompanyID or CompanyID is null)
-------service type 7
select ID,ServiceName,Price  from emr_bill_type where CompanyId=@CompanyID and ServiceName='Consultation'

---------token no 8
select isnull(MAX(TokenNo),0)+1 as TokenNo from emr_appointment_mf where CompanyId=@CompanyID and AppointmentDate=cast(GETDATE() as  date)
--BackDated entry 9
select IsBackDatedAppointment from adm_company where ID=@CompanyID
---All status list 10
select ID,Value from sys_drop_down_value where DropDownID=1 and (CompanyID=@CompanyID or CompanyID is null)
---bill type 11
select ID,Value from sys_drop_down_value where DropDownID=22 and (CompanyID=@CompanyID or CompanyID is null)

---Tittle list 12
select ID,Value from sys_drop_down_value where DropDownID=23 and (CompanyID=@CompanyID or CompanyID is null)

-----AdmissionNo 13
select isnull(MAX(ID),0)+1 as AdmissionNo from ipd_admission where CompanyId=@CompanyID and CAST(AdmissionDate as  date)=cast(GETDATE() as  date)
 
-----MRNO 14
select isnull(MAX(ID),0)+1 as MRNO from emr_patient_mf where CompanyId=@CompanyID

---Admission type list 15
select ID,Value from sys_drop_down_value where DropDownID=25 and (CompanyID=@CompanyID or CompanyID is null)


---Visit Type list 16
select ID,Value from sys_drop_down_value where DropDownID=30 and (CompanyID=@CompanyID or CompanyID is null)

---Status Type list 17
select ID,Value from sys_drop_down_value where DropDownID=31 and (CompanyID=@CompanyID or CompanyID is null)

---Result Type list 18
select ID,Value from sys_drop_down_value where DropDownID=32 and (CompanyID=@CompanyID or CompanyID is null)
---Imaging Type List 19
select ID,Value from sys_drop_down_value where DropDownID=27 and (CompanyID=@CompanyID or CompanyID is null)
---Lab Type List 20
select ID,Value from sys_drop_down_value where DropDownID=28 and (CompanyID=@CompanyID or CompanyID is null)

---Ward List 21
select ID,Value from sys_drop_down_value where DropDownID=33 and (CompanyID=@CompanyID or CompanyID is null)

---Room List 22
select ID,Value,DependedDropDownValueID,DependedDropDownID from sys_drop_down_value where DropDownID=34 and (CompanyID=@CompanyID or CompanyID is null)
---Bed List 23
select ID,Value,DependedDropDownValueID,DependedDropDownID from sys_drop_down_value where DropDownID=35 and (CompanyID=@CompanyID or CompanyID is null)

GO
/****** Object:  StoredProcedure [dbo].[SP_OnDischargeUpdateBillAmount]    Script Date: 9/6/2024 5:45:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--exec SP_OnDischargeUpdateBillAmount 1,1,1
CREATE procedure [dbo].[SP_OnDischargeUpdateBillAmount]
@CompanyId numeric(18,0),
@AdmissionId numeric(18,0),
@PatientId numeric(18,0)
as
begin
update emr_patient_bill set Price=(select isnull(sum(isnull((AnnualPE+General+Medical+ICUCharges+
ExamRoom+PrivateWard+RIP+OtherAllCharges),0)),0)as total from ipd_admission_charges where AdmissionId=@AdmissionId and PatientId=@PatientId and CompanyId=@CompanyId)
where AdmissionId=@AdmissionId and PatientId=@PatientId and CompanyId=@CompanyId
end

GO
/****** Object:  StoredProcedure [dbo].[SP_PR_CalculateSalary]    Script Date: 9/6/2024 5:45:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--  select FirstName+' '+LastName,ID,CompanyID,PayScheduleID,CreatedBy from pr_employee_mf where CompanyID=13
-- [SP_PR_CalculateSalary]   2973,3367,'71111',4833
-- 71107,71108,71109,71110,71111,71113,71168,71169,71170,71171,71172,71173,72170
CREATE PROCEDURE [dbo].[SP_PR_CalculateSalary]     --[SP_PR_CalculateSalary]   13,1940,'68',21
@CompanyID     NUMERIC(18, 0),
@PayScheduleID NUMERIC(18, 0),  
@EmployeeIds   Varchar(Max),
@LoginID       NUMERIC(9)


AS
BEGIN
CREATE table #Emp2
(
    EMPID varchar(10) 
)
CREATE table #Emp3
(
    EMPID varchar(10) 
)
CREATE table #WKDAYS
(
    WKDays varchar(10) 
)
BEGIN Transaction

DECLARE
@EmployeeID       NUMERIC(18, 0),
@PeriodStartDate  Date,
@PeriodENDDate    Date,
@PayDate          Date,
@Days             NUMERIC(9,2),
@LeaveTypeID      NUMERIC(9),
@PayrollDtlID     NUMERIC(9),
@PayrollID        NUMERIC(9),
@LDID             NUMERIC(9),
@AllowDedID       NUMERIC(9),
@Type             Varchar(20),
@Value            NUMERIC(24,2),
@SETtingIdOrName  Varchar(20),
@SETtingIdOrNameValue       Varchar(Max),
@SETtingIdOrNameAllowValue  Varchar(500),
@SETtingIdOrNameDepValue    Varchar(500),
@IsActiveManual as bit,
@SalaryMethodID as int

SELECT @PayrollID		= IsNull(Max(ID),0) FROM pr_employee_payroll_mf
SELECT @PayrollDtlID	= IsNull(Max(ID),0) FROM pr_employee_payroll_dt

 SELECT	ID INTO	#Emp 
 FROM FNC_Split(@EmployeeIds,',')

  INSERT INTO #Emp2 SELECT ID FROM #Emp	
  INSERT INTO #Emp3 SELECT ID FROM #Emp	

  SET @SalaryMethodID=(SELECT SalaryMethodID FROM adm_company WHERE ID=@CompanyID)

	
  	SELECT		@PayDate = PayDate,	@Days = DATEDIFF(day, PeriodStartDate,PeriodENDDate) + 1,
				@PeriodStartDate = PeriodStartDate, @PeriodENDDate = PeriodENDDate
	FROM		pr_pay_schedule
	WHERE		CompanyID = @CompanyID and ID = @PayScheduleID


	SELECT		Cast(0 as NUMERIC(9)) PayrollID,m.CompanyID,ID EMPID,EmployeeCode,IsNull(JoiningDate,@PeriodStartDate) JoiningDate,IsNull(EffectiveDate,@PeriodStartDate) EffectiveDate,
				case when @PeriodStartDate > IsNull(EffectiveDate,@PeriodStartDate) then @PeriodStartDate else IsNull(EffectiveDate,@PeriodStartDate) END FROMDate,@PeriodENDDate ToDate,
				Cast(DATEDIFF(day, case when @PeriodStartDate > IsNull(EffectiveDate,@PeriodStartDate) then @PeriodStartDate else IsNull(EffectiveDate,@PeriodStartDate) END ,@PeriodENDDate) 
				+ 1 as NUMERIC(24,2)) NoOfDays,
				m.TerminatedDate,m.FinalSETtlementDate,Case When m.FinalSETtlementDate between @PeriodStartDate and @PeriodENDDate then 1 else 0 END IsFinalSETtlement,IsNull(M.BasicSalary,0) BasicSalary,M.IsActive
	INTO		#EmpMaster
	FROM		pr_employee_mf m
	WHERE		m.CompanyID = @CompanyID and IsNull(m.ExculdeFROMPayroll,0) = 0 and PayScheduleID = @PayScheduleID
				and m.StatusDropDownID = 40 and m.StatusID = 1
				and	Convert(char(8),IsNull(EffectiveDate,@PeriodStartDate),112) <= Convert(char(8),@PeriodENDDate,112)
				and	Exists (SELECT 1 FROM #Emp d WHERE d.ID = m.ID)
				and (	TerminatedDate is null 
						Or (
								TerminatedDate > @PeriodStartDate and 
								Not TerminatedDate between @PeriodStartDate and @PeriodENDDate
							) 
						Or FinalSETtlementDate between @PeriodStartDate and @PeriodENDDate
					)

	If @@ERROR != 0
	BEGIN
		Rollback
		Return
	END
	

	If Exists (SELECT 1 FROM #EmpMaster WHERE IsFinalSETtlement = 1)
	BEGIN

				SELECT		EM.CompanyID,EM.EMPID EmployeeID,DateDiff(Day,IsNull(Max(PM.PayScheduleToDate),IsNull(EM.EffectiveDate,@PeriodStartDate)),EM.FinalSETtlementDate) + 1 NoOfDays,
							IsNull(Max(PM.PayScheduleToDate),IsNull(EM.EffectiveDate,@PeriodStartDate)) FROMDate,EM.FinalSETtlementDate TODate
				INTO		#FSEmployee
				FROM		#EmpMaster EM 
				Left join	pr_employee_payroll_mf PM On PM.CompanyID = EM.CompanyID
							and PM.EmployeeID = EM.EMPID WHERE EM.IsFinalSETtlement = 1
				Group By	EM.CompanyID,EM.EMPID,IsNull(EM.EffectiveDate,@PeriodStartDate),EM.FinalSETtlementDate
	
				Update		#EmpMaster SET NoOfDays = d.NoOfDays , FROMDate = d.FROMDate, ToDate = d.ToDate
				FROM		#FSEmployee d
				WHERE		D.CompanyID = #EmpMaster.CompanyID
							and D.EmployeeID = #EmpMaster.EMPID

				DELETE FROM #EmpMaster WHERE NoOfDays < 0
	END
	
	-- Get Employee details FROM Action form
SELECT  CompanyID,EmployeeID as AEmpID,OldBasicSalary,BasicSalary as bs,BasicSalary as TempNewBasicSalary,ActionEffectiveFROM,PaidEffectiveDate as 'NewBasicSalaryEffectiveFROM' INTO #TempActionForm 
FROM    pr_employee_Action_mf 
WHERE   CompanyID=@CompanyID and (cast(PaidEffectiveDate as date) >= @PeriodStartDate and @PeriodENDDate<=cast(PaidEffectiveDate as date)
OR cast(PaidEffectiveDate as date) between @PeriodStartDate and @PeriodENDDate
)and UseAsDefault=1 and InfoTypeID<>2 and EmployeeID In (SELECT ID FROM #Emp)

update #EmpMaster SET #EmpMaster.BasicSalary=#TempActionForm.OldBasicSalary 
                  FROM #TempActionForm  
				  WHERE #EmpMaster.EMPID=#TempActionForm.AEmpID

--Calculate Arrear of Current month




SELECT   CompanyID,EmployeeID,OldBasicSalary, BasicSalary as 'NewBasicSalary',
CASE WHEN  (cast(PaidEffectiveDate as date) >= @PeriodStartDate and @PeriodENDDate <=cast(PaidEffectiveDate as date))

THEN (BasicSalary-OldBasicSalary) 
ELSE 0 END as ArrearsAmount,
'AR' as Type,
ActionEffectiveFROM,PaidEffectiveDate,0 as IsPaid,@LoginID as CreatedBy,GETDATE() as CreatedDate  INTO     #TempArrearAmount
FROM    pr_employee_Action_mf 
WHERE   CompanyID=@CompanyID and (cast(PaidEffectiveDate as date) >= @PeriodStartDate and @PeriodENDDate <=cast(PaidEffectiveDate as date) OR 
cast(PaidEffectiveDate as date) between @PeriodStartDate and @PeriodENDDate )
and UseAsDefault=1 and InfoTypeID<>2 and EmployeeID  In (SELECT ID FROM #Emp)



--SELECT   CompanyID,EmployeeID,OldBasicSalary, BasicSalary as 'NewBasicSalary',(BasicSalary-OldBasicSalary) as ArrearsAmount,'AR' as Type,ActionEffectiveFROM,PaidEffectiveDate,0 as IsPaid,@LoginID as CreatedBy,GETDATE() as CreatedDate 
--INTO     #TempArrearAmount
--FROM    pr_employee_Action_mf 
--WHERE   CompanyID=@CompanyID and (cast(PaidEffectiveDate as date) >= @PeriodStartDate and @PeriodENDDate<=cast(PaidEffectiveDate as date)
--OR cast(PaidEffectiveDate as date) between @PeriodStartDate and @PeriodENDDate
--)and UseAsDefault=1 and InfoTypeID<>2 and EmployeeID In (SELECT ID FROM #Emp)




-- Add arrear in a table of current month
    DECLARE @tempPayrollID as NUMERIC(18,0)
    SET     @tempPayrollID = @PayrollID+1

INSERT INTO pr_employee_arrears_mf (CompanyID,EmployeeID,OldBasicSalary,NewBasicSalary,ArrearsAmount,Type,ActionEffectiveDate,PaidEffectiveDate,IsPaid,
                                    CreatedBy,CreatedDate,ArrearReleatedMonth,PayrollID)
SELECT                              CompanyID,EmployeeID,OldBasicSalary,NewBasicSalary,ArrearsAmount,Type,ActionEffectiveFROM,PaidEffectiveDate,IsPaid,
                                    CreatedBy,CreatedDate,@PeriodStartDate as ArrearReleatedMonth,@tempPayrollID
                             FROM   #TempArrearAmount 

SELECT  * INTO #WithinMonthTemp  
          FROM #TempActionForm 
          WHERE AEmpID  in (SELECT EmployeeID FROM #TempArrearAmount)





IF(@SalaryMethodID=1 or @SalaryMethodID is null)
BEGIN
 DECLARE @cnt11 INT = 0;
 DECLARE @tillCount11 as int;
 SET     @tillCount11=(SELECT Count(AEmpID) FROM #WithinMonthTemp)
 WHILE   @cnt11 < @tillCount11
 BEGIN
 DECLARE  @IsEmployeeExist as NUMERIC(18,0)
 SET      @IsEmployeeExist=(SELECT Top(1) AEmpID FROM #WithinMonthTemp T inner join #Emp3 E on E.EMPID=t.AEmpID where NewBasicSalaryEffectiveFROM between @PeriodStartDate and @PeriodENDDate )

IF (@IsEmployeeExist is not null)
BEGIN
 DECLARE @PaidEffectiveDate as date = (SELECT NewBasicSalaryEffectiveFROM FROM #TempActionForm WHERE AEmpID=@IsEmployeeExist)
 DECLARE @FirstDays as INT          = (SELECT DATEDIFF(DAY, @PeriodStartDate,@PaidEffectiveDate))
 DECLARE @LastDays  as INT          = (SELECT DATEDIFF(DAY,@PaidEffectiveDate ,@PeriodENDDate)+1)


UPDATE #TempActionForm SET OldBasicSalary = (OldBasicSalary/@Days)*@FirstDays ,bs=(bs/@Days)*@LastDays 
                       WHERE  AEmpID =(SELECT Top(1) AEmpID  FROM #WithinMonthTemp)

UPDATE #EmpMaster      SET #EmpMaster.BasicSalary=case when #TempActionForm.OldBasicSalary>0 then #TempActionForm.OldBasicSalary else #TempActionForm.bs END
                       FROM #TempActionForm  
					   WHERE #EmpMaster.EMPID=(SELECT Top(1) AEmpID  FROM #WithinMonthTemp)

 SELECT   CompanyID,AEmpID,OldBasicSalary, bs as 'NewBSArrearsAmount', TempNewBasicSalary,'AR' as Type,ActionEffectiveFROM,NewBasicSalaryEffectiveFROM,0 as IsPaid,@LoginID as CreatedBy,GETDATE() as CreatedDate 
 INTO     #WithInMonthArrearAmount 
 FROM #TempActionForm 
 WHERE  AEmpID = (SELECT Top(1) AEmpID  FROM #WithinMonthTemp) 
 --select * from #TempArrearAmount
--Issue

SET @tempPayrollID=(SELECT Max(ID) FROM pr_employee_payroll_mf)
-- Add arrear in a table of current month
INSERT INTO pr_employee_arrears_mf 
            (CompanyID,EmployeeID,OldBasicSalary,NewBasicSalary,ArrearsAmount,Type,ActionEffectiveDate,PaidEffectiveDate,
			IsPaid,CreatedBy,CreatedDate,ArrearReleatedMonth,PayrollID)

SELECT CompanyID,AEmpID,OldBasicSalary,TempNewBasicSalary,NewBSArrearsAmount,Type,ActionEffectiveFROM,NewBasicSalaryEffectiveFROM,IsPaid,
       CreatedBy,CreatedDate,@PeriodStartDate as ArrearReleatedMonth,@tempPayrollID+1
FROM   #WithInMonthArrearAmount  where @FirstDays<>0

IF OBJECT_ID('tempdb..#WithInMonthArrearAmount') IS NOT NULL
	BEGIN
			drop table #WithInMonthArrearAmount
	END

END
--ELSE IF(@IsEmployeeExist is null)
--BEGIN
--Update		#EmpMaster SET BasicSalary =  BasicSalary / @Days * NoOfDays 
--                       WHERE EMPID in (SELECT EMPID FROM #Emp3) 					  
--END

   SET @cnt11 = @cnt11 + 1;
   DELETE FROM #Emp3 WHERE EMPID=@IsEmployeeExist
   DELETE TOP(1) FROM #WithinMonthTemp   
END;

Update		#EmpMaster SET BasicSalary =  BasicSalary / @Days * NoOfDays 
                       WHERE EMPID in (SELECT EMPID FROM #Emp3) 

END


-- [SP_PR_CalculateSalary]   529,555,'69238,68705',1103
if(@SalaryMethodID=2 )
BEGIN
select 'Method 2'
SET @Days=(SELECT
   (DATEDIFF(dd, @PeriodStartDate, @PeriodENDDate) + 1)
  -(DATEDIFF(wk, @PeriodStartDate, @PeriodENDDate) * 2)
  -(CASE WHEN DATENAME(dw, @PeriodStartDate) = 'Sunday'   THEN 1 ELSE 0 END)
  -(CASE WHEN DATENAME(dw, @PeriodENDDate)   = 'Saturday' THEN 1 ELSE 0 END))

   SELECT * INTO  #EmpMaster1 FROM #EmpMaster

   --UPDATE       t1 SET t1.NoOfDays=t2.TotWorkingDays FROM #EmpMaster AS t1 INNER JOIN 
   --         (SELECT * FROM fn_GetTotalWorkingDaysAccordingToShift (@CompanyID,@EmployeeIds,@PeriodStartDate,@PeriodENDDate)) AS t2
   --          ON t1.EMPID = t2.Employeeid


 DECLARE @cnt INT = 0;
 DECLARE @tillCount as int;
 SET @tillCount=(SELECT Count(EMPID) FROM #Emp2)
 WHILE @cnt < @tillCount
 BEGIN
    DECLARE @EMPID NUMERIC(18,0) = (SELECT TOP(1) EMPID FROM #Emp2)
    DECLARE @TotWorkingDays INT= 0;
	SET     @TotWorkingDays = (SELECT TotWorkingDays FROM fn_GetTotalWorkingDaysAccordingToShift(@CompanyID,@EMPID,@PeriodStartDate,@PeriodENDDate))

    DECLARE @IsEmployeeExist1 as NUMERIC(18,0)
    SET @IsEmployeeExist1=(SELECT  AEmpID  FROM #WithinMonthTemp WHERE AEmpID in (SELECT Top 1 EMPID FROM #Emp2) and NewBasicSalaryEffectiveFROM between @PeriodStartDate and @PeriodENDDate  )
IF(@IsEmployeeExist1 is not null)
BEGIN
IF OBJECT_ID('tempdb..#WithInMonthArrearAmount1') IS NOT NULL
	BEGIN
			drop table #WithInMonthArrearAmount1
	END
	
DECLARE @PaidEffectiveDate1 as date=(SELECT NewBasicSalaryEffectiveFROM FROM #TempActionForm WHERE AEmpID=@IsEmployeeExist1 )
DECLARE @FirstDays1 as int =0;
DECLARE @LastDays1 as int  =0;
DECLARE @FirstDateFROM DATETIME
DECLARE @FirstDateTo DATETIME
DECLARE @LastDateFROM DATETIME
DECLARE @LastDateTo DATETIME

SET @FirstDateFROM = DATEADD(day,-1,@PeriodStartDate)--@PeriodStartDate
SET @FirstDateTo =DATEADD(day,-1,@PaidEffectiveDate1)--@PaidEffectiveDate1

SET @LastDateFROM =DATEADD(day,-1,@PaidEffectiveDate1) --@PaidEffectiveDate1
SET @LastDateTo =DATEADD(day,-1,@PeriodENDDate) --@PeriodENDDate

SET @FirstDays1 = (SELECT TotWorkingDays FROM fn_GetTotalWorkingDaysAccordingToShift(@CompanyID,@IsEmployeeExist1,@FirstDateFROM,@FirstDateTo))

SET @LastDays1 =  (SELECT TotWorkingDays+1 FROM fn_GetTotalWorkingDaysAccordingToShift (@CompanyID,@IsEmployeeExist1,@LastDateFROM,@LastDateTo))

SELECT TotWorkingDays FROM fn_GetTotalWorkingDaysAccordingToShift(@CompanyID,@IsEmployeeExist1,@FirstDateFROM,@FirstDateTo)
SELECT TotWorkingDays+1 FROM fn_GetTotalWorkingDaysAccordingToShift(@CompanyID,@IsEmployeeExist1,@LastDateFROM,@LastDateTo)
update  #TempActionForm  SET OldBasicSalary=cast( (CASE WHEN  OldBasicSalary>0 THEN OldBasicSalary ELSE 0 END)/(CASE WHEN @TotWorkingDays=0 THEN 1 ELSE @TotWorkingDays END) as NUMERIC(18,2))*@FirstDays1,
         bs=cast( (case WHEN  bs>0 THEN bs ELSE 0 END)/(CASE WHEN @TotWorkingDays=0 THEN 1 ELSE @TotWorkingDays END)*@LastDays1 as NUMERIC(18,2))
         WHERE  AEmpID =@IsEmployeeExist1

--update  #TempActionForm  SET OldBasicSalary=cast( (CASE WHEN  OldBasicSalary>0 THEN OldBasicSalary ELSE 0 END)/(CASE WHEN @TotWorkingDays=0 THEN 1 ELSE @TotWorkingDays END)*@FirstDays1 as NUMERIC(18,2)),
--         bs=cast( (case WHEN  bs>0 THEN bs ELSE 0 END)/(CASE WHEN @TotWorkingDays=0 THEN 1 ELSE @TotWorkingDays END)*@LastDays1 as NUMERIC(18,2))
--         WHERE  AEmpID =@IsEmployeeExist1


update  #EmpMaster       SET #EmpMaster.BasicSalary= CASE WHEN isnull(#TempActionForm.OldBasicSalary,0) >0 THEN isnull(#TempActionForm.OldBasicSalary,0) ELSE isnull(#TempActionForm.bs,0) END
                         FROM #TempActionForm  WHERE #EmpMaster.EMPID=@IsEmployeeExist1

SELECT    CompanyID,AEmpID,OldBasicSalary, bs as 'NewBSArrearsAmount', TempNewBasicSalary,'AR' as Type,ActionEffectiveFROM,NewBasicSalaryEffectiveFROM,0 as IsPaid,
          @LoginID as CreatedBy,GETDATE() as CreatedDate 
INTO      #WithInMonthArrearAmount1 
FROM      #TempActionForm
WHERE     AEmpID =@IsEmployeeExist1

SET @tempPayrollID=(SELECT Max(ID) FROM pr_employee_payroll_mf)
-- Add arrear in a table of current month

INSERT INTO pr_employee_arrears_mf (CompanyID,EmployeeID,OldBasicSalary,NewBasicSalary,ArrearsAmount,Type,ActionEffectiveDate,PaidEffectiveDate,
            IsPaid,CreatedBy,CreatedDate,ArrearReleatedMonth,PayrollID)
SELECT CompanyID,AEmpID,OldBasicSalary,TempNewBasicSalary,NewBSArrearsAmount,Type,ActionEffectiveFROM,NewBasicSalaryEffectiveFROM,
       IsPaid,CreatedBy,CreatedDate,@PeriodStartDate as ArrearReleatedMonth,@tempPayrollID+1
FROM #WithInMonthArrearAmount1 WHERE  AEmpID =@IsEmployeeExist1 and NewBasicSalaryEffectiveFROM<>@PeriodStartDate --and OldBasicSalary>0

END
else if(@IsEmployeeExist is null)
BEGIN
DECLARE @HireFROMDATE DATE SET @HireFROMDATE=(select FROMDate from #EmpMaster WHERE EMPID In (SELECT Top 1 EMPID FROM #Emp2) )
DECLARE @HireTODATE   DATE SET @HireTODATE  =(select ToDate from #EmpMaster WHERE EMPID In (SELECT Top 1 EMPID FROM #Emp2) )

 UPDATE  #EmpMaster SET  NoOfDays= 
           (SELECT TotWorkingDays FROM fn_GetTotalWorkingDaysAccordingToShift(@CompanyID,@EMPID,@HireFROMDATE,@HireTODATE))
            WHERE #EmpMaster.EMPID In (SELECT Top 1 EMPID FROM #Emp2) 

 Update	#EmpMaster SET BasicSalary = cast( case when  BasicSalary>0 then BasicSalary/(case when @TotWorkingDays=0 then 1 else @TotWorkingDays END  )*NoOfDays  else 0 END as NUMERIC(18,2)) WHERE #EmpMaster.EMPID In (SELECT Top 1 EMPID FROM #Emp2) 
END

   SET @cnt = @cnt + 1;
   DELETE Top(1) FROM #Emp2
END;

END;


	SELECT		Cast(0 as NUMERIC(9)) PayrollDtlID,Cast(0 as NUMERIC(9)) PayrollID,
				M.CompanyID,M.EMPID,A.PayScheduleID,Cast('A' as varchar(20)) Type,A.AllowanceID AllowDedID,
				Cast(A.Amount / @Days * M.NoOfDays as NUMERIC(24,2)) Amount,IsNull(A.Taxable,0) Taxable,Cast(0 as NUMERIC(9)) RefID, Cast('' as varchar(50)) Remarks,M.IsActive,Cast(NULL as datetime) as ArrearReleatedMonth
	INTO		#Salary
	FROM		pr_employee_allowance A
	Inner Join	#EmpMaster M on M.EMPID = A.EmployeeID and A.CompanyID = M.CompanyID and A.PayScheduleID = @PayScheduleID
	Inner Join	pr_allowance PA on PA.ID = A.AllowanceID and PA.CompanyID = A.CompanyID

	DECLARE @ExpenseAllowance  NUMERIC(18,0)
	SET     @ExpenseAllowance=(SELECT ID FROM pr_allowance WHERE CompanyID=@CompanyID and SystemGenerated=1 and AllowanceName='Expense')
	
SELECT		    Cast(0 as NUMERIC(9)) PayrollDtlID,Cast(0 as NUMERIC(9)) PayrollID,
				M.CompanyID,M.EMPID,@PayScheduleID PayScheduleID,Cast('A' as varchar(20)) Type,isNULL(@ExpenseAllowance,0) AllowDedID,
				Cast(sum(A.TotalAmount) as NUMERIC(24,2)) Amount,IsNull(1,0) Taxable,Cast(0 as NUMERIC(9)) RefID, Cast('Expense' as varchar(50)) Remarks,M.IsActive,Cast(NULL as datetime) as ArrearReleatedMonth
	INTO		#Salary1
	FROM		pr_expense_application A
	inner Join	#EmpMaster M on M.EMPID = A.EmployeeID and A.CompanyID = M.CompanyID and A.ApprovalStatusID='1' 
	and  Cast(A.Date as date) between @PeriodStartDate and @PeriodENDDate
	inner Join	sys_drop_down_value PA on PA.ID = A.ExpenSETypeID and pa.DropDownID=30
	group by M.CompanyID,M.EMPID,PA.ID,M.IsActive

	
	DECLARE @ArrearsAllowance  NUMERIC(18,0)
	SET     @ArrearsAllowance=(SELECT ID FROM pr_allowance WHERE CompanyID=@CompanyID and SystemGenerated=1 and AllowanceName='Arrears')
	
	SELECT		Cast(0 as NUMERIC(9)) PayrollDtlID,Cast(0 as NUMERIC(9)) PayrollID,
				M.CompanyID,M.EMPID,@PayScheduleID PayScheduleID,Cast('A' as varchar(20)) Type,isnull(cast(@ArrearsAllowance as int),0) AllowDedID,
				Cast(Sum(AR.ArrearsAmount) as NUMERIC(18,0)) Amount,IsNull(1,0) Taxable,Cast(0 as NUMERIC(9)) RefID, Cast('Arrears' as varchar(50)) Remarks,M.IsActive,
			     AR.ArrearReleatedMonth as ArrearReleatedMonth
	INTO		#Salary2
	FROM		pr_employee_arrears_mf AR
	inner Join	#EmpMaster M on M.EMPID = AR.EmployeeID and AR.CompanyID = M.CompanyID 
	and 	   convert(varchar(7), AR.PaidEffectiveDate, 126)= convert(varchar(7), @PeriodENDDate, 126)
	
	group by M.CompanyID,M.EMPID,M.IsActive,AR.ID,AR.ArrearReleatedMonth
	 
	 

	If @@ERROR != 0
	BEGIN
		Rollback
		Return
	END

	INSERT INTO #Salary  SELECT * FROM #Salary1
    INSERT INTO #Salary (PayrollDtlID,PayrollID,CompanyID,EMPID,PayScheduleID,Type,AllowDedID,Amount,Taxable,RefID,Remarks,IsActive,ArrearReleatedMonth) 
	SELECT      PayrollDtlID,PayrollID,CompanyID,EMPID,PayScheduleID,Type,AllowDedID,Amount,Taxable,RefID,Remarks,IsActive,ArrearReleatedMonth FROM #Salary2

	INSERT INTO #Salary (PayrollDtlID,PayrollID,CompanyID,EMPID,PayScheduleID,Type,AllowDedID,Amount,Taxable,RefID,Remarks,IsActive,ArrearReleatedMonth)
	SELECT		0 PayrollDtlID,0 PayrollID,M.CompanyID,EMPID,D.PayScheduleID,D.Category Type,D.DeductionContributionID  AllowDedID,
				(case when M.IsActive=1 then (D.Amount / @Days * M.NoOfDays) else d.Amount END) Amount, IsNull(D.Taxable,0), 0,'',M.IsActive,cast(NULL as datetime) as ArrearReleatedMonth
	FROM		pr_employee_ded_contribution D
	Inner Join	#EmpMaster M on M.EMPID = D.EmployeeID and D.CompanyID = M.CompanyID and D.PayScheduleID = @PayScheduleID
	Inner Join	pr_deduction_contribution PDC on PDC.ID = D.DeductionContributionID and PDC.CompanyID = D.CompanyID
		
	If @@ERROR != 0
	BEGIN
		Rollback
		Return
	END

	DECLARE		Formula_cursor CURSOR FOR 
	SELECT		Distinct d.AllowDedID,d.Type,f.SETtingIdOrName,f.SETtingIdOrNameValue,
				IsNull(f.SETtingIdOrNameDepAllowValue,0) SETtingIdOrNameDepAllowValue,
				IsNull(f.SETtingIdOrNameDepDedValue,0) SETtingIdOrNameDepDedValue
	FROM		#Salary d 
	inner join	pr_allowance a on a.CompanyID = d.CompanyID and a.ID = d.AllowDedID and d.Type = 'A' and a.Formula = 1
	inner join	sys_SETting f on f.SETtingIdOrName = a.AllowanceName
	WHERE		a.Formula = 1
	Union All
	SELECT		Distinct d.AllowDedID,d.Type,f.SETtingIdOrName,f.SETtingIdOrNameValue,
				IsNull(f.SETtingIdOrNameDepAllowValue,0) SETtingIdOrNameDepAllowValue,
				IsNull(f.SETtingIdOrNameDepDedValue,0) SETtingIdOrNameDepDedValue
	FROM		#Salary d 
	inner join	pr_deduction_contribution a on a.CompanyID = d.CompanyID and a.ID = d.AllowDedID 
				and d.Type = a.Category and d.Type = 'D' and a.Formula = 1
	inner join	sys_SETting f on f.SETtingIdOrName = a.DeductionContributionName
	WHERE		a.Formula = 1
	
	OPEN Formula_cursor  
	FETCH NEXT FROM Formula_cursor INTO @AllowDedID,@Type,@SETtingIdOrName,@SETtingIdOrNameValue,@SETtingIdOrNameAllowValue,@SETtingIdOrNameDepValue

	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		 
			DECLARE @tblAmount TABLE(Amount FLOAT,BonusTax FLOAT,LASTONETIMEBONUSAMOUNT FLOAT);
			DECLARE @Formula NVARCHAR(MAX), @SQLParam NVARCHAR(100), @SQLQuery NVARCHAR(MAX),@IsAllowanceType as varchar(1);
			
			SET	 @Formula = IsNull(@SETtingIdOrNameValue,'')
			SET	 @SQLParam = REPLACE(REPLACE(SUBSTRING(@Formula, 0, CHARINDEX('BEGIN', LOWER(@Formula))), '<', ''), '>', '');
			SET	 @SQLQuery = SUBSTRING(@Formula, CHARINDEX('BEGIN', LOWER(@Formula)), LEN(@Formula));
			SET	 @IsAllowanceType = Case When @SETtingIdOrNameAllowValue != '' then 'A' when @SETtingIdOrNameDepValue != '' then 'D' else 'A' END

		if @IsActiveManual=1
			Update	#Salary 
			SET		Amount = d.Amount 
			FROM		( 
						 SELECT CompanyID,EMPID,IsNull(Sum(Amount),0) Amount 
						 FROM	(
								SELECT		d.CompanyID,d.EMPID,IsNull(Sum(d.Amount),0) Amount 
								FROM		#Salary D 
								WHERE 		d.Type = @IsAllowanceType
								group by	d.CompanyID,d.EMPID
								Union All
								SELECT		CompanyID,EMPID ,BasicSalary
								FROM		#EmpMaster d WHERE @IsAllowanceType = 'A'
								) R
						Group By CompanyID,EMPID
					) d 
			WHERE	d.CompanyID = #Salary.CompanyID and d.EMPID = #Salary.EMPID 
					and #Salary.AllowDedID = @AllowDedID and #Salary.Type = @Type

		DECLARE	AllowDed_cursor CURSOR FOR
			SELECT  EMPID,Amount,IsActive
			FROM	#Salary
			WHERE	CompanyID = @CompanyID and AllowDedID = @AllowDedID and Type = @Type

			OPEN AllowDed_cursor  
			FETCH NEXT FROM AllowDed_cursor INTO @EmployeeID,@Value,@IsActiveManual
			
			WHILE @@FETCH_STATUS = 0  
			BEGIN
		
				     DELETE FROM @tblAmount;
					 INSERT INTO @tblAmount
					 EXEC sp_executesql 
						  @SQLQuery, 
						  N'@CompanyID NUMERIC(18, 0),@EmployeeID NUMERIC(18, 0), @PayScheduleID NUMERIC(18, 0),@Type as varchar(1),@AllowDedID as NUMERIC(9), @Value NUMERIC(24,2),@IsSalaryProcess as bit,@IsActiveManual as bit,@ISAddAllowanceOnTime as bit,@OnTimeAllowancesIDs as nvarchar(MAX)', 
						  @CompanyID, 
						  @EmployeeID, 
						  @PayScheduleID,
						  @Type,
						  @AllowDedID,
						  @Value,
						  0,
						  @IsActiveManual,
						  0,
						  null;
						  
					Update	#Salary SET Amount = IsNull((SELECT Amount FROM @tblAmount) ,0)
					WHERE	CompanyID = @CompanyID and EMPID = @EmployeeID and AllowDedID = @AllowDedID and Type = @Type

					FETCH NEXT FROM AllowDed_cursor INTO @EmployeeID,@Value,@IsActiveManual
			END

			CLOSE AllowDed_cursor  
			DEALLOCATE AllowDed_cursor 

		  FETCH NEXT FROM Formula_cursor INTO @AllowDedID,@Type,@SETtingIdOrName,@SETtingIdOrNameValue,@SETtingIdOrNameAllowValue,@SETtingIdOrNameDepValue
	END 

	CLOSE Formula_cursor  
	DEALLOCATE Formula_cursor 


	SELECT		@LDID = ID FROM pr_deduction_contribution 
	WHERE		CompanyID = @CompanyID and DeductionContributionName = 'Loan'

	SELECT		p.ID LoanID,p.CompanyID,p.EmployeeID,Cast(0 as NUMERIC(24,2)) BasicSalary,Cast(0 as NUMERIC(24,2)) SalaryAmount
	INTO		#Loan
	FROM		pr_loan p 
	Inner Join	#Emp e on e.ID = p.EmployeeID and p.CompanyID = @CompanyID
	Inner Join	sys_drop_down_value d on d.ID = p.PaymentMethodID and d.DropDownID = 12 and d.Value='Salary'
	WHERE		P.PaymentStartDate <= @PeriodENDDate

	If IsNull(@LDID,0) > 0 and Exists (SELECT 1 FROM #Loan)
	BEGIN

			Update	#Loan
			SET		BasicSalary = IsNull(M.BasicSalary,0),
					SalaryAmount = IsNull(M.BasicSalary,0) + d.Amount
			FROM	(
						SELECT		D.CompanyID,D.EMPID,Sum(Case when Type = 'A' then D.Amount when Type = 'D' then D.Amount * -1 else 0 END) Amount
						FROM		#Salary D
						Group By	D.CompanyID,D.EMPID
					) d
			Inner Join	#EmpMaster M on M.CompanyID = D.CompanyID And M.EMPID =  D.EMPID
			WHERE	d.CompanyID = #Loan.CompanyID and d.EMPID = #Loan.EmployeeID


			DELETE FROM #Salary WHERE Exists(SELECT 1 FROM #Loan l 
			WHERE l.CompanyID = #Salary.CompanyID and l.EmployeeID = #Salary.EMPID )
			and #Salary.Type = 'D' and #Salary.AllowDedID = @LDID
											
			INSERT INTO #Salary (PayrollDtlID,PayrollID,CompanyID,EMPID,PayScheduleID,Type,AllowDedID,Amount,Taxable,RefID,Remarks,IsActive)
			SELECT					0 PayrollDtlID,0 PayrollID,CompanyID,EmployeeID,@PayScheduleID,'D' Type,@LDID  AllowDedID,
						dbo.ufn_PR_CalculateLoanDeduction(CompanyID,EmployeeID,LoanID,@PeriodENDDate,BasicSalary,SalaryAmount) Amount, 0,LoanID,''Remarks,@IsActiveManual
			FROM		#Loan

			If @@ERROR != 0
			BEGIN
				Rollback
				Return
			END

			DELETE FROM #Salary WHERE Amount=0 and AllowDedID=@LDID and exists( SELECT 1 FROM #Loan  l WHERE l.CompanyID=#Salary.CompanyID and l.EmployeeID=#Salary.EMPID and l.LoanID=#Salary.RefID)

			If @@ERROR != 0
			BEGIN
				Rollback
				Return
			END
	END
		DECLARE @IsHoulyPercentageBased as bit
			SET @IsHoulyPercentageBased=(SELECT IsHoulyPercentageBased FROM pr_time_rule_mf WHERE CompanyID=@CompanyID and ActiveHoursInaDayPolicyID=1)
			if (@IsHoulyPercentageBased=1)
			BEGIN
	INSERT INTO #Salary (PayrollDtlID,PayrollID,CompanyID,EMPID,PayScheduleID,Type,AllowDedID,Amount,Taxable,RefID,Remarks,IsActive)
		SELECT 
		0 PayrollDtlID
		,0 PayrollID
		,ded.ComanyID as CompanyID
		,ded.Employeeid as EMPID
		,@PayScheduleID
		,'D' as Type
		,PDC.ID as AllowDedID
		,ded.DeductInADayAmount as Amount
		,0 Taxable
		,0 RefID
		,'Shortage Hour' as Remarks
		,@IsActiveManual
				 FROM dbo.ufn_PR_CalculateInADayDeduction(@CompanyID,@EmployeeIds,@PeriodStartDate,@PeriodENDDate) ded
				 Inner Join	pr_deduction_contribution PDC on PDC.DeductionContributionName = 'Late & Early Out Deduction' and PDC.CompanyID =  ded.ComanyID
	If @@ERROR != 0
			BEGIN
				Rollback
				Return
			END
			END

				DECLARE @IsHoulyWageBased as bit
			SET @IsHoulyWageBased=(SELECT IsHoulyWageBased FROM pr_time_rule_mf WHERE CompanyID=@CompanyID and ActiveHoursInaDayPolicyID=1)
			if (@IsHoulyWageBased=1)
			BEGIN
	INSERT INTO #Salary (PayrollDtlID,PayrollID,CompanyID,EMPID,PayScheduleID,Type,AllowDedID,Amount,Taxable,RefID,Remarks,IsActive)
		SELECT 
		0 PayrollDtlID
		,0 PayrollID
		,ded.CompanyID as CompanyID
		,ded.Employeeid as EMPID
		,@PayScheduleID
		,'D' as Type
		,PDC.ID as AllowDedID
		,ded.Amount as Amount
		,0 Taxable
		,0 RefID
		,'Shortage Hour' as Remarks
		,@IsActiveManual
				 FROM dbo.ufn_PR_HoursInaDayDeduction(@CompanyID,@EmployeeIds,@PeriodStartDate,@PeriodENDDate) ded
				 Inner Join	pr_deduction_contribution PDC on PDC.DeductionContributionName = 'Late & Early Out Deduction' and PDC.CompanyID =  ded.CompanyID
	If @@ERROR != 0
			BEGIN
				Rollback
				Return
			END
			END

			 DECLARE @LateDeductionWageRateType as BIT
	         SET @LateDeductionWageRateType=(select IsNULL(LateDeductionWageRateType,1) from pr_time_rule_mf where CompanyID=@CompanyID)
			 IF (@LateDeductionWageRateType=1)
			BEGIN
			
			select 'Mission passed'
				INSERT INTO #Salary (PayrollDtlID,PayrollID,CompanyID,EMPID,PayScheduleID,Type,AllowDedID,Amount,Taxable,RefID,Remarks,IsActive)
		SELECT 
		0 PayrollDtlID
		,0 PayrollID
		,ded.CompanyID as CompanyID
		,ded.Employeeid as EMPID
		,@PayScheduleID
		,'D' as Type
		,PDC.ID as AllowDedID
		,ded.Amount as Amount
		,0 Taxable
		,0 RefID
		,'Late Arrival(Wage Based)' as Remarks
		,@IsActiveManual
				 FROM dbo.ufn_PR_LateArivalWageBasedInHoursAndMinutesDeduction(@CompanyID,@EmployeeIds,@PeriodStartDate,@PeriodENDDate) ded
				 Inner Join	pr_deduction_contribution PDC on PDC.DeductionContributionName = 'Late & Early Out Deduction' and PDC.CompanyID =  ded.CompanyID
	
	If @@ERROR != 0
			BEGIN
				Rollback
				Return
			END
			END

			ELSE 
			BEGIN
	INSERT INTO #Salary (PayrollDtlID,PayrollID,CompanyID,EMPID,PayScheduleID,Type,AllowDedID,Amount,Taxable,RefID,Remarks,IsActive)
		SELECT 
		0 PayrollDtlID
		,0 PayrollID
		,ded.ComanyID as CompanyID
		,ded.Employeeid as EMPID
		,@PayScheduleID
		,'D' as Type
		,PDC.ID as AllowDedID
		,ded.DeductLateAmount as Amount
		,0 Taxable
		,0 RefID
		,'Late Arrival' as Remarks
		,@IsActiveManual
				 FROM dbo.ufn_PR_CalculateLateArrivalDeduction(@CompanyID,@EmployeeIds,@PeriodStartDate,@PeriodENDDate) ded
				 Inner Join	pr_deduction_contribution PDC on PDC.DeductionContributionName = 'Late & Early Out Deduction' and PDC.CompanyID =  ded.ComanyID
	
	If @@ERROR != 0
			BEGIN
				Rollback
				Return
			END
		END

				INSERT INTO #Salary (PayrollDtlID,PayrollID,CompanyID,EMPID,PayScheduleID,Type,AllowDedID,Amount,Taxable,RefID,Remarks,IsActive)
		SELECT 
		0 PayrollDtlID
		,0 PayrollID
		,ded.CompanyID as CompanyID
		,ded.Employeeid as EMPID
		,@PayScheduleID
		,'D' as Type
		,PDC.ID as AllowDedID
		,ded.Amount as Amount
		,0 Taxable
		,0 RefID
		,'Early Out(Wage Based)' as Remarks
		,@IsActiveManual
				 FROM dbo.ufn_PR_EarlyOutWageBasedInHoursAndMinutesDeduction(@CompanyID,@EmployeeIds,@PeriodStartDate,@PeriodENDDate) ded
				 Inner Join	pr_deduction_contribution PDC on PDC.DeductionContributionName = 'Late & Early Out Deduction' and PDC.CompanyID =  ded.CompanyID
	
	If @@ERROR != 0
			BEGIN
				Rollback
				Return
			END


	INSERT INTO #Salary (PayrollDtlID,PayrollID,CompanyID,EMPID,PayScheduleID,Type,AllowDedID,Amount,Taxable,RefID,Remarks,IsActive)
		SELECT 
		0 PayrollDtlID
		,0 PayrollID
		,ded.ComanyID as CompanyID
		,ded.Employeeid as EMPID
		,@PayScheduleID
		,'D' as Type
		,PDC.ID as AllowDedID
		,ded.DeductRestrictionAmount as Amount
		,0 Taxable
		,0 RefID
		,'Missing Swipe' as Remarks
		,@IsActiveManual
				 FROM dbo.ufn_PR_CalculateLeaveRestrictionDeduction(@CompanyID,@EmployeeIds,@PeriodStartDate,@PeriodENDDate) ded
				 Inner Join	pr_deduction_contribution PDC on PDC.DeductionContributionName = 'Late & Early Out Deduction' and PDC.CompanyID =  ded.ComanyID
	If @@ERROR != 0
			BEGIN
				Rollback
				Return
			END
	INSERT INTO #Salary (PayrollDtlID,PayrollID,CompanyID,EMPID,PayScheduleID,Type,AllowDedID,Amount,Taxable,RefID,Remarks,IsActive)
		SELECT 
		0 PayrollDtlID
		,0 PayrollID
		,ded.ComanyID as CompanyID
		,ded.Employeeid as EMPID
		,@PayScheduleID
		,'D' as Type
		,PDC.ID as AllowDedID
		,ded.DeductMissingAmount as Amount
		,0 Taxable
		,0 RefID
		,'Missing Attendance' as Remarks
		,@IsActiveManual
				 FROM dbo.ufn_PR_CalculateMissingDeduction(@CompanyID,@EmployeeIds,@PeriodStartDate,@PeriodENDDate) ded
				 Inner Join	pr_deduction_contribution PDC on PDC.DeductionContributionName = 'Late & Early Out Deduction' and PDC.CompanyID =  ded.ComanyID
	If @@ERROR != 0
			BEGIN
				Rollback
				Return
			END
			DECLARE @isOverTimeSELECT as nvarchar(2) 
			SET @isOverTimeSELECT=(SELECT isOverTimeSELECT FROM pr_time_rule_mf WHERE CompanyID=@CompanyID)
			if @isOverTimeSELECT = '2'
	BEGIN
			INSERT INTO #Salary (PayrollDtlID,PayrollID,CompanyID,EMPID,PayScheduleID,Type,AllowDedID,Amount,Taxable,RefID,Remarks,IsActive)
		SELECT 
		0 PayrollDtlID
		,0 PayrollID
		,ded.CompanyID as CompanyID
		,ded.Employeeid as EMPID
		,@PayScheduleID
		,'A' as Type
		,PA.ID as AllowDedID
		,ded.Total as Amount
		,1 Taxable
		,0 RefID
		,'' as Remarks
		,@IsActiveManual
				 FROM dbo.ufn_PR_CalculateOvertimeAllowance(@CompanyID,@EmployeeIds,@PeriodStartDate,@PeriodENDDate) ded
				 Inner Join	pr_allowance PA on PA.AllowanceName = 'Overtime Allowance' and PA.CompanyID = ded.CompanyID

If @@ERROR != 0
			BEGIN
				Rollback
				Return
			END
			END
			print 2

if @isOverTimeSELECT = '3'
	BEGIN
			INSERT INTO #Salary (PayrollDtlID,PayrollID,CompanyID,EMPID,PayScheduleID,Type,AllowDedID,Amount,Taxable,RefID,Remarks,IsActive)
		SELECT 
		0 PayrollDtlID
		,0 PayrollID
		,dedo.CompanyID as CompanyID
		,dedo.Employeeid as EMPID
		,@PayScheduleID
		,'A' as Type
		,PA.ID as AllowDedID
		,dedo.Total as Amount
		,1 Taxable
		,0 RefID
		,'' as Remarks
		,@IsActiveManual
				 FROM dbo.ufn_PR_CalculateServicePeriodOvertimeAllowance(@CompanyID,@EmployeeIds,@PeriodStartDate,@PeriodENDDate) dedo
				 Inner Join	pr_allowance PA on PA.AllowanceName = 'Overtime Allowance' and PA.CompanyID = dedo.CompanyID

If @@ERROR != 0
			BEGIN
				Rollback
				Return
			END
			END
			print 3

		

	DELETE FROM #Salary WHERE Amount=0 and AllowDedID=@LDID and exists( SELECT 1 FROM #Salary  l WHERE l.CompanyID=#Salary.CompanyID and l.EMPID=#Salary.EMPID and l.RefID=#Salary.RefID)

	Update #EmpMaster SET PayrollID		= @PayrollID, @PayrollID += 1
	Update #Salary    SET PayrollDtlID	= @PayrollDtlID, @PayrollDtlID += 1
	Update #Salary    SET PayrollID     = d.PayrollID FROM   #EmpMaster d 
	WHERE				  d.CompanyID = #Salary.CompanyID and d.EMPID = #Salary.EMPID	

INSERT        INTO pr_employee_payroll_mf
			  (ID,CompanyID,PayScheduleID,PayDate,EmployeeID,PayScheduleFROMDate,
			  PayScheduleToDate,FROMDate,ToDate,BasicSalary,Status,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
	SELECT 
				 PayrollID,CompanyID,@PayScheduleID as 'PayScheduleID',@PayDate as 'PayDate',EMPID,@PeriodStartDate as 'PeriodStartDate',
				 @PeriodEndDate as 'PeriodEndDate',FROMDate as FROMDate,ToDate as 'ToDate',BasicSalary,'O' as 'Status',
				 @LoginID as 'CreatedBy',GetDate() as 'CreatedDate',@LoginID as 'CreatedBy',GetDate() as 'CreatedDate'
	FROM		 #EmpMaster
	
	If @@ERROR != 0
	BEGIN
		Rollback
		Return
	END

	
	

If @@ERROR != 0
	BEGIN
		Rollback
		Return
	END


	INSERT INTO pr_employee_payroll_dt
				(ID,PayrollID,CompanyID,PayScheduleID,PayDate,EmployeeID,Type,AllowDedID,Amount,Taxable,RefID,Remarks,ArrearReleatedMonth)
	SELECT
				PayrollDtlID,PayrollID,CompanyID,PayScheduleID,@PayDate,EMPID,Type,AllowDedID,ISNULL(Amount,0) Amount,Taxable,RefID,Remarks,ArrearReleatedMonth as ArrearReleatedMonth
	FROM		#Salary --WHERE ISNULL(Amount,0)!=0
	If @@ERROR != 0
	BEGIN
		Rollback
		Return
	END

	Commit
	--SELECT 1 'Result'
	--[SP_PR_CalculateSalary]   2973,1941,'5142',21
END
GO
/****** Object:  StoredProcedure [dbo].[SP_PrescriptionList]    Script Date: 9/6/2024 5:45:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--exec SP_PrescriptionList 1 ,1,10,'azmat'
CREATE procedure [dbo].[SP_PrescriptionList]
@CompanyID Numeric(18),
@CurrentPageNo as Int ,
@RecordPerPage  as Int ,
@SearchText as Varchar(200)
As
begin
declare
@TotalRecord as int;
select mf.ID,mf.AppointmentDate,patient.PatientName,isnull(vital.PaidAmount,0)PaidAmount,isnull(vital.OutstandingBalance,0)OutstandingBalance,
STUFF(
         (SELECT distinct ', ' + med.Medicine
          FROM  emr_prescription_treatment treat
          inner join emr_medicine med on med.Id=treat.MedicineId and treat.PrescriptionId=mf.Id
		  where treat.CompanyID=@CompanyID
          FOR XML PATH ('')), 1, 1, '') MedicineName
Into #Result
from emr_prescription_mf mf
inner join emr_patient_mf patient on patient.ID=mf.PatientId
left join(
select sum(PaidAmount)PaidAmount,sum(OutstandingBalance)OutstandingBalance,PatientId,CompanyId,DoctorId
from emr_patient_bill bill where bill.CompanyId=@CompanyID
group by PatientId,CompanyId,DoctorId
) vital on vital.PatientId=patient.ID and vital.DoctorId=mf.DoctorId and vital.CompanyId=mf.CompanyID
where mf.CompanyID=@CompanyID

Select * Into #Patient From #Result

If			IsNull(@SearchText,'') != '' and @SearchText!=null
			Delete From #Patient Where PatientName like '%' + @SearchText + '%'

Select		@TotalRecord = Count(1) From #Patient

Select		* Into #PatientList
From		(
				SELECT *,ROW_NUMBER()OVER (Order by ID) RNumber FROM #Patient 
			) E Where E.RNumber Between ((@CurrentPageNo - 1) * @RecordPerPage) + 1 and (@CurrentPageNo * @RecordPerPage)

Select		*,@TotalRecord TotalRecord from #Result Order By  ID desc

end


GO
/****** Object:  StoredProcedure [dbo].[SP_ProvisionalBillRpt]    Script Date: 9/6/2024 5:45:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--exec SP_ProvisionalBillRpt 1,4,165,5

CREATE procedure [dbo].[SP_ProvisionalBillRpt]
@CompanyId numeric(18,0),
@AdmissionId numeric(18,0),
@AppointmentId numeric(18,0),
@PatientId numeric(18,0)
as
begin
select comp.CompanyName,comp.CompanyAddress1,comp.Phone,comp.Email,comp.CompanyLogo,mf.Father_Husband,
patient.ID,admis.AdmissionDate,mf.MRNO,mf.PatientName,admis.DischargeDate,mf.Address,mf.Mobile,u.Name
,val.Value as Room,val1.Value as Ward
from emr_patient_bill patient
inner join adm_company comp on comp.ID=patient.CompanyId
inner join emr_patient_mf mf on mf.ID=patient.PatientId and mf.CompanyId=@CompanyId
inner join emr_bill_type type on type.ID=patient.ServiceId and type.CompanyId=@CompanyId
inner join ipd_admission admis on admis.ID=patient.AdmissionId and admis.CompanyId=@CompanyId
left join sys_drop_down_value val on val.ID=admis.RoomId and val.DropDownID=admis.RoomDropdownId
left join sys_drop_down_value val1 on val.ID=admis.WardTypeId and val.DropDownID=admis.WardTypeDropdownId
inner join adm_user_mf u on u.ID=patient.DoctorId
where patient.AdmissionId=@AdmissionId and patient.AppointmentId=@AppointmentId and patient.PatientId=@PatientId


select u.Name, count(adm.id)as TotalVisit,
sum(charg.AnnualPE
+charg.ExamRoom
+charg.General
+charg.ICUCharges
+charg.OtherAllCharges
+charg.PrivateWard
+charg.RIP
+charg.Medical) as Amount
from ipd_admission adm
inner join emr_appointment_mf app on app.AdmissionId=adm.ID and app.CompanyId=@CompanyId
inner join ipd_admission_charges charg on charg.AppointmentId=app.ID and charg.CompanyId=@CompanyId and charg.AdmissionId=adm.ID
inner join adm_user_mf u on u.ID=app.DoctorId
where adm.ID=@AdmissionId and adm.PatientId=@PatientId and app.IsAdmit=0
group by app.DoctorId,u.Name


select charg.AnnualPE,charg.ExamRoom,charg.General,charg.ICUCharges,charg.OtherAllCharges,charg.PrivateWard,charg.RIP,charg.Medical
from ipd_admission adm
inner join emr_appointment_mf app on app.AdmissionId=adm.ID and app.CompanyId=@CompanyId
inner join ipd_admission_charges charg on charg.AppointmentId=app.ID and charg.CompanyId=@CompanyId and charg.AdmissionId=adm.ID
where adm.ID=@AdmissionId and adm.PatientId=@PatientId and app.IsAdmit=1

-----Total amount

select sum(charg.AnnualPE
+charg.ExamRoom
+charg.General
+charg.ICUCharges
+charg.OtherAllCharges
+charg.PrivateWard
+charg.RIP
+charg.Medical) as Total
from ipd_admission adm
inner join emr_appointment_mf app on app.AdmissionId=adm.ID and app.CompanyId=@CompanyId
inner join ipd_admission_charges charg on charg.AppointmentId=app.ID and charg.CompanyId=@CompanyId and charg.AdmissionId=adm.ID
inner join adm_user_mf u on u.ID=app.DoctorId
where adm.ID=@AdmissionId and adm.PatientId=@PatientId
--------advance amount
select sum(payment.Amount)AdvanceAmount from emr_patient_bill bill
inner join emr_patient_bill_payment payment on payment.BillId=bill.ID
where bill.AdmissionId=@AdmissionId and bill.PatientId=@PatientId 


end






GO
