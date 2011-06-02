insert into Users (Email,HashedPassword) values ('mikecomstock@gmail.com','$2a$10$OF9caiAHmaafsoNtvgfYi./ps5tT7k4BWlLMGoEce1lOn8lMsPTpO')

insert into TransportationTypes (Name) values ('Taxi')
insert into TransportationTypes (Name) values ('Bus')
insert into TransportationTypes (Name) values ('Drive')
insert into TransportationTypes (Name) values ('Fly')
insert into TransportationTypes (Name) values ('Walk')
insert into TransportationTypes (Name) values ('Bike')
insert into TransportationTypes (Name) values ('Boat')
insert into TransportationTypes (Name) values ('Train')

insert into Tags (Name) values ('Surfing')

/*
insert into Trips(Name, Created_By, Created_On, ShowInSiteMap) values ('Test Trip 1', (select id from users where email = 'mikecomstock@gmail.com'), SYSDATETIME(), 0)

insert into Activities(Trip_Id,BeginDay,EndDay) values (1,1,1)
insert into TransportationActivities(Id, TransportationType_Id, FromCity_GeoNameId, ToCity_GeoNameId) values (SCOPE_IDENTITY(), (select id from TransportationTypes where Name = 'Fly'), 5053511, 4831830) 

insert into Activities(Trip_Id,BeginDay,EndDay) values (1,2,3)
insert into HotelActivities(Id, Hotel_Id) values (SCOPE_IDENTITY(), 74503)

insert into Activities(Trip_Id,BeginDay,EndDay) values (1,3,3)
insert into WebsiteActivities(Id, URL, Title) values (SCOPE_IDENTITY(), 'http://www.triptitude.com', 'Triptitude.com')

insert into Activities(Trip_Id,BeginDay,EndDay) values (1,4,4)
insert into TagActivities(Id, Tag_Id, City_GeoNameId) values (SCOPE_IDENTITY(), (select id from Tags where Name = 'Surfing'), 5053511)
*/