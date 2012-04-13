using System;
using System.Collections.Generic;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Forms
{
    //public class ActivityForm
    //{
    //    public int? ActivityId { get; set; }
    //    public string Title { get; set; }
    //    public int TripId { get; set; }
    //    public int? BeginDay { get; set; }
    //    public string BeginTime { get; set; }
    //    public int? EndDay { get; set; }
    //    public string EndTime { get; set; }
    //    public string TagString { get; set; }
    //    public string Note { get; set; }

    //    // Used only for display purposes
    //    public IEnumerable<Note> Notes { get; set; }
    //    public Tabs SelectedTab { get; set; }

    //    public enum Tabs { Details, Notes }

    //    public void SetBaseProps(Activity activity)
    //    {
    //        ActivityId = activity.Id;
    //        Title = activity.Title;
    //        BeginDay = activity.BeginDay;
    //        BeginTime = activity.BeginTime.HasValue ? DateTime.Today.Add(activity.BeginTime.Value).ToShortTimeString() : null;
    //        EndDay = activity.EndDay;
    //        EndTime = activity.EndTime.HasValue ? DateTime.Today.Add(activity.EndTime.Value).ToShortTimeString() : null;
    //        TripId = activity.Trip.Id;
    //        TagString = activity.TagString;
    //        Notes = activity.Notes;
    //    }
    //}

    //public class TransportationActivityForm : ActivityForm
    //{
    //    public int? TransportationTypeId { get; set; }
    //    public string FromGoogReference { get; set; }
    //    public string FromGoogId { get; set; }
    //    public string ToGoogReference { get; set; }
    //    public string ToGoogId { get; set; }

    //    // For display only
    //    public string FromName { get; set; }
    //    public string ToName { get; set; }

    //}

    //public class PlaceActivityForm : ActivityForm
    //{
    //    public int? PlaceId { get; set; }
    //    public string GoogReference { get; set; }
    //    public string GoogId { get; set; }
    //    public string Name { get; set; }
    //    public string Address { get; set; }
    //    public string City { get; set; }
    //    public string State { get; set; }
    //    public string Country { get; set; }

    //    public string Telephone { get; set; }

    //    public string Website { get; set; }

    //    public string Latitude { get; set; }
    //    public string Longitude { get; set; }
    //}
}