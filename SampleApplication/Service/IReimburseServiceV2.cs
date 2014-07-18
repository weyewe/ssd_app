using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SampleApplication.Models;

namespace SampleApplication.Service
{
    public interface IReimburseServiceV2
    {
        List<ReimburseModel.List> GetReimburseList(string sortby, string sortdir, string vWhere);
        List<ReimburseModel.Detail> GetListReimburseDetail(int reimburseid, string sortby, string sortdir, string vWhere);
        ReimburseModel GetReimburseInfo(int reimburseId);
        ReimburseModel InsertReimburse(ReimburseModel reimburse);
        ReimburseModel DeleteReimburse(ReimburseModel reimburse);
        ReimburseModel UpdateReimburse(ReimburseModel reimburse);
        ReimburseModel SubmitReimburse(ReimburseModel reimburse);
        ReimburseModel ConfirmReimburse(ReimburseModel reimburse);
        ReimburseModel ClearReimburse(ReimburseModel reimburse);
        Dictionary<string, string> ValidationInsert(ReimburseModel model);
        Dictionary<string, string> ValidationUpdate(ReimburseModel model);

        ReimburseModel.Detail InsertReimburseDetail(ReimburseModel.Detail reimburseDetail);
        ReimburseModel.Detail UpdateReimburseDetail(ReimburseModel.Detail reimburseDetail);
        ReimburseModel.Detail DeleteReimburseDetail(ReimburseModel.Detail reimburseDetail);
        ReimburseModel.Detail RejectReimburseDetail(ReimburseModel.Detail reimburseDetail);
    }
}