﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GreenOClock.Data;
using GreenOClock.Mvc.Models.Progresses;

namespace GreenOClock.Mvc.Models.Activities
{
    public abstract class ActivityViewModel
    {
        [Required]
        [StringLength(64, ErrorMessage = "Names need to be between 3 and 30 charactars inclusively.", MinimumLength = 3)]
        public string Name { get; set; }
        
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "Description has to be between 6 and 250 charactars long.", MinimumLength = 6)]
        public string Description { get; set; }

        [ScaffoldColumn(false)]
        public bool IsActive { get; set; }

        [ScaffoldColumn(false)]
        public ProgressesViewModel Progresses { get; set; }

        protected ActivityViewModel()
        {
        }

        protected ActivityViewModel(Activity activity)
        {
            Name = activity.Name;
            Id = activity.Id;
            Description = activity.Description;
            IsActive = activity.IsActive;
        }
    }
}