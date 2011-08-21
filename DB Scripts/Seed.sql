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
insert into Tags (Name) values ('Fishing')

insert into Items (Name, URL) values ('Surf Board','http://www.surf-board.com')
insert into Items (Name, URL) values ('Beach Towel','http://www.beach-towel.com')
insert into Items (Name, URL) values ('Fishing Pole','http://www.fishing-pole.com')

insert into ItemTags values ((select id from Tags where name = 'Surfing'), (select id from Items where name = 'Surf Board'))
insert into ItemTags values ((select id from Tags where name = 'Surfing'), (select id from Items where name = 'Beach Towel'))
insert into ItemTags values ((select id from Tags where name = 'Fishing'), (select id from Items where name = 'Fishing Pole'))

/*
insert into Trips(Name, Created_By, Created_On, ShowInSiteMap) values ('Test Trip 1', (select id from users where email = 'mikecomstock@gmail.com'), SYSDATETIME(), 0)

insert into Activities(Trip_Id,BeginDay,EndDay) values (1,1,1)
insert into TransportationActivities(Id, TransportationType_Id, FromCity_GeoNameId, ToCity_GeoNameId) values (SCOPE_IDENTITY(), (select id from TransportationTypes where Name = 'Fly'), 5053511, 4831830) 

insert into Activities(Trip_Id,BeginDay,EndDay) values (1,2,3)
insert into HotelActivities(Id, Hotel_Id) values (SCOPE_IDENTITY(), 74503)
*/