using Account.DTO.Library;
using MES.Business.Repositories.Setup.Process;
using NPE.Core;
using NPE.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPE.Core.Extensions;
using System.Data.Entity.Core.Objects;


namespace MES.Business.Library.BO.Setup.Process
{
    class Process : ContextBusinessBase, IProcessRepository
    {
        public Process() : base("Process") { }

        public ITypedResponse<bool?> Delete(int id)
        {
            var processToBeDeleted = this.DataContext.Processes.Where(a => a.Id == id).SingleOrDefault();
            if (processToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("ProcessNotExists"));
            }
            else
            {
                processToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                processToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(processToBeDeleted).State = EntityState.Modified;
                processToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("ProcessDeletedSuccess"));
            }
        }

        public ITypedResponse<DTO.Library.Setup.Process.Process> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public ITypedResponse<int?> Save(DTO.Library.Setup.Process.Process process)
        {
            string errMSg = null;
            string successMsg = null;
            //check for the uniqueness
            if (this.DataContext.Processes.AsNoTracking().Any(a => a.ProcessName == process.process && a.IsDeleted == false && a.Id != process.Id))
            {
                errMSg = Languages.GetResourceText("ProcessExists");
            }
            else
            {
                var recordToBeUpdated = new MES.Data.Library.Process();

                if (process.HasId())
                {
                    recordToBeUpdated = this.DataContext.Processes.Where(a => a.Id == process.Id).SingleOrDefault();

                    if (recordToBeUpdated == null)
                        errMSg = Languages.GetResourceText("ProcesseNotExists");
                    else
                    {
                        recordToBeUpdated.UpdatedDate = AuditUtils.GetCurrentDateTime();
                        recordToBeUpdated.UpdatedBy = CurrentUser;
                        this.DataContext.Entry(recordToBeUpdated).State = EntityState.Modified;
                    }
                }
                else
                {
                    recordToBeUpdated.CreatedBy = recordToBeUpdated.UpdatedBy = CurrentUser;
                    recordToBeUpdated.CreatedDate = AuditUtils.GetCurrentDateTime();
                    this.DataContext.Processes.Add(recordToBeUpdated);
                }
                if (string.IsNullOrEmpty(errMSg))
                {
                    recordToBeUpdated.ProcessName = process.process;
                    this.DataContext.SaveChanges();
                    process.Id = recordToBeUpdated.Id;
                    successMsg = Languages.GetResourceText("ProcessSavedSuccess");
                }
            }
            return SuccessOrFailedResponse<int?>(errMSg, process.Id, successMsg);
        }

        public ITypedResponse<List<DTO.Library.Setup.Process.Process>> GetProcesses(IPage<DTO.Library.Setup.Process.SearchCriteria> paging)
        {

            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);


            string errMSg = null;

            //declare paging variables
            /*int PageNumber = paging.PageNo > 0 ? paging.PageNo - 1 : 0;
            int PageSize = paging.PageSize > 0 ? paging.PageSize : 10;
            int RecordStart = PageNumber * PageSize;*/

            List<DTO.Library.Setup.Process.Process> processes = new List<DTO.Library.Setup.Process.Process>();
            DTO.Library.Setup.Process.Process process;
            this.RunOnDB(context =>
            {
                var processList = context.SearchProcess(paging.Criteria.process, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                if (processList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                    foreach (var item in processList)
                    {
                        process = new DTO.Library.Setup.Process.Process();
                        process.Id = item.Id;
                        process.process = item.ProcessName;
                        processes.Add(process);
                    }
                }
            });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.Setup.Process.Process>>(errMSg, processes);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }


        public ITypedResponse<List<DTO.Library.Setup.Process.Process>> Search(IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

    }
}
