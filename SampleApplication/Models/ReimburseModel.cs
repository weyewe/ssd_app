using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleApplication.Models
{
    public class ReimburseModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Total { get; set; }
        public decimal ActualPaid { get; set; }
        public bool IsConfimed { get; set; }
        public bool IsSubmitted { get; set; }
        public bool IsClearaned { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public DateTime? ConfirmedDate { get; set; }
        public DateTime? ClearanceDate { get; set; }
        public List<Detail> ListDetail { get; set; }
        public Dictionary<string, string> Errors { get; set; }

        public class List
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public decimal Total { get; set; }
            public string Name { get; set; }
            public bool IsConfimed { get; set; }
            public DateTime? ConfirmedDate { get; set; }
            public bool IsSubmitted { get; set; }
            public DateTime? SubmittedDate { get; set; }
            public bool IsClearaned { get; set; }
            public DateTime? ClearanceDate { get; set; }
            public DateTime? CreatedDate { get; set; }
            public decimal ActualPaid { get; set; }
        }

        public class Detail
        {
            public int Id { get; set; }
            public int ReimburseId { get; set; }
            public string Description { get; set; }
            public decimal Amount { get; set; }
            public bool IsRejected { get; set; }
            public DateTime? ExpenseDate { get; set; }
            public Dictionary<string, string> Errors { get; set; }
        }


        public class Print
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public decimal Total { get; set; }
        }
    }
}