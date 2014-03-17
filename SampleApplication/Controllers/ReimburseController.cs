using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SampleApplication.Models;
using SampleApplication.Service;

namespace SampleApplication.Controllers
{
    public class ReimburseController : Controller
    {
        private readonly static log4net.ILog LOG = log4net.LogManager.GetLogger("ReimburseController");
        private IReimburseService _reimburseService;

        public ReimburseController()
        {
            _reimburseService = new ReimburseService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public dynamic GetListReimburse(string _search, long nd, int rows, int? page, string sidx, string sord, string filters = "")
        {
            // Construct where statement
            string strWhere = GeneralFunction.ConstructWhere(filters);

            // Get Data
            var reimburse = _reimburseService.GetReimburseList(sidx, sord, strWhere) as IEnumerable<ReimburseModel.List>;
            var pageIndex = Convert.ToInt32(page) - 1;
            var pageSize = rows;
            var totalRecords = reimburse.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            // default last page
            if (totalPages > 0)
            {
                if (!page.HasValue)
                {
                    pageIndex = totalPages - 1;
                    page = totalPages;
                }
            }

            reimburse = reimburse.Skip(pageIndex * pageSize).Take(pageSize);

            return Json(new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from item in reimburse
                    select new
                    {
                        id = item.Id,
                        cell = new object[] {
                            item.Name,
                            item.Description,
                            item.Total,
                            item.IsSubmitted,
                            item.SubmittedDate,
                            item.IsConfimed,
                            item.ConfirmedDate,
                            item.IsClearaned,
                            item.ClearanceDate,
                            item.ActualPaid,
                            item.CreatedDate
                      }
                    }).ToArray()
            }, JsonRequestBehavior.AllowGet);
        }

        public dynamic GetReimburseInfo(int Id)
        {
            bool isValid = true;
            string message = "OK";
            object objJob = null;
            try
            {
                ResponseModel respModel = _reimburseService.GetReimburseInfo(Id);
                objJob = respModel.objResult;
                isValid = respModel.isValid;
                message = respModel.message;
            }
            catch (Exception ex)
            {
                LOG.Error("GetReimburseInfo Failed", ex);
                isValid = false;
                message = "UnKnown Error.. ErrorMessage:" + ex.Message;
            }

            return Json(new
            {
                isValid,
                message,
                objJob
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public dynamic Insert(ReimburseModel model)
        {
            string message = "OK";
            bool isValid = true;
            object objResult = null;
            try
            {                
                ResponseModel respModel = _reimburseService.InsertReimburse(model);
                isValid = respModel.isValid;
                message = respModel.message;
                objResult = respModel.objResult;
                if (!respModel.isValid)
                {
                    return Json(new
                    {
                        isValid,
                        message,
                        objResult
                    });
                }
                
                LOG.Info("InsertReimburse Success");
            }
            catch (Exception ex)
            {
                LOG.Error("InsertReimburse Failed", ex);
                isValid = false;
                message = "Insert data failed, Please try again or contact your administrator. ErrorMessage:" + ex.Message;
            }

            return Json(new
            {
                isValid,
                message,
                objResult
            });
        }

        [HttpPost]
        public dynamic Update(ReimburseModel model)
        {
            string message = "OK";
            bool isValid = true;
            object objResult = null;
            try
            {
                ResponseModel respModel = _reimburseService.UpdateReimburse(model);
                isValid = respModel.isValid;
                message = respModel.message;
                objResult = respModel.objResult;
                if (!respModel.isValid)
                {
                    return Json(new
                    {
                        isValid,
                        message,
                        objResult
                    });
                }


                LOG.Info("UpdateReimburse Success");
            }
            catch (Exception ex)
            {
                LOG.Error("UpdateReimburse Failed", ex);
                isValid = false;
                message = "Update data failed, Please try again or contact your administrator. ErrorMessage:" + ex.Message;
            }

            return Json(new
            {
                isValid,
                message,
                objResult
            });
        }

        [HttpPost]
        public dynamic Delete(int Id)
        {
            string message = "OK";
            bool isValid = true;
            object objResult = null;
            try
            {
                ResponseModel respModel = _reimburseService.DeleteReimburse(Id);
                isValid = respModel.isValid;
                message = respModel.message;
                objResult = respModel.objResult;
                if (!respModel.isValid)
                {
                    return Json(new
                    {
                        isValid,
                        message,
                        objResult
                    });
                }


                LOG.Info("DeleteReimburse Success");
            }
            catch (Exception ex)
            {
                LOG.Error("DeleteReimburse Failed", ex);
                isValid = false;
                message = "Delete data failed, Please try again or contact your administrator. ErrorMessage:" + ex.Message;
            }

            return Json(new
            {
                isValid,
                message
            });
        }

        [HttpPost]
        public dynamic Submit(ReimburseModel model)
        {
            string message = "OK";
            bool isValid = true;
            object objResult = null;
            try
            {
                ResponseModel respModel = _reimburseService.SubmitReimburse(model);
                isValid = respModel.isValid;
                message = respModel.message;
                objResult = respModel.objResult;
                if (!respModel.isValid)
                {
                    return Json(new
                    {
                        isValid,
                        message,
                        objResult
                    });
                }


                LOG.Info("SubmitReimburse Success");
            }
            catch (Exception ex)
            {
                LOG.Error("SubmitReimburse Failed", ex);
                isValid = false;
                message = "Submit data failed, Please try again or contact your administrator. ErrorMessage:" + ex.Message;
            }

            return Json(new
            {
                isValid,
                message,
                objResult
            });
        }

        [HttpPost]
        public dynamic Confirm(ReimburseModel model)
        {
            string message = "OK";
            bool isValid = true;
            object objResult = null;
            try
            {
                ResponseModel respModel = _reimburseService.ConfirmReimburse(model);
                isValid = respModel.isValid;
                message = respModel.message;
                objResult = respModel.objResult;
                if (!respModel.isValid)
                {
                    return Json(new
                    {
                        isValid,
                        message,
                        objResult
                    });
                }


                LOG.Info("ConfirmReimburse Success");
            }
            catch (Exception ex)
            {
                LOG.Error("ConfirmReimburse Failed", ex);
                isValid = false;
                message = "Confirm data failed, Please try again or contact your administrator. ErrorMessage:" + ex.Message;
            }

            return Json(new
            {
                isValid,
                message,
                objResult
            });
        }

        [HttpPost]
        public dynamic Clear(ReimburseModel model)
        {
            string message = "OK";
            bool isValid = true;
            object objResult = null;
            try
            {
                ResponseModel respModel = _reimburseService.ClearReimburse(model);
                isValid = respModel.isValid;
                message = respModel.message;
                objResult = respModel.objResult;
                if (!respModel.isValid)
                {
                    return Json(new
                    {
                        isValid,
                        message,
                        objResult
                    });
                }


                LOG.Info("ClearReimburse Success");
            }
            catch (Exception ex)
            {
                LOG.Error("ClearReimburse Failed", ex);
                isValid = false;
                message = "Clear data failed, Please try again or contact your administrator. ErrorMessage:" + ex.Message;
            }

            return Json(new
            {
                isValid,
                message,
                objResult
            });
        }

        [HttpPost]
        public dynamic DeleteReimburseDetail(int Id)
        {
            string message = "OK";
            bool isValid = true;
            try
            {
                ResponseModel respModel = _reimburseService.DeleteReimburseDetail(Id);
                isValid = respModel.isValid;
                message = respModel.message;

                if (isValid)
                {
                    message = "Delete Data Success...";
                    LOG.Info("DeleteReimburseDetail Success");
                }
            }
            catch (Exception ex)
            {
                LOG.Error("DeleteReimburseDetail Failed", ex);
                isValid = false;
                message = "Delete data failed, Please try again or contact your administrator.";
            }
            return Json(new
            {
                message = message,
                isValid = isValid
            });
        }

        [HttpPost]
        public dynamic InsertReimburseDetail(ReimburseModel.Detail model)
        {
            string message = "OK";
            bool isValid = true;
            object objResult = null;
            try
            {
                ResponseModel respModel = _reimburseService.InsertReimburseDetail(model);
                isValid = respModel.isValid;
                message = respModel.message;
                objResult = respModel.objResult;
                if (!respModel.isValid)
                {
                    return Json(new
                    {
                        isValid,
                        message,
                        objResult
                    });
                }

                LOG.Info("InsertReimburseDetail Success");                
            }
            catch (Exception ex)
            {
                LOG.Error("InsertReimburseDetail Failed", ex);
                isValid = false;
                message = "Insert data failed, Please try again or contact your administrator.";
            }
            return Json(new
            {
                isValid,
                message,
                objResult
            });
        }

        [HttpPost]
        public dynamic UpdateReimburseDetail(ReimburseModel.Detail model)
        {
            string message = "OK";
            bool isValid = true;
            object objResult = null;
            try
            {
                ResponseModel respModel = _reimburseService.UpdateReimburseDetail(model);
                isValid = respModel.isValid;
                message = respModel.message;
                objResult = respModel.objResult;
                if (!respModel.isValid)
                {
                    return Json(new
                    {
                        isValid,
                        message,
                        objResult
                    });
                }


                LOG.Info("UpdateReimburseDetail Success");
            }
            catch (Exception ex)
            {
                LOG.Error("UpdateReimburseDetail Failed", ex);
                isValid = false;
                message = "Update data failed, Please try again or contact your administrator.";
            }

            return Json(new
            {
                isValid,
                message,
                objResult
            });
        }

        [HttpPost]
        public dynamic RejectReimburseDetail(ReimburseModel.Detail model)
        {
            string message = "OK";
            bool isValid = true;
            try
            {
                model.IsRejected = true;
                ResponseModel respModel = _reimburseService.RejectReimburseDetail(model);
                isValid = respModel.isValid;
                message = respModel.message;

                if (isValid)
                {
                    message = "Reject Data Success...";
                    LOG.Info("RejectReimburseDetail Success");
                }
            }
            catch (Exception ex)
            {
                LOG.Error("RejectReimburseDetail Failed", ex);
                isValid = false;
                message = "Reject data failed, Please try again or contact your administrator.";
            }
            return Json(new
            {
                message = message,
                isValid = isValid
            });
        }

        [HttpPost]
        public dynamic UnRejectReimburseDetail(ReimburseModel.Detail model)
        {
            string message = "OK";
            bool isValid = true;
            try
            {
                model.IsRejected = false;
                ResponseModel respModel = _reimburseService.RejectReimburseDetail(model);
                isValid = respModel.isValid;
                message = respModel.message;

                if (isValid)
                {
                    message = "UnReject Data Success...";
                    LOG.Info("RejectReimburseDetail Success");
                }
            }
            catch (Exception ex)
            {
                LOG.Error("UnRejectReimburseDetail Failed", ex);
                isValid = false;
                message = "UnReject data failed, Please try again or contact your administrator.";
            }
            return Json(new
            {
                message = message,
                isValid = isValid
            });
        }

        public ActionResult Detail()
        {
            return View();
        }

    }
}
