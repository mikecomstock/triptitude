--truncate table usertrips
--select * from usertrips

insert into usertrips (user_id, trip_id, iscreator, [status], StatusUpdatedOnUTC)
select [User_Id], Id trip_id, 1 iscreator, 1 [status], GETUTCDATE() StatusUpdatedOnUTC from trips