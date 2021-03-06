﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using GreenOClock.Data;
using GreenOClock.Mvc.Models;
using GreenOClock.Mvc.Models.Activities;
using GreenOClock.Mvc.Models.Companies;
using GreenOClock.Services;

namespace GreenOClock.Mvc.Controllers
{
    [Authorize]
    public class ActivityController : Controller
    {
        //
        // GET: /Activity/
        private readonly ActivityService _activityService;
        private readonly ActivitySavingUtility _activitySavingUtility;
        private readonly PersonalActivityService _personalActivities;
        private readonly CompanyEmployeeService _companyEmployees;

        public ActivityController()
        {
            _activityService = new ActivityService();
            _activitySavingUtility = new ActivitySavingUtility(_activityService);
            _personalActivities = new PersonalActivityService();
            _companyEmployees = new CompanyEmployeeService();
        }

        public ActionResult Index()
        {
            var usersGroupActivities = _activityService.GroupActivityService.ByUserId(ActiveSessionData.ActiveUser.Id).ToList().Where(a => a.IsActive);
            var usersPersonalActivities = _activityService.PersonalActivityService.ByUserId(ActiveSessionData.ActiveUser.Id).ToList().Where(a => a.IsActive);
            return View(new ActivitiesViewModel(usersGroupActivities, usersPersonalActivities));
        }

        #region Personal Activity
        public ActionResult CreatePersonalActivity()
		{
            return View(new PersonalActivityViewModel() { UserId = ActiveSessionData.ActiveUser.Id });
		}
		
        [HttpPost]
        public ActionResult CreatePersonalActivity(PersonalActivityViewModel activity)
        {
            try
            {
                activity.Id = _activitySavingUtility.SaveActivity(activity);
                return RedirectToAction("PersonalActivityDetails", new { activityId = activity.Id });
            }
            catch
            {
                TempData["Message"] = "Error creating new personal activity.";
            }
            return View(activity);
        }

        public ActionResult EditPersonalActivity(int activityId)
        {
            var activity = new PersonalActivityService().ById(activityId);
            return View(new PersonalActivityViewModel(activity));
        }

        [HttpPost]
        public ActionResult EditPersonalActivity(PersonalActivityViewModel activity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    activity.Id = _activitySavingUtility.SaveActivity(activity);
                    return RedirectToAction("PersonalActivityDetails", new { activityId = activity.Id });
                }
            }
            catch
            {
                TempData["Message"] = "Error while updating activity.";
            }
            return View(activity);
        }

        public ActionResult PersonalActivityDetails(int activityId)
        {
            var activity = _activityService.PersonalActivityService.ById(activityId);
            
            return View(new PersonalActivityViewModel(activity));
        }

        #endregion

        #region Group Activity
        public ActionResult CreateGroupActivity(int companyId)
        {
            return View(new GroupActivityViewModel() { CompanyId = companyId });
        }

        [HttpPost]
        public ActionResult CreateGroupActivity(GroupActivityViewModel activity, int companyId)
        {
            try
            {
                activity.CompanyId = companyId;
                activity.Id = _activitySavingUtility.SaveActivity(activity);

                return RedirectToAction("GroupActivityDetails", new { activityId = activity.Id });
            } catch {
                TempData["Message"] = "Error while creating activity";
            }
            return View(activity);
        }

        public ActionResult EditGroupActivity(int activityId)
        {
            var activity = new GroupActivityService().ById(activityId);
            return View(new GroupActivityViewModel(activity));
        }

        [HttpPost]
        public ActionResult EditGroupActivity(GroupActivityViewModel activity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    activity.Id = _activitySavingUtility.SaveActivity(activity);
                    return RedirectToAction("GroupActivityDetails", new { activityId = activity.Id });
                }
            }
            catch
            {
                TempData["Message"] = "Error while updating activity.";
            }
            return View(activity);
        }

        public ActionResult GroupActivityDetails(int activityId)
        {
            var activity = _activityService.GroupActivityService.ById(activityId);
            return View(new GroupActivityViewModel(activity));
        }
        #endregion
		
        public ActionResult Details(int activityId)
        {
            if (_activityService.PersonalActivityService.ById(activityId) != null)
                return RedirectToAction("PersonalActivityDetails", new {activityId});
            else
                return RedirectToAction("GroupActivityDetails", new {activityId});
        }

        public ActionResult History()
        {
            var usersGroupActivities = _activityService.GroupActivityService.ByUserId(ActiveSessionData.ActiveUser.Id).ToList().Where(a => !a.IsActive);
            var usersPersonalActivities = _activityService.PersonalActivityService.ByUserId(ActiveSessionData.ActiveUser.Id).ToList().Where(a => !a.IsActive);
            return View(new ActivitiesViewModel(usersGroupActivities, usersPersonalActivities));
        }

        public PartialViewResult ActivityManagerControls(GroupActivityViewModel activity)
        {
            var isEmployee = _companyEmployees.UserHasManagerPriveleges(ActiveSessionData.ActiveUser.Id, activity.Company.Id);

            return isEmployee ? PartialView("CompanyManagerControls", activity) : null;
        }

        public ActionResult EnablePersonalActivity(int activityId)
        {
            _activityService.PersonalActivityService.EnableActivity(activityId);
            return RedirectToAction("Index");
        }

        public ActionResult DeletePersonalActivity(int activityId)
        {
            _activityService.PersonalActivityService.DeleteActivity(activityId);
            return RedirectToAction("Index");
        }

        public ActionResult EnableGroupActivity(int activityId)
        {
            _activityService.GroupActivityService.EnableActivity(activityId);
            return RedirectToAction("Index");
        }

        public ActionResult DeleteGroupActivity(int activityId)
        {
            _activityService.GroupActivityService.DeleteActivity(activityId);
            return RedirectToAction("Index");
        }
    }
}
