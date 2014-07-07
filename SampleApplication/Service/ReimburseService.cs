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
    public class ReimburseService : IReimburseService, IDisposable
    {
        private readonly static log4net.ILog LOG = log4net.LogManager.GetLogger("ReimburseService");
        private readonly IReimburseRepository _reimburseRepo;

        public ReimburseService()
        {
            _reimburseRepo = new ReimburseRepository();
        }

        // Invoice List
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

        // Reimburse Info
        public ResponseModel GetReimburseInfo(int reimburseId)
        {
            ResponseModel respModel = new ResponseModel();
            respModel.isValid = true;
            respModel.message = "OK";
            respModel.objResult = null;
            try
            {
                var reimburse = _reimburseRepo.Find(r => r.Id == reimburseId);
                if (reimburse != null)
                {
                    ReimburseModel model = new ReimburseModel();
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

                    respModel.isValid = true;
                    respModel.message = "OK";
                    respModel.objResult = model;
                }
                else
                {
                    respModel.isValid = false;
                    respModel.message = "Reimburse not found";
                }
            }
            catch (Exception ex)
            {
                LOG.Error("GetReimburseInfo Failed", ex);
                respModel.isValid = false;
                respModel.message = "There is error when load this invoice..";
            }

            return respModel;
        }

        // Insert Reimburse
        public ResponseModel InsertReimburse(ReimburseModel reimburse)
        {
            ResponseModel respModel = new ResponseModel();
            respModel.isValid = true;
            respModel.message = "OK";
            respModel.objResult = null;
            try
            {
                string message = "";
                respModel.isValid = this.ValidationInsert(reimburse, out message);
                if (!respModel.isValid)
                {
                    respModel.message = message;
                    return respModel;
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

                respModel.isValid = true;
                respModel.message = "Insert Data Success...";
                respModel.objResult = reimburse;

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
                respModel.isValid = false;
                respModel.message = "Insert Reimburse failed, Please try again or contact your administrator.";
            }
            catch (Exception ex)
            {
                LOG.Error("InsertReimburse Failed", ex);
                respModel.isValid = false;
                respModel.message = "Insert Reimburse Failed, Please try again or contact your administrator.";
            }

            return respModel;
        }

        // Delete Reimburse
        public ResponseModel DeleteReimburse(int reimburseId)
        {
            ResponseModel respModel = new ResponseModel();
            respModel.isValid = true;
            respModel.message = "OK";
            respModel.objResult = null;
            try
            {
                Reimburse deleteReimburse = _reimburseRepo.Find(p => p.Id == reimburseId);
                if (deleteReimburse != null)
                {
                    // Delete Reimburse Detail
                    _reimburseRepo.DeleteReimburseDetail(reimburseId);

                    // Delete Reimburse
                    _reimburseRepo.DeleteReimburse(reimburseId);

                    respModel.isValid = true;
                    respModel.message = "Delete Reimburse Success...";
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
                respModel.isValid = false;
                respModel.message = "Delete Reimburse failed, Please try again or contact your administrator.";
            }
            catch (Exception ex)
            {
                LOG.Error("DeleteReimburse Failed", ex);
                respModel.isValid = false;
                respModel.message = "Delete Reimburse Failed, Please try again or contact your administrator.";
            }

            return respModel;
        }

        // Update Reimburse
        public ResponseModel UpdateReimburse(ReimburseModel reimburse)
        {
            ResponseModel respModel = new ResponseModel();
            respModel.isValid = true;
            respModel.message = "OK";
            respModel.objResult = null;
            try
            {
                string message = "";
                respModel.isValid = this.ValidationUpdate(reimburse, out message);
                if (!respModel.isValid)
                {
                    respModel.message = message;
                    return respModel;
                }

                Reimburse updateReimburse = _reimburseRepo.Find(p => p.Id == reimburse.Id);
                if (updateReimburse != null)
                {
                    updateReimburse.Description = reimburse.Description;
                    updateReimburse.ActualPaid = reimburse.ActualPaid;
                    updateReimburse.Total = reimburse.Total;

                    _reimburseRepo.UpdateReimburse(updateReimburse);

                    respModel.isValid = true;
                    respModel.message = "Update Data Success...";
                    respModel.objResult = reimburse;

                    LOG.Info("UpdateReimburse Success");
                }
                else
                {
                    respModel.isValid = false;
                    respModel.message = "Reimburse not found...";
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
                respModel.isValid = false;
                respModel.message = "Update data failed, Please try again or contact your administrator.";
            }
            catch (Exception ex)
            {
                LOG.Error("UpdateReimburse Failed", ex);
                respModel.isValid = false;
                respModel.message = "Update data Failed, Please try again or contact your administrator.";
            }

            return respModel;
        }

        // Submit Reimburse
        public ResponseModel SubmitReimburse(ReimburseModel reimburse)
        {
            ResponseModel respModel = new ResponseModel();
            respModel.isValid = true;
            respModel.message = "OK";
            respModel.objResult = null;
            try
            {
                string message = "";
                respModel.isValid = this.ValidationSubmit(reimburse, out message);
                if (!respModel.isValid)
                {
                    respModel.message = message;
                    return respModel;
                }

                Reimburse submitReimburse = _reimburseRepo.Find(p => p.Id == reimburse.Id);
                if (submitReimburse != null)
                {
                    submitReimburse.IsSubmitted = true;
                    submitReimburse.SubmittedDate = reimburse.SubmittedDate;

                    _reimburseRepo.UpdateReimburse(submitReimburse);

                    respModel.isValid = true;
                    respModel.message = "Submit Data Success...";
                    respModel.objResult = reimburse;

                    LOG.Info("SubmitReimburse Success");
                }
                else
                {
                    respModel.isValid = false;
                    respModel.message = "Reimburse not found...";
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
                respModel.isValid = false;
                respModel.message = "Submit data failed, Please try again or contact your administrator.";
            }
            catch (Exception ex)
            {
                LOG.Error("SubmitReimburse Failed", ex);
                respModel.isValid = false;
                respModel.message = "Update Submit Failed, Please try again or contact your administrator.";
            }

            return respModel;
        }

        // Confirm Reimburse
        public ResponseModel ConfirmReimburse(ReimburseModel reimburse)
        {
            ResponseModel respModel = new ResponseModel();
            respModel.isValid = true;
            respModel.message = "OK";
            respModel.objResult = null;
            try
            {
                string message = "";
                respModel.isValid = this.ValidationConfirm(reimburse, out message);
                if (!respModel.isValid)
                {
                    respModel.message = message;
                    return respModel;
                }

                Reimburse confirmReimburse = _reimburseRepo.Find(p => p.Id == reimburse.Id);
                if (confirmReimburse != null)
                {
                    confirmReimburse.ConfirmedDate = reimburse.ConfirmedDate;
                    confirmReimburse.IsConfirmed = true;

                    _reimburseRepo.UpdateReimburse(confirmReimburse);

                    respModel.isValid = true;
                    respModel.message = "Confirm Data Success...";
                    respModel.objResult = reimburse;

                    LOG.Info("ConfirmReimburse Success");
                }
                else
                {
                    respModel.isValid = false;
                    respModel.message = "Reimburse not found...";
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
                respModel.isValid = false;
                respModel.message = "Confirm data failed, Please try again or contact your administrator.";
            }
            catch (Exception ex)
            {
                LOG.Error("ConfirmReimburse Failed", ex);
                respModel.isValid = false;
                respModel.message = "Confirm data Failed, Please try again or contact your administrator.";
            }

            return respModel;
        }

        // Clear Reimburse
        public ResponseModel ClearReimburse(ReimburseModel reimburse)
        {
            ResponseModel respModel = new ResponseModel();
            respModel.isValid = true;
            respModel.message = "OK";
            respModel.objResult = null;
            try
            {
                string message = "";
                respModel.isValid = this.ValidationClear(reimburse, out message);
                if (!respModel.isValid)
                {
                    respModel.message = message;
                    return respModel;
                }

                Reimburse clearReimburse = _reimburseRepo.Find(p => p.Id == reimburse.Id);
                if (clearReimburse != null)
                {
                    clearReimburse.ClearanceDate = reimburse.ClearanceDate;
                    clearReimburse.IsCleared = true;

                    _reimburseRepo.UpdateReimburse(clearReimburse);

                    respModel.isValid = true;
                    respModel.message = "Clear Data Success...";
                    respModel.objResult = reimburse;

                    LOG.Info("ClearReimburse Success");
                }
                else
                {
                    respModel.isValid = false;
                    respModel.message = "Reimburse not found...";
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
                respModel.isValid = false;
                respModel.message = "Clear data failed, Please try again or contact your administrator.";
            }
            catch (Exception ex)
            {
                LOG.Error("ClearReimburse Failed", ex);
                respModel.isValid = false;
                respModel.message = "Clear data Failed, Please try again or contact your administrator.";
            }

            return respModel;
        }

        public bool Validation(ReimburseModel model, out string message)
        {
            bool isValid = true;
            message = "OK";

            // Description
            if (String.IsNullOrEmpty(model.Description) || model.Description.Trim() == "")
            {
                message = "Invalid Description...";
                return false;
            }

            // Total
            if (model.Total <= 0)
            {
                message = "Invalid Total...";
                return false;
            }

            return isValid;
        }

        public bool ValidationInsert(ReimburseModel model, out string message)
        {
            bool isValid = this.Validation(model, out message);

            if (AccountModel.GetUserTypeId() == AccountModel.UserTypeCashier)
            {
                isValid = false;
                message = "You dont have permission...";
            }

            return isValid;
        }

        public bool ValidationUpdate(ReimburseModel model, out string message)
        {
            bool isValid = this.Validation(model, out message);

            if (AccountModel.GetUserTypeId() == AccountModel.UserTypeCashier)
            {
                isValid = false;
                message = "You dont have permission...";
            }

            var reimburse = _reimburseRepo.Find(r => r.Id == model.Id);
            if (reimburse != null)
            {
                if (reimburse.IsSubmitted.HasValue && reimburse.IsSubmitted.Value == true)
                {
                    message = "This reimburse has been Submitted...";
                    isValid = false;
                    return isValid;
                }
            }
            else
            {
                isValid = false;
                message = "Reimburse not found...";
            }


            return isValid;
        }

        public bool ValidationSubmit(ReimburseModel model, out string message)
        {
            message = "";
            bool isValid = true;

            // Except Employee can not submit
            if (AccountModel.GetUserTypeId() != AccountModel.UserTypeEmployee)
            {
                message = "You dont have permission to submit reimburse...";
                isValid = false;
                return isValid;
            }

            var reimburse = _reimburseRepo.Find(r => r.Id == model.Id);
            if (reimburse != null)
            {
                if (reimburse.IsSubmitted.HasValue && reimburse.IsSubmitted.Value == true)
                {
                    message = "This reimburse has been Submitted...";
                    isValid = false;
                    return isValid;
                }
            }
            else
            {
                isValid = false;
                message = "Reimburse not found...";
            }

            return isValid;
        }

        public bool ValidationConfirm(ReimburseModel model, out string message)
        {
            message = "";
            bool isValid = true;

            // Except Cashier can not confirm
            if (AccountModel.GetUserTypeId() != AccountModel.UserTypeCashier)
            {
                message = "You dont have permission to confirm reimburse...";
                isValid = false;
                return isValid;
            }

            var reimburse = _reimburseRepo.Find(r => r.Id == model.Id);
            if (reimburse != null)
            {
                if (!reimburse.IsSubmitted.HasValue || (reimburse.IsSubmitted.HasValue && reimburse.IsSubmitted.Value == false))
                {
                    message = "Please Submit Reimbuse First...";
                    isValid = false;
                    return isValid;
                }

                if (reimburse.IsConfirmed.HasValue && reimburse.IsConfirmed.Value == true)
                {
                    message = "This reimburse has been Confirmed...";
                    isValid = false;
                    return isValid;
                }
            }
            else
            {
                isValid = false;
                message = "Reimburse not found...";
            }

            return isValid;
        }

        public bool ValidationClear(ReimburseModel model, out string message)
        {
            message = "";
            bool isValid = true;

            // Except Employee can not clear
            if (AccountModel.GetUserTypeId() != AccountModel.UserTypeEmployee)
            {
                message = "You dont have permission to clear reimburse...";
                isValid = false;
                return isValid;
            }

            var reimburse = _reimburseRepo.Find(r => r.Id == model.Id);
            if (reimburse != null)
            {
                if (!reimburse.IsSubmitted.HasValue || (reimburse.IsSubmitted.HasValue && reimburse.IsSubmitted.Value == false))
                {
                    message = "Please Submit Reimbuse First...";
                    isValid = false;
                    return isValid;
                }

                if (!reimburse.IsConfirmed.HasValue || (reimburse.IsConfirmed.HasValue && reimburse.IsConfirmed.Value == false))
                {
                    message = "Please Confirm Reimbuse First...";
                    isValid = false;
                    return isValid;
                }

                if (reimburse.IsCleared.HasValue && reimburse.IsCleared.Value == true)
                {
                    message = "This reimburse has been Cleared...";
                    isValid = false;
                    return isValid;
                }
            }
            else
            {
                isValid = false;
                message = "Reimburse not found...";
            }

            return isValid;
        }

        public bool ValidationRejectDetail(ReimburseModel.Detail model, out string message)
        {
            message = "";
            bool isValid = true;

            // Except Cashier can not clear
            if (AccountModel.GetUserTypeId() == AccountModel.UserTypeEmployee)
            {
                message = "You dont have permission to reject...";
                isValid = false;
                return isValid;
            }

            var reimburseDetail = _reimburseRepo.GetReimburseDetail(model.Id);
            if (reimburseDetail != null)
            {
                int reimburseId = reimburseDetail.ReimburseId.HasValue ? reimburseDetail.ReimburseId.Value : 0;
                if (!this.IsValidReimburse(reimburseId))
                {
                    message = "Invalid Reimburse...";
                    isValid = false;
                    return isValid;
                }

                // Unable Reject/UnReject before submiting
                if (!this.IsSubmittedReimburse(reimburseId))
                {
                    message = "Unable Reject/UnReject before Submitting...";
                    isValid = false;
                    return isValid;
                }

                // Unable Reject/UnReject if already Confirmed
                if (this.IsConfirmedReimburse(reimburseId))
                {
                    message = "This Reimburse already Confirmed...";
                    isValid = false;
                    return isValid;
                }
            }
            else
            {
                isValid = false;
                message = "Reimburse Detail not found...";
            }

            return isValid;
        }

        public ResponseModel InsertReimburseDetail(ReimburseModel.Detail reimburseDetail)
        {
            ResponseModel respModel = new ResponseModel();
            respModel.isValid = true;
            respModel.message = "OK";
            respModel.objResult = null;
            try
            {
                // Except Employee
                if (AccountModel.GetUserTypeId() != AccountModel.UserTypeEmployee)
                {
                    respModel.isValid = false;
                    respModel.message = "You dont have permission..";
                    return respModel;
                }

                if (!this.IsValidReimburse(reimburseDetail.ReimburseId))
                {
                    respModel.isValid = false;
                    respModel.message = "Invalid Reimburse Id..";
                    return respModel;
                }

                if (this.IsSubmittedReimburse(reimburseDetail.ReimburseId))
                {
                    respModel.isValid = false;
                    respModel.message = "This Reimburse has been Submitted and Unable to Add or Edit more..";
                    return respModel;
                }

                ReimburseDetail newReimburseDetail = new ReimburseDetail();
                newReimburseDetail.Description = reimburseDetail.Description;
                newReimburseDetail.Amount = reimburseDetail.Amount;
                newReimburseDetail.CreatedDate = DateTime.Today;
                newReimburseDetail.ExpenseDate = reimburseDetail.ExpenseDate;
                newReimburseDetail.IsRejected = reimburseDetail.IsRejected;
                newReimburseDetail.ReimburseId = reimburseDetail.ReimburseId;

                newReimburseDetail = _reimburseRepo.InsertReimburseDetail(newReimburseDetail);

                // Update Total and Actual Paid on Reimburse table
                this.ReCalculateTotal(reimburseDetail.ReimburseId);

                respModel.isValid = true;
                respModel.message = "Insert Reimburse Detail Success...";
                respModel.objResult = reimburseDetail;

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
                respModel.isValid = false;
                respModel.message = "Insert Reimburse Detail failed, Please try again or contact your administrator.";
            }
            catch (Exception ex)
            {
                LOG.Error("InsertReimburseDetail Failed", ex);
                respModel.isValid = false;
                respModel.message = "Insert Reimburse Detail Failed, Please try again or contact your administrator.";
            }

            return respModel;
        }

        public ResponseModel UpdateReimburseDetail(ReimburseModel.Detail reimburseDetail)
        {
            ResponseModel respModel = new ResponseModel();
            respModel.isValid = true;
            respModel.message = "OK";
            respModel.objResult = null;
            try
            {
                // Except Employee
                if (AccountModel.GetUserTypeId() != AccountModel.UserTypeEmployee)
                {
                    respModel.isValid = false;
                    respModel.message = "You dont have permission..";
                    return respModel;
                }

                if (!this.IsValidReimburse(reimburseDetail.ReimburseId))
                {
                    respModel.isValid = false;
                    respModel.message = "Invalid Reimburse Id..";
                    return respModel;
                }

                if (this.IsSubmittedReimburse(reimburseDetail.ReimburseId))
                {
                    respModel.isValid = false;
                    respModel.message = "This Reimburse has been Submitted and Unable to Add or Edit more..";
                    return respModel;
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

                    respModel.isValid = true;
                    respModel.message = "Update Reimburse Detail Success...";
                    respModel.objResult = reimburseDetail;

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
                respModel.isValid = false;
                respModel.message = "Update Reimburse Detail failed, Please try again or contact your administrator.";
            }
            catch (Exception ex)
            {
                LOG.Error("UpdateReimburseDetail Failed", ex);
                respModel.isValid = false;
                respModel.message = "Update Reimburse Detail Failed, Please try again or contact your administrator.";
            }

            return respModel;
        }

        public ResponseModel DeleteReimburseDetail(int reimburseDetailId)
        {
            ResponseModel respModel = new ResponseModel();
            respModel.objResult = null;
            try
            {
                // Except Employee
                if (AccountModel.GetUserTypeId() != AccountModel.UserTypeEmployee)
                {
                    respModel.isValid = false;
                    respModel.message = "You dont have permission..";
                    return respModel;
                }

                ReimburseDetail detail = _reimburseRepo.GetReimburseDetail(reimburseDetailId);

                if (detail != null)
                {
                    int reimburseId = detail.ReimburseId.HasValue ? detail.ReimburseId.Value : 0;
                    if (!this.IsValidReimburse(reimburseId))
                    {
                        respModel.isValid = false;
                        respModel.message = "Invalid Reimburse Id..";
                        return respModel;
                    }

                    if (this.IsSubmittedReimburse(reimburseId))
                    {
                        respModel.isValid = false;
                        respModel.message = "This Reimburse has been Submitted and Unable to Add or Edit more..";
                        return respModel;
                    }
                }

                _reimburseRepo.DeleteReimburseDetail(reimburseDetailId);

                if (detail != null)
                {
                    int reimburseId = detail.ReimburseId.HasValue ? detail.ReimburseId.Value : 0;
                    // Update Total and Actual Paid on Reimburse table
                    this.ReCalculateTotal(reimburseId);
                }

                respModel.isValid = true;
                respModel.message = "Delete Data Success...";

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
                respModel.isValid = false;
                respModel.message = "Delete Data Failed... ErrorMessage:" + dbEx.Message;
            }
            catch (Exception ex)
            {
                LOG.Error("DeleteReimburseDetail Failed", ex);
                respModel.isValid = false;
                respModel.message = "Delete Data Failed... ErrorMessage:" + ex.Message;
            }

            return respModel;
        }

        // Reject Reimburse Detail
        public ResponseModel RejectReimburseDetail(ReimburseModel.Detail reimburseDetail)
        {
            ResponseModel respModel = new ResponseModel();
            respModel.isValid = true;
            respModel.message = "OK";
            respModel.objResult = null;
            try
            {
                string message = "";
                respModel.isValid = this.ValidationRejectDetail(reimburseDetail, out message);
                if (!respModel.isValid)
                {
                    respModel.message = message;
                    return respModel;
                }

                ReimburseDetail rejectReimburse = _reimburseRepo.GetReimburseDetail(reimburseDetail.Id);
                if (rejectReimburse != null)
                {
                    rejectReimburse.IsRejected = reimburseDetail.IsRejected;

                    _reimburseRepo.UpdateReimburseDetail(rejectReimburse);

                    // Recalculate
                    int reimburseId = rejectReimburse.ReimburseId.HasValue ? rejectReimburse.ReimburseId.Value : 0;
                    this.ReCalculateTotal(reimburseId);

                    respModel.isValid = true;
                    respModel.message = "Reject/UnReject Data Success...";

                    LOG.Info("RejectReimburseDetail Success");
                }
                else
                {
                    respModel.isValid = false;
                    respModel.message = "Reimburse not found...";
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
                respModel.isValid = false;
                respModel.message = "Reject data failed, Please try again or contact your administrator.";
            }
            catch (Exception ex)
            {
                LOG.Error("RejectReimburseDetail Failed", ex);
                respModel.isValid = false;
                respModel.message = "Reject data Failed, Please try again or contact your administrator.";
            }

            return respModel;
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