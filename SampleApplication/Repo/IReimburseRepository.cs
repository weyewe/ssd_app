using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SampleApplication.Models;

namespace SampleApplication.Repo
{
    public interface IReimburseRepository : IRepository<Reimburse>
    {
        List<ReimburseModel.List> GetReimburseList(string sortby, string sortdir, string vWhere);
        Reimburse InsertReimburse(Reimburse reimburse);
        void DeleteReimburse(int reimburseId);
        Reimburse UpdateReimburse(Reimburse reimburse);

        List<ReimburseDetail> GetReimburseDetailList(int reimburseId);
        ReimburseDetail GetReimburseDetail(int reimburseDetailId);
        ReimburseDetail InsertReimburseDetail(ReimburseDetail reimburseDetail);
        void DeleteReimburseDetail(int reimburseDetailId);
        void DeleteReimburseDetailByReimburseId(int reimburseId);
        ReimburseDetail UpdateReimburseDetail(ReimburseDetail reimburseDetail);
    }
}