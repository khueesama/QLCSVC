using System.Collections.Generic;
using QLCSVCWinApp.DataAccess;
using QLCSVCWinApp.DataAccess.QLCSVCWinApp.Models;
using QLCSVCWinApp.Models;

namespace QLCSVCWinApp.Services
{
    public class PhongHocService
    {
        private readonly PhongHocDAO _daoTB = new PhongHocDAO();
        private readonly ThietBiDAO _daoTh = new ThietBiDAO();

        public List<PhongHoc> GetAll() => _daoTB.GetAll();

        public string AddAndGetId(PhongHoc p) => _daoTB.AddAndGetId(p);
        public bool Add(PhongHoc p) => _daoTB.Add(p);
        public bool Update(PhongHoc p) => _daoTB.Update(p);
        public bool Delete(string maPhong) => _daoTB.Delete(maPhong);

        // Đếm thiết bị trong phòng
        public int CountDevicesInRoom(string maPhong)
            => _daoTh.CountByRoom(maPhong);
    }

}
