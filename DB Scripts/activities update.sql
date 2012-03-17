update a set a.IsTransportation = 1, a.TransportationType_Id = ta.TransportationType_Id
from Activities a
join TransportationActivities ta on a.Id = ta.Id