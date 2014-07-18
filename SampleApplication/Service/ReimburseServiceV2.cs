using System;
using System.Collections.Generic;
using System.Linq;
//using System.Linq.Dynamic;
using System.Web;
using System.Data.Entity.Validation;
using SampleApplication.Models;
using SampleApplication.Repo;
using log4net;

namespace SampleApplication.Service
{
    public class ReimburseServiceV2 : IReimburseServiceV2, IDisposable
    {
        private readonly static log4net.ILog LOG = log4net.LogManager.GetLogger("ReimburseServiceV2");
        private readonly IReimburseRepository _reimburseRepo;

        public ReimburseServiceV2()
        {
            _reimburseRepo = new ReimburseRepository();
        }

        // Reimburse List
        public List<ReimburseModel.List> GetReimburseList(string sortby, string sortdir, string vWhere)
        {
            List<ReimburseModel.List> model = new List<ReimburseModel.List>();
            try
            {
                model = _reimburseRepo.GetReimburseList(sortby, sortdir, vWhere);
            }
            catch (Exception ex)
            {
                LOG.Error("GetReimburseList Failed", ex);
            }

            return model;
        }

        // Reimburse Detail List
        public List<ReimburseModel.Detail> GetListReimburseDetail(int reimburseid, string sortby, string sortdir, string vWhere)
        {
            List<ReimburseModel.Detail> model = new List<ReimburseModel.Detail>();
            try
            {
                var reimburseDetail = _reimburseRepo.GetReimburseDetailList(reimburseid);
                foreach (var item in reimburseDetail)
                {
                    ReimburseModel.Detail detail = new ReimburseModel.Detail();
                    detail.Description = item.Description;
                    detail.Amount = item.Amount.HasValue ? item.Amount.Value : 0;
                    detail.ExpenseDate = item.ExpenseDate;
                    detail.Id = item.Id;
                    detail.IsRejected = item.IsRejected.HasValue ? item.IsRejected.Value : false;
                    detail.ReimburseId = item.ReimburseId.HasValue ? item.ReimburseId.Value : 0;
                    model.Add(detail);
                }
            }
            catch (Exception ex)
            {
                LOG.Error("GetListReimburseDetail Failed", ex);
            }

            return model;
        }

        // Reimburse Info
        public ReimburseModel GetReimburseInfo(int reimburseId)
        {
            ReimburseModel model = new ReimburseModel();
            model.Errors = new Dictionary<string, string>();
            try
            {
                var reimburse = _reimburseRepo.Find(r => r.Id == reimburseId);
                if (reimburse != null)
                {
                    model.Description = reimburse.Description;
                    model.Id = reimburse.Id;
                    model.IsClearaned = reimburse.IsCleared.HasValue ? reimburse.IsCleared.Value : false;
                    model.IsConfimed = reimburse.IsConfirmed.HasValue ? reimburse.IsConfirmed.Value : false;
                    model.IsSubmitted = reimburse.IsSubmitted.HasValue ? reimburse.IsSubmitted.Value : false;
                    model.SubmittedDate = reimburse.SubmittedDate;
                    model.ConfirmedDate = reimburse.ConfirmedDate;
                    model.ClearanceDate = reimburse.ClearanceDate;
                    model.Total = reimburse.Total.HasValue ? reimburse.Total.Value : 0;

                    var reimburseDetail = _reimburseRepo.GetReimburseDetailList(reimburse.Id);
                    List<ReimburseModel.Detail> listDetail = new List<ReimburseModel.Detail>();
                    foreach (var item in reimburseDetail)
                    {
                        ReimburseModel.Detail detail = new ReimburseModel.Detail();
                        detail.Description = item.Description;
                        detail.Amount = item.Amount.HasValue ? item.Amount.Value : 0;
                        detail.ExpenseDate = item.ExpenseDate;
                        detail.Id = item.Id;
                        detail.IsRejected = item.IsRejected.HasValue ? item.IsRejected.Value : false;
                        detail.ReimburseId = item.ReimburseId.HasValue ? item.ReimburseId.Value : 0;
                        listDetail.Add(detail);
                    }
                    model.ListDetail = listDetail;

                }
                else
                {
                    model.Errors.Add("Generic", "Reimburse not found");
                }
            }
            catch (Exception ex)
            {
                LOG.Error("GetReimburseInfo Failed", ex);
                model.Errors.Add("Generic", "There is error when load this invoice..");
            }

            return model;
        }

        // Insert Reimburse
        public ReimburseModel InsertReimburse(ReimburseModel reimburse)
        {
            reimburse.Errors = new Dictionary<string, string>();
            try
            {
                reimburse.Errors = this.ValidationInsert(reimburse);
                if (reimburse.Errors != null && reimburse.Errors.Count > 0)
                {
                    return reimburse;
                }

                Reimburse newReimburse = new Reimburse();
                newReimburse.Description = reimburse.Description;
                newReimburse.ActualPaid = reimburse.ActualPaid;
                newReimburse.Total = reimburse.Total;
                newReimburse.CreatedBy = AccountModel.GetUserId();
                newReimburse.CreatedDate = DateTime.Today;

                newReimburse = _reimburseRepo.InsertReimburse(newReimburse);
                reimburse.Id = newReimburse.Id;

                if (reimburse.ListDetail != null)
                {
                    foreach (var detail in reimburse.ListDetail)
                    {
                        detail.ReimburseId = newReimburse.Id;
                        this.InsertReimburseDetail(detail);
                    }
                }

                LOG.Error("InsertReimburse Sucess");
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    LOG.ErrorFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        validationErrors.Entry.Entity.GetType().Name, validationErrors.Entry.State);
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        //Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        LOG.ErrorFormat("InsertReimburse, Error:Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }

                LOG.Error("InsertReimburse Failed", dbEx);
                reimburse.Errors.Add("Generic", "Insert Reimburse failed, Please try again or contact your administrator.");
            }
            catch (Exception ex)
            {
                LOG.Error("InsertReimburse Failed", ex);
                reimburse.Errors.Add("Generic", "Insert Reimburse Failed, Please try again or contact your administrator.");
            }

            return reimburse;
        }

        // Delete Reimburse
        public ReimburseModel DeleteReimburse(ReimburseModel reimburse)
        {
            reimburse.Errors = new Dictionary<string, string>();
            try
            {
                Reimburse deleteReimburse = _reimburseRepo.Find(p => p.Id == reimburse.Id);
                if (deleteReimburse != null)
                {
                    // Delete Reimburse Detail
                    _reimburseRepo.DeleteReimburseDetail(deleteReimburse.Id);

                    // Delete Reimburse
                    _reimburseRepo.DeleteReimburse(deleteReimburse.Id);
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    LOG.ErrorFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        validationErrors.Entry.Entity.GetType().Name, validationErrors.Entry.State);
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        //Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        LOG.ErrorFormat("DeleteReimburse, Error:Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }

                LOG.Error("DeleteReimburse Failed", dbEx);
                reimburse.Errors.Add("Generic", "Delete Reimburse failed, Please try again or contact your administrator.");
            }
            catch (Exception ex)
            {
                LOG.Error("DeleteReimburse Failed", ex);
                reimburse.Errors.Add("Generic", "Delete Reimburse Failed, Please try again or contact your administrator.");
            }

            return reimburse;
        }

        // Update Reimburse
        public ReimburseModel UpdateReimburse(ReimburseModel reimburse)
        {
            reimburse.Errors = new Dictionary<string, string>();
            try
            {
                reimburse.Errors = this.ValidationUpdate(reimburse);
                if (reimburse.Errors != null && reimburse.Errors.Count > 0)
                {
                    return reimburse;
                }

                Reimburse updateReimburse = _reimburseRepo.Find(p => p.Id == reimburse.Id);
                if (updateReimburse != null)
                {
                    updateReimburse.Description = reimburse.Description;
                    updateReimburse.ActualPaid = reimburse.ActualPaid;
                    updateReimburse.Total = reimburse.Total;

                    _reimburseRepo.UpdateReimburse(updateReimburse);

                    LOG.Info("UpdateReimburse Success");
                }
                else
                {
                    reimburse.Errors.Add("Generic", "Reimburse not found...");
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    LOG.ErrorFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        validationErrors.Entry.Entity.GetType().Name, validationErrors.Entry.State);
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        //Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        LOG.ErrorFormat("UpdateReimburse, Error:Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }

                LOG.Error("UpdateReimburse Failed", dbEx);
                reimburse.Errors.Add("Generic", "Update data failed, Please try again or contact your administrator.");
            }
            catch (Exception ex)
            {
                LOG.Error("UpdateReimburse Failed", ex);
                reimburse.Errors.Add("Generic", "Update data Failed, Please try again or contact your administrator.");
            }

            return reimburse;
        }

        // Submit Reimburse
        public ReimburseModel SubmitReimburse(ReimburseModel reimburse)
        {
            reimburse.Errors = new Dictionary<string, string>();
            try
            {
                reimburse.Errors = this.ValidationSubmit(reimburse);
                if (reimburse.Errors != null && reimburse.Errors.Count > 0)
                {
                    return reimburse;
                }

                Reimburse submitReimburse = _reimburseRepo.Find(p => p.Id == reimburse.Id);
                if (submitReimburse != null)
                {
                    submitReimburse.IsSubmitted = true;
                    submitReimburse.SubmittedDate = reimburse.SubmittedDate;

                    _reimburseRepo.UpdateReimburse(submitReimburse);

                    LOG.Info("SubmitReimburse Success");
                }
                else
                {
                    reimburse.Errors.Add("Generic", "Reimburse not found...");
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    LOG.ErrorFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        validationErrors.Entry.Entity.GetType().Name, validationErrors.Entry.State);
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        //Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        LOG.ErrorFormat("SubmitReimburse, Error:Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }

                LOG.Error("SubmitReimburse Failed", dbEx);
                reimburse.Errors.Add("Generic", "Submit data failed, Please try again or contact your administrator.");
            }
            catch (Exception ex)
            {
                LOG.Error("SubmitReimburse Failed", ex);
                reimburse.Errors.Add("Generic", "Update Submit Failed, Please try again or contact your administrator.");
            }

            return reimburse;
        }

        // Confirm Reimburse
        public ReimburseModel ConfirmReimburse(ReimburseModel reimburse)
        {
            reimburse.Errors = new Dictionary<string, string>();
            try
            {
                reimburse.Errors = this.ValidationConfirm(reimburse);
                if (reimburse.Errors != null && reimburse.Errors.Count > 0)
                {
                    return reimburse;
                }

                Reimburse confirmReimburse = _reimburseRepo.Find(p => p.Id == reimburse.Id);
                if (confirmReimburse != null)
                {
                    confirmReimburse.ConfirmedDate = reimburse.ConfirmedDate;
                    confirmReimburse.IsConfirmed = true;

                    _reimburseRepo.UpdateReimburse(confirmReimburse);

                    LOG.Info("ConfirmReimburse Success");
                }
                else
                {
                    reimburse.Errors.Add("Generic", "Reimburse not found...");
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    LOG.ErrorFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        validationErrors.Entry.Entity.GetType().Name, validationErrors.Entry.State);
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        //Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        LOG.ErrorFormat("ConfirmReimburse, Error:Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }

                LOG.Error("ConfirmReimburse Failed", dbEx);
                reimburse.Errors.Add("Generic", "Confirm data failed, Please try again or contact your administrator.");
            }
            catch (Exception ex)
            {
                LOG.Error("ConfirmReimburse Failed", ex);
                reimburse.Errors.Add("Generic", "Confirm data Failed, Please try again or contact your administrator.");
            }

            return reimburse;
        }

        // Clear Reimburse
        public ReimburseModel ClearReimburse(ReimburseModel reimburse)
        {
            reimburse.Errors = new Dictionary<string, string>();
            try
            {
                reimburse.Errors = this.ValidationClear(reimburse);
                if (reimburse.Errors != null && reimburse.Errors.Count > 0)
                {
                    return reimburse;
                }

                Reimburse clearReimburse = _reimburseRepo.Find(p => p.Id == reimburse.Id);
                if (clearReimburse != null)
                {
                    clearReimburse.ClearanceDate = reimburse.ClearanceDate;
                    clearReimburse.IsCleared = true;

                    _reimburseRepo.UpdateReimburse(clearReimburse);

                    LOG.Info("ClearReimburse Success");
                }
                else
                {
                    reimburse.Errors.Add("Generic", "Reimburse not found...");
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    LOG.ErrorFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        validationErrors.Entry.Entity.GetType().Name, validationErrors.Entry.State);
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        //Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        LOG.ErrorFormat("ClearReimburse, Error:Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }

                LOG.Error("ClearReimburse Failed", dbEx);
                reimburse.Errors.Add("Generic", "Clear data failed, Please try again or contact your administrator.");
            }
            catch (Exception ex)
            {
                LOG.Error("ClearReimburse Failed", ex);
                reimburse.Errors.Add("Generic", "Clear data Failed, Please try again or contact your administrator.");
            }

            return reimburse;
        }

        public Dictionary<string, string> Validation(ReimburseModel model)
        {
            if (model.Errors == null)
                model.Errors = new Dictionary<string, string>();

            // Description
            if (String.IsNullOrEmpty(model.Description) || model.Description.Trim() == "")
            {
                model.Errors.Add("Description", "Invalid Description");
            }

            //// Total
            //if (model.Total <= 0)
            //{
            //    message = "Invalid Total...";
            //    return false;
            //}

            return model.Errors;
        }

        public Dictionary<string, string> ValidationInsert(ReimburseModel model)
        {
            model.Errors = this.Validation(model);

            if (AccountModel.GetUserTypeId() == AccountModel.UserTypeCashier)
            {
                model.Errors.Add("Generic", "You dont have permission...");
            }

            return model.Errors;
        }

        public Dictionary<string, string> ValidationUpdate(ReimburseModel model)
        {
            model.Errors = this.Validation(model);

            if (AccountModel.GetUserTypeId() == AccountModel.UserTypeCashier)
            {
                model.Errors.Add("Generic", "You dont have permission...");
            }

            var reimburse = _reimburseRepo.Find(r => r.Id == model.Id);
            if (reimburse != null)
            {
                if (reimburse.IsSubmitted.HasValue && reimburse.IsSubmitted.Value == true)
                {
                    model.Errors.Add("Generic", "This reimburse has been Submitted...");
                }
            }
            else
            {
                model.Errors.Add("Generic", "Reimburse not found...");
            }


            return model.Errors;
        }

        public Dictionary<string, string> ValidationSubmit(ReimburseModel model)
        {
            if (model.Errors == null)
                model.Errors = new Dictionary<string, string>();

            // Except Employee can not submit
            if (AccountModel.GetUserTypeId() != AccountModel.UserTypeEmployee)
            {
                model.Errors.Add("Generic", "You dont have permission to submit reimburse...");
            }

            var reimburse = _reimburseRepo.Find(r => r.Id == model.Id);
            if (reimburse != null)
            {
                if (reimburse.IsSubmitted.HasValue && reimburse.IsSubmitted.Value == true)
                {
                    model.Errors.Add("Generic", "This reimburse has been Submitted...");
                }
            }
            else
            {
                model.Errors.Add("Generic", "Reimburse not found...");
            }

            return model.Errors;
        }

        public Dictionary<string, string> ValidationConfirm(ReimburseModel model)
        {
            if (model.Errors == null)
                model.Errors = new Dictionary<string, string>();

            // Except Cashier can not confirm
            if (AccountModel.GetUserTypeId() != AccountModel.UserTypeCashier)
            {
                model.Errors.Add("Generic", "You dont have permission to confirm reimburse...");
            }

            var reimburse = _reimburseRepo.Find(r => r.Id == model.Id);
            if (reimburse != null)
            {
                if (!reimburse.IsSubmitted.HasValue || (reimburse.IsSubmitted.HasValue && reimburse.IsSubmitted.Value == false))
                {
                    model.Errors.Add("Generic", "Please Submit Reimbuse First...");
                }

                if (reimburse.IsConfirmed.HasValue && reimburse.IsConfirmed.Value == true)
                {
                    model.Errors.Add("Generic", "This reimburse has been Confirmed...");
                }
            }
            else
            {
                model.Errors.Add("Generic", "Reimburse not found...");
            }

            return model.Errors;
        }

        public Dictionary<string, string> ValidationClear(ReimburseModel model)
        {
            if (model.Errors == null)
                model.Errors = new Dictionary<string, string>();

            // Except Employee can not clear
            if (AccountModel.GetUserTypeId() != AccountModel.UserTypeEmployee)
            {
                model.Errors.Add("Generic", "You dont have permission to clear reimburse...");
            }

            var reimburse = _reimburseRepo.Find(r => r.Id == model.Id);
            if (reimburse != null)
            {
                if (!reimburse.IsSubmitted.HasValue || (reimburse.IsSubmitted.HasValue && reimburse.IsSubmitted.Value == false))
                {
                    model.Errors.Add("Generic", "Please Submit Reimbuse First...");
                }

                if (!reimburse.IsConfirmed.HasValue || (reimburse.IsConfirmed.HasValue && reimburse.IsConfirmed.Value == false))
                {
                    model.Errors.Add("Generic", "Please Confirm Reimbuse First...");
                }

                if (reimburse.IsCleared.HasValue && reimburse.IsCleared.Value == true)
                {
                    model.Errors.Add("Generic", "This reimburse has been Cleared...");
                }
            }
            else
            {
                model.Errors.Add("Generic", "Reimburse not found...");
            }

            return model.Errors;
        }

        public Dictionary<string, string> ValidationRejectDetail(ReimburseModel.Detail model)
        {
            if (model.Errors == null)
                model.Errors = new Dictionary<string, string>();

            // Except Cashier can not clear
            if (AccountModel.GetUserTypeId() == AccountModel.UserTypeEmployee)
            {
                model.Errors.Add("Generic", "You dont have permission to reject...");
            }

            var reimburseDetail = _reimburseRepo.GetReimburseDetail(model.Id);
            if (reimburseDetail != null)
            {
                int reimburseId = reimburseDetail.ReimburseId.HasValue ? reimburseDetail.ReimburseId.Value : 0;
                if (!this.IsValidReimburse(reimburseId))
                {
                    model.Errors.Add("Generic", "Invalid Reimburse...");
                }

                // Unable Reject/UnReject before submiting
                if (!this.IsSubmittedReimburse(reimburseId))
                {
                    model.Errors.Add("Generic", "Unable Reject/UnReject before Submitting...");
                }

                // Unable Reject/UnReject if already Confirmed
                if (this.IsConfirmedReimburse(reimburseId))
                {
                    model.Errors.Add("Generic", "This Reimburse already Confirmed...");
                }
            }
            else
            {
                model.Errors.Add("Generic", "Reimburse Detail not found...");
            }

            return model.Errors;
        }

        public Dictionary<string, string> ValidationDetail(ReimburseModel.Detail model)
        {
            if (model.Errors == null)
                model.Errors = new Dictionary<string, string>();

            // Description
            if (String.IsNullOrEmpty(model.Description) || model.Description.Trim() == "")
            {
                model.Errors.Add("DetailDescription", "Invalid Description");
            }

            // Amount
            if (model.Amount <= 0)
            {
                model.Errors.Add("DetailAmount", "Invalid Amount...");
            }

            return model.Errors;
        }

        public Dictionary<string, string> ValidationInsertDetail(ReimburseModel.Detail model)
        {
            model.Errors = this.ValidationDetail(model);

            // Except Employee
            if (AccountModel.GetUserTypeId() != AccountModel.UserTypeEmployee)
            {
                model.Errors.Add("Generic", "You dont have permission..");
            }

            if (!this.IsValidReimburse(model.ReimburseId))
            {
                model.Errors.Add("Generic", "Invalid Reimburse Id..");
            }

            if (this.IsSubmittedReimburse(model.ReimburseId))
            {
                model.Errors.Add("Generic", "This Reimburse has been Submitted and Unable to Add or Edit more..");
            }

            return model.Errors;
        }

        public Dictionary<string, string> ValidationUpdateDetail(ReimburseModel.Detail model)
        {
            model.Errors = this.ValidationDetail(model);

            // Except Employee
            if (AccountModel.GetUserTypeId() != AccountModel.UserTypeEmployee)
            {
                model.Errors.Add("Generic", "You dont have permission..");
            }

            if (!this.IsValidReimburse(model.ReimburseId))
            {
                model.Errors.Add("Generic", "Invalid Reimburse Id..");
            }

            if (this.IsSubmittedReimburse(model.ReimburseId))
            {
                model.Errors.Add("Generic", "This Reimburse has been Submitted and Unable to Add or Edit more..");
            }

            return model.Errors;
        }

        public Dictionary<string, string> ValidationDeleteDetail(ReimburseModel.Detail model)
        {
            model.Errors = this.ValidationDetail(model);

            // Except Employee
            if (AccountModel.GetUserTypeId() != AccountModel.UserTypeEmployee)
            {
                model.Errors.Add("Generic", "You dont have permission..");
            }

            if (!this.IsValidReimburse(model.ReimburseId))
            {
                model.Errors.Add("Generic", "Invalid Reimburse Id..");
            }

            if (this.IsSubmittedReimburse(model.ReimburseId))
            {
                model.Errors.Add("Generic", "This Reimburse has been Submitted and Unable to Add or Edit more..");
            }

            return model.Errors;
        }

        public ReimburseModel.Detail InsertReimburseDetail(ReimburseModel.Detail reimburseDetail)
        {
            // Construct Error
            reimburseDetail.Errors = new Dictionary<string, string>();
            try
            {
                reimburseDetail.Errors = this.ValidationInsertDetail(reimburseDetail);
                if (reimburseDetail.Errors != null && reimburseDetail.Errors.Count > 0)
                {
                    return reimburseDetail;
                }

                ReimburseDetail newReimburseDetail = new ReimburseDetail();
                newReimburseDetail.Description = reimburseDetail.Description;
                newReimburseDetail.Amount = reimburseDetail.Amount;
                newReimburseDetail.CreatedDate = DateTime.Today;
                newReimburseDetail.ExpenseDate = reimburseDetail.ExpenseDate;
                newReimburseDetail.IsRejected = reimburseDetail.IsRejected;
                newReimburseDetail.ReimburseId = reimburseDetail.ReimburseId;

                newReimburseDetail = _reimburseRepo.InsertReimburseDetail(newReimburseDetail);
                reimburseDetail.Id = newReimburseDetail.Id;

                // Update Total and Actual Paid on Reimburse table
                this.ReCalculateTotal(reimburseDetail.ReimburseId);

                LOG.Info("InsertReimburseDetail Success");
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    LOG.ErrorFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        validationErrors.Entry.Entity.GetType().Name, validationErrors.Entry.State);
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        //Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        LOG.ErrorFormat("InsertReimburseDetail, Error:Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }

                LOG.Error("InsertReimburseDetail Failed", dbEx);
                reimburseDetail.Errors.Add("Generic", "Insert Reimburse Detail failed, Please try again or contact your administrator.");
            }
            catch (Exception ex)
            {
                LOG.Error("InsertReimburseDetail Failed", ex);
                reimburseDetail.Errors.Add("Generic", "Insert Reimburse Detail Failed, Please try again or contact your administrator.");
            }

            return reimburseDetail;
        }

        public ReimburseModel.Detail UpdateReimburseDetail(ReimburseModel.Detail reimburseDetail)
        {
            reimburseDetail.Errors = new Dictionary<string, string>();
            try
            {
                reimburseDetail.Errors = this.ValidationUpdateDetail(reimburseDetail);
                if (reimburseDetail.Errors != null && reimburseDetail.Errors.Count > 0)
                {
                    return reimburseDetail;
                }

                ReimburseDetail updateReimburseDetail = _reimburseRepo.GetReimburseDetail(reimburseDetail.Id);
                if (updateReimburseDetail != null)
                {
                    updateReimburseDetail.Description = reimburseDetail.Description;
                    updateReimburseDetail.Amount = reimburseDetail.Amount;
                    updateReimburseDetail.ExpenseDate = reimburseDetail.ExpenseDate;
                    updateReimburseDetail.IsRejected = reimburseDetail.IsRejected;

                    _reimburseRepo.UpdateReimburseDetail(updateReimburseDetail);

                    // Update Total and Actual Paid on Reimburse table
                    this.ReCalculateTotal(reimburseDetail.ReimburseId);

                    LOG.Info("UpdateReimburseDetail Success");
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    LOG.ErrorFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        validationErrors.Entry.Entity.GetType().Name, validationErrors.Entry.State);
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        //Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        LOG.ErrorFormat("UpdateReimburseDetail, Error:Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }

                LOG.Error("UpdateReimburseDetail Failed", dbEx);
                reimburseDetail.Errors.Add("Generic", "Update Reimburse Detail failed, Please try again or contact your administrator.");
            }
            catch (Exception ex)
            {
                LOG.Error("UpdateReimburseDetail Failed", ex);
                reimburseDetail.Errors.Add("Generic", "Update Reimburse Detail Failed, Please try again or contact your administrator.");
            }

            return reimburseDetail;
        }

        public ReimburseModel.Detail DeleteReimburseDetail(ReimburseModel.Detail reimburseDetail)
        {
            reimburseDetail.Errors = new Dictionary<string, string>();
            try
            {
                reimburseDetail.Errors = this.ValidationDeleteDetail(reimburseDetail);
                if (reimburseDetail.Errors != null && reimburseDetail.Errors.Count > 0)
                {
                    return reimburseDetail;
                }

                ReimburseDetail detail = _reimburseRepo.GetReimburseDetail(reimburseDetail.Id);

                if (detail != null)
                {
                    _reimburseRepo.DeleteReimburseDetail(detail.Id);

                    // Update Total and Actual Paid on Reimburse table
                    int reimburseId = detail.ReimburseId.HasValue ? detail.ReimburseId.Value : 0;
                    this.ReCalculateTotal(reimburseId);
                }

                LOG.Info("DeleteReimburseDetail Success");
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    LOG.ErrorFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        validationErrors.Entry.Entity.GetType().Name, validationErrors.Entry.State);
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        //Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        LOG.ErrorFormat("DeleteInvoiceDetail, Error:Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                LOG.Error("DeleteReimburseDetail Failed", dbEx);
                reimburseDetail.Errors.Add("Generic", "Delete Data Failed... ErrorMessage:" + dbEx.Message);
            }
            catch (Exception ex)
            {
                LOG.Error("DeleteReimburseDetail Failed", ex);
                reimburseDetail.Errors.Add("Generic", "Delete Data Failed... ErrorMessage:" + ex.Message);
            }

            return reimburseDetail;
        }

        // Reject Reimburse Detail
        public ReimburseModel.Detail RejectReimburseDetail(ReimburseModel.Detail reimburseDetail)
        {
            reimburseDetail.Errors = new Dictionary<string, string>();
            try
            {
                reimburseDetail.Errors = this.ValidationRejectDetail(reimburseDetail);
                if (reimburseDetail.Errors != null && reimburseDetail.Errors.Count > 0)
                {
                    return reimburseDetail;
                }

                ReimburseDetail rejectReimburse = _reimburseRepo.GetReimburseDetail(reimburseDetail.Id);
                if (rejectReimburse != null)
                {
                    rejectReimburse.IsRejected = reimburseDetail.IsRejected;

                    _reimburseRepo.UpdateReimburseDetail(rejectReimburse);

                    // Recalculate
                    int reimburseId = rejectReimburse.ReimburseId.HasValue ? rejectReimburse.ReimburseId.Value : 0;
                    this.ReCalculateTotal(reimburseId);

                    LOG.Info("RejectReimburseDetail Success");
                }
                else
                {
                    reimburseDetail.Errors.Add("Generic", "Reimburse not found...");
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    LOG.ErrorFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        validationErrors.Entry.Entity.GetType().Name, validationErrors.Entry.State);
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        //Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        LOG.ErrorFormat("RejectReimburseDetail, Error:Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }

                LOG.Error("RejectReimburseDetail Failed", dbEx);
                reimburseDetail.Errors.Add("Generic", "Reject data failed, Please try again or contact your administrator.");
            }
            catch (Exception ex)
            {
                LOG.Error("RejectReimburseDetail Failed", ex);
                reimburseDetail.Errors.Add("Generic", "Reject data Failed, Please try again or contact your administrator.");
            }

            return reimburseDetail;
        }

        private void ReCalculateTotal(int reimburseId)
        {
            var reimburse = _reimburseRepo.Find(r => r.Id == reimburseId);
            if (reimburse != null)
            {
                decimal total = 0;
                decimal actualPaid = 0;
                var reimbruseDetails = _reimburseRepo.GetReimburseDetailList(reimburseId);
                foreach (var item in reimbruseDetails)
                {
                    total += item.Amount.HasValue ? item.Amount.Value : 0;
                    if (!item.IsRejected.HasValue || (item.IsRejected.HasValue && item.IsRejected.Value == false))
                    {
                        actualPaid += item.Amount.HasValue ? item.Amount.Value : 0;
                    }
                }

                reimburse.Total = total;
                reimburse.ActualPaid = actualPaid;

                _reimburseRepo.UpdateReimburse(reimburse);
            }
        }

        private bool IsSubmittedReimburse(int reimburseId)
        {
            var reimburse = _reimburseRepo.Find(r => r.Id == reimburseId);
            if (reimburse != null)
            {
                if (reimburse.IsSubmitted.HasValue && reimburse.IsSubmitted.Value == true)
                    return true;
            }

            return false;
        }

        private bool IsConfirmedReimburse(int reimburseId)
        {
            var reimburse = _reimburseRepo.Find(r => r.Id == reimburseId);
            if (reimburse != null)
            {
                if (reimburse.IsConfirmed.HasValue && reimburse.IsConfirmed.Value == true)
                    return true;
            }

            return false;
        }

        private bool IsClearedReimburse(int reimburseId)
        {
            var reimburse = _reimburseRepo.Find(r => r.Id == reimburseId);
            if (reimburse != null)
            {
                if (reimburse.IsCleared.HasValue && reimburse.IsCleared.Value == true)
                    return true;
            }

            return false;
        }

        private bool IsValidReimburse(int reimburseId)
        {
            var reimburse = _reimburseRepo.Find(r => r.Id == reimburseId);
            if (reimburse != null)
            {
                return true;
            }

            return false;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _reimburseRepo.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}