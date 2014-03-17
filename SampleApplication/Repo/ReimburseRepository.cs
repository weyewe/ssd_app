using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using SampleApplication.Models;
using SampleApplication;

namespace SampleApplication.Repo
{
    public class ReimburseRepository : EfRepository<Reimburse>, IReimburseRepository
    {
        private readonly static log4net.ILog LOG = log4net.LogManager.GetLogger("ReimburseRepository");

        public List<ReimburseModel.List> GetReimburseList(string sortby, string sortdir, string vWhere)
        {
            using (var db = GetContext())
            {
                IQueryable<ReimburseModel.List> list = (from r in db.Reimburses
                                                        join u in db.Users on r.CreatedBy equals u.Id
                                                            into rux
                                                        from ru in rux.DefaultIfEmpty()
                                                        select new ReimburseModel.List
                                                        {
                                                            Id = r.Id,
                                                            Description = r.Description,
                                                            Total = r.Total.HasValue ? r.Total.Value : 0,
                                                            IsSubmitted = r.IsSubmitted.HasValue ? r.IsSubmitted.Value : false,
                                                            SubmittedDate = r.SubmittedDate,
                                                            IsClearaned = r.IsCleared.HasValue ? r.IsCleared.Value : false,
                                                            ClearanceDate = r.ClearanceDate,
                                                            IsConfimed = r.IsConfirmed.HasValue ? r.IsConfirmed.Value : false,
                                                            ConfirmedDate = r.ConfirmedDate,
                                                            Name = ru.FirstName + " " + ru.LastName,
                                                            CreatedDate = r.CreatedDate,
                                                            ActualPaid = r.ActualPaid.HasValue ? r.ActualPaid.Value : 0
                                                        }).AsQueryable();

                return list.ToList();
            }
        }

        public Reimburse InsertReimburse(Reimburse reimburse)
        {
            Reimburse newReimburse = new Reimburse
            {
                Description = reimburse.Description,
                Total = reimburse.Total,
                ActualPaid = reimburse.ActualPaid,
                CreatedBy = reimburse.CreatedBy,
                CreatedDate = reimburse.CreatedDate
            };

            return Create(newReimburse);
        }

        public void DeleteReimburse(int reimburseId)
        {
            using (var db = GetContext())
            {
                Reimburse reim = Find(r => r.Id == reimburseId);
                if (reim != null)
                {
                    Delete(reim);
                }
            }
        }

        public Reimburse UpdateReimburse(Reimburse reimburse)
        {
            Reimburse updReimburse = Find(r => r.Id == reimburse.Id);
            if (updReimburse != null)
            {
                updReimburse.ActualPaid = reimburse.ActualPaid;
                updReimburse.ClearanceDate = reimburse.ClearanceDate;
                updReimburse.ConfirmedDate = reimburse.ConfirmedDate;
                updReimburse.Description = reimburse.Description;
                updReimburse.IsCleared = reimburse.IsCleared;
                updReimburse.IsConfirmed = reimburse.IsConfirmed;
                updReimburse.IsSubmitted = reimburse.IsSubmitted;
                updReimburse.SubmittedDate = reimburse.SubmittedDate;
                updReimburse.Total = reimburse.Total;

                Update(updReimburse);
            }

            return updReimburse;
        }




        public List<ReimburseDetail> GetReimburseDetailList(int reimburseId)
        {
            using (var db = GetContext())
            {
                var detailList = (from rd in db.ReimburseDetails where rd.ReimburseId == reimburseId select rd).ToList();
                return detailList;
            }
        }

        public ReimburseDetail GetReimburseDetail(int reimburseDetailId)
        {
            using (var db = GetContext())
            {
                var detail = (from rd in db.ReimburseDetails where rd.Id == reimburseDetailId select rd).FirstOrDefault();
                return detail;
            }
        }

        public ReimburseDetail InsertReimburseDetail(ReimburseDetail reimburseDetail)
        {
            using (var db = GetContext())
            {
                ReimburseDetail newReimburseDetail = new ReimburseDetail
                {
                    Amount = reimburseDetail.Amount,
                    Description = reimburseDetail.Description,
                    CreatedDate = reimburseDetail.CreatedDate,
                    ReimburseId = reimburseDetail.ReimburseId,
                    IsRejected = reimburseDetail.IsRejected,
                    ExpenseDate = reimburseDetail.ExpenseDate
                };

                newReimburseDetail = db.ReimburseDetails.Add(newReimburseDetail);
                db.SaveChanges();

                return newReimburseDetail;
            }
        }

        public void DeleteReimburseDetail(int reimburseDetailId)
        {
            using (var db = GetContext())
            {
                ReimburseDetail reim = (from rd in db.ReimburseDetails where rd.Id == reimburseDetailId select rd).FirstOrDefault();
                if (reim != null)
                {
                    db.ReimburseDetails.Remove(reim);
                    db.SaveChanges();
                }
            }
        }

        public void DeleteReimburseDetailByReimburseId(int reimburseId)
        {
            using (var db = GetContext())
            {
                List<ReimburseDetail> reim = (from rd in db.ReimburseDetails where rd.ReimburseId == reimburseId select rd).ToList();
                if (reim != null)
                {
                    foreach (var item in reim)
                    {
                        var reimDetail = (from rd in db.ReimburseDetails where rd.Id == item.Id select rd).FirstOrDefault();
                        if (reimDetail != null)
                        {
                            db.ReimburseDetails.Remove(reimDetail);
                            db.SaveChanges();
                        }
                    }
                }
            }
        }

        public ReimburseDetail UpdateReimburseDetail(ReimburseDetail reimburseDetail)
        {
            using (var db = GetContext())
            {
                ReimburseDetail updReimburseDetail = (from rd in db.ReimburseDetails where rd.Id == reimburseDetail.Id select rd).FirstOrDefault();
                if (updReimburseDetail != null)
                {
                    updReimburseDetail.Amount = reimburseDetail.Amount;
                    updReimburseDetail.Description = reimburseDetail.Description;
                    updReimburseDetail.ExpenseDate = reimburseDetail.ExpenseDate;
                    updReimburseDetail.IsRejected = reimburseDetail.IsRejected;
                    updReimburseDetail.ReimburseId = reimburseDetail.ReimburseId;

                    db.SaveChanges();
                }

                return updReimburseDetail;
            }
        }
    }
}