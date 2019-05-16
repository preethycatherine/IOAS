using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IOAS.Models.Process;
using IOAS.DataModel;

namespace IOAS.GenericServices.Process
{
    public class ProcessGuidelineBO
    {
        public static ControlDataList LoadControls()
        {
            ControlDataList objcbl = new ControlDataList();

            using (var context = new IOASDBEntities())
            {
                var queryfunction = (from F in context.tblFunction
                                     orderby F.FunctionName
                                     select F).ToList();
                if (queryfunction.Count > 0)
                {
                    objcbl.FunctionList = new List<Function>();
                    for (int i = 0; i < queryfunction.Count; i++)
                    {
                        objcbl.FunctionList.Add(new Function()
                        {
                            FunctionId = (Int32)queryfunction[i].FunctionId,
                            FunctionName = Convert.ToString(queryfunction[i].FunctionName)
                        });
                    }
                }

                var queryApprover = (from U in context.tblUser
                                     orderby U.UserName
                                     select U).ToList();
                if (queryApprover.Count > 0)
                {
                    objcbl.ApproverList = new List<Approver>();
                    for (int i = 0; i < queryApprover.Count; i++)
                    {
                        objcbl.ApproverList.Add(new Approver()
                        {
                            ApproverId = (Int32)queryApprover[i].UserId,
                            ApproverName = Convert.ToString(queryApprover[i].UserName)
                        });
                    }
                }

                var queryStatus = (from F in context.tblFunctionStatus
                                   orderby F.Status
                                   select F).ToList();
                if (queryStatus.Count > 0)
                {
                    objcbl.StatusList = new List<ProcessFlowStatus>();
                    for (int i = 0; i < queryStatus.Count; i++)
                    {
                        objcbl.StatusList.Add(new ProcessFlowStatus()
                        {
                            StatusId = (Int32)queryStatus[i].FunctionStatusId,
                            StatusName = Convert.ToString(queryStatus[i].Status)
                        });
                    }
                }

                var queryDocument = (from D in context.tblDocument
                                     orderby D.DocumentName
                                     select D).ToList();
                if (queryDocument.Count > 0)
                {
                    objcbl.DocumentList = new List<Document>();
                    for (int i = 0; i < queryDocument.Count; i++)
                    {
                        objcbl.DocumentList.Add(new Document()
                        {
                            DocumentId = (Int32)queryDocument[i].DocumentId,
                            DocumentName = Convert.ToString(queryDocument[i].DocumentName)
                        });
                    }
                }
            }

            return objcbl;
        }

        public static List<ProcessFlowModel> GetProcessFlowList(int processGuidelineId)
        {
            List<ProcessFlowModel> processFlowDetails = new List<ProcessFlowModel>();
            using (var context = new IOASDBEntities())
            {
                var query = (from P in context.tblProcessGuidelineDetail where P.ProcessGuidelineId == processGuidelineId orderby P.ProcessGuidelineDetailId select P).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        processFlowDetails.Add(new ProcessFlowModel()
                        {
                            ProcessGuidelineDetailId = (Int32)query[i].ProcessGuidelineDetailId,
                            FlowTitle = Convert.ToString(query[i].FlowTitle)
                        });
                    }
                }
            }
            return processFlowDetails;
        }

        public static int AddProcessFlow(ProcessFlowModel model)
        {
            using (var context = new IOASDBEntities())
            {
                tblProcessGuidelineDetail objIU = new tblProcessGuidelineDetail();
                objIU.ProcessGuidelineId = model.ProcessGuidelineid;
                objIU.FlowTitle = model.FlowTitle;
                context.tblProcessGuidelineDetail.Add(objIU);
                context.SaveChanges();
                context.Dispose();
                return objIU.ProcessGuidelineDetailId;
            }
        }

        public static int UpdateProcessFlow(ProcessFlowModel model)
        {
            tblProcessGuidelineDetail pgd;
            using (var context = new IOASDBEntities())
            {
                pgd = context.tblProcessGuidelineDetail.Where(s => s.ProcessGuidelineDetailId == model.ProcessGuidelineDetailId).FirstOrDefault();
            }
            if (pgd != null)
            {
                pgd.FlowTitle = model.FlowTitle;
            }
            using (var dbCtx = new IOASDBEntities())
            {
                dbCtx.Entry(pgd).State = System.Data.Entity.EntityState.Modified;
                dbCtx.SaveChanges();
            }
            return model.ProcessGuidelineDetailId;
        }

        public static List<ProcessFlowUser> GetProcessFlowUserDetails(int processGuidelineDetailId)
        {
            List<ProcessFlowUser> processFlowUserDetails = new List<ProcessFlowUser>();
            using (var context = new IOASDBEntities())
            {
                var query = (from D in context.tblProcessGuidelineDetail
                             from U in context.tblUser
                             join PU in context.tblProcessGuidelineUser on new { DetailId = (int?)D.ProcessGuidelineDetailId, UserId = (int?)U.UserId } equals new { DetailId = PU.ProcessGuidelineDetailId, UserId = PU.UserId } into temp
                             from PUG in temp.DefaultIfEmpty()
                             where (D.ProcessGuidelineDetailId == processGuidelineDetailId)
                             orderby U.UserId
                             select new
                             {
                                 UserId = U.UserId
                                ,
                                 UserName = U.UserName
                                ,
                                 UserFlag = (PUG.UserId == null ? 0 : 1)
                                ,
                                 ProcessGuidelineDetailId = D.ProcessGuidelineDetailId//pgd.ProcessGuidelineDetailId
                             }
                             ).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        processFlowUserDetails.Add(new ProcessFlowUser()
                        {
                            ProcessGuidelineDetailId = (Int32)query[i].ProcessGuidelineDetailId,
                            UserId = Convert.ToInt32(query[i].UserId),
                            UserName = Convert.ToString(query[i].UserName),
                            UserFlag = Convert.ToInt32(query[i].UserFlag) == 0 ? false : true
                        });
                    }
                }
            }
            return processFlowUserDetails;
        }

        public static List<ProcessFlowUser> GetApproverList()
        {
            List<ProcessFlowUser> approvalList = new List<ProcessFlowUser>();
            using (var context = new IOASDBEntities())
            {
                var query = (from U in context.tblUser where U.ApproverF == true orderby U.UserId select U).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        approvalList.Add(new ProcessFlowUser()
                        {
                            UserId = Convert.ToInt32(query[i].UserId),
                            UserName = Convert.ToString(query[i].UserName)
                        });
                    }
                }
            }
            return approvalList;
        }

        public static List<ProcessFlowStatus> GetStatus()
        {
            List<ProcessFlowStatus> statusList = new List<ProcessFlowStatus>();
            using (var context = new IOASDBEntities())
            {
                var query = (from S in context.tblFunctionStatus orderby S.FunctionStatusId select S).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        statusList.Add(new ProcessFlowStatus()
                        {
                            StatusId = Convert.ToInt32(query[i].FunctionStatusId),
                            StatusName = Convert.ToString(query[i].Status)
                        });
                    }
                }
            }
            return statusList;
        }

        public static int AddApproverDetails(ProcessFlowApproverList model)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    int rowsAffected = 0, pglWorkflowId = 0;
                    if (model.ProcessguidlineworkflowId == 0)
                    {
                        tblProcessGuidelineWorkFlow objIU = new tblProcessGuidelineWorkFlow();
                        objIU.ProcessGuidelineId = model.processguidlineId;
                        objIU.ProcessGuidelineDetailId = model.ProcessGuidelineDetailId;
                        objIU.StatusId = model.StatusId;
                        objIU.ApproverId = model.UserId;
                        objIU.ApproverLevel = model.ApproverLevel;
                        objIU.Approve_f = model.ApproveFlag;
                        objIU.Reject_f = model.RejectFlag;
                        objIU.Clarify_f = model.ClarifyFlag;
                        objIU.Mark_f = model.MarkFlag;
                        context.tblProcessGuidelineWorkFlow.Add(objIU);
                        rowsAffected = context.SaveChanges();
                        pglWorkflowId = objIU.ProcessGuidelineWorkFlowId;
                    }
                    else
                    {
                        tblProcessGuidelineWorkFlow objupdate = new tblProcessGuidelineWorkFlow();
                        objupdate = context.tblProcessGuidelineWorkFlow.Where(M => M.ProcessGuidelineWorkFlowId == model.ProcessguidlineworkflowId).FirstOrDefault();
                        if (objupdate != null)
                        {
                            objupdate.ProcessGuidelineId = model.processguidlineId;
                            objupdate.ProcessGuidelineDetailId = model.ProcessGuidelineDetailId;
                            objupdate.StatusId = model.StatusId;
                            objupdate.ApproverId = model.UserId;
                            objupdate.ApproverLevel = model.ApproverLevel;
                            objupdate.Approve_f = model.ApproveFlag;
                            objupdate.Reject_f = model.RejectFlag;
                            objupdate.Clarify_f = model.ClarifyFlag;
                            objupdate.Mark_f = model.MarkFlag;
                            rowsAffected = context.SaveChanges();
                            pglWorkflowId = objupdate.ProcessGuidelineWorkFlowId;
                        }
                    }
                    //Document insertion
                    if ((model.ProcessguidlineworkflowId == 0 && rowsAffected > 0) || model.ProcessguidlineworkflowId > 0)
                    {
                        //Remove existing document
                        tblProcessGuidelineDocument objDocDelete;
                        objDocDelete = context.tblProcessGuidelineDocument.Where(s => s.WorkflowId == pglWorkflowId).FirstOrDefault();
                        if (objDocDelete != null)
                        {
                            context.Entry(objDocDelete).State = System.Data.Entity.EntityState.Deleted;
                            context.SaveChanges();
                        }
                        //Insert new document
                        tblProcessGuidelineDocument objDoc = new tblProcessGuidelineDocument();
                        objDoc.DocumentId = model.DocumentId;
                        objDoc.WorkflowId = pglWorkflowId;
                        context.tblProcessGuidelineDocument.Add(objDoc);
                        context.SaveChanges();
                    }
                    return pglWorkflowId;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public static List<ProcessFlowApproverList> GetAllApproverList(int processheaderid, int processDetailId)
        {
            try
            {
                List<ProcessFlowApproverList> approvalList = new List<ProcessFlowApproverList>();
                using (var context = new IOASDBEntities())
                {
                    var query = (
                                    from WF in context.tblProcessGuidelineWorkFlow
                                    from U in context.tblUser
                                    from S in context.tblFunctionStatus
                                    where (WF.ProcessGuidelineId == processheaderid && WF.StatusId == S.FunctionStatusId && WF.ApproverId == U.UserId && WF.ProcessGuidelineDetailId == processDetailId)
                                    orderby WF.ProcessGuidelineWorkFlowId
                                    select new { WF.ProcessGuidelineWorkFlowId, WF.ProcessGuidelineDetailId, WF.ApproverId, U.UserName, S.Status, WF.StatusId, WF.ApproverLevel, WF.Approve_f, WF.Reject_f, WF.Clarify_f, WF.Mark_f }
                                 ).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            tblProcessGuidelineDocument objDoc = new tblProcessGuidelineDocument();
                            int pgwfid = query[i].ProcessGuidelineWorkFlowId;
                            objDoc = context.tblProcessGuidelineDocument.Where(M => M.WorkflowId == pgwfid).FirstOrDefault();
                            approvalList.Add(new ProcessFlowApproverList()
                            {
                                UserId = Convert.ToInt32(query[i].ApproverId),
                                UserName = Convert.ToString(query[i].UserName),
                                ProcessGuidelineDetailId = Convert.ToInt32(query[i].ProcessGuidelineDetailId),
                                ApproverLevel = Convert.ToInt32(query[i].ApproverLevel),
                                StatusId = Convert.ToInt32(query[i].StatusId),
                                StatusName = Convert.ToString(query[i].Status),
                                ApproveFlag = Convert.ToBoolean(query[i].Approve_f),
                                RejectFlag = Convert.ToBoolean(query[i].Reject_f),
                                ClarifyFlag = Convert.ToBoolean(query[i].Clarify_f),
                                MarkFlag = Convert.ToBoolean(query[i].Mark_f),
                                ProcessguidlineworkflowId = query[i].ProcessGuidelineWorkFlowId,
                                DocumentId = objDoc != null ? Convert.ToInt32(objDoc.DocumentId) : 0
                            });
                        }
                    }
                }
                return approvalList;
            }
            catch (Exception ex)
            {
                return new List<ProcessFlowApproverList>();
            }
        }

        public static int DeletePGLWorkflow(int processguidlineworkflowId)
        {
            try
            {
                tblProcessGuidelineWorkFlow pglWF;
                int rowsAffected = 0;
                using (var context = new IOASDBEntities())
                {
                    pglWF = context.tblProcessGuidelineWorkFlow.Where(s => s.ProcessGuidelineWorkFlowId == processguidlineworkflowId).FirstOrDefault();
                    context.Entry(pglWF).State = System.Data.Entity.EntityState.Deleted;
                    rowsAffected = context.SaveChanges();
                }
                return rowsAffected;
            }
            catch
            {
                return -1;
            }
        }

        public static int InsertProcessGuideline(ProcessGuideline model)
        {
            using (var context = new IOASDBEntities())
            {
                tblProcessGuidelineHeader objIU = new tblProcessGuidelineHeader();
                objIU.ProcessGuidelineId = model.ProcessGuidelineId;
                objIU.FunctionId = model.FunctionId;
                objIU.ProcessGuidelineTitle = model.ProcessName;
                if (model.ProcessGuidelineId == 0)
                {
                    context.tblProcessGuidelineHeader.Add(objIU);
                }
                else
                {
                    context.Entry(objIU).State = System.Data.Entity.EntityState.Modified;
                }
                context.SaveChanges();
                context.Dispose();
                return objIU.ProcessGuidelineId;
            }
        }

        public static int MapProcessflowUser(List<ProcessFlowUser> selectedUser)
        {
            try
            {
                int rowsAffected = 0;
                using (var context = new IOASDBEntities())
                {
                    if (selectedUser.Count > 0)
                    {
                        foreach (var user in selectedUser)
                        {
                            tblProcessGuidelineUser objIU = new tblProcessGuidelineUser();
                            objIU.ProcessGuidelineDetailId = user.ProcessGuidelineDetailId;
                            objIU.UserId = user.UserId;
                            context.tblProcessGuidelineUser.Add(objIU);
                            rowsAffected = rowsAffected + context.SaveChanges();
                        }
                    }
                }
                return rowsAffected;
            }
            catch
            {
                return -1;
            }
        }

        public static int UnmapProcessflowUser(List<ProcessFlowUser> selectedUser)
        {
            try
            {
                tblProcessGuidelineUser pglUser;
                int rowsAffected = 0;
                using (var context = new IOASDBEntities())
                {
                    if (selectedUser.Count > 0)
                    {
                        foreach (var user in selectedUser)
                        {
                            pglUser = context.tblProcessGuidelineUser.Where(s => s.UserId == user.UserId && s.ProcessGuidelineDetailId == user.ProcessGuidelineDetailId).FirstOrDefault();
                            context.Entry(pglUser).State = System.Data.Entity.EntityState.Deleted;
                            rowsAffected = rowsAffected + context.SaveChanges();
                        }
                    }
                }
                return rowsAffected;
            }
            catch
            {
                return -1;
            }
        }

        public static List<ProcessGuideline> GetProcessGuideLineList(int functionId, string processName)
        {
            List<ProcessGuideline> objProcessGuideline = new List<ProcessGuideline>();
            using (var context = new IOASDBEntities())
            {
                var query = (from pgl in context.tblProcessGuidelineHeader
                             from f in context.tblFunction
                             where (pgl.FunctionId == f.FunctionId && (pgl.FunctionId == functionId || functionId == -1)
                             && (pgl.ProcessGuidelineTitle.Contains(processName) || processName == string.Empty)
                             )
                             orderby pgl.ProcessGuidelineId
                             select new { pgl.ProcessGuidelineId, pgl.ProcessGuidelineTitle, f.FunctionName, f.FunctionId }).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        objProcessGuideline.Add(new ProcessGuideline()
                        {
                            ProcessGuidelineId = (Int32)query[i].ProcessGuidelineId,
                            ProcessName = Convert.ToString(query[i].ProcessGuidelineTitle),
                            FunctionId = Convert.ToInt32(query[i].FunctionId),
                            FunctionName = Convert.ToString(query[i].FunctionName)
                        });
                    }
                }
            }
            return objProcessGuideline;
        }
    }
}