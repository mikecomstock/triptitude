with r_activities
as (select *, ROW_NUMBER() over (partition by trip_id order by BeginDay, BeginTime, beginat, endat) as row_num from activities where Deleted = 0)
update r_activities
set ordernumber = row_num