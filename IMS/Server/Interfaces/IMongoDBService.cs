﻿using IMS.Shared.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace IMS.Server.Services
{
    public interface IMongoDBService
    {
        Task<List<ItemModel>> GetItemsByType(int typeid);
        Task<List<UnitModel>> GetUnits(int itemtype);
        Task<string> SaveMaterial(ItemModel item);
        Task<ItemModel> GetItem(string id);
        Task<int> UpdateItem(ItemModel item);
        Task<int> DeleteItem(string[] ids);
        Task<List<WorkCategoryModel>> GetCategories();

         Task<List<WorkItemModel>> GetWorkItems();
         Task<string> EditWorkItem(WorkItemModel workitem);
         Task<string> SaveWorkitem(WorkItemModel workitem);
         Task<int> DeleteWorkItem(string id);

        Task<List<WorkItemInfoModel>> GetWorkItemsInfo(string projectid);

        #region PROJECTS

        Task<List<ProjectModel>> GetProjects();
        Task<ProjectModel> GetProject(string id);
        //Task<ProjectModel> GetProject(string id);
        Task<string> SaveProject(ProjectModel project);
        Task<string> UpdateProject(ProjectModel project);
        Task<string> RemoveProject(string Id);

        Task<string> SaveWorkItemProject(WorkItemModel workitem, string id, string isempty);
        Task<string> AddMaterial(BsonDocument material, string id, string workitemid, string isempty, string type);
        Task<string> RemoveMaterial(string id, string workitemid, string type, string itemid);
        Task<string> UpdateQuantity(string id, string workitemid, string type, string itemid, double quantity);
        Task<string> UpdateQuantityHours(string id, string workitemid, string type, string itemid, double quantity, double hours);
        Task<string> DeleteWorkItem(string id, string workitemid);
        


        //TRANSFER TO REPORT INTERFACE
        Task<List<WorkItemModel>> GetReportProject(string projectid);
        Task<List<MaterialsModel>> GetMaterials(string projectid, string workitemid);
        Task<List<EquipmentModel>> GetEquipment(string projectid, string workitemid);
        Task<List<LaborModel>> GetLabor(string projectid, string workitemid);


        #endregion

        #region PR

        Task<List<PRModel>> GetPRs(string projectid);
        Task<PRModel> GetPR(string id);
        Task<List<ProjectModel>> GetProjectname();
        Task<string> SavePR(PRModel pr);
        Task<List<MaterialsModel>> GetMaterialsQuantity(string projectid, string itemid, string workitemid);
        Task<string> SavePRItem(string id, List<BalanceMaterialModel> items, List<PRItemModel> existingitms);
        Task<string> RemovePRItem(string id, string pritemid);
        Task<string> UpdatePRItem(string id, string pritemid, double quantity);
        Task<TotalProcuredModel> GetTotalProcured(string projectid, string itemid);
        Task<string> DeletePR(string id);
        Task<string> SubmitPR(string id);
        Task<List<BalanceMaterialModel>> GetBalanceMaterials(string projectid, string workitemid);

        Task<string> SavePRItemAdmin(string id, List<ItemModel> items, List<PRItemModel> existingitms);
        
        #endregion

        #region PO

        Task<List<SupplierModel>> GetSuppliers();
        Task<List<POModel>> GetPOs(string projectid);
        Task<string> SavePO(POModel po, string prid, string isempty);
        Task<POModel> GetPO(string poid, string prid);
        Task<List<PRItemModel>> GetPRItems(string prid);
        Task<string> SavePOItems(string poid, string prid, List<POItemModel> items, List<POItemModel> existing);
        Task<string> DeletePOItem(string prid, string poid, string id);
        Task<string> UpdatePOItem(string prid, string poid, POItemModel poitem);
        Task<string> SubmitPO(string prid, string poid);
        Task<string> DeletePO(string prid, string poid);
        Task<string> EditPO(POModel po);

        #endregion

        #region VOUCHER

        Task<List<POModel>> GetPOSDetails(string projectid);
        Task<List<POModel>> GetCVs(string projectid);
        Task<POModel> GetCV(string poid);
        Task<string> SaveCV(POModel cv);
        Task<string> DeleteCV(POModel cv);
        Task<string> SubmitCV(string prid, string poid, int action);

        #endregion

        #region USER

        Task<string> SaveUser(UserModel user);
        Task<List<UserModel>> GetUsers();
        Task<List<RoleModel>> GetRoles();
        
        #endregion
        
        #region Menu

        Task<List<MenuModel>> GetMenus();
        Task<string> SaveMneu(MenuModel menu);
        Task<List<MenuModel>> GetParentMenus();
        #endregion
        
        Task<LoginModel> CheckUserCredential(string Username, string Password);
    }
}