using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.Business.Repositories.RFQ.Supplier;
using Account.DTO.Library;
using NPE.Core;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

namespace MES.Business.Library.BO.RFQ.Supplier
{
    public class SupplierAssessment : ContextBusinessBase, ISupplierAssessmentRepository
    {
        public SupplierAssessment()
            : base("SupplierAssessment")
        { }

        /// <summary>
        /// Save Supplier Assessment..
        /// </summary>
        /// <param name="assessment"></param>
        /// <returns></returns>
        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.RFQ.Supplier.SupplierAssessment assessment)
        {
            string errMSg = null;
            string successMsg = null;

            var recordToBeUpdated = new MES.Data.Library.SupplierAssessment();
            if (assessment.Id > 0)
            {
                recordToBeUpdated = this.DataContext.SupplierAssessments.Where(a => a.Id == assessment.Id).SingleOrDefault();

                if (recordToBeUpdated == null)
                    errMSg = Languages.GetResourceText("ContactsNotExists");
                else
                {
                    recordToBeUpdated.UpdatedDate = AuditUtils.GetCurrentDateTime();
                    recordToBeUpdated.UpdatedBy = CurrentUser;
                    this.DataContext.Entry(recordToBeUpdated).State = EntityState.Modified;
                }
            }
            else
            {
                recordToBeUpdated.IsDeleted = false;
                recordToBeUpdated.Revision = "0";
                recordToBeUpdated.CreatedBy = recordToBeUpdated.UpdatedBy = CurrentUser;
                recordToBeUpdated.CreatedDate = AuditUtils.GetCurrentDateTime();
                recordToBeUpdated.UpdatedDate = AuditUtils.GetCurrentDateTime();
                this.DataContext.SupplierAssessments.Add(recordToBeUpdated);
            }

            #region General Details
            recordToBeUpdated.AuditDate = DateTime.Now;
            recordToBeUpdated.SupplierId = assessment.SupplierId;
            recordToBeUpdated.LeadAuditor = assessment.LeadAuditor;
            recordToBeUpdated.Commodities = assessment.Commodities;
            recordToBeUpdated.AuditDate = assessment.AuditDate.HasValue ? assessment.AuditDate.Value : DateTime.Now;
            recordToBeUpdated.PrimaryCustomer = assessment.PrimaryCustomer;
            recordToBeUpdated.PrimaryIndustries = assessment.PrimaryIndustries;
            recordToBeUpdated.ExportExperience = assessment.ExportExperience;
            recordToBeUpdated.LastYearSales = assessment.LastYearSales;
            recordToBeUpdated.PreviousYearSales = assessment.PreviousYearSales;
            recordToBeUpdated.NoOfShifts = assessment.NoOfShifts;
            recordToBeUpdated.TotalMFGFloorSpace = assessment.TotalMFGFloorSpace;
            recordToBeUpdated.NoOfEmployees = assessment.NoOfEmployees;
            recordToBeUpdated.TotalScore = assessment.TotalScore;
            recordToBeUpdated.QualityScore = assessment.QualityScore;
            recordToBeUpdated.CoreCompetenciesList = assessment.CoreCompetenciesList;
            recordToBeUpdated.Strengths = assessment.Strengths;
            recordToBeUpdated.AreasforImprovement = assessment.AreasforImprovement;
            recordToBeUpdated.EHSScore = assessment.EHSScore;
            recordToBeUpdated.FinalEHSScore = assessment.FinalEHSScore;
            recordToBeUpdated.IsNew = true;
            
            recordToBeUpdated.PreviousYearCompanyFinanacials = assessment.PreviousYearCompanyFinancials;
            recordToBeUpdated.SupplimentaryEvidence = assessment.SupplimentaryEvidence;
            recordToBeUpdated.EquipmentCapacityList_FileName = assessment.EquipmentCapacityList_FileName;
            recordToBeUpdated.EquipmentCapacityList_FilePath = assessment.EquipmentCapacityList_FilePath;
            #endregion

            this.DataContext.SaveChanges();
            assessment.Id = recordToBeUpdated.Id;

            #region Save Element values..
            // MES.Data.Library.AssessmentElementValue objElementValue = null;
            if (assessment.QuestionType != null && assessment.QuestionType.Count > 0)
            {
                foreach (var questionItem in assessment.QuestionType)
                {
                    if (questionItem.AssessmentElement != null && questionItem.AssessmentElement.Count > 0)
                    {
                        foreach (var elementITem in questionItem.AssessmentElement)
                        {
                            if (elementITem.ElementDetails != null && elementITem.ElementDetails.Count > 0)
                            {
                                foreach (var detailItem in elementITem.ElementDetails)
                                {                                   
                                    var valueObjToBeUpdated = new MES.Data.Library.AssessmentElementValue();
                                    if (detailItem.AssessmentElementValue != null)
                                    {
                                        valueObjToBeUpdated = this.DataContext.AssessmentElementValues.Where(x => x.AssessmentId == assessment.Id && x.DetailId == detailItem.Id).FirstOrDefault();
                                        if (valueObjToBeUpdated != null)
                                        {
                                            valueObjToBeUpdated.UpdatedBy = CurrentUser;
                                            valueObjToBeUpdated.UpdatedDate = AuditUtils.GetCurrentDateTime();
                                            this.DataContext.Entry(valueObjToBeUpdated).State = EntityState.Modified;
                                        }
                                        else
                                        {
                                            valueObjToBeUpdated = new MES.Data.Library.AssessmentElementValue();
                                            valueObjToBeUpdated.CreatedBy = valueObjToBeUpdated.UpdatedBy = CurrentUser;
                                            valueObjToBeUpdated.CreatedDate = AuditUtils.GetCurrentDateTime();
                                            valueObjToBeUpdated.UpdatedDate = AuditUtils.GetCurrentDateTime();
                                            this.DataContext.AssessmentElementValues.Add(valueObjToBeUpdated);
                                        }
                                        valueObjToBeUpdated.AssessmentId = assessment.Id;
                                        valueObjToBeUpdated.DetailId = detailItem.Id;
                                        //valueObjToBeUpdated.Weight = detailItem.AssessmentElementValue.Weight;
                                        valueObjToBeUpdated.Score = Convert.ToInt32(detailItem.AssessmentElementValue.Score);
                                        valueObjToBeUpdated.Observations = detailItem.AssessmentElementValue.Observations;
                                        valueObjToBeUpdated.FileName = detailItem.AssessmentElementValue.FileName;
                                        valueObjToBeUpdated.FilePath = detailItem.AssessmentElementValue.FilePath;
                                        valueObjToBeUpdated.OptionValue = detailItem.AssessmentElementValue.OptionValue;

                                        this.DataContext.SaveChanges();
                                    }
                                    #region Save SubElement Value..
                                    //Check sub element value..
                                    if (detailItem.SubElementAvailable)
                                    {
                                        if (detailItem.SubElementDetail != null && detailItem.SubElementDetail.Count > 0)
                                        {
                                            foreach (var subItem in detailItem.SubElementDetail)
                                            {
                                                var subValueObjToBeUpdated = new MES.Data.Library.AssessmentSubElementValue();
                                                if (subItem != null)
                                                {
                                                    subValueObjToBeUpdated = this.DataContext.AssessmentSubElementValues.Where(x => x.AssessmentId == assessment.Id && x.SubElementId == subItem.Id).FirstOrDefault();
                                                    if (subValueObjToBeUpdated != null)
                                                    {
                                                        subValueObjToBeUpdated.UpdatedBy = CurrentUser;
                                                        subValueObjToBeUpdated.UpdatedDate = AuditUtils.GetCurrentDateTime();
                                                        this.DataContext.Entry(subValueObjToBeUpdated).State = EntityState.Modified;
                                                    }
                                                    else
                                                    {
                                                        subValueObjToBeUpdated = new MES.Data.Library.AssessmentSubElementValue();
                                                        subValueObjToBeUpdated.CreatedBy = subValueObjToBeUpdated.UpdatedBy = CurrentUser;
                                                        subValueObjToBeUpdated.CreatedDate = AuditUtils.GetCurrentDateTime();
                                                        subValueObjToBeUpdated.UpdatedDate = AuditUtils.GetCurrentDateTime();
                                                        this.DataContext.AssessmentSubElementValues.Add(subValueObjToBeUpdated);
                                                    }
                                                    subValueObjToBeUpdated.AssessmentId = assessment.Id;
                                                    subValueObjToBeUpdated.SubElementId = subItem.Id;
                                                    subValueObjToBeUpdated.SubElementValue = subItem.SubElementValue.SubElementValue.ToString();
                                                    this.DataContext.SaveChanges();
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            successMsg = Languages.GetResourceText("AssessmentSavedSuccess");
            return SuccessOrFailedResponse<int?>(errMSg, assessment.Id, successMsg);
        }

        /// <summary>
        /// Find Assessment by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NPE.Core.ITypedResponse<DTO.Library.RFQ.Supplier.SupplierAssessment> FindById(int id)
        {
            string errMSg = string.Empty;
            DTO.Library.RFQ.Supplier.SupplierAssessment assessmentInfo = new DTO.Library.RFQ.Supplier.SupplierAssessment();

            this.RunOnDB(context =>
            {
                var assessment = context.SupplierAssessments.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefault();
                if (assessment != null)
                {
                    assessmentInfo.Id = assessment.Id;
                    assessmentInfo.SupplierId = assessment.SupplierId;
                    assessmentInfo.LeadAuditor = assessment.LeadAuditor;
                    assessmentInfo.Commodities = assessment.Commodities;
                    assessmentInfo.AuditDate = assessment.AuditDate;
                    assessmentInfo.PrimaryCustomer = assessment.PrimaryCustomer;
                    assessmentInfo.PrimaryIndustries = assessment.PrimaryIndustries;
                    assessmentInfo.ExportExperience = assessment.ExportExperience;
                    assessmentInfo.LastYearSales = assessment.LastYearSales;
                    assessmentInfo.PreviousYearSales = assessment.PreviousYearSales;
                    assessmentInfo.NoOfShifts = assessment.NoOfShifts;
                    assessmentInfo.TotalMFGFloorSpace = assessment.TotalMFGFloorSpace;
                    assessmentInfo.NoOfEmployees = assessment.NoOfEmployees;
                    assessmentInfo.TotalScore = assessment.TotalScore.HasValue ? assessment.TotalScore.Value : 0;
                    assessmentInfo.QualityScore = assessment.QualityScore;
                    assessmentInfo.EHSScore = assessment.EHSScore.HasValue ? assessment.EHSScore.Value : 0;
                    assessmentInfo.CoreCompetenciesList = assessment.CoreCompetenciesList;
                    assessmentInfo.Strengths = assessment.Strengths;
                    assessmentInfo.AreasforImprovement = assessment.AreasforImprovement;
                    assessmentInfo.IsDeleted = assessment.IsDeleted;
                    assessmentInfo.PreviousYearCompanyFinancials = assessment.PreviousYearCompanyFinanacials;
                    assessmentInfo.EquipmentCapacityList_FileName = assessment.EquipmentCapacityList_FileName;
                    assessmentInfo.EquipmentCapacityList_FilePath = assessment.EquipmentCapacityList_FilePath;
                    assessmentInfo.SupplimentaryEvidence = assessment.SupplimentaryEvidence;
                    assessmentInfo.EHSScore = assessment.EHSScore.HasValue ? assessment.EHSScore.Value : 0;
                    assessmentInfo.FinalEHSScore = assessment.FinalEHSScore.HasValue ? assessment.FinalEHSScore.Value : 0;
                    assessmentInfo.IsNew = assessment.IsNew.HasValue ? assessment.IsNew.Value : false;

                    #region Get Scope of Work details
                    assessmentInfo.QuestionType = new List<DTO.Library.RFQ.Supplier.AssessmentQuestionType>();

                    var questionInfo = context.AssessmentQuestionTypes.ToList();
                    if (questionInfo != null && questionInfo.Count > 0)
                    {
                        DTO.Library.RFQ.Supplier.AssessmentQuestionType questionObj = null;
                        foreach (var item in questionInfo)
                        {
                            questionObj = new DTO.Library.RFQ.Supplier.AssessmentQuestionType();
                            questionObj.Id = item.Id;
                            questionObj.QuestionType = item.QuestionType;
                            questionObj.AssessmentElement = new List<DTO.Library.RFQ.Supplier.AssessmentElement>();

                            //Get element..
                            var elementInfo = context.AssessmentElements.Where(x => x.QuestionTypeId == item.Id).ToList();
                            if (elementInfo != null && elementInfo.Count > 0)
                            {
                                DTO.Library.RFQ.Supplier.AssessmentElement elementObj = null;

                                foreach (var elementItem in elementInfo)
                                {
                                    elementObj = new DTO.Library.RFQ.Supplier.AssessmentElement();
                                    elementObj.Id = elementItem.Id;
                                    elementObj.QuestionTypeId = item.Id; //question type Id..
                                    elementObj.ElementName = elementItem.ElementName;
                                    elementObj.ElementName_Chinese = elementItem.ElementName_Chinese;

                                    elementObj.ElementDetails = new List<DTO.Library.RFQ.Supplier.AssessmentElementDetail>();

                                    var elementDetailInfo = context.AssessmentElementDetails.Where(x => x.ElementId == elementItem.Id).ToList();
                                    if (elementDetailInfo != null && elementDetailInfo.Count > 0)
                                    {
                                        DTO.Library.RFQ.Supplier.AssessmentElementDetail elementDetailObj = null;

                                        foreach (var detailItem in elementDetailInfo)
                                        {
                                            elementDetailObj = new DTO.Library.RFQ.Supplier.AssessmentElementDetail();
                                            elementDetailObj.Id = detailItem.Id;
                                            elementDetailObj.ElementDetail = detailItem.ElementDetail;
                                            elementDetailObj.ElementDetail_Chinese = detailItem.ElementDetail_Chinese;
                                            elementDetailObj.SortOrder = detailItem.SortOrder;
                                            elementDetailObj.IsOptionAvailable = detailItem.IsOptionAvailable.HasValue ? detailItem.IsOptionAvailable.Value : false;
                                            elementDetailObj.Weight = detailItem.Weight.HasValue ? detailItem.Weight.Value : 0;

                                            elementDetailObj.SubElementAvailable = detailItem.SubElement.HasValue ? detailItem.SubElement.Value : false;

                                            elementDetailObj.AssessmentElementValue = new DTO.Library.RFQ.Supplier.AssessmentElementValue();

                                            //get elemenet value..
                                            var elementValueObj = context.AssessmentElementValues.Where(x => x.DetailId == detailItem.Id && x.AssessmentId == id).FirstOrDefault();
                                            if (elementValueObj != null)
                                            {
                                                elementDetailObj.AssessmentElementValue.AssementId = item.Id;
                                                elementDetailObj.AssessmentElementValue.DetailId = detailItem.Id;
                                                //elementDetailObj.AssessmentElementValue.Weight = elementValueObj.Weight.HasValue ? elementValueObj.Weight.Value : 0;
                                                //elementDetailObj.AssessmentElementValue.Weight = elementValueObj.Weight;
                                                elementDetailObj.AssessmentElementValue.Score = elementValueObj.Score.HasValue ? elementValueObj.Score.Value : 0;

                                                int? weight = elementDetailObj.Weight; // elementDetailObj.AssessmentElementValue.Weight;

                                                int? score = elementDetailObj.AssessmentElementValue.Score;
                                                if (!detailItem.Weight.HasValue && !elementValueObj.Score.HasValue)
                                                {
                                                    elementDetailObj.AssessmentElementValue.SubTotal = "N/R";
                                                }
                                                else if (detailItem.Weight.HasValue && !elementValueObj.Score.HasValue)
                                                {
                                                    elementDetailObj.AssessmentElementValue.SubTotal = "N/R";
                                                }
                                                else if (!detailItem.Weight.HasValue && elementValueObj.Score.HasValue)
                                                {
                                                    elementDetailObj.AssessmentElementValue.SubTotal = "0";
                                                }
                                                else
                                                {
                                                    elementDetailObj.AssessmentElementValue.SubTotal = (Convert.ToInt32(elementDetailObj.Weight) * Convert.ToInt32(elementValueObj.Score.Value)).ToString();
                                                }
                                                //elementDetailObj.AssessmentElementValue.SubTotal = elementDetailObj.AssessmentElementValue.Weight * Convert.ToInt32(elementDetailObj.AssessmentElementValue.Score);
                                                elementDetailObj.AssessmentElementValue.Observations = elementValueObj.Observations;
                                                elementDetailObj.AssessmentElementValue.FileName = elementValueObj.FileName;
                                                elementDetailObj.AssessmentElementValue.FilePath = elementValueObj.FilePath;
                                                elementDetailObj.AssessmentElementValue.OptionValue = elementValueObj.OptionValue;
                                            }

                                            //Get subelement...
                                            if (detailItem.SubElement.HasValue && detailItem.SubElement.Value)
                                            {
                                                elementDetailObj.SubElementDetail = new List<DTO.Library.RFQ.Supplier.AssessmentSubElementDetail>();
                                                var subElementInfo = context.AssessmentSubElementDetails.Where(x => x.DetailId == detailItem.Id).OrderBy(x => x.SortOrder).ToList();
                                                if (subElementInfo != null && subElementInfo.Count > 0)
                                                {
                                                    DTO.Library.RFQ.Supplier.AssessmentSubElementDetail subElementDetailObj = null;
                                                    foreach (var subElementItem in subElementInfo)
                                                    {
                                                        subElementDetailObj = new DTO.Library.RFQ.Supplier.AssessmentSubElementDetail();
                                                        subElementDetailObj.Id = subElementItem.Id;
                                                        subElementDetailObj.DetailId = detailItem.Id;
                                                        subElementDetailObj.SortOrder = subElementItem.SortOrder;
                                                        subElementDetailObj.SubElementDetail = subElementItem.SubElementDetail;
                                                        subElementDetailObj.SubElementDetail_Chinese = subElementItem.SubElementDetail_Chinese;
                                                        subElementDetailObj.DataTypeField = subElementItem.DataType;

                                                        subElementDetailObj.SubElementValue = new DTO.Library.RFQ.Supplier.AssessmentSubElementValue();

                                                        var subValueObj = context.AssessmentSubElementValues.Where(x => x.SubElementId == subElementItem.Id && x.AssessmentId == id).FirstOrDefault();
                                                        if (subValueObj != null)
                                                        {
                                                            subElementDetailObj.SubElementValue.AssessmentId = id;
                                                            subElementDetailObj.SubElementValue.SubElementId = subElementItem.Id;
                                                            subElementDetailObj.SubElementValue.SubElementValue = subValueObj.SubElementValue;
                                                        }


                                                        elementDetailObj.SubElementDetail.Add(subElementDetailObj);
                                                    }
                                                }
                                            }//chk subelement details is null or not..
                                            elementObj.ElementDetails.Add(elementDetailObj);
                                        }//detailItem Loop..
                                    }//chk element detail is null or not..
                                    questionObj.AssessmentElement.Add(elementObj);
                                }//elementItem loop..
                            }//chk elementinfo null is or not..
                            assessmentInfo.QuestionType.Add(questionObj);
                        }//scope Item loop..
                    }
                    #endregion
                }
            });

            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.RFQ.Supplier.SupplierAssessment>(errMSg, assessmentInfo);
            return response;
        }

        /// <summary>
        /// Delete Assessment..
        /// </summary>
        /// <param name="suppliersId"></param>
        /// <returns></returns>
        public NPE.Core.ITypedResponse<bool?> Delete(int suppliersId)
        {

            return SuccessBoolResponse(Languages.GetResourceText("PackageDeletedSuccess"));
        }

        /// <summary>
        /// Search assessment..
        /// </summary>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.Supplier.SupplierAssessment>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get Supplier Assessment List..
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        public List<MES.DTO.Library.RFQ.Supplier.AssessmentListDetail> GetSupplierAssessmentList(int suppllierId)
        {
            List<DTO.Library.RFQ.Supplier.AssessmentListDetail> lstAssessment = new List<DTO.Library.RFQ.Supplier.AssessmentListDetail>();
            DTO.Library.RFQ.Supplier.AssessmentListDetail listObj;
            this.RunOnDB(context =>
            {
                var assessmentList = context.SupplierAssessments.Where(x => x.SupplierId == suppllierId && x.IsDeleted == false).OrderBy(x => x.Revision).ToList();
                if (assessmentList != null && assessmentList.Count > 0)
                {
                    foreach (var item in assessmentList)
                    {
                        listObj = new DTO.Library.RFQ.Supplier.AssessmentListDetail();
                        listObj.AssessmentId = item.Id;
                        listObj.SupplierId = item.SupplierId;
                        listObj.CreatedDate = item.CreatedDate;
                        listObj.AuditDate = item.AuditDate;
                        listObj.LeadAuditor = item.LeadAuditor;
                        listObj.TotalScore = item.TotalScore.HasValue ? item.TotalScore.Value : 0;
                        listObj.Revision = item.Revision;
                        listObj.IsNew = item.IsNew.HasValue ? item.IsNew.Value : false;
                        lstAssessment.Add(listObj);
                    }
                }
            });
            return lstAssessment;
        }

        /// <summary>
        /// Get Assessment object..
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NPE.Core.ITypedResponse<DTO.Library.RFQ.Supplier.SupplierAssessment> GetAssessmentDetail(int supplierId)
        {
            string errMSg = string.Empty;
            DTO.Library.RFQ.Supplier.SupplierAssessment assessmentInfo = new DTO.Library.RFQ.Supplier.SupplierAssessment();
            assessmentInfo.SupplierId = supplierId;
            this.RunOnDB(context =>
            {
                #region Get Assessment Question Type
                assessmentInfo.QuestionType = new List<DTO.Library.RFQ.Supplier.AssessmentQuestionType>();
                assessmentInfo.IsNew = true;
                var questionInfo = context.AssessmentQuestionTypes.ToList();
                if (questionInfo != null && questionInfo.Count > 0)
                {
                    DTO.Library.RFQ.Supplier.AssessmentQuestionType questionObj = null;
                    foreach (var item in questionInfo)
                    {
                        questionObj = new DTO.Library.RFQ.Supplier.AssessmentQuestionType();
                        questionObj.Id = item.Id;
                        questionObj.QuestionType = item.QuestionType;
                        questionObj.AssessmentElement = new List<DTO.Library.RFQ.Supplier.AssessmentElement>();

                        //Get element..
                        var elementInfo = context.AssessmentElements.Where(x => x.QuestionTypeId == item.Id).ToList();
                        if (elementInfo != null && elementInfo.Count > 0)
                        {
                            DTO.Library.RFQ.Supplier.AssessmentElement elementObj = null;

                            foreach (var elementItem in elementInfo)
                            {
                                elementObj = new DTO.Library.RFQ.Supplier.AssessmentElement();
                                elementObj.Id = elementItem.Id;
                                elementObj.QuestionTypeId = item.Id; //question type Id..
                                elementObj.ElementName = elementItem.ElementName;
                                elementObj.ElementName_Chinese = elementItem.ElementName_Chinese;

                                elementObj.ElementDetails = new List<DTO.Library.RFQ.Supplier.AssessmentElementDetail>();

                                var elementDetailInfo = context.AssessmentElementDetails.Where(x => x.ElementId == elementItem.Id).OrderBy(x => x.SortOrder).ToList();
                                if (elementDetailInfo != null && elementDetailInfo.Count > 0)
                                {
                                    DTO.Library.RFQ.Supplier.AssessmentElementDetail elementDetailObj = null;

                                    foreach (var detailItem in elementDetailInfo)
                                    {
                                        elementDetailObj = new DTO.Library.RFQ.Supplier.AssessmentElementDetail();
                                        elementDetailObj.Id = detailItem.Id;
                                        elementDetailObj.ElementDetail = detailItem.ElementDetail;
                                        elementDetailObj.ElementDetail_Chinese = detailItem.ElementDetail_Chinese;
                                        elementDetailObj.SortOrder = detailItem.SortOrder;
                                        elementDetailObj.Weight = detailItem.Weight.HasValue ? detailItem.Weight.Value : 0;
                                        elementDetailObj.IsOptionAvailable = detailItem.IsOptionAvailable.HasValue ? detailItem.IsOptionAvailable.Value : false;

                                        elementDetailObj.SubElementAvailable = detailItem.SubElement.HasValue ? detailItem.SubElement.Value : false;

                                        elementDetailObj.AssessmentElementValue = new DTO.Library.RFQ.Supplier.AssessmentElementValue();
                                        //Get subelement...
                                        if (detailItem.SubElement.HasValue && detailItem.SubElement.Value)
                                        {
                                            elementDetailObj.SubElementDetail = new List<DTO.Library.RFQ.Supplier.AssessmentSubElementDetail>();
                                            var subElementInfo = context.AssessmentSubElementDetails.Where(x => x.DetailId == detailItem.Id).OrderBy(x => x.SortOrder).ToList();
                                            if (subElementInfo != null && subElementInfo.Count > 0)
                                            {
                                                DTO.Library.RFQ.Supplier.AssessmentSubElementDetail subElementDetailObj = null;
                                                foreach (var subElementItem in subElementInfo)
                                                {
                                                    subElementDetailObj = new DTO.Library.RFQ.Supplier.AssessmentSubElementDetail();
                                                    subElementDetailObj.Id = subElementItem.Id;
                                                    subElementDetailObj.DetailId = detailItem.Id;
                                                    subElementDetailObj.SortOrder = subElementItem.SortOrder;
                                                    subElementDetailObj.SubElementDetail = subElementItem.SubElementDetail;
                                                    subElementDetailObj.SubElementDetail_Chinese = subElementItem.SubElementDetail_Chinese;
                                                    subElementDetailObj.DataTypeField = subElementItem.DataType;

                                                    subElementDetailObj.SubElementValue = new DTO.Library.RFQ.Supplier.AssessmentSubElementValue();

                                                    elementDetailObj.SubElementDetail.Add(subElementDetailObj);
                                                }
                                            }
                                        }//chk subelement details is null or not..
                                        elementObj.ElementDetails.Add(elementDetailObj);
                                    }//detailItem Loop..
                                }//chk element detail is null or not..
                                questionObj.AssessmentElement.Add(elementObj);
                            }//elementItem loop..
                        }//chk elementinfo null is or not..
                        assessmentInfo.QuestionType.Add(questionObj);
                    }//scope Item loop..
                }
                #endregion
            });

            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.RFQ.Supplier.SupplierAssessment>(errMSg, assessmentInfo);
            return response;
        }

        /// <summary>
        /// Create Revision..
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NPE.Core.ITypedResponse<int?> CreateRevision(int assessmentId)
        {
            string errMsg = null;
            string successMsg = null;
            ObjectParameter RevisionId = new ObjectParameter("RevisionId", 0);
            this.DataContext.CreateAssessmentRevision(assessmentId, CurrentUser, RevisionId);
            int id = Convert.ToInt32(RevisionId.Value);
            return SuccessOrFailedResponse<int?>(errMsg, id, successMsg);
        }
    }
}
