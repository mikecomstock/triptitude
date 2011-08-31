using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Triptitude.Biz.Models;

namespace Triptitude.Web.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString SelectTripDropDownList(this HtmlHelper html, IEnumerable<Trip> trips, Trip selectedTrip)
        {
            return html.DropDownList("id", trips.ToList().Select(t => new SelectListItem() { Text = t.Name, Value = t.Id.ToString(), Selected = t.Id == selectedTrip.Id }));
        }

        public static MvcHtmlString SelectTransportationTypeDropDownList(this HtmlHelper html, IEnumerable<TransportationType> transportationTypes, int selectedTypeId)
        {
            var options = transportationTypes.Select(t => new SelectListItem { Text = t.Name, Value = t.Id.ToString(), Selected = t.Id == selectedTypeId });
            return html.DropDownList("transportationtypeid", options);
        }

        public static MvcHtmlString SelectActivity(this HtmlHelper html, Trip trip, object htmlAttributes = null)
        {
            var options = from i in Enumerable.Range(1, trip.TotalDays)
                          select new GroupDropListItem
                                     {
                                         Items = from n in trip.Activities.Where(a => a.BeginDay == i)
                                                 select new OptionItem
                                                            {
                                                                Text = n.Name,
                                                                Value = n.Id.ToString()
                                                            },
                                         Name = "Day " + i
                                     };

            return html.GroupDropList("activityid", options, null, htmlAttributes);
        }

        public static string RouteCss(this HtmlHelper html)
        {
            List<string> tokens = new List<string>();
            if (html.ViewContext.RouteData.DataTokens["area"] != null)
                tokens.Add(html.ViewContext.RouteData.DataTokens["area"].ToString().ToLower());
            tokens.Add(html.ViewContext.Controller.ValueProvider.GetValue("controller").RawValue.ToString().Replace("admin", string.Empty).ToLower());
            tokens.Add(html.ViewContext.Controller.ValueProvider.GetValue("action").RawValue.ToString().ToLower());
            return String.Join(" ", tokens);
        }

        #region Grouped Select List

        /// <summary>
        /// From http://stackoverflow.com/questions/607188/support-for-optgroup-in-dropdownlist-net-mvc
        /// </summary>
        public static MvcHtmlString GroupDropList(this HtmlHelper helper, string name, IEnumerable<GroupDropListItem> data, string SelectedValue, object htmlAttributes)
        {
            if (data == null && helper.ViewData != null)
                data = helper.ViewData.Eval(name) as IEnumerable<GroupDropListItem>;
            if (data == null) return MvcHtmlString.Empty;

            var select = new TagBuilder("select");

            if (htmlAttributes != null)
                select.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            select.GenerateId(name);
            select.MergeAttribute("name", name);

            var optgroupHtml = new StringBuilder();
            var groups = data.ToList();
            foreach (var group in data)
            {
                var groupTag = new TagBuilder("optgroup");
                groupTag.Attributes.Add("label", helper.Encode(group.Name));
                var optHtml = new StringBuilder();
                foreach (var item in group.Items)
                {
                    var option = new TagBuilder("option");
                    option.Attributes.Add("value", helper.Encode(item.Value));
                    if (SelectedValue != null && item.Value == SelectedValue)
                        option.Attributes.Add("selected", "selected");
                    option.InnerHtml = helper.Encode(item.Text);
                    optHtml.AppendLine(option.ToString(TagRenderMode.Normal));
                }
                groupTag.InnerHtml = optHtml.ToString();
                optgroupHtml.AppendLine(groupTag.ToString(TagRenderMode.Normal));
            }
            select.InnerHtml = optgroupHtml.ToString();
            return MvcHtmlString.Create(select.ToString(TagRenderMode.Normal));
        }

        public class GroupDropListItem
        {
            public string Name { get; set; }
            public IEnumerable<OptionItem> Items { get; set; }
        }

        public class OptionItem
        {
            public string Text { get; set; }
            public string Value { get; set; }
        }

        #endregion
    }
}