insert into Users (Email) values ('mikecomstock@gmail.com')

insert into TransportationTypes (Name) values ('Taxi')
insert into TransportationTypes (Name) values ('Bus')
insert into TransportationTypes (Name) values ('Drive')
insert into TransportationTypes (Name) values ('Fly')
insert into TransportationTypes (Name) values ('Walk')
insert into TransportationTypes (Name) values ('Bike')
insert into TransportationTypes (Name) values ('Boat')

insert into Tags (Name) values ('Transportation')
insert into Tags (Name) values ('Lodging')
insert into Tags (Name) values ('Website')
insert into Tags (Name) values ('Surfing')


/*
insert into Activities(Trip_Id,Tag_Id,City_GeoNameId,BeginDay,EndDay) values (1,(select id from Tags where Name = 'Transportation'),5053511,1,1)
insert into Activities(Trip_Id,Tag_Id,City_GeoNameId,BeginDay,EndDay) values (1,(select id from Tags where Name = 'Surfing'),5053511,1,1)
insert into Activities(Trip_Id,Tag_Id,City_GeoNameId,BeginDay,EndDay) values (1,(select id from Tags where Name = 'Lodging'),4831830,2,2)
insert into Activities(Trip_Id,Tag_Id,City_GeoNameId,BeginDay,EndDay) values (1,(select id from Tags where Name = 'Website'),5253498,3,4)
*/