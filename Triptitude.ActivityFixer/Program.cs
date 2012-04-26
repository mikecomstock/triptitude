using System;
using System.Collections.Generic;
using System.Linq;
using Triptitude.Biz;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.ActivityFixer
{
    class Program
    {
        static void Main(string[] args)
        {
            TripsRepo repo = new TripsRepo();
            ActivitiesRepo aRepo = new ActivitiesRepo();

            var trips = repo.FindAll().ToList().OrderBy(t => t.Id);

            foreach (Trip trip in trips)
            {
                Console.WriteLine(string.Format("Trip ID: {0}. Begin Date: {1} ", trip.Id, trip.BeginDate));
                var activities = trip.Activities;
                foreach (Activity activity in activities)
                {
                    if(!activity.BeginDay.HasValue) continue;

                    var beginAt = trip.BeginDate.Value.AddDays(activity.BeginDay.Value - 1);

                    if (activity.BeginTime.HasValue)
                        beginAt = beginAt.Add(activity.BeginTime.Value);

                    activity.BeginAt = beginAt;
                    Console.WriteLine(string.Format("Activity ID: {0}. Begin Day: {2}. Begin Time: {3}. BeginAt: {4}. OrderNumber: {6}",
                        activity.Id, null, activity.BeginDay, activity.BeginTime, activity.BeginAt, null, activity.OrderNumber
                        ));
                }
            }

            repo.Save();


            //{
                //var trips = repo.FindAll().ToList();
                //trips = trips.Where(t => t.Created_On.Date == DateTime.Parse("12/6/2011")
                //                         || t.Created_On.Date == DateTime.Parse("12/15/2011")
                //                         || t.Created_On.Date == DateTime.Parse("1/3/2012")
                //                         || t.Created_On.Date == DateTime.Parse("2/11/2012")).ToList();

                //Console.WriteLine(string.Join(",", trips.Select(t => t.Id.ToString())));

                //var selectMany = trips.SelectMany(t => t.Users).ToList();
                //var userTrips = trips.SelectMany(t => t.UserTrips).ToList();
                //var utRepo = new Repo<UserTrip>();
                //userTrips.ForEach(utRepo.Delete);
                //utRepo.Save();

                //var users = trips.SelectMany(t => t.Users).Distinct().ToList();
                //users.ForEach(u => u.DefaultTrip = null);
                //var uRepo = new Repo<User>();
                //users.ForEach(uRepo.Delete);
                //uRepo.Save();

                //var activities = trips.SelectMany(t => t.Activities).ToList();
                //activities.ForEach(aRepo.Delete);
                //aRepo.Save();

                //trips.ForEach(repo.Delete);
                //repo.Save();

//                select * from trips 

//select * from users

//delete from Histories
//where user_id in (
//select id from Users where DefaultTrip_Id in (
//26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99,100,101,102,103,104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,127,128,129,130,131,132,133,134,135,136,137,138,139,140,141,142,143,144,145,146,147,148,149,150,151,152,153,154,155,156,157,158,159,160,161,162,166,167,168,169,170,171,172,173,174,175,176,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,195,196,197,198,199,200,201,202,203,204,205,206,207,208,209,210,211,212,213,214,215,216,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,233,234,235,236,237,242,243,244,245,246,247,248,249,250,251,252,253,254,255,256,257,258,259,260,261,262,263,264,265,266,267,268,269,270,271,272,273,274,275,276,277,278,279,280,281,282,283,284,285,286,287,288,289,290,291,292,293,294,295,296,297,298,299,300,301,302,303,304,305,306
//))

//delete from Users where DefaultTrip_Id in (
//26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99,100,101,102,103,104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,127,128,129,130,131,132,133,134,135,136,137,138,139,140,141,142,143,144,145,146,147,148,149,150,151,152,153,154,155,156,157,158,159,160,161,162,166,167,168,169,170,171,172,173,174,175,176,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,195,196,197,198,199,200,201,202,203,204,205,206,207,208,209,210,211,212,213,214,215,216,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,233,234,235,236,237,242,243,244,245,246,247,248,249,250,251,252,253,254,255,256,257,258,259,260,261,262,263,264,265,266,267,268,269,270,271,272,273,274,275,276,277,278,279,280,281,282,283,284,285,286,287,288,289,290,291,292,293,294,295,296,297,298,299,300,301,302,303,304,305,306
//)



//select * from Trips where Id in (
//26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99,100,101,102,103,104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,127,128,129,130,131,132,133,134,135,136,137,138,139,140,141,142,143,144,145,146,147,148,149,150,151,152,153,154,155,156,157,158,159,160,161,162,166,167,168,169,170,171,172,173,174,175,176,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,195,196,197,198,199,200,201,202,203,204,205,206,207,208,209,210,211,212,213,214,215,216,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,233,234,235,236,237,242,243,244,245,246,247,248,249,250,251,252,253,254,255,256,257,258,259,260,261,262,263,264,265,266,267,268,269,270,271,272,273,274,275,276,277,278,279,280,281,282,283,284,285,286,287,288,289,290,291,292,293,294,295,296,297,298,299,300,301,302,303,304,305,306
//)

//delete from Trips where Id in (
//26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99,100,101,102,103,104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,127,128,129,130,131,132,133,134,135,136,137,138,139,140,141,142,143,144,145,146,147,148,149,150,151,152,153,154,155,156,157,158,159,160,161,162,166,167,168,169,170,171,172,173,174,175,176,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,195,196,197,198,199,200,201,202,203,204,205,206,207,208,209,210,211,212,213,214,215,216,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,233,234,235,236,237,242,243,244,245,246,247,248,249,250,251,252,253,254,255,256,257,258,259,260,261,262,263,264,265,266,267,268,269,270,271,272,273,274,275,276,277,278,279,280,281,282,283,284,285,286,287,288,289,290,291,292,293,294,295,296,297,298,299,300,301,302,303,304,305,306
//)

//select * from activities a
//left join Trips t on a.Trip_Id = t.Id
//where t.Id is null
            //}

            //{
            //    foreach (Trip trip in trips)
            //    {
            //        Console.WriteLine("Trip: " + trip.Name);

            //        foreach (Activity activity in trip.Activities.ToList())
            //        {
            //            try
            //            {
            //                var newTitle = GenerateTitle(activity);
            //                Console.WriteLine("New Title: " + newTitle);
            //                if (newTitle != activity.Title)
            //                    activity.Title = newTitle;
            //            }
            //            catch
            //            {
            //                Console.WriteLine("EXCEPTION GENERATING TITLE FOR ACTIVITY " + activity.Id);
            //                Console.WriteLine("REMOVING ACTIVITY " + activity.Id);
            //                aRepo.Delete(activity);
            //            }
            //        }
            //    }

            //    repo.Save();
            //    aRepo.Save();
            //}

            Console.WriteLine("Done");
            Console.ReadKey(true);
        }

        //public static string GenerateTitle(Activity activity)
        //{
        //    if (!string.IsNullOrWhiteSpace(activity.Title))
        //        return activity.Title;

        //    if (activity.IsTransportation)
        //    {
        //        var title = activity.TransportationType != null ? activity.TransportationType.Name : "Transportation";
        //        foreach (ActivityPlace activityPlace in activity.ActivityPlaces.ToList())
        //        {
        //            title += (activityPlace.SortIndex == 0) ? " from " : " to ";
        //            title += activityPlace.Place.Name;
        //        }
        //        return title;
        //    }

        //    if (activity.ActivityPlaces.Any())
        //    {
        //        return activity.ActivityPlaces.First().Place.Name;
        //    }

        //    if (activity.Tags.Any())
        //    {
        //        var tagss = activity.Tags.Select(t => t.NiceName);
        //        var join = String.Join(", ", tagss);
        //        return join;
        //    }

        //    throw new Exception("Title could not be generated.");
        //}
    }
}
