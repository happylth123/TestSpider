using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PageExtractor.Models
{
    public class JobInfo
    {
        public Guid JobInfoID { get; set; }

        public string CollegeMajornIDNums { get; set; }

        // 招聘类型 0 是校招 1 是社招
        public int JobInfoType { get; set; }

        public string CollegeName { get; set; }

        public string JobInfoTitle { get; set; }

        public string Contents { get; set; }

        public string JobInfoAdress { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime CreateDate { get; set; }

        public long VisitCount { get; set; }
    }
}
