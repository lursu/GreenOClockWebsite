﻿using System;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using GreenOClock.Data;
using GreenOClock.Mvc.Models.Progresses;
using GreenOClock.Services;

public static class ActiveSessionData
{
    private static HttpSessionState _session { get { return HttpContext.Current.Session; } }
    private static UserService _users = new UserService();

    public static User ActiveUser
    {
        get
        {
            try
            {
                var guid = (Guid)Membership.GetUser().ProviderUserKey;
                return _users.ById(guid);
            }
            catch (Exception)
            {
                return new User();
            }
        }
    }

    public static Activity ActiveActivity
    {
        get { return _session["ActiveActivity"] as Activity; }
        set { _session["ActiveActivity"] = value; }
    }

    public static Progress ActiveActivityProgress
    {
        get { return _session["ActiveActivityProgress"] as Progress; }
        set { _session["ActiveActivityProgress"] = value; }
    }
}