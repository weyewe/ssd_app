using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SampleApplication.Models;

namespace SampleApplication.Service
{
    public interface IReimburseService
    {
        List<ReimburseModel.List> GetReimburseList(string sortby, string sortdir, string vWhere);
        ResponseModel GetReimburseInfo(int reimburseId);
        ResponseModel InsertReimburse(ReimburseModel reimburse);
        ResponseModel DeleteReimburse(int reimburseId);
        ResponseModel UpdateReimburse(ReimburseModel reimburse);
        ResponseModel SubmitReimburse(ReimburseModel reimburse);
        ResponseModel ConfirmReimburse(ReimburseModel reimburse);
        ResponseModel ClearReimburse(ReimburseModel reimburse);
        bool ValidationInsert(ReimburseModel model, out string message);
        bool ValidationUpdate(ReimburseModel model, out string message);

        ResponseModel InsertReimburseDetail(ReimburseModel.Detail reimburseDetail);
        ResponseModel UpdateReimburseDetail(ReimburseModel.Detail reimburseDetail);
        ResponseModel DeleteReimburseDetail(int invDetailId);
        ResponseModel RejectReimburseDetail(ReimburseModel.Detail reimburseDetail);
    }
}