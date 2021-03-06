﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenOClock.Data;

namespace GreenOClock.Services
{
    public class GroupActivityService : Service<GroupActivity, GreenOClockEntities>
    {
        private readonly CompanyService _companyService;
        private readonly UserService _userService;

        public GroupActivityService()
        {
            _companyService = new CompanyService();
            _userService = new UserService();
        }
       
        public GroupActivity ById(int activityId)
        {
            return All().FirstOrDefault(a => a.Id == activityId);
        }

        public IQueryable<GroupActivity> ByUserId(Guid userId)
        {
            var companies = _companyService.ByEmployeeId(userId);
            return companies.SelectMany(c => c.GroupActivities.Where(a => a.GroupMembers.FirstOrDefault(u => u.User.Id == userId) != null));
        }

        public IQueryable<GroupActivity> ByCompanyId(int companyId)
        {
            return All().Where(ga => ga.CompanyId == companyId);
        }

        public IQueryable<GroupActivity> ByProgressId(int progressId)
        {
            return All().Where(a => a.GroupProgresses.FirstOrDefault(p => p.Id == progressId) != null);
        }

        protected internal void FinishUpdate(GroupActivity changes, GroupActivity activity, GreenOClockEntities entities)
        {
            activity.GroupMembers = changes.GroupMembers;
            activity.GroupProgresses = changes.GroupProgresses;
        }

        public void UpdateGroupMembers(List<Guid> newGroupMemberIds, int activityId)
        {
            var activity = ById(activityId);

            var toAdd = new List<GroupMember>();
            var toRemove = new List<GroupMember>();

            var currentGroupMemberIds =         // all current members
                Entities.GroupMembers.Where(gm => gm.GroupActivityId == activityId).Select(gm => gm.UserId).ToList();

            foreach (var groupMemberId in newGroupMemberIds)       // for each new member
            {
                if (!currentGroupMemberIds.Contains(groupMemberId)) // if they don't work on the activity and should
                {
                    var member = new GroupMember
                        {
                            GroupActivityId = activityId,
                            UserId = groupMemberId
                        };
                    toAdd.Add(member);
                }
            }
            foreach (var groupMemberId in currentGroupMemberIds)        // for each current member
            {
                if (!newGroupMemberIds.Contains(groupMemberId)) // if the work and shouldn't
                {
                    var removal = activity.GroupMembers.FirstOrDefault(gm => gm.UserId == groupMemberId);
                    toRemove.Add(removal);
                }
            }
            using (Entities)
            {
                foreach (var add in toAdd)
                    Entities.GroupMembers.Add(add);
                foreach (var removal in toRemove)
                    Entities.GroupMembers.Remove(removal);
                Entities.SaveChanges();
            }
        }

        public void EnableActivity(int activityId)
        {
            using (Entities)
            {
                var activity = ById(activityId);
                activity.IsActive = true;
                Entities.SaveChanges();
            }
        }

        public void DeleteActivity(int activityId)
        {
            using (Entities)
            {
                var activity = ById(activityId);
                activity.IsActive = false;
                Entities.SaveChanges();
            }
        }
    }
}
