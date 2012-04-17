using System;
using System.Linq;
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

            var trips = repo.FindAll().ToList();

            foreach (Trip trip in trips)
            {
                Console.WriteLine("Trip: " + trip.Name);

                foreach (Activity activity in trip.Activities.ToList())
                {
                    try
                    {
                        var newTitle = GenerateTitle(activity);
                        Console.WriteLine("New Title: " + newTitle);
                        if (newTitle != activity.Title)
                            activity.Title = newTitle;
                    }
                    catch
                    {
                        Console.WriteLine("EXCEPTION GENERATING TITLE FOR ACTIVITY " + activity.Id);
                        Console.WriteLine("REMOVING ACTIVITY " + activity.Id);
                        aRepo.Delete(activity);
                    }
                }
            }

            repo.Save();
            aRepo.Save();

            Console.WriteLine("Done");
            Console.ReadKey(true);
        }

        public static string GenerateTitle(Activity activity)
        {
            if (!string.IsNullOrWhiteSpace(activity.Title))
                return activity.Title;

            if (activity.IsTransportation)
            {
                var title = activity.TransportationType != null ? activity.TransportationType.Name : "Transportation";
                foreach (ActivityPlace activityPlace in activity.ActivityPlaces.ToList())
                {
                    title += (activityPlace.SortIndex == 0) ? " from " : " to ";
                    title += activityPlace.Place.Name;
                }
                return title;
            }

            if (activity.ActivityPlaces.Any())
            {
                return activity.ActivityPlaces.First().Place.Name;
            }

            if (activity.Tags.Any())
            {
                var tagss = activity.Tags.Select(t => t.NiceName);
                var join = String.Join(", ", tagss);
                return join;
            }

            throw new Exception("Title could not be generated.");
        }
    }
}
